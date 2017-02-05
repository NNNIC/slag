/*
    test93 ストップウォッチ
     
     1. 12秒系計と4秒計
　　　
　　 2. ボタン２つ。
     
[START] [RESET]
      __
    /  0 \
   /      \　　＜－－真ん中にもう一つの
   |    15|   
   \      /
    \ 30 /
      ~~
*/

var $m_watch = new GameObject("Watch");

function $_INIT($sm,$bFirst)
{
    $sm.Goto($_CREATE_BIG_FRAME);
}

var $m_big_frame;
var $m_big_hand;

function $_CREATE_BIG_FRAME($sm,$bFirst)
{
    $m_big_frame = Create_big_frame();
    $m_big_hand  = Create_big_hand($m_big_frame);
    
    $m_big_frame.transform.parent = $m_watch.transform;
    
    $sm.Goto($_CREATE_MINI_FRAME);
}

var $m_mini_frame;
var $m_mini_hand;

function $_CREATE_MINI_FRAME($sm,$bFirst)
{
    $m_mini_frame = Create_mini_frame();
    $m_mini_hand  = Create_mini_hand($m_mini_frame);
    
    $m_mini_frame.transform.localPosition += Vector3.back * 0.01;
    
    $m_mini_frame.transform.parent = $m_watch.transform;

    $sm.Goto($_TRANSFORM);
}


function $_TRANSFORM($sm,$bFirst)
{
    if ($bFirst)
    {
        $m_watch.transform.localScale = Vector3.one * 65;
        $sm.Goto($_SETBUTTONS);
    }
}

var $butman;
var $startstop_button;
var $reset_button;
function $_SETBUTTONS($sm,$bFirst)
{
    if ($bFirst)
    {
        $butman = ButtonManager();
        $startstop_button = Create_start_stop_button($butman);
        $reset_button = Create_reset_button($butman);
        $sm.Goto($_TIMERSTART);
    }
}

var $m_elapsed;
function $_TIMERSTART($sm,$bFirst)
{
    if ($bFirst)
    {
        $m_elapsed = 0;
        return;
    }
    
    if ($startstop_button.mode != "STOP")
    {
        $m_elapsed += Time.deltaTime;
    }
    if ($reset_button.mode == "RESET")
    {
        $reset_button.mode = "";
        $m_elapsed = 0;
    }
    
    var $angle_big = (360/4) * $m_elapsed;
    
    $m_big_hand.transform.localEulerAngles = Vector3.back * $angle_big;
    
    var $angle_mini =  (360/12) * Mathf.Floor($m_elapsed);
    $m_mini_hand.transform.localEulerAngles = Vector3.back * $angle_mini;
}

var $m_sm = StateManager();
$m_sm.Goto($_INIT);


//---

//var $btn = CreateButton(1,1,Color.white);
//$btn.transform.localPosition += Vector3.up * 4.2;

//var $butman = ButtonManager();
//var $startstop_button = Create_start_stop_button($butman);

/*
function $_ClickFunc($ht)
{
    PrintLn("Clicked!");
}

var $butman = ButtonManager();

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



/*
function $_UpdateCheckTouch($go)
{
    if (!Input.GetMouseButtonDown(0)) {return;}
    
    var $pos = Input.mousePosition;
    //Dump($pos);
    var $hitgo = GetObjectAtScreenPoint($pos);
    
    if ($hitgo!=null)
    {
        //Dump($hitgo);
    }
}

var $m_bhv = AddBehaviour();
$m_bhv.m_updateFunc = $_UpdateCheckTouch;
*/