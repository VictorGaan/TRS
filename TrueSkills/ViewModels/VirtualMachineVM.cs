using Microsoft.Win32;
using ReactiveUI;
using System.Net.NetworkInformation;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Threading;
using TrueSkills.APIs;
using TrueSkills.Exceptions;
using TrueSkills.Interfaces;
using TrueSkills.Views;
namespace TrueSkills.ViewModels
{
    public class VirtualMachineVM : ReactiveObject, IAsyncInitialization
    {
        private string _addressVm;
        public string AddressVm
        {
            get => _addressVm;
            set => this.RaiseAndSetIfChanged(ref _addressVm, value);
        }

        public Task Initialization { get; set; }

        public VirtualMachineVM()
        {
            Initialization = GetVM();
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
        }

        void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (!e.IsAvailable)
            {


                App.Current.Dispatcher.Invoke(() =>
                {
                    ShowReconnectionWindow();
                });

            }
        }

        private void ShowReconnectionWindow()
        {
            ReconnectingWindow reconnectingWindow = new ReconnectingWindow();
            reconnectingWindow.ShowDialog();
            GetVM();
        }
        public async Task GetVM()
        {
            AddressVm = null;
            await SetSize();
            await GetAddressAsync();
        }

        private async Task SetSize()
        {
            var content = TemporaryVariables.s_frame;
            if (content != null)
            {
                SizeAPI body = new SizeAPI();
                body.Height = content.ActualHeight-55;
                body.Width = content.ActualWidth;
                try
                {
                    await SupportingMethods.PostWebRequest(Url.s_sizeUrl, body, true);
                }
                catch (CodeException ex)
                {
                    TemporaryVariables.ShowException(ex);
                }
            }

        }

        private async Task GetAddressAsync()
        {
            try
            {
                var response = await SupportingMethods.GetWebRequest<VmAPI>(Url.s_vmUrl, true);
                AddressVm = response.Url;
            }
            catch (CodeException ex)
            {
                TemporaryVariables.ShowException(ex);
            }
        }
    }
}
