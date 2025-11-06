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
    }
}
