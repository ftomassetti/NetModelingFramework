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
	/// 
	/// Note that a MMetamodel is also a MMetaclass.
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
			return Metaclasses.Exists(e => e.Name.Equals(name));
		}
		
		public MMetaclass GetMetaClassByName(string name){
			if (!HasMetaclass(name)) throw new Exception("No metaclasses named "+name);
			return Metaclasses.Find(e => e.Name.Equals(name));
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
				throw new Exception("Unexpected: this MFeature is neither an MAttribute nor a MReference, feature: "+feature);
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
			return GetAllAttributes().Exists(e => e.Name.Equals(name));
		}
		
		public MAttribute GetAttribute(string name){
			if (!HasAttribute(name)) throw new Exception("No attribute named "+name);
			return GetAllAttributes().Find(e => e.Name.Equals(name));
		}

		public bool HasReference(string name){
			return GetAllReferences().Exists(e => e.Name.Equals(name));
		}
		
		public MReference GetReference(string name){
			if (!HasReference(name)) throw new Exception("No reference named "+name);
			return GetAllReferences().Find(e => e.Name.Equals(name));
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
