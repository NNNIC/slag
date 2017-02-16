/*
 Test 12
*/

function S_INIT(sm,bFirst)
{
    if (bFirst)
    {
        PrintLn("S_INIT");
        sm.Goto(S_SECOND);
    }
}

function S_SECOND(sm,bFirst)
{
    if (bFirst)
    {
        PrintLn("S_SECOND");
        sm.Goto(S_END);
    }
}

function S_END(sm,bFirst)
{
}

var m_sm = StateManager();
m_sm.Goto(S_INIT);

//PrintLn("S_INIT="+ S_INIT.GetFunctionName());
