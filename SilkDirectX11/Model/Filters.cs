using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkDirectX11.Model
{
    public class Filters
    {
        public int Brightness { get; set; } = 0;
        public int Contrast { get; set; } = 0;
        public int Hue { get; set; } = 0;
        public int Saturation { get; set; } = 0;
        public int NoiseReduction { get; set; } = 0;
        public int EdgeEnhancement { get; set; } = 0;
        public int AnamorphicScaling { get; set; } = 0;
        public int StereoAdjustment { get; set; } = 0;
        public string Rotation { get; set; } = "Defoult";
        public Guid CameraId { get; set; }
    }
     
}
