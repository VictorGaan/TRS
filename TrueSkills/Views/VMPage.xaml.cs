using System;
using System.Windows.Controls;
using TrueSkills.ViewModels;
using CefSharp;
using System.Windows;

namespace TrueSkills.Views
{
    /// <summary>
    /// Логика взаимодействия для VMPage.xaml
    /// </summary>
    public partial class VMPage : Page
    {

        public VMPage()
        {
            InitializeComponent();
            TemporaryVariables.webView = webView;
            DataContext = new VirtualMachineVM();
        }
    }
}
