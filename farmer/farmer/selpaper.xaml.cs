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

namespace farmer
{
    /// <summary>
    /// Логика взаимодействия для selpaper.xaml
    /// </summary>
    public partial class selpaper : Window
    {
        public selpaper()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            paperformatcomboBox.ItemsSource = cccombo.typepapercomboBoxitems;
            paperformatcomboBox.SelectedIndex = 0;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.selpapertypestr = paperformatcomboBox.Text;
            MainWindow.selpapertypeint = (int[])paperformatcomboBox.SelectedValue;
            this.Close();
        }
    }
}
