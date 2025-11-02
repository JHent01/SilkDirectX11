using DevExpress.DirectX.Common.DirectWrite;
using FFmpeg.AutoGen;
using System;
using System.Runtime.InteropServices;
using Vortice.D3DCompiler;
using Vortice.Direct2D1;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DirectWrite;
using Vortice.DXGI;
using Vortice.Mathematics;
using static DevExpress.Data.Filtering.Helpers.SubExprHelper.ThreadHoppingFiltering;
using FeatureLevel = Vortice.Direct3D.FeatureLevel;
using Filter = Vortice.Direct3D11.Filter;
using ID3D11Device = Vortice.Direct3D11.ID3D11Device;
using ID3D11DeviceContext = Vortice.Direct3D11.ID3D11DeviceContext;
using ID3D11Texture2D = Vortice.Direct3D11.ID3D11Texture2D;
using InputElementDescription = Vortice.Direct3D11.InputElementDescription;

namespace RenderANDVideoReaderVIdeoDecoder
{
    public class Test  
    {
        private IDXGIFactory1 _factory;



        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left; public int Top; public int Right; public int Bottom; }

        private ID3D11Device _device;
        private ID3D11DeviceContext _context;
        private IDXGISwapChain _swapChain;
        private ID3D11RenderTargetView _rtv;

        private int _width;
        private int _height;
        private int _bufWidth;
        private int _bufHeight;

        private unsafe SwsContext* _swsCtx;
        private int _frameWidth;
        private int _frameHeight;
        private AVPixelFormat _srcPixFmt = AVPixelFormat.AV_PIX_FMT_NONE;

         
        private byte[] _bgraBuffer;
        private int _bgraStride;  



        private ID3D11Texture2D _texY;
        private ID3D11Texture2D _texUV;
        private ID3D11ShaderResourceView _srvY;
        private ID3D11ShaderResourceView _srvUV;
        private ID3D11SamplerState _sampler;
        private ID3D11VertexShader _vs;
        private ID3D11PixelShader _ps;
        private ID3D11InputLayout _inputLayout;
        private ID3D11Buffer _vb;
        nint _wind;


        private ID2D1Factory1 _d2dFactory;
        private IDWriteFactory _dwFactory;
        private ID2D1Device _d2dDevice;
        private ID2D1DeviceContext _d2dContext;
        private ID2D1Bitmap1 _d2dTarget;
        private ID2D1SolidColorBrush _textBrush;
        private IDWriteTextFormat _textFormat;
        private string _overlayText  ;
        public unsafe void Init(int width, int height, string name, nint testWind, AVFrame Frame)
        {   _overlayText = name;
            //if (GetClientRect((IntPtr)testWind, out RECT rc))
            //{
            //    _width = Math.Max(1, rc.Right - rc.Left);
            //    _height = Math.Max(1, rc.Bottom - rc.Top);
            //}
           _wind= testWind;
            _width = width;
            _height = height;
            SwapChainDescription swapChainDesc = new SwapChainDescription
            {
                BufferCount = 1,
                OutputWindow = testWind,
                BufferUsage = Usage.RenderTargetOutput,
                BufferDescription = new ModeDescription((uint)_width, (uint)_height, new Rational(60, 1), Format.B8G8R8A8_UNorm),
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

            _factory = DXGI.CreateDXGIFactory1<IDXGIFactory1>();
            var creationFlags = DeviceCreationFlags.BgraSupport;
            _device = Vortice.Direct3D11.D3D11.D3D11CreateDevice(DriverType.Hardware, creationFlags, requestedFeatureLevels);

           // _device = Vortice.Direct3D11.D3D11.D3D11CreateDevice(DriverType.Hardware, DeviceCreationFlags.None, requestedFeatureLevels);
            _swapChain = _factory.CreateSwapChain(_device, swapChainDesc);
            _context = _device.ImmediateContext;
            
            //CreateOrUpdateD2DTarget();
            CreateOrUpdateRTV();
            InitText();

            //var viewport = new Vortice.Mathematics.Viewport(0, 0, width, height, 0, 1);
            //_context.RSSetViewport(viewport);

            _frameWidth = Frame.width;
            _frameHeight = Frame.height;
            _srcPixFmt = (AVPixelFormat)Frame.format;

            
            Resources(_frameWidth, _frameHeight);
            Shader();

        }
        public unsafe void PresentFrame(AVFrame frame)
        {
            
            if (frame.width != _frameWidth || frame.height != _frameHeight || (AVPixelFormat)frame.format != _srcPixFmt)
            {
                _frameWidth = frame.width;
                _frameHeight = frame.height;
                _srcPixFmt = (AVPixelFormat)frame.format;

                //DisposeResources();
                Resources(_frameWidth, _frameHeight);
            }
           
            //_context.OMSetRenderTargets(_rtv);
            //_context.ClearRenderTargetView(_rtv, new Vortice.Mathematics.Color4(0f, 0f, 0f, 1f)); // чёрный фон

            //if (GetClientRect((IntPtr)_wind, out RECT rc))
            //{
            //    _width = Math.Max(1, rc.Right - rc.Left);
            //    _height = Math.Max(1, rc.Bottom - rc.Top);
            //}

            // Resize();


            int w = _frameWidth;
            int h = _frameHeight;
            int uvW = (w + 1) >> 1;
            int uvH = (h + 1) >> 1;

           
            _context.UpdateSubresource(_texY, 0, null, (nint)frame.data[0], (uint)frame.linesize[0], 0);
            _context.UpdateSubresource(_texUV, 0, null, (nint)frame.data[1], (uint)frame.linesize[1], 0);

            _context.OMSetRenderTargets(_rtv);

            var viewport = new Vortice.Mathematics.Viewport(0, 0, _width, _height, 0, 1);
            _context.RSSetViewport(viewport);

            _context.IASetInputLayout(_inputLayout);
            _context.IASetPrimitiveTopology(PrimitiveTopology.TriangleStrip);
            int stride = sizeof(float) * 4;
            int offset = 0;
            _context.IASetVertexBuffers(
                0,
                new ID3D11Buffer[] { _vb },
                new uint[] { (uint)stride },
                new uint[] { (uint)offset }
            );
            _context.VSSetShader(_vs);
            _context.PSSetShader(_ps);
            _context.PSSetShaderResources(0, new ID3D11ShaderResourceView[] { _srvY });
            _context.PSSetShaderResources(1, new ID3D11ShaderResourceView[] { _srvUV });

            _context.PSSetSamplers(0, new ID3D11SamplerState[] { _sampler });

            _context.Draw(4, 0);

             UpdateText();
            _d2dContext.BeginDraw();
            var textRect = new Rect(0, 0, _width, _height);
              _d2dContext.DrawText(_overlayText , _textFormat, textRect, _textBrush);
            _d2dContext.EndDraw();
            _context.Flush();

            GetClientRect(_wind, out RECT rc);

            int newW = rc.Right - rc.Left;
            int newH = rc.Bottom - rc.Top;

            if (newW != _bufWidth && newH != _bufHeight)
                _swapChain.ResizeBuffers(0, (uint)newW, (uint)newH, Format.B8G8R8A8_UNorm, SwapChainFlags.None);
            _swapChain.Present(0, PresentFlags.None);

            //Resize();
            //  var bp = new BitmapProperties1(
            //   new Vortice.DCommon.PixelFormat(Format.B8G8R8A8_UNorm, Vortice.DCommon.AlphaMode.Premultiplied),
            //    98f,
            //    98f,
            //    BitmapOptions.Target | BitmapOptions.CannotDraw
            //);
            // GetClientRect(_wind, out RECT rc);

            // int newW = Math.Max(1, rc.Right - rc.Left);
            // int newH = Math.Max(1, rc.Bottom - rc.Top);

            // var textRect = new Rect(0, 0, _width , _height  );
            // _d2dContext.DrawText(_overlayText , _textFormat, textRect, _textBrush);

            // _d2dContext.EndDraw();

            // var bp = new BitmapProperties1(
            //   new Vortice.DCommon.PixelFormat(Format.B8G8R8A8_UNorm, Vortice.DCommon.AlphaMode.Premultiplied),
            //    200,
            //   200,
            //    BitmapOptions.Target | BitmapOptions.CannotDraw
            //);

            // using var surface = _swapChain.GetBuffer<IDXGISurface>(0);
            // _d2dTarget = _d2dContext.CreateBitmapFromDxgiSurface(surface, bp);
            // _d2dContext.Target = _d2dTarget;

            //

            //_context.Flush();


            //_swapChain.Present(0, PresentFlags.None);

        }
        private void CreateOrUpdateRTV()//???
        {
            _rtv?.Dispose();
            using var backBuffer = _swapChain.GetBuffer<ID3D11Texture2D>(0);
            _rtv = _device.CreateRenderTargetView(_swapChain.GetBuffer<ID3D11Texture2D>(0));
            _context.OMSetRenderTargets(_rtv);
            _swapChain.Present(0, PresentFlags.None);
        }
        

        private void Resize()
        {
            
            GetClientRect(_wind, out RECT rc);
               
            int newW =  rc.Right - rc.Left;
            int newH =   rc.Bottom - rc.Top;

            if (newW == _bufWidth && newH == _bufHeight) return;

            //_bufWidth = newW;
            //_bufHeight = newH;

            //_context.OMSetRenderTargets(Array.Empty<ID3D11RenderTargetView>());

            //_rtv?.Dispose();
            //_rtv = null;
           
            _swapChain.ResizeBuffers(0, (uint)newW, (uint)newH, Format.B8G8R8A8_UNorm, SwapChainFlags.None);
            //_swapChain.Present(0, PresentFlags.None);
            //CreateOrUpdateRTV();
            //var bp = new BitmapProperties1(
            //       new Vortice.DCommon.PixelFormat(Format.B8G8R8A8_UNorm, Vortice.DCommon.AlphaMode.Premultiplied),
            //        96.0f,
            //        96.0f,
            //        BitmapOptions.Target | BitmapOptions.CannotDraw
            //    );
            //var viewport = new Vortice.Mathematics.Viewport(0, 0, newW, newH, 0, 1);
            //_context.RSSetViewport(viewport);
            //using var surface = _swapChain.GetBuffer<IDXGISurface>(0);
            //_d2dTarget = _d2dContext.CreateBitmapFromDxgiSurface(surface, bp);
            //_d2dContext.Target = _d2dTarget;

            //  CreateOrUpdateD2DTarget();

        }

        private void Resources(int width, int height)
        {
             
            var descY = new Texture2DDescription
            {
                Width = (uint)width,
                Height = (uint)height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.R8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.ShaderResource,
                CPUAccessFlags = CpuAccessFlags.None,
                MiscFlags = ResourceOptionFlags.None
            };
            _texY = _device.CreateTexture2D(descY);
            _srvY = _device.CreateShaderResourceView(_texY, new ShaderResourceViewDescription
            {
                Format = Format.R8_UNorm,
                ViewDimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = new Texture2DShaderResourceView { MipLevels = 1, MostDetailedMip = 0 }
            });

             
            var descUV = new Texture2DDescription
            {
                Width = (uint)Math.Max(1, width / 2),
                Height = (uint)Math.Max(1, height / 2),
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.R8G8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.ShaderResource,
                CPUAccessFlags = CpuAccessFlags.None,
                MiscFlags = ResourceOptionFlags.None
            };
            _texUV = _device.CreateTexture2D(descUV);
            _srvUV = _device.CreateShaderResourceView(_texUV, new ShaderResourceViewDescription
            {
                Format = Format.R8G8_UNorm,
                ViewDimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = new Texture2DShaderResourceView { MipLevels = 1, MostDetailedMip = 0 }
            });
        }

       

        private void Shader()//???
        {
             
            const string hlsl = @"
struct VSIn { float2 pos : POSITION; float2 uv : TEXCOORD0; };
struct VSOut { float4 pos : SV_Position; float2 uv : TEXCOORD0; };

VSOut VSMain(VSIn input)
{
    VSOut o;
    o.pos = float4(input.pos, 0.0, 1.0);
    o.uv = input.uv;
    return o;
}

Texture2D texY  : register(t0);
Texture2D texUV : register(t1);
SamplerState samLinear : register(s0);

float4 PSMain(VSOut input) : SV_Target
{
    float y  = texY.Sample(samLinear, input.uv).r;              // 0..1
    float2 uv = texUV.Sample(samLinear, input.uv).rg;           // 0..1, interleaved U=R, V=G
    float U = uv.x - 0.5;
    float V = uv.y - 0.5;

    // BT.709 full-range
    float3 rgb;
    rgb.r = y + 1.5748 * V;
    rgb.g = y - 0.1873 * U - 0.4681 * V;
    rgb.b = y + 1.8556 * U;

    return float4(saturate(rgb), 1.0);
}";
            Blob? vsBlob = null;
            Blob? psBlob = null;
            Blob? vsErr = null;
            Blob? psErr = null;

            Compiler.Compile(hlsl, null, "VSMain", "NV12.hlsl", "vs_5_0", out vsBlob, out vsErr);
               
            Compiler.Compile(hlsl, null, "PSMain", "NV12.hlsl", "ps_5_0", out psBlob, out psErr);
           
            _vs?.Dispose();
            _ps?.Dispose();
            _inputLayout?.Dispose();
            _vb?.Dispose();
            _sampler?.Dispose();

            _vs = _device.CreateVertexShader(vsBlob!);
            _ps = _device.CreatePixelShader(psBlob!);

            var inputElements = new[]//???
            {
                new InputElementDescription("POSITION", 0, Format.R32G32_Float, 0, 0),
                new InputElementDescription("TEXCOORD", 0, Format.R32G32_Float, 8, 0),
            };
            _inputLayout = _device.CreateInputLayout(inputElements, vsBlob);

             
            float[] vertices =
               {
                    -1f, -1f,  0f, 1f,
                    -1f,  1f,  0f, 0f,
                     1f, -1f,  1f, 1f,
                     1f,  1f,  1f, 0f,
                };

            var vbDesc = new BufferDescription
            {
                Usage = ResourceUsage.Immutable,
                BindFlags = BindFlags.VertexBuffer,
                CPUAccessFlags = CpuAccessFlags.None,
                ByteWidth = (uint)(vertices.Length * sizeof(float)),
                StructureByteStride = 0,
                MiscFlags = ResourceOptionFlags.None
            };
            unsafe
            {
                fixed (float* p = vertices)
                {
                    var initData = new SubresourceData((IntPtr)p, 0, 0);
                    _vb = _device.CreateBuffer(vbDesc, initData);
                }
            }
            var samplerDesc = new SamplerDescription
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                ComparisonFunc = ComparisonFunction.Never,
                MinLOD = 0,
                MaxLOD = float.MaxValue
            };
            _sampler = _device.CreateSamplerState(samplerDesc);
            vsErr?.Dispose();
            psErr?.Dispose();
            vsBlob?.Dispose();
            psBlob?.Dispose();
        }

        
         
        private void InitText()
        {
            _d2dFactory?.Dispose();
            _dwFactory?.Dispose();
            _d2dDevice?.Dispose();
            _d2dContext?.Dispose();
            _textBrush?.Dispose();
            _textFormat?.Dispose();

            _d2dFactory = D2D1.D2D1CreateFactory<ID2D1Factory1>();//Vortice.Direct2D1.FactoryType.SingleThreaded
            _dwFactory = DWrite.DWriteCreateFactory<IDWriteFactory>();//Vortice.DirectWrite.FactoryType.Shared
            using var dxgiDevice = _device.QueryInterface<IDXGIDevice>();
            _d2dDevice = _d2dFactory.CreateDevice(dxgiDevice);
            _d2dContext = _d2dDevice.CreateDeviceContext(DeviceContextOptions.None);

            _textFormat = _dwFactory.CreateTextFormat(
                "OMG",
                null,
                FontWeight.SemiBold,
                FontStyle.Normal,
                FontStretch.Normal,
                220
               //_width/10
            );
            _textFormat.TextAlignment = TextAlignment.Leading;
            _textFormat.ParagraphAlignment = ParagraphAlignment.Near;

            _textBrush = _d2dContext.CreateSolidColorBrush(new Color4(0.1f, 1f, 0f, 1f));
        }


        private void UpdateText()
        {


            if (_d2dContext == null || _swapChain == null) return;

            // using var surface = _swapChain.GetBuffer<IDXGISurface>(0);
            var bp = new BitmapProperties1(
               new Vortice.DCommon.PixelFormat(Format.B8G8R8A8_UNorm, Vortice.DCommon.AlphaMode.Premultiplied),
                96.0f,
                96.0f,
                BitmapOptions.Target | BitmapOptions.CannotDraw
            );

            using var surface = _swapChain.GetBuffer<IDXGISurface>(0);
            _d2dTarget = _d2dContext.CreateBitmapFromDxgiSurface(surface, bp);
            _d2dContext.Target = _d2dTarget;
        }

        // // ReleaseD2DTarget();
        // private void ReleaseD2DTarget()
        //{
        //    if (_d2dContext != null)
        //    {
        //        _d2dContext.Target = null;
        //    }
        //    _d2dTarget?.Dispose();
        //    _d2dTarget = null;
        //}
        //private void DisposeResources() 
        //{
        //    _srvY?.Dispose(); _srvY = null;
        //    _srvUV?.Dispose(); _srvUV = null;
        //    _texY?.Dispose(); _texY = null;
        //    _texUV?.Dispose(); _texUV = null;
        //}
    }
}
