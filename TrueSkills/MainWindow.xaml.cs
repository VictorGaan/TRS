using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TrueSkills.ViewModels;

namespace TrueSkills
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowVM();
            Locker.Lock();
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

        private void FIO_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                List<char> text = FIO.Text.ToCharArray().ToList();
                var currentPos = FIO.SelectionStart;
                var pos = currentPos > 0 ? currentPos - 1 : currentPos;
                switch (text[pos])
                {
                    case '!':
                    case '@':
                    case '#':
                    case '$':
                    case '%':
                    case '^':
                    case '&':
                    case '*':
                    case '(':
                    case ')':
                    case '_':
                    case '/':
                    case '?':
                    case '>':
                    case '<':
                        text.Remove(text[pos]);
                        break;
                    default: return;
                }
                FIO.Text = new string(text.ToArray());
                FIO.SelectionStart = currentPos;
                e.Handled = true;
            }
            catch
            {
                return;
            }
        }
    }
}
