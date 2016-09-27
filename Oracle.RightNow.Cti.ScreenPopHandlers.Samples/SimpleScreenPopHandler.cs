// ===========================================================================================

//  Oracle RightNow Connect
//  CTI Sample Code
// ===========================================================================================
//  Copyright © Oracle Corporation.  All rights reserved.
// 
//  Sample code for training only. This sample code is provided "as is" with no warranties 
//  of any kind express or implied. Use of this sample code is pursuant to the applicable
//  non-disclosure agreement and or end user agreement and or partner agreement between
//  you and Oracle Corporation. You acknowledge Oracle Corporation is the exclusive
//  owner of the object code, source code, results, findings, ideas and any works developed
//  in using this sample code.
// ===========================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Oracle.RightNow.Cti.Model;
using RightNow.AddIns.AddInViews;
using RightNow.AddIns.Common;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using Oracle.RightNow.Cti.Configuration;

namespace Oracle.RightNow.Cti.ScreenPopHandlers.Samples
{
    /// <summary>
    /// This class is a simple screen pop handler that performs a contact search based on the source address 
    /// (phone number if the interaction is a phone call or email if an incident). If multiple matches are found, a report
    /// is executed to display the results, otherwise, the contact is opened.
    /// </summary>   

    [Export(typeof(IScreenPopHandler))]
    public class SimpleScreenPopHandler : IScreenPopHandler, IPartImportsSatisfiedNotification
    {
        private RightNowObjectProvider _objectProvider;
        private InteractionManager _interactionManager;

        [Import]
        public IGlobalContext RightNowGlobalContext { get; set; }

        readonly Dictionary<string, string> SEARCH_EXPRESSION = new Dictionary<string, string> 
        { 
            { "organization", "orgs.org_id" },
            { "contact","contacts.c_id"},
            { "incident", "incidents.i_id" },
            { "answer", "answers.a_id" },
            { "opportunity", "opportunities.op_id" },
            { "task", "tasks.task_id" }
        };

        readonly Dictionary<string, string> SEARCH_REPORT = new Dictionary<string, string> 
        { 
            { "organization", "OrgsReport" },
            { "contact","ContactsReport"},
            { "incident", "IncidentsReport" },
            { "answer", "AnswerReports" },
            { "opportunity", "OpportunitiesReport" },
            { "task", "TasksReport" }
        };

        /// <summary>
        /// Handle the Screen Pop to the user.
        /// </summary>
        /// <param name="interaction"></param>
        public void HandleInteraction(IInteraction interaction)
        {
            Boolean isChat,isIncident,isVoice=false;

            switch (interaction.Type)
            {
                case MediaType.Voice:

                    SynchronizationContext.Current.Post(scree =>
                    {
                        try
                        {
                            AsyncObservableCollection<Entities> UUIs = new AsyncObservableCollection<Entities>();
                            ScreenPopConfig screenPopConfigs = interaction.ScreenPopConfiguration;
                            
                            
                            if (screenPopConfigs.CanScreenPop)
                            {
                                string UUIvalue = interaction.InteractionData["UUI"];
                                ScreenPopOptions UUIstart = screenPopConfigs.ScreenPopOptionsList.SingleOrDefault(p => p.Type == 0 && p.Name.Equals("UUIStart", StringComparison.OrdinalIgnoreCase));
                                string start = UUIstart == null ? ";" : UUIstart.Description;
                                ScreenPopOptions UUIstop = screenPopConfigs.ScreenPopOptionsList.SingleOrDefault(p => p.Type == 0 && p.Name.Equals("UUIStop", StringComparison.OrdinalIgnoreCase));
                                string stop = UUIstop == null ? ":" : UUIstop.Description;
                                ScreenPopOptions UUIsep = screenPopConfigs.ScreenPopOptionsList.SingleOrDefault(p => p.Type == 0 && p.Name.Equals("UUISep", StringComparison.OrdinalIgnoreCase));
                                char seperator = UUIsep == null ? '!' : UUIsep.Description.ToCharArray()[0];
                                int startindex = UUIvalue.IndexOf(start, 0) + 1;
                                int lastindex = UUIvalue.LastIndexOf(stop) == -1 ? UUIvalue.Length : UUIvalue.LastIndexOf(stop);
                                int len = lastindex - startindex;
                                string[] uuival = UUIvalue.Substring(startindex, len).Split(seperator);
                                
                                isChat=Array.IndexOf(uuival,InteractionIQAType.FEAT_IQA_ORACLE_CHAT.ToString())>=0;
                                isIncident = Array.IndexOf(uuival, InteractionIQAType.FEAT_IQA_ORACLE_EMAIL.ToString()) >= 0;
                                
                                ArrayList uuiCI = new ArrayList();

                                //Namachi
                                Regex UUICIRegex = new Regex("^UUI[1-2]", RegexOptions.IgnoreCase);
                                List<ScreenPopOptions> UUICI = screenPopConfigs.ScreenPopOptionsList.Where(p => p.Type == 0 && UUICIRegex.Matches(p.Name).Count > 0).ToList();
                                Regex UUIRegex = new Regex("^UUI[3-7]", RegexOptions.IgnoreCase);
                                List<ScreenPopOptions> UUI = screenPopConfigs.ScreenPopOptionsList.Where(p => p.Type == 0 && UUIRegex.Matches(p.Name).Count > 0).ToList();

                                //foreach (string filterval in uuival) {
                                //    if (filterval != UUICI[0].Description && filterval != UUICI[1].Description) uuiCI.Add(filterval);
                                //}
                                for (int i = 0; i < uuival.Length; i++) {
                                    if (i == 0 || i == 1)
                                    {
                                        if ((uuival[i] != UUICI[0].Description && uuival[i] != UUICI[1].Description) && (uuival[i] != null && uuival[i] != "")) uuiCI.Add(uuival[i]);
                                    }
                                    else if (uuival[i] != UUICI[0].Description && uuival[i] != UUICI[1].Description)
                                    {
                                        uuiCI.Add(uuival[i]);
                                    }
                                }
                                

                                int index = 1;
                                bool IsMatched = false;
                                string searchvalue = "";
                                if ((interaction as ICall).CallType == CallType.Inbound)
                                {
                                    searchvalue = interaction.Address;
                                }
                                else
                                {
                                    string[] address = interaction.InteractionData["Consult"].Split('|');
                                    searchvalue = address[0];
                                }

                                AsyncObservableCollection<Entities> consolidatefilter = CombineEntity((string[]) uuiCI.ToArray(typeof (string)), UUI, searchvalue);

                                foreach (Entities filter in consolidatefilter)
                                {
                                    string workspace = filter.Entity;
                                    bool found = false;
                                    Entities searchval = SearchEntites(filter);
                                    if (searchval.IDs.Count > 0)
                                    {
                                        Entities dup = UUIs.FirstOrDefault(p => p.Entity.Equals(searchval.Entity, StringComparison.OrdinalIgnoreCase));
                                        if (dup != null)
                                        {
                                            searchval.IDs.ToList().ForEach(x => dup.IDs.Add(x.ToString()));
                                        }
                                        else
                                        {
                                            UUIs.Add(searchval);
                                        }

                                        found = true;
                                    }
                                    if (!IsMatched && found)
                                       IsMatched = found;
                                }                               

                                Open(UUIs);
                                UUIs.Clear();

                                
                                if ((interaction as ICall).CallType == CallType.Inbound)
                                {
                                    ScreenPopOptions aniscreenpop = screenPopConfigs.ScreenPopOptionsList.SingleOrDefault(p => p.Type == 0 && p.Name.Equals("ANIPop", StringComparison.OrdinalIgnoreCase));
                                    if (!IsMatched && aniscreenpop != null && aniscreenpop.Description == "Y")
                                    {
                                        bool found = false;// SearchAndPop("Contact", "Phones.RawNumber", "any_phone", searchvalue);

                                        Entities searchval = SearchEntites("Contact", "Phones.RawNumber", SEARCH_EXPRESSION["contact"].ToString(), searchvalue);
                                        //Entities searchval = SearchEntites("organization", "orgs.org_id", SEARCH_EXPRESSION["organization"].ToString(), searchvalue);
                                        if (searchval.IDs.Count > 0)
                                        {
                                            Entities dup = UUIs.FirstOrDefault(p => p.Entity.Equals(searchval.Entity, StringComparison.OrdinalIgnoreCase));
                                            if (dup != null)
                                            {
                                                searchval.IDs.ToList().ForEach(x => dup.IDs.Add(x.ToString()));
                                            }
                                            else
                                            {
                                                UUIs.Add(searchval);
                                            }

                                            found = true;
                                        }

                                        if (!IsMatched && found)
                                            IsMatched = found;
                                    }
                                        //if (!(screenPopConfigs.CanOpen) && !IsMatched && aniscreenpop.Description == "Y")
                                        //{
                                            //WorkspaceRecordType recordtype;
                                            //if (Enum.TryParse<WorkspaceRecordType>(screenPopConfigs.DefaultWorkSpace, out recordtype))
                                            //{
                                            //    IsMatched = true;
                                            //    RightNowGlobalContext.AutomationContext.CreateWorkspaceRecord(recordtype);
                                            //}
                                        //}

                                    WorkspaceRecordType recordtype;
                                    if (screenPopConfigs.ChatScreenPop)
                                    {
                                        if (isChat && !IsMatched)
                                        {
                                            if (screenPopConfigs.CanOpenChat == false)
                                            {
                                                if (Enum.TryParse<WorkspaceRecordType>(screenPopConfigs.ChatDefaultWorkspace, out recordtype))
                                                {
                                                    IsMatched = true;
                                                    RightNowGlobalContext.AutomationContext.CreateWorkspaceRecord(recordtype);
                                                }
                                            }
                                        }
                                    }
                                    if (screenPopConfigs.IncidentScreenPop)
                                    {
                                        if (isIncident && !IsMatched)
                                        {
                                            if (screenPopConfigs.CanOpenIncident == false)
                                            {
                                                if (Enum.TryParse<WorkspaceRecordType>(screenPopConfigs.IncidentDefaultWorkspace, out recordtype))
                                                {
                                                    IsMatched = true;
                                                    RightNowGlobalContext.AutomationContext.CreateWorkspaceRecord(recordtype);
                                                }
                                            }
                                        }
                                    }
                                    if (screenPopConfigs.VoiceScreenPop)
                                    {
                                        if (!isIncident && !isChat && !IsMatched)
                                        {
                                            if (screenPopConfigs.CanOpen == false)
                                            {
                                                if (Enum.TryParse<WorkspaceRecordType>(screenPopConfigs.DefaultWorkSpace, out recordtype))
                                                {
                                                    IsMatched = true;
                                                    RightNowGlobalContext.AutomationContext.CreateWorkspaceRecord(recordtype);
                                                }
                                            }
                                        }


                                    }
                                

                                    Logger.Logger.Log.Info(string.Format("ScreenPopHandler: Inbound ScreenPop Address {0}", searchvalue));
                                }
                                else if ((interaction as ICall).CallType == CallType.Consult)
                                {
                                    ScreenPopOptions aniscreenpop = screenPopConfigs.ScreenPopOptionsList.SingleOrDefault(p => p.Type == 0 && p.Name.Equals("TranPop", StringComparison.OrdinalIgnoreCase));
                                    if (!IsMatched && aniscreenpop != null && (aniscreenpop.Description == "Y" || aniscreenpop.Description == "D"))
                                    {

                                        bool found = false;// SearchAndPop("Contact", "Phones.RawNumber", "any_phone", searchvalue);
                                        //"organization", "orgs.org_id" 
                                        Entities searchval = SearchEntites("Contact", "Phones.RawNumber", SEARCH_EXPRESSION["contact"].ToString(), searchvalue);
                                        //Entities searchval = SearchEntites("organization", "organization.org_id", SEARCH_EXPRESSION["organization"].ToString(), searchvalue);
                                        if (searchval.IDs.Count > 0)
                                        {
                                            Entities dup = UUIs.FirstOrDefault(p => p.Entity.Equals(searchval.Entity, StringComparison.OrdinalIgnoreCase));
                                            if (dup != null)
                                            {
                                                searchval.IDs.ToList().ForEach(x => dup.IDs.Add(x.ToString()));
                                            }
                                            else
                                            {
                                                UUIs.Add(searchval);
                                            }

                                            found = true;
                                        }
                                        if (!IsMatched && found)
                                            IsMatched = found;
                                    }
                                    
                                         if (!(screenPopConfigs.CanOpen) && !IsMatched && (aniscreenpop.Description == "Y" || aniscreenpop.Description == "D"))
                                         {
                                             WorkspaceRecordType recordtype;
                                             if (Enum.TryParse<WorkspaceRecordType>(screenPopConfigs.DefaultWorkSpace, out recordtype))
                                             {
                                                 IsMatched = true;
                                                 RightNowGlobalContext.AutomationContext.CreateWorkspaceRecord(recordtype);
                                             }
                                         }
                                         Logger.Logger.Log.Info(string.Format("ScreenPopHandler: Consult ScreenPop Address {0}  UUI = {1}", searchvalue, UUIvalue));
                                   
                                    
                                }
                                Open(UUIs);

                                UUIs.Clear();
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Logger.Log.Error("ScreenPopHandler", ex);
                        }
                    }, null);

                    break;
                case MediaType.Web:
                case MediaType.Email:
                    RightNowGlobalContext.AutomationContext.EditWorkspaceRecord(WorkspaceRecordType.Incident, long.Parse(interaction.AdditionalIdentifiers["ReferenceId"]));
                    break;
                default:
                    return;
            }
        }

        public void OnImportsSatisfied()
        {
            _objectProvider = new RightNowObjectProvider(RightNowGlobalContext);
        }

        private void Open(AsyncObservableCollection<Entities> UUIs)
        {
            foreach (Entities entity in UUIs)
            {
                if (entity.IDs.Count > 1)
                {
                    var repfilter = new ReportFilter
                    {
                        Expression = entity.Expression,
                        OperatorType = ReportFilterOperatorType.InList,
                        Value = string.Join(",", entity.IDs)
                    };

                    IList<long> id = _objectProvider.GetObjectIds("AnalyticsReport", string.Format("LookupName" + "= '{0}'", SEARCH_REPORT[entity.Entity.ToLower()].ToString()));
                    RightNowGlobalContext.AutomationContext.RunReport((int)id[0], new List<IReportFilter2> { repfilter });
                }
                else
                {
                    WorkspaceRecordType recordtype;
                    if (Enum.TryParse<WorkspaceRecordType>(entity.Entity, out recordtype))
                    {
                        RightNowGlobalContext.AutomationContext.EditWorkspaceRecord(recordtype, Convert.ToInt64(entity.IDs[0]));
                    }
                }
            }
        }

        //public System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public void RunIncidents() {
            
            IncidentInfo incidentdet = _objectProvider.GetFirstIncidentDetails(RightNowGlobalContext.AccountId);
            RightNowGlobalContext.AutomationContext.EditWorkspaceRecord(WorkspaceRecordType.Incident, Convert.ToInt64(incidentdet.Id));
            //timer.Interval = 8000;
            //timer.Tick += timer_Tick;
            //timer.Start();
            
            //IList<IncidentInfo> pendingTask = _objectProvider.GetPendingIncidents(DateTime.Today.AddMonths(-1));
            //Logger.Logger.Log.Debug(string.Format("RunIncidents: pendingTask Count: ", pendingTask.Count));
            //if(pendingTask.Count >1){
            //    var incidentfilter = new ReportFilter
            //    {
            //        Expression = "Incident.ID",
            //        OperatorType = ReportFilterOperatorType.Equals,
            //        Value = "14565"
            //    };
            //    IList<long> objId = _objectProvider.GetObjectIds("Incident", string.Format("ID = '{0}'", "14565"));
            //    //IList<long> objId = _objectProvider.GetObjectIds("AnalyticsReport", string.Format("LookupName" + "= '{0}'", SEARCH_REPORT["incident"]));
            //    //RightNowGlobalContext.AutomationContext.CreateWorkspaceRecord("Create");
            //    //RightNowGlobalContext.AutomationContext.RunReport((int)pendingTask[0].Id, new List<IReportFilter2> { incidentfilter });
            //    RightNowGlobalContext.AutomationContext.RunReport((int)objId[0], new List<IReportFilter2> { incidentfilter });
            //}
        }

        //void timer_Tick(object sender, EventArgs e)
        //{
        //    IncidentInfo incidentdet = _objectProvider.GetFirstIncidentDetails(RightNowGlobalContext.AccountId);
        //    RightNowGlobalContext.AutomationContext.EditWorkspaceRecord(WorkspaceRecordType.Incident, Convert.ToInt64(incidentdet.Id));
        //    timer.Stop();
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workspace">Which workspace has to open</param>
        /// <param name="column">Where clause condition filter</param>
        /// <param name="expression">Expression for </param>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool SearchAndPop(string workspace, string column, string expression, string data)
        {
            bool isfound = false;
            IList<long> ids = _objectProvider.GetObjectIds(workspace, string.Format(column + "= '{0}'", data));
            var idCount = ids.Count();
            if (idCount > 1 && !string.IsNullOrEmpty(expression))
            {
                var repfilter = new ReportFilter
                {
                    Expression = expression,
                    OperatorType = ReportFilterOperatorType.Equals,
                    Value = data
                };
                ReportID reportid;
                if (Enum.TryParse<ReportID>(workspace, out reportid))
                {
                    isfound = true;
                    RightNowGlobalContext.AutomationContext.RunReport((int)reportid, new List<IReportFilter2> { repfilter });
                }
            }
            else if (idCount == 1)
            {
                WorkspaceRecordType recordtype;
                if (Enum.TryParse<WorkspaceRecordType>(workspace, out recordtype))
                {
                    isfound = true;
                    RightNowGlobalContext.AutomationContext.EditWorkspaceRecord(recordtype, ids[0]);
                }
            }
            return isfound;
        }

        private Entities SearchEntites(string workspace, string column, string expression, string data)
        {
            IList<long> ids = _objectProvider.GetObjectIds(workspace, string.Format(column + "= '{0}'", data));
            Entities finds = new Entities();
            finds.Entity = workspace;
            finds.Expression = expression;
            ids.ToList().ForEach(x => finds.IDs.Add(x.ToString()));
            return finds;
        }

        private Entities SearchEntites(Entities ent)
        {
            IList<long> ids = _objectProvider.GetObjectIds(ent.Entity, ent.Predicate);
            //Entities finds = new Entities();
            //finds.Entity = workspace;            

            ids.ToList().ForEach(x => ent.IDs.Add(x.ToString()));
            return ent;
        }

        private AsyncObservableCollection<Entities> CombineEntity(string[] uui, List<ScreenPopOptions> UUI, string searchvalue)
        {
            int index = 3;
            AsyncObservableCollection<Entities> UUIs = new AsyncObservableCollection<Entities>();
            foreach (string filter in uui)
            {
                if (!string.IsNullOrEmpty(filter))
                {
                    ScreenPopOptions val = UUI.SingleOrDefault(p => p.Name == ("UUI" + index) && p.Type == 0);
                    int pos = val.Description.IndexOf('.');
                    if (pos > 0)
                    {
                        string workspace = val.Description.Substring(0, pos);
                        Entities dup = UUIs.FirstOrDefault(p => p.Entity.Equals(workspace, StringComparison.OrdinalIgnoreCase));
                        if (dup != null)
                        {
                            dup.Predicate += " and " + string.Format(val.Description + "= '{0}'", filter);
                        }
                        else
                        {
                            Entities ent = new Entities();
                            ent.Entity = workspace;
                            ent.Predicate = string.Format(val.Description + "= '{0}'", filter);
                            ent.Expression = SEARCH_EXPRESSION[workspace.ToLower()].ToString();
                            UUIs.Add(ent);
                        }
                    }
                    else if (string.Equals(val.Description, "ani", StringComparison.OrdinalIgnoreCase))
                    {
                        Entities dup = UUIs.FirstOrDefault(p => p.Entity.Equals("Contact", StringComparison.OrdinalIgnoreCase));
                        Entities ent = new Entities();
                        ent.Entity = "Contact";
                        ent.Predicate = string.Format("Phones.RawNumber" + "= '{0}'", searchvalue);
                        ent.Expression = SEARCH_EXPRESSION["Contact".ToLower()].ToString();
                        UUIs.Add(ent);

                    }
                }
                index++;
            }

            return UUIs;
        }

        private enum ReportID
        {
            Organization = 123,//orgs.org_id
            Contact = 124,//contacts.c_id
            Incident = 120,//incidents.i_id
            Answer = 126,//answers.a_id
            Opportunity = 1032, //opportunities.op_id
            Task = 13033,//tasks.task_id
            //Quote = 8016,
            //Prod2Quote = 9013,
            //Generic = 0,
        }
    }

    internal class ReportFilter : IReportFilter2
    {
        public string Expression { get; set; }

        public ReportFilterOperatorType OperatorType { get; set; }

        public string Value { get; set; }
    }

    internal class Entities
    {

        public Entities()
        {
            IDs = new List<string>();
        }

        public string Entity;
        public List<string> IDs;
        public string Expression;
        public string Predicate;
    }
}