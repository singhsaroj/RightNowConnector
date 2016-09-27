using Oracle.RightNow.Cti.MediaBar.Properties;
using Oracle.RightNow.Cti.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace Oracle.RightNow.Cti.MediaBar.Converters
{
    public class MediaTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var interaction = value as IInteraction;
            MediaType curType = CISpecification.cIInfoType(interaction);
            Logger.Logger.Log.Debug("MediaTypeToStringConverter: cIState: " + curType);
            string result = Resources.MediaTypeUnknown;
            if (interaction != null)
            {
                switch (interaction.Type)
                {
                    case MediaType.Voice:
                        if (curType == MediaType.Chat) result = Resources.MediaTypeChat;
                        else if (curType == MediaType.Incident) result = Resources.MediaTypeIncident;
                        else result = Resources.MediaTypeVoice;
                        //result = Resources.MediaTypeVoice;
                        break;
                    case MediaType.Email:
                        result = Resources.MediaTypeEmail;
                        break;
                    case MediaType.Fax:
                        break;
                    case MediaType.Chat:
                        result = Resources.MediaTypeChat;
                        break;
                    case MediaType.Web:
                        result = Resources.MediaTypeWeb;
                        break;
                    case MediaType.Social:
                        break;
                    case MediaType.Sms:
                        break;
                    case MediaType.Generic:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Logger.Logger.Log.Debug("MediaTypeToStringConverter: Interaction was null. ");
            }

            return result.ToLower();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
