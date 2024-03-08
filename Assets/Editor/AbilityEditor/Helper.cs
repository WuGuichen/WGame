using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace WGame.Ability.Editor
{
    public static partial class Helper
    {
        public static readonly string Letter = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        
        public static string GetRootDirectory()
        {
            string[] directories = Directory.GetDirectories("Assets/Editor", "AbilityEditor", SearchOption.AllDirectories);
            return directories.Length > 0 ? directories[0] : string.Empty;
        }

        public static EventModifiers actionModifier
        {
            get
            {
                if (Application.platform == RuntimePlatform.OSXEditor ||
                    Application.platform == RuntimePlatform.OSXPlayer)
                    return EventModifiers.Command;

                return EventModifiers.Control;
            }
        }

        public static bool CanClearSelection(Event evt)
        {
            return !evt.control && !evt.command && !evt.shift;
        }

        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        private static readonly int maxDecimals = 15;
        public static int GetNumberOfDecimalsForMinimumDifference(float minDifference)
        {
            return Mathf.Clamp(-Mathf.FloorToInt(Mathf.Log10(Mathf.Abs(minDifference))), 0, maxDecimals);
        }

        public static void CreateDirectory(string path, bool del = false)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                if (del)
                {
                    Directory.Delete(path, true);
                    Directory.CreateDirectory(path);
                }
            }
        }

        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        public static void MoveFile(string srcPath, string destPath)
        {
            if (srcPath == destPath)
                return;

            if (!File.Exists(srcPath))
                return;

            DeleteFile(destPath);

            CreateDirectory(Path.GetDirectoryName(destPath));
            File.Move(srcPath, destPath);
        }

        public static object GetProperty(object obj, string propertyName)
        {
            return obj.GetType().InvokeMember(propertyName, BindingFlags.GetProperty, null, obj, null);
        }

        public static void SetProperty(object obj, string propertyName, object newValue)
        {
            obj.GetType().InvokeMember(propertyName, BindingFlags.SetProperty, null, obj, new object[] { newValue });
        }

        public static int GetStringIndex(List<string> list, string val)
        {
            int idx = -1;

            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i] == val)
                {
                    idx = i;
                    break;
                }
            }

            return idx;
        }
        
        public static string RandomString(int n)
        {
            StringBuilder sb = new StringBuilder();
            for (int i=0; i<n; ++i)
            {
                int index = UnityEngine.Random.Range(0, Letter.Length);
                sb.Append(Letter[index]);
            }

            return sb.ToString();
        }

        public static string NonceStr(int n = 32)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0:d19}{1}", System.DateTime.Now.Ticks, RandomString(n - 19));

            return sb.ToString();
        }

        public static Dictionary<int, string> GetTypeLabelList<T>()
        {
            var res = new Dictionary<int, string>();
            var type = typeof(T);
            var properties = type.GetProperties();
            for (var i = 0; i < properties.Length; i++)
            {
                var attrs = properties[i].GetCustomAttributes(type, false);
                if (attrs.Length == 1)
                {
                    var labelAttr = (WLableAttribute)attrs[0];
                }
            }

            return res;
        }
    }
}