using NetModelingFramework.Model;
using NetModelingFramework.Model.Markers;

namespace GeneratedModelClasses {



 class Root : MObject { 



// Reference Value
[MMReference(Containment=true)]
public Expr Value { get; set; }



}



 class Expr : MObject { 




}



 class MultExpr :  Expr  { 


// Attribute Operator
[MMAttribute()]
public string Operator { get; set; }




// Reference Left
[MMReference(Containment=true)]
public Expr Left { get; set; }


// Reference Right
[MMReference(Containment=true)]
public Expr Right { get; set; }



}



 class MinusExpr :  Expr  { 




// Reference Value
[MMReference(Containment=true)]
public Expr Value { get; set; }



}



 class AddExpr :  Expr  { 


// Attribute Operator
[MMAttribute()]
public string Operator { get; set; }




// Reference Left
[MMReference(Containment=true)]
public Expr Left { get; set; }


// Reference Right
[MMReference(Containment=true)]
public Expr Right { get; set; }



}



 class SubExpr :  Expr  { 


// Attribute Operator
[MMAttribute()]
public string Operator { get; set; }




// Reference Left
[MMReference(Containment=true)]
public Expr Left { get; set; }


// Reference Right
[MMReference(Containment=true)]
public Expr Right { get; set; }



}



 class IntExpr :  Expr  { 


// Attribute Value
[MMAttribute()]
public string Value { get; set; }





}



 class ParenExpr :  Expr  { 




// Reference Value
[MMReference(Containment=true)]
public Expr Value { get; set; }



}



 class DivExpr :  Expr  { 


// Attribute Operator
[MMAttribute()]
public string Operator { get; set; }




// Reference Left
[MMReference(Containment=true)]
public Expr Left { get; set; }


// Reference Right
[MMReference(Containment=true)]
public Expr Right { get; set; }



}



}