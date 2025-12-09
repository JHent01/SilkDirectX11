using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SilkDirectX11.SignalR
{
    public class PointsHub : Hub
    {
         
        public Task SendPoint(double xTL, double yTL, double xBR, double yBR)
        {
            return Clients.All.SendAsync("point", xTL, yTL, xBR, yBR);//pointReceived
        }
        public Task SendZoom(bool usSwapCain, int wind,int w , int h,int ID)
        {
            return Clients.All.SendAsync("swapCain", usSwapCain, wind,w,h,ID); 
        }
        public Task SendSetCon(bool Use ,int ID)
        {
            return Clients.All.SendAsync("conecting", Use ,ID); 
        }
        public Task SenNewSize(int width,int height , int ID)
        {
            return Clients.All.SendAsync("NewSize", width, height, ID);
        }
    }//Microsoft.AspNetCore.SignalR.HubException: "Failed to invoke 'conecting' due to an error on the server. HubException: Method does not exist."

}
