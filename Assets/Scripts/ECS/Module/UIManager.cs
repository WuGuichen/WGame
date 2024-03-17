using System.Collections.Generic;
using FairyGUI.Dynamic;
using FairyGUI;
using UnityEngine;

namespace WGame.UI
{
    public class UIManager : MonoBehaviour, IUIAssetManagerConfiguration
    {
        private static UIManager _inst;
        public static UIManager Inst => _inst;
        public IUIPackageHelper PackageHelper { get; private set; }
        public IUIAssetLoader AssetLoader { get; private set; }
        public bool UnloadUnusedUIPackageImmediately => unloadUnusedUIPackageImmediately;

        private IUIAssetManager m_UIAssetManager;
        [SerializeField] private UIPackageMapping m_PackageMapping;

        [Header("是否立即卸载未使用的UIPackage")] public bool unloadUnusedUIPackageImmediately;

        private bool m_isQuiting;

        // 已初始化的
        private Dictionary<string, BaseView> initedViewList = new Dictionary<string, BaseView>();

        // 隐藏状态的
        private List<string> hiddedViewList = new List<string>();
        // 打开的（可以是隐藏的）
        private Dictionary<string, BaseView> openedViewList = new Dictionary<string, BaseView>();

        private void Awake()
        {
            _inst = this;
            AssetLoader = new YooassetUIAssetLoader();
            PackageHelper = m_PackageMapping;
		    GRoot.inst.SetContentScaleFactor(1920, 1080, UIContentScaler.ScreenMatchMode.MatchWidthOrHeight);
		    FUIBinder.BindAll();

            m_UIAssetManager = new UIAssetManager();
            m_UIAssetManager.Initialize(this);
            
            ShaderConfig.imageShader = "FairyGUI/Image";
            ShaderConfig.textShader = "FairyGUI/Text";
            ShaderConfig.bmFontShader = "FairyGUI/BMFont";
            ShaderConfig.TMPFontShader = "UI/Default";
        }

        private void OnDestroy()
        {
            if (m_isQuiting)
                return;

            m_UIAssetManager.Dispose();
        }

        private void OnApplicationQuit()
        {
            m_isQuiting = true;
        }

        public static void OpenView(string viewName, BaseView parentView = null)
        {
            if (Inst.openedViewList.ContainsKey(viewName))
                return;
            if (Inst.initedViewList.TryGetValue(viewName, out BaseView view))
            {
                view.Show();
            }
            else if (VDB.viewList.TryGetValue(viewName, out var t))
            {
                view = System.Activator.CreateInstance(t) as BaseView;
                if (view != null)
                {
                    Inst.initedViewList[view.ViewName] = view;
                    if (view.isShowing == false)
                        view.Show();
                }
            }

            view.parentView = parentView;
            Inst.openedViewList.Add(viewName, view);
        }

        public static BaseView GetView(string viewName)
        {
            if (Inst.initedViewList.TryGetValue(viewName, out var view))
            {
                return view;
            }

            return null;
        }

        public static void CloseView(string viewName, bool destory = false)
        {
            if (!Inst.openedViewList.ContainsKey(viewName))
                return;
            if (Inst.initedViewList.TryGetValue(viewName, out BaseView view))
            {
                if(view.isShowing)
                    view.Hide();
                if (destory)
                {
                    view.Dispose();
                    view = null;
                    Inst.initedViewList.Remove(viewName);
                }

            }

            Inst.openedViewList.Remove(viewName);
        }

        /// <summary>
        /// 设置view的可见性
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public static bool HideView(string viewName)
        {
            if (Inst.hiddedViewList.Contains(viewName))
                return false;
            if (!Inst.openedViewList.ContainsKey(viewName))
                return false;
            var view = Inst.openedViewList[viewName];
            view.IsVisible = false;
            view.touchable = false;
            // var go = view.displayObject.gameObject;
            // if (go.activeInHierarchy
            //     go.SetActive(false);
            Inst.hiddedViewList.Add(viewName);
            return true;
        }

        public static bool ShowView(string viewName)
        {
            if (!Inst.hiddedViewList.Contains(viewName))
                return false;
            if (!Inst.openedViewList.ContainsKey(viewName))
            {
                return false;
            }
            
            var view = Inst.openedViewList[viewName];
            // view.displayObject.gameObject.SetActive(true);
            view.IsVisible = true;
            view.touchable = true;
            Inst.hiddedViewList.Remove(viewName);
            return true;
        }

        public static bool IsViewOpen(string viewName)
        {
            if (Inst.openedViewList.ContainsKey(viewName))
                return true;
            return false;
        }
    }
}