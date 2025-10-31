using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using Button = System.Windows.Controls.Button;
using Image = System.Windows.Controls.Image;
using Panel = System.Windows.Forms.Panel;
using Point = System.Windows.Point;

namespace SilkDirectX11.Behaviors
{
    class GridCustomBehavior : Behavior<Grid>
    {
        ImageDragDrop imageDragDrop = new ImageDragDrop();
        protected override void OnAttached()
        {
            base.OnAttached();

              
             AssociatedObject.PreviewDragEnter += ellipse_DragEnter;
            AssociatedObject.AllowDrop = true;
            AssociatedObject.Drop += AssociatedObject_Drop;
             
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
        private void TestMo(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var Rite = sender as Grid;
            System.Windows.Point point = e.GetPosition(Rite);
            int row = GetRowGrid(point);
            int colum = GetColumnGrid(point);
        }
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
                    

                };
                paneltest.MouseDown += Child_MouseDown;
                paneltest.MouseUp += MouseUps;
                WindowsFormsHost test = new WindowsFormsHost 
                {   DataContext=VideoHostSelect.DataContext,
                    Child= paneltest,
                    Tag = VideoHostSelect.Tag,
                    Margin = new Thickness(5),
                     Name= VideoHostSelect.Name,
                     
                };
                
                Button button = new Button
                { 
                    Content = "X",
                    Width = 20,
                    Height = 20,
                     
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,

                    Opacity = 0.5,
                };
                var Rite = AssociatedObject as Grid;
                //if (Rite.ColumnDefinitions.Count != 0)
                //{
                //    VideoHost.Height = VideoHostSelect.Height / Rite.ColumnDefinitions.Count;
                //}
                //else if (Rite.RowDefinitions.Count != 0)
                //    VideoHost.Width = VideoHostSelect.Width / Rite.RowDefinitions.Count;



                UIElement uIElement = new UIElement();
                UIElement uIElement2 = new UIElement();


                
                // uIElement = VideoHost;
                uIElement = test;

                uIElement.AllowDrop = true;

                button.Click += ButtonDeleteChildren;

                uIElement2 = button;
                uIElement2.AllowDrop = true;
                 
                grid.Name = test.Child.Name;//panel.Name;
                Rite.MouseMove += TestMo;
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
                string _videoSourceTest = test.Tag.ToString();
                string a = test.Name;
                nint RenderTargetHwnd = test.Child.Handle;
                RenderANDVideoReaderVIdeoDecoder.Program tests = new RenderANDVideoReaderVIdeoDecoder.Program();
                Task.Factory.StartNew(() => tests.Start(_videoSourceTest, a, RenderTargetHwnd));


            }
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
            var stackP = VisualTreeHelper.GetParent(button);
            var indexC = Grid.GetColumn((UIElement)stackP);
            var IndexR = Grid.GetRow((UIElement)stackP);
            var cellContent = Rite.Children.OfType<UIElement>().FirstOrDefault(c => Grid.GetRow(c) == IndexR && Grid.GetColumn(c) == indexC);
            Grid test = cellContent as Grid;
            test.Children.Clear();
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