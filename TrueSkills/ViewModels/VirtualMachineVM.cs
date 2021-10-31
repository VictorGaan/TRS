using CefSharp.Wpf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;
using TrueSkills.APIs;
using TrueSkills.Exceptions;
using TrueSkills.Interfaces;
using TrueSkills.Models;
using TrueSkills.Views;
namespace TrueSkills.ViewModels
{
    public class VirtualMachineVM : ReactiveObject, IAsyncInitialization
    {
        private string _addressVm;
        public ReactiveCommand<Unit, Unit> LoadErrorCommand { get; }
        public string AddressVm
        {
            get => _addressVm;
            set => this.RaiseAndSetIfChanged(ref _addressVm, value);
        }

        public Task Initialization { get; set; }

        public VirtualMachineVM()
        {
            Initialization = GetVM();
            LoadErrorCommand = ReactiveCommand.CreateFromTask(LoadError);
        }

        public async Task GetVM()
        {
            AddressVm = null;
            await SetSize();
            await GetAddressAsync();
        }

        private async Task LoadError()
        {
            new ReconnectingWindow().ShowDialog();
            await GetVM();
        }

        private async Task SetSize()
        {
            var content = TemporaryVariables.s_frame;
            if (content != null)
            {
                SizeAPI body = new SizeAPI();
                body.Height = content.ActualHeight;
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
