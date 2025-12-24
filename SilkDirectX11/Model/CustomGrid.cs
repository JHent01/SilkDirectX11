using LibraryForSignalR;
using SilkDirectX11.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SilkDirectX11.Model
{
    public class CustomGrid :Grid
    {
        public string ProcessTag { get; set; }
        public string WindowTag { get; set; }//??
        public CameraConnectStrings CameraConnectStrings { get; set; }
        public string CameraGuidName { get; set; }
        public Window Window { get; set; }//??
        public CustomGrid()
        {
           this.Background = System.Windows.Media.Brushes.Transparent;
           this.SizeChanged += CustomGrid_SizeChanged;
           this.IsVisibleChanged += CustomGrid_IsVisibleChanged;
           var g = System.Windows.Interop.RenderMode.SoftwareOnly;
           this.MinHeight= 50;
           this.MinWidth= 50;
           
            //Window.CacheMode =  CacheMode.CloneCurrentValue();
        }

        private void CustomGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
             if (this.Window != null&&this.Window.IsActive) Window.Visibility= this.Visibility;
        }

        internal void CustomGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Window != null)
            {

                Window.Left = this.PointToScreen(new System.Windows.Point()).X+5;
                Window.Top = this.PointToScreen(new System.Windows.Point()).Y+5;
                Window.Width = this.ActualWidth-10;
                Window.Height = this.ActualHeight-10;
                
                SetSize setSize = new SetSize((int)this.ActualWidth, (int)this.ActualHeight,int.Parse(this.ProcessTag));
                EventAggregatorProvider.Instance.Publish<SetSize>(setSize);
                 
                 
            }
        }
    }
}
//public class IsCamersSettingsViewModel<T> : BindableBase
//{
//    private T item;
//    public T Item
//    {
//        get => item;
//        set => item = value;
//    }

//    private Guid idCamera;
//    public Guid IdCamera
//    {
//        get => idCamera;
//        set => idCamera = value;

//    }

//    public IsCamersSettingsViewModel(T item)
//    {
//        Item = item;
//    }


//    // public event Action<bool>? IsSelectedChanged;

//    //protected virtual void OnIsSelectedChanged(bool newValue)
//    //{
//    //    IsSelectedChanged?.Invoke(newValue);
//    //}

//}