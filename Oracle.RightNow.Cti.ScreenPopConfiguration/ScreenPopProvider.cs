using Oracle.RightNow.Cti.Common;
using Oracle.RightNow.Cti.Common.ConnectService;
using Oracle.RightNow.Cti.ScreenPopConfiguration.Entities;
using Oracle.RightNow.Cti.ScreenPopConfiguration.Model;
using RightNow.AddIns.AddInViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oracle.RightNow.Cti.ScreenPopConfiguration
{
    public class ScreenPopProvider
    {
        private RightNowObjectProvider _rightNowObjectProvider;

        public ScreenPopProvider(IGlobalContext globalContext)
        {
            _rightNowObjectProvider = new RightNowObjectProvider(globalContext);            
        }

        public Dictionary<long, string> GetNameValues(string fieldName, string package = null)
        {
            return _rightNowObjectProvider.GetNameValues(fieldName);
        }

        public void SaveAgentConfigData(ScreenPopConfig screenPopConfigData)
        {
            try
            {
                Logger.Logger.Log.Debug(string.Format("ScreenPopProviders: {0}",screenPopConfigData));
                if (screenPopConfigData.ProfileId == 0 || (screenPopConfigData.ProfileId > 0 && !screenPopConfigData.IsDefault))
                {
                    ScreenPopConfig screenPopConfiguration = _rightNowObjectProvider.GetObject<ScreenPopConfig>(string.Format("where ProfileID={0}", screenPopConfigData.ProfileId));
                    CreateResponse createResponse = null;
                    UpdateResponse updateResponse = null;
                    long screenPopConfigId = 0;
                    if (screenPopConfiguration == null)
                    {
                        createResponse = _rightNowObjectProvider.CreateObject<ScreenPopConfig>(screenPopConfigData);
                        RNObject[] results = createResponse.RNObjectsResult;
                        if (results.Count() > 0)
                            screenPopConfigId = results[0].ID.id;
                    }
                    else
                    {
                        updateResponse = _rightNowObjectProvider.UpdateObject<ScreenPopConfig>(screenPopConfigData, screenPopConfiguration.ID);
                        screenPopConfigId = screenPopConfiguration.ID;
                    }

                    if (screenPopConfigId > 0)
                    {
                        this.Destroy(screenPopConfigId, false);
                        //foreach (ScreenPopOptions config in screenPopConfigData.ScreenPopOptionsList)
                        //{
                        //    config.ScreenPopConfigID = screenPopConfigId;
                        //    CreateResponse entityCreateResponse = _rightNowObjectProvider.CreateObject<ScreenPopOptions>(config);
                        //    RNObject[] rnObject = entityCreateResponse.RNObjectsResult;
                        //}

                        screenPopConfigData.ScreenPopOptionsList.ForEach(x => x.ScreenPopConfigID = screenPopConfigId);

                        CreateResponse entityCreateResponse = _rightNowObjectProvider.CreateObjects<ScreenPopOptions>(screenPopConfigData.ScreenPopOptionsList);
                    }
                }
                else if (screenPopConfigData.ProfileId > 0 && screenPopConfigData.IsDefault)
                {
                    ScreenPopConfig profile = _rightNowObjectProvider.GetObject<ScreenPopConfig>(string.Format("where ProfileID={0}", screenPopConfigData.ProfileId));
                    if (profile != null)
                    {
                        this.Destroy(profile.ID);
                    }
                }
                Logger.Logger.Log.Info(string.Format("ScreenPopProviders successfully Saved: {0}", screenPopConfigData));
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("Saving Screen Pop Settings :"+ ex.StackTrace);
                Logger.Logger.Log.Error("Saving Screen Pop Settings", ex);
            }
        }

        public void SaveAgentStates(List<AgentState> states)
        {
            try
            {
                Logger.Logger.Log.Debug(string.Format("Saving AgentStates:{0}", states.Count()));
                string id = "";
                IEnumerable<AgentState> lis = states.Where(p => p.ID > 0);
                foreach (AgentState st in lis)
                {
                    id += st.ID + ",";
                }
                if (id != "")
                {
                    List<AgentState> del = _rightNowObjectProvider.GetObjects<AgentState>(string.Format(" where Id not in ({0})", id.Substring(0, id.Length - 1))).ToList();
                    if (del != null && del.Count > 0)
                        _rightNowObjectProvider.DeleteObjects(del);
                }

                List<AgentState> instate = states.Where(x => x.ID < 0).ToList();
                if (instate.Count() > 0)
                {
                    _rightNowObjectProvider.CreateObjects(instate);
                }

                instate = states.Where(x => x.ID > 0).ToList();
                if (instate.Count() > 0)
                {
                    _rightNowObjectProvider.UpdateObjects(instate);
                }
                Logger.Logger.Log.Info(string.Format("Saved successfully:{0}",states.Count()  ));
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("Stack Trace:" + ex.StackTrace);
                Logger.Logger.Log.Error("Save agent states failed, Exception:", ex);
            }
            
        }

        public void Destroy(long configId, bool destroyProfile = true)
        {
            List<ScreenPopOptions> entities = new List<ScreenPopOptions>(_rightNowObjectProvider.GetObjects<ScreenPopOptions>(string.Format(" where ScreenPopConfigID={0}", configId)));
            if (entities.Count() > 0)
            {
               // foreach (ScreenPopOptions entity in entities)
                {
                    //_rightNowObjectProvider.DeleteObject<ScreenPopOptions>(entity.ID);
                }

                _rightNowObjectProvider.DeleteObjects(entities);

                //_rightNowObjectProvider.DestroyReasoncodes(configId);

                if (destroyProfile)
                {
                    _rightNowObjectProvider.DeleteObject<ScreenPopConfig>(configId);
                }
            }
        }

        public List<string> GetStandardWorkSpaceList()
        {
            return _rightNowObjectProvider.GetStandardWorkSpace();            
        }

        public ScreenPopConfig GetScreenPopConfigurationByProfile(int profileId)
        {
            return _rightNowObjectProvider.GetScreenConfigByProfileId(profileId);
        }

        public List<AgentState> GetAgentStates()
        {
            return _rightNowObjectProvider.GetAgentState();
        }
        
    }
}
