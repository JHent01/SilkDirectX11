using Newtonsoft.Json.Linq;
using Prism.Mvvm;
using SilkDirectX11.Enums;
using SilkDirectX11.Events;
using SilkDirectX11.Interfaces;
using SilkDirectX11.Model;
using SilkDirectX11.Servise;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;

namespace SilkDirectX11.ViewModels
{
    internal class CamersSettingsViewModel : BindableBase
    {
        public CamersSettingsViewModel(IEventAggregator eventAggregator, ICameraSettingsDAO camSettingsDAO, ICameraDAO cameraDAO)
        {
            _cameraSettingsDAO = camSettingsDAO;
            _eventAggregator = eventAggregator;
            CloseComaand = new DelegateCommand(() => { _eventAggregator.GetEvent<CloseCamersSettingsEvent>().Publish("Close"); });
            SaveSettingsSingleComand = new DelegateCommand(SaveSingleSettings);
            _cameraDAO = cameraDAO;
            CameraVisualSettingsList = OnLoaded();
            CopyCommand = new DelegateCommand(() => { VisibilityCheBox = true; }); 
            SaveCommand = new DelegateCommand(SaveSettingsListCamers);
            CanselCopy = new DelegateCommand(() => { VisibilityCheBox = false; });//( )=> { VisibilityCheBox = false; }

        }

       

        IEventAggregator _eventAggregator;
        ICameraSettingsDAO _cameraSettingsDAO;
        ICameraDAO _cameraDAO;
        bool flag = true;
        #region Properties
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
        private bool visibilityButton = true;
        public bool VisibilityButton
        {
            get => visibilityButton;
            set => SetProperty(ref visibilityButton, value);
        }
        private bool isCheckedAll;
        public bool IsCheckedAll
        {
            get => isCheckedAll;
            set => SetProperty(ref isCheckedAll, value, UpdateIsCheckedAll);

        }
        bool visibilitySettings = false;
        public bool VisibilitySettings
        {
            get => visibilitySettings;
            set => SetProperty(ref visibilitySettings, value);
        }
        List<Filters> cameraVisualSettings;
        public List<Filters> CameraVisualSettingsList
        {
            get => cameraVisualSettings;
            set => SetProperty(ref cameraVisualSettings, value);
        }
        #endregion
        #region Metods
        private List<Filters> OnLoaded()
        {

            var listCamers = _cameraDAO.GetAllCameras();

            foreach (var cam in listCamers)
            {
                var item = new IsSelectedViewModel<CameraStream>(cam);
                item.IsSelectedChanged += OnIsSelectedChanged;



                cameraList.Add(item);
            }
            List<Filters> cameraSettings = _cameraSettingsDAO.GetCameraSettings();
            List<Filters> cameraSettingsToRemove = new List<Filters>();
            foreach (var cam in cameraSettings)
            {
                if (!cameraList.Any(c => c.Item.CameraID == cam.CameraId))
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
        private void SelectedCamera()
        {
            VisibilitySettings = true;

            Filters cameraVisualSettings = CameraVisualSettingsList.FirstOrDefault(c => c.CameraId == IsSelectedViewModel.Item.CameraID);
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
            if (flag)
            {
                flag = false;
                bool flags = IsCheckedAll;
                var t = CameraList.Where(c => c.IsSelected != flags).Select(c => c).ToList();
                foreach (var cam in t)
                {
                    cam.IsSelected = flags;
                }
                flag = true;
            }
            //if (CameraList.All(c => c.IsSelected))
            //{
            //    IsCheckedAll = true;
            //}
            //else if (CameraList.All(c => !c.IsSelected))
            //{
            //    IsCheckedAll = false;
            //}

        }
        private void OnIsSelectedChanged(bool obj)
        {
            if (flag)
            {

                if (obj)
                {
                    IsCheckedAll = CameraList.All(c => c.IsSelected);
                    

                }
                else
                {
                    flag = false;
                    IsCheckedAll = false;
                     
                }
                flag = true;
            }
        }
        #endregion
        #region Commands
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
        public DelegateCommand SaveSettingsSingleComand { get; private set; }
        private void SaveSingleSettings()
        {
            Filters settings = null;
            Filters cameraVisualSettings = CameraVisualSettingsList.FirstOrDefault(c => c.CameraId == IsSelectedViewModel.Item.CameraID);
            if (cameraVisualSettings == null)
            {
                var cameraNewVisualSettings = cameraList.FirstOrDefault(c => c.Item.CameraID == IsSelectedViewModel.Item.CameraID);
                settings = new Filters
                {
                    CameraId = cameraNewVisualSettings.Item.CameraID,
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

            }
            else
            {
                settings = new Filters
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
            }
            if (CameraVisualSettingsList.Any(c => c.CameraId == settings.CameraId))
            {
                var existingSettings = CameraVisualSettingsList.First(c => c.CameraId == settings.CameraId);
                CameraVisualSettingsList.Remove(existingSettings);
            }
            CameraVisualSettingsList.Add(settings);
            _cameraSettingsDAO.SaveCameraSettings(CameraVisualSettingsList);
            ListDictionarySettingsCamers.DictionarySettingsCamers = CameraVisualSettingsList.ToDictionary(c => c.CameraId, c => c);

            EventAggregatorProvider.Instance.Publish(CameraVisualSettingsList);

            _eventAggregator.GetEvent<CloseCamersSettingsEvent>().Publish("Close");
        }
        public DelegateCommand SaveCommand { get; private set; }
        private void SaveSettingsListCamers()
        {
            var selectedCameras = CameraList.Where(c => c.IsSelected).Select(c => c.Item).ToList();

            foreach (var cam in selectedCameras)
            {
                var settings = new Filters
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
            EventAggregatorProvider.Instance.Publish(CameraVisualSettingsList);
            VisibilityCheBox = false;
             
        }
        public DelegateCommand CanselCopy { get; private set; }
        public DelegateCommand CloseComaand { get; private set; }
        public DelegateCommand CopyCommand { get; private set; }
        #endregion

        //  public DelegateCommand SelectedCameraComand { get; private set; }
        //private void CanselCopyExecute( )
        //{


        //    VisibilityCheBox = false;
        //}
    }
}
