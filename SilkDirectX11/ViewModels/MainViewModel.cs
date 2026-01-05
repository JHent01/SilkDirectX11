using FFmpeg.AutoGen;
using LibraryForSignalR;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using RenderANDVideoReaderVIdeoDecoder;
using SilkDirectX11.Events;
using SilkDirectX11.Interfaces;
using SilkDirectX11.Model;
using SilkDirectX11.Modules;
using SilkDirectX11.SignalR;
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
using Vortice.Direct2D1.Effects;

namespace SilkDirectX11.ViewModels
{
    internal class MainViewModel : BindableBase
    {
       
        public MainViewModel(IDialogCoordinator instance, ICameraDAO camera,IEventAggregator eventAggregator , ICameraSettingsDAO cameraSettingsDAO)
        { 
            ConnectedManager.EnsureSignalRAsync();
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
            CameraSettingsOpen = new DelegateCommand(async () => await OpenCameraSettings());
            RemoveCameraCommand = new DelegateCommand<WindowsFormsHost>(RemoveCamera);
            SettingsOpenComman = new DelegateCommand(OpenSettings);
            // StartCommand = new AsyncDelegateCommand(StartCameraStream);
            #endregion
            #region SubscribeEvents
            _eventAggregator.GetEvent<AddCameraEvent>().Subscribe(AddCameraExsample);
            _eventAggregator.GetEvent<CloseAddCameraViewEvent>().Subscribe(s => { Opasity = 1; addCameraView.DialogResult = true; });
            _eventAggregator.GetEvent<CloseSettingsViewEvent>().Subscribe(s => {  Opasity = 1; SettingsView.DialogResult = true; });
            _eventAggregator.GetEvent<CloseCamersSettingsEvent>().Subscribe(s => { Opasity = 1; camersSettingsView.DialogResult = true;   } );
           _eventAggregator.GetEvent<CloseReconectCamersViewEvent>().Subscribe( s=> { if (reconectCamersView.DialogResult!=true) { Opasity = 1; reconectCamersView.DialogResult = true; } });
             
            _eventAggregator.GetEvent<VisibilityChengeEvent>().Subscribe(VisibilitySet);
            #endregion

        }
        private void VisibilitySet(Visibility obj)
        {
            MainVisibility = (obj == Visibility.Visible);
        }
       
        public IEventAggregator _eventAggregator;
        ICameraSettingsDAO _cameraSettingsDAO;
        private IDialogCoordinator _dialogCoordinator;
        private ICameraDAO _cameraDAO;


        private double _opasity = 1;
        public double Opasity
        {
            get => _opasity;
            set => SetProperty(ref _opasity, value);
        }
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
        AddCameraView addCameraView;
        public DelegateCommand AddCameraCommand { get; private set; }
        public  async Task AddCameraDialog()
        {
            Opasity = 0.5;
          
            addCameraView = new AddCameraView();
            addCameraView.Owner = System.Windows.Application.Current.MainWindow;
            addCameraView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addCameraView.Topmost = true;
            addCameraView.ShowDialog();
 
        }
        ReconectCamersView reconectCamersView;
        public async void CameraReconnects(string nameCamera)
        {
           
            Opasity = 0.5;
            reconectCamersView = new ReconectCamersView();
            reconectCamersView.Owner = System.Windows.Application.Current.MainWindow;
            reconectCamersView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            reconectCamersView.Topmost = true;
            reconectCamersView.Title = nameCamera;
            reconectCamersView.ShowDialog();
           
        }
        public void OnMessageToReconect(string statusCamera)
        {
            _eventAggregator.GetEvent<MessegeToReconectCameraViewEvent>().Publish(statusCamera);
        }
        public void OnReconnectCamera(bool set)
        {
           _eventAggregator.GetEvent<ReconnectEvent>().Publish(set);
           
        }

        public void OnClouseCamera(string mes)
        {
            _eventAggregator.GetEvent<ClouseCameraModuleEvent>().Publish(mes);
        }

        public async void AddCameraExsample(CameraStream camera)
        {
            if (camera == null)
                return;
            if (string.IsNullOrEmpty(camera.CameraName) || string.IsNullOrEmpty(camera.ConnectStrings.MainStream) || string.IsNullOrEmpty(camera.ConnectStrings.SubStream))
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Camera name is required");
                return;
            }
            var wfh = new WindowsFormsHost();
            string buf = camera.CameraName.ToString().Replace(".", "_").Replace(";", "_"); 
            wfh.Name = buf;
            wfh.Tag = camera.ConnectStrings;
            wfh.AllowDrop = true;
            wfh.Child = new System.Windows.Forms.Panel { Name = wfh.Name, AutoSize = true };

            WindowsFormsHosts.Add(wfh);
            listCameras.Add(camera);
            addCameraView.DialogResult= true;
            Opasity = 1;
           
            //await _dialogCoordinator.ShowMessageAsync(this, "Success", "Camera added successfully");
            _cameraDAO.SaveCamera(WindowsFormsHosts);

        }
        public DelegateCommand LoadedCommand { get; private set; }
       private ObservableCollection<CameraStream> listCameras = new ObservableCollection<CameraStream>();
        private bool flag;
        private void LoadedExecute()
        {
            
          var listCamers =  _cameraDAO.GetAllCameras();
            if (listCamers.Count == 0)
             return;
            foreach (CameraStream cam in listCamers)
            {   cam.ConnectStrings = new CameraConnectStrings { MainStream = cam.CameraMainStream, SubStream = cam.CameraSubStream, CameraID = cam.CameraID };
                listCameras.Add(cam);
                var wfh = new WindowsFormsHost();
                wfh.Name = cam.CameraName;
                wfh.Tag = cam.ConnectStrings; 
                wfh.AllowDrop = true;
                wfh.Child = new System.Windows.Forms.Panel { Name = wfh.Name, AutoSize = true };
                
                WindowsFormsHosts.Add(wfh);

            }
            ListDictionarySettingsCamers.DictionarySettingsCamers = GetSettingsForDictionary(listCamers).ToDictionary(c => c.CameraId, c => c);

        }
        private List<Filters> GetSettingsForDictionary(ObservableCollection<CameraStream> listCamers)
        {

           List<CameraStream> cameraList = new List<CameraStream>();

            foreach (var cam in listCamers)
            {
                cameraList.Add(cam);
            }
            List<Filters> cameraSettings = _cameraSettingsDAO.GetCameraSettings();
            List<Filters> cameraSettingsToRemove = new List<Filters>();
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
        SettingsView SettingsView;
        public DelegateCommand SettingsOpenComman { get; private set; }
        private async void OpenSettings()
        {
            Opasity = 0.5;
            SettingsView = new SettingsView();
            SettingsView.Owner = System.Windows.Application.Current.MainWindow;
            SettingsView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            SettingsView.Topmost = true;
            SettingsView.ShowDialog();
           
        }
        CamersSettingsView camersSettingsView;
        public DelegateCommand CameraSettingsOpen { get; private set; }
        private async Task OpenCameraSettings()
        {
            Opasity = 0.5;
            camersSettingsView = new CamersSettingsView();
            camersSettingsView.Owner = System.Windows.Application.Current.MainWindow;
            camersSettingsView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            camersSettingsView.Topmost = true;
            camersSettingsView.ShowDialog();
             
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
 
    }
}
