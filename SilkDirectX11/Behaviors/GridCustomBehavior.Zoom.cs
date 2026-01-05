using LibraryForSignalR;
using MahApps.Metro.Controls;
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


//using System.Windows.Forms.Integration;
using System.Windows.Media;
using Point = System.Windows.Point;

namespace SilkDirectX11.Behaviors
{
    partial class GridCustomBehavior
    {
        private Point _startPoint;
        bool flagChengePosition = false;
        private void OnMouseUpTakePosition(object? sender, MouseButtonEventArgs e)
        {
            _pixelPanelForZoom.BottomRight = new Point(e.GetPosition(sender as Window).X, e.GetPosition(sender as Window).Y);//??

            if (_pixelPanelForZoom.TopLeft != _pixelPanelForZoom.BottomRight & _pixelPanelForZoom.TopLeft != null & _pixelPanelForZoom.TopLeft.X != 0)
            {
                if (_pixelPanelForZoom.BottomRight.X < _pixelPanelForZoom.TopLeft.X || _pixelPanelForZoom.BottomRight.Y < _pixelPanelForZoom.TopLeft.Y)
                {
                    if (_pixelPanelForZoom.BottomRight.X < _pixelPanelForZoom.TopLeft.X)
                    {
                        var bufferX = _pixelPanelForZoom.TopLeft.X;
                        _pixelPanelForZoom.TopLeft.X = _pixelPanelForZoom.BottomRight.X;
                        _pixelPanelForZoom.BottomRight.X = bufferX;
                    }
                    if (_pixelPanelForZoom.BottomRight.Y < _pixelPanelForZoom.TopLeft.Y)
                    {
                        var bufferY = _pixelPanelForZoom.TopLeft.Y;
                        _pixelPanelForZoom.TopLeft.Y = _pixelPanelForZoom.BottomRight.Y;
                        _pixelPanelForZoom.BottomRight.Y = bufferY;

                    }
                }
                if (CreateWindowForZoom(sender))

                    CreateCanvalInOverlay(  sender);
                _windowOverlay.Focus();
                 
               // _windowOverlay.Topmost = true;
                //_windowOverlay.Focusable = true;
                //_windowOverlay.Focus();
            }
        }

        private bool CreateWindowForZoom(object? sender)
        {
            var riteGrid = AssociatedObject as Grid;
          
            Window zoomWind = new()
            {
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
            };
            zoomWind.Width = (riteGrid.ActualWidth / 2 -5);
            zoomWind.Height = (riteGrid.ActualHeight -5);
            Window wind = sender as Window;
            if (wind.OwnedWindows.Count > 0  )
                wind.OwnedWindows[0].Close();
            else
            {
                _pixelPanelForZoom.TopLeft.X = _pixelPanelForZoom.TopLeft.X / 2;
                _pixelPanelForZoom.BottomRight.X = _pixelPanelForZoom.BottomRight.X / 2;
            }
            wind.Width = (riteGrid.ActualWidth / 2 -5);
            wind.Height = riteGrid.ActualHeight -10;
            
            zoomWind.Owner = wind;
            zoomWind.Width = wind.Width;
            zoomWind.Height = wind.Height;
            zoomWind.Left = wind.Left+wind.Width;
            zoomWind.Top = wind.Top;
            zoomWind.Show();
            //zoomWind.Focus();

            _windowOverlay.Width = riteGrid.ActualWidth / 2;
            _windowOverlay.Height = riteGrid.ActualHeight;
            WindowInteropHelper helper = new WindowInteropHelper(zoomWind);
            int windHandel = int.Parse(helper.Handle.ToString());
            PointsForZoom pointsForZoom = new PointsForZoom(_pixelPanelForZoom.TopLeft.X, _pixelPanelForZoom.TopLeft.Y, _pixelPanelForZoom.BottomRight.X, _pixelPanelForZoom.BottomRight.Y);
            ConnectedManager.Rectangle_MouseMove_SendPoint(wind.Tag as string, pointsForZoom);
            //Rectangle_MouseMove_SendPoint(wind.Tag as string , pointsForZoom);
            OpenZoom openZoom = new OpenZoom(true, windHandel, (int)zoomWind.Width, (int)zoomWind.Height, int.Parse(wind.Tag as string));
            ConnectedManager.SendWindowForZoom(openZoom);
            //SendWindowForZoom(openZoom);

            zoomWind.Tag = wind.Tag as string;
            return true;

        }



        private void OnRectangleMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            flagChengePosition = true;
            Canvas canvas = VisualTreeHelper.GetParent(sender as System.Windows.Shapes.Rectangle) as Canvas;
            _startPoint = e.GetPosition(canvas);

        }
        private void OnRectangleMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            flagChengePosition = false;
        }
        private void OnMoveCanvals(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (flagChengePosition)
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
                //Rectangle_MouseMove_SendPoint(canvas.Tag.ToString(), pointsForZoom);
            }
        }

        private void OnMouseDownTakePosition(object? sender, MouseButtonEventArgs e)
        {
            if (!flagChengePosition)
                _pixelPanelForZoom.TopLeft = new Point(e.GetPosition(sender as Window).X, e.GetPosition(sender as Window).Y);
            else _pixelPanelForZoom.TopLeft = new Point(0, 0);
        }
    }
}
