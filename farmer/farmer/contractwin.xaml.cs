using ProjApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    /// Логика взаимодействия для contract.xaml
    /// </summary>
    public partial class contractwindow : Window
    {
        public class SmartCollection<T> : ObservableCollection<T>
        {
            public SmartCollection()
                : base()
            {
            }

            public SmartCollection(IEnumerable<T> collection)
                : base(collection)
            {
            }

            public SmartCollection(List<T> list)
                : base(list)
            {
            }

            public void AddRange(IEnumerable<T> range)
            {
                foreach (var item in range)
                {
                    Items.Add(item);
                }
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            public void Reset(IEnumerable<T> range)
            {
                this.Items.Clear();
                AddRange(range);
            }
        }
        public SmartCollection<coordsformat> ck63Entities { get; set; }
        public SmartCollection<coordsformat> wgs84Entities { get; set; }
        List<coordsformat> wgs84;
        List<coordsformat> ck63;
        ListCollectionView ck63View;
        ListCollectionView wgs84View;
        public contractwindow()
        {
            InitializeComponent();

            ck63 = new List<coordsformat>();
            ck63Entities = new SmartCollection<coordsformat>();
            ck63Entities.Reset(ck63);
            ck63View = (ListCollectionView)CollectionViewSource.GetDefaultView(ck63Entities);
            ck63listView.ItemsSource = ck63View;

            wgs84 = new List<coordsformat>();
            wgs84Entities = new SmartCollection<coordsformat>();
            wgs84Entities.Reset(wgs84);
            wgs84View = (ListCollectionView)CollectionViewSource.GetDefaultView(wgs84Entities);
            wgs84listView.ItemsSource = wgs84View;

            typecontrcomboBox.ItemsSource = cccombo.typecomboBoxitems;
            typecontrcomboBox.SelectedIndex = 0;
            statuscomboBox.ItemsSource = cccombo.statuscomboBoxitems;
            formownercomboBox.ItemsSource = cccombo.formownercomboBoxitems;
            formunitcomboBox.ItemsSource = cccombo.formunitcomboBoxitems;
            formusecomboBox.ItemsSource = cccombo.formusecomboBoxitems;
        }
        //bdfunc funcdb;
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        bool newcoords = false;
        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (farmer.Properties.Settings.Default.editdbenable)
            {

                if (currid != 0)
                {
                    bdfunc.update(@"update contracts set contract_number = '" + numcontrtextBox.Text + "',date_contract = '" + contrdatePicker.SelectedDate.Value.ToString("dd.MM.yyyy") +
                        "',lessor = '" + lessortextBox.Text.Replace("'", "''") + "',leaser = '" + leasertextBox.Text.Replace("'", "''") + "',cadastral_number = '" + cadnumtextBox.Text + "',square = '" + sqtextBox.Text.Replace('.', ',') + "',term_lease = '" +
                        termleasedatePicker.SelectedDate.Value.ToString("dd.MM.yyyy") + "',registration_date = '" + regdatePicker.SelectedDate.Value.ToString("dd.MM.yyyy") + "',contract_status = '" +
                        statuscomboBox.SelectedIndex + "',unit_number = '" + numunittextBox.Text + "',notes = '" + notestextBox.Text + "',type_contract = '" + typecontrcomboBox.SelectedIndex +
                        "',area_number = '" + numareatextBox.Text + "',form_owner = '" + formownercomboBox.SelectedIndex + "',contractortype = '" + fizoryurcomboBox.SelectedIndex.ToString() +
                        leaserfizoryurcomboBox.SelectedIndex.ToString() + "',form_use = '" + formusecomboBox.SelectedIndex + "',form_unit = '" + formunitcomboBox.SelectedIndex + "',koatuu = '" +
                        koatuutextBox.Text + "' where id =" + currid);

                    if (fizoryurcomboBox.SelectedIndex == 0)
                    {
                        bdfunc.update("update lessorfiz set PASSPORT_DATE = '" + vidandatePicker.SelectedDate.Value.ToString("dd.MM.yyyy") + "',WHO_GIVE = '" + kemvidantextBox.Text.Replace("'", "''") +
                            "',PASSPORT_SERIES ='" + passporttextBox.Text + "',ADRESS = '" + adresstextBox.Text.Replace("'", "''") + "',TELEPHONE = '" + numbertextBox.Text + "',INN = '" + inntextBox.Text + "' where id = " + currid);
                    }
                    else if (fizoryurcomboBox.SelectedIndex == 1)
                    {
                        bdfunc.update("update lessoryur set TELEPHONE = '" + numbertextBox.Text + "',CODEPRED = '" + kodpredtextBox.Text + "',ADRESS ='" + adresstextBox.Text.Replace("'", "''") +
                            "',CHET = '" + schettextBox.Text + "',BANK = '" + banktextBox.Text + "',MFO = '" + mfotextBox.Text + "',SVIDOT = '" + svidottextBox.Text + "' where id = " + currid);
                    }

                    if (leaserfizoryurcomboBox.SelectedIndex == 0)
                    {
                        bdfunc.update("update leaserfiz set PASSPORT_DATE = '" + leaservidandatePicker.SelectedDate.Value.ToString("dd.MM.yyyy") + "',WHO_GIVE = '" + leaserkemvidantextBox.Text.Replace("'", "''") +
                            "',PASSPORT_SERIES ='" + leaserpassporttextBox.Text + "',ADRESS = '" + leaseradresstextBox.Text.Replace("'", "''") + "',TELEPHONE = '" + leasernumbertextBox.Text + "',INN = '" + leaserinntextBox.Text + "' where id = " + currid);
                    }
                    else if (leaserfizoryurcomboBox.SelectedIndex == 1)
                    {
                        bdfunc.update("update leaseryur set TELEPHONE = '" + leasernumbertextBox.Text + "',CODEPRED = '" + leaserkodpredtextBox.Text + "',ADRESS ='" + leaseradresstextBox.Text.Replace("'", "''") +
                            "',CHET = '" + leaserschettextBox.Text + "',BANK = '" + leaserbanktextBox.Text + "',MFO = '" + leasermfotextBox.Text + "',SVIDOT = '" + leasersvidottextBox.Text + "' where id = " + currid);
                    }
                    if (newcoords)
                    {
                        //delete all curr coords
                        bdfunc.delete("DELETE FROM ck63 WHERE id = '" + currid + "'");
                        bdfunc.delete("DELETE FROM wgs84 WHERE id = '" + currid + "'");
                        //insert new coords
                        if (ck63 != null)
                        {
                            for (int i = 0; i < ck63.Count; i++)
                            {
                                bdfunc.insert("INSERT INTO CK63 (ID,X,Y,RANK) VALUES ('" + currid + "','" + ck63[i].x.ToString() + "','" + ck63[i].y.ToString() + "','" + ck63[i].rank.ToString() + "')");
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
                                bdfunc.insert("INSERT INTO WGS84 (ID,X,Y,RANK) VALUES ('" + currid + "','" + wgs84[i].x.ToString() + "','" + wgs84[i].y.ToString() + "','" + wgs84[i].rank.ToString() + "')");
                            }
                        }
                        else
                        {
                            MessageBox.Show("No wgs84");
                        }
                    }
                    this.Close();
                }
                else
                {
                    if ((bdfunc.select("select id from contracts where cadastral_number='" + cadnumtextBox.Text + "'").Count > 0))
                    {
                        MessageBoxResult result = MessageBox.Show((string)Application.Current.Resources["m_doublecadnum"], (string)FindResource("m_Error"), MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            bdfunc.insert(@"insert into contracts (contract_number,date_contract,lessor,cadastral_number,square,term_lease,registration_date,contract_status," +
                                "unit_number,notes,type_contract,area_number,form_owner,form_use,form_unit,koatuu,leaser,contractortype) values ('" + numcontrtextBox.Text + "','" +
                contrdatePicker.SelectedDate.Value.ToString("dd.MM.yyyy") + "','" + lessortextBox.Text.Replace("'", "''") + "','" + cadnumtextBox.Text + "','" + sqtextBox.Text.Replace('.', ',') + "','" +
                termleasedatePicker.SelectedDate.Value.ToString("dd.MM.yyyy") + "','" + regdatePicker.SelectedDate.Value.ToString("dd.MM.yyyy") + "','" + statuscomboBox.SelectedIndex +
                "','" + numunittextBox.Text + "','" + notestextBox.Text + "','" + typecontrcomboBox.SelectedIndex + "','" + numareatextBox.Text + "','" + formownercomboBox.SelectedIndex +
                "','" + formusecomboBox.SelectedIndex + "','" + formunitcomboBox.SelectedIndex + "','" + koatuutextBox.Text + "','" + leasertextBox.Text.Replace("'", "''") + "','" +
                fizoryurcomboBox.SelectedIndex.ToString() + leaserfizoryurcomboBox.SelectedIndex.ToString() + "')");
                            ArrayList res = bdfunc.maxid();
                            int maxid = (int)res[0];
                            if (fizoryurcomboBox.SelectedIndex == 0)
                            {
                                bdfunc.insert("INSERT INTO lessorfiz (ID,PASSPORT_DATE,WHO_GIVE,PASSPORT_SERIES,ADRESS,TELEPHONE,INN) VALUES ('" + maxid + "','" + vidandatePicker.SelectedDate.Value.ToString("dd.MM.yyyy") +
                                    "','" + kemvidantextBox.Text.Replace("'", "''") + "','" + passporttextBox.Text + "','" + adresstextBox.Text.Replace("'", "''") + "','" + numbertextBox.Text + "','" + inntextBox.Text + "')");
                            }
                            else if (fizoryurcomboBox.SelectedIndex == 1)
                            {
                                bdfunc.insert("INSERT INTO lessoryur (ID,TELEPHONE,CODEPRED,ADRESS,CHET,BANK,MFO,SVIDOT) VALUES ('" +
                                    maxid + "','" + numbertextBox.Text + "','" + kodpredtextBox.Text + "','" + adresstextBox.Text.Replace("'", "''") + "','" + schettextBox.Text + "','" +
                                    banktextBox.Text + "','" + mfotextBox.Text + "','" + svidottextBox.Text + "')");
                            }
                            if (leaserfizoryurcomboBox.SelectedIndex == 0)
                            {
                                bdfunc.insert("INSERT INTO leaserfiz (ID,PASSPORT_DATE,WHO_GIVE,PASSPORT_SERIES,ADRESS,TELEPHONE,INN) VALUES ('" + maxid + "','" + leaservidandatePicker.SelectedDate.Value.ToString("dd.MM.yyyy") +
                                    "','" + leaserkemvidantextBox.Text.Replace("'", "''") + "','" + leaserpassporttextBox.Text + "','" + leaseradresstextBox.Text.Replace("'", "''") + "','" + leasernumbertextBox.Text + "','" + leaserinntextBox.Text + "')");
                            }
                            else if (leaserfizoryurcomboBox.SelectedIndex == 1)
                            {
                                bdfunc.insert("INSERT INTO leaseryur (ID,TELEPHONE,CODEPRED,ADRESS,CHET,BANK,MFO,SVIDOT) VALUES ('" +
                                    maxid + "','" + leasernumbertextBox.Text + "','" + leaserkodpredtextBox.Text + "','" + leaseradresstextBox.Text.Replace("'", "''") + "','" + leaserschettextBox.Text + "','" +
                                    leaserbanktextBox.Text + "','" + leasermfotextBox.Text + "','" + leasersvidottextBox.Text + "')");
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
                            this.Close();
                        }
                    }
                    else
                    {
                        bdfunc.insert(@"insert into contracts (contract_number,date_contract,lessor,cadastral_number,square,term_lease,registration_date,contract_status," +
                            "unit_number,notes,type_contract,area_number,form_owner,form_use,form_unit,koatuu,leaser,contractortype) values ('" + numcontrtextBox.Text + "','" +
            contrdatePicker.SelectedDate.Value.ToString("dd.MM.yyyy") + "','" + lessortextBox.Text.Replace("'", "''") + "','" + cadnumtextBox.Text + "','" + sqtextBox.Text.Replace('.', ',') + "','" +
            termleasedatePicker.SelectedDate.Value.ToString("dd.MM.yyyy") + "','" + regdatePicker.SelectedDate.Value.ToString("dd.MM.yyyy") + "','" + statuscomboBox.SelectedIndex +
            "','" + numunittextBox.Text + "','" + notestextBox.Text + "','" + typecontrcomboBox.SelectedIndex + "','" + numareatextBox.Text + "','" + formownercomboBox.SelectedIndex +
            "','" + formusecomboBox.SelectedIndex + "','" + formunitcomboBox.SelectedIndex + "','" + koatuutextBox.Text + "','" + leasertextBox.Text.Replace("'", "''") + "','" +
            fizoryurcomboBox.SelectedIndex.ToString() + leaserfizoryurcomboBox.SelectedIndex.ToString() + "')");
                        ArrayList res = bdfunc.maxid();
                        int maxid = (int)res[0];
                        if (fizoryurcomboBox.SelectedIndex == 0)
                        {
                            bdfunc.insert("INSERT INTO lessorfiz (ID,PASSPORT_DATE,WHO_GIVE,PASSPORT_SERIES,ADRESS,TELEPHONE,INN) VALUES ('" + maxid + "','" + vidandatePicker.SelectedDate.Value.ToString("dd.MM.yyyy") +
                                "','" + kemvidantextBox.Text.Replace("'", "''") + "','" + passporttextBox.Text + "','" + adresstextBox.Text.Replace("'", "''") + "','" + numbertextBox.Text + "','" + inntextBox.Text + "')");
                        }
                        else if (fizoryurcomboBox.SelectedIndex == 1)
                        {
                            bdfunc.insert("INSERT INTO lessoryur (ID,TELEPHONE,CODEPRED,ADRESS,CHET,BANK,MFO,SVIDOT) VALUES ('" +
                                maxid + "','" + numbertextBox.Text + "','" + kodpredtextBox.Text + "','" + adresstextBox.Text.Replace("'", "''") + "','" + schettextBox.Text + "','" +
                                banktextBox.Text + "','" + mfotextBox.Text + "','" + svidottextBox.Text + "')");
                        }
                        if (leaserfizoryurcomboBox.SelectedIndex == 0)
                        {
                            bdfunc.insert("INSERT INTO leaserfiz (ID,PASSPORT_DATE,WHO_GIVE,PASSPORT_SERIES,ADRESS,TELEPHONE,INN) VALUES ('" + maxid + "','" + leaservidandatePicker.SelectedDate.Value.ToString("dd.MM.yyyy") +
                                "','" + leaserkemvidantextBox.Text.Replace("'", "''") + "','" + leaserpassporttextBox.Text + "','" + leaseradresstextBox.Text.Replace("'", "''") + "','" + leasernumbertextBox.Text + "','" + leaserinntextBox.Text + "')");
                        }
                        else if (leaserfizoryurcomboBox.SelectedIndex == 1)
                        {
                            bdfunc.insert("INSERT INTO leaseryur (ID,TELEPHONE,CODEPRED,ADRESS,CHET,BANK,MFO,SVIDOT) VALUES ('" +
                                maxid + "','" + leasernumbertextBox.Text + "','" + leaserkodpredtextBox.Text + "','" + leaseradresstextBox.Text.Replace("'", "''") + "','" + leaserschettextBox.Text + "','" +
                                leaserbanktextBox.Text + "','" + leasermfotextBox.Text + "','" + leasersvidottextBox.Text + "')");
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
                        this.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show(Application.Current.MainWindow, (string)FindResource("m_enableeditdbwarning"), (string)FindResource("m_warning"), MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Close();
            }
        }
        public bool importfrxml(string FileName)
        {
            myvariable.errcount = 0;
            string regdate = xml.importtext(FileName, "RegistrationDate", false);
            DateTime nulldate = Convert.ToDateTime("01/01/90");
            if (regdate != "" && Convert.ToDateTime(regdate) > Convert.ToDateTime("01/01/90"))
            {
                statuscomboBox.SelectedIndex = 0;
                regdatePicker.SelectedDate = Convert.ToDateTime(regdate);
                if (!DateTime.TryParse(xml.importtext(FileName, "LeaseDuration", false), out nulldate))
                {
                    myvariable.errcount++;
                    MessageBox.Show((string)Application.Current.Resources["m_badleasedate"]);
                }
                termleasedatePicker.SelectedDate = nulldate;
            }
            else
            {
                statuscomboBox.SelectedIndex = 1;
                regdatePicker.SelectedDate = nulldate;
                termleasedatePicker.SelectedDate = nulldate;
            }
            string typeGrantor = xml.importtext(FileName, "typeGrantor", false);
            if (typeGrantor != "")
            {
                if (typeGrantor == "NaturalPerson")
                {
                    fizoryurcomboBox.SelectedIndex = 0;
                    lessortextBox.Text = xml.importcontractorinf(FileName, "NaturalPerson_FullName", "Grantor", false);
                    inntextBox.Text = xml.importcontractorinf(FileName, "NaturalPerson_TaxNumber", "Grantor", false);
                    passporttextBox.Text = xml.importcontractorinf(FileName, "NaturalPerson_Passport", "Grantor", false);
                    if (!DateTime.TryParse(xml.importcontractorinf(FileName, "NaturalPerson_PassportIssuedDate", "Grantor", false), out nulldate))
                    {
                        MessageBox.Show("неверный формат PassportIssuedDate");
                    }
                    vidandatePicker.SelectedDate = nulldate;
                    kemvidantextBox.Text = xml.importcontractorinf(FileName, "NaturalPerson_IssuanceAuthority", "Grantor", false);
                    adresstextBox.Text = xml.importcontractorinf(FileName, "NaturalPerson_Address", "Grantor", false);
                }
                else if (typeGrantor == "LegalEntity")
                {
                    fizoryurcomboBox.SelectedIndex = 1;
                    lessortextBox.Text = xml.importcontractorinf(FileName, "LegalEntity_Name", "Grantor", false);
                    kodpredtextBox.Text = xml.importcontractorinf(FileName, "LegalEntity_EDRPOU", "Grantor", false);
                    adresstextBox.Text = xml.importcontractorinf(FileName, "LegalEntity_Address", "Grantor", false);
                }
            }
            string typeGrantee = xml.importtext(FileName, "typeGrantee", false);
            if (typeGrantee != "")
            {
                if (typeGrantee == "NaturalPerson")
                {
                    leaserfizoryurcomboBox.SelectedIndex = 0;
                    leasertextBox.Text = xml.importcontractorinf(FileName, "NaturalPerson_FullName", "Grantee", false);
                    leaserinntextBox.Text = xml.importcontractorinf(FileName, "NaturalPerson_TaxNumber", "Grantee", false);
                    leaserpassporttextBox.Text = xml.importcontractorinf(FileName, "NaturalPerson_Passport", "Grantee", false);
                    if (xml.importcontractorinf(FileName, "NaturalPerson_PassportIssuedDate", "Grantee", false) != "")
                    {
                        leaservidandatePicker.SelectedDate = Convert.ToDateTime(xml.importcontractorinf(FileName, "NaturalPerson_PassportIssuedDate", "Grantee", false));
                    }
                    else
                    {
                        leaservidandatePicker.SelectedDate = Convert.ToDateTime("01/01/53");
                    }
                    leaserkemvidantextBox.Text = xml.importcontractorinf(FileName, "NaturalPerson_IssuanceAuthority", "Grantee", false);
                    leaseradresstextBox.Text = xml.importcontractorinf(FileName, "NaturalPerson_Address", "Grantee", false);
                }
                else if (typeGrantee == "LegalEntity")
                {
                    leaserfizoryurcomboBox.SelectedIndex = 1;
                    leasertextBox.Text = xml.importcontractorinf(FileName, "LegalEntity_Name", "Grantee", false);
                    leaserkodpredtextBox.Text = xml.importcontractorinf(FileName, "LegalEntity_EDRPOU", "Grantee", false);
                    leaseradresstextBox.Text = xml.importcontractorinf(FileName, "LegalEntity_Address", "Grantee", false);
                }
            }
            cadnumtextBox.Text = xml.importtext(FileName, "CadastralCode", false);
            sqtextBox.Text = xml.importtext(FileName, "Size", false);
            koatuutextBox.Text = xml.importtext(FileName, "KOATUU", false);
            formownercomboBox.SelectedIndex = xml.importint(FileName, "OwnershipInfo", false);
            formusecomboBox.SelectedIndex = xml.importint(FileName, "CategoryPurposeInfo", false);
            formunitcomboBox.SelectedIndex = xml.importint(FileName, "LandCode", false);
            ck63.Clear();
            wgs84.Clear();
            ck63 = xml.importcoords(FileName, false);
            if (ck63 != null )
            {
                newcoords = true;
                ck63Entities.Reset(ck63);
                ck63View = (ListCollectionView)CollectionViewSource.GetDefaultView(ck63Entities);
                for (int i = 0; i < ck63.Count; i++)
                {
                    double[] x = { Convert.ToDouble(ck63[i].y.ToString().Replace(".", ",")) };
                    double[] y = { Convert.ToDouble(ck63[i].x.ToString().Replace(".", ",")) };
                    Projection src = new Projection(IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "CK63", "z" + x[0].ToString()[0]));
                    Projection dst = new Projection(@"+proj=longlat +ellps=WGS84 +datum=WGS84 +no_defs");
                    Projection.Transform(src, dst, x, y, null);
                    wgs84.Add(new coordsformat() { x = y[0].ToString().Replace(",", "."), y = x[0].ToString().Replace(",", "."), rank = ck63[i].rank });
                }
                wgs84Entities.Reset(wgs84);
                wgs84View = (ListCollectionView)CollectionViewSource.GetDefaultView(wgs84Entities);
            }
            if (myvariable.errcount > 0)
            { return false; }
            return true;
        }
        private void buttonImportXML_Click(object sender, RoutedEventArgs e)
        {
            if (farmer.Properties.Settings.Default.editdbenable)
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".xml";
                dlg.Filter = "XML (.xml)|*.xml";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    importfrxml(dlg.FileName);
                }
            }
            else
            {
                MessageBox.Show(Application.Current.MainWindow, (string)FindResource("m_enableeditdbwarning"), (string)FindResource("m_warning"), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        int currid = 0;
        public void loadcontract(int id)
        {
            currid = id;
            ArrayList contr = bdfunc.select(@"select * from contracts where id = " + id);
            numcontrtextBox.Text = contr[1].ToString();
            contrdatePicker.SelectedDate = Convert.ToDateTime(contr[2].ToString());
            lessortextBox.Text = contr[3].ToString();
            cadnumtextBox.Text = contr[4].ToString();
            sqtextBox.Text = contr[5].ToString();
            termleasedatePicker.SelectedDate = Convert.ToDateTime(contr[6].ToString());
            regdatePicker.SelectedDate = Convert.ToDateTime(contr[7].ToString());
            statuscomboBox.SelectedIndex = (int)contr[8];
            numunittextBox.Text = contr[9].ToString();
            notestextBox.Text = contr[10].ToString();
            typecontrcomboBox.SelectedIndex = (int)contr[11];
            numareatextBox.Text = contr[12].ToString();
            formownercomboBox.SelectedIndex = (int)contr[13];
            formusecomboBox.SelectedIndex = (int)contr[14];
            formunitcomboBox.SelectedIndex = (int)contr[15];
            koatuutextBox.Text = contr[16].ToString();
            leasertextBox.Text = contr[17].ToString();
            switch (contr[18].ToString())
            {
                case "00":
                    fizoryurcomboBox.SelectedIndex = Convert.ToInt32(contr[18].ToString().Substring(0, 1));
                    ArrayList fiz = bdfunc.select(@"select * from lessorfiz where id = " + id);
                    inntextBox.Text = fiz[1].ToString();
                    passporttextBox.Text = fiz[2].ToString();
                    adresstextBox.Text = fiz[3].ToString();
                    numbertextBox.Text = fiz[4].ToString();
                    vidandatePicker.SelectedDate = Convert.ToDateTime(fiz[5].ToString());
                    kemvidantextBox.Text = fiz[6].ToString();
                    leaserfizoryurcomboBox.SelectedIndex = Convert.ToInt32(contr[18].ToString().Substring(1, 1));
                    ArrayList leaserfiz = bdfunc.select(@"select * from leaserfiz where id = " + id);
                    leaserinntextBox.Text = leaserfiz[1].ToString();
                    leaserpassporttextBox.Text = leaserfiz[2].ToString();
                    leaseradresstextBox.Text = leaserfiz[3].ToString();
                    leasernumbertextBox.Text = leaserfiz[4].ToString();
                    leaservidandatePicker.SelectedDate = Convert.ToDateTime(leaserfiz[5].ToString());
                    leaserkemvidantextBox.Text = leaserfiz[6].ToString();
                    break;
                case "01":
                    fizoryurcomboBox.SelectedIndex = Convert.ToInt32(contr[18].ToString().Substring(0, 1));
                    fiz = bdfunc.select(@"select * from lessorfiz where id = " + id);
                    inntextBox.Text = fiz[1].ToString();
                    passporttextBox.Text = fiz[2].ToString();
                    adresstextBox.Text = fiz[3].ToString();
                    numbertextBox.Text = fiz[4].ToString();
                    vidandatePicker.SelectedDate = Convert.ToDateTime(fiz[5].ToString());
                    kemvidantextBox.Text = fiz[6].ToString();
                    leaserfizoryurcomboBox.SelectedIndex = Convert.ToInt32(contr[18].ToString().Substring(1, 1));
                    ArrayList leaseryur = bdfunc.select(@"select * from leaseryur where id = " + id);
                    leasernumbertextBox.Text = leaseryur[1].ToString();
                    leaserkodpredtextBox.Text = leaseryur[2].ToString();
                    leaseradresstextBox.Text = leaseryur[3].ToString();
                    leaserschettextBox.Text = leaseryur[4].ToString();
                    leaserbanktextBox.Text = leaseryur[5].ToString();
                    leasermfotextBox.Text = leaseryur[6].ToString();
                    leasersvidottextBox.Text = leaseryur[7].ToString();
                    break;
                case "10":
                    fizoryurcomboBox.SelectedIndex = Convert.ToInt32(contr[18].ToString().Substring(0, 1));
                    ArrayList yur = bdfunc.select(@"select * from lessoryur where id = " + id);
                    numbertextBox.Text = yur[1].ToString();
                    kodpredtextBox.Text = yur[2].ToString();
                    adresstextBox.Text = yur[3].ToString();
                    schettextBox.Text = yur[4].ToString();
                    banktextBox.Text = yur[5].ToString();
                    mfotextBox.Text = yur[6].ToString();
                    svidottextBox.Text = yur[7].ToString();
                    leaserfizoryurcomboBox.SelectedIndex = Convert.ToInt32(contr[18].ToString().Substring(1, 1));
                    leaserfiz = bdfunc.select(@"select * from leaserfiz where id = " + id);
                    leaserinntextBox.Text = leaserfiz[1].ToString();
                    leaserpassporttextBox.Text = leaserfiz[2].ToString();
                    leaseradresstextBox.Text = leaserfiz[3].ToString();
                    leasernumbertextBox.Text = leaserfiz[4].ToString();
                    leaservidandatePicker.SelectedDate = Convert.ToDateTime(leaserfiz[5].ToString());
                    leaserkemvidantextBox.Text = leaserfiz[6].ToString();
                    break;
                case "11":
                    fizoryurcomboBox.SelectedIndex = Convert.ToInt32(contr[18].ToString().Substring(0, 1));
                    yur = bdfunc.select(@"select * from lessoryur where id = " + id);
                    numbertextBox.Text = yur[1].ToString();
                    kodpredtextBox.Text = yur[2].ToString();
                    adresstextBox.Text = yur[3].ToString();
                    schettextBox.Text = yur[4].ToString();
                    banktextBox.Text = yur[5].ToString();
                    mfotextBox.Text = yur[6].ToString();
                    svidottextBox.Text = yur[7].ToString();
                    leaserfizoryurcomboBox.SelectedIndex = Convert.ToInt32(contr[18].ToString().Substring(1, 1));
                    leaseryur = bdfunc.select(@"select * from leaseryur where id = " + id);
                    leasernumbertextBox.Text = leaseryur[1].ToString();
                    leaserkodpredtextBox.Text = leaseryur[2].ToString();
                    leaseradresstextBox.Text = leaseryur[3].ToString();
                    leaserschettextBox.Text = leaseryur[4].ToString();
                    leaserbanktextBox.Text = leaseryur[5].ToString();
                    leasermfotextBox.Text = leaseryur[6].ToString();
                    leasersvidottextBox.Text = leaseryur[7].ToString();
                    break;
            }
            ck63 = bdfunc.selcoords(new List<string>() { id.ToString() },"ck63");
            ck63Entities.Reset(ck63);
            ck63View = (ListCollectionView)CollectionViewSource.GetDefaultView(ck63Entities);

            wgs84 = bdfunc.selcoords(new List<string>() { id.ToString() }, "wgs84");
            wgs84Entities.Reset(wgs84);
            wgs84View = (ListCollectionView)CollectionViewSource.GetDefaultView(wgs84Entities);
        }

        private void statuscomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (statuscomboBox.SelectedIndex == 0)
            {
                regdatePicker.Visibility = Visibility.Visible;
                regtextBlock.Visibility = Visibility.Visible;
            }
            else if (statuscomboBox.SelectedIndex == 1)
            {
                regdatePicker.Visibility = Visibility.Hidden;
                regtextBlock.Visibility = Visibility.Hidden;
            }
        }

        private void fizoryurcomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fizoryurcomboBox.SelectedIndex == 0)
            {
                yurgrid.Visibility = Visibility.Hidden;
                fizgrid.Visibility = Visibility.Visible;
            }
            else if (fizoryurcomboBox.SelectedIndex == 1)
            {
                fizgrid.Visibility = Visibility.Hidden;
                yurgrid.Visibility = Visibility.Visible;
            }
        }
        private void passporttextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (passporttextBox.Text.Length != 0)
            {
                bool alldig = true;
                for (int i = 0; i < 2; i++)
                {
                    if (i < passporttextBox.Text.Length)
                    {
                        if (Char.IsUpper(passporttextBox.Text[i]) && Char.IsLetter(passporttextBox.Text[i]))
                        {
                            alldig = false;
                            break;
                        }
                    }
                    else { break; }
                }
                if (!alldig)
                {
                    if (passporttextBox.Text.Length > 2)
                    {
                        alldig = true;
                        for (int i = 2; i < passporttextBox.Text.Length; i++)
                        {
                            if (!Char.IsDigit(passporttextBox.Text[i]))
                            {
                                alldig = false;
                                break;
                            }
                        }
                        if (!alldig)
                        {
                            Dispatcher.BeginInvoke(new Action(() => passporttextBox.Undo()));
                        }
                    }
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() => passporttextBox.Undo()));
                }
            }
        }

        private void cadnumtextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cadnumtextBox.Text.Length != 0)
            {
                bool alldig = false;
                for (int i = 0; i < cadnumtextBox.Text.Length; i++)
                {
                    if (Char.IsDigit(cadnumtextBox.Text[i]) || cadnumtextBox.Text[i] == ':')
                    {
                        alldig = true;
                    }
                    else
                    {
                        alldig = false;
                        break;
                    }
                }
                if (alldig)
                {
                    if (e.Changes.ElementAt(0).AddedLength > 0)
                    {
                        switch (cadnumtextBox.Text.Length)
                        {
                            case 10:
                                cadnumtextBox.Text = cadnumtextBox.Text + ':';
                                cadnumtextBox.SelectionStart = cadnumtextBox.Text.Length;
                                cadnumtextBox.SelectionLength = 0;
                                break;
                            case 13:
                                cadnumtextBox.Text = cadnumtextBox.Text + ':';
                                cadnumtextBox.SelectionStart = cadnumtextBox.Text.Length;
                                cadnumtextBox.SelectionLength = 0;
                                break;
                            case 17:
                                cadnumtextBox.Text = cadnumtextBox.Text + ':';
                                cadnumtextBox.SelectionStart = cadnumtextBox.Text.Length;
                                cadnumtextBox.SelectionLength = 0;
                                break;
                        }
                    }
                    else
                    {
                        switch (cadnumtextBox.Text.Length)
                        {
                            case 11:
                                cadnumtextBox.Text = cadnumtextBox.Text.Remove(cadnumtextBox.Text.Length - 1, 1);
                                break;
                            case 14:
                                cadnumtextBox.Text = cadnumtextBox.Text.Remove(cadnumtextBox.Text.Length - 1, 1);
                                break;
                            case 18:
                                cadnumtextBox.Text = cadnumtextBox.Text.Remove(cadnumtextBox.Text.Length - 1, 1);
                                break;
                        }
                    }
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() => cadnumtextBox.Undo()));
                }
            }
        }

        private void leaserfizoryurcomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (leaserfizoryurcomboBox.SelectedIndex == 0)
            {
                leaseryurgrid.Visibility = Visibility.Hidden;
                leaserfizgrid.Visibility = Visibility.Visible;
            }
            else if (leaserfizoryurcomboBox.SelectedIndex == 1)
            {
                leaserfizgrid.Visibility = Visibility.Hidden;
                leaseryurgrid.Visibility = Visibility.Visible;
            }
        }

        private void leaserpassporttextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (leaserpassporttextBox.Text.Length != 0)
            {
                bool alldig = true;
                for (int i = 0; i < 2; i++)
                {
                    if (i < leaserpassporttextBox.Text.Length)
                    {
                        if (Char.IsUpper(leaserpassporttextBox.Text[i]) && Char.IsLetter(leaserpassporttextBox.Text[i]))
                        {
                            alldig = false;
                            break;
                        }
                    }
                    else { break; }
                }
                if (!alldig)
                {
                    if (leaserpassporttextBox.Text.Length > 2)
                    {
                        alldig = true;
                        for (int i = 2; i < leaserpassporttextBox.Text.Length; i++)
                        {
                            if (!Char.IsDigit(leaserpassporttextBox.Text[i]))
                            {
                                alldig = false;
                                break;
                            }
                        }
                        if (!alldig)
                        {
                            Dispatcher.BeginInvoke(new Action(() => leaserpassporttextBox.Undo()));
                        }
                    }
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() => leaserpassporttextBox.Undo()));
                }
            }
        }
    }
}
