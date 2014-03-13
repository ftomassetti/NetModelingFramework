/*
 * Net Modeling Framework - http://github.com/ftomassetti/NetModelingFramework
 * 
 * Federico Tomassetti, 2014.
 */
using System;
using System.Collections.Generic;
using DotLiquid;
using System.Reflection;

using NetModelingFramework.Metamodel;

namespace NetModelingFramework.Model
{
	
	namespace Markers {
		
		[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
		public class MMAttribute : System.Attribute {
			
			public bool ID { get; set; }
			public bool Derived {get; set;}
			
			public MMAttribute(bool id){
				this.ID = id;
				this.Derived = false;
			}
			public MMAttribute(){
				this.ID = false;
				this.Derived = false;
			}
		}
		
		[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
		public class MMReference : System.Attribute {
			
			public bool Containment { get; set; }
			public bool Derived {get; set;}
			
			public MMReference(bool containment = true){
				this.Containment = containment;
				this.Derived = false;
			}
		
		}
		
	}
	
	public interface GroupOfToHashSpecializer
	{
		bool CanSpecialize(Enum val);
		Object SpecializedValue(Enum val);
	}
	
	/// <summary>
	/// Each node of the AST is an MObject.
	/// Each node of the intermediate trasformation is an MObject.
	/// We feed template with an MObject (trasformed to an Hash).
	/// </summary>
	public class MObject
	{
		/// <summary>
		/// Internal cache: just to not calculate the MMetaclass each time.
		/// </summary>
		private static IDictionary<Type,MMetaclass> metaclassesByTypeCache = new Dictionary<Type,MMetaclass>();
		
		/// <summary>
		/// Maybe it should move to another class.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static MMetaclass GetMetaClass(Type type)
		{
			if (!metaclassesByTypeCache.ContainsKey(type)){
				metaclassesByTypeCache.Add(type,CalcMetaclass(type));
			}
			return metaclassesByTypeCache[type];
		}
		
		public MMetaclass GetMetaClass()
		{
			return MObject.GetMetaClass(this.GetType());
		}
		
		private static MMetaclass CalcMetaclass(Type type)
		{
			MMetaclass metaclass = new MMetaclass();
			metaclass.Name = type.Name;
			metaclass.Abstract = type.IsAbstract;
			if (type.BaseType!=typeof(MObject)){
				metaclass.SuperMetaclass = GetMetaClass(type.BaseType);
			}
			foreach (var p in type.GetProperties()){
				if (p.DeclaringType==type){
					if (p.IsDefined(typeof(Markers.MMAttribute),false)){
						MAttribute a = new MAttribute();
						a.Name = p.Name;
						a.Multi = IsAList(p.PropertyType);
						metaclass.Attributes.Add(a);
					}
					if (p.IsDefined(typeof(Markers.MMReference),false)){
						MReference a = new MReference();
						object[] annotations = p.GetCustomAttributes(false);
						Markers.MMReference annotation = null;
						foreach (var an in annotations){
							if (an is Markers.MMReference){
								annotation = an as Markers.MMReference;
							}
						}
						if (annotation==null){
							throw new Exception("Annotation not found");
						}
						a.Name = p.Name;
						a.Containment = annotation.Containment;
						a.Multi = IsAList(p.PropertyType);
						metaclass.References.Add(a);
					}	
				}
			}
			return metaclass;
		}
		
		private static bool IsAList(Type type)
		{
			return type.Name.StartsWith("List`") && type.Namespace.Equals("System.Collections.Generic");
		}
		
		private MMetaclass CalcMetaclass()
		{
			return CalcMetaclass(this.GetType());
		}
		
		public void Set(MReference reference, MObject val)
		{
			PropertyInfo p = FindPropByName(reference.Name);
			p.SetValue(this,val,new object[]{});
		}
		
		public void Set(MAttribute attr, object val)
		{
			PropertyInfo p = FindPropByName(attr.Name);
			p.SetValue(this,val,new object[]{});
		}
		
		private PropertyInfo FindPropByName(string propName)
		{
			return this.GetType().GetProperty(propName);
		}
		
		public Hash ToHash(GroupOfToHashSpecializer specializer = null)
		{
			Hash hash = new Hash();
			
			foreach (var a in this.GetMetaClass().GetAllAttributes()){
				var key = a.Name;
				var val = a.GetValue(this);
				if (val.GetType().IsEnum){
					Enum valAsEnum = (Enum)val;
					if (specializer!=null && specializer.CanSpecialize(valAsEnum)){
						hash.Add(a.Name,specializer.SpecializedValue(valAsEnum));
					} else {
						hash.Add(key,valAsEnum.ToString());
					}
				} else{
					hash.Add(key,val);	
				}
			}
			foreach (var r in this.GetMetaClass().GetAllReferences()){
				var key = r.Name;
				if (r.Multi){
					var valList = (System.Collections.IList)r.GetValue(this);
					var hashesList = new List<Hash>();
					foreach (var val in valList){
						MObject valAsMobject = (MObject)val;
						hashesList.Add(valAsMobject.ToHash(specializer));
					}
					hash.Add(key,hashesList);						
				} else {
					var val = (MObject)r.GetValue(this);
					if (val!=null){
						hash.Add(key,val.ToHash(specializer));
					}
				}
			}
			
			return hash;
		}
	}
	
	public class Printer {
		
		private static string space = "  ";
		
		public static void Print(MObject obj, string indent=""){
			if (obj==null){
				Console.WriteLine(indent+"null");	
				return;
			}
			
			Console.WriteLine(indent+obj.GetMetaClass().Name+" {");
			foreach (var a in obj.GetMetaClass().GetAllAttributes()){
				Console.WriteLine(indent+space+a.Name+"="+a.GetValue(obj));
			}
			foreach (var a in obj.GetMetaClass().GetAllReferences()){
				if (a.Containment){
					var value = a.GetValue(obj);
					if (value is System.Collections.ICollection){
						if ((value as System.Collections.ICollection).Count==0){
							Console.WriteLine(indent+space+a.Name+"= [ ]");
						} else {
							Console.WriteLine(indent+space+a.Name+"= [");
							foreach (var v in (value as System.Collections.ICollection)){
								Print(v as MObject,indent+space+space);
							}
							Console.WriteLine(indent+space+"]");
						}
					} else {
						if (value==null){
							Console.WriteLine(indent+space+a.Name+"= null");
						} else {
							Console.WriteLine(indent+space+a.Name+"=");
							Print(value as MObject,indent+space+space);
						}
					}
				} else{
					var value = a.GetValue(obj);
					// TODO We could look for an attribute Name or add the Marker ID
					//      to identify a particular attribute.
					Console.WriteLine(indent+space+a.Name+"= ref to "+value);
				}
			}			
			Console.WriteLine(indent+"}");
		}
	}
	
}
