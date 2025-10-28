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
        private int _frameWidth;
        private int _frameHeight;
        private FFmpeg.AutoGen.AVPixelFormat _srcPixFmt = FFmpeg.AutoGen.AVPixelFormat.AV_PIX_FMT_NONE;

        static int _width;
        static int _height;

        // Буфер для BGRA-конверсии
        private byte[] _bgraBuffer;
        private int _bgraStride;
        private unsafe FFmpeg.AutoGen.SwsContext* _swsCtx;
        public unsafe void Init(int width, int height, string name, nint testWind, FFmpeg.AutoGen.AVFrame frame)
        {
            if (GetClientRect((IntPtr)testWind, out RECT rc1))
            {
                int cw = Math.Max(1, rc1.Right - rc1.Left);//rc.Right - rc.Left
                int ch = Math.Max(1, rc1.Bottom - rc1.Top);// rc.Bottom - rc.Top

                _height = ch;
                _width = cw;

            }
          //  ID3D11DeviceContext iD3D11DeviceContext = null;
            FeatureLevel? featureLevel;
            var swapChainDesc = new SwapChainDescription
            {
                BufferCount = 2,
                OutputWindow = testWind,
                BufferUsage = Usage.RenderTargetOutput,
                BufferDescription = new ModeDescription((uint)_width, (uint)_height, new Rational(60, 1), Format.B8G8R8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                Flags = SwapChainFlags.None
            };
            FeatureLevel[] featureLevels =
           {
                FeatureLevel.Level_11_1,
                FeatureLevel.Level_11_0,
                FeatureLevel.Level_10_1,
                FeatureLevel.Level_10_0
            };
            DriverType driverType;  
            
            driverType = DriverType.Hardware;

            var creationFlags = DeviceCreationFlags.BgraSupport;
            //creationFlags |= DeviceCreationFlags.Debug;
            FeatureLevel createdLevel;
            var hr = D3D11.D3D11CreateDevice(driverType, creationFlags,featureLevels);
            
            _context = hr.ImmediateContext;



             
          //  _frameWidth = frame.width;
          //  _frameHeight = frame.height;
          //  _srcPixFmt = (FFmpeg.AutoGen.AVPixelFormat)frame.format;

          ////  EnsureSwsForFrame();
           
          //  _bgraStride = _frameWidth * 4;
          //  int bgraSize = _bgraStride * _frameHeight;
          //  _bgraBuffer = new byte[bgraSize];
 
          //  var viewport = new Vortice.Mathematics.Viewport(0, 0, _width, _height, 0, 1);
          //  _context.RSSetViewport(viewport);

          //  //----
          //  _swsCtx = FFmpeg.AutoGen.ffmpeg.sws_getContext(
          //     _frameWidth, _frameHeight, _srcPixFmt,
          //     _frameWidth, _frameHeight, FFmpeg.AutoGen.AVPixelFormat.AV_PIX_FMT_BGRA,
          //     FFmpeg.AutoGen.ffmpeg.SWS_BILINEAR, null, null, null);
          //  fixed (byte* dstPtr0 = _bgraBuffer)
          //  {
          //      byte*[] dstData = new byte*[4];
          //      int[] dstLinesize = new int[4];

          //      dstData[0] = dstPtr0;
          //      dstLinesize[0] = _bgraStride;
          //      dstData[1] = null;
          //      dstData[2] = null;
          //      dstData[3] = null;

          //      byte*[] srcData = new byte*[4];
          //      int[] srcLinesize = new int[4];

          //      //for (int i = 0; i < 4; i++)
          //      //{
          //      //    srcData[i] = frame.data[i];
          //      //    srcLinesize[i] = frame.linesize[i];
          //      //}

          //      int r = FFmpeg.AutoGen.ffmpeg.sws_scale(
          //          _swsCtx,
          //          srcData, srcLinesize,
          //          0, _frameHeight,
          //          dstData, dstLinesize
          //      );
          //      using (var backBuffer = _swapChain.GetBuffer<ID3D11Texture2D>(0))
          //      {
          //          fixed (byte* pData = _bgraBuffer)
          //          {
          //             // _context.UpdateSubresource(backBuffer, 0, null, (IntPtr)pData, _bgraStride, 0);
          //          }

                     
                     
          //          _swapChain.Present(1, PresentFlags.None);
          //      }

            //}

        }
        
    }
}
