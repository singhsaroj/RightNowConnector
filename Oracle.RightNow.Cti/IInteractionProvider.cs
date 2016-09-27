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
using System.Net;
using Oracle.RightNow.Cti.Model;

namespace Oracle.RightNow.Cti {
    /// <summary>
    /// Defines an interaction manager, which manages interactions with a CTI/ACD platform.
    /// </summary>
    public interface IInteractionProvider {
        /// <summary>
        /// Fired when the current call changes.
        /// </summary>
        event EventHandler<InteractionEventArgs> CurrentInteractionChanged;

        /// <summary>
        /// Occurs when a custom notification/event is available from the provider.
        /// </summary>
        event EventHandler<CustomEventArgs> CustomProviderEvent;

        /// <summary>
        /// Fired when the interaction session is finished.
        /// </summary>
        event EventHandler<InteractionEventArgs> InteractionCompleted;

        /// <summary>
        /// Fired when the interaction session is Conferenced.
        /// </summary>
        event EventHandler<InteractionEventArgs> ScreenPopEvent;

        /// <summary>
        /// Fired when the interaction is connected to an agent.
        /// </summary>
        event EventHandler<InteractionEventArgs> InteractionConnected;

        /// <summary>
        /// Fired when the interaction is disconnected.
        /// </summary>
        event EventHandler<InteractionEventArgs> InteractionDisconnected;

        /// <summary>
        /// Fired when a new interaction is available.
        /// </summary>
        event EventHandler<InteractionEventArgs> NewInteraction;

        /// <summary>
        /// Gets the agent manager.
        /// </summary>
        IAgent Agent { get; }

        /// <summary>
        /// The switch credentials.
        /// </summary>
        SwitchCredentials Credentials { get; }

        /// <summary>
        /// Gets an instance of the current device.
        /// </summary>
        IDevice Device { get; }

        /// <summary>
        /// Gets or sets the endpoint used by this provider.
        /// </summary>
        IList<DnsEndPoint> Endpoints { get; set; }

        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the platform the provider enables connectivity with.
        /// </summary>
        string Platform { get; }

        /// <summary>
        /// Gets a collection of provider specific properties.
        /// </summary>
        Dictionary<string, string> Properties { get; }

        /// <summary>
        /// This method is called by the runtime when an interaction is completed and the agent has finished
        /// all work on the current session.
        /// </summary>
        void CompleteInteraction();

        /// <summary>
        /// Initializes the interaction provider
        /// </summary>
        /// <param name="device">The current device.</param>
        void Initialize();

        void ReInitialize(string agentID, string password, string location, string queue);
        /// <summary>
        /// Performs a login against the switch.
        /// </summary>
        /// 
        void Login();

        void Login(string agentId, string password, string extension, string queue);

        void ReLogin(string agentID, string password, string location, string queue);
        /// <summary>
        /// Logs out of the switch.
        /// </summary>
        void Logout();

        /// <summary>
        /// Sends a notification to the interaction platform.
        /// </summary>
        /// <param name="notification">The notification to be sent.</param>
        void SendNotification(CtiNotification notification);


        /// <summary>
        /// Gets or sets the curret active interaction.
        /// </summary>
        IInteraction CurrentInteraction { get; set; }

        /// <summary>
        /// Gets a collection with all the interactions in the current session.
        /// </summary>
        AsyncObservableCollection<IInteraction> Interactions { get; }
    }

    public interface IInteractionProvider<T> : IInteractionProvider where T : IInteraction {
        /// <summary>
        /// Gets the curret active interaction if one is available; oterwise, null.
        /// </summary>
        T CurrentInteraction { get; }

        /// <summary>
        /// Gets a collection with all the interactions in the current session.
        /// </summary>
        AsyncObservableCollection<T> Interactions { get; }
    }
}