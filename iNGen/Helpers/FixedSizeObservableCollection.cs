using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace iNGen.Helpers
{
    public class FixedSizeObservableCollection<T> : ObservableCollection<T>
    {
        public int MaxCollectionSize { get; set; }

        public FixedSizeObservableCollection(int maxCollectionSize = 0)
            : base()
        {
            MaxCollectionSize = maxCollectionSize;
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            if (MaxCollectionSize > 0 && MaxCollectionSize < Count)
            {
                int trimCount = Count - MaxCollectionSize;
                for (int i = 0; i < trimCount; i++)
                {
                    RemoveAt(0);
                }
            }
        }

        public override event NotifyCollectionChangedEventHandler CollectionChanged;
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler CollectionChanged = this.CollectionChanged;
            if (CollectionChanged != null)
                foreach (NotifyCollectionChangedEventHandler nh in CollectionChanged.GetInvocationList())
                {
                    DispatcherObject dispObj = nh.Target as DispatcherObject;
                    if (dispObj != null)
                    {
                        Dispatcher dispatcher = dispObj.Dispatcher;
                        if (dispatcher != null && !dispatcher.CheckAccess())
                        {
                            dispatcher.BeginInvoke(
                                (Action)(() => nh.Invoke(this,
                                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))),
                                DispatcherPriority.DataBind);
                            continue;
                        }
                    }
                    nh.Invoke(this, e);
                }
        }
    }
}