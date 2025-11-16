using System;

namespace SilkDirectX11.Model
{
    // План (псевдокод):
    // - Добавить публичный ивент Action<bool> IsSelectedChanged
    // - В сеттере IsSelected при фактическом изменении значения вызывать OnIsSelectedChanged(newValue)
    // - Реализовать protected virtual void OnIsSelectedChanged(bool) для вызова ивента
    // - Убрать приватный ивент и Subscribe-метод, т.к. достаточно стандартной подписки на публичный ивент

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
