using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Логика взаимодействия для newpasswin.xaml
    /// </summary>
    public partial class newpasswin : Window
    {
        public newpasswin()
        {
            InitializeComponent();
        }
        string GetMd5Hash(string input)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            farmer.Properties.Settings.Default.pass = GetMd5Hash(newpasswordBox.Password);
            farmer.Properties.Settings.Default.Save();
            this.Close();
        }

        private void newpasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter)) button_Click(this, e);
        }
        private void cancelbutton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
