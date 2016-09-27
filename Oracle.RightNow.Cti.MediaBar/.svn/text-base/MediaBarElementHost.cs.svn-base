using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Oracle.RightNow.Cti.MediaBar;
using Oracle.RightNow.Cti.MediaBar.ViewModels;

namespace Oracle.RightNow.Cti.MediaBar {
    [Export]
    public class MediaBarElementHost : Panel, IPartImportsSatisfiedNotification {
        public MediaBarElementHost() : base() {
            BackColor = Color.Transparent;
            Height = 50;
        }

        public void OnImportsSatisfied() {
            var elementHost = new ElementHost();
            elementHost.Child = MediaBarView;
            elementHost.Dock = DockStyle.Fill;
            this.Controls.Add(elementHost);
        }

        public override Size GetPreferredSize(Size proposedSize) {
            var width = Math.Max(proposedSize.Width, 1000);
            return new Size(width - 5, Height);
        }

        [Import]
        public MediaBarView MediaBarView { get; set; }
    }
}