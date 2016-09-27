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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.RightNow.Cti.Configuration;
using Oracle.RightNow.Cti.Model;

namespace Oracle.RightNow.Cti {
    [Export]   
    public class InteractionManager : IPartImportsSatisfiedNotification {
        [ImportingConstructor]
        public InteractionManager(IConfigurationProvider configurationProvider) {
            initialize(configurationProvider);

            ScreenPopHandlers = new List<IScreenPopHandler>();
        }
        static readonly object _Imgrobject = new object();
        public IEnumerable<AgentState> AgentStates { get; private set; }

        public IList<Contact> Contacts { get; set; }

        public SwitchCredentials Credentials { get; set; }

        public ScreenPopConfig AgentScreenPop { get; set; }

        public bool HideorDisabled { get; set; }

        [Import]
        public ICredentialsProvider CredentialsProvider { get; private set; }

        [Import]
        public IInteractionProvider InteractionProvider { get; private set; }

        [ImportMany(AllowRecomposition = true)]
        public ICollection<IScreenPopHandler> ScreenPopHandlers { get; set; }

        public void OnImportsSatisfied() {
            if (InteractionProvider != null) {
                InteractionProvider.Initialize();
            }          
                         
            InteractionProvider.NewInteraction += newInteractionHandler;            
            InteractionProvider.InteractionConnected += InteractionProvider_InteractionConnected;            
        }     

        void InteractionProvider_InteractionConnected(object sender, InteractionEventArgs e)
        {
            ///
            /// This for the when call is established instead of ringing mode.
            ///
           
            ///
            /// this for when call is transfere and conferenced  need to do Screen Popup Handler
            /// 
            //ScreenPopOptions aniscreenpop = AgentScreenPop.ScreenPopOptionsList.SingleOrDefault(p => p.Type == 0 && p.Name.Equals("TranPop", StringComparison.OrdinalIgnoreCase));
            //if (InteractionProvider.CurrentInteraction != null && ((InteractionProvider.CurrentInteraction as ICall).CallType == CallType.Consult && aniscreenpop != null && aniscreenpop.Description == "D"))             
            //{
            //    lock (_Imgrobject)
            //    {
            //        foreach (var handler in ScreenPopHandlers)
            //        {
            //            if (SynchronizationContext != null)
            //                SynchronizationContext.Post(o => ((InvocationTarget)o).HandleInteraction(), new InvocationTarget
            //                {
            //                    Interaction = e.Interaction,
            //                    Handler = handler
            //                });
            //            else
            //                handler.HandleInteraction(e.Interaction);
            //        }
            //        Logger.Logger.Log.Info(string.Format("Interaction >> {0}", e.Interaction));
            //    }
            //}
        }

        private void initialize(IConfigurationProvider configurationProvider) {
            try
            {
                AgentStates = configurationProvider.GetAgentStates();
                AgentScreenPop = configurationProvider.GetAgentScreenByProfile();
                HideorDisabled = configurationProvider.HideorDisabled;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Info(string.Format("Saroj Trapping issue >> {0}", ex.Message));
                
            }
            
        }

        public SynchronizationContext SynchronizationContext { get; set; }

        private void newInteractionHandler(object sender, InteractionEventArgs e)
        {

            ScreenPopOptions aniscreenpop = AgentScreenPop.ScreenPopOptionsList.SingleOrDefault(p => p.Type == 0 && p.Name.Equals("ANIPop", StringComparison.OrdinalIgnoreCase));
            ScreenPopOptions transaniscreenpop = AgentScreenPop.ScreenPopOptionsList.SingleOrDefault(p => p.Type == 0 && p.Name.Equals("TranPop", StringComparison.OrdinalIgnoreCase));

            // if ((e.Interaction as ICall).CallType == CallType.Inbound || ((e.Interaction as ICall).CallType == CallType.Consult))
            if (InteractionProvider.CurrentInteraction != null && (((InteractionProvider.CurrentInteraction as ICall).CallType == CallType.Inbound) || ((InteractionProvider.CurrentInteraction as ICall).CallType == CallType.Consult && transaniscreenpop != null && transaniscreenpop.Description == "Y")))
            {
                lock (_Imgrobject)
                {
                    foreach (var handler in ScreenPopHandlers)
                    {
                        if (SynchronizationContext != null)
                            SynchronizationContext.Post(o => ((InvocationTarget)o).HandleInteraction(), new InvocationTarget
                            {
                                Interaction = e.Interaction,
                                Handler = handler
                            });
                        else
                            handler.HandleInteraction(e.Interaction);
                    }
                    Logger.Logger.Log.Info(string.Format("InteractionManager: Address {0}, UUI {1}, {2}", e.Interaction.Address, e.Interaction.InteractionData["UUI"], e.Interaction));
                }
            }
        }

        private class InvocationTarget {
            public IInteraction Interaction { get; set; }

            public IScreenPopHandler Handler { get; set; }

            public void HandleInteraction() {
                Handler.HandleInteraction(Interaction);
            }
        }
    }
}