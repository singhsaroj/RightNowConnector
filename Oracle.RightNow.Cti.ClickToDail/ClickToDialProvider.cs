using Oracle.RightNow.Cti.AddIn;
using RightNow.AddIns.AddInViews;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace Oracle.RightNow.Cti.ClickToDail
{
    [Export(typeof(IClickToDialProvider))]
    public class ClickToDialProvider : IClickToDialProvider
    {
        public ClickToDialProvider()
        { 
        }

        public System.Windows.Forms.Control GetControl(bool inDesignMode, IRecordContext recordContext)
        {
            try
            {
                Logger.Logger.Log.Debug("Click To Dial GetControl...");

                HostControl = new ClickToDialHost(inDesignMode, recordContext);
                return HostControl;
            }
            catch (Exception ex)
            {
               Logger.Logger.Log.Error("Click To Dial GetControl failed", ex);
                return null;
            }
        }

        public ClickToDialHost HostControl { get; set; }
    }
}
