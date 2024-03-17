using UnityEditor;
using UnityEngine;

public class CmdUtil : MonoBehaviour
{
    public static System.Diagnostics.Process CreateShellExProcess(string cmd, string args, string workingDir = "")
    {
        Debug.Log(workingDir);
        var pStartInfo = new System.Diagnostics.ProcessStartInfo(cmd);
        pStartInfo.Arguments = args;
        pStartInfo.CreateNoWindow = false;
        pStartInfo.UseShellExecute = true;
        pStartInfo.RedirectStandardError = false;
        pStartInfo.RedirectStandardInput = false;
        pStartInfo.RedirectStandardOutput = false;
        if (!string.IsNullOrEmpty(workingDir))
            pStartInfo.WorkingDirectory = workingDir;
        return System.Diagnostics.Process.Start(pStartInfo);
    }
    
    public static void RunBat(string batfile, string args, string workingDir = "")
    {
        var p = CreateShellExProcess(batfile, args, workingDir);
        p.Close();
    }

    [MenuItem("Utils/数据表/导出数据表")]
    public static void BuildTableData()
    {
        RunBat("run_luban_server.bat", "", Application.dataPath.Replace("Assets", "Luban"));
    }
    
    [MenuItem("Utils/HotUpdate/更新到CDN")]
    public static void UpdateToCDN()
    {
        RunBat("UpdateCDN.bat", "", Application.dataPath.Replace("Assets", "Bundles"));
    }

    [MenuItem("Utils/数据表/打开动画配置表")]
    public static void OpenAnimConfig()
    {
        RunBat("animationClips.xlsx", "", Application.dataPath.Replace("Assets", "Luban/Config/Datas"));
    }
}
