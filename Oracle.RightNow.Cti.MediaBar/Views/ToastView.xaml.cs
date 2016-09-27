using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Oracle.RightNow.Cti.MediaBar.Views;
using Oracle.RightNow.Cti.MediaBar.ViewModels;

namespace Oracle.RightNow.Cti.MediaBar.Views {
    /// <summary>
    
    public partial class ToastView : Window {
        // non UI bound.
        private ToastViewModel _model;
        public ToastView() {
            InitializeComponent();
        }

        public ToastViewModel Model {
            get {
                return _model;
            }
            set {
                _model = value;
                DataContext = _model;
            }
        }
    }
}
