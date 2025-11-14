using SilkDirectX11.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkDirectX11.Interfaces
{
    interface ICameraSettingsDAO
    {
        void SaveCameraSettings(List<CameraVisualSettings> settings);
        List<CameraVisualSettings> GetCameraSettings();
    }
}
