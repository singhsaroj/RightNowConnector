using System.Threading.Tasks;
using Oracle.RightNow.Cti.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Oracle.RightNow.Cti.CtiServiceLibrary;
using System.Windows;

namespace Oracle.RightNow.Cti.MediaBar.ViewModels {
    public class ToastViewModel : ViewModel {

        private CtiCallInfo ctiCallInfoObject;

        public ToastViewModel() {
            initializeCommands();
            ctiCallInfoObject = CtiCallInfo.GetCtiCallInfoObject();
        }
        public IInteraction Interaction { get; set; }
        public DateTime LastActivity { get; set; }

        public bool Inactive { get; set; }

        public ICommand AcceptCommand { get; set; }

        private void initializeCommands() {
            AcceptCommand = new DelegateCommand(o => accept());
        }

        private void Close()
        {
            try
            {

                Application.Current.Dispatcher.Thread.Abort();
            }
            catch (Exception)
            {

                throw;
            }

        }
        private void accept() {
            Task.Factory.StartNew(() => {
                if (Interaction != null && Interaction.State == InteractionState.Ringing) {
                    //Interaction.Accept();
                    ctiCallInfoObject.OnCallAnswered(Interaction.CallId);
                    //Application.Current.MainWindow.Close();
                    //Application.Current.Dispatcher.Invoke(new Action(() => this.Close()));
                    
                }
            });

            
        }
    }
}
