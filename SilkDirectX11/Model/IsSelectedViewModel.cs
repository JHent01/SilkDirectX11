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
            set { SetProperty(ref isSelected, value); SubscribeSelectionChanged(SelectionChanged); }// хуня такая
                                                                                                    // надо будет сделать ивентчто бы потом отпрвлялся запрос на изменения для ихменения главного чека 

        }
        public IsSelectedViewModel(T item )
        {
            Item = item;
            
        }
       event Action<bool>? SelectionChanged;
        public void SubscribeSelectionChanged(Action<bool> action)
        {
            SelectionChanged += action;
        }
         
    }
}
