// using System.Collections;
// using System.Collections.Generic;
// using Antlr4.Runtime;
// using Antlr4.Runtime.Tree;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;
//
// public class ArrayInitTest
// {
//     // A Test behaves as an ordinary method
//     [Test]
//     public void ArrayInitTestSimplePasses()
//     {
//         // Use the Assert class to test conditions
//         string input = @"{1, 3, 2, 12, 134}";
//         var stream = new AntlrInputStream(input);
//         var lexer = new ArrayInitLexer(stream);
//         var tokens = new CommonTokenStream(lexer);
//         var parser = new ArrayInitParser(tokens);
//         var tree = parser.init();
//
//         ParseTreeWalker walker = new ParseTreeWalker();
//         walker.Walk(new ShortToUnicodeString(), tree);
//         WLogger.Print("End");
//     }
//
//     // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
//     // `yield return null;` to skip a frame.
//     [UnityTest]
//     public IEnumerator ArrayInitTestWithEnumeratorPasses()
//     {
//         // Use the Assert class to test conditions.
//         // Use yield to skip a frame.
//         yield return null;
//     }
//
//     class ShortToUnicodeString : ArrayInitBaseListener
//     {
//         public override void EnterInit(ArrayInitParser.InitContext context)
//         {
//             WLogger.AppendBuffer("\"");
//         }
//
//         public override void ExitInit(ArrayInitParser.InitContext context)
//         {
//             WLogger.AppendBuffer("\"");
//             WLogger.PrintBuffer();
//             WLogger.ClearBuffer();
//         }
//
//         public override void EnterValue(ArrayInitParser.ValueContext context)
//         {
//             int value = int.Parse(context.INT().GetText());
//             WLogger.AppendBuffer(string.Format("\\u{0:X4}", value));
//         }
//     }
// }
