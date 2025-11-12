using SilkDirectX11.Events;
using SilkDirectX11.Model;
using SilkDirectX11.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
namespace SilkDirectX11.ViewModels
{
    public class AddCameraViewModel : BindableBase
    {
        public IEventAggregator _eventAggregator;
       public AddCameraViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
           AddCameraCommand = new DelegateCommand(AddCameraExecute);
        }
        string _cameraName;
        public string CameraName
        {
            get => _cameraName;
            set => SetProperty(ref _cameraName, value);
        }
        string _mainStreamURL;
        public string MainSreamURL
        {
            get => _mainStreamURL;
            set => SetProperty(ref _mainStreamURL, value);
        }
        string _subStreamURL;
        public string SubStreamURL
        {
            get => _subStreamURL;
            set => SetProperty(ref _subStreamURL, value);
        }
        //bool flag = false;
        //public bool Flag
        //    {
        //    get => flag;
        //    set => SetProperty(ref flag, value);
        //}
        public DelegateCommand AddCameraCommand { get; private set; }
        private void AddCameraExecute()
        {
            Process s = Process.GetCurrentProcess();
            
            

            _eventAggregator.GetEvent<CameraEvent>().Publish(new Model.CameraStream { CameraName = CameraName, ConnectStrings  = new CameraConnectStrings { mainStream = MainSreamURL,subStream = SubStreamURL } });
         
        }
         
    }
}
