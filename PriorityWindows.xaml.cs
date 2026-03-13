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
using System.Windows.Shapes;

namespace Юмагулов_Глазки_save
{
    /// <summary>
    /// Логика взаимодействия для PriorityWindows.xaml
    /// </summary>
    public partial class PriorityWindows : Window
    {
        public int NewPriority { get; set; }
        public PriorityWindows(int maxPriority)
        {
            InitializeComponent();
            TBox_Priority.Text = maxPriority.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (int.TryParse(TBox_Priority.Text, out int result))
            {
                if (result < 0)
                {
                    MessageBox.Show("Приоритет не может быть меньше 0!");
                    return;
                }


                NewPriority = result;
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Введите корректное целое число!");
            }
        }
    }
}
