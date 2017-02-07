/*
 Test 54
 
 サークル作成

*/

"using UnityEngine";
"using System.Collections";

function Create_GameObject($d)
{
    var $go = new GameObject();
    var $mr = $go.AddComponent(typeof(MeshRenderer));
    $mr.material = new Material(Shader.Find("Standard"));

    var $mf = $go.AddComponent(typeof(MeshFilter));

    var $mesh = new Mesh();

    $mesh.vertices  = ToArray(Vector3,        $d.verts);
    $mesh.uv        = ToArray(Vector2,        $d.uvs);
    $mesh.normals   = ToArray(Vector3,        $d.nrms);
    $mesh.triangles = ToArray(int,            $d.trs);


    $mf.mesh = $mesh;

    return $go;
}


function Add_Vertex($d,$v0,$v1,$v2,$uv0,$uv1,$uv2)
{
    var $i0 = $d.size++;
    var $i1 = $d.size++;
    var $i2 = $d.size++;

    $d.verts.Add($v0);
    $d.verts.Add($v1);
    $d.verts.Add($v2);

    $d.uvs.Add($uv0);
    $d.uvs.Add($uv1);
    $d.uvs.Add($uv2);

    var $n = Vector3.Cross($v0 - $v1, $v1 - $v2);
    
    $d.nrms.Add($n);
    $d.nrms.Add($n);
    $d.nrms.Add($n);

    $d.trs.Add($i0);
    $d.trs.Add($i1);
    $d.trs.Add($i2);

    return $d;
}

function ToVector2From3($v)
{
    return new Vector2($v.x,$v.y);
}

function Circle_Create_test($radius,$num_of_div)
{
     var $step_angle = 360 / $num_of_div;
     var $d = new HashTable();
     $d.size   = 0;
     $d.verts  = [];
     $d.uvs    = [];
     $d.nrms   = [];
     $d.trs    = [];

     var $v0  = new Vector3(0, 0, 0);
     var $v1  = new Vector3(0, 1, 0);
     var $v2  = new Vector3(2, 0, 0);

     var $uv0 = new Vector2(0,   0  );
     var $uv1 = new Vector2(0,   0.1);
     var $uv2 = new Vector2(0.2, 0  );

     $d = Add_Vertex($d, $v0, $v1, $v2, $uv0, $uv1, $uv2);

     var $go = Create_GameObject($d);
     $go.name = "Circle";
}

function Circle_Create($radius,$num_of_div,$rev)
{
/*
    rev : reverse   true or false

*/
    var $d = new HashTable();
    $d.size   = 0;
    $d.verts  = [];
    $d.uvs    = [];
    $d.nrms   = [];
    $d.trs    = [];

    var $angle_deg = 360 / $num_of_div;
    var $angle = $angle_deg * Mathf.Deg2Rad;


    for(var $n = 0; $n<$num_of_div; $n++)
    {
         var $v0 = Vector3.zero;

         var $a1 = $angle * $n;
         var $x1 = Mathf.Cos($a1);
         var $y1 = Mathf.Sin($a1);
         var $v1 = new Vector3($x1,$y1,0);


         var $a2 = $angle * ($n+1);
         var $x2 = Mathf.Cos($a2);
         var $y2 = Mathf.Sin($a2);
         var $v2 = new Vector3($x2,$y2,0);

         var $uv0 = new Vector2(0.5,0.5); //UVは2次元で正方向1x1の大きさ
         var $uv1 = ToVector2From3($v1/2) + $uv0;
         var $uv2 = ToVector2From3($v2/2) + $uv0;


	     if ($radius != 1)
         {
             $v1 *= $radius;
             $v2 *= $radius;
         }


         if ($rev)
         {
  	        $d = Add_Vertex($d,$v2,$v1,$v0,$uv2,$uv1,$uv0);
         }
         else
         {
  	        $d = Add_Vertex($d,$v0,$v1,$v2,$uv0,$uv1,$uv2);
         }
    }
    
    var $go = Create_GameObject($d);
    $go.name = "Circle_div_"+$num_of_div;
    return $go;
}

var $rg = new GameObject("root");

var $g1 = Circle_Create(1, 3, true);
$g1.name = "#1:" + $g1.name;
$g1.transform.position = Vector3.up * 5 + Vector3.left * 2;
$g1.transform.parent   = $rg.transform;

var $g2 = Circle_Create(1, 4, true);
$g2.name = "#1:" + $g2.name;
$g2.transform.position = Vector3.up * 3 + Vector3.left * 2;
$g2.transform.parent   = $rg.transform;

var $g3 = Circle_Create(1, 5, true);
$g3.name = "#3:" + $g3.name;
$g3.transform.position = Vector3.up * 1 + Vector3.left * 2;
$g3.transform.parent   = $rg.transform;

var $g4 = Circle_Create(1, 6, true);
$g4.name = "#4:" + $g4.name;
$g4.transform.position = Vector3.up * (-1) + Vector3.left * 2;
$g4.transform.parent   = $rg.transform;

var $g5 = Circle_Create(1, 8, true);
$g5.name = "#5:" + $g5.name;
$g5.transform.position = Vector3.up * (-3) + Vector3.left * 2;
$g5.transform.parent   = $rg.transform;

var $g6 = Circle_Create(1, 20, true);
$g6.name = "#6:" + $g6.name;
$g6.transform.position = Vector3.up * (-5) + Vector3.left * 2;
$g6.transform.parent   = $rg.transform;

var $g7 = Circle_Create(2, 40, true);
$g7.name = "#7:" + $g7.name;
$g7.transform.position = Vector3.up * 0 + Vector3.left * (-1);
$g7.transform.parent   = $rg.transform;

$rg.transform.localScale = Vector3.one * 60;

