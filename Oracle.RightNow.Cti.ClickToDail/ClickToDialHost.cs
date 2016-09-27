using Oracle.RightNow.Cti.CtiServiceLibrary;
using RightNow.AddIns.AddInViews;
using RightNow.AddIns.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Oracle.RightNow.Cti.ClickToDail
{
    public class ClickToDialHost : Panel
    {
        private IRecordContext _recordContext;
        private IContact _contactRecord;

        private PictureBox _btnDial;
        //private ContextMenuStrip _contextMenuSkills;
        //private string _numberToDial;
        //private bool _enableClickToDial;
        private ToolTip _click2DialTooltip;

        public ClickToDialHost(bool inDesignMode, IRecordContext context)
        {
            _recordContext = context;
            /*****************************/
            /*    InitializeComponenet   */
            /*****************************/
            this.SuspendLayout();
            _btnDial = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._btnDial)).BeginInit();
            //
            this._btnDial.Name = "btnDial";
            this._btnDial.Image = Properties.Resources.Phone20x20;
            this._btnDial.Margin = new Padding(0);
            this._btnDial.Padding = new Padding(0);
            this._btnDial.SizeMode = PictureBoxSizeMode.AutoSize;
            this._btnDial.Location = new Point(0, 0);
            this._btnDial.MouseEnter += new System.EventHandler(PictureBox_MouseEnter);
            this._btnDial.MouseLeave += new System.EventHandler(PictureBox_MouseLeave);
            this._btnDial.Click += new System.EventHandler(PictureBox_Click);
            //
            this.Margin = new Padding(0);
            this.Controls.Add(_btnDial);
            //
            this.Size = new Size(20, 40);
            this.ResumeLayout(false);
            this.PerformLayout();
            /*****************************/
            /* End InitializeComponenet  */
            /*****************************/
            _click2DialTooltip = new ToolTip();
            //_click2DialTooltip.ToolTipTitle = AgentStrings.ClickToDialTooltipTitle;
            if (!inDesignMode && _recordContext != null)
            {
                _recordContext.DataLoaded += new System.EventHandler(RecordContext_DataLoaded);
            }
        }

        /// <summary>
        /// Data loaded event of the Contact workspace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RecordContext_DataLoaded(object sender, System.EventArgs e)
        {
            if (_recordContext.WorkspaceType == WorkspaceRecordType.Contact)
            {
                _contactRecord = (IContact)_recordContext.GetWorkspaceRecord(WorkspaceRecordType.Contact);                
            }
        }

        /// <summary>
        /// Click event of the Dial button, to make call. 
        /// Show the menu to select if there are multiple outbound skills
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PictureBox_Click(object sender, System.EventArgs e)
        {
            //Get the mouse position while clicking on the button
            int xCursor = System.Windows.Forms.Cursor.Position.X;
            int yCursor = System.Windows.Forms.Cursor.Position.Y;
            //if (!string.IsNullOrEmpty(_numberToDial) && _enableClickToDial)
            //{
            //    this.MakeCall(_numberToDial);
            //}
            //else
            //    MessageBox.Show("Outbound dialing is not available because the current user is not logged", "RightNow CX");
            }

        /// <summary>
        /// Making a call to dial number by skill name.
        /// </summary>
        /// <param name="dialNumber"></param>
        /// <param name="skillName"></param>
        private void MakeCall(string dialNumber)
        {
           // Logger.Log.DebugFormat("ClickToDialHost->MakeCall place call by dialnumber:{0}, skillname:{1}", dialNumber, skillName); 
            //CtiCallInfo.GetCtiCallInfoObject().MakeCall(dialNumber);
            //MessageBox.Show("Make a call" + dialNumber);
        }

        /// <summary>
        /// MouseLeave event to set image back to original
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PictureBox_MouseLeave(object sender, System.EventArgs e)
        {
            this._btnDial.Image = Properties.Resources.Phone20x20;
            _click2DialTooltip.RemoveAll();
        }

        /// <summary>
        /// MouseEnter event of image control to change the image as highlighted one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PictureBox_MouseEnter(object sender, System.EventArgs e)
        {

            FormPhoneNo ph = new FormPhoneNo(Cursor.Position.X, Cursor.Position.Y, _recordContext);
            ph.ShowDialog();
            
            
            //_numberToDial = null;
            //foreach (Control field in this.Parent.Controls)
            //{
            //    if (field.Name == "Contact.PhOffice") //This name is to identify the contacts phone number field
            //    {
            //        PropertyInfo valueInfo = field.GetType().GetProperty("Value");
            //        object value = valueInfo.GetValue(field, null);
            //        _numberToDial = (string)value;
            //        break;
            //    }
            //}
            //if ((this.Parent.Controls.Owner.CompanyName.ToString().ToUpper().Contains("MICROSOFT")) && this.Parent.Controls[0].Controls.Owner.ToString().ToUpper().Contains("CLICKTODAIL"))
            //{
            //    _numberToDial = _contactRecord.PhOffice;
            //    if (string.IsNullOrEmpty(_numberToDial))
            //    {
            //        _enableClickToDial = false;
            //        _click2DialTooltip.SetToolTip(this._btnDial, "Click To dial disabled");
            //        this._btnDial.Image = Properties.Resources.Phone20x20;
            //    }
            //    else
            //    {
            //         _enableClickToDial = true;
            //        _click2DialTooltip.SetToolTip(this._btnDial, "Click to dial");
            //        this._btnDial.Image = Properties.Resources.Phone20x20Hover;
            //    }
                
            //}

            //if (!string.IsNullOrEmpty(_numberToDial))
            //{
            //    _enableClickToDial = true;
            //    _click2DialTooltip.SetToolTip(this._btnDial, "Click to dial");
            //    this._btnDial.Image = Properties.Resources.Phone20x20Hover;
               
            //}
            //else
            //{
            //    _click2DialTooltip.SetToolTip(this._btnDial, "Click To dial disabled");
            //}
        }
    }
}


Braching for Oracle Use case RTNOWCNXN-220 -- Just to checkin used the closed JIRA ticket