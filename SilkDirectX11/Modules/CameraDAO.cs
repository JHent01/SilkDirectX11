using SilkDirectX11.Interfaces;
using SilkDirectX11.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;
namespace SilkDirectX11.Modules
{
    internal class CameraDAO : ICameraDAO
    {
        private ISettingsDAO _settingsDAO;
        public CameraDAO(ISettingsDAO settingsDAO)
        {
            _settingsDAO = settingsDAO;
        }

        public void SaveCamera(ObservableCollection<WindowsFormsHost> whf)
        {
            try
            {
                ObservableCollection<CameraStream> cameras = new ObservableCollection<CameraStream>();
                foreach (var cam in whf)
                {
                   cameras.Add(new CameraStream
                    {
                        CameraName = cam.Name,
                        ConnectStrings = cam.Tag as CameraConnectStrings,
                        CameraMainStream = (cam.Tag as CameraConnectStrings)?.MainStream,
                        CameraSubStream = (cam.Tag as CameraConnectStrings)?.SubStream,
                        CameraID = (cam.Tag as CameraConnectStrings).CameraID ,


                   });
                }
               
                var path = _settingsDAO.ReadGeneralSettings();
                if (string.IsNullOrEmpty(path.SavePathSettings))
                    return;
                 string json = System.Text.Json.JsonSerializer.Serialize(cameras);
                File.WriteAllText(Path.Combine(path.SavePathSettings,"Cameras.json"), json);
                
            }
            catch (Exception ex)
            {
              
                throw new Exception("Error saving camera", ex);
            }

        }

        public ObservableCollection<CameraStream> GetAllCameras()
        {
            try
            {
                var path = _settingsDAO.ReadGeneralSettings();
                if (string.IsNullOrEmpty(path.SavePathSettings))
                    return new ObservableCollection<CameraStream>();
                var filePath = Path.Combine(path.SavePathSettings, "Cameras.json");
                if (!File.Exists(filePath))
                    return new ObservableCollection<CameraStream>();
                string json = File.ReadAllText(filePath);
                var cameras = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<CameraStream>>(json);
                return cameras ?? new ObservableCollection<CameraStream>();
            }
            catch (Exception ex)
            {
                return new ObservableCollection<CameraStream>();  
            }
        }
        public bool GetFlagRestartCameras()//-----------------
        {
            try
            {
                var path = _settingsDAO.ReadGeneralSettings();
                return path.ShowRestartCamers;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
