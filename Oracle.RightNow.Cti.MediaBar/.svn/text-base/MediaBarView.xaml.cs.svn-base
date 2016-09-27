using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using Oracle.RightNow.Cti.MediaBar.ViewModels;

namespace Oracle.RightNow.Cti.MediaBar {
    /// <summary>
    /// Interaction logic for MediaBarView.xaml
    /// </summary>
    [Export]
    public partial class MediaBarView : UserControl {
        public MediaBarView() {
            InitializeComponent();

        }

        [Import]
        public MediaBarViewModel ViewModel {
            set {
                DataContext = value;
            }            
        }

        private void MediaBar_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            (this.DataContext as MediaBarViewModel).ImplictLogin();
        }
    }
}