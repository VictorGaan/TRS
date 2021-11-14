using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TrueSkills.APIs;
using TrueSkills.Enums;

namespace TrueSkills.Views
{
    /// <summary>
    /// Логика взаимодействия для DefaultHeaderUC.xaml
    /// </summary>
    public partial class DefaultHeaderUC : UserControl
    {
        public Visibility VisibilityStudent { get; set; }
        public Visibility VisibilityDate { get; set; }
        public Visibility VisibilityLogout { get; set; }
        private DispatcherTimer _timer;
        public DefaultHeaderUC()
        {
            InitializeComponent();
            VisibilityDate = Visibility.Collapsed;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            DataContext = this;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
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
                    VisibilityDate = Visibility.Visible;
                }
                else
                {
                    VisibilityDate = Visibility.Collapsed;
                }
            }
        }

        private void Logout_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TemporaryVariables.Exit();
        }

        private void ImgStudent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TemporaryVariables.currentParticipent != null)
            {
                new ChatWindow(Room.Expert).ShowDialog();
            }
            else
            {
                new MessageBoxWindow(TemporaryVariables.GetProperty("a_Auth"), TemporaryVariables.GetProperty("a_Error"), MessageBoxWindow.MessageBoxButton.Ok);
            }
        }

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
    }
}
