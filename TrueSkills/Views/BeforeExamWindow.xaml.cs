using System;
using System.Collections.Generic;
using System.Text;
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
    /// Логика взаимодействия для BeforeExamWindow.xaml
    /// </summary>
    public partial class BeforeExamWindow : Window
    {
        public BeforeExamWindow()
        {
            InitializeComponent();
            TbExam.Text = TemporaryVariables.GetProperty("a_Step" + (int)TemporaryVariables.s_step);
        }
    }
}
