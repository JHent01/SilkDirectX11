using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkDirectX11.Model
{
    public class CameraVisualSettings
    {
        public int Brightness { get; set; }
        public int Contrast { get; set; }
        public int Hue { get; set; }
        public int Saturation { get; set; }
        public int NoiseReduction { get; set; }
        public int EdgeEnhancement { get; set; }
        public int AnamorphicScaling { get; set; }
        public int StereoAdjustment { get; set; }
        public string Rotation { get; set; } = "Defoult";
    }
     
}
