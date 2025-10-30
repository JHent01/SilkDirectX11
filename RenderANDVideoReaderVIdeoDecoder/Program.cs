using FFmpeg.AutoGen;
using System.Reflection;

namespace RenderANDVideoReaderVIdeoDecoder
{
    public class Program
    {
        Queue<AVPacket> pacets = new Queue<AVPacket>();
        static void Main(string[] args)
        {

        }
        public void Start(string _videoSourceTest, string name, nint testWind)
        {


            ffmpeg.RootPath = "Autogen";
            FFmpeg.AutoGen.DynamicallyLoadedBindings.Initialize();

            VideoReader videoRider = new VideoReader(_videoSourceTest);
            VideoStreamDecoder videoStreamDecoder = new VideoStreamDecoder(videoRider);


            Task.Factory.StartNew(() => Read(videoRider));
            Task.Factory.StartNew(() => DecodeAllFramesToImages(videoStreamDecoder, name, testWind));
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

                Thread.Sleep(10);

            }
            videoStreamDecoder.TryDecodeNextFrame(out var frame1, pacets.Dequeue());
            var convertedFrame1 = vfc.Convert(frame1);

            Render rend = new Render();
            Test test = new Test();
             // rend.Init(convertedFrame1.width, convertedFrame1.height, name, testWind);
            //  NewReander newReander = new NewReander();
            //newReander.TestMain(testWind);
         test.Init(convertedFrame1.width, convertedFrame1.height, name, testWind, convertedFrame1);
            while (true)
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
            return Task.CompletedTask;

        }
        unsafe void Read(VideoReader videoRide)
        {

            while (true)
            {


                videoRide.ReadPacket(pacets);
                //if (pacets.Count > 10000)
                //{
                //    Thread.Sleep(50);
                //}
            }
        }
    }
}