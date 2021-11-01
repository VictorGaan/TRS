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
        public bool IsStartTimer = true;
        public DefaultHeaderUC()
        {
            InitializeComponent();
            if (IsStartTimer)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromSeconds(1);
                _timer.Tick += Timer_Tick;
                _timer.Start();
            }
            
            DataContext = this;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (TemporaryVariables.s_time!=null)
            {
                TbTime.Text = TemporaryVariables.s_time.ToString();
            }
        }

        private void Logout_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TemporaryVariables.Exit();
        }

        private void ImgStudent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TemporaryVariables.s_currentParticipent != null)
            {
                new ChatWindow(Room.Expert).ShowDialog();
            }
            else
            {
                MessageBox.Show(TemporaryVariables.GetProperty("a_Auth"), TemporaryVariables.GetProperty("a_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImgAgent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TemporaryVariables.s_currentParticipent != null)
            {
                new ChatWindow(Room.Support).ShowDialog();
            }
            else
            {
                MessageBox.Show(TemporaryVariables.GetProperty("a_Auth"), TemporaryVariables.GetProperty("a_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
