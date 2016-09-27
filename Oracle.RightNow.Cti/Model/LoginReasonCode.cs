using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oracle.RightNow.Cti.Model
{
    public enum LoginReasonCode
    {
        Aux = 1,
        AfterCall = 2,
        AutoIn = 3,
        ManualIn = 4,      
    }

    public enum LoginMode
    {
        ACD = 1,
        NonACD = 2,
        Split = 3
    }
}
