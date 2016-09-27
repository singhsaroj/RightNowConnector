using RightNow.AddIns.AddInViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Oracle.RightNow.Cti.AddIn
{
    public interface IClickToDialProvider
    {
        Control GetControl(bool inDesignMode, IRecordContext recordContext);
    }
}
