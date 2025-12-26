using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilkDirectX11.Interfaces;
using SilkDirectX11.Model;

namespace SilkDirectX11.Modules
{
    class CameraSettingsDAO : ICameraSettingsDAO
    {
        ISettingsDAO _settingsDAO;
        public CameraSettingsDAO(ISettingsDAO settingsDAO) 
        {
            _settingsDAO = settingsDAO;
        }

        public void SaveCameraSettings(List<Filters> settings)
        {
            try
            {
                var path = _settingsDAO.ReadGeneralSettings();
                if (string.IsNullOrEmpty(path.SavePathSettings))
                    return;
                string json = System.Text.Json.JsonSerializer.Serialize(settings);
                System.IO.File.WriteAllText(System.IO.Path.Combine(path.SavePathSettings, "CameraSettings.json"), json);
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving camera settings", ex);
            }
        }
        public List<Filters> GetCameraSettings()
        {
            try
            {
                var path = _settingsDAO.ReadGeneralSettings();
                if (string.IsNullOrEmpty(path.SavePathSettings))
                    return new List<Filters>();
                var filePath = System.IO.Path.Combine(path.SavePathSettings, "CameraSettings.json");
                if (!System.IO.File.Exists(filePath))
                    return new List<Filters>();
                string json = System.IO.File.ReadAllText(filePath);
                 List<Filters> settings = System.Text.Json.JsonSerializer.Deserialize<List<Filters>>(json);
                return settings;
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading camera settings", ex);
            }
        }
    }
}
