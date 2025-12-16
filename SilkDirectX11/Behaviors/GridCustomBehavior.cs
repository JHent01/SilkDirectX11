using LibraryForSignalR;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Xaml.Behaviors;
using RenderANDVideoReaderVIdeoDecoder;
using SilkDirectX11.Model;
using SilkDirectX11.SignalR;
using SilkDirectX11.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using Vortice.Direct2D1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Controls.Button;
using Image = System.Windows.Controls.Image;
using Panel = System.Windows.Forms.Panel;
using Point = System.Windows.Point;
using Window = System.Windows.Window;

namespace SilkDirectX11.Behaviors
{
    class GridCustomBehavior : Behavior<Grid>
    {
        //static string patch = Path.Combine(GetSolutionParentPath(), "RenderANDVideoReaderVIdeoDecoder", "RenderANDVideoReaderVIdeoDecoder", "bin", "Debug", "net8.0", "RenderANDVideoReaderVIdeoDecoder.exe");
        System.Windows.Window window = new();
         static string patch = Path.Combine(GetSolutionParentPath(), "Rend", "RenderANDVideoReaderVIdeoDecoder", "RenderANDVideoReaderVIdeoDecoder", "bin", "Debug", "net8.0", "RenderANDVideoReaderVIdeoDecoder.exe");
        bool flagForOverlay;
        CameraDragDrop cameraDragDrop = new CameraDragDrop();
        PixelPanelForZoom pixelPanelForZoom = new PixelPanelForZoom();
        private HubConnection? _connection;
        protected override void OnAttached()
        {
            base.OnAttached();

            _ = EnsureSignalRAsync();

            AssociatedObject.PreviewDragEnter += ellipse_DragEnter;
          
            AssociatedObject.Drop += AssociatedObject_Drop;

            InitWindow();

        }
        private async void Dialog(string title , string messege)
        { Grid grid = AssociatedObject as Grid;
            grid.Visibility = Visibility.Hidden;
           
            var metroWindow = System.Windows.Application.Current.MainWindow as MetroWindow;
            var VM = metroWindow.DataContext as MainViewModel;
            await VM.ShowMahapsDialog(title, messege);
            
            grid.Visibility = Visibility.Visible;
        }

        private async Task EnsureSignalRAsync()
        {
            try
            {
                _connection ??= new HubConnectionBuilder()
                    .WithUrl("http://localhost:5178/hubs/points")
                    .WithAutomaticReconnect()
                    .Build();
                if (_connection.State != HubConnectionState.Connected)
                    await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR connect error: {ex.Message}");
            }
        }
        private void InitWindow()
        {


            Grid Rite = AssociatedObject as Grid;


            window.Background = System.Windows.Media.Brushes.Transparent;
            window.WindowStyle = WindowStyle.None;
            window.AllowsTransparency = true;
            window.ShowInTaskbar = false;


            Button buttonOverlay = new Button
            {
                Content = "X",
                Width = 30,
                Height = 30,

                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,

            };
            Border border = new Border
            {

                Width = window.Width,
                Height = window.Height,


            };

            buttonOverlay.Click += ButtonDeleteChildren;

            Grid gridOverlay = new Grid()
            {
                Visibility = Visibility.Collapsed,
                Background = System.Windows.Media.Brushes.Black,
                Opacity = 0.5,
                Width = buttonOverlay.Width,
                Height = buttonOverlay.Height,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
            };
            Grid gridOll = new Grid()
            {
                Background = System.Windows.Media.Brushes.Transparent,

                Width = window.Width,
                Height = window.Height,
            };
            gridOverlay.Children.Add(buttonOverlay);
            gridOll.Children.Add(gridOverlay);
            border.Child = gridOll;

            window.Content = border;

            window.Width = Rite.ActualWidth / Rite.ColumnDefinitions.Count;
            if (Rite.RowDefinitions.Count != 0) window.Height = Rite.ActualHeight / Rite.RowDefinitions.Count;
            else window.Height = Rite.ActualHeight;

            window.Visibility = Visibility.Visible;
            

            gridOverlay.MouseMove += WindowShow;

            gridOverlay.MouseLeave += Leave;

        }

        private int GetRowGrid(Point point)
        {
            var Rite = AssociatedObject as Grid;
            int countRow = Rite.RowDefinitions.Count;
            double heightCell = Rite.ActualHeight / (countRow == 0 ? 1 : countRow);
            int row = (int)(point.Y / heightCell);
            return row;
        }
        private int GetColumnGrid(Point point)
        {
            var Rite = AssociatedObject as Grid;
            int countCollumn = Rite.ColumnDefinitions.Count;
            double widthCell = Rite.ActualWidth / (countCollumn == 0 ? 1 : countCollumn);
            int colum = (int)(point.X / widthCell);
            return colum;

        }

        private void ellipse_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            var Rite = AssociatedObject as Grid;
            System.Windows.Point point = e.GetPosition(Rite);
            int row = GetRowGrid(point);
            int colum = GetColumnGrid(point);
            var cellChil2 = Rite.Children.OfType<Grid>().Where(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == colum).FirstOrDefault();

            if (cellChil2 != null)
            {
                cameraDragDrop.GridChange = cellChil2;

            }

        }


        private void MouseUps(object s, System.Windows.Forms.MouseEventArgs args)
        {
            if (args.Button != System.Windows.Forms.MouseButtons.Left) return;
            if (cameraDragDrop.GridTake != null & cameraDragDrop.GridChange != null)
                DragDrop.DoDragDrop(cameraDragDrop.GridTake, cameraDragDrop, System.Windows.DragDropEffects.Move);

        }

        private void Child_MouseDown(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
            var Rite = AssociatedObject as Grid;
            var panel = sender as BetterPanelTest;
            var grid = Rite.Children.OfType<Grid>().Where(c => c.Name == panel.Name).FirstOrDefault();

            if (grid != null)
            {
                cameraDragDrop.GridTake = grid;

            }
        } 
       
        private void AssociatedObject_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CameraDragDrop)))
            {
                var Rite = AssociatedObject as Grid;
                Point point = e.GetPosition(Rite);
                int row = GetRowGrid(point);
                int colum = GetColumnGrid(point);

                var cellChil = Rite.Children.OfType<Grid>().Where(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == colum).FirstOrDefault();
                if (cellChil == null)
                {
                    Grid.SetRow(cameraDragDrop.GridTake, row);
                    Grid.SetColumn(cameraDragDrop.GridTake, colum);
                }
                else
                {
                    Swich();
                }

            }
            else if (e.Data.GetDataPresent(typeof(WindowsFormsHost)))
            {

                WindowsFormsHost VideoHostSelect = e.Data.GetData(typeof(WindowsFormsHost)) as WindowsFormsHost;
                Grid grid = new Grid();
                BetterPanelTest panel = new BetterPanelTest
                {
                    DataContext = VideoHostSelect.Child.DataContext,
                    Name = "Test" + Guid.NewGuid().ToString("N"),
                     AutoSize = false,
                    //Anchor = AnchorStyles.Bottom,
                    Dock = DockStyle.Fill,
                    Tag = VideoHostSelect.Tag,
                      //BackColor = System.Drawing.Color.Green
                };
                panel.MouseDown += Child_MouseDown;
                panel.MouseUp += MouseUps;
                panel.MouseClick += MouseRiteClick;
                // panel.MouseDoubleClick += MouseDoubleClick;

                WindowsFormsHost VideoHost = new WindowsFormsHost
                {
                    DataContext = VideoHostSelect.DataContext,
                    Child = panel,
                    //Tag = VideoHostSelect.Tag,
                    Margin = new Thickness(5),
                    Name = VideoHostSelect.Name,

                    //Background = System.Windows.Media.Brushes.Green
                };

                 
                var Rite = AssociatedObject as Grid;

                UIElement uIElement = new UIElement();
                UIElement uIElement2 = new UIElement();

                uIElement = VideoHost;

                grid.Name = VideoHost.Child.Name; 

                // grid.Children.Add(uIElement2);

                //------------

                VideoHost.SizeChanged += VideoHost_SizeChanged;
                grid.Margin = new Thickness(5);
                grid.SizeChanged += Grid_SizeChanged;
                //---------

                try
                {

                    grid.Children.Add(uIElement);
                    
                    //grid.Children.Add(uIElement);

                }
                catch (Exception ex)
                {
                    string exs = ex.Message;



                }
                
                int colum = GetColumnGrid(e.GetPosition(Rite));
                int row = GetRowGrid(e.GetPosition(Rite));

                 List<string> arguments = new List<string>() { ((CameraConnectStrings)panel.Tag).subStream, ((CameraConnectStrings)panel.Tag).mainStream, VideoHost.Name, VideoHost.Child.Handle.ToString()/*, Process.GetCurrentProcess().Id.ToString()*/ };
                var process = StartProcess(arguments,(panel.Tag as CameraConnectStrings).CameraID);

                 grid.Tag = process.Id.ToString();
                VideoHost.Tag = VideoHost.Child.Handle.ToString();// window; меняю это 

                window.Owner = System.Windows.Application.Current.MainWindow;
                window.Show();
                panel.MouseLeave += Leave;
                panel.MouseMove += WindowShow;
             

                if (Rite.ColumnDefinitions.Count <= 1) // заполнение первых двух ячеек 
                {
                    Grid.SetColumn(grid, Rite.ColumnDefinitions.Count);
                    Grid.SetColumn(grid, Rite.ColumnDefinitions.Count);

                    Rite.ColumnDefinitions.Add(new ColumnDefinition());

                    Rite.Children.Add(grid);


                }
                else if (Rite.RowDefinitions.Count == 0) // вставляется третья фотка 
                {
                    Rite.RowDefinitions.Add(new RowDefinition());
                    Grid.SetRow(grid, Rite.RowDefinitions.Count);
                    Grid.SetRow(grid, Rite.RowDefinitions.Count);

                    Rite.RowDefinitions.Add(new RowDefinition());

                    Rite.Children.Add(grid);


                }
                else
                {
                    var cellChil = Rite.Children.OfType<Grid>().Where(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == colum).FirstOrDefault();
                    if (cellChil == null)
                    {
                        Grid.SetRow(grid, row);
                        Grid.SetColumn(grid, colum);
                        Rite.Children.Add(grid);
                    }
                    else
                    {
                        if (CheckEmptyChildInGrid(Rite))// заполнение ячеек по порядку  если ячейка правая нижняя пустая
                        {
                            AddGrid(Rite, grid);
                        }
                        else // тут проверка на заполненность последней ячеки правой нижней
                        {
                            Rite.RowDefinitions.Add(new RowDefinition());
                            Rite.ColumnDefinitions.Add(new ColumnDefinition());
                            Grid.SetRow(grid, Rite.RowDefinitions.Count - 1);
                            Rite.Children.Add(grid);
                        }
                    }


                }
              


            }
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Grid grid = sender as Grid;
            WindowsFormsHost wfh = grid.Children.OfType<WindowsFormsHost>().FirstOrDefault();
            if (wfh != null)
            {
                //SetSize setSize = new SetSize((int)e.NewSize.Width, (int)e.NewSize.Height, int.Parse(grid.Tag.ToString()));
                //wfh.Child.Size = new System.Drawing.Size((int)e.NewSize.Width, (int)e.NewSize.Height);
                wfh.Width = e.NewSize.Width;
                wfh.Height = e.NewSize.Height;
                //wfh.Child.Width = (int)grid.ActualWidth;
                //wfh.Child.Height = (int)grid.ActualWidth;
                //SendSetSize(setSize);
            }
        }

        private void VideoHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           WindowsFormsHost host = sender as WindowsFormsHost;
            if (host != null)
            {
                Grid grid = host.Parent as Grid;

                SetSize setSize = new SetSize((int)e.NewSize.Width, (int)e.NewSize.Height, int.Parse(grid.Tag.ToString()));
                //SendSetSize((int)e.NewSize.Width, (int)e.NewSize.Height,int.Parse(grid.Tag.ToString()));
                SendSetSize(setSize);
                //host.Child.Width = (int)e.NewSize.Width;
                //host.Child.Height = (int)e.NewSize.Height;
               // host.Margin = new Thickness(5);
            }
        }

        private void Leave(object? sender, EventArgs e)
        {
            if (!flagForOverlay) return;
            Border border = (Border)window.Content;
            Grid grids = (Grid)border.Child;
            Grid grid = grids.Children.OfType<Grid>().FirstOrDefault();
            grid.Visibility = Visibility.Hidden;
        }

        private void WindowShow(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            var panel = sender as BetterPanelTest;
            Border border = (Border)window.Content;
            Grid grids = (Grid)border.Child;
            Grid grid = grids.Children.OfType<Grid>().FirstOrDefault();
            Button b = grid.Children.OfType<Button>().FirstOrDefault();
            b.Name = panel.Name;

            grid.Visibility = Visibility.Visible;
            window.Height = panel.Height;
            window.Width = panel.Width;
            window.Left = panel.PointToScreen(new System.Drawing.Point()).X;
            window.Top = panel.PointToScreen(new System.Drawing.Point()).Y;
        }


        private void Leave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            flagForOverlay = true;
            Grid grd = sender as Grid;
            grd.Visibility = Visibility.Hidden;


        }

        private void WindowShow(object s, System.Windows.Input.MouseEventArgs ev)
        {
            flagForOverlay = false;
            Grid grid = s as Grid;

            grid.Visibility = Visibility.Visible;
        }


        static string GetSolutionParentPath()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                if (dir.GetFiles("*.sln").Any())
                    return dir.Parent!.FullName;

                dir = dir.Parent;
            }
            throw new InvalidOperationException("Solution folder not found");
        }
        private void MouseRiteClick(object? sender, System.Windows.Forms.MouseEventArgs e)
          
        { 
            if ( e.Button != System.Windows.Forms.MouseButtons.Right ) return;
            var panel = sender as BetterPanelTest;
            var riteGrid = AssociatedObject as Grid;
            window.Width = riteGrid.ActualWidth;
            window.Height = riteGrid.ActualHeight;

            Grid gridOverlayCanvals = ((Border)window.Content).Child as Grid;

            Grid gridOverlay = gridOverlayCanvals.Children.OfType<Grid>().FirstOrDefault();
            gridOverlay.Visibility = Visibility.Hidden;

            var grid = riteGrid.Children.OfType<Grid>().Where(c => c.Name == panel.Name).FirstOrDefault();
           
            if (grid != null)
            {
                var processTag = grid.Tag as string;

                //riteGrid.Visibility = Visibility.Hidden;
                
                var full = riteGrid.Parent as Grid;
                var wfh = grid.Children.OfType<WindowsFormsHost>().FirstOrDefault();
                if (wfh != null)
                {
                    AssociatedObject.Drop -= AssociatedObject_Drop;
                    //panel.MouseUp -= MouseUps;//----------
                    grid.Children.Remove(wfh);
                     
                    wfh.Width = riteGrid.ActualWidth;
                    wfh.Height = riteGrid.ActualHeight;
                    wfh.Child.MouseLeave -= Leave;
                    wfh.Child.MouseMove -= WindowShow;
                   
                    window.Left = riteGrid.PointToScreen(new Point()).X;
                    window.Top = riteGrid.PointToScreen(new Point()).Y;
                    if (riteGrid.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault() == null)
                    {
                        riteGrid.Children.Add(new Grid()
                        {
                            Name = "FullScreenGrid",
                            Width = riteGrid.ActualWidth,
                            Height = riteGrid.ActualHeight,
                            Children = { wfh },//newWind

                            //Margin = new Thickness((full.ColumnDefinitions.First().Width).Value + 5, 0, 0, 0),
                            Tag = processTag
                        });
                        SetConnect setConnect = new SetConnect(false, int.Parse(processTag));
                        SendChandeConekting(setConnect);
                        wfh.Child.MouseUp += MouseUpTakePixel;
                        wfh.Child.MouseDown += MouseDownTakePxel;
                        pixelPanelForZoom.TopLeft.X = 0;

                        riteGrid.SizeChanged += Full_SizeChanged;
                    }

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
                window.Width = riteGrid.ActualWidth;
                window.Height = riteGrid.ActualHeight;
                try
                {
                    window.Left = host.PointToScreen(new Point()).X;
                    window.Top = host.PointToScreen(new Point()).Y;
                }
                catch
                { }
            }
        }

        private void CloseZoomPanel(Grid gridOverlay, Grid Rite, Grid full, Grid gridOverlayCanvals,Grid parent, string processTag)//тут тоже поправить так что бы wfh менял своего перента и размеры 
        {

            Grid gridFullScreen = Rite.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();

            if (gridOverlayCanvals.Children.OfType<Canvas>().FirstOrDefault() != null)
            {
               
                if (gridFullScreen.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault()!=null/*gridFullScreen.Children.Count > 1*/)
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
            wfh.Child.MouseUp -= MouseUpTakePixel;
            wfh.Child.MouseDown -= MouseDownTakePxel;
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

        private Process StartProcess(List<string> argument,Guid IDCamera)
        {
            Process process = new();
            ProcessStartInfo start = new ProcessStartInfo(patch);

            if (ListDictionarySettingsCamers.DictionarySettingsCamers.TryGetValue(IDCamera, out var t))
            {
                var setttings = ListDictionarySettingsCamers.DictionarySettingsCamers[IDCamera];
                start.ArgumentList.Add(setttings.Brightness.ToString());
                start.ArgumentList.Add(setttings.Contrast.ToString());
                start.ArgumentList.Add(setttings.Hue.ToString());
                start.ArgumentList.Add(setttings.Saturation.ToString());
                start.ArgumentList.Add(setttings.NoiseReduction.ToString());
                start.ArgumentList.Add(setttings.EdgeEnhancement.ToString());
                start.ArgumentList.Add(setttings.AnamorphicScaling.ToString());
                start.ArgumentList.Add(setttings.StereoAdjustment.ToString());
                start.ArgumentList.Add(setttings.Rotation);
            }
            else
            {
                start.ArgumentList.Add("0");
                start.ArgumentList.Add("0");
                start.ArgumentList.Add("0");
                start.ArgumentList.Add("0");
                start.ArgumentList.Add("0");
                start.ArgumentList.Add("0");
                start.ArgumentList.Add("0");
                start.ArgumentList.Add("0");
                start.ArgumentList.Add("");
            }
                start.ArgumentList.Add(Process.GetCurrentProcess().Id.ToString());
            for (int i = 0; i < argument.Count; i++)
            {
                start.ArgumentList.Add(argument[i]);

            }

            //start.CreateNoWindow = true;
            
            process.StartInfo = start;
            
            process.Start();

            return process;
        }
        private void MouseUpTakePixel(object? sender, System.Windows.Forms.MouseEventArgs e)
        {


            pixelPanelForZoom.BottomRight = new Point(e.X, e.Y);

            if (pixelPanelForZoom.TopLeft != pixelPanelForZoom.BottomRight & pixelPanelForZoom.TopLeft != null&pixelPanelForZoom.TopLeft.X!=0)
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
               if( CreateWFHForZoom(sender))


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
            if (gridFullScreen.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault()!=null/*gridFullScreen.Children.Count > 1*/)//0000000000000000000000000000000000000000000000000000000
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
            int  h = ZoomHost.Child.Height;
            PointsForZoom pointsForZoom = new PointsForZoom(pixelPanelForZoom.TopLeft.X, pixelPanelForZoom.TopLeft.Y, pixelPanelForZoom.BottomRight.X, pixelPanelForZoom.BottomRight.Y);
            //{
            //    xTL = pixelPanelForZoom.TopLeft.X,
            //    yTL = pixelPanelForZoom.TopLeft.Y,
            //    xBR = pixelPanelForZoom.BottomRight.X,
            //    yBR = pixelPanelForZoom.BottomRight.Y
            //};
           // proc.ToString() ,
            Rectangle_MouseMove_SendPoint(proc.ToString(),pointsForZoom);//(pixelPanelForZoom.TopLeft.X, pixelPanelForZoom.TopLeft.Y, pixelPanelForZoom.BottomRight.X, pixelPanelForZoom.BottomRight.Y);
            OpenZoom openZoom = new OpenZoom(true, wind, w,h, int.Parse(proc));
            
            SendWindowForZoom(openZoom);

            ZoomHost.Tag = proc;//process.Id.ToString();
            return true;

        }
        
        private void CreateCanvalInOverlay()
        {
            var riteGrid = AssociatedObject as Grid;
            //var full = riteGrid.Parent as Grid;
            Grid gridFullScreen = riteGrid.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();

            Grid gridOverlay = ((Border)window.Content).Child as Grid;
            var child = gridOverlay.Children.OfType<Canvas>().FirstOrDefault();
           
            int buffer = 2;
            if (child != null)
            {  
                buffer = 1;
                gridOverlay.Children.Remove(child);
            }
            Canvas canvas = new Canvas()
            {
                Width = window.Width,
                Height = window.Height, 
                Background = System.Windows.Media.Brushes.Transparent,
                Tag = gridFullScreen.Tag

            };
            canvas.Children.Add(new System.Windows.Shapes.Rectangle
            {
                Width = (pixelPanelForZoom.BottomRight.X - pixelPanelForZoom.TopLeft.X),
                Height = pixelPanelForZoom.BottomRight.Y - pixelPanelForZoom.TopLeft.Y,
                Stroke = System.Windows.Media.Brushes.Red,
                StrokeThickness = 2,
                Fill = System.Windows.Media.Brushes.Red,
                Opacity = 0.3

            });
            gridOverlay.Children.Add(canvas);
            System.Windows.Shapes.Rectangle rectangle = gridOverlay.Children.OfType<Canvas>().FirstOrDefault().Children.OfType<System.Windows.Shapes.Rectangle>().FirstOrDefault();//.PointFromScreen(new Point(pixelPanelForZoom.TopLeft.X, pixelPanelForZoom.TopLeft.Y)) ;
            rectangle.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            rectangle.VerticalAlignment = VerticalAlignment.Top;
            Canvas.SetLeft(rectangle, (pixelPanelForZoom.TopLeft.X  /*/ buffer*/) - 10);
            Canvas.SetTop(rectangle, pixelPanelForZoom.TopLeft.Y);

            canvas.MouseMove += MouseMoveCanvals;
            rectangle.MouseDown += RectangleMouseDown;
            rectangle.MouseUp += RectangleMouseUp;
             
        }



        private void MouseDownTakePxel(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!flagChengePosition)
                pixelPanelForZoom.TopLeft = new Point(e.X, e.Y);
            else pixelPanelForZoom.TopLeft = new Point(0,0);
        }


        private void Swich()
        {
            var rowSet = Grid.GetRow(cameraDragDrop.GridChange);
            var columSet = Grid.GetColumn(cameraDragDrop.GridChange);
            var flipS = Grid.GetRow(cameraDragDrop.GridTake);
            var flipC = Grid.GetColumn(cameraDragDrop.GridTake);



            Grid.SetRow(cameraDragDrop.GridTake, rowSet);
            Grid.SetColumn(cameraDragDrop.GridTake, columSet);

            Grid.SetRow(cameraDragDrop.GridChange, flipS);
            Grid.SetColumn(cameraDragDrop.GridChange, flipC);

        }
        private void AddGrid(Grid mainGrid, Grid children)
        {

            for (int i = 0; i < mainGrid.ColumnDefinitions.Count; i++)
            {


                for (int j = 0; j < mainGrid.RowDefinitions.Count; j++)
                {
                    var cellR = mainGrid.Children.OfType<Grid>().Where(c => Grid.GetRow(c) == j && Grid.GetColumn(c) == i).FirstOrDefault(); // заполняется последняя строчка слева напрво 
                    if (cellR == null)
                    {
                        Grid.SetColumn(children, i);
                        Grid.SetRow(children, j);


                        mainGrid.Children.Add(children);

                        return;
                    }
                }
            }
        }
        private bool CheckEmptyChildInGrid(Grid mainGrid)
        {
            bool result = false;

            for (int i = 0; i < mainGrid.ColumnDefinitions.Count; i++)
            {
                for (int j = 0; j < mainGrid.RowDefinitions.Count; j++)
                {
                    var cellR = mainGrid.Children.OfType<Grid>().Where(c => Grid.GetRow(c) == j && Grid.GetColumn(c) == i); // заполняется последняя строчка слева напрво 
                    if (cellR.FirstOrDefault() == null)
                    {
                        result = true;
                        break;
                    }

                }
            }
            return result;
        }

        private void ButtonDeleteChildren(object sender, RoutedEventArgs e)
        {
            var Rite = AssociatedObject as Grid;

            Button button = sender as Button;
            var griddelet = Rite.Children.OfType<Grid>().Where(s => s.Name == button.Name).FirstOrDefault();
            
            var indexC = Grid.GetColumn(griddelet);
            var IndexR = Grid.GetRow(griddelet);
            var cellContent = Rite.Children.OfType<UIElement>().FirstOrDefault(c => Grid.GetRow(c) == IndexR && Grid.GetColumn(c) == indexC);
            Grid grid = cellContent as Grid;
            if (grid != null)
            {
                var tag = grid.Tag as string;
                if (!string.IsNullOrEmpty(tag))
                {
                    try
                    {
                        Process.GetProcessById(int.Parse(tag)).Kill();
                    }
                    catch { }
                        Border border = (Border)window.Content;
                    Grid grids = (Grid)border.Child;
                    Grid gridOverlay = grids.Children.OfType<Grid>().FirstOrDefault();

                    gridOverlay.Visibility = Visibility.Hidden;


                }
            }

            grid.Children.Clear();
            Rite.Children.Remove(cellContent);
            List<int> list = new List<int>();
            for (int i = 0; i < Rite.ColumnDefinitions.Count; i++)
            {
                if (Rite.Children.OfType<UIElement>().FirstOrDefault(c => Grid.GetRow(c) == Rite.RowDefinitions.Count - 1 && Grid.GetColumn(c) == i) != null)
                {
                    list.Add(i);
                }


            }
            for (int i = 0; i < Rite.RowDefinitions.Count; i++)
            {
                if ((Rite.Children.OfType<UIElement>().FirstOrDefault(c => Grid.GetRow(c) == i && Grid.GetColumn(c) == Rite.ColumnDefinitions.Count - 1)) != null)
                {
                    list.Add(i);
                }
            }

            if (list.Count == 0)
            {
                if (Rite.RowDefinitions.Count != 0)
                {
                    Rite.RowDefinitions.RemoveAt(Rite.RowDefinitions.Count - 1);
                    Rite.ColumnDefinitions.RemoveAt(Rite.ColumnDefinitions.Count - 1);
                }
            }
            if (Rite.RowDefinitions.Count == 1 | Rite.RowDefinitions.Count == 0)
            {
                if ((Rite.Children.OfType<UIElement>().FirstOrDefault(c => Grid.GetRow(c) == 0 && Grid.GetColumn(c) == Rite.ColumnDefinitions.Count - 1)) == null)
                {
                    try
                    {
                        Rite.ColumnDefinitions.RemoveAt(Rite.ColumnDefinitions.Count - 1);
                    }
                    catch
                    {
                        return;
                    }
                }

            }


        }

        private async void Rectangle_MouseMove_SendPoint(string groupId, PointsForZoom pointsForZoom) 
        {
            if (_connection == null) return;
            try
            {
                await _connection.InvokeAsync("SendPointToGroup", groupId, pointsForZoom); //SendPointToGroup
            }
            catch (Exception ex)
            {
                Debug.WriteLine($" error: {ex.Message}");
            }
        }

        private async void SendWindowForZoom(OpenZoom openZoom)// bool usZoom, int window, int w, int h, int ID
        {
            if (_connection == null) return;
            try
            {
                await _connection.InvokeAsync("SendZoomToGroup", openZoom); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine($" error: {ex.Message}");
            }
        }
        private async void SendChandeConekting(SetConnect setConnect)//bool Use, int ID
        {
            if (_connection == null) return;
            try
            {
                await _connection.InvokeAsync("SendSetConToGroup", setConnect);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($" error: {ex.Message}");
            }
        }
        private async void SendSetSize(SetSize setSize)
        {
            if (_connection == null) return;
            try
            {

                await _connection.InvokeAsync("SenNewSize", setSize);
                     }
            catch (Exception ex)
            {
                Debug.WriteLine($" error: {ex.Message}");
            }
        }
        

        private Point _startPoint;
        bool flagChengePosition = false;
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
                Grid parent = VisualTreeHelper.GetParent(canvas)as Grid;
               
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

                Rectangle_MouseMove_SendPoint(canvas.Tag.ToString(),pointsForZoom);
            }
        }
    }
}