// using System.Collections;
// using System.Collections.Generic;
// using Antlr4.Runtime;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;
//
// public class LabeledExprTest
// {
//     // A Test behaves as an ordinary method
//     [Test]
//     public void TabeledExprTestSimplePasses()
//     {
//         // Use the Assert class to test conditions
//         string input = "1 + 2\na = 3\nb=4\n2 * a * 4\n2 * b - 3 * 4 / ( 5 + 1)\n";
//         var stream = new AntlrInputStream(input);
//         var lexer = new LabeledExprLexer(stream);
//         var tokens = new CommonTokenStream(lexer);
//         var parser = new LabeledExprParser(tokens);
//         var tree = parser.prog();
//         var visitor = new EvalVisitor();
//         visitor.Visit(tree);
//     }
//
//     // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
//     // `yield return null;` to skip a frame.
//     [UnityTest]
//     public IEnumerator TabeledExprTestWithEnumeratorPasses()
//     {
//         // Use the Assert class to test conditions.
//         // Use yield to skip a frame.
//         yield return null;
//     }
//
//     class EvalVisitor : LabeledExprBaseVisitor<int>
//     {
//         private Dictionary<string, int> memory = new Dictionary<string, int>();
//
//         public override int VisitAssign(LabeledExprParser.AssignContext context)
//         {
//             var id = context.ID().GetText();
//             int value = Visit(context.expr());
//             memory.Add(id, value);
//             return value;
//         }
//
//         public override int VisitPrintExpr(LabeledExprParser.PrintExprContext context)
//         {
//             int value = Visit(context.expr());
//             WLogger.Print(value);
//             return 0;
//         }
//
//         public override int VisitInt(LabeledExprParser.IntContext context)
//         {
//             return int.Parse(context.INT().GetText());
//         }
//
//         public override int VisitId(LabeledExprParser.IdContext context)
//         {
//             string id = context.ID().GetText();
//             if (memory.TryGetValue(id, out int value))
//                 return value;
//             return 0;
//         }
//
//         public override int VisitMulDiv(LabeledExprParser.MulDivContext context)
//         {
//             int left = Visit(context.expr(0));
//             int right = Visit(context.expr(1));
//             if (context.op.Type == LabeledExprParser.MUL)
//                 return left * right;
//             return left / right;
//         }
//
//         public override int VisitAddSub(LabeledExprParser.AddSubContext context)
//         {
//             int left = Visit(context.expr(0));
//             int right = Visit(context.expr(1));
//             if (context.op.Type == LabeledExprParser.ADD)
//                 return left + right;
//             return left - right;
//         }
//
//         public override int VisitParens(LabeledExprParser.ParensContext context)
//         {
//             return Visit(context.expr());
//         }
//     }
// }
