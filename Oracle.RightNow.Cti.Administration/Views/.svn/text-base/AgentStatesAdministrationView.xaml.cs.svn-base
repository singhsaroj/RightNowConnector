using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using Oracle.RightNow.Cti.Administration.ViewModels;

namespace Oracle.RightNow.Cti.Administration {
    /// <summary>
    /// Interaction logic for AgentStatesAdministrationView.xaml
    /// </summary>
    [Export]
    public partial class AgentStatesAdministrationView : UserControl, IAgentAdministrationView {
        public AgentStatesAdministrationView() {
            InitializeComponent();
            Loaded += AgentStatesAdministrationView_Loaded;
        }

        void AgentStatesAdministrationView_Loaded(object sender, RoutedEventArgs e) {
            ViewModel.Load();
        }

        [Import]
        public IAgentStatesAdministrationViewModel ViewModel {
            set {
                DataContext = value;
            }
            private get { return DataContext as IAgentStatesAdministrationViewModel; }
        }
    }

    public interface IAgentAdministrationView { }
}