using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Oracle.RightNow.Cti.Model;

namespace Oracle.RightNow.Cti.MediaBar.ViewModels {
    public class AgentLoginViewModel : ViewModel
    {
        private Contact _selectedContact;
        private string _userInput;
        private readonly Action<bool,Contact> _resultHandler;

        public AgentLoginViewModel(IList<Contact> contacts, Action<bool, Contact> resultHandler, string caption = "Agent Login",bool isQueueEnabled=false) {
            _resultHandler = resultHandler;
            Contacts = new ObservableCollection<Contact>(contacts);

            initializeCommands();
            Caption = caption;
            IsQueueEnabled = isQueueEnabled;
        }

        public ObservableCollection<Contact> Contacts { get; set; }

        public string Caption { get; set; }


        public bool IsQueueEnabled { get; set; }

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
        }
  
        private void digitInput(object obj) {
            UserInput += obj.ToString();
        }
    }
}
