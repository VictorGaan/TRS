using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TrueSkills.Views
{
    /// <summary>
    /// Логика взаимодействия для BlackWindow.xaml
    /// </summary>
    public partial class BlackWindow : Window
    {
        public BlackWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.None;
            this.Height +=1000;
            this.Width +=1000;
            Topmost = false;
            new Locker();
        }
    }
}
