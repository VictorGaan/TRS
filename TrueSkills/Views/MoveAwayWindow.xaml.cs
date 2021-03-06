using System;
using System.Collections.Generic;
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

namespace TrueSkills.Views
{
    /// <summary>
    /// Логика взаимодействия для MoveAwayWindow.xaml
    /// </summary>
    public partial class MoveAwayWindow : Window
    {
        public MoveAwayWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var lask = new { status = 2 };
            try
            {
                await SupportingMethods.PostWebRequest(Url.s_laskUrl, lask, true);
                Close();
            }
            catch (Exception ex)
            {
                new MessageBoxWindow(ex.Message, TemporaryVariables.GetProperty("a_Error"), MessageBoxWindow.MessageBoxButton.Ok);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    DragMove();
            }
            catch
            {
                return;
            }
        }
    }
}
