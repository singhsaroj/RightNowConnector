using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Oracle.RightNow.Cti.Model;
//using Oracle.RightNow.Cti.Common;
using RightNow.AddIns.AddInViews;

namespace Oracle.RightNow.Cti.Configuration {
    [Export(typeof(IConfigurationProvider))]
    public class RightNowConnectConfigurationProvider : IConfigurationProvider {
        private RightNowObjectProvider _objectProvider;

        [ImportingConstructor]
        public RightNowConnectConfigurationProvider(IGlobalContext globalContext) {
            _objectProvider = new RightNowObjectProvider(globalContext);
        }

        public bool HideorDisabled { get; set; }

        public IEnumerable<AgentState> GetAgentStates() {
            Logger.Logger.Log.Debug("RightNowConnectConfigurationProvider Loading Standard AgentStates");
            // Get configured states      
            var states = new List<AgentState>();
            try
            {
                var customstates = new List<AgentState>(_objectProvider.GetObjects<AgentState>());

                HideorDisabled = customstates.FirstOrDefault(p => p.DefaultName == "Buttons").Name.Equals("true", StringComparison.OrdinalIgnoreCase) ? true : false;
                //// Add standard states
                foreach (AgentState st in customstates)
                {
                    switch (st.DefaultName)
                    {
                        case "Available":
                            StandardAgentStates.Available.Description = st.Description;
                            StandardAgentStates.Available.Name = st.Name;
                            break;
                        case "Wrap Up":
                            StandardAgentStates.WrapUp.Description = st.Description;
                            StandardAgentStates.WrapUp.Name = st.Name;
                            break;
                        case "Not Ready":
                            StandardAgentStates.NotReady.Description = st.Description;
                            StandardAgentStates.NotReady.Name = st.Name;
                            break;
                        case "New Reason":
                            StandardAgentStates.NewReason.Description = st.Description;
                            StandardAgentStates.NewReason.Name = st.Name;
                            break;
                        case "Logged In":
                            StandardAgentStates.LoggedIn.Description = st.Description;
                            StandardAgentStates.LoggedIn.Name = st.Name;
                            break;
                        case "Logged Out":
                            StandardAgentStates.LoggedOut.Description = st.Description;
                            StandardAgentStates.LoggedOut.Name = st.Name;
                            break;
                        case "Handling Interaction":
                            StandardAgentStates.InCall.Description = st.Description;
                            StandardAgentStates.InCall.Name = st.Name;
                            break;
                    }
                }

                states.Add(new AgentState().NewAgentState(StandardAgentStates.Available));
                states.Add(new AgentState().NewAgentState(StandardAgentStates.WrapUp));
                states.Add(new AgentState().NewAgentState(StandardAgentStates.NotReady));

                states.Add(new AgentState().NewAgentState(StandardAgentStates.NewReason));

                states.Add(new AgentState().NewAgentState(StandardAgentStates.LoggedIn));

                states.Add(new AgentState().NewAgentState(StandardAgentStates.LoggedOut));

                states.Add(new AgentState().NewAgentState(StandardAgentStates.InCall));
                states.Add(new AgentState().NewAgentState(StandardAgentStates.Calling));
                states.Add(new AgentState().NewAgentState(StandardAgentStates.Unknown));

            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("RightNowConnectConfigurationProvider Trace Error:"+ ex.StackTrace);
                Logger.Logger.Log.Error("RightNowConnectConfigurationProvider :", ex);
            }
            return states;
        }

        public ScreenPopConfig GetAgentScreenByProfile()
        {
            ScreenPopConfig conf = null;
            try
            {
                Logger.Logger.Log.Debug("RightNowConnectConfigurationProvider Getting ScreenPop ");

                conf = _objectProvider.GetScreenConfigByProfileId(Global.Context.ProfileId);
                if (conf.ScreenPopOptionsList == null)
                    conf = _objectProvider.GetScreenConfigByProfileId(0);
                
                Logger.Logger.Log.DebugFormat("ScreenPopConfigModel <<< get-agent-configuration-by-profile.");

                if (conf.ScreenPopOptionsList != null && conf.ScreenPopOptionsList.Where(p => p.Type == 0).GroupBy(p => new { p.Type, p.Name }).Count() != conf.ScreenPopOptionsList.Where(p => p.Type == 0).Count())
                {
                    List<ScreenPopOptions> tempdefault = new List<ScreenPopOptions>();
                    foreach (ScreenPopOptions pop in conf.ScreenPopOptionsList.Where(p => p.Type == 0))
                    {
                        ScreenPopOptions val = conf.ScreenPopOptionsList.FirstOrDefault(item => item.Type == 0 && item.Name == pop.Name);
                        if (val == null)
                            tempdefault.Add(pop);
                        else if (!tempdefault.Contains(val))
                            tempdefault.Add(val);
                    }
                    conf.ScreenPopOptionsList.RemoveAll(rem => rem.Type == 0);
                    tempdefault.ForEach((vap) => conf.ScreenPopOptionsList.Add(vap));
                }
                if (conf.ScreenPopOptionsList != null && conf.ScreenPopOptionsList.Where(p => p.Type == 1).GroupBy(p => new { p.Type, p.Name }).Count() != conf.ScreenPopOptionsList.Where(p => p.Type == 1).Count())
                {
                    List<ScreenPopOptions> tempdefault = new List<ScreenPopOptions>();
                    foreach (ScreenPopOptions pop in conf.ScreenPopOptionsList.Where(item => item.Type == 1))
                    {
                        ScreenPopOptions val = conf.ScreenPopOptionsList.FirstOrDefault(item => item.Type == 1 && item.Name == pop.Name);
                        if (val == null)
                            tempdefault.Add(pop);
                        else if (!tempdefault.Contains(val))
                            tempdefault.Add(val);
                    }
                    conf.ScreenPopOptionsList.RemoveAll(rem => rem.Type == 1);
                    tempdefault.ForEach((vap) => conf.ScreenPopOptionsList.Add(vap));
                }
                if (conf.ScreenPopOptionsList != null && conf.ScreenPopOptionsList.Where(p => p.Type == 2).GroupBy(p => new { p.Type, p.Name }).Count() != conf.ScreenPopOptionsList.Where(p => p.Type == 2).Count())
                {
                    List<ScreenPopOptions> tempdefault = new List<ScreenPopOptions>();
                    foreach (ScreenPopOptions pop in conf.ScreenPopOptionsList.Where(p => p.Type == 2))
                    {
                        ScreenPopOptions val = conf.ScreenPopOptionsList.FirstOrDefault(item => item.Type == 2 && item.Name == pop.Name);
                        if (val == null)
                            tempdefault.Add(pop);
                        else if (!tempdefault.Contains(val))
                            tempdefault.Add(val);
                    }
                    conf.ScreenPopOptionsList.RemoveAll(rem => rem.Type == 2);
                    tempdefault.ForEach((vap) => conf.ScreenPopOptionsList.Add(vap));
                }
                if (conf.ScreenPopOptionsList != null && conf.ScreenPopOptionsList.Where(p => p.Type == 3).GroupBy(p => new { p.Type, p.Name }).Count() != conf.ScreenPopOptionsList.Where(p => p.Type == 3).Count())
                {
                    List<ScreenPopOptions> tempdefault = new List<ScreenPopOptions>();
                    foreach (ScreenPopOptions pop in conf.ScreenPopOptionsList.Where(p => p.Type == 3))
                    {
                        ScreenPopOptions val = conf.ScreenPopOptionsList.FirstOrDefault(item => item.Type == 3 && item.Name == pop.Name);
                        if (val == null)
                            tempdefault.Add(pop);
                        else if (!tempdefault.Contains(val))
                            tempdefault.Add(val);
                    }
                    conf.ScreenPopOptionsList.RemoveAll(rem => rem.Type == 3);
                    tempdefault.ForEach((vap) => conf.ScreenPopOptionsList.Add(vap));
                }
                
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error(ex);
                return conf;
            }
            finally {
               
            }
            return conf;
        }
    }
}
