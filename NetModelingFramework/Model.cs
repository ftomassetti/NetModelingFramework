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
			// Ok, that is probably not the best solution
			return type.Name.StartsWith("List`") && type.Namespace.Equals("System.Collections.Generic");
		}
		
		private bool IsRootNode(Type type)
        {
            return (Owner == null);
        }
		
        public MObject Owner { get; set; }
        
        public MObject Root()
        {
            if (Owner == null)
            {
                return this;
            }
            else
            {
                return Owner.Root();
            }
        }
        public void ReplaceWith(MObject newObj)
        {
            // TODO also relations refering (but not containing the object)
            // could be revised, but we need to track them
            if (Owner == null)
            { 
                return; 
            }
            Owner.ReplaceYourChild(this, newObj);
        }
        protected void ReplaceYourChild(MObject oldChild, MObject newChild)
        {
        	// I look among all the references were the oldChild was contained
        	// and I place the newChild there
            foreach (var reference in this.GetMetaClass().References)
            {
                if (reference.Containment)
                {
                    var valueOfReference = reference.GetValue(this);
                    if (reference.Multi)
                    {
                        var valueOfReferenceAsList = (System.Collections.IList)valueOfReference;
                        for (int i = 0; i < valueOfReferenceAsList.Count; i++)
                        {
                            if (valueOfReferenceAsList[i] == oldChild)
                            {
                                this.SetAt(reference, i, newChild);
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (valueOfReference == oldChild)
                        {
                            this.Set(reference, newChild);
                            return;
                        }
                    }

                }
            }
            throw new Exception("I don't have this child!");
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
                if (val != null)
                {
                    if (val.GetType().IsEnum)
                    {
                        Enum valAsEnum = (Enum)val;
                        if (specializer != null && specializer.CanSpecialize(valAsEnum))
                        {
                            hash.Add(a.Name, specializer.SpecializedValue(valAsEnum));
                        }
                        else
                        {
                            hash.Add(key, valAsEnum.ToString());
                        }
                    }
                    else
                    {
                        hash.Add(key, val);
                    }
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
		        
		public List<MObject> GetChildren()
        {
            List<MObject> children = new List<MObject>();
            foreach (var r in this.GetMetaClass().References)
            {
                if (r.Containment)
                {
                    if (r.Multi)
                    {
                        var listChild = r.GetValue(this);
                        if (listChild != null)
                        {
                            var listOfChildren = (System.Collections.IList)listChild;
                            foreach (var c in listOfChildren)
                            {
                                children.Add((MObject)c);
                            }
                        }
                    }
                    else
                    {
                        var child = r.GetValue(this);
                        if (child != null)
                        {
                            children.Add((MObject)child);
                        }
                    }
                }
            }
            return children;
        }
        public void Add(MReference reference, MObject val)
        {
            if (!reference.Multi)
            {
                throw new Exception("Call only on Multi references!");
            }
            PropertyInfo p = FindPropByName(reference.Name);
            System.Collections.IList list = (System.Collections.IList)p.GetValue(this, new object[] { });
            list.Add(val);
        }
        public void SetAt(MReference reference, int index, MObject val)
        {
            if (!reference.Multi)
            {
                throw new Exception("Call only on Multi references!");
            }
            PropertyInfo p = FindPropByName(reference.Name);
            System.Collections.IList list = (System.Collections.IList)p.GetValue(this, new object[] { });
            list[index] = val;
        }
		

	}
	
	public class Printer {
		
		private static string space = "  ";
		
		public static void Print(MObject obj, string indent=""){
        	Console.WriteLine(PrintToString(obj));
		}

        public static string PrintToString(MObject obj, string indent = "")
        {
            string result = "";
            if (obj == null)
            {
                return(indent + "null");
            }

            result+=(indent + obj.GetMetaClass().Name + " {");
            foreach (var a in obj.GetMetaClass().GetAllAttributes())
            {
                result += (indent + space + a.Name + "=" + a.GetValue(obj)+"\n");
            }
            foreach (var a in obj.GetMetaClass().GetAllReferences())
            {
                if (a.Containment)
                {
                    var value = a.GetValue(obj);
                    if (value is System.Collections.ICollection)
                    {
                        if ((value as System.Collections.ICollection).Count == 0)
                        {
                            result += (indent + space + a.Name + "= [ ]" + "\n");
                        }
                        else
                        {
                            result += (indent + space + a.Name + "= [" + "\n");
                            foreach (var v in (value as System.Collections.ICollection))
                            {
                                result += PrintToString(v as MObject, indent + space + space);
                            }
                            result += (indent + space + "]" + "\n");
                        }
                    }
                    else
                    {
                        if (value == null)
                        {
                            result += (indent + space + a.Name + "= null" + "\n");
                        }
                        else
                        {
                            result += (indent + space + a.Name + "=" + "\n");
                            result += PrintToString(value as MObject, indent + space + space);
                        }
                    }
                }
                else
                {
                    var value = a.GetValue(obj);
                    // TODO we could print the Name attribute or an attribute marked as ID, if present
                    result += (indent + space + a.Name + "= ref to " + value + "\n");
                }
            }
            result += (indent + "}" + "\n");
            return result;
        }
	}
	
}
