using System;
using System.Text;
using WGame.Runtime;

public static class WTerminal
{
	public static bool isInTerminal = false;
	private static StringBuilder msgBuffer = new StringBuilder();
	private static string lastMessage = String.Empty;
	public static string LastMessage => lastMessage;

	public static string Message
	{
		get => msgBuffer.ToString();
	}

	public static void Input(string message)
	{
		msgBuffer.Append("<< ");
		msgBuffer.AppendLine(message);
		EventCenter.Trigger(EventDefine.OnTerminalMessageUpdate);
	}

	public static void Output(string message)
	{
		msgBuffer.Append(">> ");
		msgBuffer.AppendLine(message);
		lastMessage = message;
		EventCenter.Trigger(EventDefine.OnTerminalMessageUpdate);
	}

	public static void Clear()
	{
		msgBuffer.Clear();
		EventCenter.Trigger(EventDefine.OnTerminalMessageUpdate);
	}
}
