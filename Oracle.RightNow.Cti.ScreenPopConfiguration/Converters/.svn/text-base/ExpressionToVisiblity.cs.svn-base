using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;

namespace Oracle.RightNow.Cti.ScreenPopConfiguration.Converters
{
    public class ExpressionToVisiblity : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {            
              Regex eventName = new Regex("^UUI[1-5]", RegexOptions.IgnoreCase);
              if (value != null && eventName.Matches(value.ToString()).Count > 0)
              {
                  return Visibility.Visible;
              }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
    }
}
