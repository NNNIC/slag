//
// test 08a
//



function S_START($sm,$bFirst)
{
    if ($bFirst)
    {
        PrintLn("== START ==");
        $sm.Goto(S_READ);
    }
}

function S_READ($sm,$bFirst)
{
    if ($bFirst)
    {
        ReadLineStart("Input>");
    }
    else
    {
        var $s = ReadLineDone();
        if ($s!=null)
        {
            $sm.Goto(S_END);
        }
    }
}

function S_END($sm,$bFirst)
{
    if ($bFirst)
    {
        PrintLn("== END ==");
    }
}

var $sm = StateManager();
$sm.Goto(S_START);

