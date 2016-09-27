using Oracle.RightNow.Cti.MediaBar.ViewModels;
using Oracle.RightNow.Cti.MediaBar.Views;
using Oracle.RightNow.Cti.Model;
using Oracle.RightNow.Cti.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Oracle.RightNow.Cti.MediaBar {
    public class ToastManager {
        private static List<ToastView> _openToasts = new List<ToastView>();
        private static System.Threading.Timer _timer;
        public static void Initialize(IInteractionProvider provider) {
            provider.NewInteraction += newInteraction;
            _timer = new System.Threading.Timer(timerTick, null, Timeout.Infinite, Timeout.Infinite);
        }

        private static void timerTick(object state) {
            var now = DateTime.Now;
            try
            {
                foreach (var item in _openToasts) {
                    
                    if (item.Model.Inactive && item.Model.LastActivity < now)
                    {
                        if (Application.OpenForms.Count > 0)
                        {
                            Application.OpenForms[0].BeginInvoke(new Action(() => item.Close()));
                        }
                        else
                        {
                            if (_openToasts.Count == 1)
                            {
                                //Thread.Sleep(3000);
                                //Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => item.Close()));
                                //item.Visibility = System.Windows.Visibility.Hidden;
                            }
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                string s = ex.Message.ToString();
                throw;
            }
            
          
        }

        private static void newInteraction(object sender, InteractionEventArgs e) {
            if (Application.OpenForms.Count > 0)
            {
                Application.OpenForms[0].BeginInvoke(new Action<IInteraction>(o => showToast(o)), e.Interaction);
            }
            else
            {
                //Dispatcher.CurrentDispatcher.BeginInvoke(new Action<IInteraction>(o => showToast(o)), e.Interaction);
                //showToast(e.Interaction);
            }
        }

        private static IntPtr findHandle()
        {

            Process Processes = Process.GetCurrentProcess();
            IntPtr hWnd = IntPtr.Zero;
            hWnd = Processes.Handle;
            return hWnd;
        }
        private static void showToast(IInteraction interaction) {
            
            var call = interaction as ICall;
            var cxScreen = Screen.PrimaryScreen;
            if (call.CallType == CallType.Outbound)
                return;
            if (Application.OpenForms.Count > 0)
            {
               cxScreen = Screen.FromHandle(Application.OpenForms[0].Handle);
            }
            else
            {
                cxScreen = Screen.FromHandle(findHandle());
                
            }
            
            
            var toast = new ToastView {
                Model = new ToastViewModel {
                    Interaction = interaction,
                    LastActivity = DateTime.Now.AddSeconds(5),
                    Inactive = true
                }
            };

            interaction.StateChanged += interaction_StateChanged;

            toast.Left = (cxScreen.WorkingArea.X + cxScreen.WorkingArea.Width) - (toast.Width + 1);
            toast.Top = (cxScreen.WorkingArea.Y + cxScreen.WorkingArea.Height) - (toast.Height + 1) - (_openToasts.Count * toast.Height);
            toast.MouseEnter += toast_MouseEnter;
            toast.MouseLeave += toast_MouseLeave;
            toast.Closed += toast_Closed;
            _openToasts.Add(toast);
            toast.Show();
            
            if (_openToasts.Count == 1) {
                _timer.Change(5000, 1000);
            }
        }

        static void interaction_StateChanged(object sender, InteractionStateChangedEventArgs e) {
            var interation = (IInteraction) sender;
            interation.StateChanged -= interaction_StateChanged;

            var view = _openToasts.FirstOrDefault(v => v.Model.Interaction.Id == interation.Id);
            
            if (view != null) {
                _openToasts.Remove(view);
                if (Application.OpenForms.Count > 0)
                {
                    Application.OpenForms[0].BeginInvoke(new Action(() => view.Close()));
                }
                else
                {
                    
                }
            }
        }

        static void toast_Closed(object sender, EventArgs e) {
            var view = ((ToastView)sender);

            _openToasts.Remove(view);
            view.Closed -= toast_Closed;
            view.MouseLeave -= toast_MouseLeave;
            view.MouseEnter -= toast_MouseEnter;
            if (_openToasts.Count == 0) {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        static void toast_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
           var view = ((ToastView)sender);
          view.Model.LastActivity = DateTime.Now.AddSeconds(3);
          view.Model.Inactive = true;
        }

        static void toast_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
            ((ToastView)sender).Model.Inactive = false;    
        }
    }
}
