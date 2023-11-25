// using System.Collections;
// using System.Collections.Generic;
// using Antlr4.Runtime;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;
//
// public class ExprTest
// {
//     // A Test behaves as an ordinary method
//     [Test]
//     public void ExprTestSimplePasses()
//     {
//         // Use the Assert class to test conditions
//         string input = @"a = 1\nb = 2\n1 + a + b\n";
//         var stream = new AntlrInputStream(input);
//         var lexer = new ExprLexer(stream);
//         var tokens = new CommonTokenStream(lexer);
//         var parser = new ExprParser(tokens);
//         var tree = parser.prog();
//         WLogger.Print(tree.ToStringTree());
//     }
//
//     // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
//     // `yield return null;` to skip a frame.
//     [UnityTest]
//     public IEnumerator ExprTestWithEnumeratorPasses()
//     {
//         // Use the Assert class to test conditions.
//         // Use yield to skip a frame.
//         yield return null;
//     }
// }
