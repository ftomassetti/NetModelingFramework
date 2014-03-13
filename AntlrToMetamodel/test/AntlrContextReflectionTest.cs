/*
 * Net Modeling Framework - http://github.com/ftomassetti/NetModelingFramework
 * 
 * Federico Tomassetti, 2014.
 */
using System;
using NetModelingFramework.Metamodel;
using NUnit.Framework;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace AntlrToMetamodel
{

	/// <summary>
	/// Test sulla trasformazione da classi Antlr (le XxxContext) a oggetti MMetaclass
	/// dai quali poi generiamo le classi che rappresentano i nodi degli AST.
	/// </summary>
	[TestFixture]
	public class AntlrContextReflectionTest
	{
		[Test]
		public void TestNameIsCorrect()
		{
			MMetaclass exprMetaclass = new AntlrContextReflection("DUMMY").ConvertSingleClass(typeof(ExprParser.ExprContext));
			Assert.AreEqual("Expr",exprMetaclass.Name);
		}
		
		[Test]
		public void TestDerivedClass()
		{
			AntlrContextReflection converter = new AntlrContextReflection("DUMMY");
			converter.ConvertSingleClass(typeof(ExprParser.ExprContext)); // so it save Expr, baseclass of MultExpr
			MMetaclass mulMetaclass = converter.ConvertSingleClass(typeof(ExprParser.MultContext));
			Assert.AreEqual("MultExpr",mulMetaclass.Name);
		}
		
		[Test]
		public void TestWithoutFeatures()
		{
			MMetaclass exprMetaclass = new AntlrContextReflection("DUMMY").ConvertSingleClass(typeof(ExprParser.ExprContext));
			Assert.AreEqual(0,exprMetaclass.GetAllAttributes().Count);
			Assert.AreEqual(0,exprMetaclass.GetAllReferences().Count);
		}
		
		[Test]
		public void TestNumberOfFeatures()
		{
			AntlrContextReflection converter = new AntlrContextReflection("DUMMY");
			converter.ConvertSingleClass(typeof(ExprParser.ExprContext)); // so it save Expr, baseclass of MultExpr
			MMetaclass exprMetaclass = converter.ConvertSingleClass(typeof(ExprParser.MultContext));
			Assert.AreEqual(1,exprMetaclass.GetAllAttributes().Count);
			Assert.AreEqual(2,exprMetaclass.GetAllReferences().Count);
		}
		
		[Test]
		public void TestFeatureFromToken()
		{
			AntlrContextReflection converter = new AntlrContextReflection("DUMMY");
			converter.ConvertSingleClass(typeof(ExprParser.ExprContext)); // so it save Expr, baseclass of MultExpr
			MMetaclass mulMetaclass = converter.ConvertSingleClass(typeof(ExprParser.MultContext));
			MAttribute oper = mulMetaclass.GetAllAttributes()[0];
			Assert.AreEqual("Operator",oper.Name);
			Assert.AreEqual(SimpleType.String,oper.Type);
		}
		
		[Test]
		public void TestConvertWholeParser()
		{
			AntlrContextReflection converter = new AntlrContextReflection("DUMMY");
			MMetamodel metamodel = converter.ExtractFromParser(typeof(ExprParser));
			Assert.AreEqual(true,metamodel.HasMetaclass("Expr"));
			Assert.AreEqual(true,metamodel.HasMetaclass("MultExpr"));
			Assert.AreEqual(true,metamodel.HasMetaclass("DivExpr"));
			Assert.AreEqual(true,metamodel.HasMetaclass("MinusExpr"));
			Assert.AreEqual(true,metamodel.HasMetaclass("IntExpr"));
			Assert.AreEqual(true,metamodel.HasMetaclass("ParenExpr"));
			Assert.AreEqual(true,metamodel.HasMetaclass("AddExpr"));
			Assert.AreEqual(true,metamodel.HasMetaclass("SubExpr"));
			Assert.AreEqual(true,metamodel.HasMetaclass("Root"));
			Assert.AreEqual(9,metamodel.Metaclasses.Count);			                                                                                                                
		}
		
	}
}
