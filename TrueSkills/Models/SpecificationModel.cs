using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using TrueSkills.APIs;
using TrueSkills.Enums;
using TrueSkills.Exceptions;
using TrueSkills.Interfaces;

namespace TrueSkills.Models
{
    public class SpecificationModel : ReactiveObject, IAsyncInitialization
    {
        #region Requests
        private const string PROCESSOR_QUERY = "SELECT * FROM Win32_Processor";
        private const string NETWORK_ADAPTER_QUERY = "SELECT * FROM Win32_NetworkAdapter";
        private const string COMPUTER_SYSTEM_QUERY = "SELECT * FROM Win32_ComputerSystem";
        #endregion
        private bool _isEnabled;
        private bool _isFitMonitors;
        private Visibility _visibilityGrid;
        private SpecificationPcAPI.Rootobject _specificationPc;
        private ServerSpecificationPcAPI.Rootobject _serverSpecificationPCDto;

        public SpecificationModel()
        {
            _visibilityGrid = Visibility.Collapsed;
            _specificationPc = new SpecificationPcAPI.Rootobject();
            _serverSpecificationPCDto = new ServerSpecificationPcAPI.Rootobject();
            Initialization = InitializationAsync();
            Task.Run(() => SubscribeGetMonitorsAsync());
        }
        public bool IsFitMonitors
        {
            get => _isFitMonitors;
            set => this.RaiseAndSetIfChanged(ref _isFitMonitors, value);
        }
        public bool IsEnabled
        {
            get => _isEnabled;
            set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
        }
        public Visibility VisibilityGrid
        {
            get => _visibilityGrid;
            set => this.RaiseAndSetIfChanged(ref _visibilityGrid, value);
        }

        public ServerSpecificationPcAPI.Rootobject ServerSpecificationPc
        {
            get => _serverSpecificationPCDto;
            set => this.RaiseAndSetIfChanged(ref _serverSpecificationPCDto, value);
        }

        public SpecificationPcAPI.Rootobject SpecificationPc
        {
            get => _specificationPc;
            set => this.RaiseAndSetIfChanged(ref _specificationPc, value);
        }
        public Task Initialization { get; set; }

        private async Task InitializationAsync()
        {
            await GetParamsAsync();
            await SendSpecificationAsync();
            await Task.Run(() =>
                {
                    IsFitMonitors = ServerSpecificationPc.Status.Monitors.Any(x => x.Measurement != StatusSpecification.NotFit);
                    IsEnabled = IsValidSpecification();
                    VisibilityGrid = Visibility.Visible;
                });
        }

        private async Task SendSpecificationAsync()
        {
            if (App.IsNetwork)
            {
                try
                {
                    var response = await SupportingMethods.PostWebRequest<SpecificationPcAPI.Rootobject, ServerSpecificationPcAPI.Rootobject>(Url.s_specificationUrl, SpecificationPc, true);
                    ServerSpecificationPc = response;
                }
                catch (CodeException ex) { TemporaryVariables.ShowException(ex); }
            }
        }
        private bool IsValidSpecification()
        {
            var specification = ServerSpecificationPc.Status;
            return specification.CPU != StatusSpecification.NotFit
                && specification.FreeSpace != StatusSpecification.NotFit
                && specification.Internet != StatusSpecification.NotFit
                && specification.RAM != StatusSpecification.NotFit
                && IsFitMonitors;
        }

        private async Task GetParamsAsync()
        {
            await GetCPUAsync();
            await GetFreeSpaceAsync();
            await GetInternetSpeedAsync();
            await GetMonitorsAsync();
            await GetRAMAsync();
        }

        #region Get PC Params
        private async Task GetCPUAsync()
        {
            await Task.Run(() =>
            {
                ManagementObjectSearcher search = new ManagementObjectSearcher(PROCESSOR_QUERY);
                foreach (ManagementObject obj in search.Get())
                {
                    if (obj.Properties["Name"].Value != null)
                    {
                        SpecificationPc.CPU.Name = obj["Name"].ToString().Trim();
                    }
                }
            });
        }

        private async Task GetFreeSpaceAsync()
        {
            await Task.Run(() =>
            {
                DriveInfo[] allDrives = DriveInfo.GetDrives();
                foreach (DriveInfo d in allDrives)
                {
                    if (d.IsReady)
                    {
                        SpecificationPc.FreeSpace.Value += d.TotalFreeSpace / (1024 * 1024 * 1024);
                        SpecificationPc.FreeSpace.Measurement = "Gb";
                    }
                }
            });
        }

        private async Task GetInternetSpeedAsync()
        {
            await Task.Run(() =>
            {
                ManagementObjectSearcher search = new ManagementObjectSearcher(NETWORK_ADAPTER_QUERY);
                foreach (ManagementObject obj in search.Get())
                {
                    if (obj.Properties["Speed"].Value != null)
                    {
                        SpecificationPc.Internet.Value = long.Parse(obj.Properties["Speed"].Value.ToString()) / 1000000;
                        SpecificationPc.Internet.Measurement = "Mbit/s";
                    }
                }
            });
        }
        private int GetCountMonitors() => System.Windows.Forms.Screen.AllScreens.Count();
        private bool _isNewMonitor;

        private async Task SubscribeGetMonitorsAsync()
        {
            if (SpecificationPc.Monitors.Count != 0)
            {
                if (GetCountMonitors() != SpecificationPc.Monitors.Count)
                {
                    _isNewMonitor = true;
                }
            }
            if (_isNewMonitor)
            {
                await Task.Run(() => InitializationAsync());
            }
            await Task.Delay(5000);
            await GetMonitorsAsync();
        }

        private async Task GetMonitorsAsync()
        {
            await Task.Run(() =>
            {
                SpecificationPc.Monitors.Clear();
                int value = 1;
                foreach (var screen in System.Windows.Forms.Screen.AllScreens)
                {
                    SpecificationPc.Monitors.Add(new SpecificationPcAPI.Monitor() { Value = value, Measurement = screen.Bounds.Width + "*" + screen.Bounds.Height });
                    value += 1;
                }
            });
        }

        private async Task GetRAMAsync()
        {
            await Task.Run(() =>
            {
                ManagementObjectSearcher search = new ManagementObjectSearcher(COMPUTER_SYSTEM_QUERY);
                string ram = string.Empty;
                foreach (ManagementObject obj in search.Get())
                {
                    if (obj.Properties["TotalPhysicalMemory"].Value != null)
                        SpecificationPc.RAM.Value = Convert.ToInt32(Math.Round(Convert.ToDouble(obj["TotalPhysicalMemory"]) / (1024 * 1024 * 1024)));
                    SpecificationPc.RAM.Measurement = "Gb";
                }
            });
        }
        #endregion
    }
}
