"using UnityEngine";
"using System";

function util_CreateTextObj($s)
{
    var $go = new GameObject($s);
    var $tm = $go.AddComponent(typeof(TextMesh));
    $tm.alignment = TextAlignment.Center;
    $tm.anchor    = TextAnchor.MiddleCenter;
    $tm.characterSize = 6;
    $tm.fontSize = 64;
    $tm.text = $s;
    
    return $go;
}

// ####################
// # ボタンマネージャ #
function $__BM_Update($bhv)
{
    if (!Input.GetMouseButtonDown(0)) return;
    
    var $pos = Input.mousePosition;
    //Dump($pos);
    var $hitgo = GetObjectAtScreenPoint($pos);
    
    if ($hitgo!=null)
    {
       //Dump($hitgo);
       SendMsg($hitgo,"CLICK");
    }
}
function util_buttonManager()
{
    var $ht     = Hashtable();
    $ht.go      = new GameObject("ButtonManager");
    $ht.bhv     = AddBehaviour($ht.go);
    
    $ht.bhv.m_updateFunc = $__BM_Update;
    $ht.list    = [];
    
    return $ht;
}
// # ボタンマネージャ #
// ####################