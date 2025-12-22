using LibraryForSignalR;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Xaml.Behaviors;
using RenderANDVideoReaderVIdeoDecoder;
using SilkDirectX11.Events;
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
using System.Windows.Media.Media3D;
using Vortice.Direct2D1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using Button = System.Windows.Controls.Button;
using Image = System.Windows.Controls.Image;
using Panel = System.Windows.Forms.Panel;
using Point = System.Windows.Point;
using Window = System.Windows.Window;

namespace SilkDirectX11.Behaviors;

 partial class  GridCustomBehavior : Behavior<Grid>
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
          EventAggregatorProvider.Instance.Subscribe<MassegeFromModul>(DialogMessegeReceived);
        EventAggregatorProvider.Instance.Subscribe<MessageClousedModul>(ClouseModul);
        EventAggregatorProvider.Instance.Subscribe<List<CameraVisualSettings>>(ChengeSettingsCamera);
    }
    
    private async void Dialog(string name , string messege)
    {
        Grid grid = AssociatedObject as Grid;
        var metroWindow = System.Windows.Application.Current.MainWindow as MetroWindow;
        var VM = metroWindow.DataContext as MainViewModel;
        if (grid.Visibility==Visibility.Hidden)
        {
            VM.Message(messege);
            
        }else
        {
            //grid.Visibility = Visibility.Hidden;
            VM.CameraProgressBar(name);
         
        }
        
    }
   
    private void ReconCemera()
    {
       // Grid grid = AssociatedObject as Grid;
        var metroWindow = System.Windows.Application.Current.MainWindow as MetroWindow;
        var VM = metroWindow.DataContext as MainViewModel;
        //if (grid.Visibility == Visibility.Hidden)
        //{
            VM.SetProgressBar(false);
            // grid.Visibility = Visibility.Visible;
        //}
    }

    private void ClouseModul(MessageClousedModul obj)
    {
        
            System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
            {
                CLouseCamera(obj);

            });
      
    }

    private void CLouseCamera(MessageClousedModul obj)
    {
        Grid grid = AssociatedObject as Grid;
        var metroWindow = System.Windows.Application.Current.MainWindow as MetroWindow;
        var VM = metroWindow.DataContext as MainViewModel;
        VM.ClouseCamera(obj.Message);
       var remuvGrid = grid.Children.OfType<Grid>().Where(c => c.Name == obj.IDprocces).FirstOrDefault();
        if (remuvGrid.Children.Count!=0)
        remuvGrid.Children.Clear();
        grid.Children.Remove(remuvGrid);
         
       // grid.Visibility = Visibility.Visible;//??

    }


    private void DialogMessegeReceived(MassegeFromModul obj)
    {
        if (obj.Message != "reopened")
            System.Windows.Application.Current.Dispatcher.BeginInvoke(() => 
        {
            Dialog(obj.ModulName,obj.Message); 
        });
        else
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
            {
                ReconCemera();
            });
        }

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
    


    private void ChengeSettingsCamera(List<CameraVisualSettings> settingsForCamera)
    {
        var grid = AssociatedObject as Grid;
        
        if (grid != null)
        {

            foreach (var it in settingsForCamera)
            {
                // (grid.Children.OfType<Grid>().FirstOrDefault()?.Children.OfType<WindowsFormsHost>().FirstOrDefault().Where(c => (c.Tag as CameraConnectStrings).CameraID == it.CameraId));//BetterPanelTest
                //var g =((grid.Children.OfType<Grid>().FirstOrDefault().Children.OfType<WindowsFormsHost>().FirstOrDefault().Child.Tag) as CameraConnectStrings).CameraID == it.CameraId;

                var grids = ((grid.Children.OfType<Grid>().Where(c => (c.Children.OfType<WindowsFormsHost>().FirstOrDefault().Child as Panel).Tag is CameraConnectStrings cs && cs.CameraID == it.CameraId).ToList()) ) ;
                if (grids != null)
                {
                    foreach (var pan in grids)
                    {
                         
                        
                            CameraSettingsVisual cameraSettingsVisual = new CameraSettingsVisual(pan.Tag.ToString(), it.Brightness, it.Contrast, it.Hue, it.Saturation, it.NoiseReduction, it.EdgeEnhancement, it.AnamorphicScaling, it.StereoAdjustment, it.Rotation);
                            
                            SendSettingsToGroup(cameraSettingsVisual);
                         
                    }
                    //Grid p =  grid.Children.OfType<Grid>().Where(c => (c.Children.OfType<WindowsFormsHost>().FirstOrDefault().Child as Panel).Name == pan.Name).FirstOrDefault();
                    //CameraSettingsVisual cameraSettingsVisual = new CameraSettingsVisual(grids.Tag.ToString(), it.Brightness, it.Contrast, it.Hue, it.Saturation, it.NoiseReduction, it.EdgeEnhancement, it.AnamorphicScaling, it.StereoAdjustment, it.Rotation);
                    
                    //SendSettingsToGroup(cameraSettingsVisual);
                }




                   
            }
        }
    }

    private  async void SendSettingsToGroup(CameraSettingsVisual settingsForCamera)
    {
        if (_connection == null) return;
        try
        {
            await _connection.InvokeAsync("ChangeSettingsCamera", settingsForCamera);  
        }
        catch (Exception ex)
        {
            Debug.WriteLine($" error: {ex.Message}");
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

            await _connection.InvokeAsync("SenNewSizeToGroup", setSize);
                 }
        catch (Exception ex)
        {
            Debug.WriteLine($" error: {ex.Message}");
        }
    }
    

    
   

}