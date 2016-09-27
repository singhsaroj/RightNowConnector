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
using System.Drawing;
using System.Linq;

namespace Oracle.RightNow.Cti.MediaBar {
    public class MediaBarButton : NotifyingObject {
        private string _name;
        private string _toolTip;
        private bool _isEnabled;
        private bool _isVisible;
        private Image _image;

        public event EventHandler Click;

        /// <summary>
        /// Gets or sets the ID value of this button.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the button's name.
        /// </summary>
        public string Name {
            get {
                return _name;
            }
            set {
                if (_name != value) {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets or sets the tooltip text displayed for this button.
        /// </summary>
        public string ToolTip {
            get {
                return _toolTip;
            }
            set {
                if (_toolTip != value) {
                    _toolTip = value;
                    OnPropertyChanged("ToolTip");
                }
            }
        }

        /// <summary>
        /// Gets or sets the image displayed for this button.
        /// </summary>
        public Image Image {
            get {
                return _image;
            }
            set {
                if (_image != value) {
                    _image = value;
                    OnPropertyChanged("Image");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this button is enabled in the user interface (UI).
        /// </summary>
        public bool IsEnabled {
            get {
                return _isEnabled;
            }
            set {
                if (_isEnabled != value) {
                    _isEnabled = value;
                    OnPropertyChanged("IsEnabled");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this button is visible in the user interface (UI).
        /// </summary>
        public bool IsVisible {
            get {
                return _isVisible;
            }
            set {
                if (_isVisible != value) {
                    _isVisible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }

        internal void RaiseClick() {
            OnClick();
        }
  
        /// <summary>
        /// Called when a MediaBarButton is clicked.
        /// </summary>
        protected virtual void OnClick() {
            var handler = Click;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }
    }
}