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

            windowOverlay.Background = System.Windows.Media.Brushes.Transparent;
            windowOverlay.WindowStyle = WindowStyle.None;
            windowOverlay.AllowsTransparency = true;
            windowOverlay.ShowInTaskbar = false;
            windowOverlay.Topmost = true;

            Button buttonClouseInOverlay = new Button
            {
                Content = "X",
                Width = 30,
                Height = 30,

                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,

            };
            Border border = new Border
            {

                Width = windowOverlay.Width,
                Height = windowOverlay.Height,


            };
            buttonClouseInOverlay.Click += DeleteChild;
            Grid gridWithButtonOverlay = new Grid()
            {
                Visibility = Visibility.Collapsed,
                Background = System.Windows.Media.Brushes.Black,
                Opacity = 0.5,
                Width = buttonClouseInOverlay.Width,
                Height = buttonClouseInOverlay.Height,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
            };
            Grid GridOverlay = new Grid()
            {
                Background = System.Windows.Media.Brushes.Transparent,

                Width = windowOverlay.Width,
                Height = windowOverlay.Height,
            };
            gridWithButtonOverlay.Children.Add(buttonClouseInOverlay);
            GridOverlay.Children.Add(gridWithButtonOverlay);
            border.Child = GridOverlay;

            windowOverlay.Content = border;

            windowOverlay.Width = Rite.ActualWidth / Rite.ColumnDefinitions.Count;
            if (Rite.RowDefinitions.Count != 0) windowOverlay.Height = Rite.ActualHeight / Rite.RowDefinitions.Count;
            else windowOverlay.Height = Rite.ActualHeight;

            windowOverlay.Visibility = Visibility.Visible;


            gridWithButtonOverlay.MouseMove += OnMouseEnterWindowShowOverlay;

            gridWithButtonOverlay.MouseLeave += OnMouseLeaveOverLay;

        }
        private void OnMouseLeaveHideOverlay(object? sender, EventArgs e)
        {
            if (!flagForOverlay) return;
            Border border = (Border)windowOverlay.Content;
            Grid grids = (Grid)border.Child;
            Grid grid = grids.Children.OfType<Grid>().FirstOrDefault();
            grid.Visibility = Visibility.Hidden;
        }

        private void OnMouseMuveWindowShow(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var panel = sender as Window;
            Border border = (Border)windowOverlay.Content;
            Grid grids = (Grid)border.Child;
            Grid grid = grids.Children.OfType<Grid>().FirstOrDefault();
            Button b = grid.Children.OfType<Button>().FirstOrDefault();
            b.Name = panel.Name;

            grid.Visibility = Visibility.Visible;
            windowOverlay.Height = panel.Height;
            windowOverlay.Width = panel.Width;
            windowOverlay.Left = panel.PointToScreen(new System.Windows.Point()).X;
            windowOverlay.Top = panel.PointToScreen(new System.Windows.Point()).Y;
        }
        private void OnMouseLeaveOverLay(object sender, System.Windows.Input.MouseEventArgs e)
        {
            flagForOverlay = true;
            Grid grd = sender as Grid;
            grd.Visibility = Visibility.Hidden;

        }

        private void OnMouseEnterWindowShowOverlay(object s, System.Windows.Input.MouseEventArgs ev)
        {
            flagForOverlay = false;
            Grid grid = s as Grid;

            grid.Visibility = Visibility.Visible;
        }
        private void CreateCanvalInOverlay(object? sender)
        {
            var riteGrid = AssociatedObject as Grid;
          
            Grid gridOverlay = ((Border)windowOverlay.Content).Child as Grid;
            var child = gridOverlay.Children.OfType<Canvas>().FirstOrDefault();

            int buffer = 2;
            if (child != null)
            {
                buffer = 1;
                gridOverlay.Children.Remove(child);
            }
            Canvas canvas = new Canvas()
            {
                Width = windowOverlay.Width,
                Height = windowOverlay.Height,
                Background = System.Windows.Media.Brushes.Transparent,
                Tag = (sender as Window).Tag

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
            Canvas.SetLeft(rectangle, (pixelPanelForZoom.TopLeft.X ) - 10);
            Canvas.SetTop(rectangle, pixelPanelForZoom.TopLeft.Y);

            canvas.MouseMove += OnMoveCanvals;
            rectangle.MouseDown += OnRectangleMouseDown;
            rectangle.MouseUp += OnRectangleMouseUp;

        }
    }
}
