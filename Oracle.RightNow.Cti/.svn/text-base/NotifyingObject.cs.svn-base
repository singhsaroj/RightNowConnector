using System.ComponentModel;
using System.Runtime.Serialization;

namespace Oracle.RightNow.Cti
{
    [DataContract]
    public abstract class NotifyingObject : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}