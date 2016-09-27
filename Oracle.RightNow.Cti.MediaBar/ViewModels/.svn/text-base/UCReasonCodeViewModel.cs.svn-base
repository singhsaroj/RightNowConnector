using Oracle.RightNow.Cti.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Oracle.RightNow.Cti.MediaBar.ViewModels
{
    public class UCReasonCodeViewModel : NotifyingObject
    {
        private AgentState _selectedAgentState;        
        private readonly Action<bool, AgentState> _resultHandler;
        
        public UCReasonCodeViewModel(IList<AgentState> AgentStates, Action<bool, AgentState> resultHandler)
        {
            _resultHandler = resultHandler;
            ReasonCodes = new List<AgentState>(AgentStates.Where(p=>!string.IsNullOrWhiteSpace(p.Description)));
            OkReasonCode = new DelegateCommand(o => ExecuteOkReasonCode());
            CancelReasonCode = new DelegateCommand(o => ExecuteCanceReasonCode());
            SelectedReasonCode = ReasonCodes.Count()>0? ReasonCodes[0] : null;
            OnPropertyChanged("SelectedReasonCode");
        }


        public List<AgentState> ReasonCodes { get; set; }

        private AgentState _selectedReasonCode;
        public AgentState SelectedReasonCode
        {
            get {
                return _selectedAgentState;
            }
            set
            {
                _selectedAgentState = value;
                OnPropertyChanged("SelectedReasonCode");
            }
        }
        public ICommand OkReasonCode { get; set; }
        public ICommand CancelReasonCode { get; set; }

        public void ExecuteOkReasonCode()
        {
            AgentState agentState = SelectedReasonCode;
            //if (agentState == null)
            //{
            //    agentState = new AgentState
            //    {
                    
            //    };
            //}
            _resultHandler(true, agentState);
        }
        
        public void ExecuteCanceReasonCode()
        {
            _resultHandler(false, null);
        }
    }
}
