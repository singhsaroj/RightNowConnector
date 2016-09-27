using Oracle.RightNow.Cti.CtiServiceLibrary;
using RightNow.AddIns.AddInViews;
using RightNow.AddIns.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Oracle.RightNow.Cti.ClickToDail
{
    public partial class FormPhoneNo : Form
    {
        private int desiredStartLocationX;
        private int desiredStartLocationY;
        private IContact _contactRecord;
        private ToolTip _click2DialFormTooltip;
        private IRecordContext _recordContext;
        DataTable dt = new DataTable();

        public FormPhoneNo()
        {
            InitializeComponent();
            BindGrid();
        }
        public FormPhoneNo(int x, int y, IRecordContext context)
            : this()
        {

            this.desiredStartLocationX = x;
            this.desiredStartLocationY = y;
            Load += new EventHandler(FormPhoneNo_Load);
            _recordContext = context;
            GetSelectedContact(context);
            _click2DialFormTooltip = new ToolTip();

        }

        private void GetSelectedContact(IRecordContext context)
        {
            try
            {
                if (context.WorkspaceType == WorkspaceRecordType.Contact)
                {
                    _contactRecord = (IContact)context.GetWorkspaceRecord(WorkspaceRecordType.Contact);
                }

                dt.Columns.Add("");
                dt.Columns.Add("");

                dt.Rows.Add(new object[] { Properties.Resources.Office, (_contactRecord.PhOffice != null) ? _contactRecord.PhOffice.ToString() : "" });
                dt.Rows.Add(new object[] { Properties.Resources.Assistant, (_contactRecord.PhAsst != null) ? _contactRecord.PhAsst.ToString() : "" });
                dt.Rows.Add(new object[] { Properties.Resources.Home, (_contactRecord.PhHome != null) ? _contactRecord.PhHome.ToString() : "" });
                dt.Rows.Add(new object[] { Properties.Resources.Mobile, (_contactRecord.PhMobile != null) ? _contactRecord.PhMobile.ToString() : "" });
                dt.Rows.Add(new object[] { Properties.Resources.Fax, (_contactRecord.PhFax != null) ? _contactRecord.PhFax.ToString() : "" });

            }
            catch (Exception EX )
            {

                Logger.Logger.Log.Error("Error Occured at selecting the contact record to display in Popup :-" + EX.Message);
            }

            
        }
        private void BindGrid()
        {
            dataGridView1.DataSource = dt;
        }
        private void FormPhoneNo_Load(object sender, EventArgs e)
        {
            this.SetDesktopLocation(desiredStartLocationX, desiredStartLocationY);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int rowindex = dataGridView1.CurrentRow.Index;
            if (dataGridView1.CurrentRow.Index >= 0)
            {
                if (string.IsNullOrEmpty(dataGridView1.SelectedCells[1].Value.ToString()))
                {
                    MessageBox.Show(Properties.Resources.InvalidPhoneorEmpty);
                }
                else
                {
                    CtiCallInfo.GetCtiCallInfoObject().MakeCall(dataGridView1.SelectedCells[1].Value.ToString());
                }

            }
            Close();
        }

        private void linkLabel2_MouseHover(object sender, EventArgs e)
        {
            _click2DialFormTooltip.SetToolTip(this.linkLabel2, "Close");
        }

        private void linkLabel1_MouseHover(object sender, EventArgs e)
        {
            _click2DialFormTooltip.SetToolTip(this.linkLabel1, "Click To Dial");
        }

    }
}
