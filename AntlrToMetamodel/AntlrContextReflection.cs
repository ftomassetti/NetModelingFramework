/*
 * Net Modeling Framework - http://github.com/ftomassetti/NetModelingFramework
 * 
 * Federico Tomassetti, 2014.
 */
using System;
using NetModelingFramework.Metamodel;
using System.Collections.Generic;

namespace AntlrToMetamodel
{
	/// <summary>
	/// Description of AntrlContextReflection.
	/// </summary>
	public class AntlrContextReflection
	{
		private MMetamodel metamodel;
		
		public AntlrContextReflection(string metamodelName)
		{
			this.metamodel = new MMetamodel();
			this.metamodel.Name = metamodelName;
		}
		
		public MMetamodel ExtractFromParser(Type parserClass){
			// First I create all the metaclasses, so that later I can reference them
			foreach (var t in parserClass.GetNestedTypes()){
				CreateMetaClass(t);            	
            }
			
			foreach (var t in parserClass.GetNestedTypes()){
				MMetaclass metaclass = this.metamodel.GetMetaClassByName(DeriveMetaclassName(t));
				FillMetaClass(metaclass,t);            	
            }
            return this.metamodel;	
		}
		
		private MMetaclass CreateMetaClass(Type contextType)
		{
			MMetaclass metaclass = new MMetaclass();
			metaclass.Name = DeriveMetaclassName(contextType);
			
			this.metamodel.Metaclasses.Add(metaclass);
			return metaclass;
		}
		
		private void FillMetaClass(MMetaclass metaclass, Type contextType)
		{
			Type baseClass = contextType.BaseType;
			if (!(baseClass.Name.Equals("ParserRuleContext"))){
				System.Diagnostics.Debug.WriteLine("NAME <"+baseClass.Name+">");
				metaclass.SuperMetaclass = this.metamodel.GetMetaClassByName(DeriveMetaclassName(contextType.BaseType));	
				System.Diagnostics.Debug.WriteLine(">> "+metaclass.SuperMetaclass.Name);
			}
			
			// Either the class has only one method (in that case we name the field implicitly 'value')
			// or it has fields (which derive from labels)			
			var methods = new List<System.Reflection.MethodInfo>();
	        foreach (var p in contextType.GetMethods()){
				// I check if it is inherithed
	        	if (p.DeclaringType==contextType && p.GetParameters().Length==0 && !p.ReturnType.IsPrimitive){
					methods.Add(p);
	        	}
	        }
			var fields = new List<System.Reflection.FieldInfo>();
	        foreach (var p in contextType.GetFields()){
	        	if (p.DeclaringType==contextType){
					fields.Add(p);
	        	}
	        }			
			if (methods.Count==0){
				// use fields
				FillMetaclassFromFields(metaclass,fields);
			} else if (methods.Count==1 && fields.Count==0){
				// use single method
				FillMetaclassFromMethod(metaclass,methods[0]);
			} else {
				// use fields
				FillMetaclassFromFields(metaclass,fields);
				//throw new Exception("Don't know what to do with "+contextType.Name+": fields "+fields.Count+", methods "+methods.Count);
			}
	
		}
		
		/// <summary>
		/// Pay attention: if you use this method instead of ExtractFromParser you
		/// need to have call this method on the superclass.
		/// 
		/// Ex. MulExpr extends Expr
		/// 
		/// Is correct to call Convert(Expr) and Convert(MulExpr)
		/// call only Convert(MulExpr) is BAD!
		/// </summary>
		/// <param name="contextType"></param>
		/// <returns></returns>
		public MMetaclass ConvertSingleClass(Type contextType)
		{
			MMetaclass metaclass = CreateMetaClass(contextType);
			FillMetaClass(metaclass,contextType);					
			return metaclass;
		}
		
		// Ex.
		//
		// root : decl; 
		//
		// I create a class Root with a property value of type Decl
		private void FillMetaclassFromMethod(MMetaclass metaclass, System.Reflection.MethodInfo method){
			metaclass.AddFeature(DeriveFeature("value", method.ReturnType));
		}

		private void FillMetaclassFromFields(MMetaclass metaclass, IList<System.Reflection.FieldInfo> fields){
			//Console.WriteLine("(extracting use fields)");
			foreach (var f in fields){
				metaclass.AddFeature(DeriveFeature(f.Name, f.FieldType));
			}
		}
		
		private string SuperMetaclassName(Type contextType){
			if (contextType.BaseType.Equals(typeof(Antlr4.Runtime.ParserRuleContext))){
				return null;
			} else {
				return DeriveMetaclassName(contextType.BaseType);
			}
		}

		private MFeature DeriveFeature(string Name, Type type){
			//Console.WriteLine(" * Feature "+Name+" : "+type);
			if (type==typeof(Antlr4.Runtime.IToken)){
				MAttribute a = new MAttribute();
				a.Name = Capitalize(Name);
				a.Type = SimpleType.String;
				a.Multi = false;
				return a;
			} else if (type==typeof(Antlr4.Runtime.Tree.ITerminalNode)){
				MAttribute a = new MAttribute();
				a.Name = Capitalize(Name);
				a.Type = SimpleType.String;
				a.Multi = false;
				return a;
			} else {
				var a = new MReference();
				a.Name = Capitalize(Name);
				a.Type = this.metamodel.GetMetaClassByName(DeriveMetaclassName(type));
				a.Containment = true;
				a.Multi = false;
				return a;
			}
		}

		private static string Capitalize(string s)
		{
			return s.Substring(0,1).ToUpper()+s.Substring(1);
		}
		
		public static String DeriveMetaclassName(Type type){
			if (!type.Name.EndsWith("Context")){
				throw new Exception("Expected to end with 'Context', instead it is "+type.Name);
			}
			var baseName = type.Name.Substring(0,type.Name.Length-"Context".Length);
			var suffix = "";
			if (type.BaseType!=null && type.BaseType!=typeof(Antlr4.Runtime.ParserRuleContext)){
				suffix = DeriveMetaclassName(type.BaseType);
			}
			return baseName+suffix;
		}
		
	}
}
