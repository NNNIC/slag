/*
   カード表面作成
   
   <-  80 ->
 　+-------+---
   |  ♡    |  ^
   |  A    |  | 100
   |       |  v
   +-------+---
   
*/

function util_create_cardfront_obj($mark,$num) //out: gameobj /in: $mark = "h","d","c" or "s" $num = 1 ～ 13
{
    var $go = new GameObject("front");
    
    // 背景 : マーク付き
    var $bggo  = GameObject.CreatePrimitive(PrimitiveType.Quad);
    $bggo.name = "bg";
    $bggo.transform.parent = $go.transform;
    $bggo.transform.localScale = new Vector3(80,100,1);
    
    UnityEngine.Object.DestroyImmediate($bggo.GetComponent(typeof(MeshCollider)));
    
    var $rdr = $bggo.GetComponent(typeof(Renderer));
    $rdr.material = new Material(Shader.Find("Unlit/Texture"));
    
    var $png = null;
    var $m = $mark.substring(0,1);
    switch($m)
    {
    case "h": $png = "heart";   break;
    case "d": $png = "diamond"; break;
    case "c": $png = "clab";    break;
    default : $png = "spade";   break;
    }
    
    var $tex = Resources.Load("2d/" + $png);
    
    $rdr.material.setTexture("_MainTex",$tex);
    $rdr.material.mainTextureScale  = new Vector2(1*2.5,1.25*2.5);
    $rdr.material.mainTextureOffset = new Vector2(-0.75,-1.85);
    
    //数字
    var $s = $num.ToString();
    if      ($num == 1)  $s = "A";
    else if ($num == 11) $s = "J";
    else if ($num == 12) $s = "Q";
    else if ($num == 13) $s = "K";
    
    var $txtgo    = util_CreateTextObj($s);
    $txtgo.transform.parent = $go.transform;
    $txtgo.transform.localPosition = Vector3.back + Vector3.up * (-21);
    var $tm       = $txtgo.GetComponent(typeof(TextMesh));
    $tm.color = Color.Black;
    
    //強調用
    var $empgo = GameObject.CreatePrimitive(PrimitiveType.Quad);
    $empgo.name = "emp";
    $empgo.transform.parent        = $go.transform;
    $empgo.transform.localPosition = new Vector3(0,0,1);
    $empgo.transform.localScale    = new Vector3(80*1.1,100*1.1,1);
    $rdr = $empgo.GetComponent(typeof(Renderer));
    $rdr.material = new Material(Shader.Find("Unlit/Color"));
    $rdr.material.Color = Color.red;
    
    //選択
    var $chgo_parent = new GameObject("chg");
    $chgo_parent.transform.parent = $go.transform;
    var $chgo = util_CreateTextObj("CHANGE");
    $chgo.transform.parent = $chgo_parent.transform;
    $chgo.transform.localPosition = Vector3.back * 2;
    $chgo.transform.localScale = Vector3.one * 0.5;
    $tm       = $chgo.GetComponent(typeof(TextMesh));
    $tm.color = new Color(1,195/255,0,1);
    
    return $go;
}

//単体試験
//var $go = util_create_cardfront_obj("h",1);

