using FFmpeg.AutoGen;
using System.Reflection;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace RenderANDVideoReaderVIdeoDecoder
{
    public class Program
    {   
        //Task taskRead;
        //Task taskDecode;
        //VideoReader videoRider;
        //VideoStreamDecoder videoStreamDecoder;
        //Queue<AVPacket> pacets = new Queue<AVPacket>();
        static void Main(string[] args)
        {
            bool stoper = true;
            Task taskRead;
            Task taskDecode;
            VideoReader videoRider;
            VideoStreamDecoder videoStreamDecoder;
            Queue<AVPacket> pacets = new Queue<AVPacket>();
            string sourse = "";
            string name = "";
              
            nint testWind = 0;
            if (args.Length >= 3)
            {
                sourse = args[0];
                name = args[1];
                testWind = nint.Parse(args[2]);
            }
            Start(sourse, name, testWind);
            void Start(string _videoSourceTest, string name, nint testWind)
            {


            ffmpeg.RootPath = "Autogen";
            FFmpeg.AutoGen.DynamicallyLoadedBindings.Initialize();

            videoRider = new VideoReader(_videoSourceTest);
            videoStreamDecoder = new VideoStreamDecoder(videoRider);


            taskRead = Task.Factory.StartNew(() => Read(videoRider));
            taskDecode = Task.Factory.StartNew(() => DecodeAllFramesToImages(videoStreamDecoder, name, testWind));


            }
            unsafe Task DecodeAllFramesToImages(VideoStreamDecoder videoStreamDecoder, string name, nint testWind)
            {
                var sourceSize = videoStreamDecoder.FrameSize;
                var sourcePixelFormat = videoStreamDecoder.PixelFormat;
                var destinationSize = sourceSize;
                var destinationPixelFormat = AVPixelFormat.@AV_PIX_FMT_BGRA;
                var vfc = new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat);
                if (pacets.Count == 0)
                {

                    Thread.Sleep(50);

                }
                videoStreamDecoder.TryDecodeNextFrame(out var frame1, pacets.Dequeue());
                var convertedFrame1 = vfc.Convert(frame1);

                //Render rend = new Render();
                Test test = new Test();
                // rend.Init(convertedFrame1.width, convertedFrame1.height, name, testWind);
                //  NewReander newReander = new NewReander();
                //newReander.TestMain(testWind);
                test.Init(convertedFrame1.width, convertedFrame1.height, name, testWind, convertedFrame1);
                while (stoper)
                {
                    if (pacets.Count == 0)
                    {

                        Thread.Sleep(20);

                    }
                    else if (videoStreamDecoder.TryDecodeNextFrame(out var frame, pacets.Dequeue()))
                    {
                        test.PresentFrame(frame);
                        //  rend.PushFrameFromDecoder(frame);

                    }

                }
                //    rend.DestroyProgram();
                //rend = null;
                vfc.Dispose();
                vfc = null;
                test.DisposeResources();
                test = null;
                videoStreamDecoder.Dispose();
                videoStreamDecoder = null;

                return Task.CompletedTask;

            }
            unsafe Task Read(VideoReader videoRide)
            {

                while (stoper)
                {


                    videoRide.ReadPacket(pacets);
                    //if (pacets.Count > 10000)
                    //{
                    //    Thread.Sleep(50);
                    //}
                }
                videoRide.Dispose();
                videoRide = null;
                return Task.CompletedTask;
            }
              async Task Destroy()
            {
            stoper = false;
            await taskRead;
            await taskDecode;
            //videoRider.Dispose();

            videoRider = null;
            // videoStreamDecoder.Dispose();
            // videoStreamDecoder = null;
            GC.Collect();
            }
        }
 
        //public void Start(string _videoSourceTest, string name, nint testWind)
        //{


        //    ffmpeg.RootPath = "Autogen";
        //    FFmpeg.AutoGen.DynamicallyLoadedBindings.Initialize();

        //    videoRider = new VideoReader(_videoSourceTest);
        //    videoStreamDecoder = new VideoStreamDecoder(videoRider);


        //    taskRead=   Task.Factory.StartNew(() => Read(videoRider));
        //    taskDecode = Task.Factory.StartNew(() => DecodeAllFramesToImages(videoStreamDecoder, name, testWind));


        //}
        //unsafe Task DecodeAllFramesToImages(VideoStreamDecoder videoStreamDecoder, string name, nint testWind)
        //{
        //    var sourceSize = videoStreamDecoder.FrameSize;
        //    var sourcePixelFormat = videoStreamDecoder.PixelFormat;
        //    var destinationSize = sourceSize;
        //    var destinationPixelFormat = AVPixelFormat.@AV_PIX_FMT_BGRA;
        //    var vfc = new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat);
        //    if (pacets.Count == 0)
        //    {

        //        Thread.Sleep(50);

        //    }
        //    videoStreamDecoder.TryDecodeNextFrame(out var frame1, pacets.Dequeue());
        //    var convertedFrame1 = vfc.Convert(frame1);

        //    //Render rend = new Render();
        //    Test test = new Test();
        //     // rend.Init(convertedFrame1.width, convertedFrame1.height, name, testWind);
        //    //  NewReander newReander = new NewReander();
        //    //newReander.TestMain(testWind);
        //    test.Init(convertedFrame1.width, convertedFrame1.height, name, testWind, convertedFrame1);
        //    while (stoper)
        //    {
        //        if (pacets.Count == 0)
        //        {

        //            Thread.Sleep(20);

        //        }
        //        else if (videoStreamDecoder.TryDecodeNextFrame(out var frame, pacets.Dequeue()))
        //        {
        //               test.PresentFrame(frame);
        //            //  rend.PushFrameFromDecoder(frame);

        //        }

        //    }
        //    //    rend.DestroyProgram();
        //    //rend = null;
        //    vfc.Dispose();
        //    vfc = null;
        //    test.DisposeResources();
        //    test = null;
        //    videoStreamDecoder.Dispose();
        //      videoStreamDecoder = null;

        //    return Task.CompletedTask;

        //}
        //bool stoper = true;
        //unsafe Task Read(VideoReader videoRide)
        //{

        //    while (stoper)
        //    {


        //        videoRide.ReadPacket(pacets);
        //        //if (pacets.Count > 10000)
        //        //{
        //        //    Thread.Sleep(50);
        //        //}
        //    }
        //    videoRide.Dispose();
        //    videoRide = null;
        //    return Task.CompletedTask;
        //}

        ////public void Stop()
        ////{
        ////    stoper = false;
        ////}   
        //public async Task Destroy()
        //{
        //    stoper = false;
        //    await taskRead;
        //        await taskDecode;
        //    //videoRider.Dispose();

        //    videoRider = null;
        //   // videoStreamDecoder.Dispose();
        //   // videoStreamDecoder = null;
        //    GC.Collect();
        //}
    }
}