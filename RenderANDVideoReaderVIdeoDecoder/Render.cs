using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Vortice;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
namespace RenderANDVideoReaderVIdeoDecoder
{
    public class Render
    {
        [DllImport("user32.dll", SetLastError = true)] static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        struct RECT { public int Left; public int Top; public int Right; public int Bottom; }

        private ID3D11Device _device;
        private ID3D11DeviceContext _context;
        private IDXGISwapChain _swapChain;

       static int _width;
        static int _height;



        public void Init(int width, int height, string name, nint testWind)
        {
            if (GetClientRect((IntPtr)testWind, out RECT rc1))
            {
                int cw = Math.Max(1, rc1.Right - rc1.Left);//rc.Right - rc.Left
                int ch = Math.Max(1, rc1.Bottom - rc1.Top);// rc.Bottom - rc.Top

                _height = ch;
                _width = cw;

            }
            ID3D11DeviceContext iD3D11DeviceContext = null;
            FeatureLevel? featureLevel;
            SwapChainDescription swapChainDesc = new SwapChainDescription
            {
                BufferCount = 1,
                OutputWindow = testWind,
                BufferUsage = Vortice.DXGI.Usage.RenderTargetOutput,
                BufferDescription = new Vortice.DXGI.ModeDescription((uint)_width, (uint)_height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                Flags = SwapChainFlags.None
            };
            //D3D11.D3D11CreateDeviceAndSwapChain( null, DriverType.Hardware, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_11_1,
            //    swapChainDesc, out _swapChain  ,out _device,out featureLevel, out iD3D11DeviceContext);
           // D3D11.D3D11CreateDevice( );  ///
            _context = _device.ImmediateContext;

        }
        
    }
}
