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
            string[] args = File.ReadAllText("Args.txt").Split('&');
            if (args.Length > 1)
            {
                TemporaryVariables.PathXaml = args[1];
                TemporaryVariables.EnglishName = args[0];
            }
            Exit += App_Exit;
        }
        

        private void App_Exit(object sender, ExitEventArgs e)
        {
            TemporaryVariables.Exit();
        }
    }
}
