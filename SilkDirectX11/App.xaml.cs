using MahApps.Metro.Controls.Dialogs;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using SilkDirectX11.SignalR;
using SilkDirectX11.Views;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Windows;

namespace SilkDirectX11
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // containerRegistry.RegisterSingleton<TestWindCamera>();    
            containerRegistry.Register<MainView>();
            containerRegistry.Register<IDialogCoordinator, DialogCoordinator>();
        }
        protected override Window CreateShell()
        {
            var w = Container.Resolve<MainView>();

            return w;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Start embedded SignalR server
            SignalRHost.StartIfNeeded();
        }
    }

}
