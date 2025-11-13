using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
        private string cameraMainStream;
        public string CameraMainStream
        {
            get { return cameraMainStream; }
            set { cameraMainStream = value; }
        }
        private string cameraSubStream;
        public string CameraSubStream
        {
            get { return cameraSubStream; }
            set { cameraSubStream = value; }
        }

        private Guid cameraID;
        public Guid CameraID
        {
            get { return cameraID; }
            set { cameraID = value; }
        }

      //  public bool IsSelected { get; set; } = false;
    }
}
