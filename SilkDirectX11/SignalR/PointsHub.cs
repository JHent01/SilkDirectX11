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
        public Task SendZoom(bool usSwapCain, int wind,int ID)
        {
            return Clients.All.SendAsync("swapCain", usSwapCain, wind,ID); 
        }
        public Task SendSetCon(nint wind, string url, /*int width, int height,*/int ID)
        {
            return Clients.All.SendAsync("conecting", wind, url, /*width, height,*/ID); 
        }
    }
}
