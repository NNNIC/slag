/*
    TEST93 Sub
*/
function _create_circle_notch($go,$radius,$num_of_div,$scale,$col)
{
    var $angle_div = 360 / $num_of_div;
    var $angle     = $angle_div * Mathf.Deg2Rad;
    
    for(var $n = 0; $n<$num_of_div; $n++)
    {
        var $notch = CreateNotch($scale);
        $notch.transform.parent = $go.transform;
        
        var $a = $angle * $n;
        var $x = Mathf.Cos($a);
        var $y = Mathf.Sin($a);
        
        $notch.transform.localPosition = new Vector3($x*$radius,$y*$radius,0);
        $notch.transform.eulerAngles   = Vector3.forward * (90 + $angle_div*$n);
        
        util_ChangeColor($notch,$col);
    }
}
function _create_circle_text($go, $strlist, $radius, $scale, $zpos, $col)
{
    var $num_of_div = $strlist.Count;
    var $angle_div  = 360 / $num_of_div;
    var $angle      = $angle_div * Mathf.Deg2Rad;

    for(var $n = 0; $n<$num_of_div; $n++)
    {
        var $obj = CreateTxtObj($strlist[$n],$scale);
        $obj.transform.parent = $go.transform;
        
        var $a = - $angle * $n + 90*Mathf.Deg2Rad;
        //PrintLn($a);
        var $x = Mathf.Cos($a);
        var $y = Mathf.Sin($a);
        
        $obj.transform.localPosition = new Vector3($x*$radius,$y*$radius,$zpos);
    }
}


// ####################
// # CREATE BIG FRAME #
function Create_big_frame()
{
	var $frame = CreateCircle(3.5,80,true);
	util_ChangeColor($frame,new Color(164/255,164/255,164/255,1));

    _create_circle_notch($frame,3,4,new      Vector3(0.05,0.1,0.1),   Color.red);
    _create_circle_notch($frame,3.025,8,new  Vector3(0.05,0.075,0.05),Color.white);
    _create_circle_notch($frame,3.05,8*5,new Vector3(0.05,0.075,0.05),Color.black);
    
    _create_circle_text($frame,["0.0","0.5","1.0","1.5","2.0","2.5","3.0","3.5"],2.7,0.05,-0.01,Color.white);
    
    return $frame;
}
function Create_big_hand($go)
{
    var $hand                    = CreateHand(0.02,3.2,Color.red);
    $hand.transform.parent       = $go.transform;
    $hand.transform.localPosition= Vector3.forward * (-0.15);
    
    return $hand;
}
// # CREATE BIG FRAME #
// ####################

// #####################
// # CREATE MINI FRAME #
function Create_mini_frame()
{
    var $frame = CreateCircle(1.2,40,true);
    util_ChangeColor($frame,new Color(94/255,94/255,94/255,1));
    
    _create_circle_notch($frame,1,12,new Vector3(0.02,0.02,0.1),Color.white);
    
    _create_circle_text($frame,["0","1","2","3","4","5","6","7","8","9","10","11"],0.8,0.04,-0.01,Color.white);
    
    return $frame;
}
function Create_mini_hand($go)
{
    var $hand = CreateHand(0.02,1.1,Color.red);
    $hand.transform.parent =  $go.transform;
    $hand.transform.localPosition= Vector3.forward * (-0.15);
    
    return $hand;
}
// # CREATE MINI FRAME #
// #####################

// ############################
// # CREATE START/STOP BUTTON #

function $_StartStopClickFunc($ht)
{
    //PrintLn("Start/Stop Button");
    
    if ($ht.mode == "START")
    {
        $ht = $_StartStopClick_mode_STOP($ht);
    }
    else if ($ht.mode == "STOP")
    {
        $ht = $_StartStopClick_mode_START($ht);
    }
}
function $_StartStopClick_mode_START($ht)
{
    $ht.on_col      = Color.green;
    $ht.on_text     = "stop";
    $ht.on_text_col = Color.white;

    $ht.off_col     = Color.red;
    $ht.off_text    = "STOP";
    $ht.off_text_col= Color.black;
    
    $ht.mode = "START";
    return $ht;
}
function $_StartStopClick_mode_STOP($ht)
{
    $ht.on_col      = Color.green;
    $ht.on_text     = "start";
    $ht.on_text_col = Color.white;

    $ht.off_col     = Color.red;
    $ht.off_text    = "START";
    $ht.off_text_col= Color.black;
    
    $ht.mode = "STOP";
    return $ht;
}

function Create_start_stop_button($butman)
{
    var $ht = hashtable();

    $ht.width       = 150;
    $ht.height      = 50;
    $ht.clickFunc   = $_StartStopClickFunc;

    $ht = $_StartStopClick_mode_START($ht);
    
    var $but = CreateButton($butman, "START-STOP-Button", $ht);  //$butman:ボタンマネージャ

    $but.go.transform.localPosition = new Vector3(-115,275,0);
    
    return $but;
}

function $_ResetClickFunc($ht)
{
    $ht.mode = "RESET";
    //Dump($ht);
    //PrintLn("RESET CLIKED!");
}
function Create_reset_button($butman)
{
    var $ht = hashtable();

    $ht.width       = 150;
    $ht.height      = 50;
    $ht.clickFunc   = $_ResetClickFunc;

    $ht.on_col      = Color.green;
    $ht.on_text     = "reset";
    $ht.on_text_col = Color.white;

    $ht.off_col     = Color.red;
    $ht.off_text    = "RESET";
    $ht.off_text_col= Color.black;
    
    var $but = CreateButton($butman, "RESET-Button", $ht);  //$butman:ボタンマネージャ $id;テキスト

    $but.go.transform.localPosition = new Vector3(115,275,0);
    
    return $but;
}
// # CREATE START/STOP BUTTON #
// ############################


