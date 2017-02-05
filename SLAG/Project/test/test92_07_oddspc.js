/*

   オッズパネル
   
   +----------------------+
   | ONE PAIR         2   |
   | TWO PAIR         4   |
   | THREE CARDS      10  |
   | FULL HOUSE       50  |
   | FOUR CARDS       50  |
   | STRAIGHT         50  |
   | FLUSH            50  |
   | STRAIGHT FLUSH   100 |
   +----------------------+

*/
var HD_ONEPAIR   =  1;
var HD_TWOPAIR   =  2;
var HD_THREECARDS=  3;
var HD_FULLHOUSE =  4;
var HD_FOURCARDS =  5;
var HD_STRAIGHT  =  6;
var HD_FLUSH     =  7;
var HD_SF        =  8;

var ODDS_ONEPAIR   =   1;
var ODDS_TWOPAIR   =   4;
var ODDS_THREECARDS=  10;
var ODDS_FULLHOUSE = 100;
var ODDS_FOURCARDS = 200;
var ODDS_STRAIGHT  =  50;
var ODDS_FLUSH     = 100;
var ODDS_SF        = 400;

var oddspc_$ht = null;

function oddspc_createPanel__create_lineobj($num, $hand, $odds, $parentgo)
{
    var $ht = Hashtable();
    $ht.go      = new GameObject($num.ToString() + $hand);
    $ht.go.transform.parent = $parentgo.transform;
    $ht.handobj = util_create_msgobj($hand);
    $ht.oddsobj = util_create_msgobj($odds.ToString());
    $ht.odds    = $odds;
    
    //位置
    var $y = 350 - 30 * $num;
    
    $ht.handobj.go.transform.parent = $ht.go.transform;
    $ht.handobj.go.transform.localPosition = new Vector3(-85,$y,0);
    $ht.handobj.go.transform.localScale = Vector3.one * 0.7;
    
    $ht.oddsobj.go.transform.parent = $ht.go.transform;
    $ht.oddsobj.go.transform.localPosition = new Vector3(135,$y,0);
    $ht.oddsobj.go.transform.localScale = Vector3.one * 0.7;
    
    //色
    $ht.handobj.setcol(Color.white);
    $ht.handobj.setbgcol(Color.black);
    
    $ht.oddsobj.setcol(Color.white);
    $ht.oddsobj.setbgcol(Color.black);
    
    
    return $ht;
}
function oddspc_createPanel__set_credits($lineobj,$credits)
{
    var $ht   = $lineobj;
    var $odds = $ht.odds;
    $ht.gain = $odds * $credits;
    
    $ht.oddsobj.setmsg($ht.gain.ToString());
    $ht.oddsobj.setbgsize_reset(1.4);
}
function oddspc_createPanel__blink($lineobj,$onoff)
{
    var $ht   = $lineobj;
    $ht.handobj.blink($onoff);
    $ht.oddsobj.blink($onoff);
}

// public

function oddspc_createPanel()
{
    oddspc_$ht = Hashtable();
    oddspc_$ht.go = new GameObject("oddspc");
    oddspc_$ht.onepair       = oddspc_createPanel__create_lineobj(0,"ONE PAIR",       ODDS_ONEPAIR   , oddspc_$ht.go);
    oddspc_$ht.twopair       = oddspc_createPanel__create_lineobj(1,"TWO PAIR",       ODDS_TWOPAIR   , oddspc_$ht.go);
    oddspc_$ht.threecards    = oddspc_createPanel__create_lineobj(2,"THREE CARDS",    ODDS_THREECARDS, oddspc_$ht.go);
    oddspc_$ht.fourcards     = oddspc_createPanel__create_lineobj(3,"FOUR CARDS",     ODDS_FOURCARDS , oddspc_$ht.go);
    oddspc_$ht.fullhouse     = oddspc_createPanel__create_lineobj(4,"FULL HOUSE",     ODDS_FULLHOUSE , oddspc_$ht.go);
    oddspc_$ht.straight      = oddspc_createPanel__create_lineobj(5,"STRAIGHT",       ODDS_STRAIGHT  , oddspc_$ht.go);
    oddspc_$ht.flush         = oddspc_createPanel__create_lineobj(6,"FLUSH",          ODDS_FLUSH     , oddspc_$ht.go);
    oddspc_$ht.straightflush = oddspc_createPanel__create_lineobj(7,"STRAIGHT FLUSH", ODDS_SF        ,oddspc_$ht.go);
}

function oddspc_set_credits($credits)
{
    oddspc_createPanel__set_credits(oddspc_$ht.onepair,      $credits);
    oddspc_createPanel__set_credits(oddspc_$ht.twopair,      $credits);
    oddspc_createPanel__set_credits(oddspc_$ht.threecards,   $credits);
    oddspc_createPanel__set_credits(oddspc_$ht.fourcards,    $credits);
    oddspc_createPanel__set_credits(oddspc_$ht.fullhouse,    $credits);
    oddspc_createPanel__set_credits(oddspc_$ht.straight,     $credits);
    oddspc_createPanel__set_credits(oddspc_$ht.flush,        $credits);
    oddspc_createPanel__set_credits(oddspc_$ht.straightflush,$credits);
}

function oddspc_set_blink($num)
{
    switch($num)
    {
    case HD_ONEPAIR:    oddspc_createPanel__blink(oddspc_$ht.onepair,   true);   break;
    case HD_TWOPAIR:    oddspc_createPanel__blink(oddspc_$ht.twopair,   true);   break;
    case HD_THREECARDS: oddspc_createPanel__blink(oddspc_$ht.threecards,true);   break;
    case HD_FULLHOUSE:  oddspc_createPanel__blink(oddspc_$ht.fullhouse, true);   break;
    case HD_FOURCARDS:  oddspc_createPanel__blink(oddspc_$ht.fourcards, true);   break;
    case HD_STRAIGHT:   oddspc_createPanel__blink(oddspc_$ht.straight,  true);   break;
    case HD_FLUSH:      oddspc_createPanel__blink(oddspc_$ht.flush,     true);   break;
    case HD_SF:         oddspc_createPanel__blink(oddspc_$ht.straightflush,true);   break;
    }

    if ($num==null)
    {
        oddspc_createPanel__blink(oddspc_$ht.onepair,      false);         
        oddspc_createPanel__blink(oddspc_$ht.twopair,      false);         
        oddspc_createPanel__blink(oddspc_$ht.threecards,   false);         
        oddspc_createPanel__blink(oddspc_$ht.fullhouse,    false);         
        oddspc_createPanel__blink(oddspc_$ht.fourcards,    false);         
        oddspc_createPanel__blink(oddspc_$ht.straight,     false);         
        oddspc_createPanel__blink(oddspc_$ht.flush,        false);         
        oddspc_createPanel__blink(oddspc_$ht.straightflush,false);      
    }
}

// 単体試験
//oddspc_createPanel();
//oddspc_set_credits(2);
//oddspc_set_blink(HD_FLUSH);
//oddspc_set_blink(HD_FULLHOUSE);

