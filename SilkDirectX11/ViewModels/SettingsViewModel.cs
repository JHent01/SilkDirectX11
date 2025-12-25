using SilkDirectX11.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using SilkDirectX11.Model;
using Prism.Events;
using SilkDirectX11.Events;


namespace SilkDirectX11.ViewModels
{
    internal class SettingsViewModel : BindableBase
    {
       public SettingsViewModel(ISettingsDAO settingsDAO, IEventAggregator eventAggregator)
        {
            _settingsDAO = settingsDAO;
            SaveSettingsCommand = new DelegateCommand(SaveSettingsExecute);
            _eventAggregator = eventAggregator;
            
            CanselCommand = new DelegateCommand(CanselExecute);
            SelectForderForCamers = new DelegateCommand(SelectForderForCamersExecute);
        }
        IEventAggregator _eventAggregator;
        ISettingsDAO _settingsDAO;
        private string _savePathSettings  ;
        public string SavePathSettings
        {
            get { return _savePathSettings; }
            set { SetProperty(ref _savePathSettings, value); }
        }

        public DelegateCommand SelectForderForCamers { get; private set; }
        private void SelectForderForCamersExecute()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "Select Folder";
            dialog.Filter = "Folder|*.thisfolder";
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                string folderPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                SavePathSettings = folderPath;
            }
        }

        public DelegateCommand SaveSettingsCommand { get; private set; }
        private void SaveSettingsExecute()
        {
             

            _settingsDAO.SaveGeneralSettings(new Model.PathSettingsJson { SavePathSettings = SavePathSettings });
            _eventAggregator.GetEvent<CloseViewSettingsEvent>().Publish("Close");

        }
       // public DelegateCommand LoadedCommand { get;   set; }
        public string OnLoadedExecute()
        {
            var settings = _settingsDAO.ReadGeneralSettings();
            SavePathSettings = settings.SavePathSettings;
            return SavePathSettings;
        }

        public DelegateCommand CanselCommand { get; private set; }
        private void CanselExecute()
        {
            _eventAggregator.GetEvent<CloseViewSettingsEvent>().Publish("Close");

        }
    }
}
