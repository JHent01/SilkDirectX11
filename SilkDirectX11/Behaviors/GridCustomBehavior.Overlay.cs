using SilkDirectX11.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Button = System.Windows.Controls.Button;

namespace SilkDirectX11.Behaviors
{
    partial class GridCustomBehavior
    {
        private void InitWindow()
        {


            Grid Rite = AssociatedObject as Grid;


            window.Background = System.Windows.Media.Brushes.Transparent;
            window.WindowStyle = WindowStyle.None;
            window.AllowsTransparency = true;
            window.ShowInTaskbar = false;


            Button buttonOverlay = new Button
            {
                Content = "X",
                Width = 30,
                Height = 30,

                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,

            };
            Border border = new Border
            {

                Width = window.Width,
                Height = window.Height,


            };

            buttonOverlay.Click += ButtonDeleteChildren;

            Grid gridOverlay = new Grid()
            {
                Visibility = Visibility.Collapsed,
                Background = System.Windows.Media.Brushes.Black,
                Opacity = 0.5,
                Width = buttonOverlay.Width,
                Height = buttonOverlay.Height,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
            };
            Grid gridOll = new Grid()
            {
                Background = System.Windows.Media.Brushes.Transparent,

                Width = window.Width,
                Height = window.Height,
            };
            gridOverlay.Children.Add(buttonOverlay);
            gridOll.Children.Add(gridOverlay);
            border.Child = gridOll;

            window.Content = border;

            window.Width = Rite.ActualWidth / Rite.ColumnDefinitions.Count;
            if (Rite.RowDefinitions.Count != 0) window.Height = Rite.ActualHeight / Rite.RowDefinitions.Count;
            else window.Height = Rite.ActualHeight;

            window.Visibility = Visibility.Visible;


            gridOverlay.MouseMove += WindowShow;

            gridOverlay.MouseLeave += Leave;

        }
        private void Leave(object? sender, EventArgs e)
        {
            if (!flagForOverlay) return;
            Border border = (Border)window.Content;
            Grid grids = (Grid)border.Child;
            Grid grid = grids.Children.OfType<Grid>().FirstOrDefault();
            grid.Visibility = Visibility.Hidden;
        }

        private void WindowShow(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            var panel = sender as BetterPanelTest;
            Border border = (Border)window.Content;
            Grid grids = (Grid)border.Child;
            Grid grid = grids.Children.OfType<Grid>().FirstOrDefault();
            Button b = grid.Children.OfType<Button>().FirstOrDefault();
            b.Name = panel.Name;

            grid.Visibility = Visibility.Visible;
            window.Height = panel.Height;
            window.Width = panel.Width;
            window.Left = panel.PointToScreen(new System.Drawing.Point()).X;
            window.Top = panel.PointToScreen(new System.Drawing.Point()).Y;
        }


        private void Leave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            flagForOverlay = true;
            Grid grd = sender as Grid;
            grd.Visibility = Visibility.Hidden;


        }

        private void WindowShow(object s, System.Windows.Input.MouseEventArgs ev)
        {
            flagForOverlay = false;
            Grid grid = s as Grid;

            grid.Visibility = Visibility.Visible;
        }
        private void CreateCanvalInOverlay()
        {
            var riteGrid = AssociatedObject as Grid;
            //var full = riteGrid.Parent as Grid;
            Grid gridFullScreen = riteGrid.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();

            Grid gridOverlay = ((Border)window.Content).Child as Grid;
            var child = gridOverlay.Children.OfType<Canvas>().FirstOrDefault();

            int buffer = 2;
            if (child != null)
            {
                buffer = 1;
                gridOverlay.Children.Remove(child);
            }
            Canvas canvas = new Canvas()
            {
                Width = window.Width,
                Height = window.Height,
                Background = System.Windows.Media.Brushes.Transparent,
                Tag = gridFullScreen.Tag

            };
            canvas.Children.Add(new System.Windows.Shapes.Rectangle
            {
                Width = (pixelPanelForZoom.BottomRight.X - pixelPanelForZoom.TopLeft.X),
                Height = pixelPanelForZoom.BottomRight.Y - pixelPanelForZoom.TopLeft.Y,
                Stroke = System.Windows.Media.Brushes.Red,
                StrokeThickness = 2,
                Fill = System.Windows.Media.Brushes.Red,
                Opacity = 0.3

            });
            gridOverlay.Children.Add(canvas);
            System.Windows.Shapes.Rectangle rectangle = gridOverlay.Children.OfType<Canvas>().FirstOrDefault().Children.OfType<System.Windows.Shapes.Rectangle>().FirstOrDefault();//.PointFromScreen(new Point(pixelPanelForZoom.TopLeft.X, pixelPanelForZoom.TopLeft.Y)) ;
            rectangle.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            rectangle.VerticalAlignment = VerticalAlignment.Top;
            Canvas.SetLeft(rectangle, (pixelPanelForZoom.TopLeft.X  /*/ buffer*/) - 10);
            Canvas.SetTop(rectangle, pixelPanelForZoom.TopLeft.Y);

            canvas.MouseMove += MouseMoveCanvals;
            rectangle.MouseDown += RectangleMouseDown;
            rectangle.MouseUp += RectangleMouseUp;

        }
    }
}
