using System;
using System.Collections.Generic;
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
using TrueSkills.Enums;

namespace TrueSkills.Views
{
    /// <summary>
    /// Логика взаимодействия для VmHeaderUC.xaml
    /// </summary>
    public partial class VmHeaderUC : Window
    {
        private DispatcherTimer _timer;
        public VmHeaderUC()
        {
            InitializeComponent();
            DateGrid.Visibility = Visibility.Collapsed;
             _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
            DataContext = this;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!Topmost)
            {
                Topmost = true;
            }
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
                    if (TemporaryVariables.time.Value.Hours <= 0)
                    {
                        TbHours.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        TbHours.Visibility = Visibility.Visible;
                    }
                    if (TemporaryVariables.time.Value.Minutes <= 0)
                    {
                        TbMinutes.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        TbMinutes.Visibility = Visibility.Visible;
                    }
                    if (TemporaryVariables.time.Value.Seconds <= 0)
                    {
                        TbSeconds.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        TbSeconds.Visibility = Visibility.Visible;
                    }
                    DateGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    DateGrid.Visibility = Visibility.Collapsed;
                }
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var lask = new { status = 1 };
            try
            {
                await SupportingMethods.PostWebRequest(Url.s_laskUrl, lask, true);
                new MoveAwayWindow().ShowDialog();
            }
            catch (Exception ex)
            {
                new MessageBoxWindow(ex.Message, TemporaryVariables.GetProperty("a_Error"), MessageBoxWindow.MessageBoxButton.Ok);
            }
        }

        private void BtnEnd_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxWindow message = new MessageBoxWindow(TemporaryVariables.GetProperty("vm_SubmitEnd"), TemporaryVariables.GetProperty("a_Question"), MessageBoxWindow.MessageBoxButton.YesNo);
            if (message.IsYes)
            {
                ExamEndWindow examEndWindow = new ExamEndWindow();
                examEndWindow.Show();
                TemporaryVariables.CloseAllWindows();
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
