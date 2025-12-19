using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using LibraryForSignalR;
using Vortice.Mathematics;
using SilkDirectX11;
using SilkDirectX11.Events;
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
            //MessageBox.Show(message.Message);
            EventAggregatorProvider.Instance.Publish(message);
          //  return Clients.All.SendAsync("ClosingModulRender", message);
        }

        public   void MassegeFromModul(MassegeFromModul message)
        {
            
            // MessageBox.Show(message.Message);
            EventAggregatorProvider.Instance.Publish(message);
           // return Clients.All.SendAsync("MassegeFromModul", message);
        }




        //public Task SendPoint(PointsForZoom pointsForZoom)//double xTL, double yTL, double xBR, double yBR
        //{
        //    return Clients.All.SendAsync("point", pointsForZoom);//pointReceived
        //}
        //public Task SendZoom(OpenZoom openZoom)//bool usSwapCain, int wind,int w , int h,int ID
        //{
        //    return Clients.All.SendAsync("swapCain", openZoom); 
        //}
        //public Task SendSetCon(SetConnect setConnect)//bool Use ,int ID
        //{
        //    return Clients.All.SendAsync("conecting", setConnect); 
        //}
        //public Task SenNewSize(SetSize setSize)
        //{
        //    return Clients.All.SendAsync("NewSize", setSize);//width, height, ID

        //}


    } 
}
