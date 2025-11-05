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

namespace SilkDirectX11.Behaviors
{
    class GridCustomBehavior : Behavior<Grid>
    { // static string patch = Path.Combine(GetSolutionParentDir(), "RenderANDVideoReaderVIdeoDecoder", "RenderANDVideoReaderVIdeoDecoder", "bin", "Debug", "net8.0", "RenderANDVideoReaderVIdeoDecoder.exe");
        System.Windows.Window window = new();
        static string patch = Path.Combine(GetSolutionParentDir(), "Rend", "RenderANDVideoReaderVIdeoDecoder", "RenderANDVideoReaderVIdeoDecoder", "bin", "Debug", "net8.0", "RenderANDVideoReaderVIdeoDecoder.exe");
        bool flagForOverlay;
        ImageDragDrop imageDragDrop = new ImageDragDrop();
        PixelPanelForZoom pixelPanelForZoom = new PixelPanelForZoom();
        protected override void OnAttached()
        {
            base.OnAttached();

              
             AssociatedObject.PreviewDragEnter += ellipse_DragEnter;
            //AssociatedObject.AllowDrop = true;
            AssociatedObject.Drop += AssociatedObject_Drop;



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
                imageDragDrop.GridChange = cellChil2;

            }

        }


        private void MouseUps(object s, System.Windows.Forms.MouseEventArgs args)
        {
            if (imageDragDrop.GridTake!=null&imageDragDrop.GridChange!=null)
            DragDrop.DoDragDrop(imageDragDrop.GridTake, imageDragDrop, System.Windows.DragDropEffects.Move);

        }
         
        private void Child_MouseDown(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            var Rite = AssociatedObject as Grid;
            var panel = sender as Panel;
            var grid = Rite.Children.OfType<Grid>().Where(c => c.Name == panel.Name).FirstOrDefault();

            if (grid != null)
            {
                imageDragDrop.GridTake = grid;

            }
        }
        //private void Child_MouseDown(object? sender, EventArgs e)
        //{
        //    var Rite = AssociatedObject as Grid;
        //    var panel = sender as Panel;
        //    var grid = Rite.Children.OfType<Grid>().Where(c => c.Name == panel.Name).FirstOrDefault();

        //    if (grid != null)
        //    {
        //        imageDragDrop.GridTake = grid;

        //    }
        //}
        private void AssociatedObject_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ImageDragDrop)))
            {
                var Rite = AssociatedObject as Grid;
                Point point = e.GetPosition(Rite);
                int row = GetRowGrid(point);
                int colum = GetColumnGrid(point);

                var cellChil = Rite.Children.OfType<Grid>().Where(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == colum).FirstOrDefault();
                if (cellChil == null)
                {
                    Grid.SetRow(imageDragDrop.GridTake, row);
                    Grid.SetColumn(imageDragDrop.GridTake, colum);
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
                WindowsFormsHost VideoHost = new WindowsFormsHost 
                {   DataContext=VideoHostSelect.DataContext,
                    Child= paneltest,
                    //Tag = VideoHostSelect.Tag,
                    Margin = new Thickness(5),
                     Name= VideoHostSelect.Name,
                     
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
                CameraConnectStrings tag = (CameraConnectStrings)paneltest.Tag;
                //string _videoSourceTest = tag.subStream;
                string nameCamera = VideoHost.Name;
                nint RenderTargetHwnd = VideoHost.Child.Handle;

                var mainFlow = Process.GetCurrentProcess();
              //  string patch = Path.Combine(GetSolutionParentDir(), "Rend", "RenderANDVideoReaderVIdeoDecoder", "RenderANDVideoReaderVIdeoDecoder", "bin", "Debug", "net8.0", "RenderANDVideoReaderVIdeoDecoder.exe");
                Process process = new();
               ProcessStartInfo start = new ProcessStartInfo(patch);
                
                start.ArgumentList.Add(tag.subStream);
                start.ArgumentList.Add(nameCamera);
                start.ArgumentList.Add(RenderTargetHwnd.ToString());
                start.ArgumentList.Add(mainFlow.Id.ToString());
              
                process.StartInfo = start;
                process.Start();
                grid.Tag = process.Id.ToString();
                VideoHost.Tag = window;
                window.Show();
                paneltest.MouseLeave += Leave;
                paneltest.MouseMove += WindowShow;
                
                window.Owner = System.Windows.Application.Current.MainWindow;

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
                //Grid child =  window.Content as Grid;
                // Button  b = child.Children.OfType<Button>().FirstOrDefault();
                // b.Name = paneltest.Name;
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

                //  };
                //  //   window.Opacity = 0.01;
                //  button1.Click += ButtonDeleteChildren;
                //  //UIElement uI = new UIElement();
                // // uI = button1;

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


                //  window.Owner = System.Windows.Application.Current.MainWindow;

                //  window.Width = Rite.ActualWidth/Rite.ColumnDefinitions.Count;
                //if     (Rite.RowDefinitions.Count!=0)  window.Height = Rite.ActualHeight/ Rite.RowDefinitions.Count;
                //else   window.Height = Rite.ActualHeight;



                //  window.Visibility = Visibility.Visible;

                //  //paneltest.MouseEnter += (s, ev) =>
                //  //{
                //  //    WindowShow(s, ev, window);
                //  //};


                //  //paneltest.MouseMove += (s, ev) =>
                //  //{
                //  //    WindowShow(s, ev, window);
                //  //};
                //  //paneltest.MouseLeave += (s, ev) =>
                //  //{
                //  //    Leave(  window);
                //  //};

                //  grid2.MouseMove += WindowShow;

                //  grid2.MouseLeave += Leave;
                //paneltest.MouseLeave += (s, ev) =>
                //{
                //    WindowHide(s, ev, window);
                //};
                //  grid.SizeChanged += SizeWindowChange;


                ////   start.Arguments = "";
                //   Process.Start(start);
                //  Program tests = new Program();

                // Task.Factory.StartNew(() => tests.Start(tag.subStream, nameCamera, RenderTargetHwnd));


            }
        }
       
        private void Leave(object? sender, EventArgs e)
        {   if (!flagForOverlay) return;
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
        {  flagForOverlay = false;
            Grid grid = s as Grid;

            grid.Visibility = Visibility.Visible;
        }
         

        static string GetSolutionParentDir()
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
            var Rite = AssociatedObject as Grid;
            var grid = Rite.Children.OfType<Grid>().Where(c => c.Name == panel.Name).FirstOrDefault();
            if (grid != null)
            { Rite.Visibility = Visibility.Hidden;
                var wfh = grid.Children.OfType<WindowsFormsHost>().FirstOrDefault();
                 
                Process process = new();
                var full = Rite.Parent as Grid;
                var newWind = new WindowsFormsHost
                {
                    Width = Rite.ActualWidth,
                    Height = Rite.ActualHeight,
                    Child = new Panel
                    {
                        DataContext = panel.DataContext,
                        Name = panel.Name,
                     //   AutoSize = true,
                        Tag= panel.Tag
                        
                    },
                    DataContext = grid.Children.OfType<WindowsFormsHost>().FirstOrDefault().DataContext,
                    Name = grid.Children.OfType<WindowsFormsHost>().FirstOrDefault().Name,
                   // Tag = Process.GetProcessById(),
                };
                newWind.Child.MouseDoubleClick += MouseDoubleClick;
               newWind.Child.MouseDown += MouseDownTakePxel;
                newWind.Child.MouseUp += MouseUpTakePixel;

                if (full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault() == null)
                {
                    full.Children.Add(new Grid()
                    //Grid nGrid = new Grid() 
                    {
                        Name = "FullScreenGrid",
                        Width = Rite.ActualWidth,
                        Height = Rite.ActualHeight,
                        Children =
                    {
                        newWind

                    }
                     ,
                        Margin = new Thickness((full.ColumnDefinitions.First().Width).Value + 5, 0, 0, 0),
                       
                    });
                    var panelNew = wfh.Child as Panel;
                    CameraConnectStrings tag = (CameraConnectStrings)panel.Tag;
                    var mainFlow = Process.GetCurrentProcess();
                   
                    ProcessStartInfo start = new ProcessStartInfo(patch);
                    start.ArgumentList.Add(tag.mainStream);
                    start.ArgumentList.Add(wfh.Name);
                    start.ArgumentList.Add(newWind.Child.Handle.ToString());
                    start.ArgumentList.Add(mainFlow.Id.ToString());
                    process.StartInfo = start;
                    process.Start();

                    //System.Windows.Window window = new();
                    //window.Topmost = true;
                    //window.Background = System.Windows.Media.Brushes.Transparent;
                    //window.WindowStyle = WindowStyle.None;
                    //window.AllowsTransparency = true;
                    //window.ShowInTaskbar = false;
                     
                     

                    //window.Show();


                    full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault().Tag = process.Id.ToString();
                }
                else
                {
                    Rite.Visibility = Visibility.Visible;
                    var t = full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault().Tag as string;
                     Process.GetProcessById(int.Parse(t)).Kill();
                    var t2 = full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault().Children.OfType<WindowsFormsHost>().FirstOrDefault().Tag;
                    if (t2!=null)Process.GetProcessById(int.Parse(t2.ToString())).Kill();
                    full.Children.Remove(full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault());
                    Border border = (Border)window.Content;
                    Grid gridOverlay = border.Child as Grid;
                   var child = gridOverlay.Children.OfType<Canvas>().FirstOrDefault();
                    if (child != null)
                    {
                        gridOverlay.Children.Remove(child);
                    }
                }
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

                //  Program startVideo = new Program();

                //if (panel.Tag == null)
                //{
                //    // startVideo.Stop();
                //    //        startVideo.Destroy();
                //    //   startVideo = null;
                //    GC.Collect();
                //    return;
                //}

                //string _videoSourceTest = panel.Tag.ToString();

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

                //  //  startVideo.Stop();
                //  WindowsFormsHost g = window.Content as WindowsFormsHost;
                //  g.Child.Dispose();
                // // g.Child = null;
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
      
        private void MouseUpTakePixel(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            var Rite = AssociatedObject as Grid;
            var full = Rite.Parent as Grid;

            pixelPanelForZoom.BottomRight = new Point(e.X, e.Y);
            if (pixelPanelForZoom.TopLeft != pixelPanelForZoom.BottomRight&pixelPanelForZoom.TopLeft!=null)
            {
                if (pixelPanelForZoom.BottomRight.X < pixelPanelForZoom.TopLeft.X)
                {
                    if (pixelPanelForZoom.BottomRight.Y < pixelPanelForZoom.TopLeft.Y)
                    { 
                        
                    }
                }
                Process process = new();
                Grid grid=  full.Children.OfType<Grid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();
                int W = (int)(Rite.ActualWidth/2);
                WindowsFormsHost wfh = grid.Children.OfType<WindowsFormsHost>().FirstOrDefault();
                wfh.Width = W;
                wfh.Child.Width = W;
                wfh.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                Panel panel = sender as Panel;
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                 grid.Children.Add(new WindowsFormsHost
                { 
                    Width = Rite.ActualWidth / 2,
                    Height = Rite.ActualHeight,
                    Child = new Panel
                    {
                        DataContext = panel.DataContext,
                        // Name = "ZoomPanel" + Guid.NewGuid().ToString("N"),UpdateZoomConstantBuffer(topLeft, buttomRight);  _context.PSSetConstantBuffers(0, new ID3D11Buffer[] { _zoomCB });   _rtv = _device.CreateRenderTargetView(backBuffer);
                        AutoSize = true,
                        Tag = panel.Tag
                    },
                    DataContext = panel.DataContext,
                    Name = "ZoomHost",
                    Margin = new Thickness(15,0,0,0),
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right
                 });
                var mainFlow = Process.GetCurrentProcess();
                ProcessStartInfo start = new ProcessStartInfo(patch);
                CameraConnectStrings tag = (CameraConnectStrings)grid.Children.OfType<WindowsFormsHost>().FirstOrDefault().Child.Tag;
                start.ArgumentList.Add(tag.mainStream);
                start.ArgumentList.Add("");
                start.ArgumentList.Add(grid.Children.OfType<WindowsFormsHost>().Where(s=>s.Name== "ZoomHost").FirstOrDefault().Child.Handle.ToString());
                start.ArgumentList.Add(mainFlow.Id.ToString());
                start.ArgumentList.Add(pixelPanelForZoom.TopLeft.X.ToString());
                start.ArgumentList.Add(pixelPanelForZoom.TopLeft.Y.ToString());
                start.ArgumentList.Add(pixelPanelForZoom.BottomRight.X.ToString());
                start.ArgumentList.Add(pixelPanelForZoom.BottomRight.Y.ToString());
                Border border = (Border)window.Content;
                Grid gridOverlay = border.Child as Grid;
                var child = gridOverlay.Children.OfType<Canvas>().FirstOrDefault();
                if (child != null)
                {
                    gridOverlay.Children.Remove(child);
                }
                Canvas canvas = new Canvas()
                {
                    Width =window.Width,
                    Height = window.Height, //pixelPanelForZoom.BottomRight.Y - pixelPanelForZoom.TopLeft.Y,
                    Background = System.Windows.Media.Brushes.Transparent,
                    
                };
                canvas.Children.Add(new System.Windows.Shapes.Rectangle
                {
                    Width = (pixelPanelForZoom.BottomRight.X - pixelPanelForZoom.TopLeft.X)/2,
                    Height = pixelPanelForZoom.BottomRight.Y - pixelPanelForZoom.TopLeft.Y,
                    Stroke = System.Windows.Media.Brushes.Red,
                    StrokeThickness = 2,
                    
                });
                gridOverlay.Children.Add(canvas);
                System.Windows.Shapes.Rectangle rectangle = gridOverlay.Children.OfType<Canvas>().FirstOrDefault().Children.OfType<System.Windows.Shapes.Rectangle>().FirstOrDefault();//.PointFromScreen(new Point(pixelPanelForZoom.TopLeft.X, pixelPanelForZoom.TopLeft.Y)) ;
                 rectangle.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                rectangle.VerticalAlignment = VerticalAlignment.Top;
                Canvas.SetLeft(rectangle, pixelPanelForZoom.TopLeft.X/2);
                Canvas.SetTop(rectangle, pixelPanelForZoom.TopLeft.Y);


                process.StartInfo = start;
                process.Start();
                grid.Children.OfType<WindowsFormsHost>().FirstOrDefault().Tag = process.Id.ToString();




            }
        }

        private void MouseDownTakePxel(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            pixelPanelForZoom.TopLeft = new Point(e.X, e.Y);
        }
       
       
        private void Swich()
        {
            var rowSet = Grid.GetRow(imageDragDrop.GridChange);
            var columSet = Grid.GetColumn(imageDragDrop.GridChange);
            var flipS = Grid.GetRow(imageDragDrop.GridTake);
            var flipC = Grid.GetColumn(imageDragDrop.GridTake);



            Grid.SetRow(imageDragDrop.GridTake, rowSet);
            Grid.SetColumn(imageDragDrop.GridTake, columSet);

            Grid.SetRow(imageDragDrop.GridChange, flipS);
            Grid.SetColumn(imageDragDrop.GridChange, flipC);

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
            var griddelet = Rite.Children.OfType<Grid>().Where(s=> s.Name == button.Name). FirstOrDefault();
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
                     
                        Process.GetProcessById(int.Parse(tag)).Kill();
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
    }
}
public class ImageDragDrop
{
  
    public Grid GridChange { get; set; }
    public Grid GridTake { get; set; }
}