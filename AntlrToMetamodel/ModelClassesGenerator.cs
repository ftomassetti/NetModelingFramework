/*
 * Net Modeling Framework - http://github.com/ftomassetti/NetModelingFramework
 * 
 * Federico Tomassetti, 2014.
 */
using System;
using NetModelingFramework.Metamodel;
using NetModelingFramework.Model;
using DotLiquid;

namespace AntlrToMetamodel
{
	public class MySpecializer : GroupOfToHashSpecializer {
		public bool CanSpecialize(Enum val)
		{
			if (val is SimpleType){
				return true;
			}
			return false;
		}
		
		public Object SpecializedValue(Enum val)
		{
			if (val.Equals(SimpleType.String)){
				return "string";
			} else {
				throw new Exception("Unknown val "+val);
			}
		}
	}
		
	public class ModelClassesGenerator
	{
		public ModelClassesGenerator()
		{
		}
		
		
		public string Generate(MMetamodel metamodel)
		{
			String templateCode = System.IO.File.ReadAllText(@"metamodel.liq");
			Template template = Template.Parse(templateCode);
			var specializer = new MySpecializer();
			var code = template.Render(metamodel.ToHash(specializer));
			return code;
		}
	}
}
