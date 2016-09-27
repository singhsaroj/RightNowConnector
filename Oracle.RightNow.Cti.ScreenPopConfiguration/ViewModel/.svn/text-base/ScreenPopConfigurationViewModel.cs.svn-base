using Oracle.RightNow.Cti.Common;
using Oracle.RightNow.Cti.ScreenPopConfiguration.Entities;
using Oracle.RightNow.Cti.ScreenPopConfiguration.Model;
using RightNow.AddIns.AddInViews;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Oracle.RightNow.Cti.ScreenPopConfiguration.ViewModel
{
    [Export(typeof(IScreenPopConfigurationViewModel))]
    public class ScreenPopConfigurationViewModel : NotifyingObject
    {

        public ScreenPopConfigurationViewModel()
        {
            ConfigCommand = new DelegateCommand(ExecuteConfigCommand);
            NewScreenConfigCommand = new DelegateCommand(ExecuteNewScreenConfig);
            
            NewScreenConfigChatCommand = new DelegateCommand(ExecuteNewScreenChatConfig);
            NewScreenConfigIncidentCommand = new DelegateCommand(ExecuteNewScreenIncidentConfig);
            VoiceCheckBoxChecked = new DelegateCommand(ExecuteVoiceCheckBoxChecked);
            
            WrapUpCommand = new DelegateCommand(ExecuteWrapUP);
            SaveScreenSettingCommands = new DelegateCommand(ExecuteSaveScreentSettings);
            RemoveWrapCommand = new DelegateCommand(ExecuteRemoveWrapUp);
            LogoutCommand = new DelegateCommand(ExecuteLogout);
            CancelScreenSettingCommands = new DelegateCommand(ExecuteCancelScreenSettings);
            RemoveLogoutCommand = new DelegateCommand(ExecuteRemoveLogout);
            NotReadyCommand = new DelegateCommand(ExecuteNotReady);
            RemoveNotReadyCommand = new DelegateCommand(ExecuteRemoveNotReady);
            SaveAgentStateCommand = new DelegateCommand(ExecuteSaveAgentStateCommand);
            HideOrDisableCommand = new DelegateCommand(ExecuteHideOrDisableCommand);
        }

        public ICommand ConfigCommand { get; set; }
        public ICommand NewScreenConfigCommand { get; set; }

        public ICommand NewScreenConfigChatCommand { get; set; }
        public ICommand NewScreenConfigIncidentCommand { get; set; }
        public ICommand VoiceCheckBoxChecked { get; set; }
        public ICommand WrapUpCommand { get; set; }
        public ICommand SaveScreenSettingCommands { get; set; }
        public ICommand RemoveWrapCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand RemoveLogoutCommand { get; set; }
        public ICommand NotReadyCommand { get; set; }
        public ICommand RemoveNotReadyCommand { get; set; }
        public ICommand CancelScreenSettingCommands { get; set; }
        public ICommand SaveAgentStateCommand { get; set; }
        public ICommand HideOrDisableCommand { get; set; }

        [Import]
        public IGlobalContext Context { get; set; }

        ScreenPopConfig screenpopConfigs;

        private AsyncObservableCollection<NameValueViewModel> _profiles = new AsyncObservableCollection<NameValueViewModel>();
        public AsyncObservableCollection<NameValueViewModel> Profiles
        {
            get
            {
                return _profiles;
            }
            set
            {
                _profiles = value;
                OnPropertyChanged("Profiles");
            }
        }

        private AsyncObservableCollection<string> _workSpaceList = new AsyncObservableCollection<string>();
        public AsyncObservableCollection<string> WorkSpaceList
        {
            get
            {
                return _workSpaceList;
            }
            set
            {
                _workSpaceList = value;
                OnPropertyChanged("WorkSpaceList");
            }
        }

        private AsyncObservableCollection<ScreenPopUpEntity> _searchPopOptions = new AsyncObservableCollection<ScreenPopUpEntity>();
        public AsyncObservableCollection<ScreenPopUpEntity> SearchPopOptions
        {
            get
            {
                return _searchPopOptions;
            }
            set
            {
                _searchPopOptions = value;
                OnPropertyChanged("SearchPopOptions");
            }
        }

        private int _selectedItem;
        public int SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
                UpdateTabItem(_selectedItem);
            }
        }

        private AsyncObservableCollection<ScreenPopUpEntity> _wrapupReasoncodes = new AsyncObservableCollection<ScreenPopUpEntity>();
        public AsyncObservableCollection<ScreenPopUpEntity> WrapupReasoncodes
        {
            get
            {
                return _wrapupReasoncodes;
            }
            set
            {
                _wrapupReasoncodes = value;
                OnPropertyChanged("WrapupReasoncodes");
            }
        }

        private AsyncObservableCollection<ScreenPopUpEntity> _logoutReasonCodes = new AsyncObservableCollection<ScreenPopUpEntity>();
        public AsyncObservableCollection<ScreenPopUpEntity> LogoutReasonCodes
        {
            get
            {
                return _logoutReasonCodes;
            }
            set
            {
                _logoutReasonCodes = value;
                OnPropertyChanged("LogoutReasonCodes");
            }
        }

        private AsyncObservableCollection<ScreenPopUpEntity> _auxReasonCodes = new AsyncObservableCollection<ScreenPopUpEntity>();
        public AsyncObservableCollection<ScreenPopUpEntity> AUXReasonCodes
        {
            get
            {
                return _auxReasonCodes;
            }
            set
            {
                _auxReasonCodes = value;
                OnPropertyChanged("AUXReasonCodes");
            }
        }

        private AsyncObservableCollection<AgentState> _agentStates = new AsyncObservableCollection<AgentState>();
        public AsyncObservableCollection<AgentState> AgentStates
        {
            get
            {
                return _agentStates;
            }
            set
            {
                _agentStates = value;
                OnPropertyChanged("AgentStates");
            }
        }

        private AsyncObservableCollection<AgentState> tempAgents = new AsyncObservableCollection<AgentState>();

        private NameValueViewModel _selectedProfile;
        public NameValueViewModel SelectedProfile
        {
            get
            {
                return _selectedProfile;
            }
            set
            {
                if (_selectedProfile != value)
                {
                    _selectedProfile = value;
                    OnPropertyChanged("SelectedProfile");
                    LoadScreenConfigSettings();
                }
            }
        }

        private string _selectedWorkSpaceList;
        public string SelectedWorkSpaceList
        {
            get
            {
                return _selectedWorkSpaceList;
            }
            set
            {
                if (_selectedWorkSpaceList != value)
                {
                    _selectedWorkSpaceList = value;
                    OnPropertyChanged("SelectedWorkSpaceList");

                }
            }
        }

        private string _selectedChatWorkSpaceList;
        public string SelectedChatWorkSpaceList
        {
            get
            {
                return _selectedChatWorkSpaceList;
            }
            set
            {
                if (_selectedChatWorkSpaceList != value)
                {
                    _selectedChatWorkSpaceList = value;
                    OnPropertyChanged("SelectedChatWorkSpaceList");

                }
            }
        }

        private string _selectedIncidentWorkSpaceList;
        public string SelectedIncidentWorkSpaceList
        {
            get
            {
                return _selectedIncidentWorkSpaceList;
            }
            set
            {
                if (_selectedIncidentWorkSpaceList != value)
                {
                    _selectedIncidentWorkSpaceList = value;
                    OnPropertyChanged("SelectedIncidentWorkSpaceList");

                }
            }
        }

        private string _wrapup;
        public string Wrapup
        {
            get { return _wrapup; }
            set
            {
                _wrapup = value;
                OnPropertyChanged("Wrapup");

            }
        }


        private string _logoutReason;
        public string LogoutReason
        {
            get { return _logoutReason; }
            set
            {
                _logoutReason = value;
                OnPropertyChanged("LogoutReason");

            }
        }

        private string _notReadyReason;
        public string NotReadyReason
        {
            get { return _notReadyReason; }
            set
            {
                _notReadyReason = value;
                OnPropertyChanged("NotReadyReason");

            }
        }

        private bool _canScreenPop;
        public bool CanScreenPop
        {
            get { return _canScreenPop; }
            set
            {
                _canScreenPop = value;
                OnPropertyChanged("CanScreenPop");
            }
        }

        private bool _isEnhanced;
        public bool IsEnhanced
        {
            get { return _isEnhanced; }
            set
            {
                _isEnhanced = value;
                OnPropertyChanged("Enhanced");
            }
        }

        private bool _voiceScreenPop;
        public bool VoiceScreenPop
        {
            get { return _voiceScreenPop; }
            set
            {
                _voiceScreenPop = value;
                OnPropertyChanged("VoiceScreenPop");
            }
        }
        private bool _chatScreenPop;
        public bool ChatScreenPop
        {
            get { return _chatScreenPop; }
            set
            {
                _chatScreenPop = value;
                OnPropertyChanged("ChatScreenPop");
            }
        }
        private bool _incidentScreenPop;
        public bool IncidentScreenPop
        {
            get { return _incidentScreenPop; }
            set
            {
                _incidentScreenPop = value;
                OnPropertyChanged("IncidentScreenPop");
            }
        }

        private bool _autoRecieve;
        public bool AutoRecieve
        {
            get { return _autoRecieve; }
            set
            {
                _autoRecieve = value;
                OnPropertyChanged("AutoRecieve");
            }
        }

        private bool _isDefaultConfig;
        public bool IsDefaultConfig
        {
            get { return _isDefaultConfig; }
            set
            {
                _isDefaultConfig = value;
                OnPropertyChanged("IsDefaultConfig");
                SetCustomConfigVisibility();
            }
        }

        private bool _isDontOpen;
        public bool IsDontOpen
        {
            get
            {
                return _isDontOpen;
            }
            set
            {
                _isDontOpen = value;
                OnPropertyChanged("IsDontOpen");

            }
        }

        private bool _isDontOpenChat;
        public bool IsDontOpenChat
        {
            get
            {
                return _isDontOpenChat;
            }
            set
            {
                _isDontOpenChat = value;
                OnPropertyChanged("IsDontOpenChat");

            }
        }

        private bool _isDontOpenIncident;
        public bool IsDontOpenIncident
        {
            get
            {
                return _isDontOpenIncident;
            }
            set
            {
                _isDontOpenIncident = value;
                OnPropertyChanged("IsDontOpenIncident");

            }
        }


        private bool _isWrapup;
        public bool IsWrapup
        {
            get { return _isWrapup; }
            set
            {
                _isWrapup = value;
                OnPropertyChanged("IsWrapup");

            }
        }

        private bool _isLogout;
        public bool IsLogout
        {
            get { return _isLogout; }
            set
            {
                _isLogout = value;
                OnPropertyChanged("IsLogout");

            }
        }

        private bool _isAuxReason;
        public bool IsAUXReason
        {
            get { return _isAuxReason; }
            set
            {
                _isAuxReason = value;
                OnPropertyChanged("IsAUXReason");

            }
        }

        private string _progressInfo;
        public string ProgressInfo
        {
            get
            {
                return _progressInfo;
            }

            set
            {
                _progressInfo = value;
                OnPropertyChanged("ProgressInfo");
            }
        }

        private string _agentProgressInfo;
        public string AgentProgressInfo
        {
            get
            {
                return _agentProgressInfo;
            }

            set
            {
                _agentProgressInfo = value;
                OnPropertyChanged("AgentProgressInfo");
            }
        }


        private Visibility _showProgress;
        public Visibility ShowProgress
        {
            get
            {
                return _showProgress;
            }
            set
            {
                _showProgress = value;
                OnPropertyChanged("ShowProgress");
            }
        }

        private Visibility _showAgentProgress;
        public Visibility ShowAgentProgress
        {
            get
            {
                return _showAgentProgress;
            }
            set
            {
                _showAgentProgress = value;
                OnPropertyChanged("ShowAgentProgress");
            }
        }

        private Visibility _agentVisibility;
        public Visibility AgentVisibility
        {
            get
            {
                return _agentVisibility;
            }
            set
            {
                _agentVisibility = value;
                OnPropertyChanged("AgentVisibility");
            }
        }



        private Visibility _showConfigAll;
        public Visibility ShowConfigAll
        {
            get
            {
                return _showConfigAll;
            }
            set
            {
                _showConfigAll = value;
                OnPropertyChanged("ShowConfigAll");
            }
        }

        private Visibility _showAll;
        public Visibility ShowAll
        {
            get
            {
                return _showAll;
            }
            set
            {
                _showAll = value;
                OnPropertyChanged("ShowAll");
            }
        }

        private Visibility _showCustomConfig;
        public Visibility ShowCustomConfig
        {
            get
            {
                return _showCustomConfig;
            }
            set
            {
                _showCustomConfig = value;
                OnPropertyChanged("ShowCustomConfig");
            }
        }


        private string _wrapLabel;
        public string WrapLabel
        {
            get { return _wrapLabel; }
            set
            {
                _wrapLabel = value;
                OnPropertyChanged("WrapLabel");

            }
        }

        private string _logoutLabel;
        public string LogoutLabel
        {
            get { return _logoutLabel; }
            set
            {
                _logoutLabel = value;
                OnPropertyChanged("LogoutLabel");

            }
        }

        private string _notReadyLabel;
        public string NotReadyLabel
        {
            get { return _notReadyLabel; }
            set
            {
                _notReadyLabel = value;
                OnPropertyChanged("NotReadyLabel");

            }
        }

        private string _primary;
        public string PrimaryCTIEngine
        {
            get { return _primary; }
            set
            {
                _primary = value;
                OnPropertyChanged("PrimaryCTIEngine");

            }
        }

        private string _secondary;
        public string SecondaryCTIEngine
        {
            get { return _secondary; }
            set
            {
                _secondary = value;
                OnPropertyChanged("SecondaryCTIEngine");

            }
        }

        private bool _isQueueEnabled;
        public bool IsQueueEnabled
        {
            get { return _isQueueEnabled; }
            set
            {
                _isQueueEnabled = value;
                OnPropertyChanged("IsQueueEnabled");
            }
        }

        private string _agentStateSaveOrUpdate;
        public string AgentStateSaveOrUpdate
        {
            get
            {
                return _agentStateSaveOrUpdate;
            }
            set
            {
                _agentStateSaveOrUpdate = value;
            }
        }

        private bool _hideOrdisable;
        public bool HideOrDisable
        {
            get
            {
                return _hideOrdisable;
            }
            set
            {
                _hideOrdisable = value;
                HindMsg = (_hideOrdisable) ? "(Behaviour in hidden mode)" : "(Behaviour in greyout mode)";
                OnPropertyChanged("HideOrDisable");
            }
        }

        private bool _enhanced;
        public bool Enhanced
        {
            get { return _enhanced; }
            set
            {
                _enhanced = value;
                OnPropertyChanged("Enhanced");
            }
        }

        private string _hintmsg;
        public string HindMsg
        {
            get
            {
                return _hintmsg;
            }
            set
            {
                _hintmsg = value;
                OnPropertyChanged("HindMsg");
            }
        }

        public AgentState Hide { get; set; }

        enum ScreenOptions
        {
            ScreenSearch = 0,
            WrapupReasonCodes = 1,
            LogoutReasonCodes = 2,
            AUXReasonCodes = 3
        }


        private void ExecuteConfigCommand(object parameter)
        {
            if (parameter.ToString() == "default")
            {
                IsDefaultConfig = true;
            }
            else
            {
                IsDefaultConfig = false;
            }
        }

        private void ExecuteRemoveWrapUp(object obj)
        {
            if (WrapupReasoncodes.Count > 0)
                WrapupReasoncodes.RemoveAt(WrapupReasoncodes.Count - 1);

            SetReasonCodeLabel();
        }

        private void ExecuteRemoveLogout(object obj)
        {
            if (LogoutReasonCodes.Count > 0)
                LogoutReasonCodes.RemoveAt(LogoutReasonCodes.Count - 1);

            SetReasonCodeLabel();
        }

        private void ExecuteRemoveNotReady(object obj)
        {
            if (AUXReasonCodes.Count > 0)
                AUXReasonCodes.RemoveAt(AUXReasonCodes.Count - 1);

            SetReasonCodeLabel();
        }

        private void ExecuteWrapUP(object obj)
        {
            int count = WrapupReasoncodes.Count();            
            WrapupReasoncodes.Add(new ScreenPopUpEntity() { Name = string.Format("Wrapup{0}", (count + 1).ToString("00")), Description = Wrapup, Pop_Label = "Wrap-up " + (count + 1), Type = (int)ScreenOptions.WrapupReasonCodes });            
            SetReasonCodeLabel();
            Wrapup = "";
        }

        private void ExecuteCancelScreenSettings(object obj)
        {
            Wrapup = "";
            LogoutReason = "";
            NotReadyReason = "";
            DefaultData();
        }

        private void ExecuteNotReady(object obj)
        {
            int count = AUXReasonCodes.Count();            
            AUXReasonCodes.Add(new ScreenPopUpEntity() { Name = string.Format("AUX{0}", (count + 1).ToString("00")), Description = NotReadyReason, Pop_Label = "Not Ready " + (count + 1), Type = (int)ScreenOptions.AUXReasonCodes });           
            SetReasonCodeLabel();
            NotReadyReason = "";
        }

        private void ExecuteLogout(object obj)
        {
            int count = LogoutReasonCodes.Count();
            LogoutReasonCodes.Add(new ScreenPopUpEntity() { Name = string.Format("Logout{0}", (count + 1).ToString("00")), Description = LogoutReason, Pop_Label = "Logout " + (count + 1), Type = (int)ScreenOptions.LogoutReasonCodes });            
            SetReasonCodeLabel();
            LogoutReason = "";
        }

        private void ExecuteNewScreenConfig(object parameter)
        {
            if (parameter.ToString() == "No")
            {
                IsDontOpen = false;
            }
            else
            {
                IsDontOpen = true;
            }

        }
        private void ExecuteNewScreenChatConfig(object parameter)
        {
            if (parameter.ToString() == "No")
            {
                IsDontOpenChat = false;
            }
            else
            {
                IsDontOpenChat = true;
            }

        }

        private void ExecuteVoiceCheckBoxChecked(object parameter)
        {
            if (parameter.ToString() == "False")
            { 
                SelectedWorkSpaceList = ""; 
            }
            else
            {
                WorkSpaceList = new AsyncObservableCollection<string>(ScreenPopConfigModel.Instance.GetStandardWorkSpace().ToList());
                SelectedWorkSpaceList = WorkSpaceList.FirstOrDefault(p => p.ToString() == screenpopConfigs.DefaultWorkSpace);
            }
        }

        private void ExecuteNewScreenIncidentConfig(object parameter)
        {
            if (parameter.ToString() == "No")
            {
                IsDontOpenIncident = false;
            }
            else
            {
                IsDontOpenIncident = true;
            }

        }

        public void DefaultData()
        {
            ProgressInfo = "Loading Default Data..";
            NameValueViewModel defaultprofile = Profiles.SingleOrDefault(p => p.Value == 0);

            if (defaultprofile != null)
                SelectedProfile = defaultprofile;
        }

        private void ExecuteSaveScreentSettings(object obj)
        {
            ProgressInfo = "Saving " + SelectedProfile.Name + "...";
            ShowAll = Visibility.Collapsed;
            ShowConfigAll = Visibility.Collapsed;
            ShowProgress = Visibility.Visible;
            System.Threading.Tasks.Task.Factory.StartNew(() =>
           {
               screenpopConfigs = new ScreenPopConfig();
               screenpopConfigs.AUXReasonEnabled = this.IsAUXReason;
               screenpopConfigs.CanScreenPop = this.CanScreenPop;
               screenpopConfigs.AutoRecieve = this.AutoRecieve;
               screenpopConfigs.VoiceScreenPop = this.VoiceScreenPop;
               screenpopConfigs.ChatScreenPop = this.ChatScreenPop;
               screenpopConfigs.IncidentScreenPop = this.IncidentScreenPop;
               screenpopConfigs.WrapupReasonEnabled = this.IsWrapup;
               screenpopConfigs.DefaultWorkSpace = SelectedWorkSpaceList;

               screenpopConfigs.IsEnhanced=this.Enhanced;

               screenpopConfigs.ChatDefaultWorkspace= SelectedChatWorkSpaceList;
               screenpopConfigs.IncidentDefaultWorkspace= SelectedIncidentWorkSpaceList;

               screenpopConfigs.CanOpen = IsDontOpen;
               screenpopConfigs.CanOpenChat = IsDontOpenChat;
               screenpopConfigs.CanOpenIncident = IsDontOpenIncident;
               
               screenpopConfigs.IsDefault = this.IsDefaultConfig;
               screenpopConfigs.LogoutReasonEnabled = this.IsLogout;
               screenpopConfigs.ProfileId = (int)SelectedProfile.Value;
               screenpopConfigs.PrimaryCTIEngine = (string.IsNullOrEmpty(PrimaryCTIEngine) ? " " : PrimaryCTIEngine);
               screenpopConfigs.SecondaryCTIEngine = (string.IsNullOrEmpty(SecondaryCTIEngine) ? " " : SecondaryCTIEngine);
               screenpopConfigs.IsQueueEnabled = this.IsQueueEnabled;
               screenpopConfigs.ScreenPopOptionsList = new List<ScreenPopOptions>();
               SearchPopOptions.ToList().ForEach(p =>
               {
                   screenpopConfigs.ScreenPopOptionsList.Add(new ScreenPopOptions() { Name = p.Name, Pop_Label = p.Pop_Label, Type = p.Type, Description = p.Description});
               });

               WrapupReasoncodes.ToList().ForEach(p =>
               {
                   screenpopConfigs.ScreenPopOptionsList.Add(new ScreenPopOptions() { Name = p.Name, Pop_Label = p.Pop_Label, Type = p.Type, Description = p.Description });
               });

               LogoutReasonCodes.ToList().ForEach(p =>
               {
                   screenpopConfigs.ScreenPopOptionsList.Add(new ScreenPopOptions() { Name = p.Name, Pop_Label = p.Pop_Label, Type = p.Type, Description = p.Description });
               });

               AUXReasonCodes.ToList().ForEach(p =>
               {
                   screenpopConfigs.ScreenPopOptionsList.Add(new ScreenPopOptions() { Name = p.Name, Pop_Label = p.Pop_Label, Type = p.Type, Description = p.Description });
               });

               ScreenPopConfigModel.Instance.SaveAgentConfiguration(screenpopConfigs);

               BindData();

           }).ContinueWith((s) =>
           {
               ShowConfigAll = Visibility.Visible;
               ShowProgress = Visibility.Collapsed;
               ShowAll = Visibility.Visible;
           });
        }

        private void ExecuteSaveAgentStateCommand(object obj)
        {
            AgentProgressInfo = string.Format("Saving Agent States...");
            ShowAll = Visibility.Collapsed;
            ShowAgentProgress = Visibility.Visible;
            AgentVisibility = Visibility.Collapsed;

            

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                Hide.Name = HideOrDisable.ToString();
                
                Hide.Description = string.Empty;
                AgentStates.Add(Hide);
                ScreenPopConfigModel.Instance.SaveAgentStates(AgentStates.ToList());
                ExecuteSaveScreentSettings(new object());
                BindAgentStates();
            }).ContinueWith((wait) =>
            {
                ShowAgentProgress = Visibility.Collapsed;
                AgentVisibility = Visibility.Visible;
                ShowAll = Visibility.Visible;
            });

        }

        private void ExecuteSaveCallConfigurationCommand(object obj)
        {
            AgentProgressInfo = string.Format("Saving Call Configurations...");
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                Hide.Name = HideOrDisable.ToString();
                Hide.Description = string.Empty;
                AgentStates.Add(Hide);
                ScreenPopConfigModel.Instance.SaveAgentStates(AgentStates.ToList());
                BindAgentStates();
            }).ContinueWith((wait) =>
            {
                ShowAgentProgress = Visibility.Collapsed;
                AgentVisibility = Visibility.Visible;
                ShowAll = Visibility.Visible;
            });

        }


        private void ExecuteHideOrDisableCommand(object obj)
        {

        }

        public void LoadProfiles()
        {
            ProgressInfo = "Loading profiles...";
            Profiles = new AsyncObservableCollection<NameValueViewModel>(ScreenPopConfigModel.Instance.LoadProfiles().ToList());
        }

        private void LoadWorkSpaceList()
        {
            ProgressInfo = "Loading Default Work Space...";
            WorkSpaceList = new AsyncObservableCollection<string>(ScreenPopConfigModel.Instance.GetStandardWorkSpace().ToList());
        }

        private void SetCustomConfigVisibility()
        {
            this.ShowCustomConfig = Visibility.Collapsed;
            if (SelectedProfile != null)
            {
                if (SelectedProfile.Value == 0 || (SelectedProfile.Value > 0 && !IsDefaultConfig))
                {
                    this.ShowCustomConfig = Visibility.Visible;
                }
            }
        }

        private void LoadScreenConfigSettings()
        {
            ProgressInfo = "Loading " + SelectedProfile.Name + "...";
            ShowConfigAll = Visibility.Collapsed;
            ShowProgress = Visibility.Visible;
            ShowAll = Visibility.Collapsed;
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                BindData();
                SetCustomConfigVisibility();

            }).ContinueWith((handleThread) =>
            {
                ProgressInfo = "";
                ShowProgress = Visibility.Collapsed;
                ShowConfigAll = Visibility.Visible;
            }).ContinueWith((handleThread) =>
            {
                AgentProgressInfo = "";
                ShowAgentProgress = Visibility.Collapsed;
                AgentVisibility = Visibility.Visible;
                ShowAll = Visibility.Visible;
            });
        }


        private void SetReasonCodeLabel()
        {
            this.WrapLabel = "Wrap-up " + (WrapupReasoncodes.Count() + 1);
            this.LogoutLabel = "Logout " + (LogoutReasonCodes.Count() + 1);
            this.NotReadyLabel = "Not Ready " + (AUXReasonCodes.Count() + 1);
        }


        private void BindData()
        {
            try
            {
                this.SearchPopOptions.Clear();
                this.LogoutReasonCodes.Clear();
                this.AUXReasonCodes.Clear();
                this.WrapupReasoncodes.Clear();
                this.Wrapup = "";
                this.LogoutReason = "";
                this.NotReadyReason = "";
                screenpopConfigs = ScreenPopConfigModel.Instance.GetAgentConfigurationByProfile(Int32.Parse(SelectedProfile.Value.ToString()));
                this.CanScreenPop = screenpopConfigs.CanScreenPop;
                this.AutoRecieve= screenpopConfigs.AutoRecieve;
                this.VoiceScreenPop = screenpopConfigs.VoiceScreenPop;
                this.ChatScreenPop = screenpopConfigs.ChatScreenPop;
                this.IncidentScreenPop = screenpopConfigs.IncidentScreenPop;
                this.IsDefaultConfig = screenpopConfigs.IsDefault;
                this.IsDontOpen = screenpopConfigs.CanOpen;
                this.IsDontOpenIncident = screenpopConfigs.CanOpenIncident;
                this.IsDontOpenChat = screenpopConfigs.CanOpenChat;
                this.IsWrapup = screenpopConfigs.WrapupReasonEnabled;
                this.IsLogout = screenpopConfigs.LogoutReasonEnabled;
                this.IsAUXReason = screenpopConfigs.AUXReasonEnabled;
                this.PrimaryCTIEngine = screenpopConfigs.PrimaryCTIEngine;
                this.SecondaryCTIEngine = screenpopConfigs.SecondaryCTIEngine;
                this.IsQueueEnabled = screenpopConfigs.IsQueueEnabled;
                this.AutoRecieve = screenpopConfigs.AutoRecieve;
                this.Enhanced = screenpopConfigs.IsEnhanced;

                if (!IsDontOpen)
                {
                    SelectedWorkSpaceList = WorkSpaceList.FirstOrDefault(p => p.ToString() == screenpopConfigs.DefaultWorkSpace);
                }
                if (!IsDontOpenChat)
                {
                    SelectedChatWorkSpaceList = WorkSpaceList.FirstOrDefault(p => p.ToString() == screenpopConfigs.ChatDefaultWorkspace);
                }
                if (!IsDontOpenIncident)
                {
                    SelectedIncidentWorkSpaceList = WorkSpaceList.FirstOrDefault(p => p.ToString() == screenpopConfigs.IncidentDefaultWorkspace);
                }

                if (screenpopConfigs.ScreenPopOptionsList != null)
                {

                    foreach (ScreenPopOptions opt in screenpopConfigs.ScreenPopOptionsList.Where(opt => opt.Type == (int)ScreenOptions.ScreenSearch))
                    {
                        SearchPopOptions.Add(new ScreenPopUpEntity() { ID = opt.ID, Pop_Label = opt.Pop_Label, Type = opt.Type, Description = opt.Description, Name = opt.Name });
                    }

                    foreach (ScreenPopOptions opt in screenpopConfigs.ScreenPopOptionsList.Where(opt => opt.Type == (int)ScreenOptions.WrapupReasonCodes))
                    {
                        WrapupReasoncodes.Add(new ScreenPopUpEntity() { ID = opt.ID, Pop_Label = opt.Pop_Label, Type = opt.Type, Description = opt.Description, Name = opt.Name });
                    }
                    foreach (ScreenPopOptions opt in screenpopConfigs.ScreenPopOptionsList.Where(opt => opt.Type == (int)ScreenOptions.LogoutReasonCodes))
                    {
                        LogoutReasonCodes.Add(new ScreenPopUpEntity() { ID = opt.ID, Pop_Label = opt.Pop_Label, Type = opt.Type, Description = opt.Description, Name = opt.Name });
                    }

                    foreach (ScreenPopOptions opt in screenpopConfigs.ScreenPopOptionsList.Where(opt => opt.Type == (int)ScreenOptions.AUXReasonCodes))
                    {
                        AUXReasonCodes.Add(new ScreenPopUpEntity() { ID = opt.ID, Pop_Label = opt.Pop_Label, Type = opt.Type, Description = opt.Description, Name = opt.Name });
                    }
                }
                SetReasonCodeLabel();
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("ScreenpopConfigurationViewModel: ", ex);
            }
        }

        private void BindAgentStates()
        {
            //AgentStates.Clear();
            List<AgentState> temp = ScreenPopConfigModel.Instance.GetAgentStates();
            temp.ForEach(x => { tempAgents.Add(x); });
            //tempAgents.ToList().ForEach(x => { if (!x.DefaultName.Equals("Buttons")) AgentStates.Add(x); });
            Hide = temp.FirstOrDefault(p => p.DefaultName == "Buttons");
            HideOrDisable = Hide.Name.Equals("true", StringComparison.OrdinalIgnoreCase) ? true : false;
        }

        public void LoadScreenPopConfigData()
        {
            ShowAll = Visibility.Collapsed;
            ShowProgress = Visibility.Visible;
            ShowConfigAll = Visibility.Collapsed;
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                this.LoadProfiles();
                this.LoadWorkSpaceList();
                this.DefaultData();
                SetReasonCodeLabel();

            }).ContinueWith((agentstates) =>
            {
                AgentProgressInfo = "Loading Agent States...";
                ShowAgentProgress = Visibility.Visible;
                AgentVisibility = Visibility.Collapsed;
                BindAgentStates();

            }).ContinueWith((wait) =>
            {
                ShowAll = Visibility.Visible;
                ShowProgress = Visibility.Collapsed;
                ShowConfigAll = Visibility.Visible;

            });
        }

        public void Intilization()
        {
            ShowAll = Visibility.Collapsed;
            ShowProgress = Visibility.Visible;
            ShowConfigAll = Visibility.Collapsed;
        }

        private void UpdateTabItem(int index)
        {
            if (index == 1)
            {
                AgentStates.Clear();
                tempAgents.ToList().ForEach(x => { if (!x.DefaultName.Equals("Buttons")) AgentStates.Add(x); });
                Hide = tempAgents.FirstOrDefault(p => p.DefaultName == "Buttons");
                HideOrDisable = Hide.Name.Equals("true", StringComparison.OrdinalIgnoreCase) ? true : false;
            }
        }
    }
}
