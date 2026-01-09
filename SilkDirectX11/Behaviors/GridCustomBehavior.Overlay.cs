using LibraryForSignalR;
using SilkDirectX11;
using SilkDirectX11.Model;
using SilkDirectX11.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using Button = System.Windows.Controls.Button;
using Point = System.Windows.Point;

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
            //windOverlay.Topmost = true;

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
        private void OnMoveCanvals(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {

                Canvas canvas = sender as Canvas;
                System.Windows.Shapes.Rectangle rectangle = canvas.Children.OfType<System.Windows.Shapes.Rectangle>().FirstOrDefault();
                Grid parent = VisualTreeHelper.GetParent(canvas) as Grid;

                Point currentPoint = e.GetPosition(canvas);
                double deltaX = currentPoint.X - _startPoint.X;
                double deltaY = currentPoint.Y - _startPoint.Y;

                double newLeft = Canvas.GetLeft(rectangle) + deltaX;
                double newTop = Canvas.GetTop(rectangle) + deltaY;
                double newRight = newLeft + rectangle.Width;
                double newBottom = newTop + rectangle.Height;

                if (newLeft < 0)
                    newLeft = 0;
                if (newTop < 0)
                    newTop = 0;
                if (newRight > canvas.ActualWidth)
                    newLeft = canvas.ActualWidth - rectangle.Width;
                if (newBottom > canvas.ActualHeight)
                    newTop = canvas.ActualHeight - rectangle.Height;

                Canvas.SetLeft(rectangle, newLeft);
                Canvas.SetTop(rectangle, newTop);
                _startPoint = currentPoint;
                PointsForZoom pointsForZoom = new PointsForZoom(newLeft, newTop, newRight, newBottom);
                ConnectedManager.Rectangle_MouseMove_SendPoint(canvas.Tag.ToString(), pointsForZoom);

            }
        }
         
        private void CreateCanvalInOverlay(object? sender)
        {
            var hostGrid = AssociatedObject as Grid;
            Window surfaceWindow = sender as Window;
            Grid gridOverlay = ((Border)surfaceWindow.OwnedWindows[0].Content).Child as Grid;
            var child = gridOverlay.Children.OfType<Canvas>().FirstOrDefault();
            var zoomButtonGrid = gridOverlay.Children.OfType<Grid>().Where(s => s.Name == "GridWithButtonZoomMode").FirstOrDefault();
            var clouceButtonGrid = gridOverlay.Children.OfType<Grid>().Where(s => s.Name == "GridWithButtonClouseOverlay").FirstOrDefault();
            gridOverlay.Children.Remove(zoomButtonGrid);
            gridOverlay.Children.Remove(clouceButtonGrid);
            if (child != null)
            {
                zoomButtonGrid = child.Children.OfType<Grid>().Where(s => s.Name == "GridWithButtonZoomMode").FirstOrDefault();
                clouceButtonGrid = child.Children.OfType<Grid>().Where(s => s.Name == "GridWithButtonClouseOverlay").FirstOrDefault();
                child.Children.Remove(zoomButtonGrid);
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
            if (zoomButtonGrid != null)
            {
                canvas.Children.Add(zoomButtonGrid);
                zoomButtonGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            }
            if (clouceButtonGrid != null)
            {
                canvas.Children.Add(clouceButtonGrid);
                Canvas.SetLeft(clouceButtonGrid,canvas.Width-clouceButtonGrid.Width - 5 );
            }
            gridOverlay.Children.Add(canvas);
            if (surfaceWindow.OwnedWindows.Count > 2)
                surfaceWindow.OwnedWindows[2].Close();


            gridOverlay.Tag = surfaceWindow.OwnedWindows[1];
            System.Windows.Shapes.Rectangle rectangle = gridOverlay.Children.OfType<Canvas>().FirstOrDefault().Children.OfType<System.Windows.Shapes.Rectangle>().FirstOrDefault();
            rectangle.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            rectangle.VerticalAlignment = VerticalAlignment.Top;
            Canvas.SetLeft(rectangle, (_pixelPanelForZoom.TopLeft.X));
            Canvas.SetTop(rectangle, _pixelPanelForZoom.TopLeft.Y);

            canvas.MouseMove += OnMoveCanvals;
            rectangle.MouseDown += OnRectangleMouseDown;
            
            gridOverlay.SizeChanged -= OnSizeChengeOverlayWindow;
            gridOverlay.SizeChanged += OnSizeChengeOverlayWindow;

            WindowInteropHelper helper = new WindowInteropHelper(surfaceWindow.OwnedWindows[1]);

            OpenZoom openZoom = new OpenZoom(true, int.Parse(helper.Handle.ToString()), (int)gridOverlay.ActualWidth, (int)gridOverlay.ActualHeight, int.Parse(canvas.Tag as string));
            ConnectedManager.SendWindowForZoom(openZoom);


            PointsForZoom pointsForZoom = new PointsForZoom(_pixelPanelForZoom.TopLeft.X, _pixelPanelForZoom.TopLeft.Y, _pixelPanelForZoom.TopLeft.X + rectangle.Width, _pixelPanelForZoom.TopLeft.Y + rectangle.Height);
            ConnectedManager.Rectangle_MouseMove_SendPoint(canvas.Tag.ToString(), pointsForZoom);
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
                var gridWithButtonClouseOverlay = canvas.Children.OfType<Grid>().Where(s => s.Name == "GridWithButtonClouseOverlay").FirstOrDefault();
                Canvas.SetLeft(gridWithButtonClouseOverlay, canvas.Width - gridWithButtonClouseOverlay.Width - 15);
                WindowInteropHelper helper = new WindowInteropHelper(wind);
                OpenZoom openZoom = new OpenZoom(true, int.Parse(helper.Handle.ToString()), (int)gridOverlay.ActualWidth, (int)gridOverlay.ActualHeight, int.Parse(canvas.Tag as string));
                ConnectedManager.SendWindowForZoom(openZoom);
                double rectangeWidth = canvas.Children.OfType<System.Windows.Shapes.Rectangle>().FirstOrDefault().Width;
                double rectangeHeidth = canvas.Children.OfType<System.Windows.Shapes.Rectangle>().FirstOrDefault().Height;
                int hh = int.Parse(helper.Handle.ToString());
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
