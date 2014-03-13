using NetModelingFramework.Model;
using NetModelingFramework.Model.Markers;

namespace {{name}} {

{% for mc in metaclasses %}

{% if mc.abstract %}abstract{% endif %} class {{mc.name}} : {% if mc.SuperMetaclass %} {{mc.SuperMetaclass.Name}} {% else %}MObject{% endif %} { 

{% for a in mc.attributes %}
// Attribute {{a.Name}}
{% if a.multi %}
[MMAttribute()]
public List<{{a.Type}}> {{a.Name}} { get; set; }
{% else %}
[MMAttribute()]
public {{a.Type}} {{a.Name}} { get; set; }
{% endif %}
{% endfor %}

{% for r in mc.references %}
// Reference {{r.Name}}
{% if r.multi %}
[MMReference(Containment=true)]
public List<{{r.Type.Name}}> {{r.Name}} { get; set; }
{% else %}
[MMReference(Containment=true)]
public {{r.Type.Name}} {{r.Name}} { get; set; }
{% endif %}
{% endfor %}

}

{% endfor %}

}