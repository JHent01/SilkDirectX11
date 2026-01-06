using LibraryForSignalR;
using MahApps.Metro.Controls;
using SilkDirectX11.Events;
using SilkDirectX11.Model;
using SilkDirectX11.SignalR;
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
           
          
            var grid = riteGrid.Children.OfType<CustomGrid>().Where(c => c.CameraGuidName == wind.Name).FirstOrDefault();
               if (grid.Window != null)
                {
 
                OpenFullScreenAndZoom(wind);
                }
                else
                {
                   
                    CloseFullScreenAndZoom( wind ); 
                }
             
        }
        private void OpenFullScreenAndZoom(Window wind )  
        {
            var riteGrid = AssociatedObject as Grid;
            var MainGrid = riteGrid.Parent as Grid;
            var grid = riteGrid.Children.OfType<CustomGrid>().Where(c => c.CameraGuidName == wind.Name).FirstOrDefault();
            Grid gridOverlayCanvals = ((Border)grid.Window.OwnedWindows[0].Content).Child as Grid;
            Grid gridOverlay = gridOverlayCanvals.Children.OfType<Grid>().FirstOrDefault();

           gridOverlay.Visibility = Visibility.Hidden;
           

            //_windowOverlay.Width = riteGrid.ActualWidth;
            //_windowOverlay.Height = riteGrid.ActualHeight;
            //_windowOverlay.Left = riteGrid.PointToScreen(new Point()).X;
            //_windowOverlay.Top = riteGrid.PointToScreen(new Point()).Y;


            AssociatedObject.Drop -= AssociatedObject_Drop;
            grid.Window.Drop -= AssociatedObject_Drop;
            grid.Window.MouseLeave -= OnMouseLeaveHideOverlay;
            grid.Window.MouseMove -= OnMouseMuveWindowShow;
            grid.Window.MouseDown -= StartDragDrop;
           
            wind.Tag = grid.ProcessTag;
            grid.Window.Width = riteGrid.ActualWidth;
            grid.Window.Height = riteGrid.ActualHeight;
            _pixelPanelForZoom.TopLeft.X = 0;
            // wind.Focus();

            

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
                newGr.Children.Add(new Border()
                {
                    Background = System.Windows.Media.Brushes.Transparent,
                    BorderBrush = System.Windows.Media.Brushes.Yellow,
                    BorderThickness = new Thickness(5),
                    CornerRadius = new CornerRadius(5),
                    Margin = new Thickness(1, 1, 1, 1),
                    Padding = new Thickness(1, 1, 1, 1),
                    

                });
                grid.Window = null;

                MainGrid.Children.Add(newGr);
                Grid.SetColumn(newGr, MainGrid.ColumnDefinitions.Count);
                SetConnect setConnect = new SetConnect(false, int.Parse(grid.ProcessTag));
                ConnectedManager.SendChandeConekting(setConnect);
                //SendChandeConekting(setConnect);
                CustomGrid GridFullScreen = MainGrid.Children.OfType<CustomGrid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();

                MainGrid.SizeChanged += OnMainGridSizeChenge;

                GridFullScreen.Window.MouseUp += OnMouseUpTakePosition;
                GridFullScreen.Window.MouseDown += OnMouseDownTakePosition;
                GridFullScreen.Window.Left = riteGrid.PointToScreen(new Point()).X;
                GridFullScreen.Window.Top = riteGrid.PointToScreen(new Point()).Y;

                //GridFullScreen.Window.OwnedWindows[0].Focus();
                _eventAggregator.GetEvent<VisibilityChengeEvent>().Publish(Visibility.Hidden);
            }
        }

        private void CloseFullScreenAndZoom(Window wind)
        {
            wind.Tag = null;
              
            Grid gridOverlayCanvals = ((Border)wind.OwnedWindows[0].Content).Child as Grid;
   
            if (gridOverlayCanvals.Children.OfType<Canvas>().FirstOrDefault() != null)
            {
                ClouseZoom();
              
            }
             ClouseFullScreen(wind);
        }

        private void ClouseFullScreen(Window wind)
        {
            var riteGrid = AssociatedObject as Grid;
            var MainGrid = riteGrid.Parent as Grid;
            var parent = riteGrid.Children.OfType<CustomGrid>().Where(c => c.CameraGuidName == wind.Name).FirstOrDefault();
            CustomGrid gridFullScreen = MainGrid.Children.OfType<CustomGrid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();

            Grid gridOverlayCanvals = ((Border)gridFullScreen.Window.OwnedWindows[0].Content).Child as Grid;
            Grid gridOverlay = gridOverlayCanvals.Children.OfType<Grid>().FirstOrDefault();
            gridOverlay.Visibility = Visibility.Visible;
            _eventAggregator.GetEvent<VisibilityChengeEvent>().Publish(Visibility.Visible);
            //_windowOverlay.Width = riteGrid.ActualWidth;
            //_windowOverlay.Height = riteGrid.ActualHeight;
             
            AssociatedObject.Drop += AssociatedObject_Drop;
            gridFullScreen.Window.MouseUp -= OnMouseUpTakePosition;
            gridFullScreen.Window.MouseDown -= OnMouseDownTakePosition;
            MainGrid.SizeChanged -= OnMainGridSizeChenge;
             
            parent.Window = gridFullScreen.Window;
            gridFullScreen.Window = null;
            gridFullScreen.Children.Clear();
            MainGrid.Children.Remove(gridFullScreen);

            parent.Window.Width = parent.ActualWidth - 10;
            parent.Window.Height = parent.ActualHeight - 10;
            parent.Window.MouseLeave += OnMouseLeaveHideOverlay;
            parent.Window.MouseMove += OnMouseMuveWindowShow;
            parent.Window.MouseDown += StartDragDrop;

            parent.Window.Drop += AssociatedObject_Drop;
            parent.Window.Left = parent.PointToScreen(new Point()).X + 5;
            parent.Window.Top = parent.PointToScreen(new Point()).Y + 5;

            SetConnect setConnect = new SetConnect(true, int.Parse(parent.ProcessTag));
            ConnectedManager.SendChandeConekting(setConnect);
           // SendChandeConekting(setConnect);
            
            _pixelPanelForZoom.TopLeft.X = 0;
            var canvas = gridOverlay.Children.OfType<Canvas>().FirstOrDefault();
             
            if (canvas != null)

                gridOverlay.Children.Remove(canvas);
        }
        private void ClouseZoom(   )
        {
            var riteGrid = AssociatedObject as Grid;
            var MainGrid = riteGrid.Parent as Grid;
            CustomGrid gridFullScreen = MainGrid.Children.OfType<CustomGrid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();
            Grid gridOverlayCanvals = ((Border)gridFullScreen.Window.OwnedWindows[0].Content).Child as Grid;
            if (gridFullScreen.Window.OwnedWindows.Count > 1)
                {
                    OpenZoom openZoom = new OpenZoom(false, 0, 1, 1, int.Parse(gridFullScreen.ProcessTag));
                ConnectedManager.SendWindowForZoom(openZoom);
               // SendWindowForZoom(openZoom);
                    gridFullScreen.Window.OwnedWindows[1].Close();
                 
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
                if (gridFullScreen.Window.OwnedWindows.Count > 1 )
                {
                    gridFullScreen.Window.OwnedWindows[1].Width = gridRite.ActualWidth/2-10;
                    gridFullScreen.Window.OwnedWindows[1].Height = gridRite.ActualHeight-10;
                    gridFullScreen.Width = gridRite.ActualWidth/2;
                    gridFullScreen.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    gridFullScreen.Window.OwnedWindows[1].Left = gridRite.PointToScreen(new Point()).X+ gridRite.ActualWidth / 2+5;
                    gridFullScreen.Window.OwnedWindows[1].Top = gridRite.PointToScreen(new Point()).Y+5;
                }
                else
                {
                    gridFullScreen.Width = gridRite.ActualWidth;
                }

                //_windowOverlay.Width = gridFullScreen.ActualWidth;
                //_windowOverlay.Height = gridFullScreen.ActualHeight;
                var canvas = gridFullScreen.Window.OwnedWindows[0].FindChild<Canvas>();
                if (canvas != null)
                {
                    canvas.Width = gridFullScreen.ActualWidth;
                    canvas.Height = gridFullScreen.ActualHeight;
                }
                
                //_windowOverlay.Left = gridFullScreen.PointToScreen(new Point()).X;
                //_windowOverlay.Top = gridFullScreen. PointToScreen(new Point()).Y;
               
            }
        }
    }
}
