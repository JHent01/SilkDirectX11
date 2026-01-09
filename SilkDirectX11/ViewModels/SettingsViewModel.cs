using LibraryForSignalR;
using Microsoft.AspNetCore.SignalR.Client;
using SilkDirectX11.Events;
using SilkDirectX11.Interfaces;
using SilkDirectX11.Servise;
using SilkDirectX11.SignalR;
using SilkDirectX11.Views;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkDirectX11.ViewModels
{
    internal class SettingsViewModel : BindableBase
    {
         
        public SettingsViewModel(ISettingsDAO settingsDAO, IEventAggregator eventAggregator)
        {
            _settingsDAO = settingsDAO;
            SaveSettingsCommand = new DelegateCommand(SaveSettingsExecute);
            _eventAggregator = eventAggregator;
            LoadedCommand= new DelegateCommand(OnLoadedExecute);
            CanselCommand = new DelegateCommand(CanselExecute);
            SelectForderForCamers = new DelegateCommand(SelectForderForCamersExecute);
             
        }
        IEventAggregator _eventAggregator;
        ISettingsDAO _settingsDAO;
        private string _savePathSettings;
        public string SavePathSettings
        {
            get { return _savePathSettings; }
            set { SetProperty(ref _savePathSettings, value); }
        }
        private bool _showRestartCamers;
        public bool ShowRestartCamers
        {
            get { return _showRestartCamers; }
            set { SetProperty(ref _showRestartCamers, value); }
        }
        private bool _cengeZoomMode;
        public bool CengeZoomMode
        {
            get { return _cengeZoomMode; }
            set { SetProperty(ref _cengeZoomMode, value, OnZoomInNewWindowExecute);  }
        }

        private void OnZoomInNewWindowExecute()
        {
            _eventAggregator.GetEvent<CengeZoomModeEvent>().Publish(CengeZoomMode);

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


            _settingsDAO.SaveGeneralSettings(new Model.PathSettingsJson { SavePathSettings = SavePathSettings , ShowRestartCamers = ShowRestartCamers });

           ConnectedManager.SendRestartCamersMessageAsync(new ShowRestartCamersMessage(ShowRestartCamers));
            EventAggregatorProvider.Instance.Publish<bool>(ShowRestartCamers);
             _eventAggregator.GetEvent<CloseSettingsViewEvent>().Publish("Close");
            
        }
         public DelegateCommand LoadedCommand { get;   set; }
        public void OnLoadedExecute()
        {
            var settings = _settingsDAO.ReadGeneralSettings();
            SavePathSettings = settings.SavePathSettings;
            ShowRestartCamers = settings.ShowRestartCamers;
            //return SavePathSettings;
        }

        public DelegateCommand CanselCommand { get; private set; }
        private void CanselExecute()
        {
            
             _eventAggregator.GetEvent<CloseSettingsViewEvent>().Publish("Close");
           
        }

      
        //private async void SendRestartCamersMessageAsync(ShowRestartCamersMessage rest)
        //{
        //    if (_connection != null && _connection.State == HubConnectionState.Connected)
        //    {
        //        try
        //        {
        //            await _connection.InvokeAsync("ShowRestartingCamers", rest);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error sending message: {ex.Message}");
        //        }
        //    }
        //}

    }
}
