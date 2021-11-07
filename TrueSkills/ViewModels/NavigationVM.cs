using DotNetPusher.Pushers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Reactive;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using TrueSkills.APIs;
using TrueSkills.Enums;
using TrueSkills.Exceptions;
using TrueSkills.Interfaces;
using TrueSkills.Views;

namespace TrueSkills.ViewModels
{
    public class NavigationVM : ReactiveObject, IAsyncInitialization
    {
        private UserControl _content;
        private int _heightContent;
        private DispatcherTimer _timer;
        public ReactiveCommand<Unit, Unit> ContentRenderedCommand { get; }
        public Task Initialization { get; set; }
        public int HeightContent
        {
            get => _heightContent;
            set => this.RaiseAndSetIfChanged(ref _heightContent, value);
        }
        public NavigationVM()
        {
            Initialization = InitializationAsync();
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
            Content = new DefaultHeaderUC();
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
            ContentRenderedCommand = ReactiveCommand.Create(ContentRendered);
            SupportingMethods.RtmpScreen(TemporaryVariables.GetStream().Result.Screen);
        }
        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (e.IsAvailable)
            {
                Initialization = InitializationAsync();
            }
        }
        private async Task InitializationAsync()
        {
            if (App.IsNetwork)
            {
                await TemporaryVariables.SubscribeLoadStepAsync();
                await TemporaryVariables.SubscribeGetCountMessagesAsync();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var time = TemporaryVariables.Time;
            if (time != null)
            {
                if (time.Value.TotalSeconds > 0)
                {
                    time = time.Value.Add(TimeSpan.FromSeconds(-1));
                    TemporaryVariables.Time = time;
                    if (Content is VmHeaderUC vmHeader)
                    {
                        vmHeader.TbTime.Text = time.Value.ToString();
                    }
                    if (Content is DefaultHeaderUC defaultHeader)
                    {
                        defaultHeader.IsStartTimer = false;
                        defaultHeader.TbTime.Text = time.Value.ToString();
                    }
                }
            }
        }



        public UserControl Content
        {
            get => _content;
            set
            {
                if (value.GetType().Name == typeof(VmHeaderUC).Name)
                {
                    HeightContent = 135;
                }
                else
                {
                    HeightContent = 80;
                }
                this.RaiseAndSetIfChanged(ref _content, value);

            }
        }

        public void ContentRendered()
        {
            Page page = TemporaryVariables.s_frame.Content as Page;
            TemporaryVariables.Sources.Add(TemporaryVariables.s_frame.Content.ToString());
            if (page is VMPage)
            {
                if (Content.GetType() == typeof(VmHeaderUC))
                {
                    return;
                }
                Content = new VmHeaderUC();
            }
            else
            {
                if (Content.GetType() == typeof(DefaultHeaderUC))
                {
                    return;
                }
                Content = new DefaultHeaderUC();
            }
        }

    }
}
