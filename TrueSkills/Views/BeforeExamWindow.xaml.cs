using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TrueSkills.APIs;
using TrueSkills.Interfaces;

namespace TrueSkills.Views
{
    /// <summary>
    /// Логика взаимодействия для BeforeExamWindow.xaml
    /// </summary>
    public partial class BeforeExamWindow : Window, IAsyncInitialization
    {
        public BeforeExamWindow(StepAPI step)
        {
            InitializeComponent();
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
            TbExam.Text = TemporaryVariables.GetProperty("a_Step" + (int)step.Step);
            Initialization = InitializationAsync();
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
    }
}
