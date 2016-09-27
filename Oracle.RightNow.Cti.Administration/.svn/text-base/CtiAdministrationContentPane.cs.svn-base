// ===========================================================================================
//  Oracle RightNow Connect
//  CTI Sample Code
// ===========================================================================================
//  Copyright © Oracle Corporation.  All rights reserved.
// 
//  Sample code for training only. This sample code is provided "as is" with no warranties 
//  of any kind express or implied. Use of this sample code is pursuant to the applicable
//  non-disclosure agreement and or end user agreement and or partner agreement between
//  you and Oracle Corporation. You acknowledge Oracle Corporation is the exclusive
//  owner of the object code, source code, results, findings, ideas and any works developed
//  in using this sample code.
// ===========================================================================================
  
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Oracle.RightNow.Cti.Administration.Properties;
using RightNow.AddIns.AddInViews;

namespace Oracle.RightNow.Cti.Administration {
    [Export(typeof(ICtiAdministrationContentPane))]
    public class CtiAdministrationContentPane : ICtiAdministrationContentPane {
        private const string UniqueId = "CAA37485-D581-430E-B84A-FC593B5AACA6";
        private Control _panelControl;
        
        public CtiAdministrationContentPane() {
        }

        [Import]
        public AgentStatesAdministrationView AgentStatesView { get; set; }

        [Import]
        public IGlobalContext GlobalContext { get; set; }

        public Image Image16 {
            get {
                return Resources.AdministrationIcon;
            }
        }

        public IList<IEditorRibbonButton> RibbonButtons {
            get {
                return new List<IEditorRibbonButton>();
            }
        }

        public string Text {
            get {
                return Resources.CtiAdministrationContentPaneText;
            }
        }

        public string UniqueID {
            get {
                return UniqueId;
            }
        }

        public bool BeforeClose() {
            return true;
        }

        public void Closed() {
        }

        public Control GetControl() {
            if (_panelControl == null) {
                var elementHost = new ElementHost();
                elementHost.Child = AgentStatesView;
                elementHost.Dock = DockStyle.Fill;
                _panelControl = elementHost;
            }
            return _panelControl;
        }
    }
}  