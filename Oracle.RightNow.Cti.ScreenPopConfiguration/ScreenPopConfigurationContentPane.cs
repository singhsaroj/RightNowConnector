using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using RightNow.AddIns.AddInViews;
using System.Windows.Forms.Integration;
using Oracle.RightNow.Cti.ScreenPopConfiguration.View;
using Oracle.RightNow.Cti.ScreenPopConfiguration.Properties;

namespace Oracle.RightNow.Cti.ScreenPopConfiguration
{
    //[Export(typeof(IScreenPopConfigurationContentPane))]
    public class ScreenPopConfigurationContentPane : IContentPaneControl//IScreenPopConfigurationContentPane
    {
        private const string UniqueId = "0864BB57-C9FD-40FC-89FF-C38726138EA7";

        private Control _panelControl;

        public ScreenPopConfigurationContentPane()
        { 
        }

        //[Import]
        public ScreenPopConfigurationView ConfigurationView { get; set; }

        //[Import]
        public IGlobalContext GlobalContext { get; set; }

        public bool BeforeClose()
        {
            return true;
        }

        public void Closed()
        {
        }
        public System.Drawing.Image Image16
        {
            get { return Resources.administration_phone; }
        }

        public IList<IEditorRibbonButton> RibbonButtons
        {
            get { return new List<IEditorRibbonButton>(); }
        }

        public string Text
        {
            get { return Resources.CtiScreenPopContentPaneText ; }
        }

        public string UniqueID
        {
            get { return UniqueId; }
        }

        public System.Windows.Forms.Control GetControl()
        {
            if (_panelControl == null)
            {
                var elementHost = new ElementHost();
                ConfigurationView = new ScreenPopConfigurationView();
                elementHost.Child = ConfigurationView;
                elementHost.Dock = DockStyle.Fill;
                _panelControl = elementHost;
            }
            return _panelControl;
        }
    }
}
