using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkDirectX11.Model
{
    public class IsSelectedViewModel<T> : BindableBase
    {private T item;
        public T Item 
        { get { return item; }
            set { SetProperty(ref item , value); }
        }
        private bool isSelected =false;
        public bool IsSelected
        {
            get { return isSelected; }
            set { SetProperty(ref isSelected, value); }
        }
        public IsSelectedViewModel(T item )
        {
            Item = item;
            
        }
        
       
    }
}
