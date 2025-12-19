using LibraryForSignalR;
using Prism.Events;
using SilkDirectX11.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkDirectX11.ViewModels
{
    public class ReconectCamersViewModel : BindableBase
    {

        public ReconectCamersViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<MessegeToReconectCameraEvent>().Subscribe(SetStatusCamera);
            _eventAggregator.GetEvent<ProgressBarForReconnectEvent>().Subscribe(SetBrogressBar);
            _eventAggregator.GetEvent<ClouseCameraModuleEvent>().Subscribe(ClouseCamera);
        }
        IEventAggregator _eventAggregator;
     
        private string statusCamera;
        public string ReconectStatus
        {
            get => statusCamera;
            set => SetProperty(ref statusCamera, value);
        }
        private bool brogressBar = true;
        public bool BrogressBar
        {
            get => brogressBar;
            set => SetProperty(ref brogressBar, value);
        }

        public void SetStatusCamera(string status)
        {
            ReconectStatus = status;
            RaisePropertyChanged(ReconectStatus);
        }
        public void SetBrogressBar(bool status)
        {
            // тут надо поправить что бы не заходило сюда и не открывало это окно 
            BrogressBar = status;
            ReconectStatus = "Camera Reconnected";
            Thread.Sleep(1000);

            _eventAggregator.GetEvent<CloseReconectCamersEvent>().Publish("Close");
        }
        int counter = 4;
        public   async  void ClouseCamera(string stat)
        { 
            Task.Factory.StartNew(() => Clouse(stat));
           
        }

        private  async Task Clouse(string s)
        {
            SetStatusCamera(s);
            Thread.Sleep(5000);
            while (counter > 0)
            {
                counter--;   
                SetStatusCamera($"Camera Clouse after {counter}");
                Thread.Sleep(1000);
              
            }

            _eventAggregator.GetEvent<CloseReconectCamersEvent>().Publish("Close");

        }
        
        

    }
}
