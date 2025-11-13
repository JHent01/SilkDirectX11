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
       public void SaveSettings(PathSettingsJson pathSaveSettingscs)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(pathSaveSettingscs.SavePathSettings);
            string filePath =Path.Combine(Directory.GetCurrentDirectory(),  "settings.json");
            System.IO.File.WriteAllText(filePath, json);

        }
        public PathSettingsJson ReadSettings()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "settings.json");
            if (System.IO.File.Exists(filePath))
            {
                string json = System.IO.File.ReadAllText(filePath);
                PathSettingsJson settings = new PathSettingsJson();
                   settings.SavePathSettings = System.Text.Json.JsonSerializer.Deserialize<string>(json);
                return settings;
            }
            else
            {

                string json = System.Text.Json.JsonSerializer.Serialize(Directory.GetCurrentDirectory());
                System.IO.File.WriteAllText(filePath, json);
                return new PathSettingsJson { SavePathSettings = json };
            }


            
        }

    }
}
