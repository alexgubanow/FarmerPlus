using ProjApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace farmer
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class dialogwin : Window
    {
        public dialogwin()
        {
            InitializeComponent();
        }
        bool okstate = false;
        bool cancelstate = false;
        List<coordsformat> wgs84;
        List<coordsformat> ck63;
        string numcontrtextBoxText;
        int statuscomboBoxSelectedIndex;
        DateTime regdatePickerSelectedDate;
        DateTime termleasedatePickerSelectedDate;
        int fizoryurcomboBoxSelectedIndex;
        string lessortextBoxText;
        string inntextBoxText;
        string passporttextBoxText;
        DateTime vidandatePickerSelectedDate;
        string kemvidantextBoxText;
        string adresstextBoxText;
        string kodpredtextBoxText;
        int leaserfizoryurcomboBoxSelectedIndex;
        string leasertextBoxText;
        string leaserinntextBoxText;
        string leaserpassporttextBoxText;
        DateTime leaservidandatePickerSelectedDate;
        string leaserkemvidantextBoxText;
        string leaseradresstextBoxText;
        string leaserkodpredtextBoxText;
        string cadnumtextBoxText;
        string sqtextBoxText;
        string koatuutextBoxText;
        int formownercomboBoxSelectedIndex;
        int formusecomboBoxSelectedIndex;
        int formunitcomboBoxSelectedIndex;

        public string[] filenames { get; set; }
        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (importxmlthread == null)
            {
                buttonCancel.Content = "Стоп";
                if ((bool)checkBox.IsChecked)
                {
                    importxmlthread = new Thread(autoimportxml);
                    importxmlthread.Name = "autoimportxml";
                    importxmlthread.Start();
                }
                else
                {
                    importxmlthread = new Thread(importxml);
                    importxmlthread.Name = "manualimport";
                    importxmlthread.Start();
                }
            }
            okstate = true;
        }
        private void importxml()
        {
            for (int i = 0; i < filenames.Length; i++)
            {
                textBlock.Dispatcher.Invoke(new Action(() => textBlock.Text = (string)Application.Current.Resources["m_continue"] + filenames[i]));
                while (!okstate)
                { }
                okstate = false;
                this.Dispatcher.Invoke((Action)delegate ()
                {
                    progressBar.Value = i + 1;
                    contractwindow newcontract = new contractwindow();
                    newcontract.Owner = this;
                    newcontract.importfrxml(filenames[i]);
                    newcontract.numcontrtextBox.Text = (i + 1).ToString();
                    newcontract.ShowDialog();
                });
            }
            this.Dispatcher.BeginInvoke((Action)(() => this.Close()));
        }
        private void autoimportxml()
        {
            for (int i = 0; i < filenames.Length; i++)
            {
                if (cancelstate)
                {
                    buttonCancel.Dispatcher.Invoke(new Action(() => buttonCancel.Content = (string)Application.Current.Resources["m_cancel"]));
                    importxmlthread.Abort();
                }
                textBlock.Dispatcher.Invoke(new Action(() => textBlock.Text = (string)Application.Current.Resources["m_continue"] + filenames[i]));
                progressBar.Dispatcher.Invoke(new Action(() => progressBar.Value = i + 1));
                //init
                ck63 = new List<coordsformat>();
                wgs84 = new List<coordsformat>();
                numcontrtextBoxText = (i + 1).ToString();
                statuscomboBoxSelectedIndex = 0;
                regdatePickerSelectedDate = DateTime.Now;
                termleasedatePickerSelectedDate = DateTime.Now;
                fizoryurcomboBoxSelectedIndex = 0;
                lessortextBoxText = "";
                inntextBoxText = "";
                passporttextBoxText = "";
                vidandatePickerSelectedDate = DateTime.Now;
                kemvidantextBoxText = "";
                adresstextBoxText = "";
                kodpredtextBoxText = "";
                leaserfizoryurcomboBoxSelectedIndex = 0;
                leasertextBoxText = "";
                leaserinntextBoxText = "";
                leaserpassporttextBoxText = "";
                leaservidandatePickerSelectedDate = DateTime.Now;
                leaserkemvidantextBoxText = "";
                leaseradresstextBoxText = "";
                leaserkodpredtextBoxText = "";
                cadnumtextBoxText = "";
                sqtextBoxText = "";
                koatuutextBoxText = "";
                formownercomboBoxSelectedIndex = 0;
                formusecomboBoxSelectedIndex = 0;
                formunitcomboBoxSelectedIndex = 0;
                //import
                bool res = false;
                res = importfrxml(filenames[i]);
                //insert
                if (res)
                {
                    insertindb();
                }
                else
                {
                    this.Dispatcher.Invoke((Action)delegate ()
                    {
                        contractwindow newcontract = new contractwindow();
                        newcontract.Owner = this;
                        newcontract.importfrxml(filenames[i]);
                        newcontract.numcontrtextBox.Text = (i + 1).ToString();
                        newcontract.ShowDialog();
                    });
                }
            }
            this.Dispatcher.BeginInvoke((Action)(() => this.Close()));
        }
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (buttonCancel.Content.ToString() == "Стоп")
            {
                if (importxmlthread.Name == "manualimport")
                {
                    buttonCancel.Content = (string)Application.Current.Resources["m_cancel"];
                    if (importxmlthread.IsAlive)
                    { importxmlthread.Abort(); }
                }
                cancelstate = true;
            }
            else
            {
                this.Close();
            }
        }
        Thread importxmlthread;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBlock.Text = (string)Application.Current.Resources["m_continue"] + filenames[0];
            buttonCancel.Content = (string)Application.Current.Resources["m_cancel"];
            buttonOK.Content = "Ок";
            progressBar.Minimum = 1;
            progressBar.Maximum = filenames.Length;
        }
        public bool importfrxml(string FileName)
        {
            myvariable.errcount = 0;
            myvariable.errstr = "";
            string regdate = xml.importtext(FileName, "RegistrationDate", true);
            DateTime nulldate = Convert.ToDateTime("01/01/90");
            if (regdate != "" && Convert.ToDateTime(regdate) > Convert.ToDateTime("01/01/90"))
            {
                statuscomboBoxSelectedIndex = 0;
                regdatePickerSelectedDate = Convert.ToDateTime(regdate);
                if (!DateTime.TryParse(xml.importtext(FileName, "LeaseDuration", true), out nulldate))
                {
                }
                termleasedatePickerSelectedDate = nulldate;
            }
            else
            {
                statuscomboBoxSelectedIndex = 1;
                regdatePickerSelectedDate = nulldate;
                termleasedatePickerSelectedDate = nulldate;
            }
            string typeGrantor = xml.importtext(FileName, "typeGrantor", true);
            if (typeGrantor != "")
            {
                if (typeGrantor == "NaturalPerson")
                {
                    fizoryurcomboBoxSelectedIndex = 0;
                    lessortextBoxText = xml.importcontractorinf(FileName, "NaturalPerson_FullName", "Grantor", true);
                    inntextBoxText = xml.importcontractorinf(FileName, "NaturalPerson_TaxNumber", "Grantor", true);
                    passporttextBoxText = xml.importcontractorinf(FileName, "NaturalPerson_Passport", "Grantor", true);
                    if (!DateTime.TryParse(xml.importcontractorinf(FileName, "NaturalPerson_PassportIssuedDate", "Grantor", true), out nulldate))
                    {
                        myvariable.errcount++;
                    }
                    vidandatePickerSelectedDate = nulldate;
                    kemvidantextBoxText = xml.importcontractorinf(FileName, "NaturalPerson_IssuanceAuthority", "Grantor", true);
                    adresstextBoxText = xml.importcontractorinf(FileName, "NaturalPerson_Address", "Grantor", true);
                }
                else if (typeGrantor == "LegalEntity")
                {
                    fizoryurcomboBoxSelectedIndex = 1;
                    lessortextBoxText = xml.importcontractorinf(FileName, "LegalEntity_Name", "Grantor", true);
                    kodpredtextBoxText = xml.importcontractorinf(FileName, "LegalEntity_EDRPOU", "Grantor", true);
                    adresstextBoxText = xml.importcontractorinf(FileName, "LegalEntity_Address", "Grantor", true);
                }
            }
            string typeGrantee = xml.importtext(FileName, "typeGrantee", true);
            if (typeGrantee != "")
            {
                if (typeGrantee == "NaturalPerson")
                {
                    leaserfizoryurcomboBoxSelectedIndex = 0;
                    leasertextBoxText = xml.importcontractorinf(FileName, "NaturalPerson_FullName", "Grantee", true);
                    leaserinntextBoxText = xml.importcontractorinf(FileName, "NaturalPerson_TaxNumber", "Grantee", true);
                    leaserpassporttextBoxText = xml.importcontractorinf(FileName, "NaturalPerson_Passport", "Grantee", true);
                    if (xml.importcontractorinf(FileName, "NaturalPerson_PassportIssuedDate", "Grantee", true) != "")
                    {
                        leaservidandatePickerSelectedDate = Convert.ToDateTime(xml.importcontractorinf(FileName, "NaturalPerson_PassportIssuedDate", "Grantee", true));
                    }
                    else
                    {
                        leaservidandatePickerSelectedDate = Convert.ToDateTime("01/01/53");
                    }
                    leaserkemvidantextBoxText = xml.importcontractorinf(FileName, "NaturalPerson_IssuanceAuthority", "Grantee", true);
                    leaseradresstextBoxText = xml.importcontractorinf(FileName, "NaturalPerson_Address", "Grantee", true);
                }
                else if (typeGrantee == "LegalEntity")
                {
                    leaserfizoryurcomboBoxSelectedIndex = 1;
                    leasertextBoxText = xml.importcontractorinf(FileName, "LegalEntity_Name", "Grantee", true);
                    leaserkodpredtextBoxText = xml.importcontractorinf(FileName, "LegalEntity_EDRPOU", "Grantee", true);
                    leaseradresstextBoxText = xml.importcontractorinf(FileName, "LegalEntity_Address", "Grantee", true);
                }
            }
            cadnumtextBoxText = xml.importtext(FileName, "CadastralCode", true);
            sqtextBoxText = xml.importtext(FileName, "Size", true);
            koatuutextBoxText = xml.importtext(FileName, "KOATUU", true);
            formownercomboBoxSelectedIndex = xml.importint(FileName, "OwnershipInfo", true);
            formusecomboBoxSelectedIndex = xml.importint(FileName, "CategoryPurposeInfo", true);
            formunitcomboBoxSelectedIndex = xml.importint(FileName, "LandCode", true);
            ck63.Clear();
            wgs84.Clear();
            ck63 = xml.importcoords(FileName, true);
            if (ck63 != null)
            {
                for (int i = 0; i < ck63.Count; i++)
                {
                    double[] x = { Convert.ToDouble(ck63[i].y.ToString().Replace(".", ",")) };
                    double[] y = { Convert.ToDouble(ck63[i].x.ToString().Replace(".", ",")) };
                    Projection src = new Projection(IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "CK63", "z" + x[0].ToString()[0]));
                    Projection dst = new Projection(@"+proj=longlat +ellps=WGS84 +datum=WGS84 +no_defs");
                    Projection.Transform(src, dst, x, y, null);
                    wgs84.Add(new coordsformat() { x = y[0].ToString().Replace(",", "."), y = x[0].ToString().Replace(",", "."), rank = ck63[i].rank });
                }
            }
            string[] errstrs = myvariable.errstr.Split(':');
            if (myvariable.errcount > 0)
            {
                if (errstrs != null)
                {
                    int res = 0;
                    foreach (string param in errstrs)
                    {
                        if (IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "errlvl", param) != "0" && param != "")
                        {
                            res++;
                        }
                    }
                    if (res > 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
        private void insertindb()
        {
            if ((bdfunc.select("select id from contracts where cadastral_number='" + cadnumtextBoxText + "'").Count > 0))
            {
                MessageBoxResult result = MessageBox.Show((string)Application.Current.Resources["m_doublecadnum"], (string)FindResource("m_Error"), MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    bdfunc.insert(@"insert into contracts (contract_number,date_contract,lessor,cadastral_number,square,term_lease,registration_date,contract_status," +
                        "unit_number,notes,type_contract,area_number,form_owner,form_use,form_unit,koatuu,leaser,contractortype) values ('" + numcontrtextBoxText + "','" +
        DateTime.Now.ToString("dd.MM.yyyy") + "','" + lessortextBoxText.Replace("'", "''") + "','" + cadnumtextBoxText + "','" + sqtextBoxText.Replace('.', ',') + "','" +
        termleasedatePickerSelectedDate.ToString("dd.MM.yyyy") + "','" + regdatePickerSelectedDate.ToString("dd.MM.yyyy") + "','" + statuscomboBoxSelectedIndex +
        "','" + "" + "','" + "" + "','" + "0" + "','" + "" + "','" + formownercomboBoxSelectedIndex +
        "','" + formusecomboBoxSelectedIndex + "','" + formunitcomboBoxSelectedIndex + "','" + koatuutextBoxText + "','" + leasertextBoxText + "','" +
        fizoryurcomboBoxSelectedIndex.ToString() + leaserfizoryurcomboBoxSelectedIndex.ToString() + "')");

                    ArrayList res = bdfunc.maxid();
                    int maxid = (int)res[0];
                    if (fizoryurcomboBoxSelectedIndex == 0)
                    {
                        bdfunc.insert("INSERT INTO lessorfiz (ID,PASSPORT_DATE,WHO_GIVE,PASSPORT_SERIES,ADRESS,TELEPHONE,INN) VALUES ('" + maxid + "','" + vidandatePickerSelectedDate.ToString("dd.MM.yyyy") +
                            "','" + kemvidantextBoxText.Replace("'", "''") + "','" + passporttextBoxText + "','" + adresstextBoxText.Replace("'", "''") + "','" + "" + "','" + inntextBoxText + "')");
                    }
                    else if (fizoryurcomboBoxSelectedIndex == 1)
                    {
                        bdfunc.insert("INSERT INTO lessoryur (ID,TELEPHONE,CODEPRED,ADRESS,CHET,BANK,MFO,SVIDOT) VALUES ('" +
                            maxid + "','" + "" + "','" + kodpredtextBoxText + "','" + adresstextBoxText.Replace("'", "''") + "','" + "" + "','" +
                            "" + "','" + "" + "','" + "" + "')");
                    }
                    if (leaserfizoryurcomboBoxSelectedIndex == 0)
                    {
                        bdfunc.insert("INSERT INTO leaserfiz (ID,PASSPORT_DATE,WHO_GIVE,PASSPORT_SERIES,ADRESS,TELEPHONE,INN) VALUES ('" + maxid + "','" + leaservidandatePickerSelectedDate.ToString("dd.MM.yyyy") +
                            "','" + leaserkemvidantextBoxText.Replace("'", "''") + "','" + leaserpassporttextBoxText + "','" + leaseradresstextBoxText.Replace("'", "''") + "','" + "" + "','" +
                            leaserinntextBoxText + "')");
                    }
                    else if (leaserfizoryurcomboBoxSelectedIndex == 1)
                    {
                        bdfunc.insert("INSERT INTO leaseryur (ID,TELEPHONE,CODEPRED,ADRESS,CHET,BANK,MFO,SVIDOT) VALUES ('" +
                            maxid + "','" + "" + "','" + leaserkodpredtextBoxText + "','" + leaseradresstextBoxText.Replace("'", "''") + "','" + "" + "','" +
                            "" + "','" + "" + "','" + "" + "')");
                    }
                    if (ck63 != null)
                    {
                        for (int i = 0; i < ck63.Count; i++)
                        {
                            bdfunc.insert("INSERT INTO CK63 (ID,X,Y,RANK) VALUES ('" + maxid + "','" + ck63[i].x.ToString() + "','" + ck63[i].y.ToString() + "','" + ck63[i].rank.ToString() + "')");
                        }
                    }
                    else
                    {
                        MessageBox.Show("No ck63");
                    }
                    if (wgs84 != null)
                    {
                        for (int i = 0; i < wgs84.Count; i++)
                        {
                            bdfunc.insert("INSERT INTO WGS84 (ID,X,Y,RANK) VALUES ('" + maxid + "','" + wgs84[i].x.ToString() + "','" + wgs84[i].y.ToString() + "','" + wgs84[i].rank.ToString() + "')");
                        }
                    }
                    else
                    {
                        MessageBox.Show("No wgs84");
                    }
                }
            }
            else
            {
                bdfunc.insert(@"insert into contracts (contract_number,date_contract,lessor,cadastral_number,square,term_lease,registration_date,contract_status," +
                          "unit_number,notes,type_contract,area_number,form_owner,form_use,form_unit,koatuu,leaser,contractortype) values ('" + numcontrtextBoxText + "','" +
          DateTime.Now.ToString("dd.MM.yyyy") + "','" + lessortextBoxText.Replace("'", "''") + "','" + cadnumtextBoxText + "','" + sqtextBoxText.Replace('.', ',') + "','" +
          termleasedatePickerSelectedDate.ToString("dd.MM.yyyy") + "','" + regdatePickerSelectedDate.ToString("dd.MM.yyyy") + "','" + statuscomboBoxSelectedIndex +
          "','" + "" + "','" + "" + "','" + "0" + "','" + "" + "','" + formownercomboBoxSelectedIndex +
          "','" + formusecomboBoxSelectedIndex + "','" + formunitcomboBoxSelectedIndex + "','" + koatuutextBoxText + "','" + leasertextBoxText + "','" +
          fizoryurcomboBoxSelectedIndex.ToString() + leaserfizoryurcomboBoxSelectedIndex.ToString() + "')");

                ArrayList res = bdfunc.maxid();
                int maxid = (int)res[0];
                if (fizoryurcomboBoxSelectedIndex == 0)
                {
                    bdfunc.insert("INSERT INTO lessorfiz (ID,PASSPORT_DATE,WHO_GIVE,PASSPORT_SERIES,ADRESS,TELEPHONE,INN) VALUES ('" + maxid + "','" + vidandatePickerSelectedDate.ToString("dd.MM.yyyy") +
                        "','" + kemvidantextBoxText.Replace("'", "''") + "','" + passporttextBoxText + "','" + adresstextBoxText.Replace("'", "''") + "','" + "" + "','" + inntextBoxText + "')");
                }
                else if (fizoryurcomboBoxSelectedIndex == 1)
                {
                    bdfunc.insert("INSERT INTO lessoryur (ID,TELEPHONE,CODEPRED,ADRESS,CHET,BANK,MFO,SVIDOT) VALUES ('" +
                        maxid + "','" + "" + "','" + kodpredtextBoxText + "','" + adresstextBoxText.Replace("'", "''") + "','" + "" + "','" +
                        "" + "','" + "" + "','" + "" + "')");
                }
                if (leaserfizoryurcomboBoxSelectedIndex == 0)
                {
                    bdfunc.insert("INSERT INTO leaserfiz (ID,PASSPORT_DATE,WHO_GIVE,PASSPORT_SERIES,ADRESS,TELEPHONE,INN) VALUES ('" + maxid + "','" + leaservidandatePickerSelectedDate.ToString("dd.MM.yyyy") +
                        "','" + leaserkemvidantextBoxText.Replace("'", "''") + "','" + leaserpassporttextBoxText + "','" + leaseradresstextBoxText.Replace("'", "''") + "','" + "" + "','" +
                        leaserinntextBoxText + "')");
                }
                else if (leaserfizoryurcomboBoxSelectedIndex == 1)
                {
                    bdfunc.insert("INSERT INTO leaseryur (ID,TELEPHONE,CODEPRED,ADRESS,CHET,BANK,MFO,SVIDOT) VALUES ('" +
                        maxid + "','" + "" + "','" + leaserkodpredtextBoxText + "','" + leaseradresstextBoxText.Replace("'", "''") + "','" + "" + "','" +
                        "" + "','" + "" + "','" + "" + "')");
                }
                if (ck63 != null)
                {
                    for (int i = 0; i < ck63.Count; i++)
                    {
                        bdfunc.insert("INSERT INTO CK63 (ID,X,Y,RANK) VALUES ('" + maxid + "','" + ck63[i].x.ToString() + "','" + ck63[i].y.ToString() + "','" + ck63[i].rank.ToString() + "')");
                    }
                }
                else
                {
                    MessageBox.Show("No ck63");
                }
                if (wgs84 != null)
                {
                    for (int i = 0; i < wgs84.Count; i++)
                    {
                        bdfunc.insert("INSERT INTO WGS84 (ID,X,Y,RANK) VALUES ('" + maxid + "','" + wgs84[i].x.ToString() + "','" + wgs84[i].y.ToString() + "','" + wgs84[i].rank.ToString() + "')");
                    }
                }
                else
                {
                    MessageBox.Show("No wgs84");
                }
            }
        }
    }
}
