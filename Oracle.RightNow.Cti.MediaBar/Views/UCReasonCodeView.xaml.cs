using Oracle.RightNow.Cti.MediaBar.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Oracle.RightNow.Cti.MediaBar.Views
{
    /// <summary>
    /// Interaction logic for UCReasonCodeView.xaml
    /// </summary>
    public partial class UCReasonCodeView : UserControl
    {
        public UCReasonCodeView()
        {
            InitializeComponent();
          //  this.ViewModel = new UCReasonCodeViewModel();
        }

        public UCReasonCodeViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
