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
       
        private void MouseRiteClick(object? sender, MouseButtonEventArgs e)
        {
           
            var wind = sender as Window;
            var riteGrid = AssociatedObject as Grid;
            windowOverlay.Width = riteGrid.ActualWidth;
            windowOverlay.Height = riteGrid.ActualHeight;

            Grid gridOverlayCanvals = ((Border)windowOverlay.Content).Child as Grid;

            Grid gridOverlay = gridOverlayCanvals.Children.OfType<Grid>().FirstOrDefault();
            gridOverlay.Visibility = Visibility.Hidden;
          
            var grid = riteGrid.Children.OfType<CustomGrid>().Where(c => c.CameraGuidName == wind.Name).FirstOrDefault();
             
                var full = riteGrid.Parent as Grid;
                
                if (grid.Window != null)
                {
                   
                    _eventAggregator.GetEvent<VisibilityChengeEvent>().Publish(Visibility.Hidden);
                    AssociatedObject.Drop -= AssociatedObject_Drop;
                    grid.Window.Drop -= AssociatedObject_Drop;
                    grid.Window.Width = riteGrid.ActualWidth;
                    grid.Window.Height = riteGrid.ActualHeight;
                    grid.Window.MouseLeave -= Leave;
                    grid.Window.MouseMove -= WindowShow;
                    grid.Window.MouseDown -= Child_MouseDown;
                   // grid.Window.MouseUp -= MouseUps;
                    wind.Tag = grid.ProcessTag;
                    windowOverlay.Left = riteGrid.PointToScreen(new Point()).X;
                    windowOverlay.Top = riteGrid.PointToScreen(new Point()).Y;
                    wind.Focus();
                    
                        pixelPanelForZoom.TopLeft.X = 0;
                 
                    if (full.Children.OfType<CustomGrid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault() == null)
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
                  
                    full.Children.Add(newGr);
                    Grid.SetColumn(newGr, full.ColumnDefinitions.Count);
                    SetConnect setConnect = new SetConnect(false, int.Parse(grid.ProcessTag));
                        SendChandeConekting(setConnect);
                    CustomGrid host = full.Children.OfType<CustomGrid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();

                    full.SizeChanged += Full_SizeChanged;

                    host.Window.MouseUp += MouseUpTakePixel;
                    host.Window.MouseDown += MouseDownTakePxel;
                    host.Window.Left = riteGrid.PointToScreen(new Point()).X;
                    host.Window.Top = riteGrid.PointToScreen(new Point()).Y;
                     
                    pixelPanelForZoom.TopLeft.X = 0;
                     
                    }
                
                }
                else
                {
                    wind.Tag=null;
                    AssociatedObject.Drop += AssociatedObject_Drop;
                   
                    CloseZoomPanel(gridOverlay, riteGrid, full, gridOverlayCanvals, grid, grid.ProcessTag);
                }
             
        }
         
        private void CloseZoomPanel(Grid gridOverlay, Grid Rite, Grid full, Grid gridOverlayCanvals, CustomGrid parent, string processTag) 
        {
            gridOverlay.Visibility = Visibility.Visible;
            CustomGrid gridFullScreen = full.Children.OfType<CustomGrid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();
            gridFullScreen.Window.MouseUp -= MouseUpTakePixel;
            gridFullScreen.Window.MouseDown -= MouseDownTakePxel;
            if (gridOverlayCanvals.Children.OfType<Canvas>().FirstOrDefault() != null)
            {

                if (gridFullScreen.Window.OwnedWindows.Count>0  )
                {
                    OpenZoom openZoom = new OpenZoom(false, 0, 1, 1, int.Parse(gridFullScreen.ProcessTag));
                    SendWindowForZoom(openZoom);
                    gridFullScreen.Window.OwnedWindows[0].Close();
                   
                       
                   
                }
                var childOverlay = gridOverlayCanvals.Children.OfType<Canvas>().FirstOrDefault();
                gridOverlayCanvals.Children.Remove(childOverlay);
            }
             _eventAggregator.GetEvent<VisibilityChengeEvent>().Publish(Visibility.Visible);

            full.SizeChanged -= Full_SizeChanged;
            pixelPanelForZoom.TopLeft.X = 0;
           
            parent.Window = gridFullScreen.Window;
            gridFullScreen.Window= null;
            gridFullScreen.Children.Clear();
            full.Children.Remove(gridFullScreen);

            parent.Window.Width = parent.ActualWidth;
            parent.Window.Height = parent.ActualHeight;
            parent.Window.MouseLeave += Leave;
            parent.Window.MouseMove += WindowShow;
            parent.Window.MouseDown += Child_MouseDown;
          //  parent.Window.MouseUp += MouseUps;
            parent.Window.Drop += AssociatedObject_Drop;
            parent.Window.Left = parent.PointToScreen(new Point()).X;
            parent.Window.Top = parent.PointToScreen(new Point()).Y;

            SetConnect setConnect = new SetConnect(true, int.Parse(processTag));
            SendChandeConekting(setConnect);

            var child = gridOverlay.Children.OfType<Canvas>().FirstOrDefault();
            if (child != null)
            
                gridOverlay.Children.Remove(child);
            

        }
        private void Full_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Grid full = sender as Grid;
            var gridFullScreen = full.Children.OfType<CustomGrid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();
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
