using Microsoft.Win32;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace parsekoatuu
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        Thread parseThread;
        Thread insertThread;
        private void opbutton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                optextBox.Text = openFileDialog.FileName;
            }
        }
        private class koatuuid
        {
            public string id { get; set; }
            public string name { get; set; }
            public string lat { get; set; }
            public string lng { get; set; }
        }
        private void lwinsh()
        {
            string filekoatupat = "";
            this.Dispatcher.Invoke((Action)(() =>
            {
                filekoatupat = optextBox.Text;
                progressbar1.Value = 0;
                progressbar1.Maximum = 6;
            }));
            int lineCount = 0;
            using (StreamReader sr = new StreamReader(filekoatupat, Encoding.GetEncoding(1251)))
            {
                while (sr.ReadLine() != null)
                {
                    lineCount++;
                }
            }
            this.Dispatcher.Invoke((Action)(() =>
            {
                progressbar1.Value++;
                restextBlock.Text = "Read rows: " + lineCount + " end";
            }));
            string[,] parsestr = new string[lineCount, 3];
            int i = 0;
            using (StreamReader sr = new StreamReader(filekoatupat, Encoding.GetEncoding(1251)))
            {
                string source;
                while ((source = sr.ReadLine()) != null)
                {
                    string[] str = source.Split(new Char[] { '\t' });
                    if (str[0] != null)
                    {
                        parsestr[i, 0] = str[0];
                        parsestr[i, 1] = str[1];
                        parsestr[i, 2] = str[2];
                        i++;
                    }
                }
            }
            this.Dispatcher.Invoke((Action)(() =>
            {
                progressbar1.Value++;
                restextBlock.Text = "parse string end";
            }));
            List<koatuuid> obl = new List<koatuuid>();
            for (int l = 0; l < parsestr.Length / 3; l++)
            {
                if (parsestr[l, 2].Contains("ОБЛАСТЬ") | parsestr[l, 2].Contains("АВТОНОМНА РЕСПУБЛІКА КРИМ"))
                {
                    string res = "";
                    for (int k = 0; k < parsestr[l, 2].Length; k++)
                    {
                        if (parsestr[l, 2][k] != '/')
                        {
                            res = res + parsestr[l, 2][k];
                        }
                        else
                        {
                            break;
                        }
                    }
                    obl.Add(new koatuuid { id = parsestr[l, 0], name = res.Replace(" ОБЛАСТЬ", "")});
                }
            }
            this.Dispatcher.Invoke((Action)(() =>
            {
                progressbar1.Value++;
                restextBlock.Text = "parse obl end";
            }));
            List<koatuuid> ray = new List<koatuuid>();
            for (int l = 0; l < parsestr.Length / 3; l++)
            {
                if (parsestr[l, 2].Contains("РАЙОН"))
                {
                    string res = "";
                    for (int k = 0; k < parsestr[l, 2].Length; k++)
                    {
                        if (parsestr[l, 2][k] != '/')
                        {
                            res = res + parsestr[l, 2][k];
                        }
                        else
                        {
                            break;
                        }
                    }
                    ray.Add(new koatuuid { id = parsestr[l, 0], name = res.Replace(" РАЙОН", "").Replace("'", "''") });
                }
            }
            this.Dispatcher.Invoke((Action)(() =>
            {
                progressbar1.Value++;
                restextBlock.Text = "parse ray end";
            }));
            List<koatuuid> sil = new List<koatuuid>();
            for (int l = 0; l < parsestr.Length / 3; l++)
            {
                if (parsestr[l, 2].Contains("/С."))
                {
                    string res = "";
                    for (int k = 0; k < parsestr[l, 2].Length; k++)
                    {
                        if (parsestr[l, 2][k] != '/')
                        {
                            res = res + parsestr[l, 2][k];
                        }
                        else
                        {
                            break;
                        }
                    }
                    sil.Add(new koatuuid { id = parsestr[l, 0], name = res.Replace("'", "''") });
                }
            }
            this.Dispatcher.Invoke((Action)(() =>
            {
                progressbar1.Value++;
                restextBlock.Text = "parse sil end";
            }));
            List<koatuuid> allrec = new List<koatuuid>();
            for (int l = 0; l < parsestr.Length / 3; l++)
            {
                allrec.Add(new koatuuid { id = parsestr[l, 0], name = parsestr[l, 2].Replace("'", "''") });
            }
            this.Dispatcher.Invoke((Action)(() =>
            {
                progressbar1.Value++;
                restextBlock.Text = "parse all end";
                progressbar1.Maximum = allrec.Count;
            }));
            sql("TRUNCATE obl, rayon, sil;");
            this.Dispatcher.Invoke((Action)(() =>
            {
                restextBlock.Text = "clear db";
            }));
            string sqlstr = "";
            for (int l = 13; l < obl.Count; l++)
            {
                WebClient client = new WebClient();
                Stream stream;
                if (obl[l].name.Contains("АВТОНОМНА РЕСПУБЛІКА КРИМ"))
                {
                    stream = client.OpenRead("https://geocode-maps.yandex.ru/1.x/?geocode=" + System.Uri.EscapeUriString(obl[l].name) + "&results=1");
                }
                else
                {
                    stream = client.OpenRead("https://geocode-maps.yandex.ru/1.x/?geocode=" + System.Uri.EscapeUriString(obl[l].name + " область") + "&results=1");
                }
                StreamReader reader = new StreamReader(stream);
                String content = reader.ReadToEnd();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(content);
                if (Convert.ToInt32(xmlDoc.GetElementsByTagName("found")[0].InnerText) > 0)
                {
                    XmlNodeList location = xmlDoc.GetElementsByTagName("pos");
                    string[] coods = location[0].InnerText.Split(' ');
                    string lat = coods[1];
                    string lng = coods[0];
                    sqlstr = sqlstr + ("INSERT INTO obl VALUES('" + obl[l].id + "', '" + obl[l].name + "', '" + lat + "', '" + lng + "');") + "\r\n";
                }
                else
                {
                    MessageBox.Show("No locatInfo for " + obl[l].name);
                }
                this.Dispatcher.Invoke((Action)(() =>
                {
                    progressbar1.Value++;
                    restextBlock.Text = "find locinfo for: " + obl[l].name + " end";
                }));
                for (int k = 0; k < ray.Count; k++)
                {
                    if (ray[k].id.Remove(2) == obl[l].id.Remove(2))
                    {
                        stream = client.OpenRead("https://geocode-maps.yandex.ru/1.x/?geocode=" + System.Uri.EscapeUriString(obl[l].name + " область," + ray[k].name + " район") + "&results=1");
                        reader = new StreamReader(stream);
                        content = reader.ReadToEnd();
                        xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(content);
                        if (Convert.ToInt32(xmlDoc.GetElementsByTagName("found")[0].InnerText) > 0)
                        {
                            XmlNodeList location = xmlDoc.GetElementsByTagName("pos");
                            string[] coods = location[0].InnerText.Split(' ');
                            string lat = coods[1];
                            string lng = coods[0];
                            sqlstr = sqlstr + ("INSERT INTO rayon VALUES('" + ray[k].id + "', '" + ray[k].name + "', '" + lat + "', '" + lng + "');") + "\r\n";
                        }
                        else
                        {
                            MessageBox.Show("No locatInfo for " + ray[k].name);
                        }
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            progressbar1.Value++;
                            restextBlock.Text = "find locinfo for: " + obl[l].name+", " + ray[k].name + " end";
                        }));
                        for (int m = 0; m < sil.Count; m++)
                        {
                            if (sil[m].id.Remove(5) == ray[k].id.Remove(5))
                            {
                                List<koatuuid> filselo = new List<koatuuid>();
                                for (int n = 0; n < allrec.Count; n++)
                                {
                                    if (allrec[n].id.Contains(sil[m].id.Remove(8))) { filselo.Add(allrec[n]); }
                                }
                                double[] lats = new double[filselo.Count];
                                double[] lngs = new double[filselo.Count];
                                for (int n = 0; n < filselo.Count; n++)
                                {
                                    starasd:
                                    try
                                    {
                                        stream = client.OpenRead("https://geocode-maps.yandex.ru/1.x/?geocode=" + System.Uri.EscapeUriString(obl[l].name + " область,"
                                        + ray[k].name + " район," + filselo[n].name) + "&results=1");
                                    reader = new StreamReader(stream);
                                    content = reader.ReadToEnd();
                                    xmlDoc = new XmlDocument();
                                    xmlDoc.LoadXml(content);
                                        if (Convert.ToInt32(xmlDoc.GetElementsByTagName("found")[0].InnerText) > 0)
                                        {
                                            XmlNodeList location = xmlDoc.GetElementsByTagName("pos");
                                            string[] coods = location[0].InnerText.Split(' ');
                                            lats[n] = Convert.ToDouble(coods[1].Replace('.', ','));
                                            lngs[n] = Convert.ToDouble(coods[0].Replace('.', ','));
                                        }
                                        else
                                        {
                                            MessageBox.Show("No locatInfo for " + filselo[n].name);
                                        }
                                        this.Dispatcher.Invoke((Action)(() =>
                                        {
                                            progressbar1.Value++;
                                            restextBlock.Text = "find locinfo for: " + obl[l].name + ", " + ray[k].name +", " + filselo[n].name + " end";
                                        }));
                                    }
                                    catch
                                    {
                                        goto starasd;
                                    }
                                }
                                string lat = lats.Average().ToString().Replace(',', '.');
                                string lng = lngs.Average().ToString().Replace(',', '.');
                                sqlstr = sqlstr + ("INSERT INTO sil VALUES('" + sil[m].id + "', '" + sil[m].name + "', '" + lat + "', '" + lng + "');") + "\r\n";
                            }
                        }
                    }
                }
                this.Dispatcher.Invoke((Action)(() =>
                {
                    restextBlock.Text = "save as file end";
                }));
                using (StreamWriter writetext = new StreamWriter(obl[l].name + "_"+l+"_sqlstr.txt"))
                {
                    writetext.Write(sqlstr);
                }
            }
            this.Dispatcher.Invoke((Action)(() =>
            {
                restextBlock.Text = "save as file end";
            }));
            using (StreamWriter writetext = new StreamWriter("sqlstr.txt"))
            {
                writetext.Write(sqlstr);
            }
            this.Dispatcher.Invoke((Action)(() =>
            {
                restextBlock.Text = "well done";
            }));
        }
        private void parsebutton_Click(object sender, RoutedEventArgs e)
        {
            parseThread = new Thread(lwinsh);
            parseThread.Start();
        }
        public static void sql(string SQL)
        {
            try
            {
                IniFile settings = new IniFile("settings.ini");
                using (var conn = new NpgsqlConnection("Server=" + settings.Read("server", "DB") + ";Port=" + settings.Read("port", "DB") +
                ";User Id=" + settings.Read("user", "DB") + ";Password=" + settings.Read("pass", "DB") + ";Database=" + settings.Read("database", "DB") + ";"))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;

                        // Insert some data
                        cmd.CommandText = SQL;
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Errr on " + SQL);
            }
        }
        private void insall()
        {
            sql("TRUNCATE obl, rayon, sil;");
            string[] lines = System.IO.File.ReadAllLines(@"sqlstr.txt");
            List<string> sqllines = new List<string>();
            foreach (string line in lines)
            {
                string sqllin = line;
                for (int i = 0; i < sqllin.Length; i++)
                {
                    if ((i + 1) < sqllin.Length && i !=0)
                    {
                        if (Char.IsLetter(sqllin[i - 1]) && Char.IsLetter(sqllin[i + 1]) && sqllin[i] == Char.Parse("'"))
                        {
                            sqllin ="";
                            int k = 0;
                            while (k<i)
                            {
                                sqllin = sqllin + line[k];
                                k++;
                            }
                            sqllin = sqllin + "''" + line.Substring(i + 1);
                        }
                    }
                }
                sqllines.Add(sqllin);
                this.Dispatcher.Invoke((Action)(() =>
                {
                    restextBlock.Text = sqllin + " done";
                }));
            }
            using (StreamWriter writetext = new StreamWriter("toinssqlstr.txt"))
            {
                writetext.Write(String.Join("\r\n", sqllines.ToArray()));
            }
            this.Dispatcher.Invoke((Action)(() =>
            {
                restextBlock.Text = "save as file end";
            }));
            sql(String.Join("\r\n", sqllines.ToArray()));
            this.Dispatcher.Invoke((Action)(() =>
            {
                restextBlock.Text = "insert done";
            }));
        }
        private void parsebutton_Copy_Click(object sender, RoutedEventArgs e)
        {
            insertThread = new Thread(insall);
            insertThread.Start();
        }
    }
}
