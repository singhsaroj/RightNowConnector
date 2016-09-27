using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Oracle.RightNow.Cti.Model;

namespace Oracle.RightNow.Cti
{
    public class CISpecification
    {
        public static MediaType cIInfoType(IInteraction interaction)
        {
            MediaType cInfoState = MediaType.Voice;
            try
            {
                //IInteraction cIInteraction = InteractionProvider.Interactions.FirstOrDefault(p => p.CallId == callIdNode.Value);
                if (interaction != null)
                {
                    ScreenPopConfig screenPopConfigs = interaction.ScreenPopConfiguration;
                    if (screenPopConfigs.CanScreenPop)
                    {
                        string UUIValue = interaction.InteractionData["UUI"];
                        ScreenPopOptions UUIstart = screenPopConfigs.ScreenPopOptionsList.SingleOrDefault(p => p.Type == 0 && p.Name.Equals("UUIStart", StringComparison.OrdinalIgnoreCase));
                        string start = UUIstart == null ? ";" : UUIstart.Description;
                        ScreenPopOptions UUIstop = screenPopConfigs.ScreenPopOptionsList.SingleOrDefault(p => p.Type == 0 && p.Name.Equals("UUIStop", StringComparison.OrdinalIgnoreCase));
                        string stop = UUIstop == null ? ":" : UUIstop.Description;
                        ScreenPopOptions UUIsep = screenPopConfigs.ScreenPopOptionsList.SingleOrDefault(p => p.Type == 0 && p.Name.Equals("UUISep", StringComparison.OrdinalIgnoreCase));
                        char seperator = UUIsep == null ? '!' : UUIsep.Description.ToCharArray()[0];
                        int startindex = UUIValue.IndexOf(start, 0) + 1;
                        int lastindex = UUIValue.LastIndexOf(stop) == -1 ? UUIValue.Length : UUIValue.LastIndexOf(stop);
                        int len = lastindex - startindex;
                        string[] uui = UUIValue.Substring(startindex, len).Split(seperator);
                        Regex chatIncidentRegex = new Regex("^UUI[1-2]", RegexOptions.IgnoreCase);
                        List<ScreenPopOptions> cINotification = screenPopConfigs.ScreenPopOptionsList.Where(p => p.Type == 0 && chatIncidentRegex.Matches(p.Name).Count > 0).ToList();

                        try
                        {
                            if (uui.Length > 1)
                            {
                                Logger.Logger.Log.Debug(String.Format("AnswerHangupImage cINotifiction Details: {0} {1}: UUI Details: {2} {3} ", cINotification[0].Description, cINotification[1].Description, uui[0], uui[1]));
                                if ((uui[0] == cINotification[0].Description || uui[1] == cINotification[0].Description) && (cINotification[0].Description != null && cINotification[0].Description != ""))
                                {
                                    cInfoState = MediaType.Chat;
                                }
                                else if (uui[0] == cINotification[1].Description || uui[1] == cINotification[1].Description && (cINotification[1].Description != null && cINotification[1].Description != ""))
                                {
                                    cInfoState = MediaType.Incident;
                                }
                            }
                            else if (uui.Length == 1)
                            {
                                if (uui[0] == cINotification[0].Description && (cINotification[0].Description != null && cINotification[0].Description != ""))
                                {
                                    cInfoState = MediaType.Chat;
                                }
                                else if (uui[0] == cINotification[1].Description && (cINotification[1].Description != null && cINotification[1].Description != ""))
                                {
                                    cInfoState = MediaType.Incident;
                                }
                            }
                        }
                        catch (NullReferenceException ex)
                        {
                            Logger.Logger.Log.Error("Interaction null reference." + ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Debug("Interaction was null" + ex);
            }
            return cInfoState;
        }
    }
}
