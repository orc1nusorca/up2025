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

namespace PM_04_ISP32_SSE.pages
{
    /// <summary>
    /// Логика взаимодействия для ModalQuantity.xaml
    /// </summary>
    public partial class ModalQuantity : Window
    {
        public decimal NewQuantity { get; private set; }
        public ModalQuantity(decimal currentMax)
        {
            InitializeComponent();
            QuantityTextBox.Text = currentMax.ToString();
            QuantityTextBox.SelectAll();
        }
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(QuantityTextBox.Text, out decimal quantity))
            {
                NewQuantity = quantity;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Введите число!");
            }
        }
    }
}
