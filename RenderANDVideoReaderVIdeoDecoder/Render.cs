
 
using Silk.NET.Core.Contexts;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.Windowing;
using SharpDX.DXGI;


namespace RenderANDVideoReaderVIdeoDecoder
{
    public unsafe class Render 
    {
        private Vector2 mousePos = Vector2.Zero;

        private Vector2 cameraDir = Vector2.Zero;
        private Vector3 cameraPos = new Vector3(0, 0, -10);
 

        


        INativeWindow nativeWindow  ;
        IDXGIFactory1* pFactory;
        IDXGISwapChain* swapchain;              
        ID3D11Device* dev;                      
        ID3D11DeviceContext* devcon;

        [DllImport("user32.dll", SetLastError = true)] static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
        struct RECT { public int Left; public int Top; public int Right; public int Bottom; }

        private int _wight;
        private int _height;
        private IDXGIAdapter* adapter1;
        private Guid* guid;

        public unsafe void Init(int width, int height, string name, nint testWind)
        {
            if (GetClientRect((IntPtr)testWind, out RECT rc1))
            {
                int cw = Math.Max(1, rc1.Right - rc1.Left);//rc.Right - rc.Left
                int ch = Math.Max(1, rc1.Bottom - rc1.Top);// rc.Bottom - rc.Top

                _height = ch;
                _wight = cw;

            }
            //guid = (Guid*)Marshal.AllocHGlobal(sizeof(Guid));
            //WindowOptions options = new()
            //{
            //    Size = new(_wight, _height),
            //    Title = "Foo",
            //    IsVisible = true,
            //    Position = new(50, 50),
            //     API =  new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.Default, new APIVersion(3, 3)),
            //     VideoMode = VideoMode.Default,
            //     WindowState = WindowState.Normal


            //};
            //IWindow window = Window.Create(options);


            ////  window.WindowBorder = WindowBorder.Hidden;
            //window.Initialize();
            // window.Center();

            //  nativeWindow = window.Native;
            //    nativeWindow.X11.Value.Window=((nint)testWind);   IID_IDXGIFactory1
            //SilkMarshal.ThrowHResult(DXGI.GetApi().CreateDXGIFactory1((Guid*)Unsafe.AsPointer(ref Factory1.As()), (void**)pFactory));

            //ХУЕТАЕБАНА    ЯЮБМПЬДЛЫВАЬЖЫВАДЫТОВПРЫХВДПЛЫПДЩ
            DXGI dxgi = DXGI.GetApi();
            dxgi.CreateDXGIFactory1 (guid, (void**)pFactory);
            swapchain = (IDXGISwapChain*)(IDXGISwapChain**)Marshal.AllocHGlobal(sizeof(IDXGISwapChain*));
            dev = (ID3D11Device*)(ID3D11Device**)(ID3D11Device**)Marshal.AllocHGlobal(sizeof(ID3D11Device*));
            devcon = (ID3D11DeviceContext*)(ID3D11DeviceContext**)(ID3D11DeviceContext**)Marshal.AllocHGlobal(sizeof(ID3D11DeviceContext*));
            D3D11 d3d11 = D3D11.GetApi();
            d3d11.CreateDevice((IDXGIAdapter*)adapter1, D3DDriverType.Hardware, IntPtr.Zero,
              (uint)CreateDeviceFlag.None, null, 0, D3D11.SdkVersion, ref dev, null, ref devcon);
             
             d3d11.CreateDeviceAndSwapChain((IDXGIAdapter*)adapter1, D3DDriverType.Hardware, IntPtr.Zero, (uint)CreateDeviceFlag.None,
                   null, 0, D3D11.SdkVersion, (SwapChainDesc*)swapchain, (IDXGISwapChain**)swapchain, (ID3D11Device**)dev, null, (ID3D11DeviceContext**)devcon);
            d3d11.Context.TryGetProcAddress("D3D11CreateDeviceAndSwapChain", out nint addr);//D3D11CreateDeviceAndSwapChainDelegate

            d3d11.PurgeEntryPoints();

            //window.Load += () => 
            //    {
            //        D3D11 d3d11 = D3D11.GetApi();
            //        d3d11.CreateDevice((IDXGIAdapter*)adapter1, D3DDriverType.Hardware, IntPtr.Zero,
            //          (uint)CreateDeviceFlag.None, null, 0, D3D11.SdkVersion, ref dev, null, ref devcon);
            //        d3d11.CreateDeviceAndSwapChain((IDXGIAdapter*)adapter1, D3DDriverType.Hardware, IntPtr.Zero, (uint)CreateDeviceFlag.None,
            //            null, 0, D3D11.SdkVersion, (SwapChainDesc*)swapchain, (IDXGISwapChain**)swapchain, (ID3D11Device**)dev, null, (ID3D11DeviceContext**)devcon);
            //        d3d11.Context.TryGetProcAddress("D3D11CreateDeviceAndSwapChain", out nint addr);//D3D11CreateDeviceAndSwapChainDelegate

            //        d3d11.PurgeEntryPoints();



            //    };
            //window.Run();




            //   DXGI dxgi = DXGI.GetApi();

            //  dxgi.CreateDXGIFactory1(guid, (void**)pFactory);
            //  IDXGIAdapter* adapter1 = null;

            //  IDXGIOutput* output = null;
            // if (adapter1->EnumOutputs(0, &output) != 0) {/*..*/}
            // IDXGIOutput5* output5 = (IDXGIOutput5*)output;



            //        ID3D11Device* device = null;
            //        ID3D11DeviceContext* context = null;
            //        D3DFeatureLevel featureLevel = D3DFeatureLevel.Level101;
            //        D3DFeatureLevel[] featureLevels =
            //        [
            //            D3DFeatureLevel.Level111, D3DFeatureLevel.Level110, D3DFeatureLevel.Level101,D3DFeatureLevel.Level100
            //        ];
            //        fixed (D3DFeatureLevel* pFeatureLevels = &featureLevels[0])
            //        {
            //            D3D11 d3D11 = new D3D11(new DefaultNativeContext("d3d11"));
            //            if (d3D11.CreateDevice((IDXGIAdapter*)adapter1, D3DDriverType.Unknown, IntPtr.Zero,
            //                    (uint)CreateDeviceFlag.None, pFeatureLevels, (uint)featureLevels.Length, D3D11.SdkVersion, ref device,
            //&featureLevel, ref context) != 0) {/*..*/}
            //        }
            //        ID3D11DeviceContext* immediateContext = null;
            //        device->GetImmediateContext(ref immediateContext);
            //        IDXGIOutputDuplication* outputDuplication = null;
            //        output5.DuplicateOutput((IUnknown*)device, ref outputDuplication);
            //        OutduplFrameInfo outduplFrameInfo = new OutduplFrameInfo();
            //        IDXGIResource* desktopResource = null;
            //        if (outputDuplication->AcquireNextFrame(1000, &outduplFrameInfo, &desktopResource) != 0){/*..*/}
            //        if (desktopResource->QueryInterface<ID3D11Resource>(out var desktopTexture) != 0) {/*..*/}
            //        Texture2DDesc stagingTextureDesc = new()
            //        {
            //            CPUAccessFlags = (uint)CpuAccessFlag.Read,
            //            BindFlags = (uint)(BindFlag.None),
            //            Format = Format.FormatB8G8R8A8Unorm,
            //            Width = (uint)width,
            //            Height = (uint)height,
            //            MiscFlags = (uint)ResourceMiscFlag.None,
            //            MipLevels = 1,
            //            ArraySize = 1,
            //            SampleDesc = { Count = 1, Quality = 0 },
            //            Usage = Usage.Staging
            //        };
            //        ID3D11Texture2D* stagingTexture = null;
            //        if (device->CreateTexture2D(&stagingTextureDesc, null, ref stagingTexture) != 0) {/*..*/}
            //        stagingTexture->QueryInterface<ID3D11Resource>(out var stagingResource);
            //        immediateContext->CopyResource(stagingResource, desktopTexture);
            //        MappedSubresource mappedSubresource = new MappedSubresource();
            //        if (immediateContext->Map(stagingResource, 0, Map.Read, 0, &mappedSubresource) != 0) {/*..*/}
            //        var span = new ReadOnlySpan<byte>(mappedSubresource.PData,
            //            (int)mappedSubresource.DepthPitch);





            //   SwapChainDesc1 swapChainDesc1 = new SwapChainDesc1();
            //   swapChainDesc1.Width = (uint)width;
            //   swapChainDesc1.Height = (uint)height;
            //   swapChainDesc1.Format =  Format.FormatB8G8R8A8Unorm;
            //   swapChainDesc1.SampleDesc.Count = 1;
            //   swapChainDesc1.SampleDesc.Quality = 0;
            //   swapChainDesc1.BufferUsage =  DXGI.UsageRenderTargetOutput;
            //   swapChainDesc1.BufferCount = 2;
            //    swapChainDesc1.SwapEffect = SwapEffect.FlipDiscard;
            //    swapChainDesc1.Scaling = Scaling.Stretch;
            //    swapChainDesc1.Flags =  0      ;
            //   SwapChainFullscreenDesc swapChainFullscreenDesc = new SwapChainFullscreenDesc();
            //   swapChainFullscreenDesc.Windowed = true;
            ////   swapchain = swapChainDesc1;




            // swapchain = (IDXGISwapChain*)(IDXGISwapChain**)Marshal.AllocHGlobal(sizeof(IDXGISwapChain*));
            //dev = (ID3D11Device*)(ID3D11Device**)(ID3D11Device**)Marshal.AllocHGlobal(sizeof(ID3D11Device*));
            // SharpDX.DXGI.Factory1.ToCallbackPtr( pFactory );
            //var h =  DXGI.GetApi();
            //D3D11 d3d11 = D3D11.GetApi();
            //d3d11.Context.TryGetProcAddress("D3D11CreateDeviceAndSwapChain", out nint addr);//D3D11CreateDeviceAndSwapChainDelegate
            //d3d11.CreateDeviceAndSwapChain((IDXGIAdapter*)null, (D3DDriverType)DriveType.Ram, 0, 0, null, 0,
            //    D3D11.SdkVersion, (SwapChainDesc*)&swapChainDesc1, (IDXGISwapChain**)swapchain, (ID3D11Device**)dev, null, (ID3D11DeviceContext**)devcon);

            //d3d11.CreateDeviceAndSwapChain((IDXGIAdapter*)null, D3DDriverType.Hardware, 0, 0, null, 0,
            //    D3D11.SdkVersion, (SwapChainDesc*)&swapChainDesc1, (IDXGISwapChain**)swapchain, (ID3D11Device**)dev, null, (ID3D11DeviceContext**)devcon);
            //d3d11.PurgeEntryPoints();
            //swapchain->Present(1, 0);


            //   var b= nwe.Win32;



            //var g =  D3D11.CreateDefaultContext(d3d11.Context.ToString());

        }
    }

     
}
