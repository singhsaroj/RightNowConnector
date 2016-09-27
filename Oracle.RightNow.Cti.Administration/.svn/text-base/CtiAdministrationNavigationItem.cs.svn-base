using System;
using System.AddIn;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Drawing;
using System.Linq;
using Oracle.RightNow.Cti.Administration.Properties;
using RightNow.AddIns.AddInViews;

namespace Oracle.RightNow.Cti.Administration {
    [AddIn("CTI Administration Navigation Item", Publisher = "Oracle Corporation", Version = "1.0.0.0")]
    public class CtiAdministrationNavigationItem : INavigationItem {
        private IGlobalContext _globalContext;
        private CompositionContainer _container;

        [Import]
        public ICtiAdministrationContentPane AdministrationContentPane { get; set; }

        [Export]
        public IGlobalContext GlobalContext {
            get {
                return _globalContext;
            }
        }

        public bool Initialize(IGlobalContext context) {
            _globalContext = context;
            
            var baseCatalog = new AggregateCatalog();
            baseCatalog.Catalogs.Add(new AssemblyCatalog(typeof(CtiAdministrationNavigationItem).Assembly));

            _container = new CompositionContainer(baseCatalog);
            try {
                _container.ComposeParts(this);
                return true;
            }
            catch (CompositionException exc) {
                context.LogMessage(exc.ToString());
                return false;
            }
        }

        public void Activate() {
            _globalContext.AutomationContext.OpenEditor(AdministrationContentPane);
        }

        public string Text {
            get {
                return "CTI Administration";
            }
        }

        public Image Image16 {
            get {
                return Resources.AdministrationIcon;
            }
        }
    }
}