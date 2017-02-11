/*
 Test 51
 
 キューブ生成・削除・回転制御
 
 ↑キーと↓キーで回転制御
 Delキーでキューブ削除

*/

"using UnityEngine";

var $speed = 50;
var $cur = 0;

function $_Update($bv)
{
    Debug.Log(Time.time);
    $cur += $speed / 10;
    $bv.transform.localEulerAngles = new Vector3($cur, 0, 0);

    
    var $bUp  = Input.GetKey(KeyCode.UpArrow);
    var $bDwn = Input.GetKey(KeyCode.DownArrow);
    var $bDel = Input.GetKey(KeyCode.D);

    if ($bUp)  { $speed++; Println($speed); }
    
    if ($bDwn) { $speed--; Println($speed); }

    if ($bDel) { UnityEngine.Object.Destroy($bv.gameobject); PrintLn("Destroy!");}
}

function $_OnDestroy($bv)
{
    PrintLn("OnDestroy called!");
}


var $go = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube);

$go.transform.localScale = Vector3.one * 50;

var $m_bhv = AddBehaviour($go);

$m_bhv.m_updateFunc    = $_Update;
$m_bhv.m_onDestroyFunc = $_OnDestroy;

PrintLn("Up key   --- speed++");
PrintLn("Down key --- speed--");
PrintLn("D key    --- delete");

