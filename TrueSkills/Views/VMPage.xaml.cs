﻿using Microsoft.Web.WebView2.Wpf;
using System.Windows.Controls;
using TrueSkills.ViewModels;

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
            DataContext = new VirtualMachineVM();
        }

        private void WebView2_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                ((WebView2)sender).ExecuteScriptAsync("document.querySelector('body').style.overflow='hidden'");
            }
        }
    }
}
