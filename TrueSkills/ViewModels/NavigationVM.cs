using DotNetPusher.Pushers;
using Notifications.Wpf;
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
using System.Windows.Forms;
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
        private Visibility _datePickerVisibility;
        private System.Windows.Controls.UserControl _content;
        private double _heightContent;
        private DispatcherTimer _timer;
        public ReactiveCommand<Unit, Unit> ContentRenderedCommand { get; }
        public Task Initialization { get; set; }

        public Visibility DatePickerVisibility
        {
            get => _datePickerVisibility;
            set => this.RaiseAndSetIfChanged(ref _datePickerVisibility, value);
        }
        public double HeightContent
        {
            get => _heightContent;
            set => this.RaiseAndSetIfChanged(ref _heightContent, value);
        }
        public Rtmp rtmp = new Rtmp();
        public NavigationVM()
        {
            HeightContent = double.NaN;
            DatePickerVisibility = Visibility.Visible;
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
            try
            {
                rtmp.RtmpScreen(TemporaryVariables.GetStream().Result.Screen);
            }
            catch (Exception ex)
            {
                new MessageBoxWindow(ex.Message, TemporaryVariables.GetProperty("a_Error"), MessageBoxWindow.MessageBoxButton.Ok);
            }
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
            var time = TemporaryVariables.time;
            if (time != null)
            {
                if (time.Value.TotalSeconds > 0)
                {
                    DatePickerVisibility = Visibility.Visible;
                    time = time.Value.Add(TimeSpan.FromSeconds(-1));
                    TemporaryVariables.time = time;
                    if (Content is DefaultHeaderUC defaultHeader)
                    {
                        defaultHeader.DateGrid.Visibility = DatePickerVisibility;
                        defaultHeader.TbTime.Text = time.Value.ToString();
                    }
                }
                else
                {
                    DatePickerVisibility = Visibility.Collapsed;
                }
            }
            else
            {
                DatePickerVisibility = Visibility.Collapsed;
            }
        }

        public System.Windows.Controls.UserControl Content
        {
            get => _content;
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        public void ContentRendered()
        {
            Page page = TemporaryVariables.frame.Content as Page;
            TemporaryVariables.sources.Add(TemporaryVariables.frame.Content.ToString());
            if (page is VMPage)
            {
                HeightContent = double.NaN;
                Content = null;
                new VmHeaderUC().Show();
                return;
            }
            if (Content.GetType() == typeof(DefaultHeaderUC))
            {
                return;
            }
            Content = new DefaultHeaderUC();
        }
    }
}
