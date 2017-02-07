/*
 TEST 55
 
 ボタン
*/

"using UnityEngine";

// ####################
// # ボタンマネージャ #
function $_BM_Update($bhv)
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
function ButtonManager()
{
    var $ht     = Hashtable();
    $ht.go      = new GameObject("ButtonManager");
    $ht.bhv     = AddBehaviour($ht.go);
    
    $ht.bhv.m_updateFunc = $_BM_Update;
    
    return $ht;
}
// # ボタンマネージャ #
// ####################



// #############
// #ボタン作成 #
function _CB_CreateTxtObj($s,$scale)
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
function _CB_CreateRectangleMesh($width,$height)
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

function _CB_CreateBox($width,$height)
{
    var $go = new GameObject();
    var $mr = $go.AddComponent(typeof(MeshRenderer));
    $mr.material = new Material(Shader.Find("Unlit/Color"));
    $mr.material.SetColor("_Color",Color.yellow);
    
    var $mf = $go.AddComponent(typeof(MeshFilter));
    $mf.mesh = _CB_CreateRectangleMesh($width,$height);
    
    var $bc = $go.AddComponent(typeof(BoxCollider));

    return $go;
}

// @ ボタン変更用
function _CB_SetMsg($ht,$msg,$col)
{
    var $tm =$ht.txgo.GetComponent(typeof(TextMesh));
    $tm.Text  = $msg;
    $tm.Color = $col;
}

function _CB_SetColor($ht,$col)
{
    var $r = $ht.go.GetComponent(typeof(Renderer));
    $r.sharedMaterial.SetColor("_Color",$col);
}
function _CB_SetClickFunc($ht,$func)
{
    $ht.bhv.AddMsgFunc("CLICK",$func);
}

// @ デフォルトのクリック関数
function _CB_ClickTempFunc($bhv)
{
    PrintLn("ボタン:"+$bhv.m_usrobj.id + "がクリックされました");
    PrintLn("※$ht.setClickFuncにて差替えせよ");
}

// 本体
function CreateButton($butman, $id, $width, $height)
{
    var $ht          = Hashtable();
    $ht.id           = $id;
    $ht.go           = _CB_CreateBox($width,$height);
    $ht.go.name      = $id;
    $ht.txgo         = _CB_CreateTxtObj($id, 7);

    $ht.bhv          = AddBehaviour($ht.go);
    $ht.bhv.m_usrobj = $ht;
    
    $ht.butman       = $butman;
    $ht.setmsg       = _CB_SetMsg;
    $ht.setcol       = _CB_SetColor;
    $ht.setClickFunc = _CB_SetClickFunc;

    $ht.setClickFunc(_CB_ClickTempFunc);

    return $ht;
}
// #ボタン作成 #
// #############


var $butman  = ButtonManager();
var $but     = CreateButton($butman,"TEST",160,50);

$but.setcol(FromHexColor("008080"));
$but.setmsg("START",FromHexColor("f0f8ff"));

function $_CLICK_ONE($bhv)
{
    var $ht = $bhv.m_usrobj;
    $ht.setcol(FromHexColor("ffd700"));
    $ht.setmsg("STOP",FromHexColor("f0f8ff"));
    $ht.setClickFunc($_CLICK_TWO);
    
}
function $_CLICK_TWO($bhv)
{
    var $ht = $bhv.m_usrobj;
    $ht.setcol(FromHexColor("008080"));
    $ht.setmsg("START",FromHexColor("f0f8ff"));
    $ht.setClickFunc($_CLICK_ONE);
}

$but.setClickFunc($_CLICK_ONE);

