// 数据缓存，数据逻辑操作

using System.Text;
using Antlr4.Runtime;
using WGame.Runtime;

namespace WGame.UI
{
	public class TerminalModel : Runtime.Singleton<TerminalModel>
	{
		private Interpreter interpreter;
		public Interpreter Interp => interpreter;

		public TerminalModel()
		{
			interpreter = new Interpreter(new BaseDefinition(SharedDefinition.Inst), SharedScope.Inst);
		}
		
		public void DoString(string str)
		{
			var stream = new AntlrInputStream(str);
			if (!WLangMgr.Inst.GetFileContext(stream, out var fileContext, "DoString"))
				return;
			interpreter.ReVisitFile(fileContext);
		}

		public void DoParserTree(WLangParser.FileContext tree)
		{
			interpreter.ReVisitFile(tree);
		}
	}
}
