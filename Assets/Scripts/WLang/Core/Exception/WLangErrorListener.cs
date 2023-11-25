using System.IO;
using Antlr4.Runtime;

public class WLangErrorListener : BaseErrorListener
{
    public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine,
        string msg, RecognitionException e)
    {
        WLogger.AppendBuffer( " line " + line + ":" + charPositionInLine + " " + msg);
    }
}
