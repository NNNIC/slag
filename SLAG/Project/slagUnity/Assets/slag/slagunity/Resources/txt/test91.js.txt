//
// TEST 91
//
// 数字当てゲーム
// Guess What number is.
//
// 3桁の数字を当ててください。


var MAXTRY = 20;
var NUM = 0;

var s_number = 0;
var s_try    = 0;
var s_guess  = null;


function S_Q_START(sm,bFirst)
{
    if (bFirst) {
        PrintLn("*START*");

        NUM = NUM + 1;

        while (true) {
            s_number = UnityEngine.Mathf.Floor(UnityEngine.Random.Range(100, 1000));
            var n3 = s_number % 10;
            var n2 = UnityEngine.Mathf.Floor(s_number / 10) % 10;
            var n1 = UnityEngine.Mathf.Floor(s_number / 100);

            //PrintLn(s_number);

            if (n1!=n2 && n1!=n3 && n2!=n3)
            {
                break;
            }
        }

        Print("\n\n\n");
        Print("-----------------------------\n");
        Print("  !!   #  問題　＃  !! \n");
        Print("-----------------------------\n");

        sm.WaitTime(0.5);
    }
    else
    {
        Print("3桁の数字を当ててください。\n");
        Print("それぞれの桁は異なる数字です。\n\n");

        s_try = 0;

        sm.Goto(S_Q_TRY);
    }
}

function S_Q_TRY(sm,bFirst)
{
    if (bFirst)
    {
        if (s_try > MAXTRY)
        {
            Print("試行回数が規定を超えました!!\n");
            StateGoto("S_Q_END");
            return;
        }
        Print("### 試行 " + (s_try + 1) + " ###\n");

        s_try++;

        s_guess = null;
        
        sm.Goto(S_Q_INPUT);
    }
}

function S_Q_INPUT(sm,bFirst)
{
    if (bFirst)
    {
        ReadLineStart("３桁の数字を入力してください");
    }
    else
    {
        var s = ReadLineDone();
        if (s != null) {
            var n = ToNumber(s);
            if (n < 100 || n > 999) {
                Print("入力が正しくありません\n");
                sm.Goto(S_Q_INPUT);
            }
            else {
                s_guess = n;
                sm.Goto(S_Q_CHECK);
            }
        }
    }
}

function S_Q_CHECK(sm,bFirst)
{
    if (bFirst)
    {
        if (s_guess == s_number) { 
            m_sm.Goto(S_Q_CONGRATULATION);
            return;
        }
        else if (s_guess > s_number)
        {
            Print("\n\n...答えは、" + s_guess + "より小さい数です\n\n");
        }
        else //if (s_guess < s_number)
        {
            Print("\n\n...答えは" + s_guess + "より大きい数です\n\n");
        }
        sm.WaitTime(1);
    }
    else
    {
        var n3 = s_guess % 10;
        var n2 = UnityEngine.Mathf.Floor(s_guess / 10) % 10;
        var n1 = UnityEngine.Mathf.Floor(s_guess / 100);

        //PrintLn("s_guess.split=" + n1 + n2 + n3);

        var nm3 = s_number % 10;
        var nm2 = UnityEngine.Mathf.Floor(s_number / 10) % 10;
        var nm1 = UnityEngine.Mathf.Floor(s_number / 100);

        //PrintLn("s_number.split=" + nm1 + nm2 + nm3 + "\n");

        var rightplace = 0;
        if (n3 == nm3) { rightplace = rightplace + 1; }
        if (n2 == nm2) { rightplace = rightplace + 1; }
        if (n1 == nm1) { rightplace = rightplace + 1; }

        var anyplace = 0;
        if (n1 == nm2 || n1 == nm3) { anyplace = anyplace + 1; }
        if (n2 == nm1 || n2 == nm3) { anyplace = anyplace + 1; }
        if (n3 == nm2 || n3 == nm1) { anyplace = anyplace + 1; }

        Print("数字と場所が一致       : " + rightplace + "個\n\n");
        Print("異なる場所の数字と一致 : " + anyplace + "個\n\n\n\n");

        sm.WaitTime(1);
        sm.Goto(S_Q_TRY);
    }
}

function S_Q_END(sm,bFirst)
{
    if (bFirst)
    {
        PrintLn("*ゲーム終了*");
    }
}

function S_Q_CONGRATULATION(sm,bFirst)
{
    if (bFirst) {
        Print("\n");
        Print("******************************************************\n");
        Print("... 祝　当たりです。答えは " + s_number + " です\n");
        Print("******************************************************\n\n");

        m_sm.Goto(S_Q_END);
    }
}

var m_sm = StateManager();
m_sm.Goto(S_Q_START);

