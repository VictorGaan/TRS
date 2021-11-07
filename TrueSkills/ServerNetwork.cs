using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrueSkills.APIs;
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
            var response = await SupportingMethods.GetWebRequest<NowAPI>(Url.s_nowUrl, true);
            if (response is NowAPI)
            {
                ServerNetworkAvailabilityChanged?.Invoke(true);
            }
            else
            {
                ServerNetworkAvailabilityChanged?.Invoke(false);
            }
            await Task.Delay(5000);
            await SubscribeCheckServerAsync();
        }
    }
}
