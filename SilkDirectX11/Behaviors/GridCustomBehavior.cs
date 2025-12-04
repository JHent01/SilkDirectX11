using MahApps.Metro.Controls;
using Microsoft.Xaml.Behaviors;
using RenderANDVideoReaderVIdeoDecoder;
using SilkDirectX11.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using SilkDirectX11.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using MahApps.Metro.Controls.Dialogs;
using SilkDirectX11.ViewModels;

namespace SilkDirectX11.Behaviors
{
    class GridCustomBehavior : Behavior<Grid>// доделать норм интерфейс ползунки по повороту зума и тд
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
            //AssociatedObject.AllowDrop = true;
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
            // window.Show();

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
            if (cameraDragDrop.GridTake != null & cameraDragDrop.GridChange != null)
                DragDrop.DoDragDrop(cameraDragDrop.GridTake, cameraDragDrop, System.Windows.DragDropEffects.Move);

        }

        private void Child_MouseDown(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            var Rite = AssociatedObject as Grid;
            var panel = sender as Panel;
            var grid = Rite.Children.OfType<Grid>().Where(c => c.Name == panel.Name).FirstOrDefault();

            if (grid != null)
            {
                cameraDragDrop.GridTake = grid;

            }
        }
        //private void Child_MouseDown(object? sender, EventArgs e)
        //{
        //    var Rite = AssociatedObject as Grid;
        //    var panel = sender as Panel;
        //    var grid = Rite.Children.OfType<Grid>().Where(c => c.Name == panel.Name).FirstOrDefault();
        //
        //    if (grid != null)
        //    {
        //        imageDragDrop.GridTake = grid;
        //
        //    }
        //}
       
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
                //WindowsFormsHost VideoHost = VideoHostSelect;
                //VideoHost.Margin = new Thickness(5);

                //VideoHost.Child.Name = "Test" + Guid.NewGuid().ToString("N");
                //VideoHost.Child.MouseUp += MouseUps;
                //VideoHost.Child.MouseDown += Child_MouseDown;
                Panel paneltest = new Panel
                {
                    DataContext = VideoHostSelect.Child.DataContext,
                    Name = "Test" + Guid.NewGuid().ToString("N"),
                    AutoSize = true,
                    Tag = VideoHostSelect.Tag

                };
                paneltest.MouseDown += Child_MouseDown;
                paneltest.MouseUp += MouseUps;
                paneltest.MouseDoubleClick += MouseDoubleClick;

              //  paneltest.MouseMove += Panel_MouseMove_SendPoint; --------------------

                WindowsFormsHost VideoHost = new WindowsFormsHost
                {
                    DataContext = VideoHostSelect.DataContext,
                    Child = paneltest,
                    //Tag = VideoHostSelect.Tag,
                    Margin = new Thickness(5),
                    Name = VideoHostSelect.Name,

                };

                //Button button = new Button
                //{ 
                //    Content = "X",
                //    Width = 20,
                //    Height = 20,

                //    HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                //    VerticalAlignment = VerticalAlignment.Top,

                //    Opacity = 0.5,
                //};
                var Rite = AssociatedObject as Grid;

                UIElement uIElement = new UIElement();
                UIElement uIElement2 = new UIElement();

                uIElement = VideoHost;

                grid.Name = VideoHost.Child.Name;//panel.Name;

                grid.Children.Add(uIElement2);


               
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

                 List<string> arguments = new List<string>() { ((CameraConnectStrings)paneltest.Tag).subStream, VideoHost.Name, VideoHost.Child.Handle.ToString()/*, Process.GetCurrentProcess().Id.ToString()*/ };
                var process = StartProcess(arguments,(paneltest.Tag as CameraConnectStrings).CameraID);

                grid.Tag = process.Id.ToString();
                VideoHost.Tag = window;

                window.Owner = System.Windows.Application.Current.MainWindow;
                window.Show();
                paneltest.MouseLeave += Leave;
                paneltest.MouseMove += WindowShow;
                Task.Delay(500).Wait();  
                                        
                if (process.HasExited)
                {
                    
                    
                     Dialog("ERROR","Ошибка подключения к камере");
                    
                    // dialog.ShowDialogExternally();// .ShowMetroDialogAsync(dialog);

                    // System.Windows.MessageBox.Show("Process Stoped with ID: " + process.Id );

                    return;
                }

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
                //CameraConnectStrings tag = (CameraConnectStrings)paneltest.Tag;

                //string nameCamera = VideoHost.Name;
                //  nint RenderTargetHwnd = VideoHost.Child.Handle;
                



                //  System.Windows.Window window = new () ;
                //  //window.Topmost = true;
                //  window.Background = System.Windows.Media.Brushes.Transparent;
                //   window.WindowStyle = WindowStyle.None;
                //  window.AllowsTransparency = true;
                //   window.ShowInTaskbar = false;
                //   Button button1 = new Button
                //   {
                //       Content = "X",
                //       Width = 30,
                //       Height = 30,
                //
                //       HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                //       VerticalAlignment = VerticalAlignment.Top,
                //       Name = paneltest.Name,
                //      // Opacity = 0.5,
                //     //  IsHitTestVisible = true,
                //   };
                //  Border border = new Border
                //  {
                //     // Background = System.Windows.Media.Brushes.Transparent,
                //      Width = window.Width,
                //      Height = window.Height,
                //     //  BorderBrush = System.Windows.Media.Brushes.Transparent,
                //     //  BorderThickness = new Thickness(10),
                //      // Opacity = 0.01,
                //  //    IsHitTestVisible = false,
                //
                //  };
                //  //   window.Opacity = 0.01;
                //  button1.Click += ButtonDeleteChildren;
                //  //UIElement uI = new UIElement();
                // // uI = button1;
                //
                //  Grid grid2 = new Grid() 
                //  {
                //      Visibility = Visibility.Collapsed,
                //      Background = System.Windows.Media.Brushes.Black,
                //      Opacity=0.5,
                //      Width = button1.Width,
                //      Height = button1.Height,
                //      HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                //      VerticalAlignment = VerticalAlignment.Top,
                //  };
                //  grid2.Children.Add(button1);
                //  //  grid1.Children.Add(border);
                //  // grid1.Children.Add(button1);
                //  //   grid1.Children.Add(border);
                //  border.Child = grid2;
                //  //  grid1.Children.Add(button1);
                //  // border.Child = uI;
                //  //  window.Content = button1;
                //  window.Content = border;
                //
                //
                //  window.Owner = System.Windows.Application.Current.MainWindow;
                //
                //  window.Width = Rite.ActualWidth/Rite.ColumnDefinitions.Count;
                //if     (Rite.RowDefinitions.Count!=0)  window.Height = Rite.ActualHeight/ Rite.RowDefinitions.Count;
                //else   window.Height = Rite.ActualHeight;
                //
                //
                //  window.Visibility = Visibility.Visible;
                //
                //  //paneltest.MouseEnter += (s, ev) =>
                //  //{
                //  //    WindowShow(s, ev, window);
                //  //};
                //
                //
                //  //paneltest.MouseMove += (s, ev) =>
                //  //{
                //  //    WindowShow(s, ev, window);
                //  //};
                //  //paneltest.MouseLeave += (s, ev) =>
                //  //{
                //  //    Leave(  window);
                //  //};
                //
                //  grid2.MouseMove += WindowShow;
                //
                //  grid2.MouseLeave += Leave;
                //paneltest.MouseLeave += (s, ev) =>
                //{
                //    WindowHide(s, ev, window);
                //};
                //  grid.SizeChanged += SizeWindowChange;
                //
                //
                ////   start.Arguments = "";
                //   Process.Start(start);
                //  Program tests = new Program();
                //
                // Task.Factory.StartNew(() => tests.Start(tag.subStream, nameCamera, RenderTargetHwnd));


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
            var panel = sender as Panel;
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
        private void MouseDoubleClick(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            var panel = sender as Panel;
            var riteGrid = AssociatedObject as Grid;
            window.Width = riteGrid.ActualWidth;
            window.Height = riteGrid.ActualHeight;

            Grid gridOverlayCanvals = ((Border)window.Content).Child as Grid;

            Grid gridOverlay = gridOverlayCanvals.Children.OfType<Grid>().FirstOrDefault();
            gridOverlay.Visibility = Visibility.Hidden;

            var grid = riteGrid.Children.OfType<Grid>().Where(c => c.Name == panel.Name).FirstOrDefault();
            if (grid != null)
            {
                riteGrid.Visibility = Visibility.Hidden;
                var wfh = grid.Children.OfType<WindowsFormsHost>().FirstOrDefault();

                //Process process = new();//------------
                var full = riteGrid.Parent as Grid;
                var newWind = new WindowsFormsHost
                {
                    Width = riteGrid.ActualWidth,
                    Height = riteGrid.ActualHeight,
                    Child = new Panel
                    {
                        DataContext = panel.DataContext,
                        Name = panel.Name,
                        //   AutoSize = true,
                        Tag = panel.Tag

                    },
                    DataContext = grid.Children.OfType<WindowsFormsHost>().FirstOrDefault().DataContext,
                    Name = grid.Children.OfType<WindowsFormsHost>().FirstOrDefault().Name,
                    // Tag = Process.GetProcessById(),
                };

                window.Left = riteGrid.PointToScreen(new Point()).X;
                window.Top = riteGrid.PointToScreen(new Point()).Y;
                newWind.Child.MouseDoubleClick += MouseDoubleClick;
                newWind.Child.MouseDown += MouseDownTakePxel;
                newWind.Child.MouseUp += MouseUpTakePixel;

                //((Panel)newWind.Child).MouseMove += Panel_MouseMove_SendPoint;-----------
                Process process = null;
                if (full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault() == null)
                {
                    full.Children.Add(new Grid()
                    //Grid nGrid = new Grid() 
                    {
                        Name = "FullScreenGrid",
                        Width = riteGrid.ActualWidth,
                        Height = riteGrid.ActualHeight,
                        Children = { newWind },
                        Margin = new Thickness((full.ColumnDefinitions.First().Width).Value + 5, 0, 0, 0),

                    });
                  //  var g =grid.Tag;

                    List<string> arguments = new List<string>() { ((CameraConnectStrings)panel.Tag).mainStream, wfh.Name, newWind.Child.Handle.ToString()/*, Process.GetCurrentProcess().Id.ToString() */};
                      process = StartProcess(arguments, ((CameraConnectStrings)panel.Tag).CameraID);
                    Task.Delay(500).Wait();
                    if (process.HasExited)
                    {
                        System.Windows.MessageBox.Show("Process Stoped with ID: " + process.Id);

                        gridOverlay.Visibility = Visibility.Visible;
                        riteGrid.Visibility = Visibility.Visible;
                        CloseZoomPanel(gridOverlay, riteGrid, full, gridOverlayCanvals);
                        return;
                    }

                    WindowsFormsHost host = full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault()?.Children.OfType<WindowsFormsHost>().FirstOrDefault();


                    full.SizeChanged += (s, ev) =>
                    {
                        var t = full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();
                        host.Width = wfh.ActualWidth;
                        if (t != null)
                        {
                            t.Width = wfh.ActualWidth;
                            t.Height = wfh.ActualHeight;
                        }
                        host.Height = wfh.ActualHeight;
                        Panel chil = (Panel)host.Child;
                        chil.Width = (int)wfh.ActualWidth;
                        chil.Height = (int)wfh.ActualHeight;
                        window.Width = wfh.ActualWidth;
                        window.Height = wfh.ActualHeight;
                        window.Left = wfh.PointToScreen(new Point()).X;
                        window.Top = wfh.PointToScreen(new Point()).Y;
                    };

                    full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault().Tag = process.Id.ToString();
                }
                else
                {
                    //if (process.HasExited)
                    //{
                    //    System.Windows.MessageBox.Show("Process Stoped with ID: " + process.Id);

                    //    gridOverlay.Visibility = Visibility.Visible;
                    //    riteGrid.Visibility = Visibility.Visible;
                    //    CloseZoomPanel(gridOverlay, riteGrid, full, gridOverlayCanvals);
                    //    return;
                    //}
                    CloseZoomPanel(gridOverlay, riteGrid, full, gridOverlayCanvals);
                }
                // Grid gridOverlayCanvals = ((Border)window.Content).Child as Grid;
                //// var child = gridOverlayCanvals.Children.OfType<Canvas>().FirstOrDefault();
                //
                // if (gridOverlayCanvals.Children.OfType<Canvas>().FirstOrDefault() != null)
                // {
                //     Grid gridFullScreen = full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();
                //     if (gridFullScreen.Children.Count > 1)
                //     {
                //         WindowsFormsHost zoom = gridFullScreen.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault();
                //         var tag = zoom.Tag as string;
                //         gridFullScreen.Children.Remove(zoom);
                //         //gridFullScreen.ColumnDefinitions.RemoveAt(gridFullScreen.ColumnDefinitions.Count - 1);
                //
                //         try
                //         {
                //             Process.GetProcessById(int.Parse(tag)).Kill();
                //         }
                //         catch (Exception ex)
                //         {
                //             string exs = ex.Message;
                //         }
                //     }
                //     var childOverlay = gridOverlayCanvals.Children.OfType<Canvas>().FirstOrDefault();
                //     gridOverlayCanvals.Children.Remove(childOverlay);
                // }
                //
                // Rite.Visibility = Visibility.Visible;
                // var t = full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault().Tag as string;
                //  Process.GetProcessById(int.Parse(t)).Kill();
                // var t2 = full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault().Children.OfType<WindowsFormsHost>().FirstOrDefault().Tag;
                // if (t2!=null)Process.GetProcessById(int.Parse(t2.ToString())).Kill();
                // full.Children.Remove(full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault());
                //
                //var child = gridOverlay.Children.OfType<Canvas>().FirstOrDefault();
                // if (child != null)
                // {
                //     gridOverlay.Children.Remove(child);
                // }
                //
                //var newWfh = new WindowsFormsHost
                //{Width= Rite.ActualWidth,
                //    Height= Rite.ActualHeight,
                //    Child = new Panel
                //    {
                //        DataContext = panel.DataContext,
                //        Name = panel.Name,
                //        AutoSize = true,
                //    },
                //    DataContext = grid.Children.OfType<WindowsFormsHost>().FirstOrDefault().DataContext,
                //    Name = grid.Children.OfType<WindowsFormsHost>().FirstOrDefault().Name,
                //    Tag = panel.Tag,
                //};
                //
                //
                //
                //Window window = new Window
                //{
                //    Title = "Video",
                //    Width = Rite.ActualWidth,
                //    Height = Rite.ActualHeight,
                //    //WindowStartupLocation = WindowStartupLocation.Manual,
                //    //   Left = Rite.ActualWidth,//.Bounds.Left,
                //    // Top = Rite.ActualHeight,//screen.Bounds.Top,
                //    //  WindowState = WindowState.Maximized,
                //    Content = new WindowsFormsHost
                //    {
                //        Child = new Panel
                //        {
                //            DataContext = panel.DataContext,
                //            Name = panel.Name,
                //            AutoSize = true,
                //        },
                //        DataContext = grid.Children.OfType<WindowsFormsHost>().FirstOrDefault().DataContext,
                //        Name = grid.Children.OfType<WindowsFormsHost>().FirstOrDefault().Name,
                //        Tag = panel.Tag,
                //    }
                //};
                //
                //  Program startVideo = new Program();
                //
                //if (panel.Tag == null)
                //{
                //    // startVideo.Stop();
                //    //        startVideo.Destroy();
                //    //   startVideo = null;
                //    GC.Collect();
                //    return;
                //}
                //
                //string _videoSourceTest = panel.Tag.ToString();
                //
                //  var wfh = window.Content as WindowsFormsHost;
                //  //string nameCamera = wfh.Name;
                //  var panelNew = wfh.Child as Panel;
                //  CameraConnectStrings tag = (CameraConnectStrings)panel.Tag;
                //  //        startVideo.Start(tag.mainStream, wfh.Name, panelNew.Handle);
                //  var mainFlow = Process.GetCurrentProcess();
                //  Process process = new();
                //  ProcessStartInfo start = new ProcessStartInfo("D:\\Work\\DirectX11\\Rend\\RenderANDVideoReaderVIdeoDecoder\\RenderANDVideoReaderVIdeoDecoder\\bin\\Debug\\net8.0\\RenderANDVideoReaderVIdeoDecoder.exe");
                //  start.ArgumentList.Add(tag.subStream);
                //  start.ArgumentList.Add(wfh.Name);
                //  start.ArgumentList.Add(panelNew.Handle.ToString());
                //  start.ArgumentList.Add(mainFlow.Id.ToString());
                //  process.StartInfo = start;
                //  process.Start();
                //  window.ShowDialog();
                //  process.CloseMainWindow();
                //  process.Dispose();
                //  process.Close();
                //
                //  //  startVideo.Stop();
                //  WindowsFormsHost g = window.Content as WindowsFormsHost;
                //  g.Child.Dispose();
                //// g.Child = null;
                //  g.Dispose();
                ////;
                //  //process.Start("D:\\Work\\DirectX11\\Rend\\RenderANDVideoReaderVIdeoDecoder\\RenderANDVideoReaderVIdeoDecoder\\bin\\Debug\\net8.0\\RenderANDVideoReaderVIdeoDecoder.exe");
                //  // Process.Start(start);
                //  // window.Content = null;
                //  //          startVideo.Destroy();
                //  //startVideo= null;
                //  //window = null;
                //  //wfh = null;
                //  //panelNew = null;
                //  //panel = null;
                //  window.Close();
                //process.Close();
                //process.Dispose();
                //colectGarbage();

            }
        }
        private void CloseZoomPanel(Grid gridOverlay, Grid Rite, Grid full, Grid gridOverlayCanvals)
        {
            // Grid gridOverlayCanvals = ((Border)window.Content).Child as Grid;


            if (gridOverlayCanvals.Children.OfType<Canvas>().FirstOrDefault() != null)
            {
                Grid gridFullScreen = full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();
                if (gridFullScreen.Children.Count > 1)
                {
                    WindowsFormsHost zoom = gridFullScreen.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault();
                    var tagZoom = zoom.Tag as string;
                    gridFullScreen.Children.Remove(zoom);


                    try
                    {
                        Process.GetProcessById(int.Parse(tagZoom)).Kill();
                    }
                    catch (Exception ex)
                    {//возможно потом логика вывода ошибок 
                        string exs = ex.Message;
                    }
                }
                var childOverlay = gridOverlayCanvals.Children.OfType<Canvas>().FirstOrDefault();
                gridOverlayCanvals.Children.Remove(childOverlay);
            }

            Rite.Visibility = Visibility.Visible;
            var tag = full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault().Tag as string;
            try
            {
                Process.GetProcessById(int.Parse(tag)).Kill();
            }
            catch 
            {
                 
            }
            var tagChild = full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault().Children.OfType<WindowsFormsHost>().FirstOrDefault().Tag;
            if (tagChild != null) Process.GetProcessById(int.Parse(tagChild.ToString())).Kill();

            full.Children.Remove(full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault());

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
            //start.CreateNoWindow = true;""2033296"" "29712"
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
                //if (pixelPanelForZoom.BottomRight.Y<pixelPanelForZoom.TopLeft.X||pixelPanelForZoom.BottomRight.X<pixelPanelForZoom.TopLeft.Y)
                //{
                //    if (pixelPanelForZoom.BottomRight.Y<pixelPanelForZoom.TopLeft.X)
                //    { var buffer = pixelPanelForZoom.TopLeft.X;
                //        pixelPanelForZoom.TopLeft.X = pixelPanelForZoom.BottomRight.Y;
                //        pixelPanelForZoom.BottomRight.Y = buffer;
                //    }
                //    if ( pixelPanelForZoom.BottomRight.X<pixelPanelForZoom.TopLeft.Y  )
                //    { var buffer = pixelPanelForZoom.TopLeft.Y;
                //        pixelPanelForZoom.TopLeft.Y = pixelPanelForZoom.BottomRight.X;
                //        pixelPanelForZoom.BottomRight.X = buffer;
                //    }
                //}


            }
        }

        private bool CreateWFHForZoom(object? sender)
        {
            var riteGrid = AssociatedObject as Grid;
            var full = riteGrid.Parent as Grid;



            Grid gridFullScreen = full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();


            WindowsFormsHost wfh = gridFullScreen.Children.OfType<WindowsFormsHost>().FirstOrDefault();
            wfh.Width = (riteGrid.ActualWidth / 2);
            wfh.Child.Width = (int)(riteGrid.ActualWidth / 2);
            wfh.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            wfh.Width = (riteGrid.ActualWidth / 2);
            Panel panel = sender as Panel;
            if (gridFullScreen.Children.Count > 1)
            {
                WindowsFormsHost zoom = gridFullScreen.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault();
                var t = zoom.Tag as string;
                gridFullScreen.Children.Remove(zoom);

                try
                {
                    Process.GetProcessById(int.Parse(t)).Kill();
                }
                catch (Exception ex)
                {
                    string exs = ex.Message;

                }
            }
            else
            { //panel.Width = (int)(riteGrid.ActualWidth / 2);
                pixelPanelForZoom.TopLeft.X = pixelPanelForZoom.TopLeft.X / 2;
                pixelPanelForZoom.BottomRight.X = pixelPanelForZoom.BottomRight.X / 2;
            }
            



            gridFullScreen.Children.Add(new WindowsFormsHost
            {
                Width = riteGrid.ActualWidth / 2,
                Height = riteGrid.ActualHeight,
                Child = new Panel
                {
                    DataContext = panel.DataContext,
                    // Name = "ZoomPanel" + Guid.NewGuid().ToString("N"),UpdateZoomConstantBuffer(topLeft, buttomRight);  _context.PSSetConstantBuffers(0, new ID3D11Buffer[] { _zoomCB });   _rtv = _device.CreateRenderTargetView(backBuffer);
                    //  AutoSize = true,
                    Width = (int)(riteGrid.ActualWidth / 2),
                    Height = (int)(riteGrid.ActualHeight),

                    Tag = panel.Tag
                },
                DataContext = panel.DataContext,
                Name = "ZoomHost",
                Margin = new Thickness(15, 0, 0, 0),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right
            });
            window.Width = riteGrid.ActualWidth / 2;
            window.Height = riteGrid.ActualHeight;
            // gridFullScreen.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault().Child.Width = (int)window.Width;
            List<string> arguments = new List<string>()
           { (
           (CameraConnectStrings)gridFullScreen.Children.OfType<WindowsFormsHost>().FirstOrDefault().Child.Tag).mainStream,
                "",
                gridFullScreen.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault().Child.Handle.ToString(),
              //  Process.GetCurrentProcess().Id.ToString(),
                pixelPanelForZoom.TopLeft.X.ToString(),
                pixelPanelForZoom.TopLeft.Y.ToString(),
                pixelPanelForZoom.BottomRight.X.ToString(),
                pixelPanelForZoom.BottomRight.Y.ToString(),
                gridFullScreen.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault().Child.Width.ToString(),
                gridFullScreen.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault().Child.Height.ToString()

           };
            var process = StartProcess(arguments, ((CameraConnectStrings)panel.Tag).CameraID);
            Task.Delay(500).Wait();
            if (process.HasExited)
            {
                System.Windows.MessageBox.Show("Process Stoped with ID: " + process.Id);
                Grid gridOverlay = ((Border)window.Content).Child as Grid;
                gridOverlay.Visibility = Visibility.Visible;
                riteGrid.Visibility = Visibility.Visible;
                 
                CloseZoomPanel(gridOverlay, riteGrid, full, gridOverlay);
                
                 
                 
                return false;
            }
          
            gridFullScreen.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault().Tag = process.Id.ToString();
            return true;

        }
        //private void CreateProcess(Grid gridFullScreen)
        //{
        //    Process process = new();
        //    var mainFlow = Process.GetCurrentProcess();
        //    ProcessStartInfo start = new ProcessStartInfo(patch);
        //    CameraConnectStrings tag = (CameraConnectStrings)gridFullScreen.Children.OfType<WindowsFormsHost>().FirstOrDefault().Child.Tag;
        //    start.ArgumentList.Add(tag.mainStream);
        //    start.ArgumentList.Add("");
        //    start.ArgumentList.Add(gridFullScreen.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault().Child.Handle.ToString());
        //    start.ArgumentList.Add(mainFlow.Id.ToString());
        //    start.ArgumentList.Add(pixelPanelForZoom.TopLeft.X.ToString());
        //    start.ArgumentList.Add(pixelPanelForZoom.TopLeft.Y.ToString());
        //    start.ArgumentList.Add(pixelPanelForZoom.BottomRight.X.ToString());
        //    start.ArgumentList.Add(pixelPanelForZoom.BottomRight.Y.ToString());
        //    start.ArgumentList.Add(gridFullScreen.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault().Child.Width.ToString());
        //    start.ArgumentList.Add(gridFullScreen.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault().Child.Height.ToString());
        //    start.CreateNoWindow = true;
        //
        //    //CreateCanvalInOverlay();
        //
        //
        //    process.StartInfo = start;
        //    process.Start();
        //
        //    gridFullScreen.Children.OfType<WindowsFormsHost>().Where(s => s.Name == "ZoomHost").FirstOrDefault().Tag = process.Id.ToString();
        //}
        private void CreateCanvalInOverlay()
        {
            Grid gridOverlay = ((Border)window.Content).Child as Grid;
            var child = gridOverlay.Children.OfType<Canvas>().FirstOrDefault();
            //  double W = ((pixelPanelForZoom.BottomRight.X - pixelPanelForZoom.TopLeft.X));
            int buffer = 2;
            if (child != null)
            {  // W=W*2;
                buffer = 1;
                gridOverlay.Children.Remove(child);
            }
            Canvas canvas = new Canvas()
            {
                Width = window.Width,
                Height = window.Height, //pixelPanelForZoom.BottomRight.Y - pixelPanelForZoom.TopLeft.Y,
                Background = System.Windows.Media.Brushes.Transparent,

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
            //.MouseMove += Panel_MouseMove_SendPoint;
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
            // var stackP = VisualTreeHelper.GetParent(button);
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

        private async void Rectangle_MouseMove_SendPoint(double xTL,double yTL,double xBR,double yBR)//object? sender, System.Windows.Forms.MouseEventArgs e
        {
            if (_connection == null) return;
            try
            {
                await _connection.InvokeAsync("SendPoint", xTL,yTL, xBR, yBR);/*(double)e.X, (double)e.Y, (sender as Panel)?.Name ?? "panel"*/
            }
            catch (Exception ex)
            {
                Debug.WriteLine($" error: {ex.Message}");
            }
        }
        //private async void Panel_MouseMove_SendPoint(object sender, System.Windows.Input.MouseEventArgs e)
        //{
        //    if (_connection == null) return;
        //    try
        //    {
        //        System.Windows.Shapes.Rectangle rectangle = sender as System.Windows.Shapes.Rectangle;
        //        Point point = e.GetPosition(rectangle);
        //        await _connection.InvokeAsync("SendPoint", (double)point.X, (double)point.Y, (sender as Panel)?.Name ?? "panel");
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($" error: {ex.Message}");
        //    }
        //}

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
                Rectangle_MouseMove_SendPoint(newLeft, newTop, newRight, newBottom);
            }
        }
    }
}