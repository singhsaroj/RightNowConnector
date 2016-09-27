using System;
using System.Globalization;
using System.Windows.Data;
using Oracle.RightNow.Cti.MediaBar.Properties;
using Oracle.RightNow.Cti.Model;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Oracle.RightNow.Cti.MediaBar.Converters {
    public class InteractionStateToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = string.Empty;

            if (value[0] is InteractionState) {
                var state = (InteractionState)value[0];
                if (state != InteractionState.Ringing)
                {
                    switch (state)
                    {
                        case InteractionState.Active:
                            result = Resources.InteractionStateActive;
                            break;
                        case InteractionState.Held:
                            result = Resources.InteractionStateHeld;
                            break;
                        case InteractionState.Disconnected:
                            result = Resources.InteractionStateDisconnected;
                            break;
                        case InteractionState.Dialing:
                            result = Resources.InteractionStateDialing;
                            break;
                        case InteractionState.Conferenced:
                            result = Resources.InteractionConferenced;
                            break;
                        case InteractionState.Consulting:
                            result = Resources.InteractionConsulting;
                            break;
                        default:
                            break;
                    }
                }
                else if(state==InteractionState.Ringing) {
                    var interaction = value[1] as IInteraction;
                    if (interaction != null)
                    {
                        MediaType curType = CISpecification.cIInfoType(interaction);
                        Logger.Logger.Log.Debug("InteractionStateToStringConverter: cIState: " + curType + "Interaction.State: " + interaction.State);

                        switch (interaction.State)
                        {
                            case InteractionState.Ringing:
                                if (curType == MediaType.Chat) result = Resources.InteractionStateChat;
                                else if (curType == MediaType.Incident) result = Resources.InteractionStateIncident;
                                else result = Resources.InteractionStateRinging;
                                //result = Resources.InteractionStateRinging;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        Logger.Logger.Log.Debug("InteractionStateToStringConverter: Interaction was null.");
                    }
                }
                
            }
            return result;
        }

        //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        //    // TODO: Implement this method
        //    throw new NotImplementedException();
        //}


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}