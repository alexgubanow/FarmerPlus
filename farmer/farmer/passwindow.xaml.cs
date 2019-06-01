using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
    /// Логика взаимодействия для passwindow.xaml
    /// </summary>
    public partial class passwindow : Window
    {
        //Thread ringThread;
        public passwindow()
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
            if (GetMd5Hash(passwordBox.Password) == farmer.Properties.Settings.Default.pass)
            {
                pass.oldpasstrue = true;
                passwordBox.Password = "";
                this.Close();
            }
            else
            {
                MessageBox.Show((string)Application.Current.Resources["m_wrongpass"], (string)Application.Current.Resources["m_Error"]);
                passwordBox.Password = "";
            }
        }

        private void passwordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter)) button_Click(this, e);
        }

        private void cancelbutton_Click(object sender, RoutedEventArgs e)
        {
            if (pass.nowchangepass)
            {
                this.Close();
            }
            else
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
    }
}
