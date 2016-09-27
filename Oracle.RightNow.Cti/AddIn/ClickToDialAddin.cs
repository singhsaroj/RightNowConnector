using Oracle.RightNow.Cti.Common;
using RightNow.AddIns.AddInViews;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Oracle.RightNow.Cti.AddIn
{
    public class ClickToDialAddin : IWorkspaceComponent2
    {
        [Import]
        public IClickToDialProvider ClickToDialProvider { get; set; }

        //[Export]
        public IGlobalContext GlobalContext { get; set; }

      //  [Import]
        public IRecordContext RecordContext { get; set; }
        
        private CompositionContainer _container;
        public bool InDesignMode { get; set; }

        #region IWorkSpaceComponent2 Members

        /// <summary>
        /// Sets the ReadOnly property of this control.
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Method which is called when any Workspace Rule Action is invoked.
        /// </summary>
        /// <param name="ActionName">The name of the Workspace Rule Action that was invoked.</param>
        public void RuleActionInvoked(string actionName) { }

        /// <summary>
        /// Method which is called when any Workspace Rule Condition is invoked.
        /// </summary>
        /// <param name="ConditionName">The name of the Workspace Rule Condition that was invoked.</param>
        /// <returns>The result of the condition.</returns>
        public string RuleConditionInvoked(string conditionName)
        {
            return string.Empty;
        }

        #endregion

        #region IAddInControl Member

        /// <summary>
        /// Method called by the Add-In framework to retrieve the control.
        /// </summary>
        /// <returns></returns>
        public System.Windows.Forms.Control GetControl()
        {
            try
            {
                Logger.Logger.Log.Debug("Click To Dial Control Addin GetControl...");

                var baseCatalog = new AggregateCatalog();
                var info = Directory.GetParent(Path.GetDirectoryName(typeof(ClickToDialAddin).Assembly.Location));
                this.GlobalContext = Global.Context;
                addDirectoryToCatalog(info.Parent, baseCatalog);

                _container = new CompositionContainer(baseCatalog);
                _container.ComposeParts(this);
                Control ctrl =this.ClickToDialProvider.GetControl(this.InDesignMode, this.RecordContext);

                Logger.Logger.Log.Debug("Click To Dial Control Addin GetControl success.");

                return ctrl;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("Click To Dial Control Addin GetControl failed", ex);
                return null;
            }
        }

        private void addDirectoryToCatalog(DirectoryInfo directoryInfo, AggregateCatalog catalog)
        {
            catalog.Catalogs.Add(new DirectoryCatalog(directoryInfo.FullName));

            foreach (var directory in directoryInfo.GetDirectories())
            {
                addDirectoryToCatalog(directory, catalog);
            }
        }

        #endregion
    }
}
