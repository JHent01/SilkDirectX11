using LibraryForSignalR;
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
using SilkDirectX11.Servise;
using DevExpress.XtraPrinting.Native;
using ControlzEx.Standard;
using System.Diagnostics;
using SilkDirectX11.SignalR;

namespace SilkDirectX11.Model
{



    public class CustomGrid : Grid
    {
        public string ProcessTag { get; set; }
        public string WindowTag { get; set; } 
        public CameraConnectStrings CameraConnectStrings { get; set; }
        public string CameraGuidName { get; set; }
        public Window Window { get; set; } 
        public CustomGrid()
        {
            this.Background = System.Windows.Media.Brushes.Transparent;
            this.SizeChanged += CustomGrid_SizeChanged;
            this.IsVisibleChanged += CustomGrid_IsVisibleChanged;
           
            this.MinHeight = 50;
            this.MinWidth = 50;

        }

        

        private void CustomGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            if (this.Window != null && this.Window.IsActive) Window.Visibility = this.Visibility;
        }

        internal void CustomGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Window != null)
            {

                SetSize setSize = new SetSize((int)this.ActualWidth - 10, (int)this.ActualHeight - 10, int.Parse(this.ProcessTag));
                ConnectedManager.SendSetSize(setSize);
               // EventAggregatorProvider.Instance.Publish<SetSize>(setSize);
                
                Window.Left = this.PointToScreen(new System.Windows.Point()).X + 5;
                Window.Top = this.PointToScreen(new System.Windows.Point()).Y + 5;
                Window.Width = this.ActualWidth - 10;
                Window.Height = this.ActualHeight - 10;

            }
        }
        internal void ChengeSizeOverleyWindow(object sender, SizeChangedEventArgs e)
        {
            //if (this.Window != null)
           // {
               var wind = sender as Window;
                wind.OwnedWindows[0].Left = wind.PointToScreen(new System.Windows.Point()).X ;
                wind.OwnedWindows[0].Top = wind.PointToScreen(new System.Windows.Point()).Y;
                wind.OwnedWindows[0].Width = wind.ActualWidth;
                wind.OwnedWindows[0].Height = wind.ActualHeight;
            //}
        }
        internal void ChengeLocationOverleyWindow(object sender, EventArgs e)
        {
            //if (this.Window != null)
            //{
                var wind = sender as Window;
                wind.OwnedWindows[0].Left = wind.PointToScreen(new System.Windows.Point()).X;
                wind.OwnedWindows[0].Top = wind.PointToScreen(new System.Windows.Point()).Y;
            //}
        }
        //internal void Location( )
        //{
        //    Window.Left = this.PointToScreen(new System.Windows.Point()).X + 5;
        //    Window.Top = this.PointToScreen(new System.Windows.Point()).Y + 5;
        //}
    }

     
}
 