// ###############
// #ボタンパーツ #
function $__CB_CreateTxtObj($s,$scale)
{
    var $go = new GameObject($s);
    var $tm = $go.AddComponent(typeof(TextMesh));
    $tm.alignment = TextAlignment.Center;
    $tm.anchor    = TextAnchor.MiddleCenter;
    $tm.characterSize = $scale;//0.2;
    $tm.fontSize = 64;
    $tm.text = $s;
    
    return $go;
}
function $__CB_CreateRectangleMesh($width,$height)
{
    var $verts   = new Vector3[4];
    var $normals = new Vector3[4];
    var $uv      = new Vector2[4];
    var $tri     = new int[6];
    
    var $hw = $width  / 2;
    var $hh = $height / 2;

    $verts[0] = new Vector3(-$hw, -$hh , 0);
    $verts[1] = new Vector3(+$hw, -$hh , 0);
    $verts[2] = new Vector3(-$hw, +$hh , 0);
    $verts[3] = new Vector3(+$hw, +$hh , 0);

    for (var $i = 0; $i < $normals.Length; $i++) {
        $normals[$i] = Vector3.up;
    }

    $uv[0] = new Vector2(0, 0);
    $uv[1] = new Vector2(1, 0);
    $uv[2] = new Vector2(0, 1);
    $uv[3] = new Vector2(1, 1);

    $tri[0] = 0;
    $tri[1] = 2;
    $tri[2] = 3;

    $tri[3] = 0;
    $tri[4] = 3;
    $tri[5] = 1;

    var $mesh  = new Mesh();
    $mesh.vertices  = $verts;
    $mesh.triangles = $tri;
    $mesh.uv        = $uv;
    $mesh.normals   = $normals;

    return $mesh;
}
function $__CB_CreateBox($width,$height)
{
    var $go = new GameObject();
    var $mr = $go.AddComponent(typeof(MeshRenderer));
    $mr.material = new Material(Shader.Find("Unlit/Color"));
    $mr.material.SetColor("_Color",Color.yellow);
    
    var $mf  = $go.AddComponent(typeof(MeshFilter));
    $mf.mesh = $__CB_CreateRectangleMesh($width,$height);
    
    var $bc = $go.AddComponent(typeof(BoxCollider));

    return $go;
}
// #ボタンパーツ #
// ###############

// ##############
// #ベースボタン#
function $__CB_SetMsg($ht,$msg,$col)
{
    var $tm =$ht.txgo.GetComponent(typeof(TextMesh));
    $tm.Text  = $msg;
    $tm.Color = $col;
}

function $__CB_SetColor($ht,$col)
{
    var $r = $ht.go.GetComponent(typeof(Renderer));
    $r.sharedMaterial.SetColor("_Color",$col);
}
function $__CB_SetClickFunc($ht,$func)
{
    $ht.bhv.AddMsgFunc("CLICK",$func);
}

// @ デフォルトのクリック関数
function $__CB_ClickTempFunc($bhv)
{
    //PrintLn("ボタン:"+$bhv.m_usrobj.id + "がクリックされました");
    //PrintLn("※$ht.setClickFuncにて差替えせよ");
}
// @ ボタンステート
function $__CB_ClickState_Func($bhv)
{
    //PrintLn("ボタンON:"+$bhv.m_usrobj.id + "がクリックされました");
    var $ht = $bhv.m_usrobj;
    $ht.sm.Goto($__CB_STATE_OFF);
}
function $__CB_STATE_ON($sm,$bFirst)
{
   if ($bfirst)
   {
       //PrintLn("STATE ON");
       var $ht = $sm.bhv.m_usrobj;
       $ht.setmsg($ht.on_text,$ht.on_text_col);
       $ht.setcol($ht.on_col);
       $ht.setClickFunc($__CB_ClickState_Func);
       
   }
}
function $__CB_STATE_OFF($sm,$bFirst)
{
    if ($bFirst)
    {
        var $ht = $sm.bhv.m_usrobj;
        $ht.clickFunc();
        $ht = $sm.bhv.m_usrobj;
        
        //PrintLn("STATE OFF");

        $ht.setmsg($ht.off_text,$ht.off_text_col);
        $ht.setcol($ht.off_col);
        $ht.setClickFunc(null);
        $sm.WaitTime(0.2);
    }
    else
    {
        $sm.Goto($__CB_STATE_ON);
    }
}
// #ベースボタン#
// ##############

// ##############
// # ボタン作成 #
/*
 　クリックボタン

   概要：クリック時に色・テキストが変更され指定の関数を呼ぶ。
   
　 使い方：
   var $ht = hashtable();
   
   $ht.width       = 150;
   $ht.height      =  50;
   $ht.clickFunc   = $_ClickFunc;
   
   $ht.on_col      = Color.green;
   $ht.on_text     = "ON";
   $ht.on_text_col = Color.white;
   
   $ht.off_col     = Color.red;
   $ht.off_text    = "OFF";
   $ht.off_text_col= Color.red;
   
   var $but = CreateButton($butman, $id, $ht);  //$butman:ボタンマネージャ $id;テキスト
   
*/
function CreateButton($butman, $id, $ht)
{
    $ht.id           = $id;
    $ht.go           = $__CB_CreateBox($ht.width,$ht.height);
    $ht.go.name      = $id;
    $ht.txgo         = $__CB_CreateTxtObj("Text", 7);
    $ht.txgo.transform.parent = $ht.go.transform;

    $ht.bhv          = AddBehaviour($ht.go);
    $ht.bhv.m_usrobj = $ht;
    
    $ht.butman       = $butman;
    
    $ht.setmsg       = $__CB_SetMsg;
    $ht.setcol       = $__CB_SetColor;
    $ht.setClickFunc = $__CB_SetClickFunc;

    $ht.setmsg("MSG",Color.white);
    //$ht.setClickFunc($__CB_ClickTempFunc);
    //$ht.setClickFunc($__CB_ClickState_Func);

    $ht.butman.list.Add($ht);
    
    //ステート
    $ht.sm = StateManager($ht.go);
    $ht.sm.Goto($__CB_STATE_ON);

    return $ht;
}

/* 単体テスト 
function $_ClickFunc($ht)
{
    PrintLn("Clicked!");
}


var $butman = util_buttonManager();

var $ht = hashtable();

$ht.width       = 150;
$ht.height      = 50;
$ht.clickFunc   = $_ClickFunc;

$ht.on_col      = Color.green;
$ht.on_text     = "-ON-";
$ht.on_text_col = Color.white;

$ht.off_col     = Color.red;
$ht.off_text    = "-OFF-";
$ht.off_text_col= Color.black;

var $but = CreateButton($butman, "test", $ht);  //$butman:ボタンマネージャ $id;テキスト
*/

