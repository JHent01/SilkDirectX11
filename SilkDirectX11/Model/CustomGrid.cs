using LibraryForSignalR;
using SilkDirectX11.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using System.Runtime.InteropServices;

namespace SilkDirectX11.Model
{
    


    public class CustomGrid :Grid
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);


        private readonly DispatcherTimer _resizeDebounceTimer;
        private int _pendingWidth;
        private int _pendingHeight;
        private bool _isResizing;
        private BitmapCache? _resizeCache;
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
           
           this.MinHeight= 50;
           this.MinWidth= 50;
           
            // this.SnapsToDevicePixels = true;
            // this.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);

            _resizeDebounceTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(150)
            };
            _resizeDebounceTimer.Tick += ResizeDebounceTimer_Tick;
            //this.Window.SizeToContent = SizeToContent.WidthAndHeight;
            //this.Window.Content= this;
        }

        

        private void BeginResizeVisualFreeze()
        {
            if (_isResizing) return;
            _isResizing = true;

            
            _resizeCache = new BitmapCache(1.0);
            this.CacheMode = _resizeCache;

           RenderOptions.SetEdgeMode(Window, EdgeMode.Aliased);
            RenderOptions.SetBitmapScalingMode(Window, BitmapScalingMode.LowQuality);
        }
        private void EndResizeVisualFreeze()
        {
            if (!_isResizing) return;
            _isResizing = false;

            this.CacheMode = null;
            _resizeCache = null;

           
            RenderOptions.SetBitmapScalingMode(Window, BitmapScalingMode.Unspecified);
        }
        private void ResizeDebounceTimer_Tick(object? sender, EventArgs e)
        {
            _resizeDebounceTimer.Stop();
 
            EndResizeVisualFreeze();

            if (_pendingWidth < 1 || _pendingHeight < 1) return;
            if (!int.TryParse(this.ProcessTag, out var procId)) return;

            var setSize = new SetSize(_pendingWidth, _pendingHeight, procId);
            EventAggregatorProvider.Instance.Publish<SetSize>(setSize);
        }
        
        private void CustomGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
             if (this.Window != null&&this.Window.IsActive) Window.Visibility= this.Visibility;
        }

        internal void CustomGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Window != null)
            {
                

                //(this.Window.Content as Grid).Width = this.ActualWidth-10;
                //(this.Window.Content as Grid).Height = this.ActualHeight-10;
                //this.Window.SizeToContent = SizeToContent.WidthAndHeight;
                //this.Window.Content= this;
                //RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
                //this.Window.CacheMode = null;
                Window.Left = this.PointToScreen(new System.Windows.Point()).X+5;
                  Window.Top = this.PointToScreen(new System.Windows.Point()).Y+5;
                 Window.Width = this.ActualWidth-10;
                  Window.Height = this.ActualHeight-10;
                
                 SetSize setSize = new SetSize((int)this.ActualWidth, (int)this.ActualHeight,int.Parse(this.ProcessTag));
               // EventAggregatorProvider.Instance.Publish<SetSize>(setSize);
                //BeginResizeVisualFreeze();


                _pendingWidth = (int)Math.Max(this.ActualWidth, this.MinWidth);
                _pendingHeight = (int)Math.Max(this.ActualHeight, this.MinHeight);
                // _resizeDebounceTimer.Stop();
                // _resizeDebounceTimer.Start();
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