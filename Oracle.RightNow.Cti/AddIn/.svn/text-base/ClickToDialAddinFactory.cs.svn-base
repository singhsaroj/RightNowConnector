using Oracle.RightNow.Cti.Common;
using RightNow.AddIns.AddInViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oracle.RightNow.Cti.AddIn
{
    [System.AddIn.AddIn("Oracle Control ClickToDial Addin Factory", Publisher = "Oracle Inc", Version = "111.111.111.111")]
    public class ClickToDialAddinFactory : IWorkspaceComponentFactory2
    {
        IWorkspaceComponent2 _clickToDialComponent;

        #region IWorkSpaceComponentFactory2 Member

        /// <summary>
        /// Method which is invoked by the AddIn framework when the control is created.
        /// </summary>
        /// <param name="inDesignMode">Flag which indicates if the control is being drawn on the Workspace Designer. (Use this flag to determine if code should perform any logic on the workspace record)</param>
        /// <param name="RecordContext">The current workspace record context.</param>
        /// <returns>The control which implements the IWorkspaceComponent2 interface.</returns>
        public IWorkspaceComponent2 CreateControl(bool inDesignMode, IRecordContext context)
        {
            _clickToDialComponent = new ClickToDialAddin();

            (_clickToDialComponent as ClickToDialAddin).InDesignMode = inDesignMode;
            (_clickToDialComponent as ClickToDialAddin).RecordContext = context;

            return _clickToDialComponent;
        }

        #endregion

        #region IFactoryBase Members

        /// <summary>
        /// The 16x16 pixel icon to represent the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public System.Drawing.Image Image16
        {
            get { return Properties.Resources.phone20x20; }
        }

        /// <summary>
        /// The text to represent the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Text
        {
            get { return "Click to Dail"; }
        }

        /// <summary>
        /// The tooltip displayed when hovering over the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Tooltip
        {
            get { return "Click to Dail"; }
        }

        #endregion

        #region IAddInBase Member

        /// <summary>
        /// Method which is invoked from the Add-In framework and is used to programmatically control whether to load the Add-In.
        /// </summary>
        /// <param name="GlobalContext">The Global Context for the Add-In framework.</param>
        /// <returns>If true the Add-In to be loaded, if false the Add-In will not be loaded.</returns>
        public bool Initialize(IGlobalContext context)
        {
            try
            {
                Global.Context = context;
                return true;
            }
            catch (Exception ex)
            {
                context.LogMessage(ex.Message);
                Logger.Logger.Log.Error("RightNowObjectProvider: ", ex);
                return false;
            }
        }

        #endregion
    }
}
