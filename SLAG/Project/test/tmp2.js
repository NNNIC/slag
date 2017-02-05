"using UnityEngine";
"using System";

var ht=  Hashtable();
ht.s = "TEST";
var ht2 = Hashtable();
ht2[ht.s] = 10;

Dump(ht2);
Dump(ht2[ht.s]);
