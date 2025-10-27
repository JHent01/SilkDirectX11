using FFmpeg.AutoGen;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;

namespace SilkDirectX11.ViewModels
{
    internal class MainViewModel : BindableBase
    {
        public MainViewModel()
        {
            _videoSourceTest = "rtsp://admin:123456@192.168.1.12:554/stream0?username=admin&password=E10ADC3949BA59ABBE56E057F20F883E";
            StartCommand = new AsyncDelegateCommand(StartCameraStream);
          //  Stop = new AsyncDelegateCommand(Stopeds);
            AddCameraCommand = new DelegateCommand(AddCamera);
            ffmpeg.RootPath = "Autogen";
            FFmpeg.AutoGen.DynamicallyLoadedBindings.Initialize();

            Directory.CreateDirectory("frames");


            //EncodeImagesToH264();
            
        }
        int test = 10;
        string _videoSourceTest;
        ObservableCollection<WindowsFormsHost> windowsFormsHosts = new ObservableCollection<WindowsFormsHost>();
        public ObservableCollection<WindowsFormsHost> WindowsFormsHosts
        {
            get => windowsFormsHosts;
            set => SetProperty(ref windowsFormsHosts, value);
        }
        private WindowsFormsHost _videoHost;
        public WindowsFormsHost VideoHost
        {
            get => _videoHost;
            set => SetProperty(ref _videoHost, value);
        }
        public nint RenderTargetHwnd { get; set; }


        public ICommand StartCommand { get; set; }
        public DelegateCommand AddCameraCommand { get; private set; }
        private void AddCamera()
        {

            var g = new WindowsFormsHost();
            g.Name = $"VideoHost{test}";
            //  g.Tag = $"rtsp://admin:123456@192.168.1.{test}:554/stream0?username=admin&password=E10ADC3949BA59ABBE56E057F20F883E";
            g.Tag = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
            g.AllowDrop = true;
            g.Child = new System.Windows.Forms.Panel { Name = g.Name  /*AutoSize=true*/ };
            g.Background = System.Windows.Media.Brushes.AliceBlue;
            WindowsFormsHosts.Add(g);
            if (test == 14) test = 15;
            test++;
        }
        private unsafe async Task StartCameraStream()
        {
            if (VideoHost == null)
                return;
            _videoSourceTest = VideoHost.Tag.ToString();
            //_videoSourceTest = $"rtsp://admin:123456@192.168.1.{test}:554/stream0?username=admin&password=E10ADC3949BA59ABBE56E057F20F883E";
            //    _videoSourceTest = $"http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
            string a = $"Camera {test}";

            var wind = System.Windows.Application.Current.MainWindow;

            var gr = wind.FindName("VideoCanvas1") as Grid;
            VideoHost.Height = gr.ActualHeight;
            VideoHost.Width = gr.ActualWidth;

            if (gr.Children.Contains(VideoHost))
            {
                RenderANDVideoReaderVIdeoDecoder.Program tests = new RenderANDVideoReaderVIdeoDecoder.Program();
                Task.Factory.StartNew(() => tests.Start(_videoSourceTest, a, VideoHost.Handle));
            }
            else
            {
                gr.Children.Add(VideoHost);
                RenderTargetHwnd = VideoHost.Handle;
                RenderANDVideoReaderVIdeoDecoder.Program tests = new RenderANDVideoReaderVIdeoDecoder.Program();
                Task.Factory.StartNew(() => tests.Start(_videoSourceTest, a, RenderTargetHwnd));
            }
        }
    }
}
