using SilkDirectX11.Events;
using SilkDirectX11.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;
using Prism.Mvvm;

namespace SilkDirectX11.ViewModels
{
    internal class CamersSettingsViewModel : BindableBase
    {
        public CamersSettingsViewModel(IEventAggregator eventAggregator) 
        {
            _eventAggregator = eventAggregator;
            CloseComaand = new DelegateCommand(Close);
        }
        IEventAggregator _eventAggregator;



        ObservableCollection<IsSelectedViewModel<CameraStream>> cameraList = new ObservableCollection<IsSelectedViewModel<CameraStream>>();
        public ObservableCollection<IsSelectedViewModel<CameraStream>> CameraList
        {
            get => cameraList;
            set
            {
                SetProperty(ref cameraList, value);

            }
        }
        private string Rotation;
        public string rotation
        {
            get => Rotation;
            set => SetProperty(ref Rotation, value);
        }


        private int brightness;
        public int Brightness
        {
            get => brightness;
            set => SetProperty(ref brightness, value);
        }
        private int contrast;
        public int Contrast
        {
            get => contrast;
            set => SetProperty(ref contrast, value);
        }
        private int saturation;
        public int Saturation
        {
            get => saturation;
            set => SetProperty(ref saturation, value);
        }
        private int hue;
        public int Hue
        {
            get => hue;
            set => SetProperty(ref hue, value);
        }
        private int noiseReduction;
        public int NoiseReduction
        {
            get => noiseReduction;
            set => SetProperty(ref noiseReduction, value);
        }
        private int stereoAdjustment;
        public int StereoAdjustment
        {
            get => stereoAdjustment;
            set => SetProperty(ref stereoAdjustment, value);
        }
        private int edgeEnhancement;
        public int EdgeEnhancement
        {
            get => edgeEnhancement;
            set => SetProperty(ref edgeEnhancement, value);
        }
        private int anamorphicScaling;
        public int AnamorphicScaling
        {
            get => anamorphicScaling;
            set => SetProperty(ref anamorphicScaling, value);
        }

        public DelegateCommand CloseComaand { get;private set; }
        private void Close()
        {
            _eventAggregator.GetEvent<CloseCamersSettingsEvent>().Publish("Close");

        }

    }
}
