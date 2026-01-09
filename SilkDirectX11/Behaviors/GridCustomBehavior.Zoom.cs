using DevExpress.Data.Helpers;
using LibraryForSignalR;
using MahApps.Metro.Controls;
using Microsoft.VisualBasic;
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
 
using System.Windows.Media;
using Vortice.Direct2D1.Effects;
using Border = System.Windows.Controls.Border;
using Button = System.Windows.Controls.Button;
using Point = System.Windows.Point;

namespace SilkDirectX11.Behaviors
{
    partial class GridCustomBehavior
    {
        private Point _startPoint;
        //bool flagChengePosition = false;
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

                    OnCreateWindowForZoom(sender);

                  


                _pixelPanelForZoom.TopLeft = new Point(0, 0);
             
            }
        }


        private void OnCreateWindowForZoom(object? sender)
        {
            var hostGrid = AssociatedObject as Grid;

            var MainGrid = hostGrid.Parent as Grid;
            CustomGrid fullScreen = MainGrid.Children.OfType<CustomGrid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();

            if (fullScreen != null)
            {
                CreateSurfaceInFullScreen(sender);
            }
            else
            {
                if (testFlag)
                {
                    CreateSeparateWindowForZoom(sender);
                }
                else
                    CreateSurfaceInGrid(sender);
            }



        }
        private void CreateSeparateWindowForZoom(object? sender)
        {
            var hostGrid = AssociatedObject as Grid;
            Window windSurface = sender as Window;
            CustomGrid surfaceGrid = hostGrid.Children.OfType<CustomGrid>().Where(c => c.CameraGuidName == windSurface.Name).FirstOrDefault();
            if (surfaceGrid == null) return;
            
            if (windSurface.OwnedWindows.Count>1)
            {
                windSurface.OwnedWindows[1].Close();
            }
            Window zoomWindowSurface = new()
            {
                Name = "ZoomSeparateWindowSurface",
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                Owner = windSurface,
                Width = 600,
                Height = 500,
                AllowDrop = false,
                Tag = surfaceGrid.ProcessTag,
                ShowInTaskbar = false,
                Topmost = true,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };
           
            zoomWindowSurface.Show();
            WindowInteropHelper helper = new WindowInteropHelper(zoomWindowSurface);
            zoomWindowSurface.MouseDown += ZoomWindowSurfaceMouseDown;
            int windHandel = int.Parse(helper.Handle.ToString());
            CreateCanvalInOverlay(sender);
        }

        private void ZoomWindowSurfaceMouseDown(object sender, MouseButtonEventArgs e)
        {
          (sender as Window ).DragMove();
        }

        
        private void CreateSurfaceInGrid(object? sender)
        {
            var hostGrid = AssociatedObject as Grid;
            Window windSurface = sender as Window;
            CustomGrid surfaceGrid = hostGrid.Children.OfType<CustomGrid>().Where(c => c.CameraGuidName == windSurface.Name).FirstOrDefault();
            if (surfaceGrid == null) return ;

            Window zoomWindowSurface = new()
            {

                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                Owner = windSurface,
                Width = windSurface.Width,
                Height = windSurface.Height,
                AllowDrop = true,
                Tag = surfaceGrid.ProcessTag,
                ShowInTaskbar = false,
            };


            CustomGrid grid = new CustomGrid()
            {
                Name = surfaceGrid.CameraGuidName,
                CameraGuidName = surfaceGrid.CameraGuidName,
                ProcessTag = surfaceGrid.ProcessTag,
                Background = System.Windows.Media.Brushes.Transparent,
                CameraConnectStrings = surfaceGrid.CameraConnectStrings,
                Window = zoomWindowSurface,
                Margin = new Thickness(5)
            };
            grid.Children.Add(new System.Windows.Controls.Border()
            {
                Background = System.Windows.Media.Brushes.Transparent,
                BorderBrush = System.Windows.Media.Brushes.Red,
                BorderThickness = new Thickness(5),
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(1, 1, 1, 1),
                Padding = new Thickness(1, 1, 1, 1),


            });
            zoomWindowSurface.MouseDown += StartDragDrop;
            zoomWindowSurface.MouseUp += DropCamera;
            zoomWindowSurface.Drop += AssociatedObject_Drop;
            zoomWindowSurface.PreviewDragEnter += MouseMoveDragDrop;
            zoomWindowSurface.Show();

            WindowInteropHelper helper = new WindowInteropHelper(zoomWindowSurface);
            grid.WindowTag = helper.Handle.ToString();
            int windHandel = int.Parse(helper.Handle.ToString());
            CustomGrid zoomGrid = hostGrid.Children.OfType<CustomGrid>().Where(c => c.Name == windSurface.Name).FirstOrDefault();

            CreateCanvalInOverlay(sender);
            if (zoomGrid != null)
            {
                hostGrid.Children.Add(grid);
                //zoomGrid.Window.Close();
                var rowSet = Grid.GetRow(zoomGrid);
                var columSet = Grid.GetColumn(zoomGrid);
                Grid.SetRow(grid, rowSet);
                Grid.SetColumn(grid, columSet);
                hostGrid.Children.Remove(zoomGrid);


            }
            else

            if (hostGrid.ColumnDefinitions.Count <= 1)
            {
                Grid.SetColumn(grid, hostGrid.ColumnDefinitions.Count);

                hostGrid.ColumnDefinitions.Add(new ColumnDefinition());
                hostGrid.Children.Add(grid);


            }
            else if (hostGrid.RowDefinitions.Count == 0)
            {
                hostGrid.RowDefinitions.Add(new RowDefinition());
                Grid.SetRow(grid, hostGrid.RowDefinitions.Count);

                hostGrid.RowDefinitions.Add(new RowDefinition());
                hostGrid.Children.Add(grid);

            }
            else
            {
                if (CheckEmptyChildInGrid(hostGrid))
                {
                    AddGrid(hostGrid, grid);

                }
                else
                {
                    hostGrid.RowDefinitions.Add(new RowDefinition());
                    if (hostGrid.RowDefinitions.Count > 2)
                        hostGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    Grid.SetRow(grid, hostGrid.RowDefinitions.Count - 1);
                    hostGrid.Children.Add(grid);
                    UpdateWindowPositionForGrid(grid);

                }
            }
           
        }
        private void CreateSurfaceInFullScreen(object? sender)
        {
            var hostGrid = AssociatedObject as Grid;
           
            var MainGrid = hostGrid.Parent as Grid;
            CustomGrid fullScreenGrid = MainGrid.Children.OfType<CustomGrid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();
            
                fullScreenGrid.ColumnDefinitions.Clear();

                Window zoomWindowSurface = new()
                {
                    WindowStyle = WindowStyle.None,
                    ResizeMode = ResizeMode.NoResize,
                };

                CustomGrid zoomGridSurface = new()
                {
                    Name = "ZoomGrid",
                    CameraGuidName = fullScreenGrid.CameraGuidName,
                    ProcessTag = fullScreenGrid.ProcessTag,
                    WindowTag = fullScreenGrid.WindowTag,
                    CameraConnectStrings = fullScreenGrid.CameraConnectStrings,
                    Window = zoomWindowSurface
                };
                fullScreenGrid.ColumnDefinitions.Add(new ColumnDefinition());

                Grid.SetColumn(zoomGridSurface, 1);


                zoomWindowSurface.Width = (hostGrid.ActualWidth / 2 - 5);
                zoomWindowSurface.Height = (hostGrid.ActualHeight - 5);
                Window windowSurface = sender as Window;
                if (windowSurface.OwnedWindows.Count > 1)
                    windowSurface.OwnedWindows[1].Close();
                else
                {
                    _pixelPanelForZoom.TopLeft.X = _pixelPanelForZoom.TopLeft.X / 2;
                    _pixelPanelForZoom.BottomRight.X = _pixelPanelForZoom.BottomRight.X / 2;
                }
                windowSurface.Width = (hostGrid.ActualWidth / 2 - 5);
                windowSurface.Height = hostGrid.ActualHeight - 10;

                zoomWindowSurface.Owner = windowSurface;
                zoomWindowSurface.Width = windowSurface.Width;
                zoomWindowSurface.Height = windowSurface.Height;
                zoomWindowSurface.Left = windowSurface.Left + windowSurface.Width;
                zoomWindowSurface.Top = windowSurface.Top;
                zoomWindowSurface.Show();

                WindowInteropHelper helper = new WindowInteropHelper(zoomWindowSurface);
                int windHandel = int.Parse(helper.Handle.ToString());
                PointsForZoom pointsForZoom = new PointsForZoom(_pixelPanelForZoom.TopLeft.X, _pixelPanelForZoom.TopLeft.Y, _pixelPanelForZoom.BottomRight.X, _pixelPanelForZoom.BottomRight.Y);
                ConnectedManager.Rectangle_MouseMove_SendPoint(windowSurface.Tag as string, pointsForZoom);

                OpenZoom openZoom = new OpenZoom(true, windHandel, (int)zoomWindowSurface.Width, (int)zoomWindowSurface.Height, int.Parse(windowSurface.Tag as string));
                ConnectedManager.SendWindowForZoom(openZoom);


                zoomWindowSurface.Tag = windowSurface.Tag as string;



            CreateCanvalInOverlay(sender);
           

        }
        private void OnChengeZoomMode(object sender, RoutedEventArgs e)
        {
            Button zommButton = sender as Button;
            var gridOverlay = zommButton.Parent as Grid;

            var zoom = AssociatedObject.Children.OfType<CustomGrid>().Where(c => c.CameraGuidName == zommButton.Name).FirstOrDefault();
            if (testFlag)
            {
                if (!zoom.OnZoomMode)
                {
                    zoom.OnZoomMode = true;
                    ZoomModeOn(zommButton);
                }
                else
                {
                    zoom.OnZoomMode = false;
                    ZoomModeOffInSeparateWindow(zommButton);
                }

            }
            else
            if (!zoom.OnZoomMode)
            {
                zoom.OnZoomMode = true;
                ZoomModeOn(zommButton);
            }
            else
            {
                zoom.OnZoomMode = false;
                ZoomModeOffInHostGrid(zommButton);
            }



        }


        private void ZoomModeOffInSeparateWindow(Button button)
        {
            Grid gridButton = button.Parent as Grid;
            gridButton.Visibility = Visibility.Visible;
            gridButton.MouseMove += OnMouseEnterWindowShowOverlay;
            gridButton.MouseLeave += OnMouseLeaveOverLay;
            button.Style = (Style)System.Windows.Application.Current.FindResource("ButtonStyleOnZoomMode");

            gridButton.Opacity = 0.5;
            if ((gridButton.Parent as Canvas) != null)
                ((gridButton.Parent as Canvas).Parent as Grid).SizeChanged -= OnSizeChengeOverlayWindow;

            var hostGrid = AssociatedObject as Grid;
            var surfaceGrid = hostGrid.Children.OfType<CustomGrid>().Where(c => c.CameraGuidName == button.Name).FirstOrDefault();
            if (surfaceGrid == null) return;
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
           
            
            
            if (surfaceGrid.Window.OwnedWindows.Count==1)
            {
                //surfaceGrid.Window.OwnedWindows[2].Close();
                SetConnect set = new SetConnect(true, int.Parse(surfaceGrid.ProcessTag));
                ConnectedManager.SendChandeConekting(set);
                return;
            }

            Grid gridOverlayCanvals = ((Border)surfaceGrid.Window.OwnedWindows[0].Content).Child as Grid;
            if (surfaceGrid.Window.OwnedWindows.Count > 1)
            {
                OpenZoom openZoom = new OpenZoom(false, 0, 1, 1, int.Parse(surfaceGrid.ProcessTag));
                ConnectedManager.SendWindowForZoom(openZoom);
                surfaceGrid.Window.OwnedWindows[1].Close();
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
        }
        private void ZoomModeOn(Button button)
        {

            button.Style = (Style)System.Windows.Application.Current.FindResource("ButtonStyleOffZoomMode");
            Grid gridButton = button.Parent as Grid;
            gridButton.Visibility = Visibility.Visible;
            gridButton.MouseMove -= OnMouseEnterWindowShowOverlay;
            gridButton.MouseLeave -= OnMouseLeaveOverLay;

            gridButton.Opacity = 1;
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
           

            SetConnect setConnect = new SetConnect(false, int.Parse(surfaceGrid.ProcessTag));
            ConnectedManager.SendChandeConekting(setConnect);



        }

        private void ZoomModeOffInHostGrid(Button button)
        {
            Grid gridButton = button.Parent as Grid;
            gridButton.Visibility = Visibility.Visible;
            gridButton.MouseMove += OnMouseEnterWindowShowOverlay;
            gridButton.MouseLeave += OnMouseLeaveOverLay;
            button.Style = (Style)System.Windows.Application.Current.FindResource("ButtonStyleOnZoomMode");

            gridButton.Opacity = 0.5;
            if ((gridButton.Parent as Canvas) != null)
                ((gridButton.Parent as Canvas).Parent as Grid).SizeChanged -= OnSizeChengeOverlayWindow;
            
            var hostGrid = AssociatedObject as Grid;
            var surfaceGrid = hostGrid.Children.OfType<CustomGrid>().Where(c => c.CameraGuidName == button.Name).FirstOrDefault();
            if (surfaceGrid == null) return;
            CustomGrid zoomGrid = hostGrid.Children.OfType<CustomGrid>().Where(c => c.Name == button.Name).FirstOrDefault();
            hostGrid.Children.Remove(zoomGrid);
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
            
            if (zoomGrid == null)
            {
                SetConnect set = new SetConnect(true, int.Parse(surfaceGrid.ProcessTag));
                ConnectedManager.SendChandeConekting(set);
                return;
            }

            Grid gridOverlayCanvals = ((Border)surfaceGrid.Window.OwnedWindows[0].Content).Child as Grid;
            if (surfaceGrid.Window.OwnedWindows.Count > 1)
            {
                OpenZoom openZoom = new OpenZoom(false, 0, 1, 1, int.Parse(surfaceGrid.ProcessTag));
                ConnectedManager.SendWindowForZoom(openZoom);
                zoomGrid.Window.Close();
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



        }


        private void OnRectangleMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
             
            Canvas canvas = VisualTreeHelper.GetParent(sender as System.Windows.Shapes.Rectangle) as Canvas;
            _startPoint = e.GetPosition(canvas);

        }
         
      

        private void OnMouseDownTakePosition(object? sender, MouseButtonEventArgs e)
        {
         
                _pixelPanelForZoom.TopLeft = new Point(e.GetPosition(sender as Window).X, e.GetPosition(sender as Window).Y);
             
        }

    }
}
