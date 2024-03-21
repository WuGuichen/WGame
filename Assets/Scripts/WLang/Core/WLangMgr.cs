using System.Collections.Generic;
#if UNITY_EDITOR
using System.IO;
#endif
using System.Text;
using Antlr4.Runtime;
using UnityEngine;
using WGame.Res;
using WGame.Runtime;

public class WLangMgr : Singleton<WLangMgr>
{
    private Dictionary<int, List<WLangParser.FileContext>> eventFiles;
    private Dictionary<string, List<WLangParser.FileContext>> codeFiles;
    private Dictionary<string, WLangParser.FileContext> codeCache;
    private Dictionary<string, WLangParser.BTreeBuilderContext> bTreeCodes = new();
    private Dictionary<string, WLangParser.FsmBuilderContext> fsmCodes = new();
    private int loadCount;

    private IAssetService assetService;

    private Dictionary<int, string> eventId2Name;
    private Dictionary<string, int> eventName2Id;
    private Dictionary<int, WEventCallback0> eventCallback0s;
    private Dictionary<string, List<WLangImporter>> codeImporterDict = new();

    private const string WL_GAME_EVENT = "GameEvent_";
    private const string WL_MOTION = "Motion_";
    private const string WL_BTREE = "BTree_";
    private const string WL_FSM = "FSM_";

    public string CURRENT_PATH = "";

    private static object locker = new object();
    // private const string PATH_GAME_EVENT = A
    private Interpreter _interpreter;

    #if UNITY_EDITOR
    private FileSystemWatcher watcherEvent;
    private FileSystemWatcher watcherMotion;
    private FileSystemWatcher watcherBTree;
    private FileSystemWatcher watcherFSM;
    private Dictionary<string, List<string>> codeDict;
    #endif

    public WLangMgr()
    {
        eventFiles = new Dictionary<int, List<WLangParser.FileContext>>();
        codeFiles = new Dictionary<string, List<WLangParser.FileContext>>();
        codeCache = new Dictionary<string, WLangParser.FileContext>();
        assetService = YooassetManager.Inst;
        eventId2Name = new Dictionary<int, string>();
        eventName2Id = new Dictionary<string, int>();
        eventCallback0s = new Dictionary<int, WEventCallback0>();
        var infos = typeof(EventDefine).GetFields();
        for (int i = 0; i < infos.Length; i++)
        {
            var info = infos[i];
            int value = (int)info.GetRawConstantValue();
            string name = info.Name;
            eventId2Name[value] = name;
            eventName2Id[name] = value;
        }

#if UNITY_EDITOR
        watcherEvent = GetWatcher(Application.dataPath + "/Scripts/WLang/Code/GameEvent/", (sender, args) =>
        {
            var name = args.Name.Split('.')[0];
            // RegisterEvent(WL_GAME_EVENT + name, _interpreter);
            // WLogger.Print("刷新成功：" +name);
            var list = codeDict[WL_GAME_EVENT];
            if(list.Contains(name) == false)
                list.Add(name);
            codeDict[WL_GAME_EVENT] = list;
        });
        
        watcherMotion = GetWatcher(Application.dataPath + "/Scripts/WLang/Code/Motion/", (sender, args) =>
        {
            var name = args.Name.Split('.')[0];
            // LoadCode(WL_MOTION + name, _interpreter, true);
            // WLogger.Print("刷新成功：" +name);
            var list = codeDict[WL_MOTION];
            if(list.Contains(name) == false)
                list.Add(name);
            codeDict[WL_MOTION] = list;
        });
        
        watcherBTree = GetWatcher(Application.dataPath + "/Scripts/WLang/Code/BTree/", (sender, args) =>
        {
            var name = args.Name.Split('.')[0];
            InitBehaviorTree(WL_BTREE + name, _interpreter);
            // WLogger.Print("刷新成功：" + name);
            // UnityEditor.EditorApplication.delayCall += () =>
            // {
            //     EventCenter.Trigger(EventDefine.OnBTreeHotUpdate, WEventContext.Get(name));
            // };
            var list = codeDict[WL_BTREE];
            if(list.Contains(name) == false)
                list.Add(name);
            codeDict[WL_BTREE] = list;
        });
        
        watcherFSM = GetWatcher(Application.dataPath + "/Scripts/WLang/Code/FSM/", (sender, args) =>
        {
            var name = args.Name.Split('.')[0];
            InitFSM(WL_FSM + name, _interpreter);
            // WLogger.Print("刷新成功：" + name);
            // UnityEditor.EditorApplication.delayCall += () =>
            // {
            //     WBTreeMgr.Inst.RefreshFSM(name);
            // };
            var list = codeDict[WL_FSM];
            if(list.Contains(name) == false)
                list.Add(name);
            codeDict[WL_FSM] = list;
        });
        codeDict = new Dictionary<string, List<string>>()
        {
            {WL_FSM, new List<string>()},
            {WL_MOTION, new List<string>()},
            {WL_BTREE, new List<string>()},
            {WL_GAME_EVENT, new List<string>()},
        };
#endif
    }

#if UNITY_EDITOR
    private FileSystemWatcher GetWatcher(string dir, FileSystemEventHandler handler)
    {
        var watcher = new FileSystemWatcher();
        watcher.Path = dir;
        watcher.Filter = "*.wl";
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        watcher.Changed += handler;
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;
        return watcher;
    }

    public void HotUpdate()
    {
        foreach (var kv in codeDict)
        {
            var list = codeDict[kv.Key];
            switch (kv.Key)
            {
                case WL_MOTION:
                    foreach (var name in list)
                    {
                        LoadCode(WL_MOTION + name, _interpreter, true);
                    }
                    break;
                case WL_FSM:
                    foreach (var name in list)
                    {
                        UnityEditor.EditorApplication.delayCall += () => { WBTreeMgr.Inst.RefreshFSM(name); };
                    }
                    break;
                case WL_BTREE:
                    foreach (var name in list)
                    {
                        UnityEditor.EditorApplication.delayCall += () =>
                        {
                            EventCenter.Trigger(EventDefine.OnBTreeHotUpdate, name);
                            WLogger.Print("重载BTree成功：" + name);
                        };
                    }
                    break;
                case WL_GAME_EVENT:
                    foreach (var name in list)
                    {
                        RegisterEvent(WL_GAME_EVENT + name, _interpreter);
                        WLogger.Print("重载成功：" + name);
                    }
                    break;
                default:
                    break;
            }
            
            codeDict[kv.Key].Clear();
        }
    }
#endif
    

    public void LordInitCode(Interpreter interpreter)
    {
        _interpreter = interpreter;
        LoadInitFile(new string[] {WL_GAME_EVENT, WL_MOTION, WL_BTREE, WL_FSM}, interpreter);
    }

    public bool GetFileContext(ICharStream stream, out WLangParser.FileContext file, string path)
    {
        var lexer = new WLangLexer(stream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new WLangParser(tokens);
#if UNITY_EDITOR
        parser.AddErrorListener(new WLangErrorListener());
#endif
        file = parser.file();
        if (parser.NumberOfSyntaxErrors > 0)
        {
            WLogger.AppendBuffer(" from:" + path);
            WLogger.PrintErrorBuffer();
            WLogger.ClearBuffer();
            return false;
        }
        return true;
    }
    
    public bool GetFSMContext(ICharStream stream, out WLangParser.FsmBuilderContext fsm, string path)
    {
        var lexer = new WLangLexer(stream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new WLangParser(tokens);
#if UNITY_EDITOR
        parser.AddErrorListener(new WLangErrorListener());
#endif
        fsm = parser.fsmBuilder();
        if (parser.NumberOfSyntaxErrors > 0)
        {
            WLogger.AppendBuffer(" from:" + path);
            WLogger.PrintErrorBuffer();
            WLogger.ClearBuffer();
            return false;
        }
        return true;
    }
    
    public bool GetBTreeContext(ICharStream stream, out WLangParser.BTreeBuilderContext btree, string path)
    {
        var lexer = new WLangLexer(stream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new WLangParser(tokens);
#if UNITY_EDITOR
        parser.AddErrorListener(new WLangErrorListener());
#endif
        btree = parser.bTreeBuilder();
        if (parser.NumberOfSyntaxErrors > 0)
        {
            WLogger.AppendBuffer(" from:" + path);
            WLogger.PrintErrorBuffer();
            WLogger.ClearBuffer();
            return false;
        }
        return true;
    }

    private void LoadInitFile(string[] basePaths, Interpreter interpreter)
    {
        loadCount = basePaths.Length;
        foreach (var basePath in basePaths)
        {
            var fullPath = basePath + "_Init";
            assetService.LoadRawFileASync(fullPath, asset =>
            {
                var allChars = asset.ToCharArray();
                var stream = new AntlrInputStream(allChars, asset.Length);

                if (GetFileContext(stream, out var fileContext, fullPath) == false)
                {
                    return;
                }
                
                var title = ParseTitle(ref allChars);
                lock (locker)
                {
                    CURRENT_PATH = title + "_";
                    interpreter.ReVisitFile(fileContext);
                    CURRENT_PATH = "";
                }
                OnLoadComplete();
            });
        }
    }

    private void OnLoadComplete()
    {
        loadCount--;
        if (loadCount <= 0)
        {
            EventCenter.Trigger(EventDefine.OnGameCodeInitted);
        }
    }

    private void InitFSM(string filePath, Interpreter interpreter)
    {
        assetService.LoadRawFileSync(filePath, asset =>
        {
            var chars = asset.ToCharArray();
            var stream = new AntlrInputStream(chars, asset.Length);
            if (!GetFSMContext(stream, out var fsmBuilderContext, filePath))
            {
                return;
            }
            var title = ParseTitle(ref chars);
            fsmCodes[title] = fsmBuilderContext;
        });
    }

    private void InitBehaviorTree(string filePath, Interpreter interpreter)
    {
        assetService.LoadRawFileSync(filePath, asset =>
        {
            var chars = asset.ToCharArray();
            var stream = new AntlrInputStream(chars, asset.Length);
            if (!GetBTreeContext(stream, out var treeContext, filePath))
            {
                return;
            }
            var title = ParseTitle(ref chars);
            bTreeCodes[title] = treeContext;
        });
    }

    public WBTree GenBehaviorTree(string path, Interpreter interpreter)
    {
        if (bTreeCodes.TryGetValue(path, out var builder))
        {
            return interpreter.BuildWBree(builder, path);
        }

        return null;
    }

    public WFSM GenFSM(string path, Interpreter interpreter)
    {
        if (fsmCodes.TryGetValue(path, out var builder))
        {
            return interpreter.BuildWFSM(builder, path);
        }

        return null;
    }

    public void LoadCode(string path, Interpreter interpreter, bool isReload = false)
    {
        var filePath = string.Concat(CURRENT_PATH, path);
        if (!isReload && codeCache.ContainsKey(filePath))
            return;
        if(CURRENT_PATH == WL_GAME_EVENT)
            RegisterEvent(filePath, interpreter);
        else if(CURRENT_PATH == WL_BTREE)
            InitBehaviorTree(filePath, interpreter);
        else if(CURRENT_PATH == WL_FSM)
            InitFSM(filePath, interpreter);
        else
        {
            assetService.LoadRawFileSync(filePath, asset =>
            {
                var chars = asset.ToCharArray();
                var stream = new AntlrInputStream(chars, asset.Length);
                if (!GetFileContext(stream, out var fileContext, path))
                {
                    return;
                }
                var title = ParseTitle(ref chars);
                if (codeFiles.TryGetValue(title, out var codeList))
                {
                    codeList.Remove(codeCache[filePath]);
                    codeList.Add(fileContext);
                }
                else
                {
                    codeFiles[title] = new List<WLangParser.FileContext>() { fileContext };
                }

                codeCache[filePath] = fileContext;

                if (isReload)
                {
                    WLogger.Print("重载Code成功:" + title);
                    if (codeImporterDict.TryGetValue(title, out var list))
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            if(list[i].type == ImporterType.FSM)
                                WBTreeMgr.Inst.RefreshFSM(list[i].name);
                            else if (list[i].type == ImporterType.BTree)
                            {
                                EventCenter.Trigger(EventDefine.OnBTreeHotUpdate, list[i].name);
                                WLogger.Print("重载BTree成功：" + list[i].name);
                            }
                        }
                    }
                }
            });
        }
    }

    public void LinkImportCode(string codeFileName, WLangImporter importer)
    {
        if (codeImporterDict.TryGetValue(codeFileName, out var list))
        {
            if (list.Contains(importer))
                return;
        }
        else
        {
            list = new List<WLangImporter>();
        }
        list.Add(importer);
        codeImporterDict[codeFileName] = list;
    }

    public void CallCode(string name, Interpreter interpreter = null)
    {
        if (interpreter == null)
            interpreter = _interpreter;
        if (codeFiles.TryGetValue(name, out var file))
        {
            file.ForEach((code =>
            {
                interpreter.ReVisitFile(code);
            }));
        }
        else
        {
            WLogger.Warning("未找到相应Motion: " + name + "请检查是否import");
        }
    }

    private string ParseTitle(ref char[] code)
    {
        StringBuilder buf = new StringBuilder();
        bool startName = false;
        for (int i = 0; i < code.Length; i++)
        {
            if (char.IsLetterOrDigit(code[i]) || code[i] == '_')
            {
                startName = true;
                buf.Append(code[i]);
            }
            else
            {
                if (startName)
                    break;
            }
        }
        return buf.ToString();
    }

    public void RegisterEvent(string path, Interpreter interpreter)
    {
        assetService.LoadRawFileSync(path, asset =>
        {
            var chars = asset.ToCharArray();
            var stream = new AntlrInputStream(chars, asset.Length);
            if (!GetFileContext(stream, out var fileContext, path))
            {
                return;
            }
            var title = ParseTitle(ref chars);
            if (eventName2Id.TryGetValue(title, out int id))
            {
                if (!eventFiles.TryGetValue(id, out var contexts))
                {
                    contexts = new List<WLangParser.FileContext>() { fileContext };
                }
                else
                {
                    contexts.Remove(codeCache[path]);
                    contexts.Add(fileContext);
                }

                eventFiles[id] = contexts;

                var ctxList = contexts;
                var inter = interpreter;

                RemoveEvent(id);

                eventCallback0s[id] = () =>
                {
                    ctxList.ForEach(ctx => { inter.ReVisitFile(ctx); });
                };
                EventCenter.AddListener(id, eventCallback0s[id]);
                codeCache[path] = fileContext;
            }
        });
    }

    private void PrintTest()
    {
        WLogger.Print("Test");
    }

    public void RemoveEvent(int id)
    {
        if (eventCallback0s.TryGetValue(id, out var func))
        {
            EventCenter.RemoveListener(id, func);
        }
    }

    public void OnDispose()
    {
#if UNITY_EDITOR
        watcherMotion.Dispose();
        watcherEvent.Dispose();
        watcherBTree.Dispose();
        watcherFSM.Dispose();
#endif
    }
}
