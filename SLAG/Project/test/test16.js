//
// TEST 16 for..in
//

PrintLn("#1");
var a = [0,1,2,3,4];
for(var i in a) 
{
    PrintLn(i);
}

PrintLn("#2");
var j;
for(j in a)
{
    PrintLn(j);
}

PrintLn("#3");
var s = "hoge!!";
for(var i in s)
{
    Print(i);
}
PrintLn("");

PrintLn("#4");
var bin = new byte[32];
for(var i=0;i<bin.Length;i++)
{
    bin[i] = i;
}

for(var i in bin)
{
    Print(i.toString()+",");
}
PrintLn("");

PrintLn("#5");
for(var i in System.Enum.GetNames(typeof(UnityEngine.RuntimePlatform)))
{
   PrintLn(i);
}

PrintLn("#6");
for(var i in [0,1,2,3,4,5,6,7,8,9])
{
    PrintLn(i);
}

PrintLn("#7");
for(var i in [0,1,2,3,4,5,6,7,8,9])
{
    PrintLn(i);
}

PrintLn("#8");
var ht = Hashtable();
ht._0 = 0;
ht._1 = 1;
ht._2 = 2;
ht._3 = 3;

for(var k in hashtableKeys(ht))
{
    PrintLn(k.toString() + "=" + ht[k].ToString());
}

PrintLn("#9");
for(var i in [0,1,2,3,4,5,6,7,8,9])
{
    PrintLn(i);
    if (i==5)
    {
        PrintLn("Break at " + i);
        break;
    }
}

PrintLn("#10");
for(var i in [0,1,2,3,4,5,6,7,8,9])
{
    if (i<5)
    {
        PrintLn("Continue :" + i);
        Continue;
    }
    PrintLn(i);
}

PrintLn("#11");
function hoge()
{
    for(var i in [0,1,2,3,4,5,6,7,8,9])
    {
        PrintLn(i);
        if (i==8)
        {
            PrintLn("Return :" + i);
            return;
        }
    }
}
hoge();
PrintLn("Returned from hoge()");

