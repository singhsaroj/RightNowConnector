using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using Oracle.RightNow.Cti.Model;

namespace Oracle.RightNow.Cti.MediaBar.Converters {
    public class AgentStateToBrushConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var state = value as AgentState;
            
            var color = Colors.Gray;

            if (!Cti.CtiServiceLibrary.CtiCallInfo.GetCtiCallInfoObject().IsACD)
                return (state != null && state.SwitchMode == AgentSwitchMode.LoggedOut) ? new SolidColorBrush(color) : new SolidColorBrush(Colors.YellowGreen);

            if (state != null)
                switch (state.SwitchMode)
                {
                    case AgentSwitchMode.LoggedIn:
                    case AgentSwitchMode.NotReady:
                        color = Colors.Red;
                        break;
                    case AgentSwitchMode.Ready:
                        color = Colors.YellowGreen;
                        break;
                    case AgentSwitchMode.WrapUp:
                        color = Colors.Orange;
                        break;
                    case AgentSwitchMode.LoggedOut:
                        break;
                    case AgentSwitchMode.HandlingInteraction:
                        break;
                    case AgentSwitchMode.NewReason:
                        break;
                    default:
                        break;
                }
            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
    }
}