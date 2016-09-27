using Oracle.RightNow.Cti.Common;
using Oracle.RightNow.Cti.Common.ConnectService;
using Oracle.RightNow.Cti.ScreenPopConfiguration.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oracle.RightNow.Cti.ScreenPopConfiguration.Model
{

    public class ScreenPopConfigModel
    {
        #region singleton implementation

        private static object _syncScreenPop = new object();

        private static ScreenPopConfigModel _dataModel;
        ScreenPopProvider _screenConfigProvider;


        private ScreenPopConfigModel()
        {
            _screenConfigProvider = new ScreenPopProvider(Global.Context);
            this.Profiles = new List<NameValueViewModel>();
            this.DefaultSearchOptions = new List<ScreenPopOptions>();

            DefaultSearchOptions.Add(new ScreenPopOptions() { Name = "TranPop", Pop_Label = "Pop on Transfer and Conference? (Y/N/D)", Description = "Y", Type = 0});
            DefaultSearchOptions.Add(new ScreenPopOptions() { Name = "ANIPop", Pop_Label = "Pop on ANI?", Description = "Y", Type = 0});
            DefaultSearchOptions.Add(new ScreenPopOptions() { Name = "DNIS", Pop_Label = "DNIS", Description = "", Type = 0});
            DefaultSearchOptions.Add(new ScreenPopOptions() { Name = "UUI1", Pop_Label = "UUI Chat", Description = "", Type = 0 });
            DefaultSearchOptions.Add(new ScreenPopOptions() { Name = "UUI2", Pop_Label = "UUI Incident", Description = "", Type = 0 });
            DefaultSearchOptions.Add(new ScreenPopOptions() { Name = "UUI3", Pop_Label = "UUI 3", Description = "", Type = 0 });
            DefaultSearchOptions.Add(new ScreenPopOptions() { Name = "UUI4", Pop_Label = "UUI 4", Description = "", Type = 0 });
            DefaultSearchOptions.Add(new ScreenPopOptions() { Name = "UUI5", Pop_Label = "UUI 5", Description = "", Type = 0 });
            DefaultSearchOptions.Add(new ScreenPopOptions() { Name = "UUI6", Pop_Label = "UUI 6", Description = "", Type = 0 });
            DefaultSearchOptions.Add(new ScreenPopOptions() { Name = "UUI7", Pop_Label = "UUI 7", Description = "", Type = 0 });
            DefaultSearchOptions.Add(new ScreenPopOptions() { Name = "UUIStart", Pop_Label = "UUI Start", Description = ";", Type = 0});
            DefaultSearchOptions.Add(new ScreenPopOptions() { Name = "UUIStop", Pop_Label = "UUI Stop", Description = ":", Type = 0});
            DefaultSearchOptions.Add(new ScreenPopOptions() { Name = "UUISep", Pop_Label = "UUI Separator", Description = "!", Type = 0});

            // Default AgentStates
            DefaultAgentStates.Add(new AgentState() { ID = -1, DefaultName = "Available", Name = "Available", Description = "Available to receive interactions" });
            DefaultAgentStates.Add(new AgentState() { ID = -2, DefaultName = "Wrap Up", Name = "Wrap up", Description = "Wrap up work mode" });
            DefaultAgentStates.Add(new AgentState() { ID = -3, DefaultName = "Logged Out", Name = "Logged out", Description = "Logged out of the phone" });
            DefaultAgentStates.Add(new AgentState() { ID = -4, DefaultName = "Logged In", Name = "Logged in", Description = "Logged into the phone" });
            DefaultAgentStates.Add(new AgentState() { ID = -5, DefaultName = "Handling Interaction", Name = "Handling interaction", Description = "Currently handling an interaction" });
            DefaultAgentStates.Add(new AgentState() { ID = -6, DefaultName = "Not Ready", Name = "Not Ready", Description = "Not Ready" });
            DefaultAgentStates.Add(new AgentState() { ID = -7, DefaultName = "New Reason", Name = "New Reason", Description = "New Reason for Not Ready" });
            DefaultAgentStates.Add(new AgentState() { ID = -8, DefaultName = "Buttons", Name = "false", Description = "" });




        }

        public static ScreenPopConfigModel Instance
        {
            get
            {
                //double-checked locking, to avoid the expensive lock operation
                if (_dataModel == null)
                {
                    //Thread safe singleton implementation
                    lock (_syncScreenPop)
                    {
                        if (_dataModel == null)
                        {
                            _dataModel = new ScreenPopConfigModel();
                        }
                    }
                }
                return _dataModel;

            }
        }

        #endregion

        #region Cleanup methods

        /// <summary>
        /// For testability release the singleton instance every test run is completed
        /// </summary>
        internal void DisposeSingleton()
        {
            if (_dataModel != null)
            {
                _dataModel.Dispose();
                _dataModel = null;
            }
        }

        public void Dispose()
        {
            //GC.SuppressFinalize();
        }

        #endregion

        public List<NameValueViewModel> Profiles { get; set; }

        public List<ScreenPopOptions> DefaultSearchOptions { get; set; }

        private List<AgentState> _defaultAgentStates = new List<AgentState>();
        public List<AgentState> DefaultAgentStates
        {
            get
            {
                return _defaultAgentStates;
            }
            set
            {
                _defaultAgentStates = value;
            }
        }

        public List<NameValueViewModel> LoadProfiles()
        {

            Logger.Logger.Log.Debug("ScreenPopConfigModel >>> load-profiles");
            Dictionary<long, string> profiles = new Dictionary<long, string>();
            try
            {
                Profiles.Clear();
                profiles = _screenConfigProvider.GetNameValues("Account.Profile");
                foreach (long i in profiles.Keys)
                {
                    NameValueViewModel nameValueModel = new NameValueViewModel(i, profiles[i]);
                    Logger.Logger.Log.InfoFormat("ScreenPopConfigModel <<< profiles:{0}", nameValueModel);
                    Profiles.Add(nameValueModel);
                }
                Profiles.Insert(0, new NameValueViewModel(0, "_default"));
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("ScreenPopConfigModel <<< load-profiles failed!", ex);
            }
            return Profiles;
        }

        public List<ScreenPopOptions> LoadDefaultSearchOptions()
        {
            return DefaultSearchOptions;
        }

        public List<string> GetStandardWorkSpace()
        {
            return _screenConfigProvider.GetStandardWorkSpaceList();
        }

        public void SaveAgentConfiguration(ScreenPopConfig screenPoptConfig)
        {
            Logger.Logger.Log.Debug("AgentConfigModel >>> save-agent-configuration.");
            try
            {
                _screenConfigProvider.SaveAgentConfigData(screenPoptConfig);
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("ScreenPopConfigModel >>> save-agent-configuration failed.", ex);
            }
        }

        public void SaveAgentStates(List<AgentState> agentStates)
        {
            Logger.Logger.Log.Debug("AgentConfigModel >>> save-agent State.");
   
            try
            {
                _screenConfigProvider.SaveAgentStates(agentStates);
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("ScreenPopConfigModel >>> save-agents State failed.", ex);
            }
        }

        public ScreenPopConfig GetAgentConfigurationByProfile(int profileId)
        {
            Logger.Logger.Log.DebugFormat("ScreenPopConfigModel <<< get-agent-configuration-by-profile. ProfileId:{0}", profileId);
            try
            {
                ScreenPopConfig screepop = _screenConfigProvider.GetScreenPopConfigurationByProfile(profileId);

                if (screepop.ScreenPopOptionsList == null)
                {
                    screepop.ScreenPopOptionsList = new List<ScreenPopOptions>(DefaultSearchOptions.ToList());
                }
                else if (screepop.ScreenPopOptionsList != null && screepop.ScreenPopOptionsList.Where(p => p.Type == 0).Count() != DefaultSearchOptions.Count())
                {
                    List<ScreenPopOptions> tempdefault = new List<ScreenPopOptions>();
                    foreach (ScreenPopOptions pop in DefaultSearchOptions)
                    {
                        ScreenPopOptions val = screepop.ScreenPopOptionsList.FirstOrDefault(item => item.Type == 0 && item.Name == pop.Name);
                        if (val == null)
                            tempdefault.Add(pop);
                        else if (!tempdefault.Contains(val))
                            tempdefault.Add(val);
                    }
                    screepop.ScreenPopOptionsList.RemoveAll(rem => rem.Type == 0);
                    tempdefault.ForEach((vap) => screepop.ScreenPopOptionsList.Add(vap));
                }

                if (screepop.ScreenPopOptionsList != null && screepop.ScreenPopOptionsList.Where(p => p.Type == 1).GroupBy(p => new { p.Type, p.Name }).Count() != screepop.ScreenPopOptionsList.Where(p => p.Type == 1).Count())
                {
                    List<ScreenPopOptions> tempdefault = new List<ScreenPopOptions>();
                    foreach (ScreenPopOptions pop in screepop.ScreenPopOptionsList.Where(item => item.Type == 1))
                    {
                        ScreenPopOptions val = screepop.ScreenPopOptionsList.FirstOrDefault(item => item.Type == 1 && item.Name == pop.Name);
                        if (val == null)
                            tempdefault.Add(pop);
                        else if (!tempdefault.Contains(val))
                            tempdefault.Add(val);
                    }
                    screepop.ScreenPopOptionsList.RemoveAll(rem => rem.Type == 1);
                    tempdefault.ForEach((vap) => screepop.ScreenPopOptionsList.Add(vap));
                }
                if (screepop.ScreenPopOptionsList != null && screepop.ScreenPopOptionsList.Where(p => p.Type == 2).GroupBy(p => new { p.Type, p.Name }).Count() != screepop.ScreenPopOptionsList.Where(p => p.Type == 2).Count())
                {
                    List<ScreenPopOptions> tempdefault = new List<ScreenPopOptions>();
                    foreach (ScreenPopOptions pop in screepop.ScreenPopOptionsList.Where(p => p.Type == 2))
                    {
                        ScreenPopOptions val = screepop.ScreenPopOptionsList.FirstOrDefault(item => item.Type == 2 && item.Name == pop.Name);
                        if (val == null)
                            tempdefault.Add(pop);
                        else if (!tempdefault.Contains(val))
                            tempdefault.Add(val);
                    }
                    screepop.ScreenPopOptionsList.RemoveAll(rem => rem.Type == 2);
                    tempdefault.ForEach((vap) => screepop.ScreenPopOptionsList.Add(vap));

                }
                if (screepop.ScreenPopOptionsList != null && screepop.ScreenPopOptionsList.Where(p => p.Type == 3).GroupBy(p => new { p.Type, p.Name }).Count() != screepop.ScreenPopOptionsList.Where(p => p.Type == 3).Count())
                {
                    List<ScreenPopOptions> tempdefault = new List<ScreenPopOptions>();
                    foreach (ScreenPopOptions pop in screepop.ScreenPopOptionsList.Where(p => p.Type == 3))
                    {
                        ScreenPopOptions val = screepop.ScreenPopOptionsList.FirstOrDefault(item => item.Type == 3 && item.Name == pop.Name);
                        if (val == null)
                            tempdefault.Add(pop);
                        else if (!tempdefault.Contains(val))
                            tempdefault.Add(val);
                    }
                    screepop.ScreenPopOptionsList.RemoveAll(rem => rem.Type == 3);
                    tempdefault.ForEach((vap) => screepop.ScreenPopOptionsList.Add(vap));
                }

                return screepop;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("ScreenPopConfigModel <<< get-agent-configuration failed.", ex);
                return null;
            }
        }

        public List<AgentState> GetAgentStates()
        {
            Logger.Logger.Log.DebugFormat("ScreenPopConfigModel <<< Get-AgentState-configuration");
            try
            {
                List<AgentState> agentStates = _screenConfigProvider.GetAgentStates();
                if (agentStates == null)
                {
                    agentStates = new List<AgentState>(DefaultAgentStates.ToList());                    
                }
                else if (agentStates != null && agentStates.Count() != DefaultAgentStates.Count())
                {
                    List<AgentState> tempdefault = new List<AgentState>();
                    foreach (AgentState pop in DefaultAgentStates)
                    {
                        AgentState val = agentStates.FirstOrDefault(item => item.DefaultName == pop.DefaultName);                        
                        if (val == null)
                            tempdefault.Add(pop);
                        else if (!tempdefault.Contains(val))
                            tempdefault.Add(val);
                    }
                    agentStates.Clear();
                    tempdefault.ForEach((vap) => agentStates.Add(vap));
                }                
                return agentStates;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("ScreenPopConfigModel <<< get-agent-configuration failed.", ex);
                return null;
            }
        }
    }
}
