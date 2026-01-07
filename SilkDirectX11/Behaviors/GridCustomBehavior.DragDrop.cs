using CppSharp.AST;
using DevExpress.Utils.Filtering.Internal;
using LibraryForSignalR;
using SilkDirectX11.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Win32.Graphics.Direct3D;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using Button = System.Windows.Controls.Button;
using Point = System.Windows.Point;
using Window = System.Windows.Window;

namespace SilkDirectX11.Behaviors
{
    partial class GridCustomBehavior
    {
         
        private void AssociatedObject_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CameraDragDrop)))
            {
                var hostGrid = AssociatedObject as Grid;
                System.Windows.Point point = e.GetPosition(hostGrid);
                int row = GetGridRow(point);
                int colum = GetGridColumn(point);

                var cellChil = hostGrid.Children.OfType<CustomGrid>().Where(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == colum).FirstOrDefault();
                if (cellChil == null)
                {
                    Grid.SetRow(_cameraDragDrop.GridTake, row);
                    Grid.SetColumn(_cameraDragDrop.GridTake, colum);
                    UpdateWindowPositionForGrid(_cameraDragDrop.GridTake);

                }
                else
                {
                    SwichCameraForDragDrop();
                }

            }
            else if (e.Data.GetDataPresent(typeof(WindowsFormsHost)))
            {
                var hostGrid = AssociatedObject as Grid;

                WindowsFormsHost VideoHostSelect = e.Data.GetData(typeof(WindowsFormsHost)) as WindowsFormsHost;
                CustomGrid grid = new CustomGrid()
                {
                    Window = new()
                    {
                        AllowDrop = true,
                        WindowStyle = WindowStyle.None,
                        ResizeMode = ResizeMode.NoResize,
                        ShowInTaskbar = false,
                    },
                    Name = VideoHostSelect.Name,
                    CameraGuidName = VideoHostSelect.Name + Guid.NewGuid().ToString("N"),
                    CameraConnectStrings = VideoHostSelect.Tag as CameraConnectStrings,
                    Background = System.Windows.Media.Brushes.Transparent,
                    Margin = new Thickness(5)
                };
                grid.Children.Add(new Border()
                {
                    Background = System.Windows.Media.Brushes.Transparent,
                    BorderBrush = System.Windows.Media.Brushes.White,
                    BorderThickness = new Thickness(5),
                    CornerRadius = new CornerRadius(5),
                    Margin = new Thickness(1, 1, 1, 1),
                    Padding = new Thickness(1, 1, 1, 1),

                });
                hostGrid.MouseUp += DropCamera;
                grid.Window.PreviewDragEnter += MouseMoveDragDrop;
                grid.Window.MouseLeave += ChengeBorderColor;
                grid.Window.MouseMove += CameraWindowMouseMove;
                grid.Window.Drop += AssociatedObject_Drop;
                grid.Window.Owner = System.Windows.Application.Current.MainWindow;
                grid.Window.Show();
                grid.Window.Name = grid.CameraGuidName;
                WindowInteropHelper helper = new WindowInteropHelper(grid.Window);
                grid.WindowTag = helper.Handle.ToString();
                grid.Window.MouseRightButtonDown += OnChangeFullScreen;
                grid.Window.MouseDown += StartDragDrop;
                grid.Window.MouseUp += DropCamera;
                grid.Window.MouseLeave += OnMouseLeaveHideOverlay;
                grid.Window.MouseMove += OnMouseMuveWindowShow;
                System.Windows.Application.Current.MainWindow.Closed += MainWindow_Closed;
                grid.Window.SizeChanged += grid.ChengeSizeOverleyWindow;
                grid.Window.LocationChanged += grid.ChengeLocationOverleyWindow;
                grid.Window.Owner.LocationChanged += OwnedWindowsLocationChange;
                List<string> arguments = new List<string>() { grid.CameraConnectStrings.SubStream, grid.CameraConnectStrings.MainStream, grid.Name, grid.WindowTag, grid.CameraGuidName, _flagForReconnect.ToString() };
                var process = StartProcess(arguments, grid.CameraConnectStrings.CameraID);
                grid.ProcessTag = process.Id.ToString();

                Window windOwerlay = InitOverlayWindow();
                (((Border)windOwerlay.Content).Child as Grid).Children.OfType<Grid>().Where(s => s.Name == "GridWithButtonClouseOverlay").FirstOrDefault().Children.OfType<Button>().FirstOrDefault().Name = grid.CameraGuidName;
                (((Border)windOwerlay.Content).Child as Grid).Children.OfType<Grid>().Where(s => s.Name == "GridWithButtonZoomMode").FirstOrDefault().Children.OfType<Button>().FirstOrDefault().Name = grid.CameraGuidName;
                windOwerlay.Width = grid.Window.ActualWidth;
                windOwerlay.Height = grid.Window.ActualHeight;
                windOwerlay.Owner = grid.Window;
                windOwerlay.Show();
               
                System.Windows.Application.Current.MainWindow.Focus();
                int colum = GetGridColumn(e.GetPosition(hostGrid));
                int row = GetGridRow(e.GetPosition(hostGrid));
                if (hostGrid.Children.Count <= 1) // заполнение первых двух ячеек 
                {
                    hostGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    int u =hostGrid.ColumnDefinitions.Count;
                    Grid.SetColumn(grid, hostGrid.Children.Count);
                    hostGrid.Children.Add(grid);
                }
                else if (hostGrid.RowDefinitions.Count == 0) // вставляется третья фотка 
                {
                    hostGrid.RowDefinitions.Add(new RowDefinition());
                    Grid.SetRow(grid, hostGrid.RowDefinitions.Count);
                    hostGrid.RowDefinitions.Add(new RowDefinition());
                    hostGrid.Children.Add(grid);

                }
                else
                {
                    var cellChil = hostGrid.Children.OfType<Grid>().Where(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == colum).FirstOrDefault();
                    if (cellChil == null)
                    {
                        Grid.SetRow(grid, row);
                        Grid.SetColumn(grid, colum);
                        hostGrid.Children.Add(grid);
                    }
                    else
                    {
                        if (CheckEmptyChildInGrid(hostGrid))// заполнение ячеек по порядку  если ячейка правая нижняя пустая
                        {
                            AddGrid(hostGrid, grid);
                        }
                        else // тут проверка на заполненность последней ячеки правой нижней
                        {
                            hostGrid.RowDefinitions.Add(new RowDefinition());
                            if (hostGrid.RowDefinitions.Count > 2)
                                hostGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            Grid.SetRow(grid, hostGrid.RowDefinitions.Count - 1);
                            hostGrid.Children.Add(grid);
                        }
                    }


                }


            }
            
           

        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
             
        }

        private void UpdateWindowPositionForGrid(CustomGrid grid)
        {
            if (grid == null || grid.Window == null) return;

            (AssociatedObject as Grid)?.UpdateLayout();
            try
            {
                var p = grid.PointToScreen(new Point());
                grid.Window.Left = p.X + 5;
                grid.Window.Top = p.Y + 5;
            }
            catch
            { }
        }
         

        private void OwnedWindowsLocationChange(object? sender, EventArgs e)
        { 
            foreach (var item in AssociatedObject.Children.OfType<CustomGrid>())
            {
                if (item.Window==null) continue;
                item.Window.Left = item.PointToScreen(new Point()).X + 5;
                item.Window.Top = item.PointToScreen(new Point()).Y + 5;
            }
            CustomGrid host = (AssociatedObject.Parent as Grid).Children.OfType<CustomGrid>().Where(s => s.Name == "FullScreenGrid").FirstOrDefault();
            if (host != null)
            {
                host.Window.Left = host.PointToScreen(new Point()).X + 5;
                host.Window.Top = host.PointToScreen(new Point()).Y + 5;
                
            }
           
        }

        private void CameraWindowMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            { return; }
            var w = sender as Window;
            AssociatedObject.Children.OfType<CustomGrid>().Where(c => c.CameraGuidName == w.Name).FirstOrDefault().Children.OfType<Border>().FirstOrDefault().BorderBrush = System.Windows.Media.Brushes.Green;
        }

        private int GetGridRow(Point point)
        {
            var Rite = AssociatedObject as Grid;
            int countRow = Rite.RowDefinitions.Count;
            double heightCell = Rite.ActualHeight / (countRow == 0 ? 1 : countRow);
            int row = (int)(point.Y / heightCell);
            return row;
        }
        private int GetGridColumn(Point point)
        {
            var Rite = AssociatedObject as Grid;
            int countCollumn = Rite.ColumnDefinitions.Count;
            double widthCell = Rite.ActualWidth / (countCollumn == 0 ? 1 : countCollumn);
            int colum = (int)(point.X / widthCell);
            return colum;

        }

        private void MouseMoveDragDrop(object sender, System.Windows.DragEventArgs e)
        {
            var Rite = AssociatedObject as Grid;
            System.Windows.Point point = e.GetPosition(Rite);
            int row = GetGridRow(point);
            int colum = GetGridColumn(point);
            var cellChil2 = Rite.Children.OfType<CustomGrid>().Where(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == colum).FirstOrDefault();

            if (cellChil2 != null)
            {
                
                _cameraDragDrop.GridChange = cellChil2;

            }

        }
        private void ChengeBorderColor(object sender, System.Windows.Input.MouseEventArgs e)
        { 
            var senderWind = sender as Window;
            var Rite = AssociatedObject as Grid;
            var cellChil = Rite.Children.OfType<CustomGrid>().Where(c => c.CameraGuidName == senderWind.Name).FirstOrDefault();
            if (cellChil != null)
            {
                cellChil.Children.OfType<Border>().FirstOrDefault().BorderBrush = System.Windows.Media.Brushes.White;
            }
        }
        private void DropCamera(object sender, MouseButtonEventArgs e)
        {
             if (idk) return;
            if (_cameraDragDrop.GridTake != null & _cameraDragDrop.GridChange != null)
                DragDrop.DoDragDrop(_cameraDragDrop.GridTake, _cameraDragDrop, System.Windows.DragDropEffects.Move);
           
        }
        bool idk = false;
        private void StartDragDrop(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                idk = true; return;
            }
            idk = false;
            var Rite = AssociatedObject as Grid;
            var wind = sender as Window;
            var grid = Rite.Children.OfType<CustomGrid>().Where(c => c.CameraGuidName == wind.Name).FirstOrDefault();

            if (grid != null)
            {
                _cameraDragDrop.GridTake = grid;
                return;
            }
            var gridZoom = Rite.Children.OfType<CustomGrid>().Where(c => c.Name == wind.Owner.Name).FirstOrDefault();
            if (gridZoom != null)
            {
                _cameraDragDrop.GridTake = gridZoom;
                return;
            }
        }
        private Process StartProcess(List<string> argument, Guid IDCamera)
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
            process.StartInfo = start;
            process.Start();

            return process;
        }

        private void SwichCameraForDragDrop()
        {
            var rowSet = Grid.GetRow(_cameraDragDrop.GridChange);
            var columSet = Grid.GetColumn(_cameraDragDrop.GridChange);
            var flipS = Grid.GetRow(_cameraDragDrop.GridTake);
            var flipC = Grid.GetColumn(_cameraDragDrop.GridTake);

            Grid.SetRow(_cameraDragDrop.GridTake, rowSet);
            Grid.SetColumn(_cameraDragDrop.GridTake, columSet);

            Grid.SetRow(_cameraDragDrop.GridChange, flipS);
            Grid.SetColumn(_cameraDragDrop.GridChange, flipC);
            try
            {

                _cameraDragDrop.GridTake.Window.Left = _cameraDragDrop.GridChange.PointToScreen(new Point()).X + 5;
                _cameraDragDrop.GridTake.Window.Top = _cameraDragDrop.GridChange.PointToScreen(new Point()).Y + 5;
                _cameraDragDrop.GridChange.Window.Left = _cameraDragDrop.GridTake.PointToScreen(new Point()).X + 5;
                _cameraDragDrop.GridChange.Window.Top = _cameraDragDrop.GridTake.PointToScreen(new Point()).Y + 5;
            }
            catch
            { }

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

        private void DeleteGridChild(object sender, RoutedEventArgs e)
        {
            var Rite = AssociatedObject as Grid;
            CustomGrid gridDelet = null;
            Button button = sender as Button;
           
                gridDelet = Rite.Children.OfType<CustomGrid>().Where(s => s.CameraGuidName == button.Name).FirstOrDefault();
            
            if (gridDelet != null)
            {
                var indexC = Grid.GetColumn(gridDelet);
                var IndexR = Grid.GetRow(gridDelet);
                var cellContent = Rite.Children.OfType<UIElement>().FirstOrDefault(c => Grid.GetRow(c) == IndexR && Grid.GetColumn(c) == indexC);
                CustomGrid grid = cellContent as CustomGrid;
                if (grid != null)
                {
                    var tag = grid.ProcessTag;
                    if (!string.IsNullOrEmpty(tag))
                    {
                        try
                        {
                            Process.GetProcessById(int.Parse(tag)).Kill();
                        }
                        catch { }
                        grid.Window.OwnedWindows[0].Close();
                       
                    }
                    grid.Window.Close();
                }

                grid.Children.Clear();
                Rite.Children.Remove(cellContent);


                RemuveTopLeft();
                RemuveBottomRite();
                 
               

            }
        }

        private void RemuveTopLeft()
        {
            var Rite = AssociatedObject as Grid;
            for (string i = "Null"; i == "Null";)

                switch (CheckTopLeft())
                {
                    case "Top":
                        if (Rite.RowDefinitions.Count >= 1)
                            Rite.RowDefinitions.RemoveAt(0);
                        foreach (var item in Rite.Children.OfType<CustomGrid>())
                        {
                            Grid.SetRow(item, Math.Max(0, Grid.GetRow(item) - 1));
                            
                        }
                        break;
                    case "Left":
                        if (Rite.ColumnDefinitions.Count >= 1)
                            Rite.ColumnDefinitions.RemoveAt(0);
                        foreach (var item in Rite.Children.OfType<CustomGrid>())
                        {
                            Grid.SetColumn(item, Math.Max(0, Grid.GetColumn(item) - 1));
                          
                        }
                        break;
                    case "TopLeft":
                        if (Rite.RowDefinitions.Count >= 1)
                            Rite.RowDefinitions.RemoveAt(0);
                        if (Rite.ColumnDefinitions.Count >= 1)
                            Rite.ColumnDefinitions.RemoveAt(0);
                        foreach (var item in Rite.Children.OfType<CustomGrid>())
                        {
                            Grid.SetRow(item, Math.Max(0, Grid.GetRow(item) - 1));
                            Grid.SetColumn(item, Math.Max(0, Grid.GetColumn(item) - 1));
                            
                        }
                        break;
                    case "Null":
                        i = "break";
                        break;
                }
             (AssociatedObject as Grid)?.UpdateLayout();
        }
        private void RemuveBottomRite()
        {
            var Rite = AssociatedObject as Grid;
            for (string i = "Null"; i == "Null";)
                switch (CheckBottomRite())
                {
                    case "RiteBottom":
                        if (Rite.RowDefinitions.Count >= 1)
                            Rite.RowDefinitions.RemoveAt(Rite.RowDefinitions.Count - 1);
                        if (Rite.ColumnDefinitions.Count >= 1)
                            Rite.ColumnDefinitions.RemoveAt(Rite.ColumnDefinitions.Count - 1);
                        break;
                    case "Bottom":
                        if (Rite.RowDefinitions.Count >= 1)
                            Rite.RowDefinitions.RemoveAt(Rite.RowDefinitions.Count - 1);
                        break;
                    case "hostGrid":
                        if (Rite.ColumnDefinitions.Count >= 1)
                            Rite.ColumnDefinitions.RemoveAt(Rite.ColumnDefinitions.Count - 1);
                        break;
                    case "Null":
                        i = "break";
                        break;
                }
             (AssociatedObject as Grid)?.UpdateLayout();
        }

        private string CheckBottomRite()
        {
            var Rite = AssociatedObject as Grid;

            List<int> listRow = new List<int>() { 1};
            List<int> listColumn = new List<int>() { 1};
            for (int i = 0; i < Rite.ColumnDefinitions.Count; i++)
            {
                listRow =new List<int>();
                if (Rite.Children.OfType<UIElement>().FirstOrDefault(c => Grid.GetRow(c) == Rite.RowDefinitions.Count - 1 && Grid.GetColumn(c) == i) != null)
                {
                    listRow.Add(i);
                    break;
                }


            }
            for (int i = 0; i < Rite.RowDefinitions.Count; i++)
            {
                listColumn = new List<int>();
                if ((Rite.Children.OfType<UIElement>().FirstOrDefault(c => Grid.GetRow(c) == i && Grid.GetColumn(c) == Rite.ColumnDefinitions.Count - 1)) != null)
                {
                    listColumn.Add(i);
                    break;
                }
            }
            if (Rite.ColumnDefinitions.Count <= 1 && Rite.RowDefinitions.Count <= 1)
                return "Null";

            if (listRow.Count == 0 && listColumn.Count == 0)
                return "RiteBottom";
             
            if (listRow.Count == 0)
                return "Bottom";
            
             if (listColumn.Count == 0)
                 return "hostGrid";
              
             return "Null";
        }
        private string CheckTopLeft()
        {

            var Rite = AssociatedObject as Grid;
        
            List<int> listRow = new List<int>();
            List<int> listColumn = new List<int>();
            for (int i = 0; i < Rite.ColumnDefinitions.Count; i++)
            {
                if (Rite.Children.OfType<UIElement>().FirstOrDefault(c => Grid.GetRow(c) == 0 && Grid.GetColumn(c) == i) != null)
                {
                    listRow.Add(i);
                }


            }
            for (int i = 0; i < Rite.RowDefinitions.Count; i++)
            {
                if ((Rite.Children.OfType<UIElement>().FirstOrDefault(c => Grid.GetRow(c) == i && Grid.GetColumn(c) == 0)) != null)
                {
                    listColumn.Add(i);
                }
            }
            if (Rite.ColumnDefinitions.Count <= 1 && Rite.RowDefinitions.Count <= 1)
                return "Null";

            if (listRow.Count == 0 && listColumn.Count == 0)
                return "TopLeft";
             
            if (listRow.Count == 0)
                return "Top";
              
            if (listColumn.Count == 0)
                return "Left";
           
            return "Null";
        }

      

    }
}
