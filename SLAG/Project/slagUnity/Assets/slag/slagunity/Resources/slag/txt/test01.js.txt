//
// TEST 01
//

PrintLn("### TEST 01 ###");

var $a=10;

println( $a );

var $c =[1,2,3,4];
Dump($c);

var $d =["hoge","v",-1];
Dump($d);

for(var $i = 0; $i<100; $i++) Print($i.ToString() + "," );

PrintLn();

PrintLn("56 * 3  - 1  = " +  (56 * 3  - 1));
PrintLn("-1 + 56 * 3  = " +  (-1 + 56 * 3));
PrintLn("---");
if ($a==10) for(var $i = 0; $i<10; $i++) Print($i.ToString() + ",");  else for(var $i = 10; $i>=0; $i--) Print($i.ToString() + ",");
PrintLn("---");
while($a>0) Print(($a--).ToString()+",");
