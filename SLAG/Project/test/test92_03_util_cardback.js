/*
   カード背景作成
   
   <-  80 ->
 　+-------+---
   |  ♡    |  ^
   |  A    |  | 100
   |       |  v
   +-------+---
*/
var util_create_cardback_texture_$tex = null;
function util_create_cardback_texture()
{
    if (util_create_cardback_texture_$tex!=null) return util_create_cardback_texture_$tex;

    var $w = 256;
    var $h = 256;
    
    var $tex = new Texture2D($w,$h,TextureFormat.RGBA32,false);
    
    //参照
    var $reftex   = Resources.Load("2d/snow");
    var $refpixels = $reftex.GetPixels(8,8,16,16,0);
    var $whitepixels = new Color[256];
    for(var $i=0; $i < 256; $i++) $whitepixels[$i]=Color.white;
    
    for(var $x = 0; $x < $w; $x+=16) for(var $y=0;$y<$h;$y+=16)
    {
        if ($x == 0 || $x == $w-16 || $y == 0 || $y == $h-16)
        {
            $tex.SetPixels($x,$y,16,16,$whitepixels,0);
        }
        else
        {
            $tex.SetPixels($x,$y,16,16,$refpixels,0);
        }
    }
    
    $tex.Apply(false,false);

    util_create_cardback_texture_$tex = $tex;
    
    return $tex;
}

function util_create_cardback_obj()
{
    var $go  = GameObject.CreatePrimitive(PrimitiveType.Quad);
    $go.name = "back";
    var $rdr = $go.GetComponent(typeof(Renderer));
    $rdr.material = new Material(Shader.Find("Unlit/Texture"));
    var $tex = util_create_cardback_texture();
    $rdr.material.SetTexture("_MainTex",$tex);
    $go.transform.localScale=new Vector3(80,100,1);
    
    var $cdr = $go.GetComponent(typeof(MeshCollider));
    UnityEngine.Object.DestroyImmediate($cdr);
    
    return $go;
}

//単体テスト
//var $go = util_create_cardback_obj();

