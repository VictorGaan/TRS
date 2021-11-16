using System;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Windows;
using TrueSkills.Views;

namespace TrueSkills
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static bool IsNetwork { get; set; }
        public App()
        {
            ServerNetwork serverNetwork = new ServerNetwork();
            serverNetwork.ServerNetworkAvailabilityChanged += ServerNetwork_ServerNetworkAvailabilityChanged;
            IsNetwork = NetworkInterface.GetIsNetworkAvailable();
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
            Exit += App_Exit;
        }

        private void ServerNetwork_ServerNetworkAvailabilityChanged(bool isWork)
        {
            if (!isWork)
            {
                Current.Dispatcher.Invoke(() =>
                {
                    new MessageBoxWindow(TemporaryVariables.GetProperty("a_ServerNetwork"), TemporaryVariables.GetProperty("a_Error"), MessageBoxWindow.MessageBoxButton.Ok);
                });

            }
        }

        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            IsNetwork = e.IsAvailable;
            if (!IsNetwork)
            {
                Current.Dispatcher.Invoke(() =>
                {
                    new MessageBoxWindow(TemporaryVariables.GetProperty("a_ClientNetwork"), TemporaryVariables.GetProperty("a_Error"), MessageBoxWindow.MessageBoxButton.Ok);
                });
            }
        }
        private void App_Exit(object sender, ExitEventArgs e)
        {
            TemporaryVariables.Exit();
        }
    }
}
