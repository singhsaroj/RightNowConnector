using System;
using System.Globalization;
using System.Windows.Data;
using Oracle.RightNow.Cti.MediaBar.Properties;
using Oracle.RightNow.Cti.Model;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Oracle.RightNow.Cti.MediaBar.Converters {
    public class MediaTypeToImageConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var interaction = value as IInteraction;
            MediaType curType = CISpecification.cIInfoType(interaction);
            Logger.Logger.Log.Debug("MediaTypeToImageConverter: cIState: " + curType);
            var result = string.Empty;

            if (interaction != null) {
                switch (interaction.Type)
                {
                    case MediaType.Voice:
                        if (curType == MediaType.Chat) result = Resources.MediaTypeChatImageUri;
                        else if (curType == MediaType.Incident) result = Resources.MediaTypeIncidentImageUri;
                        else result = Resources.MediaTypeVoiceImageUri;
                        //result = Resources.MediaTypeVoiceImageUri;
                        break;
                    case MediaType.Email:
                        result = Resources.MediaTypeEmailImageUri;
                        break;
                    case MediaType.Fax:
                        break;
                    case MediaType.Web:
                        result = Resources.MediaTypeWebImageUri;
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

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
    }
}