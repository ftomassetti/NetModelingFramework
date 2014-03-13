/*
 * Net Modeling Framework - http://github.com/ftomassetti/NetModelingFramework
 * 
 * Federico Tomassetti, 2014.
 */
using System;
using System.Linq;
using NetModelingFramework.Metamodel;
using NUnit.Framework;
using System.Collections.Generic;
using NetModelingFramework.Model;
using NetModelingFramework.Model.Markers;
using DotLiquid;

namespace NetModelingFramework.test
{
	[TestFixture]
	public class TestMetaclassDerivation
	{
		// I define a few metaclasses for my test
		
		abstract class WithMultiRef : MObject {
			[MMReference(Containment=true)]
			public List<Expr> Exprs { get; set; }
		}
		
		abstract class Expr : MObject {
			
		}
		
		abstract class BinaryExpr : Expr {
			[MMReference(Containment=true)]
			public Expr Left { get; set; }
			
			[MMReference(Containment=true)]
			public Expr Right { get; set; }
		}
		
		class Addition : BinaryExpr {
		}
		
		class IntegerLiteral : Expr {
			[MMAttribute()]
			public int Value { get; set; }
		}
		
		enum WaltDisneyCharacters {
			Pippo,
			Paperino,
			Pluto
		}
		
		class MyClassWithEnum : MObject {
			[MMAttribute()]
			public WaltDisneyCharacters Hero { get; set; }
		}
		
		[Test]
		public void TestName()
		{
			Assert.AreEqual("Expr",           MObject.GetMetaClass(typeof(Expr)).Name);
			Assert.AreEqual("BinaryExpr",     MObject.GetMetaClass(typeof(BinaryExpr)).Name);
			Assert.AreEqual("Addition",       MObject.GetMetaClass(typeof(Addition)).Name);
			Assert.AreEqual("IntegerLiteral", MObject.GetMetaClass(typeof(IntegerLiteral)).Name);
		}
		
		[Test]
		public void TestSuperClass()
		{			
			Assert.IsNotNull(MObject.GetMetaClass(typeof(BinaryExpr)).SuperMetaclass);		
			Assert.AreEqual("Expr",MObject.GetMetaClass(typeof(BinaryExpr)).SuperMetaclass.Name);
			
			Assert.IsNotNull(MObject.GetMetaClass(typeof(Addition)).SuperMetaclass);		
			Assert.AreEqual("BinaryExpr",MObject.GetMetaClass(typeof(Addition)).SuperMetaclass.Name);
		}
		
		[Test]
		public void TestAbstract()
		{
			Assert.AreEqual(true,MObject.GetMetaClass(typeof(Expr)).Abstract);
			Assert.AreEqual(true,MObject.GetMetaClass(typeof(BinaryExpr)).Abstract);
			Assert.AreEqual(false,MObject.GetMetaClass(typeof(Addition)).Abstract);
			Assert.AreEqual(false,MObject.GetMetaClass(typeof(IntegerLiteral)).Abstract);			
		}

		[Test]
		public void TestReferencesDeclared()
		{
			Assert.AreEqual(0,MObject.GetMetaClass(typeof(Expr)).References.Count);
			Assert.AreEqual(2,MObject.GetMetaClass(typeof(BinaryExpr)).References.Count);
			Assert.AreEqual(0,MObject.GetMetaClass(typeof(Addition)).References.Count);
			Assert.AreEqual(0,MObject.GetMetaClass(typeof(IntegerLiteral)).References.Count);
			Assert.AreEqual(new String[]{"Left","Right"},MObject.GetMetaClass(typeof(BinaryExpr)).References.Select(r => r.Name).ToArray());
		}
		
		[Test]
		public void TestReferencesInherited()
		{
			Assert.AreEqual(0,MObject.GetMetaClass(typeof(Expr)).GetAllReferences().Count);
			Assert.AreEqual(2,MObject.GetMetaClass(typeof(BinaryExpr)).GetAllReferences().Count);
			Assert.AreEqual(2,MObject.GetMetaClass(typeof(Addition)).GetAllReferences().Count);
			Assert.AreEqual(0,MObject.GetMetaClass(typeof(IntegerLiteral)).GetAllReferences().Count);
			Assert.AreEqual(new String[]{"Left","Right"},MObject.GetMetaClass(typeof(BinaryExpr)).GetAllReferences().Select(r => r.Name).ToArray());
			Assert.AreEqual(new String[]{"Left","Right"},MObject.GetMetaClass(typeof(Addition)).GetAllReferences().Select(r => r.Name).ToArray());
		}
		
		[Test]
		public void TestSimpleToHash()
		{
			IntegerLiteral num1 = new IntegerLiteral();
			num1.Value = 1;
			IntegerLiteral num2 = new IntegerLiteral();
			num2.Value = 2;
			Addition addition = new Addition();
			addition.Left = num1;
			addition.Right = num2;
			
			Hash hash = addition.ToHash();
			Assert.AreEqual(1,hash.Get<Hash>("Left").Get<int>("Value"));
			Assert.AreEqual(2,hash.Get<Hash>("Right").Get<int>("Value"));
		}
		
		[Test]
		public void TestToHashRecursive()
		{
			IntegerLiteral num1 = new IntegerLiteral();
			num1.Value = 1;
			IntegerLiteral num2 = new IntegerLiteral();
			num2.Value = 2;
			IntegerLiteral num3 = new IntegerLiteral();
			num3.Value = 3;
			
			Addition subAddition = new Addition();
			subAddition.Left = num2;
			subAddition.Right = num3;
			
			Addition addition = new Addition();
			addition.Left = num1;
			addition.Right = subAddition;
			
			Hash hash = addition.ToHash();
			Assert.AreEqual(1,hash.Get<Hash>("Left").Get<int>("Value"));
			Assert.AreEqual(2,hash.Get<Hash>("Right").Get<Hash>("Left").Get<int>("Value"));
			Assert.AreEqual(3,hash.Get<Hash>("Right").Get<Hash>("Right").Get<int>("Value"));
		}
		
		[Test]
		public void TestRecognizeMultiReferences()
		{
			MMetaclass metaclass = MObject.GetMetaClass(typeof(WithMultiRef));
			Assert.AreEqual(1,metaclass.GetAllReferences().Count);
			Assert.AreEqual("Exprs",metaclass.GetAllReferences()[0].Name);
			Assert.AreEqual(true,metaclass.GetAllReferences()[0].Multi);
		}
		
		[Test]
		public void TestEnumToHash()
		{
			MAttribute myAttribute = new MAttribute();
			myAttribute.Name = "Address";
			myAttribute.Type = SimpleType.String;
			var hash = myAttribute.ToHash();
			Assert.AreEqual("String",hash.Get<String>("Type"));
		}
		
		public class MySpecializer : GroupOfToHashSpecializer {
			public bool CanSpecialize(Enum val)
			{
				if (val is WaltDisneyCharacters){
					return true;
				}
				return false;
			}
			
			public Object SpecializedValue(Enum val)
			{
				if (val.Equals(WaltDisneyCharacters.Pippo)){
					return "Topolinia";
				} else {
					throw new Exception("Unknown val "+val);
				}
			}
		}
		
		[Test]
		public void TestEnumToHashWithSpecializer()
		{
			// In my hash I want to have "Topolinia" instead of "Pippo",
			// therefore I use MySpecializer
			MyClassWithEnum obj = new MyClassWithEnum();
			obj.Hero = WaltDisneyCharacters.Pippo;
			var hash = obj.ToHash(new MySpecializer());
			Assert.AreEqual("Topolinia",hash.Get<String>("Hero"));
		}
	}
}
