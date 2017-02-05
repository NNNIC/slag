using UnityEngine;
using System.Collections;
using slagtool;

/*
    Unity 依存　計算
*/

public class slagunity_builtincalc_op {

    public static object Calc_op(object a, object b, string op)
    {
        // a:vector3
        if (a.GetType()==typeof(Vector3) && (b.GetType()==typeof(Vector3) || b.GetType()==typeof(Vector2)) )
        {
            var v0 = (Vector3)a;
            var v1 = (Vector3)b;
            switch(op)
            {
                case "+": return v0 + v1;
                case "-": return v0 - v1;
                default: slagtool.runtime.util._error("unexpected"); break;
            }
        }
        if (a.GetType()==typeof(Vector3) && slagtool.runtime.util.IsNumeric(b.GetType()))
        {
            var v0 = (Vector3)a;
            var v1 = (float)slagtool.runtime.util.ToNumber(b);
            switch(op)
            {
                case "*": return v0 * v1;
                case "/": return v0 / v1;
                default: slagtool.runtime.util._error("unexpected"); break;
            }
        }
        //a:Vector2
        if (a.GetType()==typeof(Vector2) && (b.GetType()==typeof(Vector3) || b.GetType()==typeof(Vector2)) )
        {
            var v0 = (Vector2)a;
            var v1 = (Vector2)b;
            switch(op)
            {
                case "+": return v0 + v1;
                case "-": return v0 - v1;
                default: slagtool.runtime.util._error("unexpected"); break;
            }
        }
        if (a.GetType()==typeof(Vector2) && slagtool.runtime.util.IsNumeric(b.GetType()))
        {
            var v0 = (Vector2)a;
            var v1 = (float)slagtool.runtime.util.ToNumber(b);
            switch(op)
            {
                case "*": return v0 * v1;
                case "/": return v0 / v1;
                default: slagtool.runtime.util._error("unexpected"); break;
            }
        }

        return null;
    }	
}
