using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Monomon.UI
{
    public class UIItem
    { 
    }

    public class UIList<T> where T : IEquatable<T>
    {
        public UIList(List<T> items, Action<T> onSelectionChanged)
        {
            Debug.Assert(items.Count > 0);
            Items = items;
            OnSelectionChanged = onSelectionChanged;
            SelectedItem = Items.First();
        }

        private T selectedItem;

        public T SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnSelectionChanged(value);
            }
        }

        public void SelectNext()
        {
            var idx = Items.IndexOf(selectedItem) + 1;
            if (idx >= Items.Count)
                idx = 0;

            SelectedItem = Items[idx];
        }
        public void SelectPrevious()
        {
            var idx = Items.IndexOf(selectedItem) - 1;
            if (idx < 0)
                idx = Items.Count - 1;

            SelectedItem = Items[idx];
        }

        public List<T> Items { get; }
        public Action<T> OnSelectionChanged { get; }
    }
}
