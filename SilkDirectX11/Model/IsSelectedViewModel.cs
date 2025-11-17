using System;

namespace SilkDirectX11.Model
{
     

    public class IsSelectedViewModel<T> : BindableBase
    {
        private T item;
        public T Item
        {
            get => item;
            set => SetProperty(ref item, value);
        }

        private bool isSelected = false;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    SetProperty(ref isSelected, value);
                    OnIsSelectedChanged(value);
                }
            }
        }

        public IsSelectedViewModel(T item)
        {
            Item = item;
        }

       
        public event Action<bool>? IsSelectedChanged;

        protected virtual void OnIsSelectedChanged(bool newValue)
        {
            IsSelectedChanged?.Invoke(newValue);
        }
    }
}
