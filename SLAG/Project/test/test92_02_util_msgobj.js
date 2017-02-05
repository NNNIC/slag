/*
    テキスト ＋　背景
*/

// API
function _util_create_msgobj_setmsg($ht,$msg)  { $ht.textmesh.text  = $msg;} 
function _util_create_msgobj_setcol($ht,$col)  { $ht.textmesh.color = $col;}
function _util_create_msgobj_setbgsize($ht,$w) { $ht.bggo.transform.localScale = new Vector3($w,$ht.bggo.transform.localScale.y,1);}
function _util_create_msgobj_setbgsize_reset($ht,$scale){ 
    var $bo = $ht.txtgo.GetComponent(typeof(Renderer)).bounds;
    $ht.bggo.transform.localScale = new Vector3($bo.size.x * $scale,$bo.size.y * $scale,1);
}
function _util_create_msgobj_setbgcol($ht,$col){ 
    $ht.bggo.GetComponent(typeof(Renderer)).material.SetColor("_Color",$col); 
}
// ステート
function _util_create_msgobj_$_IDLE($sm,$bFirst) { /*PrintLn("IDLE");*/}
function _util_create_msgobj_$_BLINK_OFF($sm,$bFirst) {
    //PrintLn("OFF");
    if ($bFirst)
    {
        var $ht = $sm.usrobj;
        $ht.child.SetActive(false);
        $sm.WaitTime(0.2);
    }
    else
    {
        $sm.Goto(_util_create_msgobj_$_BLINK_ON);
    }
}
function _util_create_msgobj_$_BLINK_ON($sm,$bFirst) {
    //PrintLn("ON");
    if ($bFirst)
    {
        var $ht = $sm.usrobj;
        $ht.child.SetActive(true);
        $sm.WaitTime(0.2);
    }
    else
    {
        $sm.Goto(_util_create_msgobj_$_BLINK_OFF);
    }
}
function _util_create_msgobj_$_BLINK_STOP($sm,$bFirst) {
    if ($bFirst)
    {
        var $ht = $sm.usrobj;
        $ht.child.SetActive(true);
        $sm.Goto(_util_create_msgobj_$_IDLE);
    }
}

function _util_create_msgobj_blink($ht,$enable) {
    $ht.sm.WaitCancel();
    if ($enable)
    {
        $ht.sm.Goto(_util_create_msgobj_$_BLINK_OFF);
    }
    else
    {
        $ht.sm.Goto(_util_create_msgobj_$_BLINK_STOP);
    }
}
// 本体
function util_create_msgobj($msg)
{
    var $ht      = Hashtable();
    $ht.go       = new GameObject("msgobj");
    $ht.child    = new GameObject("child");
    $ht.child.transform.parent = $ht.go.transform;
    
    $ht.txtgo    = util_CreateTextObj($msg);
    $ht.txtgo.transform.parent = $ht.child.transform;
    $ht.txtgo.transform.localposition = Vector3.back;
    $ht.textmesh = $ht.txtgo.GetComponent(typeof(TextMesh));
    $ht.textmesh.color = Color.green;
    
    $ht.bggo     = GameObject.CreatePrimitive(PrimitiveType.Quad);
    $ht.bggo.transform.parent = $ht.child.transform;
    $ht.bggo.GetComponent(typeof(Renderer)).material = new Material(Shader.Find("Unlit/Color"));
    //$ht.bggo_material = $ht.bggo.GetComponent(typeof(renderer)).material;
    
    var $bo = $ht.txtgo.GetComponent(typeof(Renderer)).bounds;
    //PrintLn($bo);
    $ht.bggo.transform.localScale = new Vector3($bo.size.x,$bo.size.y,1);
    
    //statemachine
    $ht.sm        = StateManager($ht.go);
    $ht.sm.usrobj = $ht;
    $ht.sm.Goto(_util_create_msgobj_$_IDLE);
    
    //api
    $ht.setmsg          = _util_create_msgobj_setmsg;          //メッセージ変更
    $ht.setcol          = _util_create_msgobj_setcol;          //メッセージカラー
    $ht.setbgsize       = _util_create_msgobj_setbgsize;       //BG幅設定
    $ht.setbgsize_reset = _util_create_msgobj_setbgsize_reset; //BG幅リセット
    $ht.setbgcol        = _util_create_msgobj_setbgcol;        //BGカラー
    $ht.blink           = _util_create_msgobj_blink;
    return $ht;
}

// 単体テスト
/*
var utilmsg_$ht = util_create_msgobj("test");

utilmsg_$ht.setmsg("hoge!!");
utilmsg_$ht.setcol(Color.red);
utilmsg_$ht.setbgsize(200);
utilmsg_$ht.setbgsize_reset(1);
utilmsg_$ht.setbgcol(Color.black);


function $_test1($sm,$bFirst)
{
    if ($bFirst)
    {
        utilmsg_$ht.blink(true);
    }
    else
    {
        if (Input.Input.anyKeyDown)
        {
           $sm.WaitTime(0.5);
           $sm.Goto($_test2);
        }
    }
}
function $_test2($sm,$bFirst)
{
    if ($bFirst)
    {
        utilmsg_$ht.blink(false);
    }
    else
    {
        if (Input.Input.anyKeyDown)
        {
           $sm.WaitTime(0.5);
           $sm.Goto($_test1);
        }
    }
}

var $sm = StateManager();
$sm.Goto($_test1);
*/
