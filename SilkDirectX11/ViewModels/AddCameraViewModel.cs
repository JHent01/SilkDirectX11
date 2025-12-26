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
           AddCameraCommand = new DelegateCommand(AddCamera);
            CanselCommand = new DelegateCommand(Cansel);
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
        private void AddCamera()
        {  
            //if (string.IsNullOrEmpty(CameraName) || string.IsNullOrEmpty(MainSreamURL)||string.IsNullOrEmpty(CameraName))
            //{
            //    System.Windows.MessageBox.Show("Camera Name and Main Stream URL are required.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            _eventAggregator.GetEvent<AddCameraEvent>().Publish(new Model.CameraStream { CameraName = CameraName, ConnectStrings  = new CameraConnectStrings { MainStream = MainSreamURL,SubStream = SubStreamURL, CameraID =   Guid.NewGuid() }  });
         
        }
        public DelegateCommand CanselCommand { get; private set; }
        private void Cansel()
        {
            _eventAggregator.GetEvent<CloseAddCameraViewEvent>().Publish("Cansel");
        }

    }
}
