using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using TrueSkills.ViewModels;

namespace TrueSkills.Views
{
    /// <summary>
    /// Логика взаимодействия для VMPage.xaml
    /// </summary>
    public partial class VMPage : System.Windows.Controls.Page
    {
        public VMPage()
        {
            InitializeComponent();
            DataContext = new VirtualMachineVM();
        }
    }
}
