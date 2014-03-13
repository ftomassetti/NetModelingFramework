/*
 * Net Modeling Framework - http://github.com/ftomassetti/NetModelingFramework
 * 
 * Federico Tomassetti, 2014.
 */
using System;
using NUnit.Framework;
using Antlr4.Runtime;
using AntlrToMetamodel;

namespace AntlrToMetamodel.test
{
	[TestFixture]
	public class AntlrContextNodeToMNodesConverterTest
	{
		
		private ExprParser.RootContext ParseAsContextNode(string code)
		{
			AntlrInputStream inputStream = new AntlrInputStream(code);
            var lexer = new ExprLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            var parser = new ExprParser(commonTokenStream);
            return parser.root();
		}
		
		[Test]
		public void TestSimpleExprConversion()
		{
			ExprParser.RootContext ctxNode = ParseAsContextNode("3+2");
			AntlrContextNodeToMNodesConverter converter = new AntlrContextNodeToMNodesConverter();
			GeneratedModelClasses.Root root = (GeneratedModelClasses.Root)converter.Convert(ctxNode,"GeneratedModelClasses");
			Console.WriteLine("ROOT CLASS "+root.Value);
			Assert.IsNotNull(root.Value);
			Assert.True(root.Value is GeneratedModelClasses.AddExpr);
			GeneratedModelClasses.AddExpr addition = (GeneratedModelClasses.AddExpr)root.Value;
			Assert.IsNotNull(addition.Left);
			Assert.IsNotNull(addition.Right);
			Assert.IsNotNull(addition.Operator);
			Assert.AreEqual("+",addition.Operator);
			Assert.True(addition.Left is GeneratedModelClasses.IntExpr);
			Assert.True(addition.Right is GeneratedModelClasses.IntExpr);
			GeneratedModelClasses.IntExpr n3 = (GeneratedModelClasses.IntExpr)addition.Left;
			GeneratedModelClasses.IntExpr n2 = (GeneratedModelClasses.IntExpr)addition.Right;
			Assert.AreEqual("3",n3.Value);
			Assert.AreEqual("2",n2.Value);
		}
	}
}
