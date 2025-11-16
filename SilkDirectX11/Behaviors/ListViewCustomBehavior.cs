using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;

namespace SilkDirectX11.Behaviors
{
    class ListViewCustomBehavior : Behavior<System.Windows.Controls.Label>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseDown += MouseDown;


        }
        private void MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton != System.Windows.Input.MouseButtonState.Pressed)
                return;
            System.Windows.Controls.Label name = sender as System.Windows.Controls.Label;
            var listBox = System.Windows.Application.Current.MainWindow.FindName("ListViewCameras") as System.Windows.Controls.ListBox;
            WindowsFormsHost child = listBox.ItemsSource.Cast<WindowsFormsHost>().FirstOrDefault(x => x.Name == name.Content);
            if (child == null)
                return;
            WindowsFormsHost host = child  ;
            DragDrop.DoDragDrop(host, host, System.Windows.DragDropEffects.Move);

        }
    }
}
