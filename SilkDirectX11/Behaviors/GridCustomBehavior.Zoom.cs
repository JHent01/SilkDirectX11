using LibraryForSignalR;
using MahApps.Metro.Controls;
using SilkDirectX11.Model;
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
        private void MouseUpTakePixel(object? sender, MouseButtonEventArgs e)
        {
            pixelPanelForZoom.BottomRight = new Point(e.GetPosition(sender as Window).X, e.GetPosition(sender as Window).Y);//??

            if (pixelPanelForZoom.TopLeft != pixelPanelForZoom.BottomRight & pixelPanelForZoom.TopLeft != null & pixelPanelForZoom.TopLeft.X != 0)
            {
                if (pixelPanelForZoom.BottomRight.X < pixelPanelForZoom.TopLeft.X || pixelPanelForZoom.BottomRight.Y < pixelPanelForZoom.TopLeft.Y)
                {
                    if (pixelPanelForZoom.BottomRight.X < pixelPanelForZoom.TopLeft.X)
                    {
                        var bufferX = pixelPanelForZoom.TopLeft.X;
                        pixelPanelForZoom.TopLeft.X = pixelPanelForZoom.BottomRight.X;
                        pixelPanelForZoom.BottomRight.X = bufferX;
                    }
                    if (pixelPanelForZoom.BottomRight.Y < pixelPanelForZoom.TopLeft.Y)
                    {
                        var bufferY = pixelPanelForZoom.TopLeft.Y;
                        pixelPanelForZoom.TopLeft.Y = pixelPanelForZoom.BottomRight.Y;
                        pixelPanelForZoom.BottomRight.Y = bufferY;

                    }
                }
                if (CreateWindowForZoom(sender))

                    CreateCanvalInOverlay(  sender);

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
            zoomWind.Width = (riteGrid.ActualWidth / 2);
            zoomWind.Height = (riteGrid.ActualHeight);
            Window wind = sender as Window;
            if (wind.OwnedWindows.Count > 0  )
                wind.OwnedWindows[0].Close();
            else
            {
                pixelPanelForZoom.TopLeft.X = pixelPanelForZoom.TopLeft.X / 2;
                pixelPanelForZoom.BottomRight.X = pixelPanelForZoom.BottomRight.X / 2;
            }
            wind.Width = (riteGrid.ActualWidth / 2);
            wind.Height = riteGrid.ActualHeight;
            zoomWind.Owner = wind;
            zoomWind.Width = wind.Width;
            zoomWind.Height = wind.Height;
            zoomWind.Left = wind.Left+wind.Width;
            zoomWind.Top = wind.Top;
            zoomWind.Show();
            zoomWind.Focus();

            windowOverlay.Width = riteGrid.ActualWidth / 2;
            windowOverlay.Height = riteGrid.ActualHeight;
            WindowInteropHelper helper = new WindowInteropHelper(zoomWind);
            int windHandel = int.Parse(helper.Handle.ToString());
            PointsForZoom pointsForZoom = new PointsForZoom(pixelPanelForZoom.TopLeft.X, pixelPanelForZoom.TopLeft.Y, pixelPanelForZoom.BottomRight.X, pixelPanelForZoom.BottomRight.Y);
            Rectangle_MouseMove_SendPoint(wind.Tag as string , pointsForZoom);
            OpenZoom openZoom = new OpenZoom(true, windHandel, (int)zoomWind.Width, (int)zoomWind.Height, int.Parse(wind.Tag as string));

            SendWindowForZoom(openZoom);

            zoomWind.Tag = wind.Tag as string;
            return true;

        }



        private void RectangleMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            flagChengePosition = true;
            Canvas canvas = VisualTreeHelper.GetParent(sender as System.Windows.Shapes.Rectangle) as Canvas;
            _startPoint = e.GetPosition(canvas);

        }
        private void RectangleMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            flagChengePosition = false;
        }
        private void MouseMoveCanvals(object sender, System.Windows.Input.MouseEventArgs e)
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

                Rectangle_MouseMove_SendPoint(canvas.Tag.ToString(), pointsForZoom);
            }
        }

        private void MouseDownTakePxel(object? sender, MouseButtonEventArgs e)
        {
            if (!flagChengePosition)
                pixelPanelForZoom.TopLeft = new Point(e.GetPosition(sender as Window).X, e.GetPosition(sender as Window).Y);
            else pixelPanelForZoom.TopLeft = new Point(0, 0);
        }
    }
}
