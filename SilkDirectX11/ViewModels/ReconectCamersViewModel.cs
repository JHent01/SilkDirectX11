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
            _eventAggregator.GetEvent<MessegeToReconectCameraViewEvent>().Subscribe(SetStatusCamera);
            _eventAggregator.GetEvent<ReconnectEvent>().Subscribe(SetBrogressBar);
            _eventAggregator.GetEvent<ClouseCameraModuleEvent>().Subscribe(OnClouseCamera);
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
           
        }
        public void SetBrogressBar(bool status)
        {
             
            BrogressBar = status;
            ReconectStatus = "Camera Reconnected";
            Thread.Sleep(1000);

            _eventAggregator.GetEvent<CloseReconectCamersViewEvent>().Publish("Close");
        }
        int counter = 4;
        public   async  void OnClouseCamera(string stat)
        { 
           var ts =  Task.Factory.StartNew(() => Clouse(stat));
           
            await ts;
            _eventAggregator.GetEvent<CloseReconectCamersViewEvent>().Publish("Close");
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
             
        }
        
        

    }
}
