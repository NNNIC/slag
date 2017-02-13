/*

test 15 Hashtable/Callback

*/

function func($ht0)
{
    $ht0.v = "YES";
}

var $ht = Hashtable();

$ht.func = func;

$ht.func();

PrintLn($ht.v);

