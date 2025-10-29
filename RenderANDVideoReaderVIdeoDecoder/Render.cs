using DevExpress.Data.Mask.Internal;
using DevExpress.DirectX.Common.Direct3D;
using DevExpress.DirectX.StandardInterop.DirectWrite;
 
using SharpGen.Runtime;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Resources;
using System.Runtime.InteropServices;
using Vortice.D3DCompiler;
using Vortice.Direct2D1;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.Direct3D11.Shader; 
using Vortice.DirectWrite;
using Vortice.DXGI;
using Vortice.Mathematics;
using Win32;
using static DevExpress.Data.Filtering.Helpers.SubExprHelper.ThreadHoppingFiltering;
using Color = Vortice.Mathematics.Color;
using FeatureLevel = Vortice.Direct3D.FeatureLevel;
using IDWriteTextFormat = DevExpress.DirectX.StandardInterop.DirectWrite.IDWriteTextFormat;

namespace RenderANDVideoReaderVIdeoDecoder
{
    public unsafe class Render
    {
        [DllImport("user32.dll", SetLastError = true)] static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        struct RECT { public int Left; public int Top; public int Right; public int Bottom; }

       static private ID3D11Device _device;
        private IDXGIFactory1 _factory;
        private ID3D11DeviceContext _contextD;
        private IDXGISwapChain _swapChain;
        private int _frameWidth;
        private int _frameHeight;

        IDXGIAdapter adapter;

        static int _width;
        static int _height;

        // rgba na potom
        private byte[] _bgraBuffer;
        private int _bgraStride;
      

        private IDWriteTextFormat textFormat;

        private IDXGISwapChain1 swapChain2;
        SwapChainDescription swapChainDesc = new SwapChainDescription
        {
            BufferCount = 1,
            BufferUsage = Usage.RenderTargetOutput,
            BufferDescription = new ModeDescription((uint)_width, (uint)_height, new Rational(60, 1), Format.B8G8R8A8_UNorm),
            SampleDescription = new SampleDescription(1, 0),
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


        
        ID3D11ShaderResourceView* textureSrv;

        //ID3D11SamplerState textureSampler;
        //ID3D11Buffer vertexBuffer2;
        //ID3D11Buffer indexBuffer2;

         private ID3D11Texture2D* backBufferTexture;
        private ID3D11Texture2D* backBufferTexture2;
        private ID3D11Texture2D* texture;  
         private ID3D11Texture2D* texture2;
        private const string VertexShaderInput = "struct VertexShaderInput \n" +
                                                      "{" +
                                                         " DirectX::XMFLOAT2 pos : POSITION;" +
                                                      "};" +
                                                          "struct PixelShaderInput" +
                                                          "{" +
                                                             " float4 pos : SV_POSITION;" +
                                                      "};" +
                                                         "PixelShaderInput SimpleVertexShader(VertexShaderInput input) \n" +
                                                          "{ \n" +
                                                              "PixelShaderInput vertexShaderOutput; \n" +
                                                                   "vertexShaderOutput.pos = float4(input.pos, 0.5f, 1.0f); \n" +
                                                                   "return vertexShaderOutput; \n" +
                                                          "}";
        private const string PixelShaderInput = "struct PixelShaderInput\n" +
                                       "{\n" +
                                       " float4 pos : SV_POSITION;\n" +
                                       "};\n" +
                                       "float4 SimplePixelShader(PixelShaderInput input) : SV_TARGET \n" +
                                       " {\n" +
                                       "return float4(1.0f, 1.0f, 0.0f, 1.0f);\n" +
                                       "}\n";
        
        private const string std = "struct PixelShaderInput\n" +
                                      "{\n" +
                                      "float4 pos : SV_POSITION;\n" +
                                      "float3 color : COLOR0;\n" +
                                      "}\n" +
                                      "float4 main(PixelShaderInput input) : SV_TARGET\n" +
                                      " {\n" +
                                      "return float4(input.color, 1.0f);\n" +
                                      "}\n";
        private const string FragmentSrc = "#version 330 core\n" +
                                           "in vec2 vTex;\n" +
                                           "out vec4 FragColor;\n" +
                                           "uniform sampler2D uTexY;\n" +
                                           "uniform sampler2D uTexUV;\n" +
                                           "vec3 yuv_to_rgb(float y, float u, float v) {\n" +
                                           "    float Y = y;\n" +
                                           "    float U = u - 0.5;\n" +
                                           "    float V = v - 0.5;\n" +
                                           "    float R = Y + 1.402 * V;\n" +
                                           "    float G = Y - 0.344136 * U - 0.714136 * V;\n" +
                                           "    float B = Y + 1.772 * U;\n" +
                                           "    return clamp(vec3(R, G, B), 0.0, 1.0);\n" +
                                           "}\n" +
                                           "void main() {\n" +
                                           "    float y = texture(uTexY, vTex).r;\n" +
                                           "    vec2 uv = texture(uTexUV, vTex).rg;\n" +
                                           "    vec3 rgb = yuv_to_rgb(y, uv.x, uv.y);\n" +
                                           "    FragColor = vec4(rgb, 1.0);\n" +
                                           "}\n";



        static ID3D11VertexShader vertexShaderPositionTexture;
        static ID3D11PixelShader pixelShaderPositionTexture;
        static ID3D11InputLayout inputLayoutPositionTexture;
        static ID3D11ShaderResourceView shaderResourceView;
        static ID3D11SamplerState samplerState;
        private static ID3D11InputLayout** inputLayout;

        public void TesterToster()
        { 
           // int vs = Com
            
        }
        private static int CompileShader(ID3D11ShaderReflectionType type, string src)
        {int shader = 0;
           

            return shader; 
        }
        public void testmain()
        {
            DXGI.CreateDXGIFactory1<IDXGIFactory1>(out var factory);
            if (factory == null)
                return;

            using (factory)
            {
                IDXGIOutput output;
                factory.EnumAdapters(0, out adapter);
                adapter.EnumOutputs(0, out output);
                using var output1 = output.QueryInterface<IDXGIOutput1>();
                var device = D3D11.D3D11CreateDevice(/*adapter,*/ DriverType.Unknown, DeviceCreationFlags.None, featureLevels);
                if (device == null)
                    return;
                var deviceContext = device.ImmediateContext;
                using (device)
                {
                    var rectangle = new Rectangle(0, 0, output.Description.DesktopCoordinates.Right, output.Description.DesktopCoordinates.Bottom);
                    var texture2dDescription = new Texture2DDescription
                    {
                        ArraySize = 1,
                        CPUAccessFlags = CpuAccessFlags.Read | CpuAccessFlags.Write,
                        Format = Format.B8G8R8A8_UNorm,
                        MipLevels = 1,
                        SampleDescription = { Count = 1, Quality = 0 },
                        Usage = ResourceUsage.Staging,
                        Height = (uint)rectangle.Bottom,
                        Width = (uint)rectangle.Right
                    };

                    using var currentFrame = device.CreateTexture2D(texture2dDescription);
                    using var duplicatedOutput = output1.DuplicateOutput(device);
                    using var frame = new Bitmap(rectangle.Right, rectangle.Bottom, PixelFormat.Format32bppRgb);
                    var index = 0;
                    rectangle.X = 0;
                    do
                    {
                        duplicatedOutput.AcquireNextFrame(500, out var frameInfo, out var desktopResource);
                        if (desktopResource != null)
                        {
                            using (desktopResource)
                            {
                                using var tempTexture = desktopResource.QueryInterface<ID3D11Texture2D>();
                                deviceContext.CopyResource(currentFrame, tempTexture);
                                var dataBox = deviceContext.Map(currentFrame, 0, MapMode.Read);
                                var mapDest = frame.LockBits(rectangle, ImageLockMode.WriteOnly, frame.PixelFormat);
                                for (int y = rectangle.Y, sizeInBytesToCopy = rectangle.Width * 4; y < rectangle.Height; y++)
                                {
                                    MemoryHelpers.CopyMemory(mapDest.Scan0 + y * rectangle.Right * 4, (nint)(dataBox.DataPointer + y * dataBox.RowPitch), sizeInBytesToCopy);
                                }
                                deviceContext.Unmap(currentFrame, 0);
                                frame.UnlockBits(mapDest);
                                frame.Save("bitmap" + index++ + ".png", ImageFormat.Png);
                            }
                        }

                        duplicatedOutput.ReleaseFrame();
                    }
                    while (index < 10);
                }
            }
        }
        //public void Start()
        //{
        //   var _isCapturing = true;
        //    DXGI.CreateDXGIFactory1<IDXGIFactory1>(out var factory);
        //    if (factory == null)
        //    {
        //        return;
        //    }
        //    IDXGIOutput output;
        //    factory.EnumAdapters(0, out adapter);
        //    adapter.EnumOutputs(0, out output);
        //    using var output1 = output.QueryInterface<IDXGIOutput1>();
        //    //D3D12.D3D12CreateDevice(adapter, FeatureLevel.Level_12_0, out ID3D12Device? device);
        //  var device=  D3D11.D3D11CreateDevice(  DriverType.Unknown, DeviceCreationFlags.None, featureLevels);
        //    if (device == null)
        //        throw new Exception("Unable to Locate Device.");

        //    // Width/Height of desktop to capture
        //    Rectangle rectangle = new Rectangle(0, 0,
        //            output.Description.DesktopCoordinates.Right,
        //            output.Description.DesktopCoordinates.Bottom);

        //    // Create Staging texture CPU-accessible
        //    var texture2dDescription = new Texture2DDescription
        //    {
        //        ArraySize = 1,
        //        BindFlags = BindFlags.None,
        //        CPUAccessFlags = CpuAccessFlags.Read | CpuAccessFlags.Write,
        //        Format = Format.B8G8R8A8_UNorm,
        //        Height = rectangle.Bottom,
        //        MipLevels = 1,
        //        SampleDescription = { Count = 1, Quality = 0 },
        //        Usage = ResourceUsage.Staging,
        //        Width = rectangle.Right
        //    };

        //    Task.Factory.StartNew(() =>
        //    {
        //        // Duplicate the output
        //        using var duplicatedOutput = output1.DuplicateOutput(device);
        //        while (_isCapturing)
        //        {
        //            try
        //            {
        //                var currentFrame = device.CreateTexture2D(texture2dDescription);

        //                Thread.Sleep(50);
        //                rectangle.X = 0;
        //                duplicatedOutput.AcquireNextFrame(100, out var frameInfo, out var desktopResource);
        //                if (desktopResource == null)
        //                    continue;
        //                var tempTexture = desktopResource.QueryInterface<ID3D11Texture2D>();
        //                device.ImmediateContext.CopyResource(currentFrame, tempTexture);
        //                var dataBox = device.ImmediateContext.Map(currentFrame, 0);
        //                var frame = new Bitmap(rectangle.Right, rectangle.Bottom, PixelFormat.Format32bppRgb);
        //                var mapDest = frame.LockBits(rectangle, ImageLockMode.WriteOnly, frame.PixelFormat);
        //                for (int y = rectangle.Y, sizeInBytesToCopy = rectangle.Width * 4; y < rectangle.Height; y++)
        //                {
        //                    MemoryHelpers.CopyMemory(mapDest.Scan0 + y * rectangle.Right * 4,
        //                        dataBox.DataPointer + y * dataBox.RowPitch, sizeInBytesToCopy);
        //                }

        //                frame.UnlockBits(mapDest);
        //                ScreenRefreshed?.Invoke(this, frame);
        //                desktopResource.Dispose();
        //                frame.Dispose();
        //                tempTexture.Dispose();
        //                currentFrame.Dispose();
        //            }
        //            catch (Exception e)
        //            {
        //                if (e.HResult != Vortice.DXGI.ResultCode.WaitTimeout.Code)
        //                {
        //                    Trace.TraceError(e.Message);
        //                    Trace.TraceError(e.StackTrace);
        //                }
        //            }

        //            duplicatedOutput.ReleaseFrame();
        //        }
        //    });
        //}
        public unsafe void Init(int width, int height, string name, nint testWind/*, FFmpeg.AutoGen.AVFrame frame*/)
        {

            if (GetClientRect((IntPtr)testWind, out RECT rc1))
            {
                int cw = Math.Max(1, rc1.Right - rc1.Left);//rc.Right - rc.Left
                int ch = Math.Max(1, rc1.Bottom - rc1.Top);// rc.Bottom - rc.Top

                _height = ch;
                _width = cw;

            }
             



            swapChainDesc.OutputWindow = testWind;
            _factory = DXGI.CreateDXGIFactory1<IDXGIFactory1>();
            _device = D3D11.D3D11CreateDevice(DriverType.Hardware, DeviceCreationFlags.None, featureLevels);                                       // _device = D3D11.D3D11CreateDevice(DriverType.Hardware, DeviceCreationFlags.BgraSupport, featureLevels);
            _swapChain = _factory.CreateSwapChain(_device, swapChainDesc);
            _contextD = _device.ImmediateContext;


              _factory.EnumAdapters(0, out adapter);
               adapter.EnumOutputs(0, out IDXGIOutput output);
                var output2 = output.QueryInterface<IDXGIOutput1>();


            var bounds = output2.Description.DesktopCoordinates;
            var textureDesc = new Texture2DDescription
            {
                
               CPUAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = (uint)(bounds.Right - bounds.Left),
                Height = (uint)(bounds.Bottom - bounds.Top),
                MiscFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            };

            ID3D11ShaderResourceView pShaderResourceView = null;
            ShaderResourceViewDescription srvDesc = new() 
            {
                Format = textureDesc.Format,
                ViewDimension = ShaderResourceViewDimension.Texture2D  ,//D3D11_SRV_DIMENSION_TEXTURE2D,
                Texture2D = 
                {
                    MipLevels = textureDesc.MipLevels,
                    MostDetailedMip = 0
                },
                
            };
             
            
             Compiler.Compile(VertexShaderInput,"cyka" ,"SimpleVertexShader", "vs_5_0",out Blob blob1, out _);
          

            Rend();
             

            //D3D11_SHADER_RESOURCE_VIEW_DESC
            //var duplication = output2.DuplicateOutput(_device);
            //var currentFrame = _device.CreateTexture2D(textureDesc);

            ////Thread.Sleep(100);

            //duplication.AcquireNextFrame(500, out var frameInfo, out var desktopResource);

            //var tempTexture = desktopResource.QueryInterface<ID3D11Texture2D>();
            //_device.ImmediateContext.CopyResource(currentFrame, tempTexture);

            //var dataBox = _device.ImmediateContext.Map(currentFrame, 0, MapMode.Read, Vortice.Direct3D11.MapFlags.None);

            //var frame = new Bitmap(1920, 1080, PixelFormat.Format32bppRgb);
            //var mapDest = frame.LockBits(new Rectangle(0, 0, 1920, 1080), ImageLockMode.WriteOnly, frame.PixelFormat);
            //for (int y = 0, sizeInBytesToCopy = 1920 * 4; y < 1080; y++)
            //{
            //    MemoryHelpers.CopyMemory(mapDest.Scan0 + y * mapDest.Stride, (nint)(dataBox.DataPointer + y * dataBox.RowPitch), sizeInBytesToCopy);
            //}
            //frame.UnlockBits(mapDest);

            // return frame;




            //ID3D11Texture2D* iD3D11Texture2D;
            //ID3D11RenderTargetView* iD3D11RenderTargetView;
            //var t=  _swapChain.GetBuffer<ID3D11Texture2D>(0);
            //iD3D11Texture2D= &t;



            //var backBufferPtr = _swapChain.GetBuffer<ID3D11Texture2D>(0);
            //var buf = _device.CreateRenderTargetView(backBufferPtr);
            //_contextD.OMSetRenderTargets(0, new ID3D11RenderTargetView[] { buf }, null);




        }

        
        public void Rend(/*FFmpeg.AutoGen.AVFrame frame*/)
        { 
            _contextD.ClearRenderTargetView( _device.CreateRenderTargetView( _swapChain.GetBuffer<ID3D11Texture2D>(0)), Colors.CornflowerBlue);
           _swapChain.Present(0, 0);
            

            //_swapChain.Present(0, PresentFlags.None);
        }

        //        ID3D11Device1 iD3D11Device1 = _device.QueryInterface<ID3D11Device1>();
        //        ID3D11DeviceContext1 iD3D11DeviceContext1 = _contextD.QueryInterface<ID3D11DeviceContext1>();
        //        IDXGIFactory2 factory2 = DXGI.CreateDXGIFactory1<IDXGIFactory2>();
        //        swapChain2 = factory2.CreateSwapChainForHwnd(_device,testWind, swapChainDescription);

        //          backBufferTexture = _swapChain.GetBuffer<ID3D11Texture2D>(0);
        //           backBufferTexture2 = swapChain2.GetBuffer<ID3D11Texture2D>(0);
        //          renderTargetView = _device.CreateRenderTargetView(backBufferTexture);
        //          renderTargetView2 = _device.CreateRenderTargetView(backBufferTexture2);

        //        //BufferDescription bufferDescription = new BufferDescription
        //        //{
        //        //    BindFlags = BindFlags.VertexBuffer,
        //        //    Usage = ResourceUsage.Default,

        //        //    CPUAccessFlags = CpuAccessFlags.None,
        //        //    ByteWidth = (uint)(VertexPositionNormalTexture.SizeInBytes * 4),
        //        //    StructureByteStride = (uint)VertexPositionNormalTexture.SizeInBytes,
        //        //    MiscFlags = ResourceOptionFlags.None
        //        //};
        //        //BufferDescription bufferDescription2 = new BufferDescription
        //        //{
        //        //    BindFlags = BindFlags.IndexBuffer,
        //        //    Usage = ResourceUsage.Default,

        //        //    CPUAccessFlags = CpuAccessFlags.None,
        //        //    ByteWidth = (uint)(VertexPositionNormalTexture.SizeInBytes * 4),
        //        //    StructureByteStride = (uint)VertexPositionNormalTexture.SizeInBytes,
        //        //    MiscFlags = ResourceOptionFlags.None
        //        //};
        //        //this.vertexBuffer = iD3D11Device1.CreateBuffer(bufferDescription, 1);
        //        //this.indexBuffer = iD3D11Device1.CreateBuffer(bufferDescription2, 2);
        //        //this.constantBuffer = iD3D11Device1.CreateConstantBuffer<Matrix4x4>();
        //     //   this.constantBuffer2 = iD3D11Device1.CreateConstantBuffer<Matrix4x4>();

        //        ReadOnlySpan<Color> pixels = stackalloc Color[16] {
        //        new Color(0xFFFFFFFF),
        //        new Color(0x00000000),
        //        new Color(0xFFFFFFFF),
        //        new Color(0x00000000),
        //        new Color(0x00000000),
        //        new Color(0xFFFFFFFF), 
        //        new Color(0x00000000),
        //        new Color(0xFFFFFFFF),
        //        new Color(0xFFFFFFFF),
        //        new Color(0x00000000),
        //        new Color(0xFFFFFFFF),
        //        new Color(0x00000000),
        //        new Color(0x00000000),
        //        new Color(0xFFFFFFFF),
        //        new Color(0x00000000),
        //        new Color(0xFFFFFFFF),
        //    };



        //       var texture2D = _device.CreateTexture2D(pixels, Format.R8G8B8A8_UNorm, 4, 4);
        //        shaderResourceView = _device.CreateShaderResourceView(texture2D);
        //        samplerState = _device.CreateSamplerState(SamplerDescription.PointWrap);

        //        Span<byte> vertexShaderByteCodeCube = CompileBytecode("Cube.hlsl", "VSMain", "vs_4_0");

        //        Span<byte> pixelShaderByteCodeCube = CompileBytecode("Cube.hlsl", "PSMain", "ps_4_0");
        //        InputElementDescription inputElementDescription  = new InputElementDescription
        //        {
        //            SemanticName = "POSITION",
        //            SemanticIndex = 0,
        //            Format = Format.R32G32B32_Float,
        //           //  Classification = 0,
        //            AlignedByteOffset = 0,
        //            // Slot = InputClassification.PerVertexData,
        //            InstanceDataStepRate = 0
        //        };
        //        vertexShaderPositionTexture = _device.CreateVertexShader(vertexShaderByteCodeCube);
        //        pixelShaderPositionTexture = _device.CreatePixelShader(pixelShaderByteCodeCube);
        //        inputLayoutPositionTexture = _device.CreateInputLayout(VertexPositionNormalTexture.InputElements, vertexShaderByteCodeCube);




        //       // this.vertexBuffer2 = _device.CreateBuffer(bufferDescription,1);
        //       // this.indexBuffer2 = _device.CreateBuffer(bufferDescription, 2);

        //        this.texture = _device.CreateTexture2D(new Texture2DDescription
        //        {
        //            Width = (uint)width,
        //            Height = (uint)height,
        //            MipLevels = 1,
        //            ArraySize = 1,
        //            Format = Format.B8G8R8A8_UNorm,
        //            SampleDescription = new SampleDescription(1, 0),
        //            Usage = ResourceUsage.Dynamic,
        //            BindFlags = BindFlags.ShaderResource,
        //            CPUAccessFlags = CpuAccessFlags.Write,
        //            MiscFlags = ResourceOptionFlags.None
        //        });

        //        this.textureSrv = _device.CreateShaderResourceView(this.texture);
        //        this.textureSampler = _device.CreateSamplerState(SamplerDescription.PointWrap);
        //        Rend();
        //       // this.clock = Stopwatch.StartNew();

        //        //textFormat = writeFactory.CreateTextFormat("Arial", 20.0f);
        //        //textFormat.TextAlignment = TextAlignment.Center;
        //        //textFormat.ParagraphAlignment = ParagraphAlignment.Center;


        //        // IDXGISwapChain1 iDXGISwapChain = _factory.CreateSwapChain(_device, swapChainDescription);


        //        // FeatureLevel createdLevel;


        //        //  _context = hr.ImmediateContext;




        //        //  _frameWidth = frame.width;
        //        //  _frameHeight = frame.height;
        //        //  _srcPixFmt = (FFmpeg.AutoGen.AVPixelFormat)frame.format;

        //        ////  EnsureSwsForFrame();

        //        //  _bgraStride = _frameWidth * 4;
        //        //  int bgraSize = _bgraStride * _frameHeight;
        //        //  _bgraBuffer = new byte[bgraSize];

        //        //  var viewport = new Vortice.Mathematics.Viewport(0, 0, _width, _height, 0, 1);
        //        //  _context.RSSetViewport(viewport);

        //        //  //----
        //        //  _swsCtx = FFmpeg.AutoGen.ffmpeg.sws_getContext(
        //        //     _frameWidth, _frameHeight, _srcPixFmt,
        //        //     _frameWidth, _frameHeight, FFmpeg.AutoGen.AVPixelFormat.AV_PIX_FMT_BGRA,
        //        //     FFmpeg.AutoGen.ffmpeg.SWS_BILINEAR, null, null, null);
        //        //  fixed (byte* dstPtr0 = _bgraBuffer)
        //        //  {
        //        //      byte*[] dstData = new byte*[4];
        //        //      int[] dstLinesize = new int[4];

        //        //      dstData[0] = dstPtr0;
        //        //      dstLinesize[0] = _bgraStride;
        //        //      dstData[1] = null;
        //        //      dstData[2] = null;
        //        //      dstData[3] = null;

        //        //      byte*[] srcData = new byte*[4];
        //        //      int[] srcLinesize = new int[4];

        //        //      //for (int i = 0; i < 4; i++)
        //        //      //{
        //        //      //    srcData[i] = frame.data[i];
        //        //      //    srcLinesize[i] = frame.linesize[i];
        //        //      //}

        //        //      int r = FFmpeg.AutoGen.ffmpeg.sws_scale(
        //        //          _swsCtx,
        //        //          srcData, srcLinesize,
        //        //          0, _frameHeight,
        //        //          dstData, dstLinesize
        //        //      );
        //        //      using (var backBuffer = _swapChain.GetBuffer<ID3D11Texture2D>(0))
        //        //      {
        //        //          fixed (byte* pData = _bgraBuffer)
        //        //          {
        //        //             // _context.UpdateSubresource(backBuffer, 0, null, (IntPtr)pData, _bgraStride, 0);
        //        //          }



        //        //          _swapChain.Present(1, PresentFlags.None);
        //        //      }

        //        //}

        //    }


        //    void Rend()
        //    {
        //        _contextD.ClearRenderTargetView(renderTargetView2, Colors.CornflowerBlue);
        //        _contextD.ClearDepthStencilView(depthStencilView2, DepthStencilClearFlags.Depth, 1.0f, 0);


        //        _contextD.OMSetRenderTargets(renderTargetView2, depthStencilView2);
        //        _contextD.RSSetViewport(new Viewport(_width, _height));
        //        _contextD.RSSetScissorRect(_width, _height);

        //        _contextD.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);
        //        _contextD.IASetInputLayout(inputLayoutPositionTexture);
        //        _contextD.IASetVertexBuffer(0, this.vertexBuffer, (uint)VertexPositionNormalTexture.SizeInBytes);
        //        _contextD.IASetIndexBuffer(this.indexBuffer, Format.R16_UInt, 0);
        //        _contextD.VSSetShader(vertexShaderPositionTexture);
        //        _contextD.VSSetConstantBuffer(0, this.constantBuffer);
        //        _contextD.PSSetShader(pixelShaderPositionTexture);
        //        _contextD.PSSetShaderResource(0, shaderResourceView);
        //        _contextD.PSSetSampler(0, samplerState);
        //        _contextD.DrawIndexed(36, 0, 0);
        //        Result result = _swapChain.Present(1, PresentFlags.None);
        //    }


        //    private static int CreateProgram(string vertexSrc, string fragmentSrc)
        //    {// спизжено
        //        int vs = CompileShader(ShaderType.VertexShader, vertexSrc);
        //        int fs = CompileShader(ShaderType.FragmentShader, fragmentSrc);


        //        int program = .CreateProgram();
        //        GL.AttachShader(program, vs);
        //        GL.AttachShader(program, fs);

        //        GL.LinkProgram(program);
        //        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int linked);
        //        if (linked == 0)
        //        {
        //            GL.DeleteShader(vs);
        //            GL.DeleteShader(fs);

        //            GL.DeleteProgram(program);


        //        }
        //        GL.DetachShader(program, vs);
        //        GL.DetachShader(program, fs);

        //        GL.DeleteShader(vs);
        //        GL.DeleteShader(fs);

        //        return program;
        //    }
        //    private static int CompileShader(ShaderType type, string src)
        //    {// спизжено
        //        int shader = GL.CreateShader(type);
        //        GL.ShaderSource(shader, src);
        //        GL.CompileShader(shader);
        //        GL.GetShader(shader, ShaderParameter.CompileStatus, out int compiled);
        //        if (compiled == 0)
        //        {
        //            string log = GL.GetShaderInfoLog(shader);
        //            GL.DeleteShader(shader);


        //        }
        //        return shader;
        //    }
        //    private static Span<byte> CompileBytecode(string shaderName, string entryPoint, string profile)
        //    {
        //        string assetsPath = Path.Combine(AppContext.BaseDirectory, "Assets");
        //        string shaderFile = Path.Combine(assetsPath, shaderName);

        //        Compiler.CompileFromFile(shaderFile, entryPoint, profile, out Blob blob, out _);
        //        return blob.AsBytes();
        //    }
        //    public readonly struct VertexPositionNormalTexture
        //    {
        //        public static readonly unsafe int SizeInBytes = sizeof(VertexPositionNormalTexture);

        //        public static readonly InputElementDescription[] InputElements = {
        //    new("POSITION", 0, Format.R32G32B32_Float, 0, 0),
        //    new("NORMAL", 0, Format.R32G32B32_Float, 12, 0),
        //    new("TEXCOORD", 0, Format.R32G32_Float, 24, 0)
        //};

        //        public VertexPositionNormalTexture(in Vector3 position, in Vector3 normal, in Vector2 textureCoordinate)
        //        {
        //            Position = position;
        //            Normal = normal;
        //            TextureCoordinate = textureCoordinate;
        //        }

        //        public readonly Vector3 Position;
        //        public readonly Vector3 Normal;
        //        public readonly Vector2 TextureCoordinate;
        //    }
    }
}
