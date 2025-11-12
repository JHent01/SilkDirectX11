using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SilkDirectX11.Views
{
    /// <summary>
    /// Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainView
    {
       // private IRenderInstance renderInstance;
        public MainView()
        {
            InitializeComponent();
        }
        private void VideoCanvas1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //var grid = sender as Grid;

            //if (grid.Children.Count < 2) return;
            //var host1 = grid.Children[1];
            //var host = host1 as WindowsFormsHost;
            //if (host != null)
            //{
            //    host.Width = e.NewSize.Width;
            //    host.Height = e.NewSize.Height;
            //    var t = host.Child as System.Windows.Forms.Panel;
            //    t.Width = (int)e.NewSize.Width;
            //    t.Height = (int)e.NewSize.Height;
            //    if (grid.ColumnDefinitions.Count > 1)
            //    {
            //        t.Width = (int)e.NewSize.Width / grid.ColumnDefinitions.Count;
            //    }
            //    else if (grid.RowDefinitions.Count > 1)
            //    {
            //        t.Height = (int)e.NewSize.Height / grid.RowDefinitions.Count;
            //    }
            //  //  renderInstance?.Resize((int)e.NewSize.Width, (int)e.NewSize.Height);
            //}



            //System.Windows.Shapes.Rectangle rectangle = sender as System.Windows.Shapes.Rectangle;
            //var canvas = VisualTreeHelper.GetParent(rectangle) as Canvas;
            //var x = rectangle.PointToScreen(new Point()).X;
            //double left = Canvas.GetLeft(rectangle);
            //var y = rectangle.PointFromScreen(new Point()).Y;
            //double top = Canvas.GetTop(rectangle);
            //double right = Canvas.GetRight(rectangle);
            //double bottom = Canvas.GetBottom(rectangle);

        }
    }
}
