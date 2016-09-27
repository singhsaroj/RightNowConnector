using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Oracle.RightNow.Cti.MediaBar.Converters {
    public class BooleanToVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool) {
                var booleanValue = (bool)value;
                Logger.Logger.Log.Debug("BooleanToVisibilityConverter: Interaction.Type: " + booleanValue + "Target Media Type: " + parameter);
                if (parameter == null)
                {
                    return booleanValue
                        ? Visibility.Visible
                        : Visibility.Hidden;
                }
                else {
                    return booleanValue
                    ? Visibility.Visible
                    : Visibility.Collapsed;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
    }
}
