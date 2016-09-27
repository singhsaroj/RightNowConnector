using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RightNow.AddIns.AddInViews;

namespace Oracle.RightNow.Cti.MediaBar {
    public interface IMediaBarProvider {
        Control GetView(IGDWContext context);
    }
}
