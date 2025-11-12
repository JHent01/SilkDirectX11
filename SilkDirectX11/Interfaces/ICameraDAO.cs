using SilkDirectX11.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;
namespace SilkDirectX11.Interfaces
{
    internal interface ICameraDAO
    {
        void SaveCamera(ObservableCollection<WindowsFormsHost> camera);
        ObservableCollection<CameraStream> GetAllCameras();

    }
}
