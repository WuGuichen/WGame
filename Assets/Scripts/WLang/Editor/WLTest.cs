using System.Collections;
using Antlr4.Runtime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class WLTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void WLTestSimplePasses()
    {
        //指定文件路径
        string logPath = Application.streamingAssetsPath + @"\Code\test1.wl";

        //在文件中读取出来的信息
        // string input = File.ReadAllText(logPath, Encoding.UTF8);
        var stream = new AntlrFileStream(logPath);
        if (!WLangMgr.Inst.GetFileContext(stream, out var fileContext, "test1"))
            return;
        var visitor = new Interpreter(new BaseDefinition(), new GlobalScope());
        visitor.ReVisitFile(fileContext);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator WLTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
