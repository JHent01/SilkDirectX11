using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkDirectX11.Model
{
    public class IsCamersSettingsViewModel<T> : BindableBase
    {
        private T item;
        public T Item
        {
            get => item;
            set => item= value;
        }

        private Guid idCamera ;
        public Guid IdCamera
        {
            get => idCamera;
            set=> idCamera= value;

        }

        public IsCamersSettingsViewModel(T item)
        {
            Item = item;
        }


       // public event Action<bool>? IsSelectedChanged;

        //protected virtual void OnIsSelectedChanged(bool newValue)
        //{
        //    IsSelectedChanged?.Invoke(newValue);
        //}

    }
}
