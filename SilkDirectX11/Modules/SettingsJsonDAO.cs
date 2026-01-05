using SilkDirectX11.Interfaces;
using SilkDirectX11.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SilkDirectX11.Modules
{
    internal class SettingsJsonDAO : ISettingsDAO
    {
       public void SaveGeneralSettings(PathSettingsJson pathSaveSettingscs)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(pathSaveSettingscs.SavePathSettings);
            string filePath =Path.Combine(Directory.GetCurrentDirectory(),  "settings.json");
            System.IO.File.WriteAllText(filePath, json);
            string flagRestartsCamers = System.Text.Json.JsonSerializer.Serialize(pathSaveSettingscs.ShowRestartCamers);
            string path = Path.Combine(Directory.GetCurrentDirectory(), "flag.json");
            System.IO.File.WriteAllText(path, flagRestartsCamers);

        }
        public PathSettingsJson ReadGeneralSettings()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "settings.json");
            string  path = Path.Combine(Directory.GetCurrentDirectory(), "flag.json");
            if (System.IO.File.Exists(filePath)&& System.IO.File.Exists(path))
            {
                string json = System.IO.File.ReadAllText(filePath);
                bool flag = System.Text.Json.JsonSerializer.Deserialize<bool>(System.IO.File.ReadAllText(path));
                PathSettingsJson settings = new PathSettingsJson();
                   settings.SavePathSettings = System.Text.Json.JsonSerializer.Deserialize<string>(json);
                settings.ShowRestartCamers = flag;
                return settings;
            }
            else
            {

                string json = System.Text.Json.JsonSerializer.Serialize(Directory.GetCurrentDirectory());
                System.IO.File.WriteAllText(filePath, json);
                return new PathSettingsJson { SavePathSettings = json , ShowRestartCamers = false};
            }


            
        }

    }
}
