/*

 TEST.JS
 
 配列

*/
//"using UnityEngine";

var x = "ABCDEF";
var n = x[0];
PrintLn(n.ToString());

n = x[3];
PrintLn(n.ToString());

var l = new int[3];
l[0]=100;
l[1]=200;
l[2]=300;

var i = l[0];
PrintLn(i.ToString());


function $get()
{
    return "vwxyz";
}

PrintLn($get());
PrintLn("" + $get()[4] + $get()[3] + $get()[2] + $get()[1] + $get()[0]);


Function $get()
{
    return "0123";
}

var $n0 = $get()[0].ToString();
$n0 += $get()[1].ToString();
$n0 += $get()[2].ToString();
$n0 += $get()[3].ToString();

PrintLn($n0);

var $i = Cast(int,1);
PrintLn($i);
PrintLn($i.GetType());

$i += 4;
PrintLn($i);
PrintLn($i.GetType());

$i = Cast(uint,10);
PrintLn($i);
PrintLn($i.GetType());

PrintLn("\nEND\n");


