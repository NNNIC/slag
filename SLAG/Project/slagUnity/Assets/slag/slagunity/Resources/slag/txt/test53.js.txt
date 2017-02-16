/*
 Test 53
 
 3Dテキスト作成

*/

"using UnityEngine";

function Text3d_Create(s)
{
    var $go = new GameObject();
    var $tm = $go.AddComponent(typeof(UnityEngine.TextMesh));
    $tm.alignment = TextAlignment.Center;
    $tm.anchor    = TextAnchor.MiddleCenter;
    $tm.characterSize = 0.2;
    $tm.fontSize = 64;
    $tm.text = s;
    
    return $go;
}

var $go = Text3d_Create("123");

$go.transform.localScale = Vector3.one * 70;
