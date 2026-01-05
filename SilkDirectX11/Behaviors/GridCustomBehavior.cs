using DevExpress.CodeParser;
using LibraryForSignalR;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Xaml.Behaviors;
using RenderANDVideoReaderVIdeoDecoder;
using SilkDirectX11.Model;
using SilkDirectX11.Servise;
using SilkDirectX11.SignalR;
using SilkDirectX11.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
    System.Windows.Window _windowOverlay = new();
     static string patch = Path.Combine(GetSolutionParentPath(), "Rend", "RenderANDVideoReaderVIdeoDecoder", "RenderANDVideoReaderVIdeoDecoder", "bin", "Debug", "net8.0", "RenderANDVideoReaderVIdeoDecoder.exe");
    bool _flagForOverlay;
    CameraDragDrop _cameraDragDrop = new CameraDragDrop();
    PositionCanvasForZoom _pixelPanelForZoom = new PositionCanvasForZoom();
    private IEventAggregator _eventAggregator;
    private bool _flagForReconnect=false;
    protected override void OnAttached()
    {
        base.OnAttached();
        
       // _connection = ConnectedManager._connection;
        //_ = EnsureSignalRAsync();

        AssociatedObject.PreviewDragEnter += MouseMoveDragDrop;
      
        AssociatedObject.Drop += AssociatedObject_Drop;
       
        InitOverlayWindow();
          EventAggregatorProvider.Instance.Subscribe<MassegeFromModul>(OnDialogMessegeReceived);
        EventAggregatorProvider.Instance.Subscribe<MessageClousedModul>(OnModuleClouse);
        EventAggregatorProvider.Instance.Subscribe<List<Filters>>(ChengeSettingsCamera);
        var ev = ContainerLocator.Container.Resolve<IEventAggregator>();
        _eventAggregator = ev;
        AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;
        EventAggregatorProvider.Instance.Subscribe<bool>((s)=> _flagForReconnect= s);
        // EventAggregatorProvider.Instance.Subscribe<SetSize>(SendSetSize);

    }

    private void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Grid rite = sender as Grid;
        var chengeWindVis =  rite.Children.OfType<CustomGrid>().ToList();
        bool vis = (bool)e.NewValue;
        foreach (var gr in chengeWindVis)
        {
            if (vis)
            { 
                if (gr.Window != null)
                    gr.Window.Visibility = Visibility.Visible;
            }
            else
            {
                if (gr.Window!=null)
                gr.Window.Visibility = Visibility.Hidden;
            }
        }
    }
    bool isOpen = false;
    private async void ShowMessage(string name , string messege)
    {
        Grid grid = AssociatedObject as Grid;
        var metroWindow = System.Windows.Application.Current.MainWindow as MetroWindow;
        var VM = metroWindow.DataContext as MainViewModel;
        if (isOpen)
        {
            isOpen = true;
            VM.OnMessageToReconect(messege);
            
        }else
        {
            isOpen=true;
            VM.CameraReconnects(name);
         
        }
        
    }
   
    private void ReconnectCamera()
    {
    
        var metroWindow = System.Windows.Application.Current.MainWindow as MetroWindow;
        var VM = metroWindow.DataContext as MainViewModel;
        isOpen = false;
        VM.OnReconnectCamera(false);
        


    }

    private void OnModuleClouse(MessageClousedModul obj)
    {       
            System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
            {
                isOpen = false;
                CLouseCamera(obj);
            });
    }

    private void CLouseCamera(MessageClousedModul obj)
    {
        Grid grid = AssociatedObject as Grid;
        var metroWindow = System.Windows.Application.Current.MainWindow as MetroWindow;
        var VM = metroWindow.DataContext as MainViewModel;
        VM.OnClouseCamera(obj.Message);
       var remuvGrid = grid.Children.OfType<CustomGrid>().Where(c => c.CameraGuidName == obj.IDprocces).FirstOrDefault();
        if (remuvGrid.Children.Count!=0)
        remuvGrid.Children.Clear();
        remuvGrid.Window.Close();
        grid.Children.Remove(remuvGrid);
          
    }


    private void OnDialogMessegeReceived(MassegeFromModul obj)
    {
        if (obj.Message != "reopened")
        System.Windows.Application.Current.Dispatcher.BeginInvoke(() => 
        {
            ShowMessage(obj.ModulName,obj.Message); 
        });
        else
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
            {
                ReconnectCamera();
            });
        }

    }

    //private async Task EnsureSignalRAsync()
    //{
    //    try
    //    {
    //        _connection ??= new HubConnectionBuilder()
    //            .WithUrl("http://localhost:5178/hubs/points")
    //            .WithAutomaticReconnect()
    //            .Build();
    //        if (_connection.State != HubConnectionState.Connected)
    //            await _connection.StartAsync();
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"SignalR connect error: {ex.Message}");
    //    }
    //}

    

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
    


    private void ChengeSettingsCamera(List<Filters> settingsForCamera)
    {
        var grid = AssociatedObject as Grid;
        
        if (grid != null)
        {
            foreach (var it in settingsForCamera)
            {
                var grids = ((grid.Children.OfType<CustomGrid>().Where(c => (c.CameraConnectStrings.CameraID == it.CameraId)).ToList()) ) ;
                if (grids != null)
                {
                    foreach (var pan in grids)
                    {
                            CameraSettingsVisual cameraSettingsVisual = new CameraSettingsVisual(pan.ProcessTag, it.Brightness, it.Contrast, it.Hue, it.Saturation, it.NoiseReduction, it.EdgeEnhancement, it.AnamorphicScaling, it.StereoAdjustment, it.Rotation);
                            
                            ConnectedManager.SendSettingsToGroup(cameraSettingsVisual);
                        //SendSettingsToGroup(cameraSettingsVisual);
                    }

                }
            }
        }
    }

    //private  async void SendSettingsToGroup(CameraSettingsVisual settingsForCamera)
    //{
    //    if (_connection == null) return;
    //    try
    //    {
    //        await _connection.InvokeAsync("ChangeSettingsCamera", settingsForCamera);  
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine($" error: {ex.Message}");
    //    }
    //}




    //private async void Rectangle_MouseMove_SendPoint(string groupId, PointsForZoom pointsForZoom) 
    //{
    //    if (_connection == null) return;
    //    try
    //    {
    //        await _connection.InvokeAsync("SendPointToGroup", groupId, pointsForZoom);  
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine($" error: {ex.Message}");
    //    }
    //}

    //private async void SendWindowForZoom(OpenZoom openZoom) 
    //{
    //    if (_connection == null) return;
    //    try
    //    {
    //        await _connection.InvokeAsync("SendZoomToGroup", openZoom); 
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine($" error: {ex.Message}");
    //    }
    //}
    //private async void SendChandeConekting(SetConnect setConnect) 
    //{
    //    if (_connection == null) return;
    //    try
    //    {
    //        await _connection.InvokeAsync("SendSetConToGroup", setConnect);
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine($" error: {ex.Message}");
    //    }
    //}
    //private async void SendSetSize(SetSize setSize)
    //{
    //    if (_connection == null) return;
    //    try
    //    {

    //        await _connection.InvokeAsync("SenNewSizeToGroup", setSize);
    //             }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine($" error: {ex.Message}");
    //    }
    //}
    

    
   

}