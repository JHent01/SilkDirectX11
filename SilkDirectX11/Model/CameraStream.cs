using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkDirectX11.Model
{
    public class CameraStream
    {
        CameraConnectStrings cameraConnectStrings{ get; set; }
        public CameraConnectStrings ConnectStrings
        {
            get { return cameraConnectStrings; }
            set { cameraConnectStrings = value; }
        }
        string cameraName { get; set; }
        public string CameraName
        {
            get { return cameraName; }
            set { cameraName = value; }
        }


    }
}
