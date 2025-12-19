using LibraryForSignalR;
using SilkDirectX11.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using Point = System.Windows.Point;

namespace SilkDirectX11.Behaviors
{
    partial class GridCustomBehavior
    {
        private Point _startPoint;
        bool flagChengePosition = false;
        private void MouseUpTakePixel(object? sender, System.Windows.Forms.MouseEventArgs e)
        {


            pixelPanelForZoom.BottomRight = new Point(e.X, e.Y);

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
                if (CreateWFHForZoom(sender))


                    CreateCanvalInOverlay();



            }
        }

        private bool CreateWFHForZoom(object? sender)
        {
            var riteGrid = AssociatedObject as Grid;
            //var full = riteGrid.Parent as Grid;



            Grid gridFullScreen = riteGrid.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();


            WindowsFormsHost wfh = gridFullScreen.Children.OfType<WindowsFormsHost>().FirstOrDefault();
            wfh.Width = (riteGrid.ActualWidth / 2);
            wfh.Child.Width = (int)(riteGrid.ActualWidth / 2);
            wfh.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            wfh.Width = (riteGrid.ActualWidth / 2);
            BetterPanelTest panel = sender as BetterPanelTest;
            if (gridFullScreen.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault() != null/*gridFullScreen.Children.Count > 1*/)//0000000000000000000000000000000000000000000000000000000
            {
                WindowsFormsHost zoom = gridFullScreen.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault();
                //  var t = zoom.Tag as string;
                gridFullScreen.Children.Remove(zoom);

                //try
                //{
                //   // Process.GetProcessById(int.Parse(t)).Kill();
                //}
                //catch (Exception ex)
                //{
                //    string exs = ex.Message;

                //}
            }
            else
            {
                pixelPanelForZoom.TopLeft.X = pixelPanelForZoom.TopLeft.X / 2;
                pixelPanelForZoom.BottomRight.X = pixelPanelForZoom.BottomRight.X / 2;
            }




            gridFullScreen.Children.Add(new WindowsFormsHost
            {
                Width = riteGrid.ActualWidth / 2,
                Height = riteGrid.ActualHeight,
                Child = new BetterPanelTest
                {
                    DataContext = panel.DataContext,
                    Width = (int)(riteGrid.ActualWidth / 2),
                    Height = (int)(riteGrid.ActualHeight),

                    Tag = panel.Tag
                },
                DataContext = panel.DataContext,
                Name = "ZoomHost",
                Margin = new Thickness(25, 5, 0, 0),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right
            });
            window.Width = riteGrid.ActualWidth / 2;
            window.Height = riteGrid.ActualHeight;
            gridFullScreen.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault().Child.Width = (int)window.Width;
            var ZoomHost = gridFullScreen.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault();

            string proc = gridFullScreen.Tag as string;
            int wind = int.Parse(ZoomHost.Child.Handle.ToString());
            int w = ZoomHost.Child.Width;
            int h = ZoomHost.Child.Height;
            PointsForZoom pointsForZoom = new PointsForZoom(pixelPanelForZoom.TopLeft.X, pixelPanelForZoom.TopLeft.Y, pixelPanelForZoom.BottomRight.X, pixelPanelForZoom.BottomRight.Y);
            //{
            //    xTL = pixelPanelForZoom.TopLeft.X,
            //    yTL = pixelPanelForZoom.TopLeft.Y,
            //    xBR = pixelPanelForZoom.BottomRight.X,
            //    yBR = pixelPanelForZoom.BottomRight.Y
            //};
            // proc.ToString() ,
            Rectangle_MouseMove_SendPoint(proc.ToString(), pointsForZoom);//(pixelPanelForZoom.TopLeft.X, pixelPanelForZoom.TopLeft.Y, pixelPanelForZoom.BottomRight.X, pixelPanelForZoom.BottomRight.Y);
            OpenZoom openZoom = new OpenZoom(true, wind, w, h, int.Parse(proc));

            SendWindowForZoom(openZoom);

            ZoomHost.Tag = proc;//process.Id.ToString();
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
                double newRight = newLeft + rectangle.Width;//Canvas.GetRight(rectangle) + deltaX;
                double newBottom = newTop + rectangle.Height; //Canvas.GetBottom(rectangle) + deltaY;
                                                              //double left = Canvas.GetLeft(rectangle);
                                                              //double top = Canvas.GetTop(rectangle);
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

        private void MouseDownTakePxel(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!flagChengePosition)
                pixelPanelForZoom.TopLeft = new Point(e.X, e.Y);
            else pixelPanelForZoom.TopLeft = new Point(0, 0);
        }
    }
}
