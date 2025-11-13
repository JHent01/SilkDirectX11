using FFmpeg.AutoGen;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SilkDirectX11.Events;
using SilkDirectX11.Interfaces;
using SilkDirectX11.Model;
using SilkDirectX11.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Unity.Injection;

namespace SilkDirectX11.ViewModels
{
    internal class MainViewModel : BindableBase
    {
        private BaseMetroDialog dialog = new CustomDialog();
        private BaseMetroDialog settingsDialog = new CustomDialog();
        private BaseMetroDialog CameraSettingsDialog = new CustomDialog();



        public IEventAggregator _eventAggregator;
        public MainViewModel(IDialogCoordinator instance, ISettingsDAO dAO,ICameraDAO camera,IEventAggregator eventAggregator )
        { 
            #region FFMPEG Init
            ffmpeg.RootPath = "Autogen";
            DynamicallyLoadedBindings.Initialize();
            Directory.CreateDirectory("frames");
            #endregion
            #region init
            _settingsDAO = dAO;
            _eventAggregator = eventAggregator;
            _cameraDAO = camera;
            _dialogCoordinator = instance;
            #endregion

            #region Commands
            AddCameraCommand = new DelegateCommand(AddCamera);
            LoadedCommand = new DelegateCommand(LoadedExecute);
            CameraSettingsOpen = new DelegateCommand(async () => await CameraSettingsOpenExecute());

            SettingsOpenComman = new DelegateCommand(OpenSettings);
            // StartCommand = new AsyncDelegateCommand(StartCameraStream);
            #endregion



            #region SubscribeEvents
            _eventAggregator.GetEvent<CameraEvent>().Subscribe(AddCam);
            _eventAggregator.GetEvent<CloseAddCameraEvent>().Subscribe(s => { _dialogCoordinator.HideMetroDialogAsync(this, dialog); });
            _eventAggregator.GetEvent<CloseSettingsEvent>().Subscribe(s => { _dialogCoordinator.HideMetroDialogAsync(this, settingsDialog); });
            _eventAggregator.GetEvent<CloseCamersSettingsEvent>().Subscribe(s => { _dialogCoordinator.HideMetroDialogAsync(this, CameraSettingsDialog); });
            #endregion

            //    async (camera) =>
            //{
            //  //  if (camera == null)
            //  //      return;
            //  //  if (string.IsNullOrEmpty( camera.CameraName))
            //  //  {
            //  //      await _dialogCoordinator.ShowMessageAsync(this, "Error", "Camera name is required");
            //  //      return;
            //  //  }
            //  //  var wfh = new WindowsFormsHost();
            //  //  string buf = camera.CameraName.ToString().Replace(".", "_").Replace(";", "_");//Replace("", "_")
            //  //  wfh.Name = buf;
            //  //  wfh.Tag = camera.ConnectStrings; 
            //  //  wfh.AllowDrop = true;
            //  //  wfh.Child = new System.Windows.Forms.Panel { Name = wfh.Name, AutoSize = true };

            //  //  WindowsFormsHosts.Add(wfh);

            //  //  // _dialogCoordinator.HideMetroDialogAsync(this, dialog);

            //  //  dialog.RequestCloseAsync();
            //  //_dialogCoordinator.HideMetroDialogAsync(this, dialog);

            //  //  //_cameraDAO.SaveCamera(WindowsFormsHosts);

            //  //  await  _dialogCoordinator.ShowMessageAsync(this, "Success", "Camera added successfully");
            //  //  dialog.RequestCloseAsync();
            //  //  dialog = new CustomDialog();
            //  //  dialog.RequestCloseAsync();

            //});

        }

        public async  void AddCam(CameraStream camera)
        {
            if (camera == null)
                return;
            if (string.IsNullOrEmpty(camera.CameraName)||string.IsNullOrEmpty(camera.ConnectStrings.mainStream)||string.IsNullOrEmpty(camera.ConnectStrings.subStream))
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Camera name is required");
                return;
            }
            var wfh = new WindowsFormsHost();
            string buf = camera.CameraName.ToString().Replace(".", "_").Replace(";", "_");//Replace("", "_")
            wfh.Name = buf;
            wfh.Tag = camera.ConnectStrings;
            wfh.AllowDrop = true;
            wfh.Child = new System.Windows.Forms.Panel { Name = wfh.Name, AutoSize = true };

            WindowsFormsHosts.Add(wfh);
            listCameras.Add(camera);

            _dialogCoordinator.HideMetroDialogAsync(this, dialog);
             await _dialogCoordinator.ShowMessageAsync(this, "Success", "Camera added successfully");
            _cameraDAO.SaveCamera(WindowsFormsHosts);

            //BaseMetroDialog bb = dialog as BaseMetroDialog;
            //bb.RequestCloseAsync();
            //bb.WaitForCloseAsync    ();
            //dialog.WaitForCloseAsync();

            //dialog.RequestCloseAsync();
            // dialog = new CustomDialog();
            // dialog.RequestCloseAsync();
        }

        private IDialogCoordinator _dialogCoordinator;
        private ISettingsDAO _settingsDAO;
        private ICameraDAO _cameraDAO;
        int test = 10;
        //string _videoSourceTest;
        ObservableCollection<WindowsFormsHost> windowsFormsHosts = new ObservableCollection<WindowsFormsHost>();
        public ObservableCollection<WindowsFormsHost> WindowsFormsHosts
        {
            get => windowsFormsHosts;
            set
            {
                SetProperty(ref windowsFormsHosts, value);
                
            }
        }
        private WindowsFormsHost _videoHost;
        public WindowsFormsHost VideoHost
        {
            get => _videoHost;
            set => SetProperty(ref _videoHost, value);
        }
     //   public nint RenderTargetHwnd { get; set; }

         
        public async Task ShowMahapsDialog(string title,string messege)
        { 
             await _dialogCoordinator.ShowMessageAsync(this, title, messege);
            //ShowDialog();
        }

        public  async Task ShowDialog()
        {
            //CustomDialog dialog = new CustomDialog();
           AddCameraView view = new AddCameraView();
            //dialog = view;
            dialog.Title = "Add Camers";
            var gridLenghtConvert = new GridLengthConverter();

            // dialog.Content = view.Content;
            // dialog.DataContext = view.DataContext;
             dialog.DialogContentWidth = GridLength.Auto;
             dialog.DialogContentMargin = (GridLength)gridLenghtConvert.ConvertFromString("10");

            //MetroDialogSettings metroDialogSettings = new MetroDialogSettings()
            //{
            //    AffirmativeButtonText = "Add",
            //    NegativeButtonText = "Cancel",
            //    AnimateShow = true,
            //    AnimateHide = true,
            //    OwnerCanCloseWithDialog = true,
                

            //};
            dialog.Content = view.Content;
            dialog.DataContext = view.DataContext;
            

            //dialog.Height = 500;
              dialog.Width = 800;
            await _dialogCoordinator.ShowMetroDialogAsync(this, dialog);

            // var wind = System.Windows.Application.Current.MainWindow;


            // _dialogCoordinator.HideMetroDialogAsync(this,dialog);
            //  dialog.ShowModalDialogExternally(wind);
            //  dialog.WaitForCloseAsync();
            // dialog.ShowDialogExternally(wind);


            // BaseMetroDialog bb = dialog as BaseMetroDialog;
            // _dialogCoordinator.ShowMetroDialogAsync(this, dialog);


            //bb.ShowDialogExternally();
            // dialog.RequestCloseAsync();
            //    dialog.WaitForCloseAsync();

            // dialog.RequestCloseAsync();
            //return Task.CompletedTask;
            // dialog = new CustomDialog();
        }


        public ICommand StartCommand { get; set; }
        public DelegateCommand AddCameraCommand { get; private set; }

        private void AddCamera()
        {
            ShowDialog();

//rtsp://admin:123456@192.168.1.11:554/stream0?username=admin&password=E10ADC3949BA59ABBE56E057F20F883E
           // var g = new WindowsFormsHost();
           // g.Name = $"VideoHost{test}";
           //  g.Tag = new CameraConnectStrings { mainStream = $"rtsp://admin:123456@192.168.1.{test}:554/stream0?username=admin&password=E10ADC3949BA59ABBE56E057F20F883E", subStream = $"rtsp://admin:123456@192.168.1.{test}:554/stream1?username=admin&password=E10ADC3949BA59ABBE56E057F20F883E" }; //$"rtsp://admin:123456@192.168.1.{test}:554/stream0?username=admin&password=E10ADC3949BA59ABBE56E057F20F883E";
           //// g.Tag = new CameraConnectStrings { mainStream = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4" , subStream = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4" };
           // g.AllowDrop = true;
           // g.Child = new System.Windows.Forms.Panel { Name = g.Name , AutoSize = true };
           // g.Background = System.Windows.Media.Brushes.AliceBlue;
           // WindowsFormsHosts.Add(g);
           // if (test == 14) test = 15;
           // test++;
        }
        public DelegateCommand LoadedCommand { get; private set; }
       private ObservableCollection<CameraStream> listCameras = new ObservableCollection<CameraStream>();
        private void LoadedExecute()
        {
          var listCamers =  _cameraDAO.GetAllCameras();
            if (listCamers.Count == 0)
             return;
            foreach (CameraStream cam in listCamers)
            {   cam.ConnectStrings = new CameraConnectStrings { mainStream = cam.CameraMainStream, subStream = cam.CameraSubStream, CameraID = cam.CameraID };
                listCameras.Add(cam);
                var wfh = new WindowsFormsHost();
                wfh.Name = cam.CameraName;
                wfh.Tag = cam.ConnectStrings;// = new CameraConnectStrings { mainStream = cam.CameraMainStream,subStream = cam.CameraSubStream , CameraID = cam.CameraID}; //new CameraConnectStrings { mainStream = $"rtsp://admin:123456@192.168.1.{test}:554/stream0?username=admin&password=E10ADC3949BA59ABBE56E057F20F883E", subStream = $"rtsp://admin:123456@192.168.1.{test}:554/stream1?username=admin&password=E10ADC3949BA59ABBE56E057F20F883E" }; //$"rtsp://admin:123456@192.168.1.{test}:554/stream0?username=admin&password=E10ADC3949BA59ABBE56E057F20F883E";
                                                                                                                                                                                                                                                                                                          // g.Tag = new CameraConnectStrings { mainStream = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4" , subStream = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4" };
                wfh.AllowDrop = true;
                wfh.Child = new System.Windows.Forms.Panel { Name = wfh.Name, AutoSize = true };
                //wfh.Background = System.Windows.Media.Brushes.AliceBlue;
                WindowsFormsHosts.Add(wfh);

            }
                
          
        }
        public DelegateCommand SettingsOpenComman { get; private set; }
       
        private async void OpenSettings()
        {
            SettingsView view = new SettingsView();
            settingsDialog.Title = "Settings";
            var gridLenghtConvert = new GridLengthConverter();
            settingsDialog.DialogContentWidth = GridLength.Auto;
            settingsDialog.DialogContentMargin = (GridLength)gridLenghtConvert.ConvertFromString("10");
            settingsDialog.Content = view.Content;
            settingsDialog.DataContext = view.DataContext;
            settingsDialog.Width = 600;
            await _dialogCoordinator.ShowMetroDialogAsync(this, settingsDialog);
        }
        public DelegateCommand CameraSettingsOpen { get; private set; }
        private async Task CameraSettingsOpenExecute()
        {
            CameraSettingsDialog.Title = "Camera Settings";
            var gridLenghtConvert = new GridLengthConverter();
            CamersSettingsView camersSettingsView = new CamersSettingsView();

            var cameraList = new ObservableCollection<IsSelectedViewModel<CameraStream>>();
            foreach (var cam in listCameras)
            {
                cameraList.Add(new IsSelectedViewModel<CameraStream> ( cam));
            }
            (camersSettingsView.DataContext as CamersSettingsViewModel).CameraList = cameraList;

            CameraSettingsDialog.DialogContentWidth = GridLength.Auto;
            CameraSettingsDialog.DialogContentMargin = (GridLength)gridLenghtConvert.ConvertFromString("10");
           
            CameraSettingsDialog.Content = camersSettingsView.Content;
            CameraSettingsDialog.DataContext = camersSettingsView.DataContext;
            CameraSettingsDialog.Width = camersSettingsView.Width;

            await _dialogCoordinator.ShowMetroDialogAsync(this, CameraSettingsDialog);


        }



        private unsafe async Task StartCameraStream()
        { 
        //{
        //    if (VideoHost == null)
        //        return;
        //    CameraConnectStrings tag = (CameraConnectStrings)VideoHost.Tag;
        //    string _videoSourceTest = tag.subStream;
        //    //_videoSourceTest = VideoHost.Tag.ToString();
        //    //_videoSourceTest = $"rtsp://admin:123456@192.168.1.{test}:554/stream0?username=admin&password=E10ADC3949BA59ABBE56E057F20F883E";
        //    //    _videoSourceTest = $"http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
        //    string a = $"Camera {test}";

        //    var wind = System.Windows.Application.Current.MainWindow;

        //    var gr = wind.FindName("VideoCanvas1") as Grid;
        //    VideoHost.Height = gr.ActualHeight;
        //    VideoHost.Width = gr.ActualWidth;
        //    //  Window tt = new Window();
        //    //tt.Width = 800;
        //    //tt.Height = 600;
        //    //tt.Content = VideoHost;
        //    if (gr.Children.Contains(VideoHost))
        //    {
        //        var panel = VideoHost.Child as System.Windows.Forms.Panel;
        //        panel.CreateControl();
        //        // RenderTargetHwnd = VideoHost.Handle;
        //        RenderTargetHwnd = panel.Handle;
        //      //  RenderANDVideoReaderVIdeoDecoder.Program tests = new RenderANDVideoReaderVIdeoDecoder.Program();
        //  //      Task.Factory.StartNew(() => tests.Start(_videoSourceTest, a, RenderTargetHwnd /*VideoHost.Handle*/));
        //    }
        //    else
        //    {
        //        gr.Children.Add(VideoHost);
        //        var panel = VideoHost.Child as System.Windows.Forms.Panel;
        //        panel.CreateControl();
        //       // RenderTargetHwnd = VideoHost.Handle;
        //       RenderTargetHwnd = panel.Handle;
        //      //  RenderANDVideoReaderVIdeoDecoder.Program tests = new RenderANDVideoReaderVIdeoDecoder.Program();
        //  //      Task.Factory.StartNew(() => tests.Start(_videoSourceTest, a, RenderTargetHwnd));
            
        }
    }
}
