using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkDirectX11.Model
{
    internal class PathSettingsJson
    {
        private string savePathSettings;
         public string SavePathSettings
        {
            get { return savePathSettings; }
            set { savePathSettings = value; }
        }
        private bool showRestartCamers;
        public bool ShowRestartCamers
        {
            get { return showRestartCamers; }
            set { showRestartCamers = value; }
        }
    }
}
