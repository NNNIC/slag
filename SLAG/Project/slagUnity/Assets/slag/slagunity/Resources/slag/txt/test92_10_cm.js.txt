// カードマネージャ

var HEART="heart";
var DAIA ="diamond";
var CLAB ="clab";
var SPAD ="spade";

function cm_create_stock_get_mark($i)
{
    switch($i)
    {
    case 0: return HEART;
    case 1: return DAIA;
    case 2: return CLAB;
    case 3: return SPAD;
    }
}
function cm_create_stock()
{
    var $stock = [];
    for(var $i = 0; $i<4; $i++)
    {
        var $mark = cm_create_stock_get_mark($i);
        for(var $j = 1; $j<=13; $j++)
        {
            var $cd = [$mark,$j];
            $stock.Add($cd);
        }
    }
    return $stock;
}
function cm_shuffle($stock)
{
    //Dump($stock);

    var tmp = [];
    while($stock.count>0)
    {
        var n = Cast(int, UnityEngine.Random.Range(0,$stock.count-1));
        tmp.Add($stock[n]);
        $stock.RemoveAt(n);
    }
    for(var $i=0;$i<tmp.Count;$i++) $stock.Add(tmp[$i]);
    
    //Dump($stock);
}
function cm_get_fivecards($stock)
{
    var $hand = [];
    
    for(var $i = 0; $i < 5; $i++)
    {
        $hand.Add($stock[0]);
        $stock.RemoveAt(0);
    }
    return $hand;
}
function cm_get_onecard($stock)
{
    var $card = $stock[0];
    $stock.RemoveAt(0);
    return $card;
}

//役を求める
function cm_get_result__sort_by_num($h)
{
    for(var $i = 0; $i<5; $i++)
    {
        for(var $j=0;$j<5;$j++)
        {
            if ($i==$j) continue;
            
            var a = $h[$i];
            var b = $h[$j];
            if (a[1] < b[1])
            {
                $h[$i] = b;
                $h[$j] = a;
            }
        }
    }
    //確認
    //Dump($h);
    
    return $h;
}
function cm_get_result__has_onepair($h)
{
    var c1 = $h[0]; var n1 = c1[1];
    var c2 = $h[1]; var n2 = c2[1];
    var c3 = $h[2]; var n3 = c3[1];
    var c4 = $h[3]; var n4 = c4[1];
    var c5 = $h[4]; var n5 = c5[1];
    
    if (n1==n2 && n2!=n3 && n3!=n4 && n4!=n5) {
	    return true;
	} //AABCD
	if (n1!=n2 && n2==n3 && n3!=n4 && n4!=n5) {
	    return true;
    } //ABBCD
	if (n1!=n2 && n2!=n3 && n3==n4 && n4!=n5) {
	    return true;
	} //ABCCD
	if (n1!=n2 && n2!=n3 && n3!=n4 && n4==n5) {
	    return true;
	} //ABCDD

	return false;
}
function cm_get_result__has_twopair($h)
{
    var c1 = $h[0]; var n1 = c1[1];
    var c2 = $h[1]; var n2 = c2[1];
    var c3 = $h[2]; var n3 = c3[1];
    var c4 = $h[3]; var n4 = c4[1];
    var c5 = $h[4]; var n5 = c5[1];

	if (n1==n2 && n3==n4 && n1!=n3 && n4!=n5) {return true;} // A,A,B,B,C
	if (n1==n2 && n4==n5 && n1!=n3 && n3!=n4) {return true;} // A,A,B,C,C
	if (n2==n3 && n4==n5 && n1!=n2 && n2!=n4) {return true;} // A,B,B,C,C
	return false;
}
function cm_get_result__has_threecards($h)
{
    var c1 = $h[0]; var n1 = c1[1];
    var c2 = $h[1]; var n2 = c2[1];
    var c3 = $h[2]; var n3 = c3[1];
    var c4 = $h[3]; var n4 = c4[1];
    var c5 = $h[4]; var n5 = c5[1];

    if (n1==n2 && n2==n3 && n3!=n4 && n4!=n5) {return true;} // A,A,A,B,C
	if (n1!=n2 && n2==n3 && n3==n4 && n4!=n5) {return true;} // A,B,B,B,C
	if (n1!=n2 && n2!=n3 && n3==n4 && n4==n5) {return true;} // A,B,C,C,C
	return false;
}
function cm_get_result__has_fourcards($h)
{
    var c1 = $h[0]; var n1 = c1[1];
    var c2 = $h[1]; var n2 = c2[1];
    var c3 = $h[2]; var n3 = c3[1];
    var c4 = $h[3]; var n4 = c4[1];
    var c5 = $h[4]; var n5 = c5[1];

	if (n1==n2 && n1==n3 && n1==n4 && n1!=n5) {return true;}
	if (n1!=n2 && n2==n3 && n2==n4 && n2==n5) {return true;}
	return false;
}
function cm_get_result__has_fullhouse($h)
{
    var c1 = $h[0]; var n1 = c1[1];
    var c2 = $h[1]; var n2 = c2[1];
    var c3 = $h[2]; var n3 = c3[1];
    var c4 = $h[3]; var n4 = c4[1];
    var c5 = $h[4]; var n5 = c5[1];

    if (n1==n2 && n3==n4 && n4==n5 && n1!=n3) {return true;}    //AABBB
    if (n1==n2 && n1==n3 && n4==n5 && n1!=n4) {return true;}    //AAABB
	return false;
}
function cm_get_result__has_straight($h)
{
    var c1 = $h[0]; var n1 = c1[1];
    var c2 = $h[1]; var n2 = c2[1];  
    var c3 = $h[2]; var n3 = c3[1];
    var c4 = $h[3]; var n4 = c4[1];
    var c5 = $h[4]; var n5 = c5[1];
    
    if (n5-n4==1 && n4-n3==1 && n3-n2==1 && n2-n1==1) return true;
    
    //1XJQK,12JQK,123QK,1234K -- circulations
    if (n1==1 && n2==10 && n3==11 && n4==12 && n5==13) return true;
    if (n1==1 && n2==2  && n3==11 && n4==12 && n5==13) return true;
    if (n1==1 && n2==2  && n3==3  && n4==12 && n5==13) return true;
    if (n1==1 && n2==2  && n3==3  && n4==4  && n5==13) return true;
    
    return false;
}
function cm_get_result__has_flush($h)
{
    var c1 = $h[0]; var m1 = c1[0];
    var c2 = $h[1]; var m2 = c2[0];  
    var c3 = $h[2]; var m3 = c3[0];
    var c4 = $h[3]; var m4 = c4[0];
    var c5 = $h[4]; var m5 = c5[0];
    
    return (m1==m2 && m2==m3 && m3==m4 && m4==m5);
}

function cm_get_result($hands)
{
    var $sortByNum = cm_get_result__sort_by_num($hands);     //ナンバーでソート
    if (cm_get_result__has_onepair($sortByNum)) //ワンペアのみ？
    {
        return "ONEPAIR";
    }
    if (cm_get_result__has_twopair($sortByNum)) //ツーペアのみ？
    {
        return "TWOPAIR";
    }
    if (cm_get_result__has_threecards($sortByNum)) //スリーカード
    {
        return "THREECARDS";
    }
    if (cm_get_result__has_fourcards($sortByNum)) //フォーカード
    {
        return "FOURCARDS";
    }
    if (cm_get_result__has_fullhouse($sortByNum)) //フルハウス
    {
        return "FULLHOUSE";
    }
    var bStraight = cm_get_result__has_straight($sortByNum); //ストレート
    var bFlush    = cm_get_result__has_flush($sortByNum);    //フラッシュ
    
    if (bStraight && bFlush)
    {
        return "STRAIGHTFLASH";
    }
    if (bStraight)
    {
        return "STRAIGHT";
    }
    if (bFlush)
    {
        return "FLUSH";
    }
    
    return "NONE";
}
function cm_emphasis($hand,$htlist)
{
    if ($hand=="ONEPAIR" || $hand=="TWOPAIR" || $hand=="THREECARDS" || $hand=="FOURCARDS" )
    {
        for(var i = 0; i<4; i++)
        {
            for(var j = i+1; j<5; j++)
            {
                var a = $htlist[i];
                var b = $htlist[j];
                if (a.num==b.num)
                {
                    a.SetStrong(true);
                    b.SetStrong(true);
                }
            }
        }
        return;
    }
    if ($hand!="NONE")
    {
        for(var i=0;i<5;i++)
        {
            //$htlist[i].SetStrong(true);
            var $a = $htlist[i];
            $a.SetStrong(true);
        }
        return;
    }
}
