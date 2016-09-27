using System;
using System.AddIn;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Drawing;
using System.Linq;
using RightNow.AddIns.AddInViews;
using Oracle.RightNow.Cti.ScreenPopConfiguration.Properties;
using System.IO;
using Oracle.RightNow.Cti.Common;


namespace Oracle.RightNow.Cti.ScreenPopConfiguration
{
    [AddIn("CTI screen Navigation Item", Publisher = "Oracle Corporation", Version = "1.0.0.0")]
    public class ScreenPopConfigurationNavigationItem : INavigationItem
    {
        private IGlobalContext _globalContext;
        private CompositionContainer _container;

       // [Import]
       // public IScreenPopConfigurationContentPane ScreenPopContentPane { get; set; }


        [Export]
        public IGlobalContext GlobalContext
        {
            get { return _globalContext; }
        }
        public bool Initialize(IGlobalContext context)
        {
            _globalContext = context;
            Global.Context = context;
            var baseCatalog = new AggregateCatalog();
            var info = Directory.GetParent(Path.GetDirectoryName(typeof(ScreenPopConfigurationNavigationItem).Assembly.Location));
            AddDirectoryToCatalog(info.Parent, baseCatalog);

            _container = new CompositionContainer(baseCatalog);
            try
            {
                _container.ComposeParts(this);
                return true;
            }
            catch (CompositionException exc)
            {
                context.LogMessage(exc.ToString());
                return false;
            }
        }

        public void Activate()
        {
            _globalContext.AutomationContext.OpenEditor(new ScreenPopConfigurationContentPane());
        }
        
        public string Text
        {
            get
            {
                return Resources.CtiScreenNavigationItemText;
            }
        }

        public Image Image16
        {
            get
            {
                return Resources.administration_phone;
            }
        }

        private void AddDirectoryToCatalog(DirectoryInfo directoryInfo, AggregateCatalog catalog)
        {
            catalog.Catalogs.Add(new DirectoryCatalog(directoryInfo.FullName));

            foreach (var directory in directoryInfo.GetDirectories())
            {
                AddDirectoryToCatalog(directory, catalog);
            }
        }
    }
}
