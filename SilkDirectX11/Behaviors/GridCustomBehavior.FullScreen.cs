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
using System.Windows.Input;
using Point = System.Windows.Point;

namespace SilkDirectX11.Behaviors
{
    partial class GridCustomBehavior
    {
       
        private void MouseRiteClick(object? sender, MouseButtonEventArgs e)
        {
           // if (e.Button != System.Windows.Forms.MouseButtons.Right) return;
            var wind = sender as Window;
            var riteGrid = AssociatedObject as Grid;
            windowOverlay.Width = riteGrid.ActualWidth;
            windowOverlay.Height = riteGrid.ActualHeight;

            Grid gridOverlayCanvals = ((Border)windowOverlay.Content).Child as Grid;

            Grid gridOverlay = gridOverlayCanvals.Children.OfType<Grid>().FirstOrDefault();
            gridOverlay.Visibility = Visibility.Hidden;

            var grid = riteGrid.Children.OfType<CustomGrid>().Where(c => c.CameraGuidName == wind.Name).FirstOrDefault();

            if (grid != null)
            {
                var processTag = grid.ProcessTag ;

                //riteGrid.Visibility = Visibility.Hidden;

                var full = riteGrid.Parent as Grid;
              //  var wfh = grid.Children.OfType<WindowsFormsHost>().FirstOrDefault();
                if (grid.Window.OwnedWindows != null)
                {
                    AssociatedObject.Drop -= AssociatedObject_Drop;
                    //panel.MouseUp -= MouseUps;//----------
                    //grid.Children.Remove(wfh);
                    grid.Window.Owner = null;
                    grid.Window.Width = riteGrid.ActualWidth;
                    grid.Window.Height = riteGrid.ActualHeight;
                    grid.Window.MouseLeave -= Leave;
                    grid.Window.MouseMove -= WindowShow;
                    wind.Tag = processTag;
                    windowOverlay.Left = riteGrid.PointToScreen(new Point()).X;
                    windowOverlay.Top = riteGrid.PointToScreen(new Point()).Y;
                    //if (riteGrid.Children.OfType<CustomGrid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault() == null)
                    ////{
                    //    riteGrid.Children.Add(new CustomGrid()
                    //    {
                    //        Name = "FullScreenGrid",
                    //        Width = riteGrid.ActualWidth,
                    //        Height = riteGrid.ActualHeight,
                             


                    //        //Margin = new Thickness((full.ColumnDefinitions.First().Width).Value + 5, 0, 0, 0),
                    //        Tag = processTag
                    //    });
                        SetConnect setConnect = new SetConnect(false, int.Parse(processTag));
                        SendChandeConekting(setConnect);
                    grid.Window.MouseUp += MouseUpTakePixel;
                    grid.Window.MouseDown += MouseDownTakePxel;
                        pixelPanelForZoom.TopLeft.X = 0;

                        riteGrid.SizeChanged += Full_SizeChanged;
                    //}

                    //if (full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault() == null)
                    //{
                    //    full.Children.Add(new Grid()

                    //    {
                    //        Name = "FullScreenGrid",
                    //        Width = riteGrid.ActualWidth,
                    //        Height = riteGrid.ActualHeight,
                    //        Children = { wfh },//newWind

                    //        Margin = new Thickness((full.ColumnDefinitions.First().Width).Value + 5, 0, 0, 0),
                    //        Tag = processTag
                    //    });
                    //    SetConnect setConnect = new SetConnect(false, int.Parse(processTag));
                    //    SendChandeConekting(setConnect); 
                    //    WindowsFormsHost host = full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault()?.Children.OfType<WindowsFormsHost>().FirstOrDefault();

                    //    wfh.Child.MouseUp += MouseUpTakePixel;
                    //    wfh.Child.MouseDown += MouseDownTakePxel;
                    //    pixelPanelForZoom.TopLeft.X = 0;

                    //    full.SizeChanged += Full_SizeChanged;

                    //}
                    //else
                    //{// -
                    //    CloseZoomPanel(gridOverlay, riteGrid, full, gridOverlayCanvals, grid, processTag);
                    //}

                }
                else
                {
                    AssociatedObject.Drop += AssociatedObject_Drop;
                    //panel.MouseUp += MouseUps;//------
                    CloseZoomPanel(gridOverlay, riteGrid, full, gridOverlayCanvals, grid, processTag);
                }
            }
        }

        

        private void Full_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Grid riteGrid = sender as Grid;
            //WindowsFormsHost host = riteGrid.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault()?.Children.OfType<WindowsFormsHost>().FirstOrDefault();
            if (riteGrid.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault() != null)//мотом тут донастроить 
            {
                WindowsFormsHost host = riteGrid.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault()?.Children.OfType<WindowsFormsHost>().FirstOrDefault();

                Grid fullScreenGrid = riteGrid.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();

                if (fullScreenGrid != null)
                {
                    WindowsFormsHost zoomHost = fullScreenGrid.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault();

                    fullScreenGrid.Width = riteGrid.ActualWidth;
                    fullScreenGrid.Height = riteGrid.ActualHeight;
                    if (zoomHost != null)
                    {
                        zoomHost.Height = riteGrid.ActualHeight;
                        zoomHost.Width = (int)(riteGrid.ActualWidth / 2);
                        zoomHost.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                        host.Width = riteGrid.ActualWidth / 2;
                        host.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    }
                }
                host.Width = riteGrid.ActualWidth;
                host.Height = riteGrid.ActualHeight;
                BetterPanelTest chil = (BetterPanelTest)host.Child;
                chil.Width = (int)riteGrid.ActualWidth;
                chil.Height = (int)riteGrid.ActualHeight;
                windowOverlay.Width = riteGrid.ActualWidth;
                windowOverlay.Height = riteGrid.ActualHeight;
                try
                {
                    windowOverlay.Left = host.PointToScreen(new Point()).X;
                    windowOverlay.Top = host.PointToScreen(new Point()).Y;
                }
                catch
                { }
            }
        }

        private void CloseZoomPanel(Grid gridOverlay, Grid Rite, Grid full, Grid gridOverlayCanvals, Grid parent, string processTag) 
        {

            Grid gridFullScreen = Rite.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();

            if (gridOverlayCanvals.Children.OfType<Canvas>().FirstOrDefault() != null)
            {

                if (gridFullScreen.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault() != null/*gridFullScreen.Children.Count > 1*/)
                {
                    WindowsFormsHost zoom = gridFullScreen.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault();
                    //var tagZoom = zoom.Tag as string;
                    BetterPanelTest panel = zoom.Child as BetterPanelTest;
                    gridFullScreen.Children.Remove(zoom);

                    try
                    {
                        OpenZoom openZoom = new OpenZoom(false, 0, 1, 1, int.Parse(gridFullScreen.Tag.ToString()));
                        SendWindowForZoom(openZoom);
                    }
                    catch (Exception ex)
                    {//возможно потом логика вывода ошибок 
                        string exs = ex.Message;
                    }
                }
                var childOverlay = gridOverlayCanvals.Children.OfType<Canvas>().FirstOrDefault();
                gridOverlayCanvals.Children.Remove(childOverlay);
            }
            var wfh = gridFullScreen.Children.OfType<WindowsFormsHost>().FirstOrDefault();
            Rite.Visibility = Visibility.Visible;
           // wfh.Child.MouseUp -= MouseUpTakePixel;
           // wfh.Child.MouseDown -= MouseDownTakePxel;
            var tag = gridFullScreen.Tag as string;
            gridFullScreen.SizeChanged -= Full_SizeChanged;
            pixelPanelForZoom.TopLeft.X = 0;
            // string tagChild = wfh.Tag.ToString();




            gridFullScreen.Children.Clear();
            Rite.Children.Remove(gridFullScreen);

            wfh.Width = parent.ActualWidth;
            wfh.Height = parent.ActualHeight;
            wfh.Child.MouseLeave += Leave;
            wfh.Child.MouseMove += WindowShow;


            parent.Children.Add(wfh);
            parent.Margin = new Thickness(5);
            SetConnect setConnect = new SetConnect(true, int.Parse(processTag));
            SendChandeConekting(setConnect);

            var child = gridOverlay.Children.OfType<Canvas>().FirstOrDefault();
            if (child != null)
            {
                gridOverlay.Children.Remove(child);
            }

        }
    }
}
