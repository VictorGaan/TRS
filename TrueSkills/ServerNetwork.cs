using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TrueSkills.APIs;
using TrueSkills.Exceptions;
using TrueSkills.Interfaces;

namespace TrueSkills
{
    public class ServerNetwork : IAsyncInitialization
    {
        public Task Initialization { get; set; }

        public delegate void ServerNetworkChange(bool isWork);
        public event ServerNetworkChange ServerNetworkAvailabilityChanged;
        public ServerNetwork()
        {
            Initialization = SubscribeCheckServerAsync();
        }
        public async Task SubscribeCheckServerAsync()
        {
            try
            {
                var response = await SupportingMethods.GetWebRequest<NowAPI>(Url.s_nowUrl, false);
                ServerNetworkAvailabilityChanged?.Invoke(true);
            }
            catch (CodeException)
            {
                ServerNetworkAvailabilityChanged?.Invoke(false);
            }
            await Task.Delay(5000);
            await SubscribeCheckServerAsync();
        }
    }
}
