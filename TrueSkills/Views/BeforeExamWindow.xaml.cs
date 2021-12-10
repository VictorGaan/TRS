using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using TrueSkills.APIs;
using TrueSkills.Enums;
using TrueSkills.Interfaces;

namespace TrueSkills.Views
{
    /// <summary>
    /// Логика взаимодействия для BeforeExamWindow.xaml
    /// </summary>
    public partial class BeforeExamWindow : Window, IAsyncInitialization
    {
        private DispatcherTimer _timer;

        public BeforeExamWindow(StepAPI step)
        {
            InitializeComponent();
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
            TbExam.Text = TemporaryVariables.GetProperty("a_Step" + (int)step.Step);
            Initialization = InitializationAsync();
            DateGrid.Visibility = Visibility.Collapsed;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TbLanguage.Text = GetInputLanguage();
            if (TemporaryVariables.time != null)
            {
                TbTime.Text = TemporaryVariables.time.ToString();
                if (TemporaryVariables.time.Value.Ticks > 0)
                {
                    switch (TemporaryVariables.s_step.Step)
                    {
                        case Step.ExamNotRun:
                        case Step.ExamHasStartedDocumentDisplayed:
                        case Step.ExamHasStartedModuleNotStarted:
                        case Step.ExamStartTaskDisplay:
                            TbModule.Text = TemporaryVariables.GetProperty("a_Module1");
                            break;
                        case Step.ExamStartModuleUnderway:
                            TbModule.Text = TemporaryVariables.GetProperty("a_Module2");
                            break;
                    }

                    if (TemporaryVariables.time.Value.Days <= 0)
                    {
                        TbDays.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        TbDays.Visibility = Visibility.Visible;
                    }
                    if (TemporaryVariables.time.Value.Hours <= 0 && TemporaryVariables.time.Value.Days <= 0)
                    {
                        TbHours.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        TbHours.Visibility = Visibility.Visible;
                    }
                    if (TemporaryVariables.time.Value.Minutes <= 0 && TemporaryVariables.time.Value.Hours <= 0 && TemporaryVariables.time.Value.Days <= 0)
                    {
                        TbMinutes.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        TbMinutes.Visibility = Visibility.Visible;
                    }
                    if (TemporaryVariables.time.Value.Seconds <= 0 && TemporaryVariables.time.Value.Hours <= 0 && TemporaryVariables.time.Value.Days <= 0)
                    {
                        TbSeconds.Visibility = Visibility.Collapsed;
                        TemporaryVariables.time = null;
                    }
                    else
                    {
                        TbSeconds.Visibility = Visibility.Visible;
                    }
                    DateGrid.Visibility = Visibility.Visible;
                    if (TemporaryVariables.time!=null)
                    {
                        TemporaryVariables.time = TemporaryVariables.time.Value.Add(TimeSpan.FromSeconds(-1));
                    }
                }
                else
                {
                    DateGrid.Visibility = Visibility.Collapsed;
                }
            }
        }

        private string GetInputLanguage()
        {
            return InputLanguage.CurrentInputLanguage.Culture.TwoLetterISOLanguageName.ToUpper();
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
            }
        }
        public Task Initialization { get; set; }

        private void ImgAgent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TemporaryVariables.currentParticipent != null)
            {
                new ChatWindow(Room.Support).ShowDialog();
            }
            else
            {
                new MessageBoxWindow(TemporaryVariables.GetProperty("a_Auth"), TemporaryVariables.GetProperty("a_Error"), MessageBoxWindow.MessageBoxButton.Ok);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    DragMove();
            }
            catch
            {
                return;
            }
        }
    }
}
