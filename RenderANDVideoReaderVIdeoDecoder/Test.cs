using System;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using ID3D11Device = Vortice.Direct3D11.ID3D11Device;
using ID3D11DeviceContext = Vortice.Direct3D11.ID3D11DeviceContext;
using ID3D11Texture2D = Vortice.Direct3D11.ID3D11Texture2D;

namespace RenderANDVideoReaderVIdeoDecoder
{
    public class Test : IDisposable
    {
        
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left; public int Top; public int Right; public int Bottom; }

        private ID3D11Device _device;
        private ID3D11DeviceContext _context;
        private IDXGISwapChain _swapChain;
        private ID3D11RenderTargetView _rtv;

        private int _bbWidth;
        private int _bbHeight;

        // FFmpeg SWS
        private unsafe SwsContext* _swsCtx;
        private int _frameWidth;
        private int _frameHeight;
        private AVPixelFormat _srcPixFmt = AVPixelFormat.AV_PIX_FMT_NONE;

        // Буфер для BGRA-конверсии
        private byte[] _bgraBuffer;
        private int _bgraStride; // bytes per row

        public unsafe void Init(int width, int height, string name, nint testWind, AVFrame Frame)
        {
             
            if (GetClientRect((IntPtr)testWind, out RECT rc))
            {
                _bbWidth = Math.Max(1, rc.Right - rc.Left);
                _bbHeight = Math.Max(1, rc.Bottom - rc.Top);
            }
            else
            {
                _bbWidth = Math.Max(1, width);
                _bbHeight = Math.Max(1, height);
            }

       
            var swapChainDesc = new SwapChainDescription
            {
                BufferCount = 2,
                OutputWindow = testWind,
                BufferUsage = Usage.RenderTargetOutput,
                BufferDescription = new ModeDescription((uint)_bbWidth, (uint)_bbHeight, new Rational(60, 1), Format.B8G8R8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                Flags = SwapChainFlags.None
            };

            FeatureLevel[] requestedFeatureLevels =
            {
                FeatureLevel.Level_11_1,
                FeatureLevel.Level_11_0,
                FeatureLevel.Level_10_1,
                FeatureLevel.Level_10_0
            };

            var creationFlags = DeviceCreationFlags.BgraSupport;

#if DEBUG
            creationFlags |= DeviceCreationFlags.Debug;
#endif

            FeatureLevel? createdLevel;
            var hr = D3D11.D3D11CreateDeviceAndSwapChain(
                null,
                DriverType.Hardware,
                creationFlags,
                requestedFeatureLevels,
                swapChainDesc,
                out _swapChain,
                out _device,
                out  createdLevel,
                out _context

            );
            hr.CheckError();

            _context = _device.ImmediateContext;

            CreateOrUpdateRTV();

            
            _frameWidth = Frame.width;
            _frameHeight = Frame.height;
            _srcPixFmt = (AVPixelFormat)Frame.format;

            EnsureSwsForFrame();

          
            _bgraStride = _frameWidth * 4;
            int bgraSize = _bgraStride * _frameHeight;
            _bgraBuffer = new byte[bgraSize];

             
            var viewport = new Vortice.Mathematics.Viewport(0, 0, _bbWidth, _bbHeight, 0, 1);
            _context.RSSetViewport(viewport);
        }

        public unsafe void PresentFrame(  AVFrame frame)
        {
            // if (frame.width != _frameWidth || frame.height != _frameHeight || (AVPixelFormat)frame.format != _srcPixFmt)
            //{
            //    _frameWidth = frame.width;
            //    _frameHeight = frame.height;
            //    _srcPixFmt = (AVPixelFormat)frame.format;

            //    EnsureSwsForFrame();

            //    _bgraStride = _frameWidth * 4;
            //    int bgraSize = _bgraStride * _frameHeight;
            //    if (_bgraBuffer == null || _bgraBuffer.Length != bgraSize)
            //        _bgraBuffer = new byte[bgraSize];
            //}

            
            ResizeIfNeeded();

            
            fixed (byte* dstPtr0 = _bgraBuffer)
            {
                byte*[] dstData = new byte*[4];
                int[] dstLinesize = new int[4];

                dstData[0] = dstPtr0;
                dstLinesize[0] = _bgraStride;
                dstData[1] = null;
                dstData[2] = null;
                dstData[3] = null;

                byte*[] srcData = new byte*[4];
                int[] srcLinesize = new int[4];

                for (int i = 0; i < 4; i++)
                {
                    srcData[i] = frame.data[(uint)i];
                    srcLinesize[i] = frame.linesize[(uint)i];
                }

                //int r = ffmpeg.sws_scale(
                //    _swsCtx,
                //    srcData, srcLinesize,
                //    0, _frameHeight,
                //    dstData, dstLinesize
                //);


            }
             
            using (var backBuffer = _swapChain.GetBuffer<ID3D11Texture2D>(0))
            {
                 _context.UpdateSubresource(_bgraBuffer, backBuffer, 0, (uint)_bgraStride, 0);
                
                _swapChain.Present(1, PresentFlags.None);
            }
        }

        private void CreateOrUpdateRTV()
        {
            _rtv?.Dispose();
            using var backBuffer = _swapChain.GetBuffer<ID3D11Texture2D>(0);
            _rtv = _device.CreateRenderTargetView(backBuffer);
            _context.OMSetRenderTargets(_rtv);
        }

        private unsafe void EnsureSwsForFrame()
        {
            if (_frameWidth <= 0 || _frameHeight <= 0)
                return;

            // Освободить старый контекст, если был
            if (_swsCtx != null)
            {
                ffmpeg.sws_freeContext(_swsCtx);
                _swsCtx = null;
            }

            _swsCtx = ffmpeg.sws_getContext(
                _frameWidth, _frameHeight, _srcPixFmt,
                _frameWidth, _frameHeight, AVPixelFormat.AV_PIX_FMT_BGRA,
                ffmpeg.SWS_BILINEAR, null, null, null
            );
        }

        private void ResizeIfNeeded()
        {
            if (!GetClientRect(_swapChain.Description.OutputWindow, out RECT rc)) return;

            int newW = Math.Max(1, rc.Right - rc.Left);
            int newH = Math.Max(1, rc.Bottom - rc.Top);

            if (newW == _bbWidth && newH == _bbHeight) return;

            _bbWidth = newW;
            _bbHeight = newH;

            _context.OMSetRenderTargets(Array.Empty<ID3D11RenderTargetView>());

            _rtv?.Dispose();
            _rtv = null;

            _swapChain.ResizeBuffers(0, (uint)_bbWidth, (uint)_bbHeight, Format.B8G8R8A8_UNorm, SwapChainFlags.None);
            CreateOrUpdateRTV();

            var viewport = new Vortice.Mathematics.Viewport(0, 0, _bbWidth, _bbHeight, 0, 1);
            _context.RSSetViewport(viewport);
        }

        public void Dispose()
        {
            unsafe
            {
                if (_swsCtx != null)
                {
                    ffmpeg.sws_freeContext(_swsCtx);
                    _swsCtx = null;
                }
            }

            _rtv?.Dispose();
            _swapChain?.Dispose();
            _context?.Dispose();
            _device?.Dispose();
        }
    }
}
