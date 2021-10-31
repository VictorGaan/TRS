using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Linq;
using System.IO;
using System.Windows.Threading;
using TrueSkills.APIs;
using System.Threading.Tasks;
using TrueSkills.Interfaces;
using System.Diagnostics;
using System.Threading;

namespace TrueSkills
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            //var args = Environment.GetCommandLineArgs();
            if (TemporaryVariables.IsMoreMice())
            {
                MessageBox.Show(TemporaryVariables.GetProperty("a_Mice"),TemporaryVariables.GetProperty("a_Error"),MessageBoxButton.OK,MessageBoxImage.Error);
                Current.Shutdown();
            }
            string[] args = new string[] { "1", @"ru-RU", @"C:\Users\gaste\source\repos\TrueSkills.Launcher\TrueSkills.Launcher\bin\Debug\netcoreapp3.1" };
            if (args.Length > 1)
            {
                TemporaryVariables.PathXaml = args[2];
                TemporaryVariables.EnglishName = args[1];
            }
            Exit += App_Exit;
        }
        

        private void App_Exit(object sender, ExitEventArgs e)
        {
            TemporaryVariables.Exit();
        }
    }
}
