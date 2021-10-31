using Notifications.Wpf.Core;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using TrueSkills.Interfaces;
using TrueSkills.ViewModels;

namespace TrueSkills.Views
{
    /// <summary>
    /// Логика взаимодействия для NavigationWindow.xaml
    /// </summary>
    public partial class NavigationWindow : Window
    {
        public NavigationWindow()
        {
            InitializeComponent();
            TemporaryVariables.s_frame = MainFrame;
            DataContext = new NavigationVM(this);
            TemporaryVariables.StartKiosk(new WindowInteropHelper(this).Handle);

        }
    }
}
