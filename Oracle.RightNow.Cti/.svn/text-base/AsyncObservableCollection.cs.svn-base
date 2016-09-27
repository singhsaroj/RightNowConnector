using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Threading;
using System.Collections.Generic;

namespace Oracle.RightNow.Cti
{
    public class AsyncObservableCollection<T> : ObservableCollection<T>
    {
        public AsyncObservableCollection()
            : base()
        {
        }
        public AsyncObservableCollection(List<T> list)
            : base(list)
        {
        }

        public override event NotifyCollectionChangedEventHandler CollectionChanged;
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler CollectionChanged = this.CollectionChanged;
            var eventHandler = CollectionChanged;
            if (eventHandler != null)
            {
                Dispatcher dispatcher = (from NotifyCollectionChangedEventHandler nh in eventHandler.GetInvocationList()
                                         let dpo = nh.Target as DispatcherObject
                                         where dpo != null
                                         select dpo.Dispatcher).FirstOrDefault();

                if (dispatcher != null && dispatcher.CheckAccess() == false)
                {
                    dispatcher.Invoke(DispatcherPriority.DataBind, (Action)(() => OnCollectionChanged(e)));
                }
                else
                {
                    foreach (NotifyCollectionChangedEventHandler notifyHandler in eventHandler.GetInvocationList())
                        notifyHandler.Invoke(this, e);
                }
            }
        }
    }
}
