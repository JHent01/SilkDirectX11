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
        }

        private void CustomGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Window.Left = this.PointToScreen(new System.Windows.Point()).X;
            Window.Top = this.PointToScreen(new System.Windows.Point()).Y;
            Window.Width = this.ActualWidth;
            Window.Height = this.ActualHeight;
        }
    }
}
