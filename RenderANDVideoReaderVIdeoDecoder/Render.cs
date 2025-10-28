using DevExpress.DirectX.Common.Direct3D;
using DevExpress.DirectX.NativeInterop.Direct3D;
using FFmpeg.AutoGen;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Silk.NET.Core;

//using SharpDX.Direct3D11;
//using SharpDX.DXGI;


//using SharpDX.Direct3D11;

//using SharpDX.DXGI;

//using SharpDX.Direct3D;
//using SharpDX.Direct3D11;
using Silk.NET.Core.Contexts;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using Silk.NET.GLFW;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Filter = Silk.NET.Direct3D11.Filter;
using Format = Silk.NET.DXGI.Format;
using ID3D11Device = Silk.NET.Direct3D11.ID3D11Device;
using ID3D11DeviceContext = Silk.NET.Direct3D11.ID3D11DeviceContext;
using ID3D11Texture2D = Silk.NET.Direct3D11.ID3D11Texture2D;
using InputClassification = Silk.NET.Direct3D11.InputClassification;
using Rational = Silk.NET.DXGI.Rational;
using SwapEffect = Silk.NET.DXGI.SwapEffect;
using TextureAddressMode = Silk.NET.Direct3D11.TextureAddressMode;
using Usage = Silk.NET.Direct3D11.Usage;

//using SharpDX.DXGI;


namespace RenderANDVideoReaderVIdeoDecoder
{
    public unsafe class Render
    {
        static IWindow _window;
        static D3D11 d3d11 = D3D11.GetApi();
        static DXGI dxgi = DXGI.GetApi();

        static ID3D11Device* device;
        static ID3D11DeviceContext* context;
        static IDXGIFactory* factory;
        static IDXGISwapChain* swapChain;
        static ID3D11RenderTargetView* rtv;

        static ID3D11Texture2D* texY;
        static ID3D11ShaderResourceView* srvY;
        static ID3D11Texture2D* texUV;
        static ID3D11ShaderResourceView* srvUV;

        static ID3D11VertexShader* vs;
        static ID3D11PixelShader* ps;
        static ID3D11InputLayout* inputLayout;
        static ID3D11Buffer* vb;
        static ID3D11SamplerState* sampler;

         static int _wight = 0;
        static int _height = 0;

         static object frameLock = new object();
        static bool haveFrame = false;
        static IntPtr frameY = IntPtr.Zero;
        static IntPtr frameUV = IntPtr.Zero;
        static int framePitchY = 0;
        static int framePitchUV = 0;
        static int frameWidth = 0;
        static int frameHeight = 0;

        [DllImport("user32.dll", SetLastError = true)] static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
        struct RECT { public int Left; public int Top; public int Right; public int Bottom; }
        //  [DllImport("D3DCompiler_47.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        //  static extern int D3DCompile(
        //byte* pSrcData, nuint SrcDataSize,
        //[MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pSourceName,
        //IntPtr pDefines,
        //IntPtr pInclude,
        //[MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pEntryPoint,
        //[MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pTarget,
        //uint Flags1, uint Flags2,
        //out ID3DBlob* ppCode,
        //out ID3DBlob* ppErrorMsgs);

         
        //  [StructLayout(LayoutKind.Sequential)]
        //  struct ID3DBlob { public IntPtr Ptr; }
        //  static IDXGIFactory* factory;
        //  // NV12 resources
        //  static ID3D11Texture2D* texY;
        //  static ID3D11ShaderResourceView* srvY;
        //  static ID3D11Texture2D* texUV;
        //  static ID3D11ShaderResourceView* srvUV;

       
        //  static ID3D11VertexShader* vs;
        //  static ID3D11PixelShader* ps;
        //  static ID3D11InputLayout* inputLayout;
        //  static ID3D11Buffer* vb;
        //  static ID3D11SamplerState* sampler;

         
        //  static int videoW = 640;
        //  static int videoH = 480;

         //  static volatile bool haveFrame = false;
        //  static Nv12Frame latestFrame;

         //  public struct Nv12Frame
        //  {
        //      public int Width;
        //      public int Height;
        //      public IntPtr Y;     
        //      public IntPtr UV;     
        //      public int PitchY;
        //      public int PitchUV;
        //  }
        //  private int _wight;
        //  private int _height;

        //  private static nint _hwnd;
        //  //private D3D11? _d3d11;

        //  //private IDXGISwapChain* _swapChain;
        //  //private ID3D11Device* _device;
        //  //private ID3D11DeviceContext* _context;

        //  //private ID3D11Texture2D* _frameTexture;
        //  //private ID3D11Texture2D* _backBuffer;
        //  // NV12 ресурсы
        //  //private ID3D11Texture2D* _nv12Y;
        //  //private ID3D11Texture2D* _nv12UV;
        //  //private ID3D11ShaderResourceView* _srvY;
        //  //private ID3D11ShaderResourceView* _srvUV;
        //  //private ID3D11SamplerState* _sampler;
        //  //private ID3D11VertexShader* _vs;
        //  //private ID3D11PixelShader* _ps;
        //  //private ID3D11RenderTargetView* _rtv;
        //  //D3DCompiler

        //  static IWindow _window;
        //  static D3D11 _d3d11;
        //  //static DXGI _dxgi;

        //  static ID3D11Device* _device;
        //  static ID3D11DeviceContext* _context;


        //  static IDXGISwapChain* _swapChain;
        //  private static ID3D11RenderTargetView* _rtv;

        // static ID3D11RenderTargetView* _rtv;


        //  static ID3D11Texture2D* _texY;
        //  static ID3D11ShaderResourceView* _srvY;
        //  static ID3D11Texture2D* _texUV;
        //   static ID3D11ShaderResourceView* _srvUV;


        // static ID3D11VertexShader* _vs;
        //   static ID3D11PixelShader* _ps;
        //   static ID3D11InputLayout* _inputLayout;
        //  static ID3D11Buffer* _vb;
        //   static ID3D11SamplerState* _sampler;





        //private ID3D11ComputeShader?  _compiler;
        public unsafe void Init(int width, int height, string name, nint testWind)
        {
            //_hwnd = testWind;
            if (GetClientRect((IntPtr)testWind, out RECT rc1))
            {
                int cw = Math.Max(1, rc1.Right - rc1.Left);
                int ch = Math.Max(1, rc1.Bottom - rc1.Top);

                _height = ch;
                _wight = cw;
            }

            var opts = WindowOptions.Default;
            opts.Title = "NV12 → RGB (Silk.NET + D3D11)";
            opts.Size = new Vector2D<int>(_wight, _height);
            _window = Window.Create(opts);

            _window.Load += OnLoad;
            _window.Render += OnRender;
            _window.FramebufferResize += OnResize;
            



            _window.Run();




            //// Создание фабрики DXGI
            //var dxgi = DXGI.GetApi();
            //IDXGIFactory2* pFactory = null;
            //int hr = dxgi.CreateDXGIFactory2(0, SilkMarshal.GuidPtrOf<IDXGIFactory2>(), (void**)&pFactory);
            //SilkMarshal.ThrowHResult(hr);
            //  var dxgiFactory = new IDXGIFactory2((void**)(nint)pFactory);

            //// Получение первого адаптера
            //IDXGIAdapter1* pAdapter1 = null;
            //hr = pFactory->EnumAdapters1(0, &pAdapter1);
            //SilkMarshal.ThrowHResult(hr);
            //// Приведение к IDXGIAdapter
            //IDXGIAdapter* pAdapter = null;
            //hr = pAdapter1->QueryInterface(SilkMarshal.GuidPtrOf<IDXGIAdapter>(), (void**)&pAdapter);
            //SilkMarshal.ThrowHResult(hr);
            //  var adapter = new IDXGIAdapter((void**)(nint)pAdapter);
            //// Создание устройства D3D11 и immediate контекста
            //var device = new ID3D11Device(adapter, DeviceCreationFlags.BgraSupport);
            //var context = device.Ge;
            //// Текстуры для Y и UV (плейсхолдеры под последующий аплоад NV12/прочих форматов)
            //var texDescY = new Texture2DDescription
            //{
            //    Width = _wight,
            //    Height = _height,
            //    Format = SharpDX.DXGI.Format.R8_UNorm,
            //    BindFlags = BindFlags.ShaderResource,
            //    Usage = ResourceUsage.Dynamic,
            //    CpuAccessFlags = CpuAccessFlags.Write,
            //    ArraySize = 1,
            //    MipLevels = 1,
            //    SampleDescription = new SampleDescription(1, 0)
            //};
            //var texY = new D3D11Texture2D(testWind);

            //var texDescUV = texDescY;
            //texDescUV.Width = Math.Max(1, _wight / 2);
            //texDescUV.Height = Math.Max(1, _height / 2);
            //texDescUV.Format = SharpDX.DXGI.Format.R8_UNorm;
            //var texUV = new  D3D11Texture2D(testWind);


            //var opts = WindowOptions.Default;
            //opts.Title = "Silk.NET D3D11";  
            //opts.Size = new Silk.NET.Maths.Vector2D<int>(1280, 720);
            //opts.PreferredBitDepth = new Vector4D<int>(); //new Silk.NET.Windowing.(8, 8, 8, 8);//BitDepth

            //window = Window.Create(opts);

            //window.Load += () =>
            //{
            //    // DXGI factory
            //    dxgiFactory = DXGI.GetApi().CreateDXGIFactory();//.CreateFactory();

            //    // Device + context
            //    D3D11.GetApi().CreateDevice(null,
            //        D3DDriverType.Hardware,
            //        (nint)null,
            //        (uint)CreateDeviceFlag.BgraSupport,
            //        null, 0,
            //        D3D11.SdkVersion,
            //        device, null, context);

            //    // Swap chain
            //    var desc = new SwapChainDesc
            //    {
            //        BufferDesc = new ModeDesc
            //        {
            //            Width = (uint)window.Size.X,
            //            Height = (uint)window.Size.Y,
            //            Format = Format.FormatR8G8B8A8Unorm,
            //            RefreshRate = new Rational(60, 1)
            //        },
            //        SampleDesc = new SampleDesc(1, 0),
            //        BufferUsage = (uint)Usage.RenderTargetOutput,
            //        BufferCount = 2,
            //        OutputWindow = (nint)window.Native!.Win32!.Hwnd,
            //        Windowed = true,
            //        SwapEffect = SwapEffect.Discard,
            //        Flags = 0
            //    };

            //    dxgiFactory->CreateSwapChain(device, &desc, &swapChain);

            //    // RTV from back buffer
            //    ID3D11Texture2D* backBuffer;
            //    swapChain->GetBuffer(0, typeof(ID3D11Texture2D).GUID, (void**)&backBuffer);
            //    device->CreateRenderTargetView((ID3D11Resource*)backBuffer, null, &rtv);
            //    backBuffer->Release();

            //    // Viewport
            //    var vp = new Viewport(0, 0, window.Size.X, window.Size.Y, 0.0f, 1.0f);
            //    context->RSSetViewports(1, &vp);
            //};

            //window.FramebufferResize += s =>
            //{
            //    // Resize swap chain buffers and recreate RTV
            //    rtv->Release();
            //    swapChain->ResizeBuffers(2, (uint)s.X, (uint)s.Y, Format.FormatR8G8B8A8Unorm, 0);
            //    ID3D11Texture2D* backBuffer;
            //    swapChain->GetBuffer(0, typeof(ID3D11Texture2D).GUID, (void**)&backBuffer);
            //    device->CreateRenderTargetView((ID3D11Resource*)backBuffer, null, &rtv);
            //    backBuffer->Release();

            //    var vp = new Viewport(0, 0, s.X, s.Y, 0.0f, 1.0f);
            //    context->RSSetViewports(1, &vp);
            //};

            //window.Render += dt =>
            //{
            //    // Clear + draw + present
            //    var clear = stackalloc float[4] { 0.06f, 0.06f, 0.09f, 1.0f };
            //    context->OMSetRenderTargets(1, &rtv, null);
            //    context->ClearRenderTargetView(rtv, clear);

            //    // TODO: bind shaders/textures and draw fullscreen quad

            //    swapChain->Present(1, 0);
            //};

            //window.Run();



            //var dxgiFactory = new IDXGIFactory2();
            //var adapter = dxgiFactory.EnumAdapters1(0);
            //var device = new ID3D11Device(adapter, DeviceCreationFlags.BgraSupport);
            //var context = device.GetImmediateContext;

            //// Текстуры для Y и UV
            //var texDescY = new Texture2DDescription
            //{
            //    Width = _wight,
            //    Height = _height,
            //    Format = Format.FormatB8G8R8A8Unorm/*R8_UNorm*/,
            //    BindFlags = BindFlags.ShaderResource,
            //    Usage = ResourceUsage.Dynamic,
            //    CpuAccessFlags = CpuAccessFlags.Write,
            //    ArraySize = 1,
            //    MipLevels = 1,
            //    SampleDescription = new SampleDescription(1, 0)
            //};
            //var texY = new ID3D11Texture2D(device, texDescY);

            //var texDescUV = texDescY;
            //texDescUV.Width = _wight / 2;
            //texDescUV.Height = _height / 2;
            //texDescUV.Format = Format.FormatB8G8R8A8Unorm;
            //var texUV = new ID3D11Texture2D(device, texDescUV);
            // _d3d11 ??= D3D11.GetApi();
            // SwapChainDesc desc = default;
            // desc.BufferDesc.Width = (uint)_wight;
            // desc.BufferDesc.Height = (uint)_height;
            // desc.BufferDesc.RefreshRate = new Rational(0, 1);
            // desc.BufferDesc.Format = Format.FormatB8G8R8A8Unorm;
            // desc.BufferDesc.ScanlineOrdering = ModeScanlineOrder.Unspecified;
            // desc.BufferDesc.Scaling = ModeScaling.Unspecified;

            // desc.SampleDesc.Count = 1;
            // desc.SampleDesc.Quality = 0;


            // desc.BufferUsage = (uint)DXGI.UsageRenderTargetOutput;
            // desc.BufferCount = 2; // двойная буферизация для стабильного показа каждого кадра
            // desc.OutputWindow = _hwnd;
            // desc.Windowed = 1;
            // desc.SwapEffect = SwapEffect.Discard; // совместимо и стабильно
            // desc.Flags = 0;

            // IDXGISwapChain* swapChain = null;
            // ID3D11Device* device = null;
            // ID3D11DeviceContext* context = null;

            // int hr = _d3d11!.CreateDeviceAndSwapChain( 
            //     (IDXGIAdapter*)null,
            //     D3DDriverType.Hardware,
            //     0,
            //     (uint)CreateDeviceFlag.BgraSupport,
            //     null,
            //     0,
            //     D3D11.SdkVersion,
            //     &desc,
            //     &swapChain,
            //     &device,
            //     null,
            //     &context
            // );
            // SilkMarshal.ThrowHResult(hr);

            // _swapChain = swapChain;
            // _device = device;
            // _context = context;

            // //EnsureDeviceAndSwapChain();
            // Texture2DDesc texDesc = new()//naxyi
            // {
            //     Width = (uint)width,
            //     Height = (uint)height,
            //     MipLevels = 1,
            //     ArraySize = 1,
            //     Format = Format.FormatB8G8R8A8Unorm,
            //     SampleDesc = new SampleDesc(1, 0),
            //     Usage = Usage.Default,
            //     BindFlags = (uint)BindFlag.None,
            //     CPUAccessFlags = 0,
            //     MiscFlags = 0
            // };

            // ID3D11Texture2D* texture = null;
            // int hr1 = _device->CreateTexture2D(&texDesc, null, &texture);
            // SilkMarshal.ThrowHResult(hr1);

            // _frameTexture = texture;
            //// EnsureBackBuffer();
            // Release(ref _backBuffer);

            // var iid = SilkMarshal.GuidPtrOf<ID3D11Texture2D>();
            // ID3D11Texture2D* backBuffer = null;
            // int hr2 = _swapChain->GetBuffer(0, iid, (void**)&backBuffer);
            // SilkMarshal.ThrowHResult(hr2);
            // _backBuffer = backBuffer;
            // CreateOrRecreateRTV();
            // _compiler ??= D3DCompiler.GetApi();
            // EnsureNV12Resources(width, height);
        }
        public unsafe void PushFrameFromDecoder(AVFrame frame1)
        {
            AVFrame* frame = &frame1;

            if (frame->format != (int)AVPixelFormat.AV_PIX_FMT_NV12) return;

            int w = frame->width;
            int h = frame->height;
            int pitchY = frame->linesize[0];
            int pitchUV = frame->linesize[1];
            int ySize = pitchY * h;
            int uvSize = pitchUV * (h / 2);

            IntPtr yBuf = Marshal.AllocHGlobal(ySize);
            IntPtr uvBuf = Marshal.AllocHGlobal(uvSize);
             
            byte* srcY = (byte*)frame->data[0];
            for (int row = 0; row < h; row++)
            {
                IntPtr dstRow = yBuf + row * pitchY;
                Marshal.Copy(new ReadOnlySpan<byte>(srcY + row * pitchY, w).ToArray(), 0, dstRow, w);
            }
 
            byte* srcUV = (byte*)frame->data[1];
            int halfH = h / 2;
            for (int row = 0; row < halfH; row++)
            {
                IntPtr dstRow = uvBuf + row * pitchUV;
                  Marshal.Copy(new ReadOnlySpan<byte>(srcUV + row * pitchUV, pitchUV).ToArray(), 0, dstRow, pitchUV);
            }

           
            lock (frameLock)
            { 
                if (frameY != IntPtr.Zero) Marshal.FreeHGlobal(frameY);
                if (frameUV != IntPtr.Zero) Marshal.FreeHGlobal(frameUV);

                frameY = yBuf;
                frameUV = uvBuf;
                framePitchY = pitchY;
                framePitchUV = pitchUV;
                frameWidth = w;
                frameHeight = h;
                haveFrame = true;
            }
        }
        static void OnLoad()
        {
            
            ID3D11Device* dev;
            ID3D11DeviceContext* ctx;
            d3d11.CreateDevice(
                null,
                D3DDriverType.Hardware,
                0,
                (uint)CreateDeviceFlag.BgraSupport,
                null, 0,
                D3D11.SdkVersion,
                &dev,
                null,
                &ctx);
            device = dev;
            context = ctx;
 
            IDXGIFactory* f;
            dxgi.CreateDXGIFactory(SilkMarshal.GuidPtrOf<IDXGIFactory>(), (void**)&f);
            factory = f;

           
            var desc = new SwapChainDesc
            {
                BufferDesc = new ModeDesc
                {
                    Width = (uint)_window.Size.X,
                    Height = (uint)_window.Size.Y,
                    Format = Format.FormatR8G8B8A8Unorm,
                    RefreshRate = new Rational(60, 1)
                },
                SampleDesc = new SampleDesc(1, 0),
                BufferUsage = 0x20u, // DXGI_USAGE_RENDER_TARGET_OUTPUT
                BufferCount = 2,
                OutputWindow = (nint)_window.Native!.Win32!.Value.Hwnd,
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                Flags = 0
            };

            IDXGISwapChain* sc;
            factory->CreateSwapChain((IUnknown*)device, &desc, &sc);
            swapChain = sc;

            CreateBackBufferAndViewport();
            CreateFullscreenVB();
            LoadShadersAndCreateInputLayout();
            CreateSamplerState();
             
        }

        static void CreateBackBufferAndViewport()
        {
            ID3D11Texture2D* back;
            swapChain->GetBuffer(0, SilkMarshal.GuidPtrOf<ID3D11Texture2D>(), (void**)&back);
            ID3D11RenderTargetView* r;
            device->CreateRenderTargetView((ID3D11Resource*)back, null, &r);
            rtv = r;
            back->Release();

            var vp = new Viewport(0, 0, _window.Size.X, _window.Size.Y, 0.0f, 1.0f);
            context->RSSetViewports(1, &vp);
        }

        static void CreateFullscreenVB()
        {
            // Fullscreen triangle (pos.xy, uv.xy)
            float[] verts = new float[]
            {
            -1f, -1f, 0f, 1f,
             3f, -1f, 2f, 1f,
            -1f,  3f, 0f, -1f
            };
            fixed (float* p = verts)
            {
                var bd = new BufferDesc
                {
                    Usage = Silk.NET.Direct3D11.Usage.Default,
                    BindFlags = (uint)BindFlag.VertexBuffer,
                    ByteWidth = (uint)(sizeof(float) * verts.Length),
                    CPUAccessFlags = 0,
                    MiscFlags = 0,
                    StructureByteStride = 0
                };
                SubresourceData init = new SubresourceData { PSysMem = p };
                device->CreateBuffer(&bd, &init, (ID3D11Buffer**)vb);
            }
        }


        static void LoadShadersAndCreateInputLayout()
        {
            string vsSrc = @"
struct VSIn { float2 pos : POSITION; float2 uv : TEXCOORD; };
struct PSIn { float4 pos : SV_Position; float2 uv : TEXCOORD; };
PSIn VSMain(VSIn v) { PSIn o; o.pos = float4(v.pos,0,1); o.uv = v.uv; return o; }";

            string psSrc = @"
Texture2D yTex : register(t0);
Texture2D uvTex : register(t1);
SamplerState s0 : register(s0);
float3 YUVtoRGB(float Y, float2 UV){
  float U = UV.x - 0.5; float V = UV.y - 0.5;
  float R = Y + 1.5748 * V;
  float G = Y - 0.1873 * U - 0.4681 * V;
  float B = Y + 1.8556 * U;
  return float3(R,G,B);
}
float4 PSMain(float2 uv : TEXCOORD) : SV_Target {
  float y = yTex.Sample(s0, uv).r;
  float2 uvPacked = uvTex.Sample(s0, uv * 0.5).rg;
  return float4(saturate(YUVtoRGB(y, uvPacked)), 1.0);
}";


           
             byte[] vsBytes = CompileHlslToBytecode(vsSrc, "VSMain", "vs_5_0");
            byte[] psBytes = CompileHlslToBytecode(psSrc, "PSMain", "ps_5_0");




            fixed (byte* pVS = vsBytes)
            fixed (byte* pPS = psBytes)
            {
                device->CreateVertexShader(pVS, (nuint)vsBytes.Length, null, (ID3D11VertexShader**)vs);
                device->CreatePixelShader(pPS, (nuint)psBytes.Length, null, (ID3D11PixelShader**)ps);

                 
                InputElementDesc[] elems = new InputElementDesc[2];
                elems[0] = new InputElementDesc
                {
                    SemanticName = (byte*)SilkMarshal.StringToPtr("POSITION"),
                    SemanticIndex = 0,
                    Format = Format.FormatR32G32Float,
                    InputSlot = 0,
                    AlignedByteOffset = 0,
                    InputSlotClass = InputClassification.PerVertexData,
                    InstanceDataStepRate = 0
                };
                elems[1] = new InputElementDesc
                {
                    SemanticName = (byte*)SilkMarshal.StringToPtr("TEXCOORD"),
                    SemanticIndex = 0,
                    Format = Format.FormatR32G32Float,
                    InputSlot = 0,
                    AlignedByteOffset = 8,
                    InputSlotClass = InputClassification.PerVertexData,
                    InstanceDataStepRate = 0
                };

                fixed (InputElementDesc* pElems = &elems[0])
                {
                    device->CreateInputLayout(pElems, 2, pVS, (nuint)vsBytes.Length, (ID3D11InputLayout**)inputLayout);
                }

             
                SilkMarshal.Free((nint)elems[0].SemanticName);
                SilkMarshal.Free((nint)elems[1].SemanticName);
            }
        }

        static void CreateSamplerState()
        {
            var sd = new SamplerDesc
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                ComparisonFunc = ComparisonFunc.Never,
                MinLOD = 0,
                MaxLOD = float.MaxValue,
                MaxAnisotropy = 1
            };
            device->CreateSamplerState(&sd, (ID3D11SamplerState**)sampler);
        }
         
        static void CreateNv12Textures(int w, int h)
        {
            
              srvY->Release(); srvY = null;   
              texY->Release(); texY = null;  
              srvUV->Release(); srvUV = null;  
              texUV->Release(); texUV = null;  

            // Y 
            var descY = new Texture2DDesc
            {
                Width = (uint)w,
                Height = (uint)h,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.FormatR8Unorm,
                SampleDesc = new SampleDesc(1, 0),
                Usage = Usage.Dynamic,
                BindFlags = (uint)BindFlag.ShaderResource,
                CPUAccessFlags = (uint)CpuAccessFlag.Write,
                MiscFlags = 0
            };
            device->CreateTexture2D(&descY, null, (ID3D11Texture2D**)texY);

            var srvDescY = new ShaderResourceViewDesc
            {
                Format = Format.FormatR8Unorm,
                ViewDimension = D3DSrvDimension.D3D101SrvDimensionTexture2D,
                Anonymous = new ShaderResourceViewDescUnion
                {
                    Texture2D = new Tex2DSrv { MipLevels = 1, MostDetailedMip = 0 }
                }
            };
            device->CreateShaderResourceView((ID3D11Resource*)texY, &srvDescY, (ID3D11ShaderResourceView**)srvY);

            // UV 
            var descUV = descY;
            descUV.Width = (uint)(w / 2);
            descUV.Height = (uint)(h / 2);
            descUV.Format = Format.FormatR8G8Unorm;
            device->CreateTexture2D(&descUV, null, (ID3D11Texture2D**)texUV);

            var srvDescUV = new ShaderResourceViewDesc
            {
                Format = Format.FormatR8G8Unorm,
                ViewDimension = D3DSrvDimension.D3D101SrvDimensionTexture2D,
                Anonymous = new ShaderResourceViewDescUnion
                {
                    Texture2D = new Tex2DSrv { MipLevels = 1, MostDetailedMip = 0 }
                }
            };
            device->CreateShaderResourceView((ID3D11Resource*)texUV, &srvDescUV, (ID3D11ShaderResourceView**)srvUV);

            _wight = w; _height = h;
        }
         
        static void UploadPinnedFrameIfAny()
        {
             
            IntPtr yPtr = IntPtr.Zero, uvPtr = IntPtr.Zero;
            int pY = 0, pUV = 0, w = 0, h = 0;
            lock (frameLock)
            {
                if (!haveFrame) return;
                
                yPtr = frameY; uvPtr = frameUV;
                pY = framePitchY; pUV = framePitchUV; w = frameWidth; h = frameHeight;
                frameY = IntPtr.Zero; frameUV = IntPtr.Zero;
                haveFrame = false;
            }

            if (yPtr == IntPtr.Zero || uvPtr == IntPtr.Zero) return;

          
            if (_wight != w || _height != h)
            {
                CreateNv12Textures(w, h);
            }

            //   Y
            MappedSubresource mY;
            context->Map((ID3D11Resource*)texY, 0, Map.WriteDiscard, 0, &mY);
            byte* dstY = (byte*)mY.PData;
            for (int row = 0; row < h; row++)
            {
                byte* srcRow = (byte*)yPtr + row * pY;
                byte* dstRow = dstY + row * (int)mY.RowPitch;
                System.Buffer.MemoryCopy(srcRow, dstRow, mY.RowPitch, w);  
            }
            context->Unmap((ID3D11Resource*)texY, 0);

             
            MappedSubresource mUV;
            context->Map((ID3D11Resource*)texUV, 0, Map.WriteDiscard, 0, &mUV);
            byte* dstUV = (byte*)mUV.PData;
            int halfH = h / 2;
            int bytesPerRowUV = w; // W/2 texels * 2 bytes = W bytes
            for (int row = 0; row < halfH; row++)
            {
                byte* srcRow = (byte*)uvPtr + row * pUV;
                byte* dstRow = dstUV + row * (int)mUV.RowPitch;
                System.Buffer.MemoryCopy(srcRow, dstRow, mUV.RowPitch, bytesPerRowUV);
            }
            context->Unmap((ID3D11Resource*)texUV, 0);
             
            Marshal.FreeHGlobal(yPtr);
            Marshal.FreeHGlobal(uvPtr);
        }
 
        static void OnRender(double dt)
        {
             
            UploadPinnedFrameIfAny();

            var clearColor = stackalloc float[4] { 0f, 0f, 0f, 1f };
            context->OMSetRenderTargets(1, rtv, null);
            context->ClearRenderTargetView(rtv, clearColor);

            
            if (srvY == null || srvUV == null)
            {
                swapChain->Present(1, 0);
                return;
            }

            
            uint stride = (uint)(sizeof(float) * 4);
            uint offset = 0;
            context->IASetInputLayout(inputLayout);
            context->IASetPrimitiveTopology(D3DPrimitiveTopology.D3D10PrimitiveTopologyTrianglelist);// PrimitiveTopology.TriangleList
            context->IASetVertexBuffers(0, 1, vb, &stride, &offset);

            context->VSSetShader(vs, null, 0);
            context->PSSetShader(ps, null, 0);
            context->PSSetSamplers(0, 1, sampler);

            
            var srvs = stackalloc ID3D11ShaderResourceView*[2] { srvY, srvUV };
            context->PSSetShaderResources(0, 2, srvs);

            context->Draw(3, 0);
            swapChain->Present(1, 0);
        }

        
        static void OnResize(Vector2D<int> size)
        {
            if (swapChain == null) return;
            if (rtv != null) { rtv->Release(); rtv = null; }
            swapChain->ResizeBuffers(2, (uint)size.X, (uint)size.Y, Format.FormatR8G8B8A8Unorm, 0);
            CreateBackBufferAndViewport();
        }

        static void OnClosing()
        {
            // free any copied frame buffers
            lock (frameLock)
            {
                if (frameY != IntPtr.Zero) Marshal.FreeHGlobal(frameY);
                if (frameUV != IntPtr.Zero) Marshal.FreeHGlobal(frameUV);
                frameY = frameUV = IntPtr.Zero;
                haveFrame = false;
            }

            // Release COM objects (check for null)
            //sampler?.Release();
            //vb?.Release();
            //inputLayout?.Release();
            //vs?.Release();
            //ps?.Release();

            //srvUV?.Release(); texUV?.Release();
            //srvY?.Release(); texY?.Release();

            //rtv?.Release();
            //swapChain?.Release();
            //factory?.Release();
            //context?.Release();
            //device?.Release();
        }






        //public static unsafe void OnLoad()
        //{
        //    var d3d11 = D3D11.GetApi();
        //    var dxgi = DXGI.GetApi();

        //     
        //    ID3D11Device* device = null;
        //    ID3D11DeviceContext* context = null;

        //    int hr = d3d11.CreateDevice(
        //        (IDXGIAdapter*)null,
        //        D3DDriverType.Hardware,
        //        0,
        //        (uint)CreateDeviceFlag.BgraSupport,
        //        null,
        //        0,
        //        D3D11.SdkVersion,
        //        &device,
        //        null,
        //        &context
        //    );
        //    SilkMarshal.ThrowHResult(hr);

        //    _device = device;
        //    _context = context;

        //    
        //    IDXGIFactory* factory = null;
        //    hr = dxgi.CreateDXGIFactory(SilkMarshal.GuidPtrOf<IDXGIFactory>(), (void**)&factory);
        //    SilkMarshal.ThrowHResult(hr);

        //     
        //    var size = _window.Size;
        //    SwapChainDesc desc = default;
        //    desc.BufferDesc = new ModeDesc
        //    {
        //        Width = (uint)size.X,
        //        Height = (uint)size.Y,
        //        Format = Silk.NET.DXGI.Format.FormatR8G8B8A8Unorm,
        //        RefreshRate = new Silk.NET.DXGI.Rational(60, 1),
        //        ScanlineOrdering = ModeScanlineOrder.Unspecified,
        //        Scaling = ModeScaling.Unspecified
        //    };


        //    desc.SampleDesc = new SampleDesc(1, 0);
        //    desc.BufferUsage = (uint)Silk.NET.DXGI.DXGI.UsageRenderTargetOutput;

        //    desc.BufferCount = 2;
        //    desc.OutputWindow = (nint)_window.Native!.Win32!.Value.Hwnd; // REQUIRED
        //    desc.Windowed = 1;
        //    desc.SwapEffect = Silk.NET.DXGI.SwapEffect.Discard;
        //    desc.Flags = 0;

        //     
        //    IDXGISwapChain* swapChain = null;
        //    hr = factory->CreateSwapChain((IUnknown*)_device, &desc, &swapChain);
        //    SilkMarshal.ThrowHResult(hr);

        //    _swapChain = swapChain;

        //     
        //    ID3D11Texture2D* backBuffer = null;
        //    hr = _swapChain->GetBuffer(0, SilkMarshal.GuidPtrOf<ID3D11Texture2D>(), (void**)&backBuffer);
        //    SilkMarshal.ThrowHResult(hr);

        //    ID3D11RenderTargetView* rtv = null;
        //    hr = _device->CreateRenderTargetView((ID3D11Resource*)backBuffer, null, &rtv);
        //    SilkMarshal.ThrowHResult(hr);

        //    _rtv = rtv;

        //     
        //    if (backBuffer != null)
        //    {
        //        ((IUnknown*)backBuffer)->Release();
        //        backBuffer = null;
        //    }
        //    if (factory != null)
        //    {
        //        ((IUnknown*)factory)->Release();
        //        factory = null;
        //    }

        //    
        //    Viewport vp = new Viewport
        //    {
        //        TopLeftX = 0,
        //        TopLeftY = 0,
        //        Width = size.X,
        //        Height = size.Y,
        //        MinDepth = 0f,
        //        MaxDepth = 1f
        //    };
        //    _context->RSSetViewports(1, &vp);

        //    
        //    var clear = stackalloc float[4] { 1.1f, 0.1f, 0.1f, 1.0f };
        //    _context->OMSetRenderTargets(1, _rtv, null);

        //    //_context->ClearRenderTargetView(_rtv, clear);




        //    //{D3D11 d3D11 = D3D11.GetApi();
        //    //    PfnVoidFunction* swapChain;
        //    //    d3D11.CreateDeviceAndSwapChain(null,D3DDriverType.Hardware,null,
        //    //        (uint)D3D111CreateDeviceContextStateFlag.D3D111CreateDeviceContextStateSinglethreaded,null,
        //    //        0,D3D11.SdkVersion,out _swapChain,out _device,out _context);

        //    //var clearColor = stackalloc float[4] { 0.1f, 0.1f, 0.1f, 1.0f };
        //    // _context->OMSetRenderTargets(1, &_rtv, null);
        //    // _context->ClearRenderTargetView(_rtv, clearColor);
        //}
        //        static void OnRender(double dt)
        //        {
        //            // If we have new frame, upload it (keep only latest frame)
        //            if (haveFrame)
        //            {
        //                // copy struct locally to avoid race
        //                var f = latestFrame;
        //                haveFrame = false;
        //                UploadFrame(in f);
        //            }

        //            var clear = stackalloc float[4] { 0f, 0f, 0f, 1f };
        //            _context->OMSetRenderTargets(1, _rtv, null);
        //            _context->ClearRenderTargetView(_rtv, clear);

        //            // Set pipeline
        //            uint stride = (uint)(sizeof(float) * 4);
        //            uint offset = 0;
        //            _context->IASetInputLayout(inputLayout);
        //            _context->IASetPrimitiveTopology( D3DPrimitiveTopology.D3D10PrimitiveTopologyTrianglelist);//TriangleList
        //            _context->IASetVertexBuffers(0, 1, vb, &stride, &offset);

        //            _context->VSSetShader(vs, null, 0);
        //            _context->PSSetShader(ps, null, 0);
        //            _context->PSSetSamplers(0, 1, sampler);

        //            // Bind SRVs (t0 = Y, t1 = UV)
        //            var srvs = stackalloc ID3D11ShaderResourceView*[2] { srvY, srvUV };
        //            _context->PSSetShaderResources(0, 2, [srvY, srvUV]); 


        //            _context->Draw(3, 0);
        //            _swapChain->Present(1, 0);
        //        }

        //        static void CreateNv12Textures(int w, int h)
        //        {
        //            // Release existing
        //            if (srvY != null) { srvY->Release(); srvY = null; }
        //            if (texY != null) { texY->Release(); texY = null; }
        //            if (srvUV != null) { srvUV->Release(); srvUV = null; }
        //            if (texUV != null) { texUV->Release(); texUV = null; }

        //            // Y texture
        //            var descY = new Texture2DDesc
        //            {
        //                Width = (uint)w,
        //                Height = (uint)h,
        //                MipLevels = 1,
        //                ArraySize = 1,
        //                Format = Silk.NET.DXGI.Format.FormatR8Unorm,
        //                SampleDesc = new SampleDesc(1, 0),
        //                Usage = Silk.NET.Direct3D11.Usage.Dynamic,//Silk.NET.DXGI.Usage
        //                BindFlags = (uint)BindFlag.ShaderResource,
        //                CPUAccessFlags = (uint)CpuAccessFlag.Write,
        //                MiscFlags = 0
        //            };
        //            _device->CreateTexture2D(&descY, null, (ID3D11Texture2D**)texY);

        //            var srvDescY = new ShaderResourceViewDesc
        //            {
        //                Format = Silk.NET.DXGI.Format.FormatR8Unorm,
        //                ViewDimension =  D3DSrvDimension.D3D101SrvDimensionTexture2D,
        //                Anonymous = new ShaderResourceViewDescUnion { Texture2D = new Tex2DSrv { MipLevels = 1, MostDetailedMip = 0 } }
        //            };
        //            _device->CreateShaderResourceView((ID3D11Resource*)texY, &srvDescY, (ID3D11ShaderResourceView**)srvY);

        //            
        //            var descUV = descY;
        //            descUV.Width = (uint)(w / 2);
        //            descUV.Height = (uint)(h / 2);
        //            descUV.Format = Silk.NET.DXGI.Format.FormatR8G8Unorm;
        //            _device->CreateTexture2D(&descUV, null, (ID3D11Texture2D**)texUV);

        //            var srvDescUV = new ShaderResourceViewDesc
        //            {
        //                Format = Silk.NET.DXGI.Format.FormatR8G8Unorm,
        //                ViewDimension = (D3DSrvDimension)ShaderResourceViewDimension.Texture2D,
        //                Anonymous = new ShaderResourceViewDescUnion { Texture2D =  new Tex2DSrv { MipLevels = 1, MostDetailedMip = 0 } }
        //            };
        //            _device->CreateShaderResourceView((ID3D11Resource*)texUV, &srvDescUV, (ID3D11ShaderResourceView**)srvUV);




        //        }

        //        public static unsafe void UploadFrame(AVFrame frame )
        //        {



        //            MappedSubresource mY;
        //            _context->Map((ID3D11Resource*)texY, 0, Map.WriteDiscard, 0, &mY);
        //            byte* dstY = (byte*)mY.PData;
        //            byte* srcY = frame.data[0];
        //            int srcPitchY = frame.linesize[0];
        //            for (int row = 0; row < frame.height; row++)
        //            {
        //                // копируем ровно width байт (Y — 1 byte per pixel)
        //                Buffer.MemoryCopy(srcY + row * srcPitchY, dstY + row * (int)mY.RowPitch, mY.RowPitch, frame.width);
        //            }
        //            _context->Unmap((ID3D11Resource*)texY, 0);
        //            MappedSubresource mUV;
        //            _context->Map((ID3D11Resource*)texUV, 0, Map.WriteDiscard, 0, &mUV);
        //            byte* dstUV = (byte*)mUV.PData;
        //            byte* srcUV = frame.data[1];
        //            int srcPitchUV = frame.linesize[1];
        //            int halfH = frame.height / 2;
        //            int bytesPerRowUV = frame.width; // (W/2 texels) * 2 bytes = W bytes
        //            for (int row = 0; row < halfH; row++)
        //            {
        //                Buffer.MemoryCopy(srcUV + row * srcPitchUV, dstUV + row * (int)mUV.RowPitch, mUV.RowPitch, bytesPerRowUV);
        //            }
        //            _context->Unmap((ID3D11Resource*)texUV, 0);

        //            // latestFrame=frame;
        //            //if (f.Width != videoW || f.Height != videoH)//sfl,sldfm,sl;dmf,sl;f
        //            //{
        //            //    videoW = f.Width; videoH = f.Height;
        //            //    CreateNv12Textures(videoW, videoH);
        //            //}

        //            //// Map Y
        //            //MappedSubresource mY;
        //            //_context->Map((ID3D11Resource*)texY, 0, Map.WriteDiscard, 0, &mY);
        //            //byte* dstY = (byte*)mY.PData;
        //            //byte* srcY = (byte*)f.Y;
        //            //for (int row = 0; row < f.Height; row++)
        //            //{
        //            //    Buffer.MemoryCopy(srcY + row * f.PitchY, dstY + row * (int)mY.RowPitch, mY.RowPitch, f.Width);
        //            //}
        //            //_context->Unmap((ID3D11Resource*)texY, 0);

        //            //// Map UV (interleaved, W bytes per row where W = frame.Width)
        //            //MappedSubresource mUV;
        //            //_context->Map((ID3D11Resource*)texUV, 0, Map.WriteDiscard, 0, &mUV);
        //            //byte* dstUV = (byte*)mUV.PData;
        //            //byte* srcUV = (byte*)f.UV;
        //            //int h2 = f.Height / 2;
        //            //int bytesPerRowUV = f.Width; // (W/2 texels) * 2 bytes = W bytes
        //            //for (int row = 0; row < h2; row++)
        //            //{
        //            //    Buffer.MemoryCopy(srcUV + row * f.PitchUV, dstUV + row * (int)mUV.RowPitch, mUV.RowPitch, bytesPerRowUV);
        //            //}
        //            //_context->Unmap((ID3D11Resource*)texUV, 0);


        //        }
        //        static void ProducerDemo( )
        //        {

        //            int w = 640, h = 480;
        //            int pitchY = w;
        //            int pitchUV = w; // interleaved UV: W bytes per row
        //            int ySize = pitchY * h;
        //            int uvSize = pitchUV * (h / 2);
        //            IntPtr yBuf = Marshal.AllocHGlobal(ySize);
        //            IntPtr uvBuf = Marshal.AllocHGlobal(uvSize);

        //            for (int i = 0; i < 10000; i++)
        //            {
        //                // Fill Y with gradient, UV constant
        //                byte valY = (byte)((i * 2) % 256);
        //                for (int p = 0; p < ySize; p++) Marshal.WriteByte(yBuf, p, valY);

        //                // UV: interleaved U,V per 2x2 block; set to mid (0x80) + small variation
        //                for (int p = 0; p < uvSize; p += 2)
        //                {
        //                    Marshal.WriteByte(uvBuf, p, (byte)(128 + ((i + p) % 16)));       // U
        //                    Marshal.WriteByte(uvBuf, p + 1, (byte)(128 + ((i + p + 7) % 16))); // V
        //                }

        //                latestFrame = new Nv12Frame
        //                {
        //                    Width = w,
        //                    Height = h,
        //                    Y = yBuf,
        //                    UV = uvBuf,
        //                    PitchY = pitchY,
        //                    PitchUV = pitchUV
        //                };
        //                haveFrame = true;
        //                System.Threading.Thread.Sleep(33); // ~30fps
        //            }

        //            // free when done
        //            Marshal.FreeHGlobal(yBuf);
        //            Marshal.FreeHGlobal(uvBuf);
        //        }

        //        static void CreateShaders()
        //        {
        //            // Vertex shader HLSL
        //            string vsSrc = @"
        //struct VSIn { float2 pos : POSITION; float2 uv : TEXCOORD; };
        //struct PSIn { float4 pos : SV_Position; float2 uv : TEXCOORD; };
        //PSIn VSMain(VSIn v) { PSIn o; o.pos = float4(v.pos, 0, 1); o.uv = v.uv; return o; }";

        //            // Pixel shader: sample Y (R) and UV (RG half-res) and convert
        //            string psSrc = @"
        //Texture2D yTex : register(t0);
        //Texture2D uvTex : register(t1);
        //SamplerState s0 : register(s0);

        //float3 YUVtoRGB(float Y, float2 UV)
        //{
        //    float U = UV.x - 0.5;
        //    float V = UV.y - 0.5;
        //    float R = Y + 1.5748 * V;
        //    float G = Y - 0.1873 * U - 0.4681 * V;
        //    float B = Y + 1.8556 * U;
        //    return float3(R,G,B);
        //}

        //float4 PSMain(float2 uv : TEXCOORD) : SV_Target
        //{
        //    float y = yTex.Sample(s0, uv).r;
        //    float2 uvv = uvTex.Sample(s0, uv * 0.5).rg; // UV half-res
        //    float3 rgb = YUVtoRGB(y, uvv);
        //    return float4(saturate(rgb), 1.0);
        //}";

        //            // Compile VS and PS
        //            ID3DBlob* vsBlob; ID3DBlob* err;
        //            fixed (byte* src = System.Text.Encoding.UTF8.GetBytes(vsSrc))
        //            {
        //                int hr = D3DCompile(src, (nuint)vsSrc.Length, "vs", IntPtr.Zero, IntPtr.Zero, "VSMain", "vs_4_0", 0, 0, out vsBlob, out err);

        //            }
        //            ID3DBlob* psBlob;
        //            fixed (byte* src2 = System.Text.Encoding.UTF8.GetBytes(psSrc))
        //            {
        //                int hr2 = D3DCompile(src2, (nuint)psSrc.Length, "ps", IntPtr.Zero, IntPtr.Zero, "PSMain", "ps_4_0", 0, 0, out psBlob, out err);

        //            }
         
        //            _device->CreateVertexShader(vsBlob->Ptr, (nuint)0, null, (ID3D11VertexShader**)vs); // will recreate correctly below with length via blob pointer hack
        //                                                                          // Proper creation using blob pointer + size: use helpers to read bytes from blob
        //                                                                          // Simpler: marshal blob to byte[] and call CreateVertexShader with pointer+size

        //          
        //            byte[] vsBytes = ReadBlob(vsBlob);
        //            byte[] psBytes = ReadBlob(psBlob);

        //            fixed (byte* pvs = vsBytes)
        //            fixed (byte* pps = psBytes)
        //            {
        //                _device->CreateVertexShader(pvs, (nuint)vsBytes.Length, null, (ID3D11VertexShader**)vs);
        //                _device->CreatePixelShader(pps, (nuint)psBytes.Length, null, (ID3D11PixelShader**)ps);

        //                 
        //                InputElementDesc[] elems = new InputElementDesc[2];
        //                elems[0] = new InputElementDesc
        //                {
        //                    SemanticName = (byte*) SilkMarshal.StringToPtr("POSITION"),
        //                    SemanticIndex = 0,
        //                    Format = Silk.NET.DXGI.Format.FormatR32G32Float,
        //                    InputSlot = 0,
        //                    AlignedByteOffset = 0,
        //                    InputSlotClass = InputClassification.PerVertexData,
        //                    InstanceDataStepRate = 0
        //                };
        //                elems[1] = new InputElementDesc
        //                {
        //                    SemanticName = (byte*)SilkMarshal.StringToPtr("TEXCOORD"),
        //                    SemanticIndex = 0,
        //                    Format = Silk.NET.DXGI.Format.FormatR32G32Float,
        //                    InputSlot = 0,
        //                    AlignedByteOffset = 8,
        //                    InputSlotClass = InputClassification.PerVertexData,
        //                    InstanceDataStepRate = 0
        //                };

        //                fixed (InputElementDesc* pElems = elems)
        //                {
        //                    _device->CreateInputLayout(pElems, 2, pvs, (nuint)vsBytes.Length, (ID3D11InputLayout**)inputLayout);
        //                }

        //                
        //                SilkMarshal.Free((nint)elems[0].SemanticName);
        //                SilkMarshal.Free((nint)elems[1].SemanticName);
        //            }

        //            
        //            ReleaseBlob(vsBlob);
        //            ReleaseBlob(psBlob);
        //        }
        //        static void ReleaseBlob(ID3DBlob* blob)
        //        {
        //            if (blob != null && blob->Ptr != IntPtr.Zero) Marshal.Release(blob->Ptr);
        //        }


        //        static byte[] ReadBlob(ID3DBlob* blob)
        //        {
        //               IntPtr blobPtr = blob->Ptr;
        //             IntPtr vtbl = Marshal.ReadIntPtr(blobPtr);
        //              throw new NotSupportedException("ReadBlob needs Vortice or helper. Replace with precompiled .cso bytes in production.");
        //        }


        //        //static void OnLoad()
        //        //{   

        //        //    _d3d11 = D3D11.GetApi();
        //        //    _dxgi = DXGI.GetApi();

        //        //     
        //        //    ID3D11Device* device;
        //        //    ID3D11DeviceContext* context;
        //        //    _d3d11.CreateDevice(
        //        //        null,
        //        //        D3DDriverType.Hardware,
        //        //        _hwnd,
        //        //        (uint)CreateDeviceFlag.BgraSupport,
        //        //        null, 0,
        //        //        D3D11.SdkVersion,
        //        //        &device,
        //        //        null,
        //        //        &context);

        //        //    _device = device;
        //        //    _context = context;

        //        //    
        //        //    IDXGIFactory* factory;
        //        //    _dxgi.CreateDXGIFactory(typeof(IDXGIFactory).GUID, (void**)&factory);
        //        //    _factory = factory;

        //        //     
        //        //    var desc = new SwapChainDesc
        //        //    {
        //        //        BufferDesc = new ModeDesc
        //        //        {
        //        //            Width = (uint)_window.Size.X,
        //        //            Height = (uint)_window.Size.Y,
        //        //            Format = Silk.NET.DXGI.Format.FormatR8G8B8A8Unorm,
        //        //            RefreshRate = new Silk.NET.DXGI.Rational(60, 1)
        //        //        },
        //        //        SampleDesc = new SampleDesc(1, 0),
        //        //        BufferUsage = (uint)SharpDX.DXGI.Usage.RenderTargetOutput,
        //        //        BufferCount = 2,
        //        //        OutputWindow = (nint)_window.Native.Win32.Hwnd,
        //        //        Windowed = true,
        //        //        SwapEffect = Silk.NET.DXGI.SwapEffect.Discard,
        //        //        Flags = 0
        //        //    };

        //        //    IDXGISwapChain* swapChain;
        //        //    _factory->CreateSwapChain((IUnknown*)_device, &desc, &swapChain);
        //        //    _swapChain = swapChain;

        //        //    CreateBackBufferRTV();
        //        //    SetViewport(_window.Size.X, _window.Size.Y);

        //        //    CreateFullScreenQuad();
        //        //    CreateShadersAndStates();
        //        //    CreateNv12Textures(_videoW, _videoH);
        //        //}

        //        //static void CreateBackBufferRTV()
        //        //{
        //        //    ID3D11Texture2D* backBuffer;
        //        //    _swapChain->GetBuffer(0, typeof(ID3D11Texture2D).GUID, (void**)&backBuffer);
        //        //    ID3D11RenderTargetView* rtv;
        //        //    _device->CreateRenderTargetView((ID3D11Resource*)backBuffer, null, &rtv);
        //        //    _rtv = rtv;
        //        //    backBuffer->Release();
        //        //}

        //        //static void SetViewport(int w, int h)
        //        //{
        //        //    var vp = new Viewport(0, 0, w, h, 0.0f, 1.0f);
        //        //    _context->RSSetViewports(1, &vp);
        //        //}


        //        //static void CreateFullScreenQuad()
        //        //{
        //        //    // Triangle covering full screen (no index buffer)
        //        //    // pos.xy, uv.xy
        //        //    var verts = stackalloc float[]
        //        //    {
        //        //    // x, y, u, v
        //        //    -1f, -1f, 0f, 1f,
        //        //     3f, -1f, 2f, 1f,
        //        //    -1f,  3f, 0f, -1f,
        //        //};

        //        //    var vbDesc = new BufferDesc
        //        //    {
        //        //        Usage = Usage.Default,
        //        //        BindFlags = (uint)BindFlag.VertexBuffer,
        //        //        ByteWidth = (uint)(sizeof(float) * 4 * 3),
        //        //        CPUAccessFlags = 0,
        //        //        MiscFlags = 0,
        //        //        StructureByteStride = 0
        //        //    };

        //        //    var init = new SubresourceData { PSysMem = verts };
        //        //    _device->CreateBuffer(&vbDesc, &init, &_vb);
        //        //}

        //        //static void CreateShadersAndStates()
        //        //{
        //        //    // Compile HLSL separately and embed as byte arrays (omitted here)
        //        //    // For brevity, assume you have compiled VS/PS blobs: vsBlob, psBlob
        //        //    // You can replace this with D3DCompile from Silk.NET or precompiled bytes.

        //        //    byte[] vsBlob = ShaderBytes.VSFullScreen; // your compiled code
        //        //    byte[] psBlob = ShaderBytes.PSNv12ToRgb;  // your compiled code

        //        //    fixed (byte* pVS = vsBlob)
        //        //    fixed (byte* pPS = psBlob)
        //        //    {
        //        //        _device->CreateVertexShader(pVS, (nuint)vsBlob.Length, null, &_vs);
        //        //        _device->CreatePixelShader(pPS, (nuint)psBlob.Length, null, &_ps);
        //        //    }

        //        //    var elems = stackalloc InputElementDesc[2];
        //        //    elems[0] = new InputElementDesc
        //        //    {
        //        //        SemanticName = (byte*)SilkMarshal.StringToPtr("POSITION"),
        //        //        SemanticIndex = 0,
        //        //        Format = Format.FormatR32G32Float,
        //        //        InputSlot = 0,
        //        //        AlignedByteOffset = 0,
        //        //        InputSlotClass = InputClassification.PerVertexData,
        //        //        InstanceDataStepRate = 0
        //        //    };
        //        //    elems[1] = new InputElementDesc
        //        //    {
        //        //        SemanticName = (byte*)SilkMarshal.StringToPtr("TEXCOORD"),
        //        //        SemanticIndex = 0,
        //        //        Format = Format.FormatR32G32Float,
        //        //        InputSlot = 0,
        //        //        AlignedByteOffset = 8, // 2 floats (position)
        //        //        InputSlotClass = InputClassification.PerVertexData,
        //        //        InstanceDataStepRate = 0
        //        //    };

        //        //    // Create input layout from VS blob
        //        //    fixed (byte* pVS = vsBlob)
        //        //    {
        //        //        _device->CreateInputLayout(elems, 2, pVS, (nuint)vsBlob.Length, &_inputLayout);
        //        //    }

        //        //     
        //        //    SilkMarshal.Free((nint)elems[0].SemanticName);
        //        //    SilkMarshal.Free((nint)elems[1].SemanticName);

        //        //    // Sampler
        //        //    var sampDesc = new SamplerDesc
        //        //    {
        //        //        Filter = Filter.MinMagMipLinear,
        //        //        AddressU = TextureAddressMode.Clamp,
        //        //        AddressV = TextureAddressMode.Clamp,
        //        //        AddressW = TextureAddressMode.Clamp,
        //        //        MaxAnisotropy = 1,
        //        //        ComparisonFunc = ComparisonFunc.Never,
        //        //        MinLOD = 0,
        //        //        MaxLOD = float.MaxValue
        //        //    };
        //        //    _device->CreateSamplerState(&sampDesc, &_sampler);
        //        //}



        //        private static void Release<T>(ref T* comPtr) where T : unmanaged
        //        {
        //            if (comPtr != null)
        //            {
        //                ((IUnknown*)comPtr)->Release();
        //                comPtr = null;
        //            }
        //        }
        //private void EnsureNV12Resources(int width, int height)
        //{
        //    if (_device == null) return;
        //    int uvW = Math.Max(1, width / 2);
        //    int uvH = Math.Max(1, height / 2);

        //    bool needY = _nv12Y == null;
        //    bool needUV = _nv12UV == null;

        //    if (!needY)
        //    {
        //        Texture2DDesc d = default;
        //        _nv12Y->GetDesc(&d);
        //        if (d.Width != (uint)width || d.Height != (uint)height || d.Format != Format.FormatR8Unorm)
        //        {
        //            Release(ref _nv12Y);
        //            Release(ref _srvY);
        //            needY = true;
        //        }
        //    }
        //    if (!needUV)
        //    {
        //        Texture2DDesc d = default;
        //        _nv12UV->GetDesc(&d);
        //        if (d.Width != (uint)uvW || d.Height != (uint)uvH || d.Format != Format.FormatR8G8Unorm)
        //        {
        //            Release(ref _nv12UV);
        //            Release(ref _srvUV);
        //            needUV = true;
        //        }
        //    }

        //    if (needY)
        //    {
        //        Texture2DDesc yDesc = new()
        //        {
        //            Width = (uint)width,
        //            Height = (uint)height,
        //            MipLevels = 1,
        //            ArraySize = 1,
        //            Format = Format.FormatR8Unorm,
        //            SampleDesc = new SampleDesc(1, 0),
        //            Usage = Usage.Default,
        //            BindFlags = (uint)BindFlag.ShaderResource,
        //            CPUAccessFlags = 0,
        //            MiscFlags = 0
        //        };
        //        ID3D11Texture2D* y = null;
        //        int hrY = _device->CreateTexture2D(&yDesc, null, &y);
        //        SilkMarshal.ThrowHResult(hrY);
        //        _nv12Y = y;

        //        ShaderResourceViewDesc srvDescY = new()
        //        {
        //            Format = Format.FormatR8Unorm,
        //            ViewDimension = D3DSrvDimension.D3D11SrvDimensionTexture2D,
        //            Texture2D = new Tex2DSrv { MipLevels = 1, MostDetailedMip = 0 }
        //        };
        //        ID3D11ShaderResourceView* srvY = null;
        //        int hrSrvY = _device->CreateShaderResourceView((ID3D11Resource*)_nv12Y, &srvDescY, &srvY);
        //        SilkMarshal.ThrowHResult(hrSrvY);
        //        _srvY = srvY;
        //    }

        //    if (needUV)
        //    {
        //        Texture2DDesc uvDesc = new()
        //        {
        //            Width = (uint)uvW,
        //            Height = (uint)uvH,
        //            MipLevels = 1,
        //            ArraySize = 1,
        //            Format = Format.FormatR8G8Unorm,
        //            SampleDesc = new SampleDesc(1, 0),
        //            Usage = Usage.Default,
        //            BindFlags = (uint)BindFlag.ShaderResource,
        //            CPUAccessFlags = 0,
        //            MiscFlags = 0
        //        };
        //        ID3D11Texture2D* uv = null;
        //        int hrUV = _device->CreateTexture2D(&uvDesc, null, &uv);
        //        SilkMarshal.ThrowHResult(hrUV);
        //        _nv12UV = uv;

        //        ShaderResourceViewDesc srvDescUV = new()
        //        {
        //            Format = Format.FormatR8G8Unorm,
        //            ViewDimension = D3DSrvDimension.D3D11SrvDimensionTexture2D,
        //            Texture2D = new Tex2DSrv { MipLevels = 1, MostDetailedMip = 0 }
        //        };
        //        ID3D11ShaderResourceView* srvUV = null;
        //        int hrSrvUV = _device->CreateShaderResourceView((ID3D11Resource*)_nv12UV, &srvDescUV, &srvUV);
        //        SilkMarshal.ThrowHResult(hrSrvUV);
        //        _srvUV = srvUV;
        //    }

        //    if (_sampler == null)
        //    {
        //        SamplerDesc sdesc = new()
        //        {
        //            Filter = Filter.FilterMinMagMipLinear,
        //            AddressU = TextureAddressMode.Clamp,
        //            AddressV = TextureAddressMode.Clamp,
        //            AddressW = TextureAddressMode.Clamp,
        //            MaxAnisotropy = 1,
        //            ComparisonFunc = (ComparisonFunc)SharpDX.Direct3D11.Comparison.Never,
        //            MinLOD = 0,
        //            MaxLOD = float.MaxValue
        //        };
        //        ID3D11SamplerState* sampler = null;
        //        int hrS = _device->CreateSamplerState(&sdesc, &sampler);
        //        SilkMarshal.ThrowHResult(hrS);
        //        _sampler = sampler;
        //    }

        //    if (_vs == null || _ps == null)
        //    {
        //        CompileAndCreateShaders();
        //    }
        //}
        //        private void CompileAndCreateShaders()
        //        {
        //            if (_compiler == null) return;

        //             string hlsl = @"
        //cbuffer Dummy : register(b0) { float2 dummy0; };
        //Texture2D YTex   : register(t0);
        //Texture2D UVTex  : register(t1);
        //SamplerState Samp : register(s0);

        //struct VSOut { float4 pos : SV_Position; float2 uv : TEXCOORD0; };

        //VSOut VS(uint vid : SV_VertexID)
        //{
        //    float2 pos[3] = { float2(-1,-1), float2(-1,3), float2(3,-1) };
        //    float2 tc [3] = { float2(0,1) , float2(0,-1), float2(2,1) };
        //    VSOut o;
        //    o.pos = float4(pos[vid], 0, 1);
        //    o.uv  = tc[vid];
        //    return o;
        //}

        //// BT.709 limited range
        //float4 PS(VSOut i) : SV_Target
        //{
        //    float y  = YTex.Sample(Samp, i.uv).r;
        //    float2 uv = UVTex.Sample(Samp, i.uv).rg;

        //    y  = 1.16438356 * (y - 16.0/255.0);
        //    float u = uv.x - 0.5;
        //    float v = uv.y - 0.5;

        //    float r = saturate(y + 1.79274107 * v);
        //    float g = saturate(y - 0.21324861 * u - 0.53290933 * v);
        //    float b = saturate(y + 2.11240179 * u);

        //    return float4(r,g,b,1);
        //}";
        //             
        //            var srcPtr = SilkMarshal.StringToPtr(hlsl, NativeStringEncoding.UTF8);
        //            try
        //            {
        //                ID3D10Blob* vsBlob = null;
        //                ID3D10Blob* psBlob = null;
        //                ID3D10Blob* err = null;

        //                int hrVS = _compiler!.Compile((sbyte*)srcPtr, (nuint)hlsl.Length, "NV12", null, null, "VS", "vs_5_0", 0, 0, &vsBlob, &err);
        //                if (hrVS < 0)
        //                {
        //                    string errStr = err != null ? SilkMarshal.PtrToString((nint)err->GetBufferPointer(), NativeStringEncoding.UTF8) ?? "VS compile error" : "VS compile error";
        //                    Release(ref err);
        //                    SilkMarshal.ThrowHResult(hrVS);
        //                }
        //                int hrPS = _compiler!.Compile((sbyte*)srcPtr, (nuint)hlsl.Length, "NV12", null, null, "PS", "ps_5_0", 0, 0, &psBlob, &err);
        //                if (hrPS < 0)
        //                {
        //                    string errStr = err != null ? SilkMarshal.PtrToString((nint)err->GetBufferPointer(), NativeStringEncoding.UTF8) ?? "PS compile error" : "PS compile error";
        //                    Release(ref err);
        //                    SilkMarshal.ThrowHResult(hrPS);
        //                }

        //                 
        //                ID3D11VertexShader* vs = null;
        //                int crVS = _device->CreateVertexShader(vsBlob->GetBufferPointer(), vsBlob->GetBufferSize(), null, &vs);
        //                SilkMarshal.ThrowHResult(crVS);
        //                _vs = vs;

        //                ID3D11PixelShader* ps = null;
        //                int crPS = _device->CreatePixelShader(psBlob->GetBufferPointer(), psBlob->GetBufferSize(), null, &ps);
        //                SilkMarshal.ThrowHResult(crPS);
        //                _ps = ps;

        //                Release(ref vsBlob);
        //                Release(ref psBlob);
        //                Release(ref err);
        //            }
        //            finally
        //            {
        //                SilkMarshal.Free((nint)srcPtr);
        //            }
        //        }
        //        private void CreateOrRecreateRTV()
        //        {
        //            Release(ref _rtv);
        //            if (_device == null || _backBuffer == null) return;

        //            ID3D11RenderTargetView* rtv = null;
        //            int hr = _device->CreateRenderTargetView((ID3D11Resource*)_backBuffer, null, &rtv);
        //            SilkMarshal.ThrowHResult(hr);
        //            _rtv = rtv;
        //        }
        //public void RenderFrame(AVFrame frame, nint testWind, string name)
        //{
        //    if (GetClientRect((IntPtr)testWind, out RECT rc1))
        //    {
        //        int cw = Math.Max(1, rc1.Right - rc1.Left);//rc.Right - rc.Left
        //        int ch = Math.Max(1, rc1.Bottom - rc1.Top);// rc.Bottom - rc.Top

        //        _height = ch;
        //        _wight = cw;
        //       // GLFW.SetWindowSize(window, cw, ch);
        //       // SetOverlayText(name, cw / 10);
        //    }  ;


        //if (_device is null || _context is null || _swapChain is null)
        //    return;
        //int width = frame.width;
        //int height = frame.height;
        //if (width != _wight || height != _height)
        //{
        //    //Resize(width, height);
        //}
        //EnsureFrameTexture(width, height);

        //byte* p = frame.data[0];

        //    _context->UpdateSubresource((ID3D11Resource*)_frameTexture, 0, null, p, (uint)frame.linesize[0], (uint)(frame.linesize[0] * _height));

        //_context->CopyResource((ID3D11Resource*)_backBuffer, (ID3D11Resource*)_frameTexture);
        //int hr = _swapChain->Present(1, 0);



        //if (_device == null || _context == null || _swapChain == null) return;

        //int w = frame.width;
        //int h = frame.height;
        //if (w <= 0 || h <= 0) return;

        
        //EnsureNV12Resources(w, h);
        //if (_nv12Y == null || _nv12UV == null || _srvY == null || _srvUV == null || _sampler == null || _vs == null || _ps == null)
        //    return;

        
        
        //{
        //    byte* srcY = frame.data[0];
        //    int srcPitchY = frame.linesize[0];
        //    _context->UpdateSubresource((ID3D11Resource*)_nv12Y, 0, null, srcY, (uint)srcPitchY, 0);
        //}
        
        //{
        //    byte* srcUV = frame.data[1];
        //    int srcPitchUV = frame.linesize[1];
        //    _context->UpdateSubresource((ID3D11Resource*)_nv12UV, 0, null, srcUV, (uint)srcPitchUV, 0);
        //}

        
        //if (_rtv == null) CreateOrRecreateRTV();

         
        //var clear = stackalloc float[4];
        //clear[0] = 0f; clear[1] = 0f; clear[2] = 0f; clear[3] = 1f;
        //var rtvs = stackalloc ID3D11RenderTargetView*[1];
        //rtvs[0] = _rtv;
        //_context->OMSetRenderTargets(1, rtvs, null);
        //_context->ClearRenderTargetView(_rtv, clear);

        //Viewport vp = new()
        //{
        //    TopLeftX = 0,
        //    TopLeftY = 0,
        //    Width = _wight,
        //    Height = _height,
        //    MinDepth = 0,
        //    MaxDepth = 1
        //};
        //_context->RSSetViewports(1, &vp);

        
        //_context->IASetInputLayout(null);
        //_context->IASetPrimitiveTopology( D3DPrimitiveTopology.D3D10PrimitiveTopologyTrianglelist );
        //_context->VSSetShader(_vs, null, 0);
        //_context->PSSetShader(_ps, null, 0);

         
        //var srvs = stackalloc ID3D11ShaderResourceView*[2];
        //srvs[0] = _srvY;
        //srvs[1] = _srvUV;
        //_context->PSSetShaderResources(0, 2, srvs);

        //var samps = stackalloc ID3D11SamplerState*[1];
        //samps[0] = _sampler;
        //_context->PSSetSamplers(0, 1, samps);

         
        //_context->Draw(3, 0);

         //var nulls = stackalloc ID3D11ShaderResourceView*[2];
        //nulls[0] = null;
        //nulls[1] = null;
        //_context->PSSetShaderResources(0, 2, nulls);

      
        //int hr = _swapChain->Present(1, 0);
        //if (hr < 0) SilkMarshal.ThrowHResult(hr);

        //}
        //public void PresentBGRA(ReadOnlySpan<byte> bgra, int width, int height, int strideBytes)
        //{
        //    if (_device is null || _context is null || _swapChain is null)
        //        return;
        //
        //    if (width != _wight || height != _height)
        //    {
        //        Resize(width, height);
        //    }
        //
        //    EnsureFrameTexture(width, height);
        //    if (_frameTexture is null)
        //        return;
        //
        //    if (_backBuffer is null)
        //    {
        //        EnsureBackBuffer();
        //        if (_backBuffer is null) return;
        //    }
        //
        //    fixed (byte* p = bgra)
        //    {
        //        _context->UpdateSubresource((ID3D11Resource*)_frameTexture, 0, null, p, (uint)strideBytes, (uint)(strideBytes * height));
        //    }
        //
        //    _context->CopyResource((ID3D11Resource*)_backBuffer, (ID3D11Resource*)_frameTexture);
        //
        //    int hr = _swapChain->Present(1, 0);
        //     
        //}

        //public void Resize(int newWidth, int newHeight)
        //{
        //    if (_swapChain is null || _device is null)
        //        return;
        //
        //    _wight = Math.Max(1, newWidth);
        //    _height = Math.Max(1, newHeight);
        //
        //     
        //    Release(ref _frameTexture);
        //    Release(ref _backBuffer);
        //
        //     
        //    int hr = _swapChain->ResizeBuffers(2, (uint)_wight, (uint)_height, Format.FormatB8G8R8A8Unorm, 0);
        //    SilkMarshal.ThrowHResult(hr);
        //
        //    EnsureBackBuffer();
        //    EnsureFrameTexture(_wight, _height);
        //}

        //private void EnsureDeviceAndSwapChain()
        //{
        //    if (_d3d11 is not null && _device is not null && _context is not null && _swapChain is not null)
        //        return;
        //
        //    _d3d11 ??= D3D11.GetApi();
        //    if (_hwnd == 0 || !GetClientRect((IntPtr)_hwnd, out var rc) || rc.Right - rc.Left <= 0 || rc.Bottom - rc.Top <= 0)
        //        throw new InvalidOperationException("Invalid or zero-sized HWND for swap chain.");
        //
        //    _wight = Math.Max(1, rc.Right - rc.Left);
        //    _height = Math.Max(1, rc.Bottom - rc.Top);
        //
        //    SwapChainDesc desc = default;
        //    desc.BufferDesc.Width = (uint)_wight;
        //    desc.BufferDesc.Height = (uint)_height;
        //    desc.BufferDesc.RefreshRate = new Rational(0, 1);
        //    desc.BufferDesc.Format = Format.FormatB8G8R8A8Unorm;
        //    desc.BufferDesc.ScanlineOrdering = ModeScanlineOrder.Unspecified;
        //    desc.BufferDesc.Scaling = ModeScaling.Unspecified;
        //
        //    desc.SampleDesc.Count = 1;
        //    desc.SampleDesc.Quality = 0;
        //
        //     
        //    desc.BufferUsage = (uint)DXGI.UsageRenderTargetOutput;
        //    desc.BufferCount = 2;  
        //    desc.OutputWindow = _hwnd;
        //    desc.Windowed = 1;
        //    desc.SwapEffect = SwapEffect.Discard;  
        //    desc.Flags = 0;
        //
        //    IDXGISwapChain* swapChain = null;
        //    ID3D11Device* device = null;
        //    ID3D11DeviceContext* context = null;
        //
        //    int hr = _d3d11!.CreateDeviceAndSwapChain(
        //        (IDXGIAdapter*)null,
        //        D3DDriverType.Hardware,
        //        0,
        //        (uint)CreateDeviceFlag.BgraSupport,
        //        null,
        //        0,
        //        D3D11.SdkVersion,
        //        &desc,
        //        &swapChain,
        //        &device,
        //        null,
        //        &context
        //    );
        //    SilkMarshal.ThrowHResult(hr);
        //
        //    _swapChain = swapChain;
        //    _device = device;
        //    _context = context;
        //}

        //private void EnsureBackBuffer()
        //{
        //    if (_swapChain is null) return;
        //
        //    Release(ref _backBuffer);
        //
        //    var iid = SilkMarshal.GuidPtrOf<ID3D11Texture2D>();
        //    ID3D11Texture2D* backBuffer = null;
        //    int hr = _swapChain->GetBuffer(0, iid, (void**)&backBuffer);
        //    SilkMarshal.ThrowHResult(hr);
        //    _backBuffer = backBuffer;
        //}

        //private void EnsureFrameTexture(int width, int height)
        //{
        //    if (_device is null) return;
        //
        //    bool needCreate = _frameTexture == null;
        //
        //    if (!needCreate)
        //    {
        //        Texture2DDesc current = default;
        //        _frameTexture->GetDesc(&current);
        //        if (current.Width != (uint)width || current.Height != (uint)height || current.Format != Format.FormatB8G8R8A8Unorm)
        //        {
        //            Release(ref _frameTexture);
        //            needCreate = true;
        //        }
        //    }
        //
        //    if (!needCreate) return;//-
        //
        //    Texture2DDesc texDesc = new()
        //    {
        //        Width = (uint)width,
        //        Height = (uint)height,
        //        MipLevels = 1,
        //        ArraySize = 1,
        //        Format = Format.FormatB8G8R8A8Unorm,
        //        SampleDesc = new SampleDesc(1, 0),
        //        Usage = Usage.Default,
        //        BindFlags = (uint)BindFlag.None,
        //        CPUAccessFlags = 0,
        //        MiscFlags = 0
        //    };
        //
        //    ID3D11Texture2D* texture = null;
        //    int hr = _device->CreateTexture2D(&texDesc, null, &texture);
        //    SilkMarshal.ThrowHResult(hr);
        //
        //    _frameTexture = texture;
        //}

        //private void TryRecreateOnDeviceLost(int hr)
        //{
        //    const int DXGI_ERROR_DEVICE_REMOVED = unchecked((int)0x887A0005);
        //    const int DXGI_ERROR_DEVICE_RESET = unchecked((int)0x887A0007);
        //
        //    if (hr == DXGI_ERROR_DEVICE_REMOVED || hr == DXGI_ERROR_DEVICE_RESET)
        //    {
        //        RecreateDeviceAndSwapChain();
        //    }
        //    else
        //    {
        //        SilkMarshal.ThrowHResult(hr);
        //    }
        //}

        //private void RecreateDeviceAndSwapChain()
        //{
        //    Release(ref _frameTexture);
        //    Release(ref _backBuffer);
        //    Release(ref _swapChain);
        //    Release(ref _context);
        //    Release(ref _device);
        //
        //    EnsureDeviceAndSwapChain();
        //    EnsureBackBuffer();
        //    EnsureFrameTexture(_wight, _height);
        //}

        //public void Dispose()
        //{
        //    Release(ref _frameTexture);
        //    Release(ref _backBuffer);
        //    Release(ref _swapChain);
        //    Release(ref _context);
        //    Release(ref _device);
        //
        //    _d3d11?.Dispose();
        //    _d3d11 = null;
        //}

        
    }
}
