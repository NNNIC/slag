using System.Collections;
using System.Collections.Generic;
using System;

namespace slagtool.runtime
{ 
    public class runtime_sub_missings {

        public virtual object Construct(Type type, object[] parameters)
        {
            return null;
        }

        public virtual object Method(object predecessor, string name, object[] parameters)
        {
            return null;
        }
    
        public virtual object Member(object predecessor, string name, object[] parameters, object value)
        {
            return null;
        }
    }
}
