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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using Button = System.Windows.Controls.Button;
using Point = System.Windows.Point;

namespace SilkDirectX11.Behaviors
{
    partial class GridCustomBehavior
    {
         
        private void AssociatedObject_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CameraDragDrop)))
            {
                var Rite = AssociatedObject as Grid;
                System.Windows.Point point = e.GetPosition(Rite);
                int row = GetGridRow(point);
                int colum = GetGridColumn(point);

                var cellChil = Rite.Children.OfType<CustomGrid>().Where(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == colum).FirstOrDefault();
                if (cellChil == null)
                {
                    Grid.SetRow(_cameraDragDrop.GridTake, row);
                    Grid.SetColumn(_cameraDragDrop.GridTake, colum);
                    _cameraDragDrop.GridTake.RaiseEvent(new RoutedEventArgs(CustomGrid.SizeChangedEvent));
                }
                else
                {
                    SwichCameraForDragDrop();
                }
                
                //System.Windows.Application.Current.MainWindow.Width += 1.1;
                //System.Windows.Application.Current.MainWindow.Width -= 1.1;
                //Rite.Width = Rite.ActualWidth + 0.1;
                //Rite.Width = Rite.ActualWidth - 0.1;
            }
            else if (e.Data.GetDataPresent(typeof(WindowsFormsHost)))
            {
                var Rite = AssociatedObject as Grid;

                WindowsFormsHost VideoHostSelect = e.Data.GetData(typeof(WindowsFormsHost)) as WindowsFormsHost;
                CustomGrid grid = new CustomGrid()
                {
                     
                    Window = new()
                    {
                        
                        AllowDrop = true,
                        WindowStyle = WindowStyle.None,
                        ResizeMode = ResizeMode.NoResize,
                        Owner = System.Windows.Application.Current.MainWindow,
                       // ToolTip = "Правый клик - полноэкранный режим\nЛевый клик - перетаскивание\nКнопка в углу - закрыть окно",
                    },
                    Name = VideoHostSelect.Name,
                    CameraGuidName = VideoHostSelect.Name + Guid.NewGuid().ToString("N"),
                    CameraConnectStrings = VideoHostSelect.Tag as CameraConnectStrings,
                    Background = System.Windows.Media.Brushes.Transparent,
                 
                     
                   
                };
                grid.Children.Add(  new Border()
                {
                    Background = System.Windows.Media.Brushes.Transparent,
                    BorderBrush = System.Windows.Media.Brushes.White,
                    BorderThickness = new Thickness(5),
                    CornerRadius = new CornerRadius(5),
                    Margin = new Thickness(1, 1, 1, 1),
                    Padding = new Thickness(1, 1, 1, 1),
                   
                    
                 });
                
                 
               
                Rite.MouseUp += DropCamera;
                grid.Window.PreviewDragEnter += MouseMoveDragDrop;
                grid.Window.BorderBrush = System.Windows.Media.Brushes.Red;
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
                grid.Margin = new Thickness(5);
                int colum = GetGridColumn(e.GetPosition(Rite));
                int row = GetGridRow(e.GetPosition(Rite));
              
                grid.Window.Owner.LocationChanged += OwnedWindowsLocationChange;
                grid.Window.Owner.SizeChanged += OwnerSizeChanged;
                List<string> arguments = new List<string>() { grid.CameraConnectStrings.SubStream, grid.CameraConnectStrings.MainStream, grid.Name, grid.WindowTag, grid.CameraGuidName};
                var process = StartProcess(arguments, grid.CameraConnectStrings.CameraID);
                grid.ProcessTag = process.Id.ToString();
             
                _windowOverlay.Owner = System.Windows.Application.Current.MainWindow;
                _windowOverlay.Show();
                


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

        private void OwnerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Border border = (Border)_windowOverlay.Content;
            Grid grids = (Grid)border.Child;
            Grid grid = grids.Children.OfType<Grid>().FirstOrDefault();
            Button b = grid.Children.OfType<Button>().FirstOrDefault();
            var selectedGrid = AssociatedObject.Children.OfType<CustomGrid>().Where(s => s.CameraGuidName == b.Name).FirstOrDefault();
            if (selectedGrid == null) return;
            if (selectedGrid.Window != null)
            {
                _windowOverlay.Width = selectedGrid.Window.ActualWidth;
                _windowOverlay.Height = selectedGrid.Window.ActualHeight;
                _windowOverlay.Left = selectedGrid.Window.PointToScreen(new Point()).X;
                _windowOverlay.Top = selectedGrid.Window.PointToScreen(new Point()).Y;
            }
            else
            {
                _windowOverlay.Width = selectedGrid.ActualWidth;
                _windowOverlay.Height = selectedGrid.ActualHeight;
                
            }
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
                host.Window.Left = host.PointToScreen(new Point()).X;
                host.Window.Top = host.PointToScreen(new Point()).Y;
                
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

            _cameraDragDrop.GridTake.Window.Left = _cameraDragDrop.GridChange.PointToScreen(new Point()).X + 5;
            _cameraDragDrop.GridTake.Window.Top = _cameraDragDrop.GridChange.PointToScreen(new Point()).Y + 5;
            _cameraDragDrop.GridChange.Window.Left = _cameraDragDrop.GridTake.PointToScreen(new Point()).X + 5;
            _cameraDragDrop.GridChange.Window.Top = _cameraDragDrop.GridTake.PointToScreen(new Point()).Y + 5;


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

            Button button = sender as Button;
            var gridDelet = Rite.Children.OfType<CustomGrid>().Where(s => s.CameraGuidName == button.Name).FirstOrDefault();
            if (gridDelet!=null)
            {
                var indexC = Grid.GetColumn(gridDelet);
                var IndexR = Grid.GetRow(gridDelet);
                var cellContent = Rite.Children.OfType<UIElement>().FirstOrDefault(c => Grid.GetRow(c) == IndexR && Grid.GetColumn(c) == indexC);
                CustomGrid grid = cellContent as CustomGrid;
                if (grid != null)
                {
                    var tag = grid.ProcessTag ;
                    if (!string.IsNullOrEmpty(tag))
                    {
                        try
                        {
                            Process.GetProcessById(int.Parse(tag)).Kill();
                        }
                        catch { }
                        Border border = (Border)_windowOverlay.Content;
                        Grid grids = (Grid)border.Child;
                        Grid gridOverlay = grids.Children.OfType<Grid>().FirstOrDefault();

                        gridOverlay.Visibility = Visibility.Hidden;



                    }
                    grid.Window.Close();
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


        //private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
            
        //    CustomGrid grid = sender as CustomGrid;
        //    grid.Window.Left = grid.PointToScreen(new Point()).X; 
        //    grid.Window.Top = grid.PointToScreen(new Point()).Y;
        //    grid.Window.Width = grid.ActualWidth; 
        //    grid.Window.Height = grid.ActualHeight;  
        //    //WindowsFormsHost wfh = grid.Children.OfType<WindowsFormsHost>().FirstOrDefault();
        //    //if (wfh != null)
        //    //{
        //    //    //SetSize setSize = new SetSize((int)e.NewSize.Width, (int)e.NewSize.Height, int.Parse(grid.Tag.ToString()));
        //    //    //wfh.Child.Size = new System.Drawing.Size((int)e.NewSize.Width, (int)e.NewSize.Height);
        //    //    wfh.Width = e.NewSize.Width;
        //    //    wfh.Height = e.NewSize.Height;
        //    //    //wfh.Child.Width = (int)grid.ActualWidth;
        //    //    //wfh.Child.Height = (int)grid.ActualWidth;
        //    //    //SendSetSize(setSize);
        //    //}
        //}

        //private void VideoHost_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    WindowsFormsHost host = sender as WindowsFormsHost;
        //    if (host != null)
        //    {
        //        Grid grid = host.Parent as Grid;

        //        SetSize setSize = new SetSize((int)e.NewSize.Width, (int)e.NewSize.Height, int.Parse(grid.Tag.ToString()));
        //        //SendSetSize((int)e.NewSize.Width, (int)e.NewSize.Height,int.Parse(grid.Tag.ToString()));
        //        SendSetSize(setSize);
        //        host.Child.Width = (int)e.NewSize.Width;
        //        host.Child.Height = (int)e.NewSize.Height;
        //        // host.Margin = new Thickness(5);
        //    }
        //}


    }
}
