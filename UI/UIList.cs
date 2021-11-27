using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
#nullable enable

namespace Monomon.UI
{
    public class UIItem<T> where T : IEquatable<T>
    {
        public UIItem(T item)
        {
            Item = item;
            OnSelected = x => { };
        }

        public UIItem(T item, Action<T> onSelected)
        {
            Item = item;
            OnSelected = onSelected;
        }

        public T Item { get; }
        public Action<T> OnSelected { get; }
        public bool Selected { get; set; }
    }

    public class UIList<T> where T : IEquatable<T>
    {
        public UIList(List<UIItem<T>> items, Action<T> onSelectionChanged, Action<T> onItemSelected)
        {
            Debug.Assert(items.Count > 0);
            Items = items;

            OnSelectionChanged = onSelectionChanged;
            OnItemSelected = onItemSelected;
            SelectedItem = Items.First().Item;
            selectedItem = SelectedItem;
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

        public void Select()
        {
            OnItemSelected(SelectedItem);
            var item = Items.First(x => x.Item.Equals(SelectedItem));
            item.OnSelected(item.Item);
        }

        public void SelectNext()
        {
            var item = Items.First(x => x.Item.Equals(selectedItem));
            item.Selected = false;
            var idx = Items.IndexOf(item) + 1;
            if (idx >= Items.Count)
                idx = 0;

            SelectedItem = Items[idx].Item;
            Items[idx].Selected = true;
        }
        public void SelectPrevious()
        {
            var item = Items.First(x => x.Item.Equals(selectedItem));
            item.Selected = false;
            var idx = Items.IndexOf(item)-1;
            if (idx < 0)
                idx = Items.Count - 1;

            SelectedItem = Items[idx].Item;
            Items[idx].Selected = true;
        }

        public List<UIItem<T>> Items { get; }
        public Action<T> OnSelectionChanged { get; }
        public Action<T> OnItemSelected { get; }
    }
}
