//
// TEST 10
//

var a = (0,1,2,3,4,5,6,7,(100,200));
Dump(a);

a[5]=-5;
Dump(a);

a[10]=10; // out of index
Dump(a);
