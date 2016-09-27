using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Forms;
using System.AddIn;
using RightNow.AddIns.AddInViews;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Oracle.RightNow.Cti.MediaBar.Converters
{
    public class LoggedInStateToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            
            if (value is bool) {
                var isLoggedIn = (bool)value;
                
                Logger.Logger.Log.Debug("LoggedInStateToImageConverter: isLoggedIn: " + isLoggedIn.ToString());

                //if (isLoggedIn)
                //{
                //    ChatAutomationClient.Login(true);
                //}
                //else
                //{
                //    ChatAutomationClient.Logoff(true);
                //}

                return isLoggedIn
                    ? "/Oracle.RightNow.Cti.Mediabar;component/Images/mediabar.disconnect.png"
                    : "/Oracle.RightNow.Cti.Mediabar;component/Images/mediabar.connect.png";
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
    }
}
