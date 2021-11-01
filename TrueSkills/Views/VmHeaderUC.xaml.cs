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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TrueSkills.Enums;

namespace TrueSkills.Views
{
    /// <summary>
    /// Логика взаимодействия для VmHeaderUC.xaml
    /// </summary>
    public partial class VmHeaderUC : UserControl
    {
        public VmHeaderUC()
        {
            InitializeComponent();
        }

        private void ImgAgent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TemporaryVariables.s_currentParticipent != null)
            {
                new ChatWindow(Room.Support).ShowDialog();
            }
            else
            {
                MessageBox.Show("Требуется авторизация!", "Title", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImgStudent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TemporaryVariables.s_currentParticipent != null)
            {
                new ChatWindow(Room.Expert).ShowDialog();
            }
            else
            {
                MessageBox.Show(TemporaryVariables.GetProperty("a_Auth"), TemporaryVariables.GetProperty("a_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var lask = new { status = 1 };
            try
            {
                await SupportingMethods.PostWebRequest(Url.s_laskUrl, lask, true);
                new MoveAwayWindow().ShowDialog();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, TemporaryVariables.GetProperty("a_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
