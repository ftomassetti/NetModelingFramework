/*
 * Net Modeling Framework - http://github.com/ftomassetti/NetModelingFramework
 * 
 * Federico Tomassetti, 2014.
 */
using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using NetModelingFramework.Model;
using NetModelingFramework.Metamodel;
using System.Reflection;
using System.Collections.Generic;

namespace AntlrToMetamodel
{
	/// <summary>
	/// Description of AntlrContextNodeToMNodesConverter.
	/// </summary>
	public class AntlrContextNodeToMNodesConverter
	{
		public AntlrContextNodeToMNodesConverter()
		{
		}
		
		public MObject Convert(ParserRuleContext ctxNode, string namespaceName)
		{
			if (ctxNode==null){
				return null;
			}
			
			// TODO pass them to next calls of Convert
			Dictionary<string,Type> types = new Dictionary<string, Type>();
			foreach (var type in Assembly.GetExecutingAssembly().GetTypes()){
				// Yes, Namespace can be null
				if (type.Namespace!=null && type.Namespace.Equals(namespaceName)){
					types.Add(type.Name,type);
				}
			}
			
			// Find the corresponding metaclass and instantiate it
			
			Console.WriteLine("ContextNode: "+ctxNode.GetType().Name);
			string correspondingMetaclassName = AntlrContextReflection.DeriveMetaclassName(ctxNode.GetType());
			Console.WriteLine("MetaclassName: "+correspondingMetaclassName);
			if (!types.ContainsKey(correspondingMetaclassName)){
				throw new Exception("Namespace "+namespaceName+" does not contain a class named "+correspondingMetaclassName);
			}
			
			Type correspondingMetaclass = types[correspondingMetaclassName];
			MObject correspondingNode = (MObject)Activator.CreateInstance(correspondingMetaclass);
			
			// Assign all the attributes
			MMetaclass metaclass = correspondingNode.GetMetaClass();
			foreach (var a in metaclass.GetAllAttributes()){
				Console.WriteLine(" -> convert att "+a.Name);
				if (a.Multi){
					throw new Exception("Multi att conversion not implemented");
				} else {
					object convertedAttrValue = ConvertSimpleType(GetCtxAttrValue(ctxNode,a.Name));
					correspondingNode.Set(a,convertedAttrValue);
				}
			}
			
			// Assign all the references
			foreach (var r in metaclass.GetAllReferences()){
				Console.WriteLine(" -> convert ref "+r.Name);
				if (!r.Containment){
					throw new Exception("All classes mapping ANTLR generated nodes should not use not-containement references");
				}
				if (r.Multi){
					throw new Exception("Multi ref conversion not implemented");
				} else {
					MObject convertedRefValue = Convert(GetCtxRefValue(ctxNode,r.Name),namespaceName);
					correspondingNode.Set(r,convertedRefValue);
				}
			}
			
			return correspondingNode;
		}
		
		private object ConvertSimpleType(object simpleType)
		{
			if (simpleType==null){
				return null;
			}
			if (simpleType is IToken)
			{
				IToken token = (IToken)simpleType;
				return token.Text;
			} else if (simpleType is ITerminalNode){
				ITerminalNode terminalNode = (ITerminalNode)simpleType;
				return terminalNode.GetText();
			} else {
				throw new Exception("I don't know what to do: "+simpleType.GetType().Name);
			}			
		}
		
		
		private object GetCtxAttrValue(ParserRuleContext ctxNode, string featureName)
		{
			Type ctxClass = ctxNode.GetType();
			// Value is used when there is just one field and no name is given,
			// it is the default name for the only field
			if (featureName.Equals("Value")){
				//throw new Exception("Not implemented: class "+ctxClass.Name+", feature "+featureName);
				return FindAndCallTheOnlyMethodReturningAContextNode(ctxNode);
			} else {
				return FindAndReadFieldByName(ctxNode, featureName);
			}
		}
		
		private ParserRuleContext GetCtxRefValue(ParserRuleContext ctxNode, string featureName)
		{
			Type ctxClass = ctxNode.GetType();
			// Value is used when there is just one sub-node and no name is given,
			// it is the default name for the only child
			if (featureName.Equals("Value")){
				return (ParserRuleContext)FindAndCallTheOnlyMethodReturningAContextNode(ctxNode);
			} else {
				return (ParserRuleContext)FindAndReadFieldByName(ctxNode, featureName);
			}
		}
		
		private object FindAndReadFieldByName(ParserRuleContext ctxNode, string fieldName)
		{			
			Type ctxClass = ctxNode.GetType();
			var fields = new List<System.Reflection.FieldInfo>();
	        foreach (var f in ctxClass.GetFields()){
				if (!f.FieldType.IsPrimitive && f.Name.ToLower().Equals(fieldName.ToLower())){
					if (f.FieldType.IsSubclassOf(typeof(ParserRuleContext)) || f.FieldType.Equals(typeof(IToken))) {
						fields.Add(f);
					}
				}	        }
			if (fields.Count!=1){
				throw new Exception("Expected ONE field named "+fieldName+" in "+ctxClass.Name+"... investigate. Found: "+fields.Count);
			}
			var fieldToRead = fields[0];
			return fieldToRead.GetValue(ctxNode);
		}
		
		private ParserRuleContext FindAndCallTheMethodByName(ParserRuleContext ctxNode, string methodName)
		{
			Type ctxClass = ctxNode.GetType();
			var methods = new List<System.Reflection.MethodInfo>();
	        foreach (var p in ctxClass.GetMethods()){
				if (p.GetParameters().Length==0 && !p.ReturnType.IsPrimitive && p.Name.Equals(methodName)){
					if (p.ReturnType.IsSubclassOf(typeof(ParserRuleContext)) ) {
						methods.Add(p);
					}
	        	}
	        }
			if (methods.Count!=1){
				throw new Exception("Expected ONE method named "+methodName+" in "+ctxClass.Name+"... investigate. Found: "+methods.Count);
			}
			var methodToCall = methods[0];
			return (ParserRuleContext)methodToCall.Invoke(ctxNode,new object[]{});
		}
		
		/// <summary>
		/// Maybe we can refactorize it, there is some duplication with AntlrContextReflection.
		/// </summary>
		/// <param name="ctxNode"></param>
		/// <returns></returns>
		private object FindAndCallTheOnlyMethodReturningAContextNode(ParserRuleContext ctxNode)
		{
			Type ctxClass = ctxNode.GetType();
			var methods = new List<System.Reflection.MethodInfo>();
	        foreach (var p in ctxClass.GetMethods()){
	        	if (p.GetParameters().Length==0 && !p.ReturnType.IsPrimitive){
					if (p.ReturnType.IsSubclassOf(typeof(ParserRuleContext)) 
					    || p.ReturnType.Equals(typeof(ITerminalNode))) {
						methods.Add(p);
					}
	        	}
	        }
			if (methods.Count!=1){
				throw new Exception("Expected ONE method of this type... investigate. Found: "+methods.Count);
			}
			var methodToCall = methods[0];
			return methodToCall.Invoke(ctxNode,new object[]{});
		}
	}
	
}
