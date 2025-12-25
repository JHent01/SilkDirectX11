using LibraryForSignalR;
using MahApps.Metro.Controls;
using SilkDirectX11.Events;
using SilkDirectX11.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Point = System.Windows.Point;

namespace SilkDirectX11.Behaviors
{
    partial class GridCustomBehavior
    {
       
        private void OnChangeFullScreen(object? sender, MouseButtonEventArgs e)
        {
           
            var wind = sender as Window;
            var riteGrid = AssociatedObject as Grid;
            windowOverlay.Width = riteGrid.ActualWidth;
            windowOverlay.Height = riteGrid.ActualHeight;

            Grid gridOverlayCanvals = ((Border)windowOverlay.Content).Child as Grid;

            Grid gridOverlay = gridOverlayCanvals.Children.OfType<Grid>().FirstOrDefault();
            gridOverlay.Visibility = Visibility.Hidden;
          
            var grid = riteGrid.Children.OfType<CustomGrid>().Where(c => c.CameraGuidName == wind.Name).FirstOrDefault();
             
                var MainGrid = riteGrid.Parent as Grid;
                
                if (grid.Window != null)
                {

                // _eventAggregator.GetEvent<VisibilityChengeEvent>().Publish(Visibility.Hidden);
                // AssociatedObject.Drop -= AssociatedObject_Drop;
                // grid.Window.Drop -= AssociatedObject_Drop;
                // grid.Window.Width = riteGrid.ActualWidth;
                // grid.Window.Height = riteGrid.ActualHeight;
                // grid.Window.MouseLeave -= OnMouseLeaveHideOverlay;
                // grid.Window.MouseMove -= OnMouseMuveWindowShow;
                // grid.Window.MouseDown -= StartDragDrop;
                //// grid.Window.MouseUp -= MouseUps;
                // wind.Tag = grid.ProcessTag;
                // windowOverlay.Left = riteGrid.PointToScreen(new Point()).X;
                // windowOverlay.Top = riteGrid.PointToScreen(new Point()).Y;
                // wind.Focus();

                //     pixelPanelForZoom.TopLeft.X = 0;

                // if (MainGrid.Children.OfType<CustomGrid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault() == null)
                // {
                // CustomGrid newGr = new CustomGrid()

                // {
                //     Name = "FullScreenGrid",
                //     Width = riteGrid.ActualWidth,
                //     Height = riteGrid.ActualHeight,
                //     DataContext = grid.DataContext,
                //     Window = grid.Window,
                //     ProcessTag = grid.ProcessTag,
                //     CameraConnectStrings = grid.CameraConnectStrings,
                //     CameraGuidName = grid.CameraGuidName

                // };

                // grid.Window = null;

                // MainGrid.Children.Add(newGr);
                // Grid.SetColumn(newGr, MainGrid.ColumnDefinitions.Count);
                // SetConnect setConnect = new SetConnect(false, int.Parse(grid.ProcessTag));
                //     SendChandeConekting(setConnect);
                // CustomGrid GridFullScreen = MainGrid.Children.OfType<CustomGrid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();

                // MainGrid.SizeChanged += OnMainGridSizeChenge;

                // GridFullScreen.Window.MouseUp += MouseUpTakePixel;
                // GridFullScreen.Window.MouseDown += MouseDownTakePxel;
                // GridFullScreen.Window.Left = riteGrid.PointToScreen(new Point()).X;
                // GridFullScreen.Window.Top = riteGrid.PointToScreen(new Point()).Y;

                // pixelPanelForZoom.TopLeft.X = 0;

                // }
                OpenFullScreenAndZoom(wind, grid);
                }
                else
                {
                    wind.Tag=null;
                    
                   
                    CloseFullScreenAndZoom(gridOverlay,  MainGrid, gridOverlayCanvals, grid );//riteGrid,grid.ProcessTag,
                }
             
        }
        private void OpenFullScreenAndZoom(Window wind,  CustomGrid grid) //Grid Rite,, string processTag
        {
            var riteGrid = AssociatedObject as Grid;
            var MainGrid = riteGrid.Parent as Grid;

            _eventAggregator.GetEvent<VisibilityChengeEvent>().Publish(Visibility.Hidden);
            AssociatedObject.Drop -= AssociatedObject_Drop;
            grid.Window.Drop -= AssociatedObject_Drop;
            grid.Window.Width = riteGrid.ActualWidth;
            grid.Window.Height = riteGrid.ActualHeight;
            grid.Window.MouseLeave -= OnMouseLeaveHideOverlay;
            grid.Window.MouseMove -= OnMouseMuveWindowShow;
            grid.Window.MouseDown -= StartDragDrop;
            // grid.Window.MouseUp -= MouseUps;
            wind.Tag = grid.ProcessTag;
            windowOverlay.Left = riteGrid.PointToScreen(new Point()).X;
            windowOverlay.Top = riteGrid.PointToScreen(new Point()).Y;
            wind.Focus();

            pixelPanelForZoom.TopLeft.X = 0;

            if (MainGrid.Children.OfType<CustomGrid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault() == null)
            {
                CustomGrid newGr = new CustomGrid()

                {
                    Name = "FullScreenGrid",
                    Width = riteGrid.ActualWidth,
                    Height = riteGrid.ActualHeight,
                    DataContext = grid.DataContext,
                    Window = grid.Window,
                    ProcessTag = grid.ProcessTag,
                    CameraConnectStrings = grid.CameraConnectStrings,
                    CameraGuidName = grid.CameraGuidName

                };

                grid.Window = null;

                MainGrid.Children.Add(newGr);
                Grid.SetColumn(newGr, MainGrid.ColumnDefinitions.Count);
                SetConnect setConnect = new SetConnect(false, int.Parse(grid.ProcessTag));
                SendChandeConekting(setConnect);
                CustomGrid GridFullScreen = MainGrid.Children.OfType<CustomGrid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();

                MainGrid.SizeChanged += OnMainGridSizeChenge;

                GridFullScreen.Window.MouseUp += MouseUpTakePixel;
                GridFullScreen.Window.MouseDown += MouseDownTakePxel;
                GridFullScreen.Window.Left = riteGrid.PointToScreen(new Point()).X;
                GridFullScreen.Window.Top = riteGrid.PointToScreen(new Point()).Y;

                pixelPanelForZoom.TopLeft.X = 0;
            }
        }

        private void CloseFullScreenAndZoom(Grid gridOverlay,  Grid MainGrid, Grid gridOverlayCanvals, CustomGrid parent) //Grid Rite,, string processTag
        {
            AssociatedObject.Drop += AssociatedObject_Drop;
            gridOverlay.Visibility = Visibility.Visible;
            CustomGrid gridFullScreen = MainGrid.Children.OfType<CustomGrid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();
            gridFullScreen.Window.MouseUp -= MouseUpTakePixel;
            gridFullScreen.Window.MouseDown -= MouseDownTakePxel;
            if (gridOverlayCanvals.Children.OfType<Canvas>().FirstOrDefault() != null)
            {
                ClouseZoom(gridFullScreen, gridOverlayCanvals);
              
            }
            _eventAggregator.GetEvent<VisibilityChengeEvent>().Publish(Visibility.Visible);

            MainGrid.SizeChanged -= OnMainGridSizeChenge;
            pixelPanelForZoom.TopLeft.X = 0;
           
            parent.Window = gridFullScreen.Window;
            gridFullScreen.Window= null;
            gridFullScreen.Children.Clear();
            MainGrid.Children.Remove(gridFullScreen);

            parent.Window.Width = parent.ActualWidth-10;
            parent.Window.Height = parent.ActualHeight-10;
            parent.Window.MouseLeave += OnMouseLeaveHideOverlay;
            parent.Window.MouseMove += OnMouseMuveWindowShow;
            parent.Window.MouseDown += StartDragDrop;
          
            parent.Window.Drop += AssociatedObject_Drop;
            parent.Window.Left = parent.PointToScreen(new Point()).X+5;
            parent.Window.Top = parent.PointToScreen(new Point()).Y+5;

            SetConnect setConnect = new SetConnect(true, int.Parse(parent.ProcessTag));
            SendChandeConekting(setConnect);

            var canvas = gridOverlay.Children.OfType<Canvas>().FirstOrDefault();
            if (canvas != null)
            
                gridOverlay.Children.Remove(canvas);
            

        }
        private void ClouseZoom(CustomGrid gridFullScreen, Grid gridOverlayCanvals)
        {
           

                if (gridFullScreen.Window.OwnedWindows.Count > 0)
                {
                    OpenZoom openZoom = new OpenZoom(false, 0, 1, 1, int.Parse(gridFullScreen.ProcessTag));
                    SendWindowForZoom(openZoom);
                    gridFullScreen.Window.OwnedWindows[0].Close();



                }
            var childOverlay = gridOverlayCanvals.Children.OfType<Canvas>().FirstOrDefault();
            gridOverlayCanvals.Children.Remove(childOverlay);

        }

        private void OnMainGridSizeChenge(object sender, SizeChangedEventArgs e)
        {
            Grid MainGrid = sender as Grid;
            var gridFullScreen = MainGrid.Children.OfType<CustomGrid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();
            var gridRite = AssociatedObject as Grid;    
            if (gridFullScreen != null)
            {
                gridFullScreen.Height = gridRite.ActualHeight;
                if (gridFullScreen.Window.OwnedWindows.Count > 0 )
                {
                    gridFullScreen.Window.OwnedWindows[0].Width = gridRite.ActualWidth/2;
                    gridFullScreen.Window.OwnedWindows[0].Height = gridRite.ActualHeight;
                    gridFullScreen.Width = gridRite.ActualWidth/2;
                    gridFullScreen.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    gridFullScreen.Window.OwnedWindows[0].Left = gridRite.PointToScreen(new Point()).X+ gridRite.ActualWidth / 2;
                    gridFullScreen.Window.OwnedWindows[0].Top = gridRite.PointToScreen(new Point()).Y;
                }
                else
                {
                    gridFullScreen.Width = gridRite.ActualWidth;
                }

                windowOverlay.Width = gridFullScreen.ActualWidth;
                windowOverlay.Height = gridFullScreen.ActualHeight;
                var canvas = windowOverlay.FindChild<Canvas>();
                if (canvas != null)
                {
                    canvas.Width = gridFullScreen.ActualWidth;
                    canvas.Height = gridFullScreen.ActualHeight;
                }
                
                windowOverlay.Left = gridFullScreen.PointToScreen(new Point()).X;
                windowOverlay.Top = gridFullScreen. PointToScreen(new Point()).Y;
               
            }
        }
    }
}
