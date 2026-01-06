using LibraryForSignalR;
using SilkDirectX11.Model;
using SilkDirectX11.SignalR;
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
        private  Window InitOverlayWindow()
        {
            Window windOverlay = new Window();
            Grid Rite = AssociatedObject as Grid;

            windOverlay.Background = System.Windows.Media.Brushes.Transparent;
            windOverlay.WindowStyle = WindowStyle.None;
            windOverlay.AllowsTransparency = true;
            windOverlay.ShowInTaskbar = false;
            windOverlay.Topmost = true;

            Button buttonClouseInOverlay = new Button
            {
                Content = "X",
                Width = 30,
                Height = 30,

                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                
            };
            Button buttonZoomMode = new Button
            {
                Content = "O",
                Width = 30,
                Height = 30,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                //Margin = new Thickness(0,35,0,0)
            };
           buttonZoomMode.Click += OnChengeZoomMode;
            Border border = new Border
            {

                Width = windOverlay.Width,
                Height = windOverlay.Height,


            };
            buttonClouseInOverlay.Click += DeleteGridChild;
            Grid gridWithButtonClouceOverlay = new Grid()
            {
                Name = "GridWithButtonClouseOverlay",
                Visibility = Visibility.Collapsed,
                Background = System.Windows.Media.Brushes.Black,
                Opacity = 0.5,
                Width = buttonClouseInOverlay.Width,
                Height = buttonClouseInOverlay.Height,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(5)
            };
            Grid GridOverlay = new Grid()
            {
                Background = System.Windows.Media.Brushes.Transparent,

                Width = windOverlay.Width,
                Height = windOverlay.Height,
            };
            Grid gridWithButtonZoomMode = new Grid()
            {
                Name = "GridWithButtonZoomMode",
                Visibility = Visibility.Collapsed,
                Background = System.Windows.Media.Brushes.Black,
                Opacity = 0.5,
                Width = buttonClouseInOverlay.Width,
                Height = buttonClouseInOverlay.Height,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(5)
            };
            gridWithButtonClouceOverlay.Children.Add(buttonClouseInOverlay);
            gridWithButtonZoomMode.Children.Add(buttonZoomMode);
            GridOverlay.Children.Add(gridWithButtonClouceOverlay);
            GridOverlay.Children.Add(gridWithButtonZoomMode);

            border.Child = GridOverlay;

            windOverlay.Content = border;

            //windOverlay.Width = Rite.ActualWidth / Rite.ColumnDefinitions.Count;
            //if (Rite.RowDefinitions.Count != 0) windOverlay.Height = Rite.ActualHeight / Rite.RowDefinitions.Count;
            //else windOverlay.Height = Rite.ActualHeight;

            windOverlay.Visibility = Visibility.Visible;

            gridWithButtonZoomMode.MouseMove += OnMouseEnterWindowShowOverlay;
            gridWithButtonZoomMode.MouseLeave += OnMouseLeaveOverLay;
            gridWithButtonClouceOverlay.MouseMove += OnMouseEnterWindowShowOverlay;
            gridWithButtonClouceOverlay.MouseLeave += OnMouseLeaveOverLay;
            return windOverlay;

        }
        private void OnMouseLeaveHideOverlay(object? sender, EventArgs e)
        {
            if (!_flagForOverlay) return;
            Window surfaceWindow = sender as Window;
            Border border = (Border)surfaceWindow.OwnedWindows[0].Content;
            Grid grids = (Grid)border.Child;
            Grid grid = grids.Children.OfType<Grid>().Where(s => s.Name == "GridWithButtonClouseOverlay").FirstOrDefault();
            
            Grid grid2 = grids.Children.OfType<Grid>().Where(s => s.Name == "GridWithButtonZoomMode").FirstOrDefault();

            grid2.Visibility = Visibility.Hidden;
            grid.Visibility = Visibility.Hidden;
        }

        private void OnMouseMuveWindowShow(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var surfaceWindow = sender as Window;
            Border border = (Border)surfaceWindow.OwnedWindows[0].Content;
            Grid grids = (Grid)border.Child;
            Grid grid = grids.Children.OfType<Grid>().Where(s=> s.Name == "GridWithButtonClouseOverlay").FirstOrDefault();
            Grid grid2 = grids.Children.OfType<Grid>().Where(s => s.Name == "GridWithButtonZoomMode").FirstOrDefault();
             
            grid2.Visibility = Visibility.Visible;
            grid.Visibility = Visibility.Visible;
            
        }
        private void OnMouseLeaveOverLay(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _flagForOverlay = true;
            Grid grd = sender as Grid;
            grd.Visibility = Visibility.Hidden;

        }

        private void OnMouseEnterWindowShowOverlay(object s, System.Windows.Input.MouseEventArgs ev)
        {
            _flagForOverlay = false;
            Grid grid = s as Grid;

            grid.Visibility = Visibility.Visible;
        }

        private void OnChengeZoomMode(object sender, RoutedEventArgs e)//дописапть тут логику закрытия 
        { 
           Button zommButton = sender as Button;
                ZoomModeOn(zommButton);

        }
        private void ZoomModeOn(Button button)
        {
            var riteGrid = AssociatedObject as Grid;
            var MainGrid = riteGrid.Parent as Grid;
            var grid = riteGrid.Children.OfType<CustomGrid>().Where(c => c.CameraGuidName == button.Name).FirstOrDefault();
            if (grid == null) return;
            Window surfaceWindow = grid.Window;
            surfaceWindow.MouseRightButtonDown -= OnChangeFullScreen;
            surfaceWindow.Drop -= AssociatedObject_Drop;
            surfaceWindow.MouseLeave -= OnMouseLeaveHideOverlay;
           // surfaceWindow.MouseMove -= OnMouseMuveWindowShow;
            surfaceWindow.MouseDown -= StartDragDrop;
            surfaceWindow.MouseUp += OnMouseUpTakePosition;
            surfaceWindow.MouseDown += OnMouseDownTakePosition;
            surfaceWindow.Tag = grid.ProcessTag;
            button.MouseLeave -= OnMouseLeaveOverLay;


            SetConnect setConnect = new SetConnect(false, int.Parse(grid.ProcessTag));
            ConnectedManager.SendChandeConekting(setConnect);




        }

        private void ZoomModeOff()
        {

        }

        private void CreateCanvalInOverlay(object? sender)
        {
            var riteGrid = AssociatedObject as Grid;
            Window surfaceWindow = sender as Window;
            Grid gridOverlay = ((Border)surfaceWindow.OwnedWindows[0].Content).Child as Grid;
            var child = gridOverlay.Children.OfType<Canvas>().FirstOrDefault();

           
            if (child != null)
            {
               
                gridOverlay.Children.Remove(child);
            }
            Canvas canvas = new Canvas()
            {
                Width = surfaceWindow.OwnedWindows[0].Width,
                Height = surfaceWindow.OwnedWindows[0].Height,
                Background = System.Windows.Media.Brushes.Transparent,
                Tag = (sender as Window).Tag

            };
            canvas.Children.Add(new System.Windows.Shapes.Rectangle
            {
                Width = (_pixelPanelForZoom.BottomRight.X - _pixelPanelForZoom.TopLeft.X),
                Height = _pixelPanelForZoom.BottomRight.Y - _pixelPanelForZoom.TopLeft.Y,
                Stroke = System.Windows.Media.Brushes.Red,
                StrokeThickness = 2,
                Fill = System.Windows.Media.Brushes.Red,
                Opacity = 0.3,
                
               
            });
            gridOverlay.Children.Add(canvas);
           
            System.Windows.Shapes.Rectangle rectangle = gridOverlay.Children.OfType<Canvas>().FirstOrDefault().Children.OfType<System.Windows.Shapes.Rectangle>().FirstOrDefault(); 
            rectangle.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            rectangle.VerticalAlignment = VerticalAlignment.Top;
            Canvas.SetLeft(rectangle, (_pixelPanelForZoom.TopLeft.X ) - 10);
            Canvas.SetTop(rectangle, _pixelPanelForZoom.TopLeft.Y);
            
            canvas.MouseMove += OnMoveCanvals;
            rectangle.MouseDown += OnRectangleMouseDown;
            rectangle.MouseUp += OnRectangleMouseUp;
            surfaceWindow.OwnedWindows[0].Focus();
          


        }
    }
}
