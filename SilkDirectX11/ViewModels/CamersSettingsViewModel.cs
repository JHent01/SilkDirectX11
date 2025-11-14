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
using SilkDirectX11.Interfaces;
using SilkDirectX11.Enums;

namespace SilkDirectX11.ViewModels
{
    internal class CamersSettingsViewModel : BindableBase
    {
        public CamersSettingsViewModel(IEventAggregator eventAggregator, ICameraSettingsDAO camSettingsDAO, ICameraDAO cameraDAO)
        {
            _cameraSettingsDAO = camSettingsDAO;
            _eventAggregator = eventAggregator;
            CloseComaand = new DelegateCommand(Close);
            SaveSettingsComand = new DelegateCommand(SaveSettings);
            _cameraDAO = cameraDAO;
            CameraVisualSettingsList = Loaded();
            CopyCommand = new DelegateCommand(Copy);
            SaveCom = new DelegateCommand(SaveComExecute);
            canselCopy = new DelegateCommand(CanselCopyExecute);

        }
        IEventAggregator _eventAggregator;
        ICameraSettingsDAO _cameraSettingsDAO;
        ICameraDAO _cameraDAO;

        ObservableCollection<IsSelectedViewModel<CameraStream>> cameraList = new ObservableCollection<IsSelectedViewModel<CameraStream>>();
        public ObservableCollection<IsSelectedViewModel<CameraStream>> CameraList
        {
            get => cameraList;
            set
            {
                SetProperty(ref cameraList, value);

            }
        }
        private EnumRotation rotation = EnumRotation.Defoult;
        public EnumRotation Rotation
        {
            get => rotation;
            set => SetProperty(ref rotation, value);
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

        public DelegateCommand CloseComaand { get; private set; }
        private void Close()
        {
            _eventAggregator.GetEvent<CloseCamersSettingsEvent>().Publish("Close");

        }
        private IsSelectedViewModel<CameraStream> isSelectedViewModel;
        public IsSelectedViewModel<CameraStream> IsSelectedViewModel
        {
            get => isSelectedViewModel;
            set
            {
                SetProperty(ref isSelectedViewModel, value);
                SelectedCamera();

            }
        }



        List<CameraVisualSettings> cameraVisualSettings;
        public List<CameraVisualSettings> CameraVisualSettingsList
        {
            get => cameraVisualSettings;
            set => SetProperty(ref cameraVisualSettings, value);
        }
        public DelegateCommand SaveSettingsComand { get; private set; }
        private void SaveSettings()
        {
            CameraVisualSettings cameraVisualSettings = CameraVisualSettingsList.FirstOrDefault(c => c.CameraId == IsSelectedViewModel.Item.CameraID);

            var settings = new CameraVisualSettings
            {
                CameraId = cameraVisualSettings.CameraId,
                Rotation = this.Rotation.ToString(),
                Brightness = this.Brightness,
                Contrast = this.Contrast,
                Saturation = this.Saturation,
                Hue = this.Hue,
                NoiseReduction = this.NoiseReduction,
                StereoAdjustment = this.StereoAdjustment,
                EdgeEnhancement = this.EdgeEnhancement,
                AnamorphicScaling = this.AnamorphicScaling
            };
            if (CameraVisualSettingsList.Any(c => c.CameraId == settings.CameraId))
            {
                var existingSettings = CameraVisualSettingsList.First(c => c.CameraId == settings.CameraId);
                CameraVisualSettingsList.Remove(existingSettings);
            }
            CameraVisualSettingsList.Add(settings);
            _cameraSettingsDAO.SaveCameraSettings(CameraVisualSettingsList);
            ListDictionarySettingsCamers.DictionarySettingsCamers = CameraVisualSettingsList.ToDictionary(c => c.CameraId, c => c);
            //var selectedCameras = CameraList.Where(c => c.IsSelected).Select(c => c.Item).ToList();

            //foreach (var cam in selectedCameras)
            //{
            //    var settings = new CameraVisualSettings
            //    {
            //        CameraId = cam.CameraID,
            //        Rotation = this.Rotation.ToString(),
            //        Brightness = this.Brightness,
            //        Contrast = this.Contrast,
            //        Saturation = this.Saturation,
            //        Hue = this.Hue,
            //        NoiseReduction = this.NoiseReduction,
            //        StereoAdjustment = this.StereoAdjustment,
            //        EdgeEnhancement = this.EdgeEnhancement,
            //        AnamorphicScaling = this.AnamorphicScaling
            //    };
            //    if (CameraVisualSettingsList.Any(c => c.CameraId == cam.CameraID))
            //    {
            //        var existingSettings = CameraVisualSettingsList.First(c => c.CameraId == cam.CameraID);
            //        CameraVisualSettingsList.Remove(existingSettings);
            //    }
            //    CameraVisualSettingsList.Add(settings);

            //}
            //_cameraSettingsDAO.SaveCameraSettings(CameraVisualSettingsList);
            //ListDictionarySettingsCamers.DictionarySettingsCamers = CameraVisualSettingsList.ToDictionary(c => c.CameraId, c => c);
            //  _eventAggregator.GetEvent<SaveCameraSettingsEvent>().Publish(settings);

        }
        private List<CameraVisualSettings> Loaded()
        {

            var listCamers = _cameraDAO.GetAllCameras();

            foreach (var cam in listCamers)
            {
                var item = new IsSelectedViewModel<CameraStream>(cam);
                cameraList.Add(item);
            }
            List<CameraVisualSettings> cameraSettings = _cameraSettingsDAO.GetCameraSettings();
            foreach (var cam in cameraSettings)
            {
                if (!cameraList.Any(c => c.Item.CameraID == cam.CameraId))
                {
                    cameraSettings.Remove(cam);
                }

            }

            return cameraSettings;
        }

        public DelegateCommand SelectedCameraComand { get; private set; }
        private void SelectedCamera()
        {


            CameraVisualSettings cameraVisualSettings = CameraVisualSettingsList.FirstOrDefault(c => c.CameraId == IsSelectedViewModel.Item.CameraID);
            if (cameraVisualSettings != null)
            {
                Rotation = (EnumRotation)Enum.Parse(typeof(EnumRotation), cameraVisualSettings.Rotation);
                Brightness = cameraVisualSettings.Brightness;
                Contrast = cameraVisualSettings.Contrast;
                Saturation = cameraVisualSettings.Saturation;
                Hue = cameraVisualSettings.Hue;
                NoiseReduction = cameraVisualSettings.NoiseReduction;
                StereoAdjustment = cameraVisualSettings.StereoAdjustment;
                EdgeEnhancement = cameraVisualSettings.EdgeEnhancement;
                AnamorphicScaling = cameraVisualSettings.AnamorphicScaling;

            }
            else
            {
                Rotation = EnumRotation.Defoult;
                Brightness = 0;
                Contrast = 0;
                Saturation = 0;
                Hue = 0;
                NoiseReduction = 0;
                StereoAdjustment = 0;
                EdgeEnhancement = 0;
                AnamorphicScaling = 0;

            }
        }
        public void UpdateIsCheckedAll()
        {
            if (CameraList.All(c => c.IsSelected))
            {
                IsCheckedAll = true;
            }
            else if (CameraList.All(c => !c.IsSelected))
            {
                IsCheckedAll = false;
            }
            
        }
        private bool visibilityCheBox = false;
        public bool VisibilityCheBox
        {
            get => visibilityCheBox;
            set
            {
                SetProperty(ref visibilityCheBox, value); 
                VisibilityButton = !value;
            }
        }
        private bool visibilityButton= true;
        public bool VisibilityButton
        {
            get => visibilityButton;
            set => SetProperty(ref visibilityButton, value);
        }
        public DelegateCommand CopyCommand { get; private set; }
        private void Copy()
        {
            VisibilityCheBox = true;


        }
        bool flag = true;
        private bool isCheckedAll;
        public bool IsCheckedAll
        {
            get => isCheckedAll;
            set
            {
                SetProperty(ref isCheckedAll, value, UpdateIsCheckedAll);
                

                //var t = CameraList.Where(c => c.IsSelected != value ).Select(c=>c).ToList();
                //foreach (var cam in t)
                //{
                //    cam.IsSelected = isCheckedAll;
                //}


                //foreach (var cam in CameraList)
                //{
                //    cam.IsSelected = isCheckedAll;
                //}
            }
        }
        



        public DelegateCommand SaveCom { get; private set; }
        private void SaveComExecute()
        {
            var selectedCameras = CameraList.Where(c => c.IsSelected).Select(c => c.Item).ToList();

            foreach (var cam in selectedCameras)
            {
                var settings = new CameraVisualSettings
                {
                    CameraId = cam.CameraID,
                    Rotation = this.Rotation.ToString(),
                    Brightness = this.Brightness,
                    Contrast = this.Contrast,
                    Saturation = this.Saturation,
                    Hue = this.Hue,
                    NoiseReduction = this.NoiseReduction,
                    StereoAdjustment = this.StereoAdjustment,
                    EdgeEnhancement = this.EdgeEnhancement,
                    AnamorphicScaling = this.AnamorphicScaling
                };
                if (CameraVisualSettingsList.Any(c => c.CameraId == cam.CameraID))
                {
                    var existingSettings = CameraVisualSettingsList.First(c => c.CameraId == cam.CameraID);
                    CameraVisualSettingsList.Remove(existingSettings);
                }
                CameraVisualSettingsList.Add(settings);

            }
            _cameraSettingsDAO.SaveCameraSettings(CameraVisualSettingsList);
            ListDictionarySettingsCamers.DictionarySettingsCamers = CameraVisualSettingsList.ToDictionary(c => c.CameraId, c => c);
            visibilityCheBox = false;

            //CameraVisualSettings cameraVisualSettings = CameraVisualSettingsList.FirstOrDefault(c => c.CameraId == IsSelectedViewModel.Item.CameraID);

            //var settings = new CameraVisualSettings
            //{
            //    CameraId = cameraVisualSettings.CameraId,
            //    Rotation = this.Rotation.ToString(),
            //    Brightness = this.Brightness,
            //    Contrast = this.Contrast,
            //    Saturation = this.Saturation,
            //    Hue = this.Hue,
            //    NoiseReduction = this.NoiseReduction,
            //    StereoAdjustment = this.StereoAdjustment,
            //    EdgeEnhancement = this.EdgeEnhancement,
            //    AnamorphicScaling = this.AnamorphicScaling
            //};
            //if (CameraVisualSettingsList.Any(c => c.CameraId == settings.CameraId))
            //{
            //    var existingSettings = CameraVisualSettingsList.First(c => c.CameraId == settings.CameraId);
            //    CameraVisualSettingsList.Remove(existingSettings);
            //}
            //CameraVisualSettingsList.Add(settings);
            //ListDictionarySettingsCamers.DictionarySettingsCamers = CameraVisualSettingsList.ToDictionary(c => c.CameraId, c => c);
            //VisibilityCheBox = false;
        }
        public DelegateCommand canselCopy { get; private set; }
        private void CanselCopyExecute()
        {
            VisibilityCheBox = false;
        }
    }
}
