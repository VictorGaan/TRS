using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Threading;
using TrueSkills.ViewModels;

namespace TrueSkills.Views
{
    /// <summary>
    /// Логика взаимодействия для TaskPage.xaml
    /// </summary>
    public partial class TaskPage : Page
    {
        public TaskPage()
        {
            InitializeComponent();
            DataContext = new TaskVM();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var index = (int)((ListBoxItem)myListBox.ContainerFromElement((Button)sender)).Content;
            var context = (DataContext as TaskVM).TaskModel;
            index -= 1;
            context.NextDocument(index);
        }
    }
}
