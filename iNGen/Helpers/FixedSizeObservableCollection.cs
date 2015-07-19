using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}