using UnityEngine;

public class UnityDebugLogService : ILogService
{
    public void LogMessage(string message)
    {
        Debug.Log(message);
    }
}
