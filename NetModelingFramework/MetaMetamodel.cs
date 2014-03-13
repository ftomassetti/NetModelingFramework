/*
 * Net Modeling Framework - http://github.com/ftomassetti/NetModelingFramework
 * 
 * Federico Tomassetti, 2014.
 */
using System;
using System.Reflection;
using System.Collections.Generic;

using NetModelingFramework.Model;
using NetModelingFramework.Model.Markers;

namespace NetModelingFramework.Metamodel
{
	/// <summary>
	/// A metamodel is basically a collection of metaclasses with a name.
	/// </summary>
	public class MMetamodel : MObject
	{
		[MMAttribute()]
		public string Name { get; set; }
		
		[MMReference(true)]
		public List<MMetaclass> Metaclasses { get; set; }
		
		public MMetamodel()
		{
			this.Metaclasses = new List<MMetaclass>();
		}

		public bool HasMetaclass(string name){
			foreach (var a in Metaclasses){
				if (a.Name.Equals(name)){
					return true;
				}
			}
			return false;
		}
		
		public MMetaclass GetMetaClassByName(string name){
			foreach (var a in Metaclasses){
				if (a.Name.Equals(name)){
					return a;
				}
			}
			throw new Exception("No metaclasses named "+name);
		}				
	}
		
	public class MMetaclass : MObject
	{	
		[MMAttribute()]
		public bool Abstract { get; set; }
		
		[MMAttribute()]
		public string Name { get; set; }
		
		[MMReference(false)]
		public MMetaclass SuperMetaclass { get; set; }
		
		/// <summary>
		/// Only the declared attributes, not the ones inherited.
		/// </summary>
		[MMReference(true)]
		public List<MAttribute> Attributes { get; set; }

		/// <summary>
		/// Only the declared references, not the ones inherited.
		/// </summary>
		[MMReference(true)]
		public List<MReference> References { get; set; }
		
		public MMetaclass()
		{
			this.Attributes = new List<MAttribute>();
			this.References = new List<MReference>();
		}
		
		public void AddFeature(MFeature feature)
		{
			if (feature is MAttribute){
				this.Attributes.Add(feature as MAttribute);
			} else if (feature is MReference){
				this.References.Add(feature as MReference);
			} else {
				throw new Exception("Also");
			}
		}
		
		/// <summary>
		/// All the atributes, also the ones inherited.
		/// </summary>
		/// <returns></returns>
		public List<MAttribute> GetAllAttributes(){
			var l = new List<MAttribute>();
			if (this.SuperMetaclass!=null){
				l.AddRange(this.SuperMetaclass.GetAllAttributes());
			}
			l.AddRange(Attributes);
			return l;
		}
		
		/// <summary>
		/// All the references, also the ones inherited.
		/// </summary>
		/// <returns></returns>
		public List<MReference> GetAllReferences(){
			var l = new List<MReference>();
			if (this.SuperMetaclass!=null){
				l.AddRange(this.SuperMetaclass.GetAllReferences());
			}
			l.AddRange(References);
			return l;
		}	

		public bool HasAttribute(string name){
			foreach (var a in GetAllAttributes()){
				if (a.Name.Equals(name)){
					return true;
				}
			}
			return false;
		}
		
		public MAttribute GetAttribute(string name){
			foreach (var a in GetAllAttributes()){
				if (a.Name.Equals(name)){
					return a;
				}
			}
			throw new Exception("No attribute named "+name);
		}

		public bool HasReference(string name){
			foreach (var a in GetAllReferences()){
				if (a.Name.Equals(name)){
					return true;
				}
			}
			return false;
		}
		
		public MReference GetReference(string name){
			foreach (var a in GetAllReferences()){
				if (a.Name.Equals(name)){
					return a;
				}
			}
			throw new Exception("No reference named "+name);
		}		
	}
	
	/// <summary>
	/// MFeature is the common ancestor of MAttribute and MReference.
	/// </summary>
	public abstract class MFeature : MObject
	{
		[MMAttribute()]
		public string Name { get; set; }
		
		[MMAttribute()]
		public bool Multi { get; set; }
		
		[MMAttribute()]
		public bool Derived {get; set; }
		
		public Object GetValue(MObject mo)
		{
			PropertyInfo prop = mo.GetType().GetProperty(this.Name);
			return prop.GetValue(mo, null);
		}		
	}
	
	/// <summary>
	/// Primitive types, used by MAttribute. 
	/// </summary>
	public enum SimpleType
	{
		String,
		Integer,
		Float,
		Bool
	}

	public class MAttribute : MFeature
	{
		[MMAttribute()]
		public SimpleType Type { get; set; }		
	}
	
	public class MReference : MFeature
	{
		[MMAttribute()]
		public bool Containment { get; set; }
		
		[MMReference(true)]
		public MMetaclass Type { get; set; }		
	}	
}
