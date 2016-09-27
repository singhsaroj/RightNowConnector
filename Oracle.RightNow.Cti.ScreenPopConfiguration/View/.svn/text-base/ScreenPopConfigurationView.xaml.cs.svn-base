using Oracle.RightNow.Cti.ScreenPopConfiguration.ViewModel;
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

namespace Oracle.RightNow.Cti.ScreenPopConfiguration.View
{
    /// <summary>
    /// Interaction logic for ScreenPopConfigurationView.xaml
    /// </summary>
    
   // [Export]
    public partial class ScreenPopConfigurationView : UserControl
    {
        public ScreenPopConfigurationView()
        {
            InitializeComponent();
            this.ViewModel = new ScreenPopConfigurationViewModel();
            this.Loaded += new RoutedEventHandler(AgentConfigurationView_Loaded);
        }

       // [Import]
        public ScreenPopConfigurationViewModel ViewModel
        {
            set
            {
                DataContext = value;
            }
            get { return DataContext as ScreenPopConfigurationViewModel; }
        }

        void AgentConfigurationView_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Intilization();

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                ViewModel.LoadScreenPopConfigData();
            }));
        }
    }
}
