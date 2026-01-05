using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using LibraryForSignalR;
using Vortice.Mathematics;
using SilkDirectX11;
using SilkDirectX11.Servise;
namespace SilkDirectX11.SignalR
{
    public class PointsHub : Hub
    {
       
        public Task JoinGroup(string groupId) 
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        }

        public Task SendPointToGroup(string groupId, PointsForZoom pointsForZoom)
        {
             
                return Clients.Group(groupId).SendAsync("point", pointsForZoom);
            
        }

        public Task SendZoomToGroup(  OpenZoom openZoom)
        {
            return Clients.Group(openZoom.ID.ToString()).SendAsync("swapCain", openZoom);
        }

        public Task SendSetConToGroup(  SetConnect setConnect)
        {
            return Clients.Group(setConnect.ID.ToString()).SendAsync("conecting", setConnect);
        }

        public Task SenNewSizeToGroup(SetSize setSize)
        {
            return Clients.Group(setSize.ID.ToString()).SendAsync("NewSize", setSize);
        }
        public   void ClosingModulRender(MessageClousedModul message)
        { 
            EventAggregatorProvider.Instance.Publish(message);  
        }

        public   void MassegeFromModul(MassegeFromModul message)
        { 
            EventAggregatorProvider.Instance.Publish(message); 
        }
        public Task ChangeSettingsCamera(CameraSettingsVisual cameraSettingsVisual)
        { 
            return Clients.Group(cameraSettingsVisual.IDprocces).SendAsync("ChangeSettings", cameraSettingsVisual);
        }

        public Task ShowRestartingCamers(ShowRestartCamersMessage restartCamersMessage)
        {
            return Clients.All.SendAsync("Flag", restartCamersMessage);
        }
         
    } 
}
