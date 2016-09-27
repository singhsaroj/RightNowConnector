using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RightNow.AddIns.AddInViews;

namespace Oracle.RightNow.Cti.MediaBar {
    [Export(typeof(IMediaBarProvider))]
    public class MediaBarProvider : IMediaBarProvider {
        public Control GetView(IGDWContext context) {
            return ElementHost;
        }

        [Import]
        public MediaBarElementHost ElementHost { get; set; }

    }
}
