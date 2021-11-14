using System;
using System.Windows.Controls;
using TrueSkills.ViewModels;
using CefSharp;

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
            webView.ExecuteScriptAsyncWhenPageLoaded("document.querySelector('body').style.overflow='hidden'");
        }
    }
}
