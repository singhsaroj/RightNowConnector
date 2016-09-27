using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Oracle.RightNow.Cti.Model;

namespace Oracle.RightNow.Cti.MediaBar.ViewModels {
    public class TransferDialogViewModel : ViewModel {
        private Contact _selectedContact;
        private string _userInput;
        private readonly Action<bool,Contact> _resultHandler;
        private string _btnText;
        private bool _isConsultTransfer;
        private bool _transferOptionsVisible;

        public TransferDialogViewModel(IList<Contact> contacts, Action<bool, Contact> resultHandler, string caption = "Transfer") {
            _resultHandler = resultHandler;
            Contacts = new ObservableCollection<Contact>(contacts);

            initializeCommands();
            Caption = caption;
            _btnText = caption;
            IsBlindTransfer = true;
            _transferOptionsVisible = false;
            //if (caption.ToLower() == "dial" || caption.ToLower() == "send dtmf")
            //{
            //    _transferOptionsVisible = false;
            //}
            //else
            //{
            //    _transferOptionsVisible = true;
            //}
        }

        public ObservableCollection<Contact> Contacts { get; set; }

        public string Caption { get; set; }

        public Contact SelectedContact {
            get {
                return _selectedContact;
            }
            set {
                _selectedContact = value;
                OnPropertyChanged("SelectedContact");
            }
        }

        public string UserInput {
            get {
                return _userInput;
            }
            set {
                _userInput = value;
                OnPropertyChanged("UserInput");
            }
        }

        public string BtnText
        {
            get
            {
                return _btnText;
            }
            set
            {
                _btnText = value;
                OnPropertyChanged("BtnText");
            }
        }

        public bool IsBlindTransfer
        {
            get
            {
                return _isConsultTransfer;
            }
            set
            {
                _isConsultTransfer = value;
                OnPropertyChanged("IsBlindTransfer");
            }
        }

        public bool ShowTransferOptions
        {
            get
            {
                return _transferOptionsVisible;
            }
            set
            {
                _transferOptionsVisible = value;
                OnPropertyChanged("ShowTransferOptions");
            }
        }

        public ICommand DigitInputCommand { get; set; }
        public ICommand AcceptCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        private void initializeCommands() {
            DigitInputCommand = new DelegateCommand(digitInput);
            AcceptCommand = new DelegateCommand(accept);
            CancelCommand = new DelegateCommand(cancel);
        }
  
        private void cancel(object obj) {
            _resultHandler(false, null);
        }
  
        private void accept(object obj) {
            Contact contact = SelectedContact;
            if (contact ==null){
                contact = new Contact{
                    Description = UserInput,
                    Name= UserInput,
                    Number = UserInput,
                    TransferType = TransferTypes.Cold
                };
            }
            _resultHandler(true, contact);
            UserInput = "";
        }
  
        private void digitInput(object obj) {
            UserInput += obj.ToString();
        }
    }
}
