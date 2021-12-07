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
            //DispatcherTimer timer = new DispatcherTimer();
            //var ghz = TemporaryVariables.RefreshFrequency();
            //timer.Interval = TimeSpan.FromSeconds(1 / ghz);
            //timer.Tick += Timer_Tick;
            //timer.Start();
        }

        //private void Timer_Tick(object sender, EventArgs e)
        //{
        //    if ((DataContext as DocumentsVM).DocumentModel.Pdfs.Any())
        //    {
        //        if (System.Windows.Controls.Panel.GetZIndex(LinesGrid) == 2)
        //        {
        //            System.Windows.Controls.Panel.SetZIndex(LinesGrid, 1);
        //            System.Windows.Controls.Panel.SetZIndex(PdfViewer, 2);
        //        }
        //        else
        //        {
        //            System.Windows.Controls.Panel.SetZIndex(LinesGrid, 2);
        //            System.Windows.Controls.Panel.SetZIndex(PdfViewer, 1);
        //        }
        //    }
        //}
    }
}
