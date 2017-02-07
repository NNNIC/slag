//
// TEST 07
//

function hoge(x,y,z) 
{
   return x+y+z;
}
function hehe() 
{
   PrintLn("A");
}
function hege()
{
	Print(hoge(5,6,7));
	hehe();
	Print("\n");
}

var c = hoge(1,2,3);

PrintLn("hoge()=" + c);

hehe();
hehe();
hehe();
PrintLn("");
hege();


