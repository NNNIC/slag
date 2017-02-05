/*
Hashtable $ht = util_create_card(cardmark mark,int num);           
          $ht.go  --- GameObject
          $ht.bhv --- slagremote_unity_monobehaviour
          $ht.setClickFunc(func)  --- クリック時のファンクション
          $ht.flip(bool showhide) --- フリップ
          $ht.setSelect(bool onoff);        --- セレクトモード
          $ht.setStrong(bool onoff);        --- 強調

*/
function util_create_card_$_clicked_dummy($bhv)
{
    //PrintLn("TEST Clicked " + $bhv.go.name);
}
function util_create_card_$_set_clickfunc($ht,$func)
{
    var $bhv = $ht.bhv;
    $bhv.AddMsgFunc("CLICK",$func);
}
function util_create_card_$_flip($bhv,$faceOrBack)
{
    var $frgo = $bhv.frgo;
    var $bkgo = $bhv.bkgo;
    if ($faceOrBack)
    {
        $frgo.SetActive(true);
        $frgo.transform.find("emp").gameobject.SetActive(false);
        $frgo.transform.find("chg").gameobject.SetActive(false);
        $bkgo.SetActive(false);
    }
    else
    {
        $frgo.transform.find("emp").gameobject.SetActive(false);
        $frgo.transform.find("chg").gameobject.SetActive(false);
        $frgo.SetActive(false);
        $bkgo.SetActive(true);           
    }
}
function  util_create_card_$_getFlip($bhv)
{
    return $bhv.frgo.activeself;
}
function util_create_card_$_setSelect($bhv, $onoff)
{
    var $frgo = $bhv.frgo;
    $frgo.transform.find("chg").gameobject.SetActive($onoff);
}
function util_create_card_$_setStrong($bhv, $onoff)
{
    var $frgo = $bhv.frgo;
    $frgo.transform.find("emp").gameobject.SetActive($onoff);
}
function util_create_card($mark, $num)
{
    var $go   = new GameObject("card_" + $mark + $num);
    var $frgo = util_create_cardfront_obj($mark,$num);
    $frgo.transform.parent = $go.transform;
    var $bkgo = util_create_cardback_obj();
    $bkgo.transform.parent = $go.transform;
    $bkgo.transform.localPosition = Vector3.front * 2;
    
    var $gobc = $go.AddComponent(typeof(BoxCollider));
    $gobc.size = new Vector3(80,100,1);

    var $bhv = AddBehaviour($go);
    $bhv.AddMsgFunc("CLICK",util_create_card_$_clicked_dummy);
    
    var $ht = Hashtable();
    $ht.go   = $go;
    $ht.frgo = $frgo;
    $ht.bkgo = $bkgo;
    $ht.bhv  = $bhv;
    
    
    $ht.setClickFunc = util_create_card_$_set_clickfunc;
    $ht.flip         = util_create_card_$_flip;
    $ht.setSelect    = util_create_card_$_setSelect;
    $ht.setStrong    = util_create_card_$_setStrong;
    $ht.getFlip      = util_create_card_$_getFlip;
    
    return $ht;
}

/* 単体試験
var $butman = util_buttonManager();
var $ht = util_create_card("c",13);
$ht.flip(true);
$ht.flip(false);
$ht.flip(true);
$ht.SetStrong(true);
$ht.SetSelect(true);
*/
