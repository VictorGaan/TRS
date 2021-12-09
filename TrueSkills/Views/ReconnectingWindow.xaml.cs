using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TrueSkills.Views
{
    /// <summary>
    /// Логика взаимодействия для ReconnectingWindow.xaml
    /// </summary>
    public partial class ReconnectingWindow : Window
    {
        public ReconnectingWindow()
        {
            InitializeComponent();
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
        }

        private bool _isAvailable;
        void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            _isAvailable = e.IsAvailable;
            if (_isAvailable && App.IsNetworkServer)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Thread.Sleep(5000);
                    Close();
                });
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Ring.Visibility = Visibility.Visible;
            if (_isAvailable && App.IsNetworkServer)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Thread.Sleep(5000);
                    Close();
                });
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
