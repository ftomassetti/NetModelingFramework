/*
 * Net Modeling Framework - http://github.com/ftomassetti/NetModelingFramework
 * 
 * Federico Tomassetti, 2014.
 */
 
namespace NetModelingFramework.Model
{
	
	/// <summary>
	/// Markers are Attribute used in MetaClasses to identify properties which
	/// have a "semantic value" (i.e., they describe info which is part of the model,
	/// not other kind of values like caches).
	/// </summary>
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
}


