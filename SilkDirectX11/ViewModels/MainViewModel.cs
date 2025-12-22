using FFmpeg.AutoGen;
using LibraryForSignalR;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SilkDirectX11.Events;
using SilkDirectX11.Interfaces;
using SilkDirectX11.Model;
using SilkDirectX11.Modules;
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
       
        public MainViewModel(IDialogCoordinator instance, ICameraDAO camera,IEventAggregator eventAggregator , ICameraSettingsDAO cameraSettingsDAO)
        { 
            #region FFMPEG Init
            ffmpeg.RootPath = "Autogen";
            DynamicallyLoadedBindings.Initialize();
            Directory.CreateDirectory("frames");
            #endregion
            #region init
            
            _eventAggregator = eventAggregator;
            _cameraDAO = camera;
            _dialogCoordinator = instance;
            _cameraSettingsDAO = cameraSettingsDAO;
            #endregion
            #region Commands
            AddCameraCommand = new DelegateCommand(async () => await AddCameraDialog());
            LoadedCommand = new DelegateCommand(LoadedExecute);
            CameraSettingsOpen = new DelegateCommand(async () => await CameraSettingsOpenExecute());
            RemoveCameraCommand = new DelegateCommand<WindowsFormsHost>(RemoveCamera);
            SettingsOpenComman = new DelegateCommand(OpenSettings);
            // StartCommand = new AsyncDelegateCommand(StartCameraStream);
            #endregion
            #region SubscribeEvents
            _eventAggregator.GetEvent<CameraEvent>().Subscribe(AddCamExsample);
            _eventAggregator.GetEvent<CloseAddCameraEvent>().Subscribe(s => { _dialogCoordinator.HideMetroDialogAsync(this, AddCameraDialogs); MainVisibility = true; });
            _eventAggregator.GetEvent<CloseSettingsEvent>().Subscribe(s => { _dialogCoordinator.HideMetroDialogAsync(this, SettingsDialog); MainVisibility = true; });
            _eventAggregator.GetEvent<CloseCamersSettingsEvent>().Subscribe(s => { _dialogCoordinator.HideMetroDialogAsync(this, CameraSettingsDialog); MainVisibility = true; } );
           _eventAggregator.GetEvent<CloseReconectCamersEvent>().Subscribe( s=> { try { if (!MainVisibility) _dialogCoordinator.HideMetroDialogAsync(this, ReconectCamera); } catch(Exception ex) { Debug.WriteLine(ex.Message); } MainVisibility = true; });
            #endregion

        }
        private BaseMetroDialog AddCameraDialogs = new CustomDialog();
        private BaseMetroDialog SettingsDialog = new CustomDialog();
        private BaseMetroDialog CameraSettingsDialog = new CustomDialog();
        private BaseMetroDialog ReconectCamera = new CustomDialog();
        public IEventAggregator _eventAggregator;
        ICameraSettingsDAO _cameraSettingsDAO;
        private IDialogCoordinator _dialogCoordinator;
        private ICameraDAO _cameraDAO;
        
     
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
        private bool _mainVisibility = true;
        public bool MainVisibility
        {
            get => _mainVisibility;
            set => SetProperty(ref _mainVisibility, value);
        }

        #region Commands and Methods
        public    void ShowMahapsDialog(string title,string messege)
        {
             
             _dialogCoordinator.ShowModalMessageExternal(this, title, messege);
            
             _dialogCoordinator.ShowProgressAsync(this, title, messege);
        }
        public DelegateCommand AddCameraCommand { get; private set; }
        public  async Task AddCameraDialog()
        {
            MainVisibility = false;
            AddCameraView view = new AddCameraView();
         
            AddCameraDialogs.Title = "Add Camers";
            var gridLenghtConvert = new GridLengthConverter();

            
             AddCameraDialogs.DialogContentWidth = GridLength.Auto;
             AddCameraDialogs.DialogContentMargin = (GridLength)gridLenghtConvert.ConvertFromString("10");
 
            AddCameraDialogs.Content = view.Content;
            AddCameraDialogs.DataContext = view.DataContext;
            

            
              AddCameraDialogs.Width = 800;
            await _dialogCoordinator.ShowMetroDialogAsync(this, AddCameraDialogs);

         }
        public async void CameraProgressBar(string nameCamera)
        {
            MainVisibility = false;
            ReconectCamersView view = new ReconectCamersView();
            view.Title = nameCamera;
            ReconectCamera.Title = nameCamera;
              ReconectCamera.DialogContentWidth = GridLength.Auto;
            ReconectCamera.Width = 400;
            // ReconectCamera.DialogContentMargin = (GridLength)(new GridLengthConverter()).ConvertFromString("10");
            ReconectCamera.Content = view.Content;
            ReconectCamera.DataContext = view.DataContext;
            try
            {
                await _dialogCoordinator.ShowMetroDialogAsync(this, ReconectCamera);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

        }
        public void Message(string statusCamera)
        {
            _eventAggregator.GetEvent<MessegeToReconectCameraEvent>().Publish(statusCamera);
        }
        public void SetProgressBar(bool set)
        {
           _eventAggregator.GetEvent<ProgressBarForReconnectEvent>().Publish(set);
            //MainVisibility = true;
        }

        public void ClouseCamera(string mes)
        {
            _eventAggregator.GetEvent<ClouseCameraModuleEvent>().Publish(mes);
        }

        public async void AddCamExsample(CameraStream camera)
        {
            if (camera == null)
                return;
            if (string.IsNullOrEmpty(camera.CameraName) || string.IsNullOrEmpty(camera.ConnectStrings.mainStream) || string.IsNullOrEmpty(camera.ConnectStrings.subStream))
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

            _dialogCoordinator.HideMetroDialogAsync(this, AddCameraDialogs);
            await _dialogCoordinator.ShowMessageAsync(this, "Success", "Camera added successfully");
            _cameraDAO.SaveCamera(WindowsFormsHosts);

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
                
                WindowsFormsHosts.Add(wfh);

            }
            ListDictionarySettingsCamers.DictionarySettingsCamers = GetSettingsForDictionary(listCamers).ToDictionary(c => c.CameraId, c => c);

        }
        private List<CameraVisualSettings> GetSettingsForDictionary(ObservableCollection<CameraStream> listCamers)
        {

           List<CameraStream> cameraList = new List<CameraStream>();

            foreach (var cam in listCamers)
            {
                cameraList.Add(cam);
            }
            List<CameraVisualSettings> cameraSettings = _cameraSettingsDAO.GetCameraSettings();
            List<CameraVisualSettings> cameraSettingsToRemove = new List<CameraVisualSettings>();
            foreach (var cam in cameraSettings)
            {
                if (!cameraList.Any(c => c.CameraID == cam.CameraId))
                {
                    cameraSettingsToRemove.Add(cam);
                }

            }
            foreach (var cam in cameraSettingsToRemove)
            {
                cameraSettings.Remove(cam);
            }
            return cameraSettings;
        }
        public DelegateCommand SettingsOpenComman { get; private set; }
        private async void OpenSettings()
        {
            MainVisibility = false;
            SettingsView view = new SettingsView();
            SettingsDialog.Title = "Settings";
            var gridLenghtConvert = new GridLengthConverter();
            SettingsDialog.DialogContentWidth = GridLength.Auto;
            SettingsDialog.DialogContentMargin = (GridLength)gridLenghtConvert.ConvertFromString("10");
            SettingsDialog.Content = view.Content;
            SettingsDialog.DataContext = view.DataContext;
            SettingsDialog.Width = 600;
            await _dialogCoordinator.ShowMetroDialogAsync(this, SettingsDialog);
        }
        public DelegateCommand CameraSettingsOpen { get; private set; }
        private async Task CameraSettingsOpenExecute()
        {
            MainVisibility = false;
            CameraSettingsDialog.Title = "Camera Settings";
            var gridLenghtConvert = new GridLengthConverter();
            CamersSettingsView camersSettingsView = new CamersSettingsView();
             
            CameraSettingsDialog.DialogContentWidth = GridLength.Auto;
            CameraSettingsDialog.DialogContentMargin = (GridLength)gridLenghtConvert.ConvertFromString("10");
           
            CameraSettingsDialog.Content = camersSettingsView.Content;
            CameraSettingsDialog.DataContext = camersSettingsView.DataContext;
            CameraSettingsDialog.Width = camersSettingsView.Width;
            CameraSettingsDialog.Height = camersSettingsView.Height;

            await _dialogCoordinator.ShowMetroDialogAsync(this, CameraSettingsDialog);


        }
        

        public DelegateCommand<WindowsFormsHost> RemoveCameraCommand { get; private set; }
        private void RemoveCamera(WindowsFormsHost host)
        {
            if (host == null)
                return;
            WindowsFormsHosts.Remove(host);

            if (host.Tag is CameraConnectStrings tag)
            {
                var toRemove = listCameras.FirstOrDefault(s => s.CameraID == tag.CameraID);
                if (toRemove != null)
                {
                    listCameras.Remove(toRemove);
                }
            }
            _cameraDAO.SaveCamera(WindowsFormsHosts);
        }
        #endregion

        //private void AddCamera()
        //{
        //    AddCameraDialog();

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
        //}

        // public DelegateCommand RemoveCameraCommand { get; private set; }
        //private void  RemoveCamera()
        // {
        //     if (VideoHost == null)
        //         return;
        //     WindowsFormsHosts.Remove(VideoHost);
        //     listCameras.Remove((listCameras.FirstOrDefault(s=> s.CameraID == (VideoHost.Tag as CameraConnectStrings).CameraID)));
        //     _cameraDAO.SaveCamera(WindowsFormsHosts);
        // }


    }
}
