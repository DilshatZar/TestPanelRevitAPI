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

namespace MyPanel
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AnswerWindow : Window
    {
        public AnswerWindow()
        {
            InitializeComponent();
        }
        public AnswerWindow(int number)
        {
            InitializeComponent();
            answerTextBlock.Text = number.ToString();
        }
        public AnswerWindow(double number, bool round = false)
        {
            InitializeComponent();
            if (round)
            {
                number = Math.Round(number);
            }
            answerTextBlock.Text = number.ToString();
        }
        public AnswerWindow(string text)
        {
            InitializeComponent();
            answerTextBlock.Text = text;
        }
    }
}
