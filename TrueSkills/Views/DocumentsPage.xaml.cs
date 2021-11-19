using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
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
            DispatcherTimer timer = new DispatcherTimer();
            var ghz = TemporaryVariables.RefreshFrequency();
            timer.Interval = TimeSpan.FromSeconds(1 / ghz);
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if ((DataContext as DocumentsVM).DocumentModel.Pdfs.Any())
            {
                if (LinesGrid.Visibility == Visibility.Collapsed)
                {
                    LinesGrid.Visibility = Visibility.Visible;
                    PdfViewer.Visibility = Visibility.Collapsed;
                }
                else
                {
                    LinesGrid.Visibility = Visibility.Collapsed;
                    PdfViewer.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
