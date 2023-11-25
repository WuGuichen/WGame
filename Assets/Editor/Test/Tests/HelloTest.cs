// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Antlr4.Runtime;
// using Antlr4.Runtime.Tree;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;
//
// public class HelloTest
// {
//
//     private class HelloListener : HelloBaseListener
//     {
//         public override void EnterProgram(HelloParser.ProgramContext context)
//         {
//             var expr = context.expression();
//         }
//
//         public override void EnterExpression(HelloParser.ExpressionContext context)
//         {
//         }
//     }
//     private class HelloVisitor : HelloBaseVisitor<string>
//     {
//         public override string VisitProgram(HelloParser.ProgramContext context)
//         {
//             context.EnterRule(new HelloListener());
//             context.expression();
//             return "Hello";
//         }
//
//         public override string Visit(IParseTree tree)
//         {
//             return base.Visit(tree);
//         }
//
//         public override string VisitExpression(HelloParser.ExpressionContext context)
//         {
//             switch (context.operate.Type)
//             {
//                 case HelloParser.INT:
//                     WLogger.Info("整数");
//                     return "整数";
//                 default:
//                     return "未定义";
//             }
//         }
//     }
//
//     // A Test behaves as an ordinary method
//     [Test]
//     public void NewTestScriptSimplePasses()
//     {
//         string input = @"1 + (2 - 3) * 4";
//         var stream = new AntlrInputStream(input);
//         var lexer = new HelloLexer(stream);
//         var tokens = new CommonTokenStream(lexer);
//         var parser = new HelloParser(tokens);
//         var tree = parser.program();
//         WLogger.Info(tree.ToStringTree());
//         //
//         // var visitor = new HelloVisitor();
//         // var result = visitor.Visit(tree);
//         //
//         // WLogger.Print(result.ToString());
//     }
//
//     // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
//     // `yield return null;` to skip a frame.
//     [UnityTest]
//     public IEnumerator NewTestScriptWithEnumeratorPasses()
//     {
//         // Use the Assert class to test conditions.
//         // Use yield to skip a frame.
//         yield return null;
//     }
// }
