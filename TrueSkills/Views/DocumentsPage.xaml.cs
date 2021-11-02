using System.Windows.Controls;
using TrueSkills.APIs;
using TrueSkills.ViewModels;

namespace TrueSkills.Views
{
    /// <summary>
    /// Логика взаимодействия для DocumentsPage.xaml
    /// </summary>
    public partial class DocumentsPage : Page
    {
        public DocumentsPage()
        {
            InitializeComponent();
            DataContext = new DocumentsVM();
        }
    }
}
