using LibraryForSignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkDirectX11.SignalR
{
    public static class ConnectedManager
    {
        public static HubConnection? _connection;
        public static async Task EnsureSignalRAsync()
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

        public static async void SendSettingsToGroup(CameraSettingsVisual settingsForCamera)
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




        public static async void Rectangle_MouseMove_SendPoint(string groupId, PointsForZoom pointsForZoom)
        {
            if (_connection == null) return;
            try
            {
                await _connection.InvokeAsync("SendPointToGroup", groupId, pointsForZoom);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($" error: {ex.Message}");
            }
        }

        public static async void SendWindowForZoom(OpenZoom openZoom)
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
        public static async void SendChandeConekting(SetConnect setConnect)
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
        public static async void SendSetSize(SetSize setSize)
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
        public static async void SendRestartCamersMessageAsync(ShowRestartCamersMessage restartCamersMessage)
        {
            if (_connection == null) return;
            
                try
                {
                    await _connection.InvokeAsync("ShowRestartingCamers", restartCamersMessage);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error sending message: {ex.Message}");
                }
             
        }
    }
}
