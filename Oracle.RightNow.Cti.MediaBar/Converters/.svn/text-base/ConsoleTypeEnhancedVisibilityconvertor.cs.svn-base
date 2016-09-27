using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Oracle.RightNow.Cti.MediaBar.Properties;
using Oracle.RightNow.Cti.Model;

namespace Oracle.RightNow.Cti.MediaBar.Converters {
    public class ConsoleTypeEnhancedVisibilityconvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            Boolean isNotEnhanced= (Boolean)value;

            if (isNotEnhanced)
                    return Visibility.Collapsed;
          else
              return Visibility.Visible;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}