using System;
using System.AddIn;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Oracle.RightNow.Cti.MediaBar;
using Oracle.RightNow.Cti.Properties;
using RightNow.AddIns.AddInViews;
using RightNow.AddIns.Common;
using Oracle.RightNow.Cti.Common;

namespace Oracle.RightNow.Cti.AddIn {
    [AddIn("Oracle RightNow Media Bar", Publisher = "Oracle Corporation", Version = "1.0.0.0")]
    public class MediaBarAddIn : IGlobalDockWindowControl {
        private IGDWContext _dockWindowContext;
        private CompositionContainer _container;
        
        public static string PrimaryEngine, SecondaryEngine;

        public bool Initialize(IGlobalContext context) {
            GlobalContext = context;
            Global.Context = context;            
            PrimaryEngine = this.PrimaryCTIEngine;
            SecondaryEngine = this.SecondaryCTIEngine;
            Logger.Logger.Configure(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return true;
        }
  
        private void initializeContainer() {
            var baseCatalog = new AggregateCatalog();
            var info = Directory.GetParent(Path.GetDirectoryName(typeof(MediaBarAddIn).Assembly.Location));

            addDirectoryToCatalog(info.Parent, baseCatalog);
            
            _container = new CompositionContainer(baseCatalog);
            try {
                _container.ComposeParts(this);
            }
            catch (CompositionException exc) {
                GlobalContext.LogMessage(exc.ToString());
                Logger.Logger.Log.Error("Composition Exception in Initial Condition:" + exc);
                throw;
            }
        }

        private void addDirectoryToCatalog(DirectoryInfo directoryInfo, AggregateCatalog catalog) {
            catalog.Catalogs.Add(new DirectoryCatalog(directoryInfo.FullName));

            foreach (var directory in directoryInfo.GetDirectories()) {
                addDirectoryToCatalog(directory, catalog);
            }
        }

        public Control GetControl() {
            if (_container == null) {
                initializeContainer();
                Logger.Logger.LogLevel = "ALL";
                
            }
            return MediaBar.GetView(_dockWindowContext);
            //return null;
        }

        public void SetGDWContext(IGDWContext context){
            _dockWindowContext = context;
            _dockWindowContext.Docking = DockingType.Top;
            _dockWindowContext.Title = Resources.MediaBarTitle;
        }

        public void ShortcutActivated() {
        }

        [Export]
        public IGlobalContext GlobalContext { get; set; }

        [Import]
        public IMediaBarProvider MediaBar { get; set; }

        [Import]
        public IClickToDialProvider ClickToDialProvider { get; set; }


        public string GroupName {
            get {
                return Resources.MediaBarGroupName;
            }
        }

        public int Order {
            get {
                return 0;
            }
        }

        public Keys Shortcut {
            get {
                return Keys.None;
            }
        }

        [ServerConfigProperty(DefaultValue = "wss://AESSFCONNECTOR.serlab.int:8090")]
        public string PrimaryCTIEngine { get; set; }

        [ServerConfigProperty(DefaultValue = "wss://AESSFCONNECTOR.serlab.int:8090")]
        public string SecondaryCTIEngine { get; set; }
    }
}