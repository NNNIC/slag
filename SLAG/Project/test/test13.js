/*
 Test 13
*/

"using UnityEngine";

function $_hoge($go,$a)
{
    PrintLn("called! - " + $a + " at " + $go.name);
}

function $_Update($bv)
{
   if (Input.GetKeyDown(KeyCode.A))
   {
       PrintLn("A Key Down");
       SendMsg($bv.gameObject,"touch","touched!!");
   }
}

var $go = new GameObject("HOGE");
var $bh = AddBehaviour($go);
$bh.AddMsgFunc("touch",$_hoge); //メッセージ駆動関数登録
$bh.m_updateFunc = $_Update;

PrintLn("Push A Key!");
