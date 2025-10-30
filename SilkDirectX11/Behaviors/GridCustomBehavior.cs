using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
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

            AssociatedObject.MouseDown += MouseDrop;
            AssociatedObject.PreviewDragEnter += ellipse_DragEnter;
            AssociatedObject.AllowDrop = true;
            AssociatedObject.Drop += AssociatedObject_Drop;
            
        }

        private void MouseDrop(object sender, MouseButtonEventArgs e)
        {


            var flag = sender as WindowsFormsHost;//Grid??



            if (flag != null && flag.Name == "")
            {
                imageDragDrop.HostTake = sender as WindowsFormsHost;
                DragDrop.DoDragDrop(imageDragDrop.HostTake, imageDragDrop, System.Windows.DragDropEffects.Move);
            }
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
        private void ellipse_DragEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var Rite = AssociatedObject as Grid;
            System.Windows.Point point = e.GetPosition(Rite);
            int row = GetRowGrid(point);
            int colum = GetColumnGrid(point);
            // var cellChil //= Rite.Children.OfType<WindowsFormsHost>().Where(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == colum).FirstOrDefault();
            var cellChil2 = Rite.Children.OfType<Grid>().Where(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == colum).FirstOrDefault();


            Panel ellipse = sender as Panel;


            if (cellChil2 != null)
            {

                //var cellChil= cellChil2.Children.OfType<WindowsFormsHost>().Where(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == colum).FirstOrDefault();
                // if (cellChil != null)
                // {
                //     imageDragDrop.HostChange = cellChil as WindowsFormsHost;
                // }
                // else
                imageDragDrop.GridChange = cellChil2;
                //DragDrop.DoDragDrop(imageDragDrop.GridChange, imageDragDrop, System.Windows.DragDropEffects.Move);
            }
            else imageDragDrop.GridChange = Rite;
        }
        private void ellipse_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            var Rite = AssociatedObject as Grid;
            System.Windows.Point point = e.GetPosition(Rite);
            int row = GetRowGrid(point);
            int colum = GetColumnGrid(point);
           // var cellChil //= Rite.Children.OfType<WindowsFormsHost>().Where(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == colum).FirstOrDefault();
            var cellChil2 = Rite.Children.OfType<Grid>().Where(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == colum).FirstOrDefault();


            Panel ellipse = sender as Panel;

          
            if (cellChil2 != null)
            {
                 
               //var cellChil= cellChil2.Children.OfType<WindowsFormsHost>().Where(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == colum).FirstOrDefault();
               // if (cellChil != null)
               // {
               //     imageDragDrop.HostChange = cellChil as WindowsFormsHost;
               // }
               // else
                    imageDragDrop.GridChange = cellChil2;
                //DragDrop.DoDragDrop(imageDragDrop.GridChange, imageDragDrop, System.Windows.DragDropEffects.Move);
            }
            else imageDragDrop.GridChange = Rite;
            //else if (sender as Grid != null)
            //{
            //    imageDragDrop.GridChange = sender as Grid;
            //    // e.Effects = DragDropEffects.Move;
            //}
        }
        
        private void MouseD(object s, System.Windows.Forms.MouseEventArgs args, object t )
        {
            //var flag = s as WindowsFormsHost; 
            
           
            //if (flag != null )
            //{
                 
            //    imageDragDrop.HostTake = s as WindowsFormsHost;
            //    DragDrop.DoDragDrop(imageDragDrop.HostTake, imageDragDrop, System.Windows.DragDropEffects.Move);
               
            //}
            var grid = s as Grid;
            if (grid != null)
            {
                imageDragDrop.GridTake = grid;
                DragDrop.DoDragDrop(imageDragDrop.GridTake, imageDragDrop, System.Windows.DragDropEffects.Move);
            }
        }
        private void testetmetod(object s, System.Windows.Forms.MouseEventArgs args)
        {
            var Rite = AssociatedObject as Grid;
            int x = args.X;
            int y = args.Y;
            System.Windows.Point point = new Point(x, y);//args.Location.X(Rite);
            int row = GetRowGrid(point);
            int colum = GetColumnGrid(point);
             var cellChil2 = Rite.Children.OfType<Grid>().Where(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == colum).FirstOrDefault();


            //Panel ellipse = grid as Panel;


            if (cellChil2 != null)
            {

                 
                imageDragDrop.GridChange = cellChil2;
                 
            }
            else imageDragDrop.GridChange = Rite;
        }
        private void testetmetod(object s, System.Windows.Input.MouseEventArgs args)
        {
            var Rite = AssociatedObject as Grid;
            //int x = args.X;
            //int y = args.Y;
            System.Windows.Point point =args.GetPosition(Rite);//args.Location.X(Rite);
            int row = GetRowGrid(point);
            int colum = GetColumnGrid(point);
            var cellChil2 = Rite.Children.OfType<Grid>().Where(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == colum).FirstOrDefault();


            //Panel ellipse = grid as Panel;


            if (cellChil2 != null)
            {


                imageDragDrop.GridChange = cellChil2;

            }
            else imageDragDrop.GridChange = Rite;
        }


        private void MouseUps(object s, System.Windows.Forms.MouseEventArgs args)
        {
            throw new NotImplementedException();
        }
        //private void dd(object s, System.Windows.Forms.MouseEventArgs args)
        //{
        //   // throw new NotImplementedException();
        //}
        //private void ellipse_Drag(object s, System.Windows.Forms.DragEventArgs arg)
        //{
        //    throw new NotImplementedException();
        //}

        //private void dragEnter(object s, PreviewKeyDownEventArgs args)
        //{
        //    throw new NotImplementedException();
        //}

        //private void Panel_PreviewKeyDown(object? sender, PreviewKeyDownEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}
        //private void dragEnter(object s, System.Windows.Forms.DragEventArgs args)
        //{


        //    //WindowsFormsHostAutomationPeer.FromElement(p);

        //    //using (Panel jj = buf as Panel)
        //    //{
        //    //    //imageDragDrop.HostChange = jj as WindowsFormsHost;
        //    //}
        //    //WindowsFormsHost wff = buf.Parent as WindowsFormsHost;
        //    //imageDragDrop.HostChange = wff;
        //}
        //private void dragEntert(object s, System.Windows.DragEventArgs args)
        //{
        //    throw new NotImplementedException();
        //}
        int tester = 0;
        private void AssociatedObject_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ImageDragDrop)))
            {
                int rowSet;
                int columSet;
                if (imageDragDrop.GridChange != null&& imageDragDrop.GridChange.Name != "VideoCanvas1")
                {

                    rowSet = Grid.GetRow(imageDragDrop.GridChange);
                    columSet = Grid.GetColumn(imageDragDrop.GridChange);
                }
                else
                {
                    rowSet = Grid.GetRow(imageDragDrop.HostChange);
                    columSet = Grid.GetColumn(imageDragDrop.HostChange);
                }
                var flipS = Grid.GetRow(imageDragDrop.GridTake);
                var flipC = Grid.GetColumn(imageDragDrop.GridTake);

                Grid.SetRow(imageDragDrop.GridTake, rowSet);
                Grid.SetColumn(imageDragDrop.GridTake, columSet);

                Grid.SetRow(imageDragDrop.GridChange, flipS);
                Grid.SetColumn(imageDragDrop.GridChange, flipC);

            }
            else if (e.Data.GetDataPresent(typeof(WindowsFormsHost)))
            {
                
                WindowsFormsHost VideoHostSelect = e.Data.GetData(typeof(WindowsFormsHost)) as WindowsFormsHost;
                Grid grid = new Grid();
                WindowsFormsHost VideoHost = VideoHostSelect;
                VideoHost.Margin = new Thickness(5);
                VideoHost.MouseDown += MouseDrop;
                VideoHost.MouseMove += testetmetod;
               // VideoHost.PreviewDragEnter += ellipse_DragEnter;
               // VideoHost.DragEnter += (s, args) => { dragEntert(s, args); };
               //VideoHost.PreviewDragEnter += (s,agr) => { dragEntert(s, agr); };

                VideoHost.AllowDrop = true;
                //VideoHost.Drop += AssociatedObject_Drop;
                //VideoHost.Name = "VideoHost" + Guid.NewGuid().ToString("N"); yf gjnjv 
                System.Windows.Forms.Panel panel = VideoHost.Child as System.Windows.Forms.Panel;

                //    new WindowsFormsHost//потом вместо ебануть форму
                //{
                //    //Source = img.Source,
                //    //Stretch = Stretch.Fill,
                //   //Cursor = Cursors.Hand,
                //    Margin = new Thickness(2)

                //};
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
                if (Rite.ColumnDefinitions.Count != 0)
                {
                    VideoHost.Height = VideoHostSelect.Height / Rite.ColumnDefinitions.Count;
                }
                else if (Rite.RowDefinitions.Count != 0)
                    VideoHost.Width = VideoHostSelect.Width / Rite.RowDefinitions.Count;



                UIElement uIElement = new UIElement();
                UIElement uIElement2 = new UIElement();

                
                
                uIElement = VideoHost;
               // uIElement.DragEnter += (s, args) => { dragEntert(s, args); };
                uIElement.MouseDown += MouseDrop;

                uIElement.AllowDrop = true;

                button.Click += ButtonDeleteImage;

                uIElement2 = button;
                uIElement2.AllowDrop = true;



                //grid.MouseDown += MouseDrop;
             // System.Windows.Input.MouseEventArgs t = new() ;
                
                grid.MouseMove += ellipse_DragEnter;
                grid.MouseMove += (s, args) => { testetmetod(s, args);   };//--------------------

                grid.AllowDrop = true;
                grid.Name=$"grid{tester}";
                tester++;
                try
                {
                    grid.Children.Add(uIElement);
                    grid.Children.Add(uIElement2);
                }
                catch (Exception ex)
                {
                    string exs = ex.Message;
                    return;
                }
                if (panel != null)
                {
                     panel.MouseDown += (s, args) => { MouseD(grid, args, s); };
                  //  panel.MouseMove += (s, args) => { var g = t; testetmetod(s, t); };
                    panel.MouseUp += (s, args) => {   MouseUps(s, args); };
                    //panel.MouseMove += (s, args) => { testetmetod(s, args); };
                    // panel.AllowDrop = true;
                    // panel.PreviewKeyDown += (s,args)=> { dragEnter(grid, args); ; };
                    // panel.DragEnter += (s, args) => { dragEnter(s, args); };

                    //panel.AllowDrop = true;
                    // panel.AllowDrop = true;
                    // panel.DragDrop += (s, args) => { dragEnter(s, args); };

                    // panel.MouseDown += (s, args) => { MouseD(VideoHost, args, s); };
                    // panel.MouseDown += (s,args) => { dd(s, args); };
                    // panel.PreviewKeyDown += (s, args) => { ellipse_DragEnter(s, e);  };
                    //panel.Parent.MouseDown += (s, args) => { MouseD(s, args,s); };
                    //panel.Parent.AllowDrop = true;
                    // panel.Parent.DragEnter += (s, args) => { dragEnter(s, args); };
                    //panel.Parent.DragDrop +=  (s,arg)=>{ ellipse_Drag(s,arg); };
                    //panel.Parent.PreviewKeyDown += (s, args) => { dragEnter(s, args); };// ellipse_DragEnter(s, e); };
                    //panel.CreateControl();
                }


                //  string _videoSourceTest = VideoHost.Tag.ToString();
                //   //_videoSourceTest = $"rtsp://admin:123456@192.168.1.{test}:554/stream0?username=admin&password=E10ADC3949BA59ABBE56E057F20F883E";
                //   //    _videoSourceTest = $"http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
                //   string a = VideoHost.Name;

                // //  var wind = System.Windows.Application.Current.MainWindow;

                ////   var gr = wind.FindName("VideoCanvas1") as Grid;
                ////   VideoHost.Height = gr.ActualHeight;
                ////   VideoHost.Width = gr.ActualWidth;

                //  // if (gr.Children.Contains(VideoHost))
                //  // {
                //      // TestConsol.Program tests = new TestConsol.Program();
                //      // Task.Factory.StartNew(() => tests.Start(_videoSourceTest, a, VideoHost.Handle));
                // //  }
                //  // else
                //  // {
                //      // gr.Children.Add(VideoHost);

                //       TestConsol.Program tests = new TestConsol.Program();
                //       Task.Factory.StartNew(() => tests.Start(_videoSourceTest, a, VideoHostSelect.Handle));
                // //  }
                // string s = this.AssociatedType.FullName;


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

                    bool result = false;
                    for (int i = 0; i < Rite.ColumnDefinitions.Count; i++)
                    {


                        for (int j = 0; j < Rite.RowDefinitions.Count; j++)
                        {
                            var cellR = Rite.Children.OfType<UIElement>().Where(c => Grid.GetRow(c) == j && Grid.GetColumn(c) == i); // заполняется последняя строчка слева напрво 
                            if (cellR.FirstOrDefault() == null)
                            {

                                result = true; break;





                            }
                        }
                    }



                    if (result)// заполнение ячеек по порядку  если ячейка правая нижняя пустая
                    {

                        for (int i = 0; i < Rite.ColumnDefinitions.Count; i++)
                        {


                            for (int j = 0; j < Rite.RowDefinitions.Count; j++)
                            {
                                var cellR = Rite.Children.OfType<UIElement>().Where(c => Grid.GetRow(c) == j && Grid.GetColumn(c) == i); // заполняется последняя строчка слева напрво 
                                if (cellR.FirstOrDefault() == null)
                                {
                                    Grid.SetColumn(grid, i);
                                    Grid.SetRow(grid, j);


                                    Rite.Children.Add(grid);

                                    return;
                                }
                            }
                        }


                    }
                    else // тут проверка на заполненность последней ячеки правой нижней
                    {
                        Rite.RowDefinitions.Add(new RowDefinition());
                        Rite.ColumnDefinitions.Add(new ColumnDefinition());
                        Grid.SetRow(grid, Rite.RowDefinitions.Count - 1);


                        Rite.Children.Add(grid);

                    }

                }
                string _videoSourceTest = VideoHost.Tag.ToString();
                string a = VideoHost.Name;
                nint RenderTargetHwnd = panel.Handle;
                RenderANDVideoReaderVIdeoDecoder.Program tests = new RenderANDVideoReaderVIdeoDecoder.Program();
                Task.Factory.StartNew(() => tests.Start(_videoSourceTest, a, RenderTargetHwnd));


            }
        }

       

        private void ButtonDeleteImage(object sender, RoutedEventArgs e)
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
                    Rite.ColumnDefinitions.RemoveAt(Rite.ColumnDefinitions.Count - 1);

                }

            }


        }
    }
}
public class ImageDragDrop
{
    public WindowsFormsHost HostTake { get; set; }//panel??
    public WindowsFormsHost HostChange { get; set; }
    public Grid GridChange { get; set; }
    public Grid GridTake { get; set; }
}