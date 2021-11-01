using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
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
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_isAvailable)
            {
                Close();
            }
        }
    }
}
