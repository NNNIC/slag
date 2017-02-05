"using UnityEngine";

function util_ChangeColor($go,$col)
{
    //PrintLn($col);
    var $r = $go.GetComponent(typeof(Renderer));
    $r.material.SetColor("_Color", $col);
}