using LibraryForSignalR;
using MahApps.Metro.Controls;
using SilkDirectX11.Model;
using SilkDirectX11.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
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


                _pixelPanelForZoom.TopLeft = new Point(0, 0);
                //_windowOverlay.Focus();

                // _windowOverlay.Topmost = true;
                //_windowOverlay.Focusable = true;
                //_windowOverlay.Focus();
            }
        }

        private bool CreateWindowForZoom(object? sender)
        {
            var riteGrid = AssociatedObject as Grid;
           
            var MainGrid = riteGrid.Parent as Grid;
            CustomGrid fullScreen = MainGrid.Children.OfType<CustomGrid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();
            if (fullScreen != null)
            {
                fullScreen.ColumnDefinitions.Clear();

                Window zoomWind = new()
                {
                    WindowStyle = WindowStyle.None,
                    ResizeMode = ResizeMode.NoResize,
                };

                CustomGrid zoomGrid = new()
                {
                    Name = "ZoomGrid",
                    CameraGuidName = fullScreen.CameraGuidName,
                    ProcessTag = fullScreen.ProcessTag,
                    WindowTag = fullScreen.WindowTag,
                    CameraConnectStrings = fullScreen.CameraConnectStrings,
                    Window = zoomWind
                };
                fullScreen.ColumnDefinitions.Add(new ColumnDefinition());

                Grid.SetColumn(zoomGrid, 1);


                zoomWind.Width = (riteGrid.ActualWidth / 2 - 5);
                zoomWind.Height = (riteGrid.ActualHeight - 5);
                Window wind = sender as Window;
                if (wind.OwnedWindows.Count > 1)
                    wind.OwnedWindows[1].Close();
                else
                {
                    _pixelPanelForZoom.TopLeft.X = _pixelPanelForZoom.TopLeft.X / 2;
                    _pixelPanelForZoom.BottomRight.X = _pixelPanelForZoom.BottomRight.X / 2;
                }
                wind.Width = (riteGrid.ActualWidth / 2 - 5);
                wind.Height = riteGrid.ActualHeight - 10;

                zoomWind.Owner = wind;
                zoomWind.Width = wind.Width;
                zoomWind.Height = wind.Height;
                zoomWind.Left = wind.Left + wind.Width;
                zoomWind.Top = wind.Top;
                zoomWind.Show();

                WindowInteropHelper helper = new WindowInteropHelper(zoomWind);
                int windHandel = int.Parse(helper.Handle.ToString());
                PointsForZoom pointsForZoom = new PointsForZoom(_pixelPanelForZoom.TopLeft.X, _pixelPanelForZoom.TopLeft.Y, _pixelPanelForZoom.BottomRight.X, _pixelPanelForZoom.BottomRight.Y);
                ConnectedManager.Rectangle_MouseMove_SendPoint(wind.Tag as string, pointsForZoom);

                OpenZoom openZoom = new OpenZoom(true, windHandel, (int)zoomWind.Width, (int)zoomWind.Height, int.Parse(wind.Tag as string));
                ConnectedManager.SendWindowForZoom(openZoom);


                zoomWind.Tag = wind.Tag as string;
                return true;
            }
            else
            {// привязать к размероам окна оверлей 
               
                Window wind = sender as Window;
                CustomGrid surfaceGrid = riteGrid.Children.OfType<CustomGrid>().Where(c => c.CameraGuidName == wind.Name).FirstOrDefault();
                if (surfaceGrid == null) return false;
                
                Window zoomWind = new()
                {
                    WindowStyle = WindowStyle.None,
                    ResizeMode = ResizeMode.NoResize,
                    Owner = wind,
                    Width = wind.Width,
                    Height = wind.Height,
                    Tag = surfaceGrid.ProcessTag

                };
               
               
                CustomGrid grid = new CustomGrid() 
                {
                    Name = surfaceGrid.CameraGuidName,
                    CameraGuidName = surfaceGrid.CameraGuidName,
                    ProcessTag = surfaceGrid.ProcessTag,
                   
                    CameraConnectStrings = surfaceGrid.CameraConnectStrings,
                    Window = zoomWind,
                };
                grid.Children.Add(new Border()
                {
                    Background = System.Windows.Media.Brushes.Transparent,
                    BorderBrush = System.Windows.Media.Brushes.Red,
                    BorderThickness = new Thickness(5),
                    CornerRadius = new CornerRadius(5),
                    Margin = new Thickness(1, 1, 1, 1),
                    Padding = new Thickness(1, 1, 1, 1),


                });
                zoomWind.Show();
                
                WindowInteropHelper helper = new WindowInteropHelper(zoomWind);
                grid.WindowTag = helper.Handle.ToString();
                int windHandel = int.Parse(helper.Handle.ToString());
                CustomGrid zoomGrid = riteGrid.Children.OfType<CustomGrid>().Where(c => c.Name == wind.Name).FirstOrDefault();
                if (zoomGrid != null)
                {
                    riteGrid.Children.Add(grid);
                    zoomGrid.Window.Close();
                    var rowSet = Grid.GetRow(zoomGrid);
                    var columSet = Grid.GetColumn(zoomGrid);
                    Grid.SetRow(grid, rowSet);
                    Grid.SetColumn(grid, columSet);
                    riteGrid.Children.Remove(zoomGrid);


                }
                else
                
                if (riteGrid.ColumnDefinitions.Count <= 1)
                {
                    Grid.SetColumn(grid, riteGrid.ColumnDefinitions.Count);

                    riteGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    riteGrid.Children.Add(grid);


                }
                else if (riteGrid.RowDefinitions.Count == 0)
                {
                    riteGrid.RowDefinitions.Add(new RowDefinition());
                    Grid.SetRow(grid, riteGrid.RowDefinitions.Count);

                    riteGrid.RowDefinitions.Add(new RowDefinition());
                    riteGrid.Children.Add(grid);

                }
                else
                {
                    if (CheckEmptyChildInGrid(riteGrid))
                    {
                        AddGrid(riteGrid, grid);

                    }
                    else
                    {
                        riteGrid.RowDefinitions.Add(new RowDefinition());
                        if (riteGrid.RowDefinitions.Count > 2)
                            riteGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        Grid.SetRow(grid, riteGrid.RowDefinitions.Count - 1);
                        riteGrid.Children.Add(grid);
                        UpdateWindowPositionForGrid(grid);

                    }
                }
               


                PointsForZoom pointsForZoom = new PointsForZoom(_pixelPanelForZoom.TopLeft.X, _pixelPanelForZoom.TopLeft.Y, _pixelPanelForZoom.BottomRight.X, _pixelPanelForZoom.BottomRight.Y);
                ConnectedManager.Rectangle_MouseMove_SendPoint(wind.Tag as string, pointsForZoom);

                OpenZoom openZoom = new OpenZoom(true, windHandel, (int)zoomWind.Width, (int)zoomWind.Height, int.Parse(wind.Tag as string));
                ConnectedManager.SendWindowForZoom(openZoom);
            }

                return true;


        }



        private void OnRectangleMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //flagChengePosition = true;
            Canvas canvas = VisualTreeHelper.GetParent(sender as System.Windows.Shapes.Rectangle) as Canvas;
            _startPoint = e.GetPosition(canvas);

        }
        private void OnRectangleMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           // flagChengePosition = false;
        }
        private void OnMoveCanvals(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton==MouseButtonState.Pressed)
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
