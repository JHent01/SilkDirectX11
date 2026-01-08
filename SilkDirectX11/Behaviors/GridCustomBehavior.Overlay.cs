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
using System.Windows.Interop;
using System.Windows.Media;
using Button = System.Windows.Controls.Button;
using SilkDirectX11;
using Brushes = System.Windows.Media.Brushes;

namespace SilkDirectX11.Behaviors
{
    partial class GridCustomBehavior
    {
        private Window InitOverlayWindow()
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
                Width = 35,
                Height = 35,
                FontSize = 16,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,

            };
            Button buttonZoomMode = new Button
            {
                Content = "O",
                Width = 35,
                Height = 35,
                Style = (Style)System.Windows.Application.Current.FindResource("ButtonStyleOnZoomMode"),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                
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
                Width = buttonZoomMode.Width,
                Height = buttonZoomMode.Height,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(5),
                
            };
            gridWithButtonClouceOverlay.Children.Add(buttonClouseInOverlay);
            gridWithButtonZoomMode.Children.Add(buttonZoomMode);
            GridOverlay.Children.Add(gridWithButtonClouceOverlay);
            GridOverlay.Children.Add(gridWithButtonZoomMode);

            border.Child = GridOverlay;

            windOverlay.Content = border;
             
            windOverlay.Visibility = Visibility.Collapsed;

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
            Grid grid = grids.Children.OfType<Grid>().Where(s => s.Name == "GridWithButtonClouseOverlay").FirstOrDefault();
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

        private void OnChengeZoomMode(object sender, RoutedEventArgs e)
        {
            Button zommButton = sender as Button;
            var gridOverlay = zommButton.Parent as Grid;
            gridOverlay.SizeChanged -= OnSizeChengeOverlayWindow;
            var zoom = AssociatedObject.Children.OfType<CustomGrid>().Where(c => c.Name == zommButton.Name).FirstOrDefault();
            if (zoom == null)
                ZoomModeOn(zommButton);
            else
                ZoomModeOff(zoom, zommButton);



        }
        private void ZoomModeOn(Button button)
        {
            
            button.Style = (Style)System.Windows.Application.Current.FindResource("ButtonStyleOffZoomMode");
            var g = button.Parent as Grid;
            g.Opacity = 1;
            var hostGrid = AssociatedObject as Grid;
            var surfaceGrid = hostGrid.Children.OfType<CustomGrid>().Where(c => c.CameraGuidName == button.Name).FirstOrDefault();
            if (surfaceGrid == null) return;
            Window surfaceWindow = surfaceGrid.Window;
            surfaceGrid.AllowDrop = false;
            surfaceWindow.MouseRightButtonDown -= OnChangeFullScreen;
            surfaceWindow.Drop -= AssociatedObject_Drop;
            surfaceWindow.MouseLeave -= OnMouseLeaveHideOverlay;
            surfaceWindow.MouseMove -= CameraWindowMouseMove;
            surfaceWindow.MouseLeave -= ChengeBorderColor;
            surfaceWindow.MouseMove -= OnMouseMuveWindowShow;
            surfaceWindow.MouseDown -= StartDragDrop;
            surfaceWindow.MouseUp += OnMouseUpTakePosition;
            surfaceWindow.MouseDown += OnMouseDownTakePosition;
            surfaceWindow.Tag = surfaceGrid.ProcessTag;
            surfaceGrid.Children.OfType<Border>().FirstOrDefault().BorderBrush = System.Windows.Media.Brushes.Red;
           
            SetConnect setConnect = new SetConnect(false, int.Parse(surfaceGrid.ProcessTag));
            ConnectedManager.SendChandeConekting(setConnect);

            Grid gridButton = button.Parent as Grid;
            gridButton.Visibility = Visibility.Visible;
            gridButton.MouseMove -= OnMouseEnterWindowShowOverlay;
            gridButton.MouseLeave -= OnMouseLeaveOverLay;

        }

        private void ZoomModeOff(CustomGrid zoomgrid, Button button)
        {
            button.Style = (Style)System.Windows.Application.Current.FindResource("ButtonStyleOnZoomMode");
            var g = button.Parent as Grid;
            g.Opacity = 0.5;
            var hostGrid = AssociatedObject as Grid;
            
            
             hostGrid.Children.Remove(zoomgrid);
            
            var surfaceGrid = hostGrid.Children.OfType<CustomGrid>().Where(c => c.CameraGuidName == zoomgrid.Name).FirstOrDefault();
            Window surfaceWindow = surfaceGrid.Window;
            surfaceGrid.AllowDrop = true;
            surfaceWindow.MouseRightButtonDown += OnChangeFullScreen;
            surfaceWindow.Drop += AssociatedObject_Drop;
            surfaceWindow.MouseLeave += OnMouseLeaveHideOverlay;
            surfaceWindow.MouseMove += CameraWindowMouseMove;
            surfaceWindow.MouseLeave += ChengeBorderColor;
            surfaceWindow.MouseMove += OnMouseMuveWindowShow;
            surfaceWindow.MouseDown += StartDragDrop;
            surfaceWindow.MouseUp -= OnMouseUpTakePosition;
            surfaceWindow.MouseDown -= OnMouseDownTakePosition;
            surfaceWindow.Tag = surfaceGrid.ProcessTag;
            surfaceGrid.Children.OfType<Border>().FirstOrDefault().BorderBrush = System.Windows.Media.Brushes.White;
           
            Grid gridOverlayCanvals = ((Border)surfaceGrid.Window.OwnedWindows[0].Content).Child as Grid;
            if (surfaceGrid.Window.OwnedWindows.Count > 0)
            {
                OpenZoom openZoom = new OpenZoom(false, 0, 1, 1, int.Parse(surfaceGrid.ProcessTag));
                ConnectedManager.SendWindowForZoom(openZoom);
                zoomgrid.Window.Close();
                RemuveTopLeft();
                RemuveBottomRite();

            }
            SetConnect setConnect = new SetConnect(true, int.Parse(surfaceGrid.ProcessTag)); 
            ConnectedManager.SendChandeConekting(setConnect);

            var gridWithButtonZoom = gridOverlayCanvals.Children.OfType<Canvas>().FirstOrDefault().Children.OfType<Grid>().Where(s => s.Name == "GridWithButtonZoomMode").FirstOrDefault();
            gridOverlayCanvals.Children.OfType<Canvas>().FirstOrDefault().Children.Clear();
            var childOverlay = gridOverlayCanvals.Children.OfType<Canvas>().FirstOrDefault();
            gridOverlayCanvals.Children.Remove(childOverlay);
            gridOverlayCanvals.Children.Add(gridWithButtonZoom);


            Grid gridButton = button.Parent as Grid;
            gridButton.Visibility = Visibility.Visible;
            gridButton.MouseMove += OnMouseEnterWindowShowOverlay;
            gridButton.MouseLeave += OnMouseLeaveOverLay;
        }

        private void CreateCanvalInOverlay(object? sender)
        {
            var hostGrid = AssociatedObject as Grid;
            Window surfaceWindow = sender as Window;
            Grid gridOverlay = ((Border)surfaceWindow.OwnedWindows[0].Content).Child as Grid;
            var child = gridOverlay.Children.OfType<Canvas>().FirstOrDefault();
            var buttonGrid = gridOverlay.Children.OfType<Grid>().Where(s => s.Name == "GridWithButtonZoomMode").FirstOrDefault();
            gridOverlay.Children.Remove(buttonGrid);
            if (child != null)
            {
                buttonGrid = child.Children.OfType<Grid>().Where(s => s.Name == "GridWithButtonZoomMode").FirstOrDefault();
                child.Children.Remove(buttonGrid);
                gridOverlay.Children.Remove(child);
            }
            Canvas canvas = new Canvas()
            {
                Width = surfaceWindow.OwnedWindows[0].Width,
                Height = surfaceWindow.OwnedWindows[0].Height,
                Background = System.Windows.Media.Brushes.Transparent,
                Tag = (sender as Window).Tag,
                Focusable = false,

            };
            canvas.Children.Add(new System.Windows.Shapes.Rectangle
            {
                Width = (_pixelPanelForZoom.BottomRight.X - _pixelPanelForZoom.TopLeft.X),
                Height = _pixelPanelForZoom.BottomRight.Y - _pixelPanelForZoom.TopLeft.Y,
                Stroke = System.Windows.Media.Brushes.Red,
                StrokeThickness = 2,
                Fill = System.Windows.Media.Brushes.Red,
                Opacity = 0.3,
                Cursor = System.Windows.Input.Cursors.SizeAll

            });
            if (buttonGrid!=null)
            canvas.Children.Add(buttonGrid);
            gridOverlay.Children.Add(canvas);

             
            gridOverlay.Tag = surfaceWindow;
            System.Windows.Shapes.Rectangle rectangle = gridOverlay.Children.OfType<Canvas>().FirstOrDefault().Children.OfType<System.Windows.Shapes.Rectangle>().FirstOrDefault();
            rectangle.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            rectangle.VerticalAlignment = VerticalAlignment.Top;
            Canvas.SetLeft(rectangle, (_pixelPanelForZoom.TopLeft.X));
            Canvas.SetTop(rectangle, _pixelPanelForZoom.TopLeft.Y);

            canvas.MouseMove += OnMoveCanvals;
            rectangle.MouseDown += OnRectangleMouseDown;
            surfaceWindow.OwnedWindows[0].Focus();
            gridOverlay.SizeChanged -= OnSizeChengeOverlayWindow;
            gridOverlay.SizeChanged += OnSizeChengeOverlayWindow;

            gridOverlay.UpdateLayout();
        }

        private void OnSizeChengeOverlayWindow(object? sender, SizeChangedEventArgs e)
        {
            Grid gridOverlay = sender as Grid;
            Canvas canvas = gridOverlay.Children.OfType<Canvas>().FirstOrDefault();
            Window wind =gridOverlay.Tag as Window;
            if (canvas !=null)
            {
                 

                double scaleX = gridOverlay.ActualWidth /canvas.ActualWidth  ;
                double scaleY = gridOverlay.ActualHeight /canvas.ActualHeight  ;

                canvas.Width = gridOverlay.ActualWidth;
                canvas.Height = gridOverlay.ActualHeight;
                WindowInteropHelper helper = new WindowInteropHelper(wind);
                OpenZoom openZoom = new OpenZoom(true, int.Parse(helper.Handle.ToString()), (int)gridOverlay.ActualWidth, (int)gridOverlay.ActualHeight, int.Parse(canvas.Tag as string));
                ConnectedManager.SendWindowForZoom(openZoom);
                double rectangeWidth = canvas.Children.OfType<System.Windows.Shapes.Rectangle>().FirstOrDefault().Width;
                double rectangeHeidth = canvas.Children.OfType<System.Windows.Shapes.Rectangle>().FirstOrDefault().Height;
                
                rectangeHeidth = rectangeHeidth*scaleY < 0 ? rectangeHeidth : rectangeHeidth * scaleY;
                rectangeWidth = rectangeWidth*scaleX < 0 ? rectangeHeidth : rectangeWidth * scaleX;
                canvas.Children.OfType<System.Windows.Shapes.Rectangle>().FirstOrDefault().Width =  rectangeWidth ;
                canvas.Children.OfType<System.Windows.Shapes.Rectangle>().FirstOrDefault().Height = rectangeHeidth  ;

                 double leftREctangle = Canvas.GetLeft(canvas.Children.OfType<System.Windows.Shapes.Rectangle>().FirstOrDefault());
                double topRectangle = Canvas.GetTop(canvas.Children.OfType<System.Windows.Shapes.Rectangle>().FirstOrDefault());
                 
                leftREctangle = leftREctangle * scaleX < 0 ? 0 : leftREctangle * scaleX;
                topRectangle = topRectangle * scaleY< 0 ? 0 : topRectangle * scaleY;

                Canvas.SetLeft(canvas.Children.OfType<System.Windows.Shapes.Rectangle>().FirstOrDefault(), leftREctangle);
                Canvas.SetTop(canvas.Children.OfType<System.Windows.Shapes.Rectangle>().FirstOrDefault(), topRectangle);

                 
                PointsForZoom pointsForZoom = new PointsForZoom(leftREctangle, topRectangle, leftREctangle+ rectangeWidth, topRectangle+ rectangeHeidth);
                ConnectedManager.Rectangle_MouseMove_SendPoint(canvas.Tag.ToString(), pointsForZoom);


            }
        }
    }
}
