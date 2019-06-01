using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ProjApi;
using Awesomium.Core;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Specialized;
using System.Xml;
using MigraDoc.Rendering;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel;
using System.Diagnostics;
using System.Net.Sockets;
using Microsoft.Win32;
using System.Globalization;
using System.Drawing;
using System.Drawing.Imaging;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp;
using System.Windows.Threading;
using System.Xml.Linq;
using System.Xml.XPath;

namespace farmer
{

    public partial class MainWindow : Fluent.RibbonWindow
    {
        //public class GroupFilter
        //{
        //    private List<Predicate<object>> _filters;

        //    public Predicate<object> Filter { get; private set; }

        //    public GroupFilter()
        //    {
        //        _filters = new List<Predicate<object>>();
        //        Filter = InternalFilter;
        //    }

        //    private bool InternalFilter(object o)
        //    {
        //        foreach (var filter in _filters)
        //        {
        //            if (!filter(o))
        //            {
        //                return false;
        //            }
        //        }

        //        return true;
        //    }

        //    public void AddFilter(Predicate<object> filter)
        //    {
        //        foreach (var item in _filters)
        //        {
        //            if (item.Method.Name.Substring(0, 10) == filter.Method.Name.Substring(0, 10))
        //            {
        //                _filters.Remove(item);
        //                break;
        //            }
        //        }
        //        _filters.Add(filter);
        //    }

        //    public void RemoveFilter(Predicate<object> filter)
        //    {
        //        foreach (var item in _filters)
        //        {
        //            if (item.Method.Name.Substring(0, 10) == filter.Method.Name.Substring(0, 10))
        //            {
        //                _filters.Remove(item);
        //                break;
        //            }
        //        }
        //    }
        //}
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
        public SmartCollection<contract> contractsEntities { get; set; }
        public List<contract> contractsList { get; set; }
        public ListCollectionView contractsView { get; set; }
        public bool manualref = false;
        loadingwin newloadwin;
        List<coordsformat> allcoords;
        Task testservThread;
        WebView webView1;
        public MainWindow()
        {
            InitializeComponent();
            newloadwin = new loadingwin();
            newloadwin.Show();
            pass.nowchangepass = false;
            if (farmer.Properties.Settings.Default.passen)
            {
                passwindow objModal = new passwindow();
                objModal.ShowDialog();
            }
            langcomboBox.Items.Clear();
            foreach (var lang in App.Languages)
            {
                langcomboBox.Items.Add(new List<KeyValuePair<CultureInfo, string>> { new KeyValuePair<CultureInfo, string>(lang, lang.DisplayName) });
            }

            langcomboBox.SelectionChanged -= langcomboBox_SelectionChanged;
            langcomboBox.SelectedValue = farmer.Properties.Settings.Default.DefaultLanguage;
            langcomboBox.SelectionChanged += langcomboBox_SelectionChanged;

            readsettings();
            contrtabfil.Group = filGroup;
            maptabfil.Group = filGroup;
            contrtabfil.Visibility = Visibility.Collapsed;
            maptabfil.Visibility = Visibility.Collapsed;
            tabrep.Group = repGroup;
            this.Opacity = 0;

            contractsList = new List<contract>();
            contractsEntities = new SmartCollection<contract>();
            contractsEntities.CollectionChanged += this.OnCollectionChanged;
            contractsEntities.Reset(contractsList);
            contractsView = (ListCollectionView)CollectionViewSource.GetDefaultView(contractsEntities);
            listView.ItemsSource = contractsView;

            oblcomboBox.ItemsSource = cccombo.oblcomboBoxitems;
            oblcomboBox.DisplayMemberPath = "Value";
            oblcomboBox.SelectedValuePath = "Key";
            oblcomboBox.SelectedIndex = 0;
            mapoblcomboBox.ItemsSource = cccombo.oblcomboBoxitems;
            mapoblcomboBox.DisplayMemberPath = "Value";
            mapoblcomboBox.SelectedValuePath = "Key";
            mapoblcomboBox.SelectedIndex = 0;
            typecomboBox.ItemsSource = cccombo.typecomboBoxitems;
            typecomboBox.SelectedIndex = 0;
            statuscomboBox.ItemsSource = cccombo.statuscomboBoxitems;
            statuscomboBox.SelectedIndex = 0;
            formownercomboBox.ItemsSource = cccombo.formownercomboBoxitems;
            formownercomboBox.SelectedIndex = 0;
            formunitcomboBox.ItemsSource = cccombo.formunitcomboBoxitems;
            formunitcomboBox.SelectedIndex = 0;
            formusecomboBox.ItemsSource = cccombo.formusecomboBoxitems;
            formusecomboBox.SelectedIndex = 0;
            maptypecomboBox.ItemsSource = cccombo.typecomboBoxitems;
            maptypecomboBox.SelectedIndex = 0;
            mapstatuscomboBox.ItemsSource = cccombo.statuscomboBoxitems;
            mapstatuscomboBox.SelectedIndex = 0;
            mapformownercomboBox.ItemsSource = cccombo.formownercomboBoxitems;
            mapformownercomboBox.SelectedIndex = 0;
            mapformunitcomboBox.ItemsSource = cccombo.formunitcomboBoxitems;
            mapformunitcomboBox.SelectedIndex = 0;
            mapformusecomboBox.ItemsSource = cccombo.formusecomboBoxitems;
            mapformusecomboBox.SelectedIndex = 0;
            webView1 = WebCore.CreateWebView(4494, 3179, WebCore.CreateWebSession(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory), new WebPreferences()
            {
                WebGL = true,
                FileAccessFromFileURL = true,
                UniversalAccessFromFileURL = true,
                WebAudio = false,
                WebSecurity = false
            }));
            webView1.Source = new Uri("file:///" + System.AppDomain.CurrentDomain.BaseDirectory + "/index.html");
            webView1.LoadingFrameComplete += webView1_LoadingFrameComplete;
            webControl1.WebSession = WebCore.CreateWebSession(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory), new WebPreferences()
            {
                WebGL = true,
                FileAccessFromFileURL = true,
                UniversalAccessFromFileURL = true,
                WebAudio = false,
                WebSecurity = false
            });
            webControl1.Source = new Uri("file:///" + System.AppDomain.CurrentDomain.BaseDirectory + "/index.html");

            testservThread = new Task(rinshfirsttime);
            testservThread.Start();
        }
        private void readsettings()
        {
            enablepasscheckBox.IsChecked = farmer.Properties.Settings.Default.passen;

            serveriptextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server");
            serverporttextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port");
            dbtextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "database");
            usertextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user");
            passtextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "pass");

            repcontrnumtextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repcontrnum");
            repdatecontrtextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repdatecontr");
            repdateregtextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repdatereg");
            replessortextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "replessor");
            repleasertextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repleaser");
            repcadnumtextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repcadnum");
            repsquaretextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repsquare");
            reptermleasetextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "reptermlease");
            repcontrstattextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repcontrstat");
            reptypecontrtextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "reptypecontr");
            repformownertextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repformowner");
            repformusetextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repformuse");
            repformunittextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repformunit");
            repkoatuutextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repkoatuu");
            repareanumtextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repareanum");
            repunitnumtextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repunitnum");
            repnotestextBox.Text = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repnotes");
        }
        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            string cumsum = 0.ToString();
            string cumsumfil = 0.ToString();
            SmartCollection<contract> curr = sender as SmartCollection<contract>;
            if (curr != null)
            {
                foreach (contract contr in contractsList)
                {
                    cumsum = (Convert.ToDouble(cumsum) + Convert.ToDouble(contr.square)).ToString();
                }
                for (int i = 0; i < curr.Count; i++)
                {
                    cumsumfil = (Convert.ToDouble(cumsumfil) + Convert.ToDouble(curr[i].square)).ToString();
                }
                if (cumsumfil == cumsum)
                {
                    contrlbl.Text = (string)FindResource("m_sumsq") + cumsum;
                }
                else
                {
                    contrlbl.Text = (string)FindResource("m_sumsq") + cumsum + (string)FindResource("m_sumsqfil") + cumsumfil;
                }
            }
        }
        private void refreshwin()
        {
            if (farmer.Properties.Settings.Default.editdbenable)
            {
                contractsList = bdfunc.selcontr();
                contractsEntities.Reset(contractsList);
                contractsView = (ListCollectionView)CollectionViewSource.GetDefaultView(contractsEntities);

                if (leasercomboBox.Items.Count > 0 && mapleasercomboBox.Items.Count > 0)
                {
                    string selleaser = leasercomboBox.SelectedItem.ToString();
                    string selmapleaser = mapleasercomboBox.SelectedItem.ToString();
                    foreach (contract contr in contractsList)
                    {
                        if (!leasercomboBox.Items.Contains(contr.leaser.ToLower()))
                        {
                            leasercomboBox.Items.Add(contr.leaser.ToLower());
                            mapleasercomboBox.Items.Add(contr.leaser.ToLower());
                        }
                    }
                    int idselcb = leasercomboBox.Items.IndexOf(selleaser);
                    int idselmapcb = mapleasercomboBox.Items.IndexOf(selmapleaser);
                    if (idselcb != -1)
                    {
                        leasercomboBox.SelectedIndex = idselcb;
                    }
                    else
                    {
                        leasercomboBox.SelectedIndex = 0;
                    }
                    if (idselmapcb != -1)
                    {
                        mapleasercomboBox.SelectedIndex = idselmapcb;
                    }
                    else
                    {
                        mapleasercomboBox.SelectedIndex = 0;
                    }
                }
                else
                {
                    foreach (contract contr in contractsList)
                    {
                        if (!leasercomboBox.Items.Contains(contr.leaser.ToLower()))
                        {
                            leasercomboBox.Items.Add(contr.leaser.ToLower());
                            mapleasercomboBox.Items.Add(contr.leaser.ToLower());
                        }
                    }
                    leasercomboBox.SelectedIndex = 0;
                    mapleasercomboBox.SelectedIndex = 0;
                }
                filtercontr();
                if (contractsList.Count > 0)
                {
                    List<string> listid = new List<string>();
                    foreach (contract contr in contractsList)
                    {
                        listid.Add(contr.id.ToString());
                    }
                    allcoords = bdfunc.selcoords(listid, "wgs84");
                }
                webControl1.ExecuteJavascript("savemapstate()");
                manualref = true;
                webView1.Reload(true);
                webControl1.Reload(true);
            }
        }
        private void Ribbon_SelectedTabChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ribbon.SelectedTabIndex)
            {
                case 0:
                    grid1.Visibility = Visibility.Visible;
                    grid2.Visibility = Visibility.Collapsed;
                    grid3.Visibility = Visibility.Collapsed;
                    grid4.Visibility = Visibility.Collapsed;
                    if (!(bool)mapmoreparam.IsChecked)
                    {
                        maptabfil.Visibility = Visibility.Collapsed;
                    }
                    if (!(bool)contrmoreparam.IsChecked)
                    {
                        contrtabfil.Visibility = Visibility.Collapsed;
                    }
                    if (!(bool)contrmoreparam.IsChecked && !(bool)mapmoreparam.IsChecked)
                    {
                        filGroup.Visibility = Visibility.Collapsed;
                    }
                    repGroup.Visibility = Visibility.Collapsed;
                    tabrep.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    grid1.Visibility = Visibility.Collapsed;
                    grid2.Visibility = Visibility.Visible;
                    grid3.Visibility = Visibility.Collapsed;
                    grid4.Visibility = Visibility.Collapsed;
                    repGroup.Visibility = Visibility.Collapsed;
                    tabrep.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    grid1.Visibility = Visibility.Collapsed;
                    grid2.Visibility = Visibility.Collapsed;
                    grid3.Visibility = Visibility.Visible;
                    grid4.Visibility = Visibility.Collapsed;
                    if (!(bool)mapmoreparam.IsChecked)
                    {
                        maptabfil.Visibility = Visibility.Collapsed;
                    }
                    if (!(bool)contrmoreparam.IsChecked)
                    {
                        contrtabfil.Visibility = Visibility.Collapsed;
                    }
                    if (!(bool)contrmoreparam.IsChecked && !(bool)mapmoreparam.IsChecked)
                    {
                        filGroup.Visibility = Visibility.Collapsed;
                    }
                    repGroup.Visibility = Visibility.Collapsed;
                    tabrep.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    filGroup.Visibility = Visibility.Visible;
                    contrtabfil.Visibility = Visibility.Visible;
                    maptabfil.Visibility = Visibility.Visible;
                    if (!(bool)mapmoreparam.IsChecked)
                    {
                        maptabfil.Visibility = Visibility.Collapsed;
                    }
                    break;
                case 4:
                    filGroup.Visibility = Visibility.Visible;
                    maptabfil.Visibility = Visibility.Visible;
                    contrtabfil.Visibility = Visibility.Visible;
                    if (!(bool)contrmoreparam.IsChecked)
                    {
                        contrtabfil.Visibility = Visibility.Collapsed;
                    }
                    break;
                case 5:
                    grid1.Visibility = Visibility.Collapsed;
                    grid2.Visibility = Visibility.Collapsed;
                    grid3.Visibility = Visibility.Collapsed;
                    grid4.Visibility = Visibility.Visible;
                    repGroup.Visibility = Visibility.Visible;
                    tabrep.Visibility = Visibility.Visible;
                    if (!(bool)mapmoreparam.IsChecked)
                    {
                        maptabfil.Visibility = Visibility.Collapsed;
                    }
                    if (!(bool)contrmoreparam.IsChecked)
                    {
                        contrtabfil.Visibility = Visibility.Collapsed;
                    }
                    if (!(bool)contrmoreparam.IsChecked && !(bool)mapmoreparam.IsChecked)
                    {
                        filGroup.Visibility = Visibility.Collapsed;
                    }
                    break;
            }
        }

        MigraDoc.DocumentObjectModel.Tables.Table outtable;
        Column column;
        Row row;
        Cell cell;
        Document document;
        private void refshtable()
        {
            if (this.IsInitialized)
            {
                document = new Document();
                document.DefaultPageSetup.FooterDistance = "0.5cm";
                MigraDoc.DocumentObjectModel.Style style = document.Styles["Normal"];
                style.Font.Name = "Times New Roman";
                style.Font.Size = 10;
                MigraDoc.DocumentObjectModel.Section page = document.AddSection();
                page.PageSetup.OddAndEvenPagesHeaderFooter = true;
                page.PageSetup.StartingNumber = 1;

                MigraDoc.DocumentObjectModel.Paragraph pagenum = new MigraDoc.DocumentObjectModel.Paragraph();
                pagenum.Format.Alignment = ParagraphAlignment.Right;
                pagenum.AddPageField();
                page.Footers.Primary.Add(pagenum);
                page.Footers.EvenPage.Add(pagenum.Clone());

                page.PageSetup.BottomMargin = "6mm";
                page.PageSetup.LeftMargin = "6mm";
                page.PageSetup.RightMargin = "6mm";
                page.PageSetup.TopMargin = "6mm";
                page.PageSetup.Orientation = MigraDoc.DocumentObjectModel.Orientation.Landscape;
                MigraDoc.DocumentObjectModel.Paragraph paragraph = page.AddParagraph((string)Application.Current.Resources["m_repfil"]);
                paragraph.Style = "Normal";
                paragraph = page.AddParagraph("");
                this.outtable = page.AddTable();
                this.outtable.Style = "Normal";
                int tr = 0;
                if ((bool)repcontrnumcheckBox.IsChecked)
                {
                    tr++;
                    column = this.outtable.AddColumn(repcontrnumtextBox.Text + "mm");
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Comment = (string)FindResource("m_contrnum");
                }
                if ((bool)repdatecontrcheckBox.IsChecked)
                {
                    tr++;
                    column = this.outtable.AddColumn(repdatecontrtextBox.Text + "mm");
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Comment = (string)FindResource("m_date_contract");
                }
                if ((bool)repdateregcheckBox.IsChecked)
                {
                    tr++;
                    column = this.outtable.AddColumn(repdateregtextBox.Text + "mm");
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Comment = (string)FindResource("m_registration_date");
                }
                if ((bool)replessorcheckBox.IsChecked)
                {
                    tr++;
                    column = this.outtable.AddColumn(replessortextBox.Text + "mm");
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Comment = (string)FindResource("m_lessor");
                }
                if ((bool)repleasercheckBox.IsChecked)
                {
                    tr++;
                    column = this.outtable.AddColumn(repleasertextBox.Text + "mm");
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Comment = (string)FindResource("m_leaser");
                }
                if ((bool)repcadnumcheckBox.IsChecked)
                {
                    tr++;
                    column = this.outtable.AddColumn(repcadnumtextBox.Text + "mm");
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Comment = (string)FindResource("m_cadnum");
                }
                if ((bool)repsquarecheckBox.IsChecked)
                {
                    tr++;
                    column = this.outtable.AddColumn(repsquaretextBox.Text + "mm");
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Comment = (string)FindResource("m_square");
                }
                if ((bool)reptermleasecheckBox.IsChecked)
                {
                    tr++;
                    column = this.outtable.AddColumn(reptermleasetextBox.Text + "mm");
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Comment = (string)FindResource("m_term_lease");
                }
                if ((bool)repcontrstatcheckBox.IsChecked)
                {
                    tr++;
                    column = this.outtable.AddColumn(repcontrstattextBox.Text + "mm");
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Comment = (string)FindResource("m_contract_status");
                }
                if ((bool)reptypecontrcheckBox.IsChecked)
                {
                    tr++;
                    column = this.outtable.AddColumn(reptypecontrtextBox.Text + "mm");
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Comment = (string)FindResource("m_type_contract");
                }
                if ((bool)repformownercheckBox.IsChecked)
                {
                    tr++;
                    column = this.outtable.AddColumn(repformownertextBox.Text + "mm");
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Comment = (string)FindResource("m_form_owner");
                }
                if ((bool)repformusecheckBox.IsChecked)
                {
                    tr++;
                    column = this.outtable.AddColumn(repformusetextBox.Text + "mm");
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Comment = (string)FindResource("m_form_use");
                }
                if ((bool)repformunitcheckBox.IsChecked)
                {
                    tr++;
                    column = this.outtable.AddColumn(repformunittextBox.Text + "mm");
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Comment = (string)FindResource("m_form_unit");
                }
                if ((bool)repkoatuucheckBox.IsChecked)
                {
                    tr++;
                    column = this.outtable.AddColumn(repkoatuutextBox.Text + "mm");
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Comment = (string)FindResource("m_koatuu");
                }
                if ((bool)repareanumcheckBox.IsChecked)
                {
                    tr++;
                    column = this.outtable.AddColumn(repareanumtextBox.Text + "mm");
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Comment = (string)FindResource("m_area_number");
                }
                if ((bool)repunitnumcheckBox.IsChecked)
                {
                    tr++;
                    column = this.outtable.AddColumn(repunitnumtextBox.Text + "mm");
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Comment = (string)FindResource("m_unit_number");
                }
                if ((bool)repnotescheckBox.IsChecked)
                {
                    tr++;
                    column = this.outtable.AddColumn(repnotestextBox.Text + "mm");
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Comment = (string)FindResource("m_notes");
                }
                if (tr > 0)
                {
                    row = this.outtable.AddRow();
                    row.Format.Alignment = ParagraphAlignment.Center;
                    for (int j = 0; j < outtable.Columns.Count; j++)
                    {
                        cell = row.Cells[j];
                        cell.AddParagraph(outtable.Columns[j].Comment);
                    }

                    for (int i = 0; i < contractsEntities.Count; i++)
                    {
                        row = this.outtable.AddRow();
                        int j = 0;
                        if ((bool)repcontrnumcheckBox.IsChecked)
                        {
                            cell = row.Cells[j];
                            cell.AddParagraph(contractsEntities[i].contract_number);
                            j++;
                        }
                        if ((bool)repdatecontrcheckBox.IsChecked)
                        {
                            cell = row.Cells[j];
                            cell.AddParagraph(contractsEntities[i].date_contract);
                            j++;
                        }
                        if ((bool)repdateregcheckBox.IsChecked)
                        {
                            cell = row.Cells[j];
                            cell.AddParagraph(contractsEntities[i].registration_date);
                            j++;
                        }
                        if ((bool)replessorcheckBox.IsChecked)
                        {
                            cell = row.Cells[j];
                            cell.AddParagraph(contractsEntities[i].lessor);
                            j++;
                        }
                        if ((bool)repleasercheckBox.IsChecked)
                        {
                            cell = row.Cells[j];
                            cell.AddParagraph(contractsEntities[i].leaser);
                            j++;
                        }
                        if ((bool)repcadnumcheckBox.IsChecked)
                        {
                            cell = row.Cells[j];
                            cell.AddParagraph(contractsEntities[i].cadastral_number);
                            j++;
                        }
                        if ((bool)repsquarecheckBox.IsChecked)
                        {
                            cell = row.Cells[j];
                            cell.AddParagraph(contractsEntities[i].square);
                            j++;
                        }
                        if ((bool)reptermleasecheckBox.IsChecked)
                        {
                            cell = row.Cells[j];
                            cell.AddParagraph(contractsEntities[i].term_lease);
                            j++;
                        }
                        if ((bool)repcontrstatcheckBox.IsChecked)
                        {
                            cell = row.Cells[j];
                            switch (contractsEntities[i].contract_status)
                            {
                                case "0":
                                    cell.AddParagraph((string)FindResource("m_registered"));
                                    break;
                                case "1":
                                    cell.AddParagraph((string)FindResource("m_noregistered"));
                                    break;
                            }
                            j++;
                        }
                        if ((bool)reptypecontrcheckBox.IsChecked)
                        {
                            cell = row.Cells[j];
                            switch (contractsEntities[i].type_contract)
                            {
                                case "0":
                                    cell.AddParagraph((string)FindResource("m_contrlease"));
                                    break;
                                case "1":
                                    cell.AddParagraph((string)FindResource("m_contrsublease"));
                                    break;
                                case "2":
                                    cell.AddParagraph((string)FindResource("m_contrjoint"));
                                    break;
                                case "3":
                                    cell.AddParagraph((string)FindResource("m_contrcommun"));
                                    break;
                            }
                            j++;
                        }
                        if ((bool)repformownercheckBox.IsChecked)
                        {
                            cell = row.Cells[j];
                            switch (contractsEntities[i].form_owner)
                            {
                                case "0":
                                    cell.AddParagraph((string)FindResource("m_privt"));
                                    break;
                                case "1":
                                    cell.AddParagraph((string)FindResource("m_gov"));
                                    break;
                                case "2":
                                    cell.AddParagraph((string)FindResource("m_commun"));
                                    break;
                            }
                            j++;
                        }
                        if ((bool)repformusecheckBox.IsChecked)
                        {
                            cell = row.Cells[j];
                            switch (contractsEntities[i].form_use)
                            {
                                case "0":
                                    cell.AddParagraph((string)FindResource("m_commagr"));
                                    break;
                                case "1":
                                    cell.AddParagraph((string)FindResource("m_farm"));
                                    break;
                                case "2":
                                    cell.AddParagraph((string)FindResource("m_privtfarm"));
                                    break;
                                case "3":
                                    cell.AddParagraph((string)FindResource("m_industrial"));
                                    break;
                            }
                            j++;
                        }
                        if ((bool)repformunitcheckBox.IsChecked)
                        {
                            cell = row.Cells[j];
                            switch (contractsEntities[i].form_unit)
                            {
                                case "0":
                                    cell.AddParagraph((string)FindResource("m_arable"));
                                    break;
                                case "1":
                                    cell.AddParagraph((string)FindResource("m_haymaking"));
                                    break;
                                case "2":
                                    cell.AddParagraph((string)FindResource("m_pasture"));
                                    break;
                                case "3":
                                    cell.AddParagraph((string)FindResource("m_perennials"));
                                    break;
                                case "4":
                                    cell.AddParagraph((string)FindResource("m_farmstead"));
                                    break;
                                case "5":
                                    cell.AddParagraph((string)FindResource("m_industrial"));
                                    break;
                            }
                            j++;
                        }
                        if ((bool)repkoatuucheckBox.IsChecked)
                        {
                            cell = row.Cells[j];
                            cell.AddParagraph(contractsEntities[i].koatuu);
                            j++;
                        }
                        if ((bool)repareanumcheckBox.IsChecked)
                        {
                            cell = row.Cells[j];
                            cell.AddParagraph(contractsEntities[i].area_number);
                            j++;
                        }
                        if ((bool)repunitnumcheckBox.IsChecked)
                        {
                            cell = row.Cells[j];
                            cell.AddParagraph(contractsEntities[i].unit_number);
                            j++;
                        }
                        if ((bool)repnotescheckBox.IsChecked)
                        {
                            cell = row.Cells[j];
                            cell.AddParagraph(contractsEntities[i].notes);
                            j++;
                        }
                    }
                    outtable.SetEdge(0, 0, outtable.Columns.Count, outtable.Rows.Count, Edge.Interior, MigraDoc.DocumentObjectModel.BorderStyle.Single, 0.5, MigraDoc.DocumentObjectModel.Colors.Black);
                    outtable.SetEdge(0, 0, outtable.Columns.Count, outtable.Rows.Count, Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.Single, 0.5, MigraDoc.DocumentObjectModel.Colors.Black);

                    paragraph = page.AddParagraph("");
                    paragraph.Style = "Normal";
                    paragraph = page.AddParagraph(contrlbl.Text);
                    this.pagePreview.Ddl = MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToString(document);
                }
                else
                {
                    this.pagePreview.Ddl = null;
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        private void gomapMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItem != null)
            {
                contract row = (contract)listView.SelectedItem;
                mapcadnumcheckBox.IsChecked = true;
                mapcadnumtextBox.Text = row.cadastral_number;
                ribbon.SelectedTabIndex = 2;
            }
        }
        private void ApplyEffect(Window win)
        {
            Dispatcher.BeginInvoke(new Action(() => {
                System.Windows.Media.Effects.BlurEffect objBlur = new System.Windows.Media.Effects.BlurEffect();
                objBlur.Radius = 4;
                overlayrect.Visibility = Visibility.Visible;
                overlayprogressring.Visibility = Visibility.Visible;
                win.Effect = objBlur;
            }));
        }
        private void ClearEffect(Window win)
        {
            Dispatcher.BeginInvoke(new Action(() => {
                win.Effect = null;
                overlayrect.Visibility = Visibility.Collapsed;
                overlayprogressring.Visibility = Visibility.Collapsed;
            }));
        }
        private void delcontrMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (farmer.Properties.Settings.Default.editdbenable)
            {
                if (listView.SelectedItem != null)
                {
                    ApplyEffect(this);
                    MessageBoxResult messresult = new MessageBoxResult();
                    messresult = MessageBox.Show(Application.Current.MainWindow, (string)FindResource("m_didyoudelcont"), (string)FindResource("m_confirm"), MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (messresult == MessageBoxResult.Yes)
                    {
                        contract row = (contract)listView.SelectedItem;
                        bdfunc.delete("DELETE FROM contracts WHERE id = '" + row.id.ToString() + "'");
                        refreshwin();
                    }
                    ClearEffect(this);
                }
            }
            else
            {
                ApplyEffect(this);
                MessageBox.Show(Application.Current.MainWindow, (string)FindResource("m_enableeditdbwarning"), (string)FindResource("m_warning"), MessageBoxButton.OK, MessageBoxImage.Warning);
                ClearEffect(this);
            }
        }
        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            newloadwin.Close();
            this.Opacity = 100;
        }
        private JSValue update(JSValue[] result)
        {
            if (!result[0].IsNull | result[0].IsArray)
            {
                bdfunc.update("UPDATE contracts SET AREA_NUMBER='" + result[0][1].ToString() + "' WHERE ID='" + result[0][0].ToString() + "'");
                contractsList = bdfunc.selcontr();
                contractsEntities.Reset(contractsList);
                contractsView = (ListCollectionView)CollectionViewSource.GetDefaultView(contractsEntities);
                listView.ItemsSource = contractsView;
                filtercontr();
                filtermap();
                return null;
            }
            else
            {
                //MessageBox.Show(main.jsvalless, main.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        private void webControl1_DocumentReady(object sender, DocumentReadyEventArgs e)
        {
            using (JSObject myGlobalObject = webControl1.CreateGlobalJavascriptObject("myGlobalObject"))
            {
                myGlobalObject.Bind("update", update);
                myGlobalObject.Bind("returnvalue", returnvalue);
            }
        }
        private void RibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            webControl1.ExecuteJavascript("savemapstate()");
            webControl1.Dispose();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        private void filteredMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ribbon.SelectedTabIndex = 5;
            //replist.ItemsSource = contractsView;
        }
        private void addcontbutton_Click(object sender, RoutedEventArgs e)
        {
            if (farmer.Properties.Settings.Default.editdbenable)
            {
                contractwindow newcontract = new contractwindow();
                newcontract.Owner = this;
                ApplyEffect(this);
                newcontract.ShowDialog();
                refreshwin();
                ClearEffect(this);
            }
            else
            {
                ApplyEffect(this);
                MessageBox.Show(Application.Current.MainWindow, (string)FindResource("m_enableeditdbwarning"), (string)FindResource("m_warning"), MessageBoxButton.OK, MessageBoxImage.Warning);
                ClearEffect(this);
            }
        }
        private void addcontXMLbutton_Click(object sender, RoutedEventArgs e)
        {
            if (farmer.Properties.Settings.Default.editdbenable)
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".xml";
                dlg.Filter = "XML (.xml)|*.xml";
                dlg.Multiselect = true;
                Nullable<bool> result = dlg.ShowDialog(Window.GetWindow(this));
                if (result == true && dlg.FileNames.Length > 0)
                {
                    ApplyEffect(this);
                    dialogwin objModal = new dialogwin();
                    objModal.Owner = this;
                    objModal.filenames = dlg.FileNames;
                    objModal.ShowDialog();
                    refreshwin();
                    ClearEffect(this);
                }
            }
            else
            {
                ApplyEffect(this);
                MessageBox.Show(Application.Current.MainWindow, (string)FindResource("m_enableeditdbwarning"), (string)FindResource("m_warning"), MessageBoxButton.OK, MessageBoxImage.Warning);
                ClearEffect(this);
            }
        }
        Task webcontrldfrcompthr;
        private void webcontrldfrcomp()
        {
            foreach (contract row in contractsList)
            {
                geojsonFeature lay = new geojsonFeature();
                lay.type = "Feature";
                lay.properties = new geojsonFeature.propertiesFeature();
                lay.geometry = new geojsonFeature.geometryFeature();
                lay.properties.color = "DeepSkyBlue";
                lay.properties.weight = "2";
                lay.properties.areanum = row.area_number;//m_leaser
                string description = @"<div id=" + '"' + "index" + '"' + " style=" + '"' + "display:none" + '"' + ">" + row.id + "</div>" +
                    "<div id=" + '"' + "cadnum" + '"' + ">" + (string)FindResource("m_cadnum") + ": " + row.cadastral_number + "</div>" +
                    "<div id=" + '"' + "owner" + '"' + ">" + (string)FindResource("m_lessor") + ": " + row.lessor + "</div>" +
                    "<div id=" + '"' + "leaser" + '"' + ">" + (string)FindResource("m_leaser") + ": " + row.leaser + "</div>" +
                    "<div id=" + '"' + "square" + '"' + ">" + (string)FindResource("m_square") + ": " + row.square + "</div>" +
                    "<div id=" + '"' + "field" + '"' + ">" + (string)FindResource("m_field") +
                    ": <input id=" + '"' + "area_input" + '"' + " type=" + '"' + "text" + '"' + " style=" + '"' + "width: 60px" + '"' + " value=" + '"' + row.area_number + '"' + "/>" +
                    "<button id=" + '"' + "savebtn" + '"' + " style=" + '"' + "visibility: visible" + '"' + " onclick=" + '"' + "updatefdb()" + '"' + ">" + (string)FindResource("m_save") + "</button></div>" +
                    "<div id=" + '"' + "unit_number" + '"' + ">" + (string)FindResource("m_unit_number") + ": " + row.unit_number + "</div>";
                string newdescrp = "";
                for (int i=0; i < description.Length; i++)
                {
                    if (description[i] == Char.Parse("'"))
                    {
                        newdescrp = newdescrp + "&#39;";
                    }
                    else
                    {
                        newdescrp = newdescrp + description[i];
                    }
                }
                List<coordsformat> currcoords = new List<coordsformat>();
                foreach (coordsformat coord in allcoords)
                {
                    if (coord.id == row.id)
                    {
                        currcoords.Add(coord);
                    }
                }
                List<int> listrank = new List<int>();
                foreach (coordsformat coord in currcoords)
                {
                    if (!listrank.Contains(coord.rank))
                    {
                        listrank.Add(coord.rank);
                    }
                }
                lay.geometry.type = "Polygon";
                lay.geometry.coordinates = new string[listrank.Count()][][];
                for (int l = 0; l < listrank.Count(); l++)
                {
                    List<coordsformat> coordss = new List<coordsformat>();
                    foreach (coordsformat coord in currcoords)
                    {
                        if (coord.rank == listrank[l])
                        {
                            coordss.Add(coord);
                        }
                    }
                    lay.geometry.coordinates[l] = new string[coordss.Count()][];
                    int i = 0;
                    foreach (var item in coordss)
                    {
                        lay.geometry.coordinates[l][i] = new string[2];
                        lay.geometry.coordinates[l][i][0] = item.x.ToString();
                        lay.geometry.coordinates[l][i][1] = item.y.ToString();
                        i++;
                    }
                }
                string jsonlay = Serializejson(lay);
                Dispatcher.BeginInvoke(new Action(() => webControl1.ExecuteJavascript("loadarea('" + Serializejson(lay) + "'" + "," + "'" + newdescrp + "')")));
            }
            Dispatcher.BeginInvoke(new Action(() => {
                webControl1.ExecuteJavascript("init()");
                string cumsum = 0.ToString();
                foreach (contract contr in contractsList)
                {
                    cumsum = (Convert.ToDouble(cumsum) + Convert.ToDouble(contr.square)).ToString();
                }
                filtermap();
                maplbl.Text = (string)FindResource("m_sumsq") + cumsum;
            }));
            ClearEffect(this);
        }
        private void webControl1_LoadingFrameComplete(object sender, FrameEventArgs e)
        {
            if (contractsList.Count > 0 && e.IsMainFrame)
            {
                ApplyEffect(this);
                webcontrldfrcompthr = new Task(webcontrldfrcomp);
                webcontrldfrcompthr.Start();
            }
        }
        private string Serializejson(geojsonFeature lay)
        {
            string coords = "[";
            for (int i = 0; i < lay.geometry.coordinates.Length; i++)
            {
                if (i > 0)
                {
                    coords = coords + ",";
                }
                coords = coords + "[";
                for (int l = 0; l < lay.geometry.coordinates[i].Length; l++)
                {
                    if (l > 0)
                    {
                        coords = coords + ",";
                    }
                    coords = coords + "[";
                    coords = coords + lay.geometry.coordinates[i][l][0].ToString() + "," + lay.geometry.coordinates[i][l][1].ToString();
                    coords = coords + "]";
                }
                coords = coords + "]";
            }
            coords = coords + "]";
            return '{'.ToString() + '"'.ToString() + "type" + '"'.ToString() + ':'.ToString() + '"'.ToString() + lay.type + '"'.ToString() + ','.ToString() +
                '"'.ToString() + "properties" + '"'.ToString() + ':'.ToString() +
                    '{'.ToString() +
                        '"'.ToString() + "name" + '"'.ToString() + ':'.ToString() + '"'.ToString() + lay.properties.name + '"'.ToString() + ','.ToString() +
                        '"'.ToString() + "color" + '"'.ToString() + ':'.ToString() + '"'.ToString() + lay.properties.color + '"'.ToString() + ','.ToString() +
                        '"'.ToString() + "weight" + '"'.ToString() + ':'.ToString() + '"'.ToString() + lay.properties.weight + '"'.ToString() + ','.ToString() +
                        '"'.ToString() + "areanum" + '"'.ToString() + ':'.ToString() + '"'.ToString() + lay.properties.areanum + '"'.ToString() + ','.ToString() +
                        '"'.ToString() + "description" + '"'.ToString() + ':'.ToString() + '"'.ToString() + lay.properties.description + '"'.ToString() + ','.ToString() +
                        '"'.ToString() + "opacity" + '"'.ToString() + ':'.ToString() + '"'.ToString() + lay.properties.opacity + '"'.ToString() +
                     '}'.ToString() + ','.ToString() +
                     '"'.ToString() + "geometry" + '"'.ToString() + ':'.ToString() +
                    '{'.ToString() +
                        '"'.ToString() + "type" + '"'.ToString() + ':'.ToString() + '"'.ToString() + lay.geometry.type + '"'.ToString() + ','.ToString() +
                        '"'.ToString() + "coordinates" + '"'.ToString() + ':'.ToString() + coords +
                    '}'.ToString() +
                 '}'.ToString();
        }
        private void filtermap()
        {
            if (this.IsInitialized && WebCore.IsInitialized && webControl1.IsDocumentReady && webView1.IsDocumentReady)
            {
                int cheknum = 0;
                List<string> listid = new List<string>();
                foreach (contract contr in contractsList)
                {
                    bool tr = true;
                    if (maptypecomboBox.SelectedIndex != -1 && (bool)maptypecheckBox.IsChecked)
                    {
                        cheknum++;
                        if (!(contr.type_contract == (maptypecomboBox.SelectedIndex).ToString()))
                        {
                            tr = false;
                        }
                    }
                    if (mapleasercomboBox.SelectedIndex != -1 && (bool)mapleasercheckBox.IsChecked)
                    {
                        cheknum++;
                        if (!(contr.leaser.ToLower() == (mapleasercomboBox.SelectedValue).ToString()))
                        {
                            tr = false;
                        }
                    }
                    if (mapstatuscomboBox.SelectedIndex != -1 && (bool)mapstatuscheckBox.IsChecked)
                    {
                        cheknum++;
                        if (!(contr.contract_status == (mapstatuscomboBox.SelectedIndex).ToString()))
                        {
                            tr = false;
                        }
                    }
                    if (mapformownercomboBox.SelectedIndex != -1 && (bool)mapformownercheckBox.IsChecked)
                    {
                        cheknum++;
                        if (!(contr.form_owner == (mapformownercomboBox.SelectedIndex).ToString()))
                        {
                            tr = false;
                        }
                    }
                    if (mapformunitcomboBox.SelectedIndex != -1 && (bool)mapformunitcheckBox.IsChecked)
                    {
                        cheknum++;
                        if (!(contr.form_unit == (mapformunitcomboBox.SelectedIndex).ToString()))
                        {
                            tr = false;
                        }
                    }
                    if (mapformusecomboBox.SelectedIndex != -1 && (bool)mapformusecheckBox.IsChecked)
                    {
                        cheknum++;
                        if (!(contr.form_use == (mapformusecomboBox.SelectedIndex).ToString()))
                        {
                            tr = false;
                        }
                    }
                    if (mapcadnumtextBox.Text.Length != 0 && (bool)mapcadnumcheckBox.IsChecked)
                    {
                        cheknum++;
                        if (!contr.cadastral_number.Contains(mapcadnumtextBox.Text))
                        {
                            tr = false;
                        }
                    }
                    if (mapareatextBox.Text.Length != 0 && (bool)mapareacheckBox.IsChecked)
                    {
                        cheknum++;
                        if (!contr.area_number.Contains(mapareatextBox.Text))
                        {
                            tr = false;
                        }
                    }
                    if (mapunittextBox.Text.Length != 0 && (bool)mapunitcheckBox.IsChecked)
                    {
                        cheknum++;
                        if (!contr.unit_number.Contains(mapunittextBox.Text))
                        {
                            tr = false;
                        }
                    }
                    if ((bool)mapdatacontscheckBox.IsChecked)
                    {
                        cheknum++;
                        if ((Convert.ToDateTime(contr.date_contract).CompareTo(mapdatacontrsdatePicker.SelectedDate)) < 0)
                        {
                            tr = false;
                        }
                    }
                    if ((bool)mapdatacontpocheckBox.IsChecked)
                    {
                        cheknum++;
                        if ((Convert.ToDateTime(contr.date_contract).CompareTo(mapdatacontrpodatePicker.SelectedDate)) > 0)
                        {
                            tr = false;
                        }
                    }
                    if ((bool)mapdataregscheckBox.IsChecked)
                    {
                        cheknum++;
                        if ((Convert.ToDateTime(contr.registration_date).CompareTo(mapdataregsdatePicker.SelectedDate)) < 0)
                        {
                            tr = false;
                        }
                    }
                    if ((bool)mapdataregpocheckBox.IsChecked)
                    {
                        cheknum++;
                        if ((Convert.ToDateTime(contr.registration_date).CompareTo(mapdataregpodatePicker.SelectedDate)) > 0)
                        {
                            tr = false;
                        }
                    }
                    if (mapoblcomboBox.SelectedIndex != -1 && (bool)mapoblcheckBox.IsChecked)
                    {
                        cheknum++;
                        if (!(Convert.ToInt64(contr.koatuu) >= Convert.ToInt64(mapoblcomboBox.SelectedValue) && Convert.ToInt64(contr.koatuu) <= (Convert.ToInt64(mapoblcomboBox.SelectedValue) + 199999999)))
                        {
                            tr = false;
                        }
                    }
                    if (mapraycomboBox.SelectedIndex != -1 && (bool)mapraycheckBox.IsChecked)
                    {
                        cheknum++;
                        if (!(Convert.ToInt64(contr.koatuu) >= Convert.ToInt64(mapraycomboBox.SelectedValue) && Convert.ToInt64(contr.koatuu) <= (Convert.ToInt64(mapraycomboBox.SelectedValue) + 199999)))
                        {
                            tr = false;
                        }
                    }
                    if (mapsilcomboBox.SelectedIndex != -1 && (bool)mapsilcheckBox.IsChecked)
                    {
                        cheknum++;
                        if (!(Convert.ToInt64(contr.koatuu) >= Convert.ToInt64(mapsilcomboBox.SelectedValue) && Convert.ToInt64(contr.koatuu) <= (Convert.ToInt64(mapsilcomboBox.SelectedValue) + 09)))
                        {
                            tr = false;
                        }
                    }
                    if (tr)
                    {
                        listid.Add(contr.id.ToString());
                    }
                }
                string cumsum = 0.ToString();
                string cumsumfil = 0.ToString();
                foreach (contract contr in contractsList)
                {
                    cumsum = (Convert.ToDouble(cumsum) + Convert.ToDouble(contr.square)).ToString();
                }
                foreach (string filid in listid)
                {
                    foreach (contract contr in contractsList)
                    {
                        if (contr.id == Convert.ToInt32(filid))
                        {
                            cumsumfil = (Convert.ToDouble(cumsumfil) + Convert.ToDouble(contr.square)).ToString();
                        }
                    }
                }
                maplbl.Text = (string)FindResource("m_sumsq") + cumsum;
                if (listid.Count > 0 && cheknum > 0)
                {
                    double[][][][] listcoords = new double[listid.Count][][][];
                    Parallel.For(0, listid.Count, k =>
                    {
                        List<coordsformat> currcoords = new List<coordsformat>();
                        int sumrank = 0;
                        for (int l = 0; l < allcoords.Count; l++)
                        {
                            if (allcoords[l].id == Convert.ToInt32(listid[k]))
                            {
                                currcoords.Add(allcoords[l]);
                                if (allcoords[l].rank > sumrank)
                                {
                                    sumrank++;
                                }
                            }
                        }
                        listcoords[k] = new double[sumrank + 1][][];
                        for (int l = 0; l < sumrank + 1; l++)
                        {
                            List<coordsformat> coordss = new List<coordsformat>();
                            for (int m = 0; m < currcoords.Count; m++)
                            {
                                if (currcoords[m].rank == Convert.ToInt32(l))
                                {
                                    coordss.Add(currcoords[m]);
                                }
                            }
                            listcoords[k][l] = new double[coordss.Count() + 1][];
                            for (int i = 0; i < coordss.Count; i++)
                            {
                                listcoords[k][l][i] = new double[2];
                                listcoords[k][l][i][0] = Convert.ToDouble(coordss[i].x.Replace('.', ','));
                                listcoords[k][l][i][1] = Convert.ToDouble(coordss[i].y.Replace('.', ','));
                            }
                            listcoords[k][l][listcoords[k][l].Count() - 1] = listcoords[k][l][0];
                        }
                    });
                    if (cumsumfil == cumsum)
                    {
                        maplbl.Text = (string)FindResource("m_sumsq") + cumsum;
                    }
                    else
                    {
                        maplbl.Text = (string)FindResource("m_sumsq") + cumsum + (string)FindResource("m_sumsqfil") + cumsumfil;
                    }
                    webControl1.ExecuteJavascript("lights('" + JsonConvert.SerializeObject(listcoords) + "',true)");
                    webView1.ExecuteJavascript("lights('" + JsonConvert.SerializeObject(listcoords) + "',true)");
                }
                else
                {
                    webControl1.ExecuteJavascript("removeLayer()");
                    webView1.ExecuteJavascript("removeLayer()");
                }
            }
        }
        private void filtercontr()
        {
            if (this.IsInitialized && WebCore.IsInitialized)
            {
                contractsEntities.Clear();
                ParallelOptions po = new ParallelOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                };
                Parallel.ForEach(contractsList, po, contr =>
                //foreach (contract contr in contractsList)
                {

                    bool tr = true;
                    Dispatcher.BeginInvoke(new Action(() => {
                        if (leasercomboBox.SelectedIndex != -1 && (bool)leasercheckBox.IsChecked)
                        {
                            if (!(contr.leaser.ToLower() == (leasercomboBox.SelectedValue).ToString()))
                            {
                                tr = false;
                            }
                        }
                        if (typecomboBox.SelectedIndex != -1 && (bool)typecheckBox.IsChecked)
                        {
                            if (!(contr.type_contract == (typecomboBox.SelectedIndex).ToString()))
                            {
                                tr = false;
                            }
                        }
                        if (statuscomboBox.SelectedIndex != -1 && (bool)statuscheckBox.IsChecked)
                        {
                            if (!(contr.contract_status == (statuscomboBox.SelectedIndex).ToString()))
                            {
                                tr = false;
                            }
                        }
                        if (formownercomboBox.SelectedIndex != -1 && (bool)formownercheckBox.IsChecked)
                        {
                            if (!(contr.form_owner == (formownercomboBox.SelectedIndex).ToString()))
                            {
                                tr = false;
                            }
                        }
                        if (formunitcomboBox.SelectedIndex != -1 && (bool)formunitcheckBox.IsChecked)
                        {
                            if (!(contr.form_unit == (formunitcomboBox.SelectedIndex).ToString()))
                            {
                                tr = false;
                            }
                        }
                        if (formusecomboBox.SelectedIndex != -1 && (bool)formusecheckBox.IsChecked)
                        {
                            if (!(contr.form_use == (formusecomboBox.SelectedIndex).ToString()))
                            {
                                tr = false;
                            }
                        }
                        if (cadnumtextBox.Text.Length != 0 && (bool)cadnumcheckBox.IsChecked)
                        {
                            if (!contr.cadastral_number.Contains(cadnumtextBox.Text))
                            {
                                tr = false;
                            }
                        }
                        if (areatextBox.Text.Length != 0 && (bool)areacheckBox.IsChecked)
                        {
                            if (!contr.area_number.Contains(areatextBox.Text))
                            {
                                tr = false;
                            }
                        }
                        if (unittextBox.Text.Length != 0 && (bool)unitcheckBox.IsChecked)
                        {
                            if (!contr.unit_number.Contains(unittextBox.Text))
                            {
                                tr = false;
                            }
                        }
                        if ((bool)datacontscheckBox.IsChecked)
                        {
                            if ((Convert.ToDateTime(contr.date_contract).CompareTo(datacontrsdatePicker.SelectedDate)) < 0)
                            {
                                tr = false;
                            }
                        }
                        if ((bool)datacontpocheckBox.IsChecked)
                        {
                            if ((Convert.ToDateTime(contr.date_contract).CompareTo(datacontrpodatePicker.SelectedDate)) > 0)
                            {
                                tr = false;
                            }
                        }
                        if ((bool)dataregscheckBox.IsChecked)
                        {
                            if ((Convert.ToDateTime(contr.registration_date).CompareTo(dataregsdatePicker.SelectedDate)) < 0)
                            {
                                tr = false;
                            }
                        }
                        if ((bool)dataregpocheckBox.IsChecked)
                        {
                            if ((Convert.ToDateTime(contr.registration_date).CompareTo(dataregpodatePicker.SelectedDate)) > 0)
                            {
                                tr = false;
                            }
                        }
                        if (oblcomboBox.SelectedIndex != -1 && (bool)oblcheckBox.IsChecked)
                        {
                            if (!(Convert.ToInt64(contr.koatuu) >= Convert.ToInt64(oblcomboBox.SelectedValue) && Convert.ToInt64(contr.koatuu) <= (Convert.ToInt64(oblcomboBox.SelectedValue) + 199999999)))
                            {
                                tr = false;
                            }
                        }
                        if (raycomboBox.SelectedIndex != -1 && (bool)raycheckBox.IsChecked)
                        {
                            if (!(Convert.ToInt64(contr.koatuu) >= Convert.ToInt64(raycomboBox.SelectedValue) && Convert.ToInt64(contr.koatuu) <= (Convert.ToInt64(raycomboBox.SelectedValue) + 199999)))
                            {
                                tr = false;
                            }
                        }
                        if (silcomboBox.SelectedIndex != -1 && (bool)silcheckBox.IsChecked)
                        {
                            if (!(Convert.ToInt64(contr.koatuu) >= Convert.ToInt64(silcomboBox.SelectedValue) && Convert.ToInt64(contr.koatuu) <= (Convert.ToInt64(silcomboBox.SelectedValue) + 09)))
                            {
                                tr = false;
                            }
                        }

                        if (tr)
                        {
                            contractsEntities.Add(new contract
                            {
                                id = contr.id,
                                contract_number = contr.contract_number,
                                date_contract = contr.date_contract,
                                registration_date = contr.registration_date,
                                lessor = contr.lessor,
                                leaser = contr.leaser,
                                cadastral_number = contr.cadastral_number,
                                square = contr.square,
                                term_lease = contr.term_lease,
                                contract_status = contr.contract_status,
                                type_contract = contr.type_contract,
                                form_owner = contr.form_owner,
                                form_use = contr.form_use,
                                form_unit = contr.form_unit,
                                area_number = contr.area_number,
                                unit_number = contr.unit_number,
                                notes = contr.notes,
                                koatuu = contr.koatuu
                            });
                        }
                    }));
                });
            }
        }
        private void cadnumtextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cadnumtextBox != null)
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
                        filtercontr();
                    }
                    else
                    {
                        cadnumtextBox.Text = "";
                        filtercontr();
                    }
                }
            }
        }
        private void mapcadnumtextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mapcadnumtextBox != null)
            {
                if (mapcadnumtextBox.Text.Length != 0)
                {
                    bool alldig = false;
                    for (int i = 0; i < mapcadnumtextBox.Text.Length; i++)
                    {
                        if (Char.IsDigit(mapcadnumtextBox.Text[i]) || mapcadnumtextBox.Text[i] == ':')
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
                            switch (mapcadnumtextBox.Text.Length)
                            {
                                case 10:
                                    mapcadnumtextBox.Text = mapcadnumtextBox.Text + ':';
                                    mapcadnumtextBox.SelectionStart = mapcadnumtextBox.Text.Length;
                                    mapcadnumtextBox.SelectionLength = 0;
                                    break;
                                case 13:
                                    mapcadnumtextBox.Text = mapcadnumtextBox.Text + ':';
                                    mapcadnumtextBox.SelectionStart = mapcadnumtextBox.Text.Length;
                                    mapcadnumtextBox.SelectionLength = 0;
                                    break;
                                case 17:
                                    mapcadnumtextBox.Text = mapcadnumtextBox.Text + ':';
                                    mapcadnumtextBox.SelectionStart = mapcadnumtextBox.Text.Length;
                                    mapcadnumtextBox.SelectionLength = 0;
                                    break;
                            }
                        }
                        else
                        {
                            switch (mapcadnumtextBox.Text.Length)
                            {
                                case 11:
                                    mapcadnumtextBox.Text = mapcadnumtextBox.Text.Remove(mapcadnumtextBox.Text.Length - 1, 1);
                                    break;
                                case 14:
                                    mapcadnumtextBox.Text = mapcadnumtextBox.Text.Remove(mapcadnumtextBox.Text.Length - 1, 1);
                                    break;
                                case 18:
                                    mapcadnumtextBox.Text = mapcadnumtextBox.Text.Remove(mapcadnumtextBox.Text.Length - 1, 1);
                                    break;
                            }
                        }
                        filtermap();
                    }
                    else
                    {
                        mapcadnumtextBox.Text = "";
                        filtermap();
                    }
                }
            }
        }

        private void mapoblcomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mapoblcomboBox.SelectedIndex != 0 && (bool)mapoblcheckBox.IsChecked)
            {
                filtermap();
                ArrayList rayname = bdfunc.select("SELECT code,NAME FROM RAYON WHERE CODE >= " + Convert.ToInt64(mapoblcomboBox.SelectedValue) + " and CODE <= " + (Convert.ToInt64(mapoblcomboBox.SelectedValue) + 199999999) + ";");
                if (rayname.Count != 0)
                {
                    List<KeyValuePair<string, string>> mapraycomboBoxitems = new List<KeyValuePair<string, string>>();
                    for (int i = 0; i < rayname.Count; i += 2)
                    {
                        mapraycomboBoxitems.Add(new KeyValuePair<string, string>(rayname[i].ToString(), rayname[i + 1].ToString()));
                    }
                    mapraycomboBox.ItemsSource = null;
                    mapsilcomboBox.ItemsSource = null;
                    mapraycomboBox.ItemsSource = mapraycomboBoxitems;
                    mapraycomboBox.DisplayMemberPath = "Value";
                    mapraycomboBox.SelectedValuePath = "Key";
                }
                else
                {
                    MessageBox.Show("No loc info for rays", (string)FindResource("m_Error"));
                }
            }
        }

        private void mapraycomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mapraycomboBox.SelectedIndex != 0 && (bool)mapraycheckBox.IsChecked)
            {
                filtermap();
                ArrayList silname = bdfunc.select("SELECT code,NAME FROM sil WHERE CODE >= " + Convert.ToInt64(mapraycomboBox.SelectedValue) + " and CODE <= " + (Convert.ToInt64(mapraycomboBox.SelectedValue) + 199999) + ";");
                if (silname.Count != 0)
                {
                    List<KeyValuePair<string, string>> mapsilcomboBoxitems = new List<KeyValuePair<string, string>>();
                    for (int i = 0; i < silname.Count; i += 2)
                    {
                        mapsilcomboBoxitems.Add(new KeyValuePair<string, string>(silname[i].ToString(), silname[i + 1].ToString()));
                    }
                    mapsilcomboBox.ItemsSource = null;
                    mapsilcomboBox.ItemsSource = mapsilcomboBoxitems;
                    mapsilcomboBox.DisplayMemberPath = "Value";
                    mapsilcomboBox.SelectedValuePath = "Key";
                }
                else
                {
                    MessageBox.Show("No loc info for sils", (string)FindResource("m_Error"));
                }
            }
        }

        private void mapsilcomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mapsilcomboBox.SelectedIndex != 0 && (bool)mapsilcheckBox.IsChecked)
            {
                filtermap();
            }
        }

        private void oblcomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (oblcomboBox.SelectedIndex != 0 && (bool)oblcheckBox.IsChecked)
            {
                ArrayList sel = bdfunc.select("select id from contracts where koatuu>= " + Convert.ToInt64(oblcomboBox.SelectedValue) + " and koatuu <= " + (Convert.ToInt64(oblcomboBox.SelectedValue) + 199999999) + ";");
                if (sel.Count != 0)
                {
                    ArrayList rayname = bdfunc.select("SELECT code,NAME FROM RAYON WHERE CODE >= " + Convert.ToInt64(oblcomboBox.SelectedValue) + " and CODE <= " + (Convert.ToInt64(oblcomboBox.SelectedValue) + 199999999) + ";");
                    if (rayname.Count != 0)
                    {
                        List<KeyValuePair<string, string>> rayitems = new List<KeyValuePair<string, string>>();
                        for (int i = 0; i < rayname.Count; i += 2)
                        {
                            rayitems.Add(new KeyValuePair<string, string>(rayname[i].ToString(), rayname[i + 1].ToString()));
                        }
                        raycomboBox.ItemsSource = null;
                        silcomboBox.ItemsSource = null;
                        raycomboBox.ItemsSource = rayitems;
                        raycomboBox.DisplayMemberPath = "Value";
                        raycomboBox.SelectedValuePath = "Key";
                    }
                    else
                    {
                        MessageBox.Show("No loc info for rays", (string)FindResource("m_Error"));
                    }
                    filtercontr();
                }
                else
                {
                    filtercontr();
                    //MessageBox.Show("Нет договоров в этой области", (string)FindResource("m_Error);
                    //oblcomboBox.SelectedIndex = 0;
                    raycomboBox.ItemsSource = null;
                    silcomboBox.ItemsSource = null;
                }
            }
        }

        private void raycomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (raycomboBox.SelectedIndex != 0 && (bool)raycheckBox.IsChecked)
            {
                ArrayList sel = bdfunc.select("select id from contracts where koatuu >= " + Convert.ToInt64(raycomboBox.SelectedValue) + " and koatuu <= " + (Convert.ToInt64(raycomboBox.SelectedValue) + 199999) + ";");
                if (sel.Count != 0)
                {
                    ArrayList silname = bdfunc.select("SELECT code,NAME FROM sil WHERE CODE >= " + Convert.ToInt64(raycomboBox.SelectedValue) + " and CODE <= " + (Convert.ToInt64(raycomboBox.SelectedValue) + 199999) + ";");
                    if (silname.Count != 0)
                    {
                        List<KeyValuePair<string, string>> silitems = new List<KeyValuePair<string, string>>();
                        for (int i = 0; i < silname.Count; i += 2)
                        {
                            silitems.Add(new KeyValuePair<string, string>(silname[i].ToString(), silname[i + 1].ToString()));
                        }
                        silcomboBox.ItemsSource = null;
                        silcomboBox.ItemsSource = silitems;
                        silcomboBox.DisplayMemberPath = "Value";
                        silcomboBox.SelectedValuePath = "Key";
                    }
                    else
                    {
                        MessageBox.Show("No loc info for sils", (string)FindResource("m_Error"));
                    }
                    filtercontr();
                }
                else
                {
                    filtercontr();
                    //MessageBox.Show("Нет договоров в этом районе", (string)FindResource("m_Error);
                    raycomboBox.SelectedIndex = 0;
                    silcomboBox.ItemsSource = null;
                }
            }
        }

        private void silcomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (silcomboBox.SelectedIndex != 0 && (bool)silcheckBox.IsChecked)
            {
                filtercontr();
            }
        }

        private void leasercheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)leasercheckBox.IsChecked)
            {
                //gf.AddFilter(x => ((contract)x).cadastral_number.Contains(cadnumtextBox.Text));
                leasercomboBox.IsEnabled = true;
                filtercontr();
            }
            else
            {
                //gf.RemoveFilter(x => ((contract)x).cadastral_number.Contains(cadnumtextBox.Text));
                leasercomboBox.IsEnabled = false;
                filtercontr();
            }
        }
        private void mapleasercheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)mapleasercheckBox.IsChecked)
            {
                mapleasercomboBox.IsEnabled = true;
                filtermap();
            }
            else
            {
                mapleasercomboBox.IsEnabled = false;
                filtermap();
            }
        }
        private void cadnumcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)cadnumcheckBox.IsChecked)
            {
                //gf.AddFilter(x => ((contract)x).cadastral_number.Contains(cadnumtextBox.Text));
                cadnumtextBox.IsEnabled = true;
                filtercontr();
            }
            else
            {
                //gf.RemoveFilter(x => ((contract)x).cadastral_number.Contains(cadnumtextBox.Text));
                cadnumtextBox.IsEnabled = false;
                filtercontr();
            }
        }
        private void mapcadnumcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)mapcadnumcheckBox.IsChecked)
            {
                mapcadnumtextBox.IsEnabled = true;
                filtermap();
            }
            else
            {
                mapcadnumtextBox.IsEnabled = false;
                filtermap();
            }
        }
        private void typecheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)typecheckBox.IsChecked)
            {
                //gf.AddFilter(x => ((contract)x).type_contract == (typecomboBox.SelectedIndex + 1).ToString());
                typecomboBox.IsEnabled = true;
                filtercontr();
            }
            else
            {
                //gf.RemoveFilter(x => ((contract)x).type_contract == (typecomboBox.SelectedIndex + 1).ToString());
                typecomboBox.IsEnabled = false;
                filtercontr();
            }
        }

        private void maptypecheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)maptypecheckBox.IsChecked)
            {
                maptypecomboBox.IsEnabled = true;
                filtermap();
            }
            else
            {
                maptypecomboBox.IsEnabled = false;
                filtermap();
            }
        }
        private void statuscheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)statuscheckBox.IsChecked)
            {
                statuscomboBox.IsEnabled = true;
                //gf.AddFilter(x => ((contract)x).contract_status == (statuscomboBox.SelectedIndex + 1).ToString());
                filtercontr();
            }
            else
            {
                statuscomboBox.IsEnabled = false;
                //gf.RemoveFilter(x => ((contract)x).contract_status == (statuscomboBox.SelectedIndex + 1).ToString());
                filtercontr();
            }
        }
        private void mapstatuscheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)mapstatuscheckBox.IsChecked)
            {
                mapstatuscomboBox.IsEnabled = true;
                filtermap();
            }
            else
            {
                mapstatuscomboBox.IsEnabled = false;
                filtermap();
            }

        }
        private void formownercheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)formownercheckBox.IsChecked)
            {
                formownercomboBox.IsEnabled = true;
                //gf.AddFilter(x => ((contract)x).form_owner == (formownercomboBox.SelectedIndex + 1).ToString());
                filtercontr();
            }
            else
            {
                formownercomboBox.IsEnabled = false;
                //gf.RemoveFilter(x => ((contract)x).form_owner == (formownercomboBox.SelectedIndex + 1).ToString());
                filtercontr();
            }
        }
        private void mapformownercheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)mapformownercheckBox.IsChecked)
            {
                mapformownercomboBox.IsEnabled = true;
                filtermap();
            }
            else
            {
                mapformownercomboBox.IsEnabled = false;
                filtermap();
            }
        }
        private void formunitcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)formunitcheckBox.IsChecked)
            {
                formunitcomboBox.IsEnabled = true;
                //gf.AddFilter(x => ((contract)x).form_unit == (formunitcomboBox.SelectedIndex + 1).ToString());
                filtercontr();
            }
            else
            {
                formunitcomboBox.IsEnabled = false;
                //gf.RemoveFilter(x => ((contract)x).form_unit == (formunitcomboBox.SelectedIndex + 1).ToString());
                filtercontr();
            }
        }
        private void mapformunitcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)mapformunitcheckBox.IsChecked)
            {
                mapformunitcomboBox.IsEnabled = true;
                filtermap();
            }
            else
            {
                mapformunitcomboBox.IsEnabled = false;
                filtermap();
            }
        }

        private void formusecheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)formusecheckBox.IsChecked)
            {
                formusecomboBox.IsEnabled = true;
                //gf.AddFilter(x => ((contract)x).form_use == (formusecomboBox.SelectedIndex + 1).ToString());
                filtercontr();
            }
            else
            {
                formusecomboBox.IsEnabled = false;
                //gf.RemoveFilter(x => ((contract)x).form_use == (formusecomboBox.SelectedIndex + 1).ToString());
                filtercontr();
            }
        }
        private void mapformusecheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)mapformusecheckBox.IsChecked)
            {
                mapformusecomboBox.IsEnabled = true;
                filtermap();
            }
            else
            {
                mapformusecomboBox.IsEnabled = false;
                filtermap();
            }
        }
        private void datacontscheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)datacontscheckBox.IsChecked)
            {
                datacontrsdatePicker.IsEnabled = true;
                //gf.AddFilter(x => (Convert.ToDateTime(((contract)sender).date_contract).CompareTo(datacontrsdatePicker.SelectedDate) < 0));
                filtercontr();
            }
            else
            {
                datacontrsdatePicker.IsEnabled = false;
                //gf.RemoveFilter(x => (Convert.ToDateTime(((contract)sender).date_contract).CompareTo(datacontrsdatePicker.SelectedDate) < 0));
                filtercontr();
            }
        }
        private void mapdatacontscheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)mapdatacontscheckBox.IsChecked)
            {
                mapdatacontrsdatePicker.IsEnabled = true;
                filtermap();
            }
            else
            {
                mapdatacontrsdatePicker.IsEnabled = false;
                filtermap();
            }
        }
        private void datacontpocheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)datacontpocheckBox.IsChecked)
            {
                datacontrpodatePicker.IsEnabled = true;
                //gf.AddFilter(x => (Convert.ToDateTime(((contract)sender).date_contract).CompareTo(datacontrpodatePicker.SelectedDate) > 0));
                filtercontr();
            }
            else
            {
                datacontrpodatePicker.IsEnabled = false;
                //gf.RemoveFilter(x => (Convert.ToDateTime(((contract)sender).date_contract).CompareTo(datacontrpodatePicker.SelectedDate) > 0));
                filtercontr();
            }
        }
        private void mapdatacontpocheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)mapdatacontpocheckBox.IsChecked)
            {
                mapdatacontrpodatePicker.IsEnabled = true;
                filtermap();
            }
            else
            {
                mapdatacontrpodatePicker.IsEnabled = false;
                filtermap();
            }
        }
        private void dataregscheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)dataregscheckBox.IsChecked)
            {
                dataregsdatePicker.IsEnabled = true;
                //gf.AddFilter(x => (Convert.ToDateTime(((contract)sender).registration_date).CompareTo(dataregsdatePicker.SelectedDate) < 0));
                filtercontr();
            }
            else
            {
                dataregsdatePicker.IsEnabled = false;
                //gf.RemoveFilter(x => ((Convert.ToDateTime(((contract)sender).registration_date).CompareTo(dataregsdatePicker.SelectedDate)) < 0));
                filtercontr();
            }
        }
        private void mapdataregscheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)mapdataregscheckBox.IsChecked)
            {
                mapdataregsdatePicker.IsEnabled = true;
                filtermap();
            }
            else
            {
                mapdataregsdatePicker.IsEnabled = false;
                filtermap();
            }
        }
        private void dataregpocheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)dataregpocheckBox.IsChecked)
            {
                dataregpodatePicker.IsEnabled = true;
                //gf.AddFilter(x => ((Convert.ToDateTime(((contract)sender).registration_date).CompareTo(dataregpodatePicker.SelectedDate)) > 0));
                filtercontr();
            }
            else
            {
                dataregpodatePicker.IsEnabled = false;
                //gf.RemoveFilter(x => ((Convert.ToDateTime(((contract)sender).registration_date).CompareTo(dataregpodatePicker.SelectedDate)) > 0));
                filtercontr();
            }
        }
        private void mapdataregpocheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)mapdataregpocheckBox.IsChecked)
            {
                mapdataregpodatePicker.IsEnabled = true;
                filtermap();
            }
            else
            {
                mapdataregpodatePicker.IsEnabled = false;
                filtermap();
            }
        }
        private void areacheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)areacheckBox.IsChecked)
            {
                areatextBox.IsEnabled = true;
                filtercontr();
            }
            else
            {
                areatextBox.IsEnabled = false;
                filtercontr();
            }
        }

        private void mapareacheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)mapareacheckBox.IsChecked)
            {
                mapareatextBox.IsEnabled = true;
                filtermap();
            }
            else
            {
                mapareatextBox.IsEnabled = false;
                filtermap();
            }
        }

        private void unitcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)unitcheckBox.IsChecked)
            {
                unittextBox.IsEnabled = true;
                filtercontr();
            }
            else
            {
                unittextBox.IsEnabled = false;
                filtercontr();
            }
        }
        private void mapunitcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)mapunitcheckBox.IsChecked)
            {
                mapunittextBox.IsEnabled = true;
                filtermap();
            }
            else
            {
                mapunittextBox.IsEnabled = false;
                filtermap();
            }
        }

        private void oblcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)oblcheckBox.IsChecked)
            {
                oblcomboBox.IsEnabled = true;
                //gf.AddFilter(x => (Convert.ToInt64(((contract)x).koatuu) >= Convert.ToInt64(oblcomboBox.SelectedValue) && Convert.ToInt64(((contract)x).koatuu) <= (Convert.ToInt64(oblcomboBox.SelectedValue) + 199999999)));
                filtercontr();
            }
            else
            {
                oblcomboBox.IsEnabled = false;
                raycomboBox.IsEnabled = false;
                raycheckBox.Checked -= raycheckBox_Checked;
                raycheckBox.Unchecked -= raycheckBox_Checked;
                raycheckBox.IsChecked = false;
                raycheckBox.Checked += raycheckBox_Checked;
                raycheckBox.Unchecked += raycheckBox_Checked;
                silcomboBox.IsEnabled = false;
                silcheckBox.Checked -= silcheckBox_Checked;
                silcheckBox.Unchecked -= silcheckBox_Checked;
                silcheckBox.IsChecked = false;
                silcheckBox.Checked += silcheckBox_Checked;
                silcheckBox.Unchecked += silcheckBox_Checked;
                //gf.RemoveFilter(x => (Convert.ToInt64(((contract)x).koatuu) >= Convert.ToInt64(oblcomboBox.SelectedValue) && Convert.ToInt64(((contract)x).koatuu) <= (Convert.ToInt64(oblcomboBox.SelectedValue) + 199999999)));
                filtercontr();
            }
        }
        private void mapoblcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)mapoblcheckBox.IsChecked)
            {
                mapoblcomboBox.IsEnabled = true;
                filtermap();
            }
            else
            {
                mapoblcomboBox.IsEnabled = false;
                mapraycomboBox.IsEnabled = false;
                mapraycheckBox.Checked -= mapraycheckBox_Checked;
                mapraycheckBox.Unchecked -= mapraycheckBox_Checked;
                mapraycheckBox.IsChecked = false;
                mapraycheckBox.Checked += mapraycheckBox_Checked;
                mapraycheckBox.Unchecked += mapraycheckBox_Checked;
                mapsilcomboBox.IsEnabled = false;
                mapsilcheckBox.Checked -= mapsilcheckBox_Checked;
                mapsilcheckBox.Unchecked -= mapsilcheckBox_Checked;
                mapsilcheckBox.IsChecked = false;
                mapsilcheckBox.Checked += mapsilcheckBox_Checked;
                mapsilcheckBox.Unchecked += mapsilcheckBox_Checked;
                filtermap();
            }
        }
        private void raycheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)oblcheckBox.IsChecked && oblcomboBox.SelectedIndex != -1)
            {
                if ((bool)raycheckBox.IsChecked)
                {
                    raycomboBox.IsEnabled = true;
                }
                else
                {
                    raycomboBox.IsEnabled = false;
                    filtercontr();
                }
            }
            else
            {
                raycheckBox.Checked -= raycheckBox_Checked;
                raycheckBox.Unchecked -= raycheckBox_Checked;
                raycheckBox.IsChecked = false;
                raycheckBox.Checked += raycheckBox_Checked;
                raycheckBox.Unchecked += raycheckBox_Checked;
                silcomboBox.IsEnabled = false;
                silcheckBox.Checked -= silcheckBox_Checked;
                silcheckBox.Unchecked -= silcheckBox_Checked;
                silcheckBox.IsChecked = false;
                silcheckBox.Checked += silcheckBox_Checked;
                silcheckBox.Unchecked += silcheckBox_Checked;
                MessageBox.Show((string)Application.Current.Resources["m_seloblfirst"]);
            }
        }
        private void mapraycheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)mapoblcheckBox.IsChecked && mapoblcomboBox.SelectedIndex != -1)
            {
                if ((bool)mapraycheckBox.IsChecked)
                {
                    mapraycomboBox.IsEnabled = true;
                    filtermap();
                }
                else
                {
                    mapraycomboBox.IsEnabled = false;
                    filtermap();
                }
            }
            else
            {
                mapraycheckBox.Checked -= mapraycheckBox_Checked;
                mapraycheckBox.Unchecked -= mapraycheckBox_Checked;
                mapraycheckBox.IsChecked = false;
                mapraycheckBox.Checked += mapraycheckBox_Checked;
                mapraycheckBox.Unchecked += mapraycheckBox_Checked;
                mapraycomboBox.IsEnabled = false;
                mapsilcomboBox.IsEnabled = false;
                mapsilcheckBox.Checked -= mapsilcheckBox_Checked;
                mapsilcheckBox.Unchecked -= mapsilcheckBox_Checked;
                mapsilcheckBox.IsChecked = false;
                mapsilcheckBox.Checked += mapsilcheckBox_Checked;
                mapsilcheckBox.Unchecked += mapsilcheckBox_Checked;
                MessageBox.Show((string)Application.Current.Resources["m_seloblfirst"]);
            }
        }
        private void silcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)raycheckBox.IsChecked && raycomboBox.SelectedIndex != -1)
            {
                if ((bool)silcheckBox.IsChecked)
                {
                    silcomboBox.IsEnabled = true;
                    filtercontr();
                }
                else
                {
                    silcomboBox.IsEnabled = false;
                    filtercontr();
                }
            }
            else
            {
                silcheckBox.Checked -= silcheckBox_Checked;
                silcheckBox.Unchecked -= silcheckBox_Checked;
                silcheckBox.IsChecked = false;
                silcheckBox.Checked += silcheckBox_Checked;
                silcheckBox.Unchecked += silcheckBox_Checked;
                MessageBox.Show((string)Application.Current.Resources["m_selrayfirst"]);
            }
        }
        private void mapsilcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)mapraycheckBox.IsChecked && mapraycomboBox.SelectedIndex != -1)
            {
                if ((bool)mapsilcheckBox.IsChecked)
                {
                    mapsilcomboBox.IsEnabled = true;
                    filtermap();
                }
                else
                {
                    mapsilcomboBox.IsEnabled = false;
                    filtermap();
                }
            }
            else
            {
                mapsilcheckBox.Checked -= mapsilcheckBox_Checked;
                mapsilcheckBox.Unchecked -= mapsilcheckBox_Checked;
                mapsilcheckBox.IsChecked = false;
                mapsilcheckBox.Checked += mapsilcheckBox_Checked;
                mapsilcheckBox.Unchecked += mapsilcheckBox_Checked;
                MessageBox.Show((string)Application.Current.Resources["m_selrayfirst"]);
            }
        }
        private void leasercomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contractsEntities != null)
            {
                filtercontr();
            }
        }

        private void mapleasercomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filtermap();
        }

        private void typecomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contractsEntities != null)
            {
                filtercontr();
            }
        }
        private void maptypecomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filtermap();
        }

        private void statuscomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contractsEntities != null)
            {
                filtercontr();
            }
        }
        private void mapstatuscomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filtermap();
        }

        private void formownercomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contractsEntities != null)
            {
                filtercontr();
            }
        }
        private void mapformownercomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filtermap();
        }
        private void formunitcomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contractsEntities != null)
            {
                filtercontr();
            }
        }
        private void mapformunitcomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filtermap();
        }
        private void formusecomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contractsEntities != null)
            {
                filtercontr();
            }
        }
        private void mapformusecomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filtermap();
        }
        private void datacontrsdatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contractsEntities != null)
            {
                filtercontr();
            }
        }
        private void mapdatacontrsdatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            filtermap();
        }
        private void datacontrpodatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contractsEntities != null)
            {
                filtercontr();
            }
        }
        private void mapdatacontrpodatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            filtermap();
        }
        private void dataregsdatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contractsEntities != null)
            {
                filtercontr();
            }
        }
        private void mapdataregsdatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            filtermap();
        }
        private void dataregpodatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contractsEntities != null)
            {
                filtercontr();
            }
        }
        private void mapdataregpodatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            filtermap();
        }


        private void areatextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (contractsEntities != null)
            {
                filtercontr();
            }
        }

        private void unittextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (contractsEntities != null)
            {
                filtercontr();
            }
        }

        private void mapunittextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            filtermap();
        }

        private void mapareatextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            filtermap();
        }
        private void qcadnumt4extBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }
        private void repcontrnumcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)repcontrnumcheckBox.IsChecked)
            {
                repcontrnumtextBox.IsEnabled = true;
                refshtable();
            }
            else
            {
                repcontrnumtextBox.IsEnabled = false;
                refshtable();
            }
        }

        private void repdatecontrcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)repdatecontrcheckBox.IsChecked)
            {
                repdatecontrtextBox.IsEnabled = true;
                refshtable();
            }
            else
            {
                repdatecontrtextBox.IsEnabled = false;
                refshtable();
            }
        }
        private void repdateregcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)repdateregcheckBox.IsChecked)
            {
                repdateregtextBox.IsEnabled = true;
                refshtable();
            }
            else
            {
                repdateregtextBox.IsEnabled = false;
                refshtable();
            }
        }

        private void replessorcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)replessorcheckBox.IsChecked)
            {
                replessortextBox.IsEnabled = true;
                refshtable();
            }
            else
            {
                replessortextBox.IsEnabled = false;
                refshtable();
            }
        }

        private void repleasercheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)repleasercheckBox.IsChecked)
            {
                repleasertextBox.IsEnabled = true;
                refshtable();
            }
            else
            {
                repleasertextBox.IsEnabled = false;
                refshtable();
            }
        }

        private void repcadnumcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)repcadnumcheckBox.IsChecked)
            {
                repcadnumtextBox.IsEnabled = true;
                refshtable();
            }
            else
            {
                repcadnumtextBox.IsEnabled = false;
                refshtable();
            }
        }

        private void repsquarecheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)repsquarecheckBox.IsChecked)
            {
                repsquaretextBox.IsEnabled = true;
                refshtable();
            }
            else
            {
                repsquaretextBox.IsEnabled = false;
                refshtable();
            }
        }

        private void reptermleasecheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)reptermleasecheckBox.IsChecked)
            {
                reptermleasetextBox.IsEnabled = true;
                refshtable();
            }
            else
            {
                reptermleasetextBox.IsEnabled = false;
                refshtable();
            }
        }

        private void repcontrstatcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)repcontrstatcheckBox.IsChecked)
            {
                repcontrstattextBox.IsEnabled = true;
                refshtable();
            }
            else
            {
                repcontrstattextBox.IsEnabled = false;
                refshtable();
            }
        }

        private void reptypecontrcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)reptypecontrcheckBox.IsChecked)
            {
                reptypecontrtextBox.IsEnabled = true;
                refshtable();
            }
            else
            {
                reptypecontrtextBox.IsEnabled = false;
                refshtable();
            }
        }

        private void repformownercheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)repformownercheckBox.IsChecked)
            {
                repformownertextBox.IsEnabled = true;
                refshtable();
            }
            else
            {
                repformownertextBox.IsEnabled = false;
                refshtable();
            }
        }

        private void repformusecheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)repformusecheckBox.IsChecked)
            {
                repformusetextBox.IsEnabled = true;
                refshtable();
            }
            else
            {
                repformusetextBox.IsEnabled = false;
                refshtable();
            }
        }

        private void repformunitcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)repformunitcheckBox.IsChecked)
            {
                repformunittextBox.IsEnabled = true;
                refshtable();
            }
            else
            {
                repformunittextBox.IsEnabled = false;
                refshtable();
            }
        }

        private void repkoatuucheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)repkoatuucheckBox.IsChecked)
            {
                repkoatuutextBox.IsEnabled = true;
                refshtable();
            }
            else
            {
                repkoatuutextBox.IsEnabled = false;
                refshtable();
            }
        }

        private void repareanumcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)repareanumcheckBox.IsChecked)
            {
                repareanumtextBox.IsEnabled = true;
                refshtable();
            }
            else
            {
                repareanumtextBox.IsEnabled = false;
                refshtable();
            }
        }

        private void repunitnumcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)repunitnumcheckBox.IsChecked)
            {
                repunitnumtextBox.IsEnabled = true;
                refshtable();
            }
            else
            {
                repunitnumtextBox.IsEnabled = false;
                refshtable();
            }
        }

        private void repnotescheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)repnotescheckBox.IsChecked)
            {
                repnotestextBox.IsEnabled = true;
                refshtable();
            }
            else
            {
                repnotestextBox.IsEnabled = false;
                refshtable();
            }
        }

        private void repcontrnumtextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (repcontrnumtextBox.Text.Length == 0)
            {
                repcontrnumtextBox.Text = "5";
            }
            if ((bool)repcontrnumcheckBox.IsChecked)
            {
                IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repcontrnum", repcontrnumtextBox.Text);
                refshtable();
            }
        }

        private void repdatecontrtextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (repdatecontrtextBox.Text.Length == 0)
            {
                repdatecontrtextBox.Text = "5";
            }
            if ((bool)repdatecontrcheckBox.IsChecked)
            {
                IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repdatecontr", repdatecontrtextBox.Text);
                refshtable();
            }
        }

        private void repdateregtextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (repdateregtextBox.Text.Length == 0)
            {
                repdateregtextBox.Text = "5";
            }
            if ((bool)repdateregcheckBox.IsChecked)
            {
                IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repdatereg", repdateregtextBox.Text);
                refshtable();
            }
        }

        private void replessortextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (replessortextBox.Text.Length == 0)
            {
                replessortextBox.Text = "5";
            }
            if ((bool)replessorcheckBox.IsChecked)
            {
                IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "replessor", replessortextBox.Text);
                refshtable();
            }
        }

        private void repleasertextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (repleasertextBox.Text.Length == 0)
            {
                repleasertextBox.Text = "5";
            }
            if ((bool)repleasercheckBox.IsChecked)
            {
                IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repleaser", repleasertextBox.Text);
                refshtable();
            }
        }

        private void repcadnumtextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (repcadnumtextBox.Text.Length == 0)
            {
                repcadnumtextBox.Text = "5";
            }
            if ((bool)repcadnumcheckBox.IsChecked)
            {
                IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repcadnum", repcadnumtextBox.Text);
                refshtable();
            }
        }

        private void repsquaretextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (repsquaretextBox.Text.Length == 0)
            {
                repsquaretextBox.Text = "5";
            }
            if ((bool)repsquarecheckBox.IsChecked)
            {
                IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repsquare", repsquaretextBox.Text);
                refshtable();
            }
        }

        private void reptermleasetextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (reptermleasetextBox.Text.Length == 0)
            {
                reptermleasetextBox.Text = "5";
            }
            if ((bool)reptermleasecheckBox.IsChecked)
            {
                IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "reptermlease", reptermleasetextBox.Text);
                refshtable();
            }
        }

        private void repcontrstattextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (repcontrstattextBox.Text.Length == 0)
            {
                repcontrstattextBox.Text = "5";
            }
            if ((bool)repcontrstatcheckBox.IsChecked)
            {
                IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repcontrstat", repcontrstattextBox.Text);
                refshtable();
            }
        }

        private void reptypecontrtextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (reptypecontrtextBox.Text.Length == 0)
            {
                reptypecontrtextBox.Text = "5";
            }
            if ((bool)reptypecontrcheckBox.IsChecked)
            {
                IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "reptypecontr", reptypecontrtextBox.Text);
                refshtable();
            }
        }

        private void repformownertextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (repformownertextBox.Text.Length == 0)
            {
                repformownertextBox.Text = "5";
            }
            if ((bool)repformownercheckBox.IsChecked)
            {
                IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repformowner", repformownertextBox.Text);
                refshtable();
            }
        }

        private void repformusetextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (repformusetextBox.Text.Length == 0)
            {
                repformusetextBox.Text = "5";
            }
            if ((bool)repformusecheckBox.IsChecked)
            {
                IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repformuse", repformusetextBox.Text);
                refshtable();
            }
        }

        private void repformunittextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (repformunittextBox.Text.Length == 0)
            {
                repformunittextBox.Text = "5";
            }
            if ((bool)repformunitcheckBox.IsChecked)
            {
                IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repformunit", repformunittextBox.Text);
                refshtable();
            }
        }

        private void repkoatuutextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (repkoatuutextBox.Text.Length == 0)
            {
                repkoatuutextBox.Text = "5";
            }
            if ((bool)repkoatuucheckBox.IsChecked)
            {
                IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repkoatuu", repkoatuutextBox.Text);
                refshtable();
            }
        }

        private void repareanumtextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (repareanumtextBox.Text.Length == 0)
            {
                repareanumtextBox.Text = "5";
            }
            if ((bool)repareanumcheckBox.IsChecked)
            {
                IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repareanum", repareanumtextBox.Text);
                refshtable();
            }
        }

        private void repunitnumtextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (repunitnumtextBox.Text.Length == 0)
            {
                repunitnumtextBox.Text = "5";
            }
            if ((bool)repunitnumcheckBox.IsChecked)
            {
                IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repunitnum", repunitnumtextBox.Text);
                refshtable();
            }
        }

        private void repnotestextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (repnotestextBox.Text.Length == 0)
            {
                repnotestextBox.Text = "5";
            }
            if ((bool)repnotescheckBox.IsChecked)
            {
                IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "colwidth", "repnotes", repnotestextBox.Text);
                refshtable();
            }
        }
        private void reloadmenuItem_Click(object sender, RoutedEventArgs e)
        {
            webControl1.Reload(false);
            webView1.Reload(false);
        }

        private void webControl1_ShowContextMenu(object sender, Awesomium.Core.ContextMenuEventArgs e)
        {
            if (webControl1.ContextMenu != null)
            {
                webControl1.ContextMenu.IsOpen = true;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if ((sender as Fluent.ToggleButton).Name == "contrmoreparam")
            {
                if ((bool)(sender as Fluent.ToggleButton).IsChecked)
                {
                    ribbon.SelectedTabIndex = 3;
                }
                else
                {
                    datacontscheckBox.Checked -= datacontscheckBox_Checked;
                    datacontpocheckBox.Checked -= datacontpocheckBox_Checked;
                    dataregscheckBox.Checked -= dataregscheckBox_Checked;
                    dataregpocheckBox.Checked -= dataregpocheckBox_Checked;
                    oblcheckBox.Checked -= oblcheckBox_Checked;
                    raycheckBox.Checked -= raycheckBox_Checked;
                    silcheckBox.Checked -= silcheckBox_Checked;
                    typecheckBox.Checked -= typecheckBox_Checked;
                    areacheckBox.Checked -= areacheckBox_Checked;
                    unitcheckBox.Checked -= unitcheckBox_Checked;

                    datacontscheckBox.IsChecked = false;
                    datacontpocheckBox.IsChecked = false;
                    dataregscheckBox.IsChecked = false;
                    dataregpocheckBox.IsChecked = false;
                    oblcheckBox.IsChecked = false;
                    raycheckBox.IsChecked = false;
                    silcheckBox.IsChecked = false;
                    typecheckBox.IsChecked = false;
                    areacheckBox.IsChecked = false;
                    unitcheckBox.IsChecked = false;

                    datacontscheckBox.Checked += datacontscheckBox_Checked;
                    datacontpocheckBox.Checked += datacontpocheckBox_Checked;
                    dataregscheckBox.Checked += dataregscheckBox_Checked;
                    dataregpocheckBox.Checked += dataregpocheckBox_Checked;
                    oblcheckBox.Checked += oblcheckBox_Checked;
                    raycheckBox.Checked += raycheckBox_Checked;
                    silcheckBox.Checked += silcheckBox_Checked;
                    typecheckBox.Checked += typecheckBox_Checked;
                    areacheckBox.Checked += areacheckBox_Checked;
                    unitcheckBox.Checked += unitcheckBox_Checked;
                    filtercontr();
                    if (!(bool)mapmoreparam.IsChecked)
                    {
                        maptabfil.Visibility = Visibility.Collapsed;
                    }
                    if (!(bool)contrmoreparam.IsChecked)
                    {
                        contrtabfil.Visibility = Visibility.Collapsed;
                    }
                    if (!(bool)contrmoreparam.IsChecked && !(bool)mapmoreparam.IsChecked)
                    {
                        filGroup.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                if ((bool)(sender as Fluent.ToggleButton).IsChecked)
                {
                    ribbon.SelectedTabIndex = 4;
                }
                else
                {

                    mapdatacontscheckBox.Checked -= mapdatacontscheckBox_Checked;
                    mapdatacontpocheckBox.Checked -= mapdatacontpocheckBox_Checked;
                    mapdataregscheckBox.Checked -= mapdataregscheckBox_Checked;
                    mapdataregpocheckBox.Checked -= mapdataregpocheckBox_Checked;
                    mapoblcheckBox.Checked -= mapoblcheckBox_Checked;
                    mapraycheckBox.Checked -= mapraycheckBox_Checked;
                    mapsilcheckBox.Checked -= mapsilcheckBox_Checked;

                    mapdatacontscheckBox.IsChecked = false;
                    mapdatacontpocheckBox.IsChecked = false;
                    mapdataregscheckBox.IsChecked = false;
                    mapdataregpocheckBox.IsChecked = false;
                    mapoblcheckBox.IsChecked = false;
                    mapraycheckBox.IsChecked = false;
                    mapsilcheckBox.IsChecked = false;

                    mapdatacontscheckBox.Checked += mapdatacontscheckBox_Checked;
                    mapdatacontpocheckBox.Checked += mapdatacontpocheckBox_Checked;
                    mapdataregscheckBox.Checked += mapdataregscheckBox_Checked;
                    mapdataregpocheckBox.Checked += mapdataregpocheckBox_Checked;
                    mapoblcheckBox.Checked += mapoblcheckBox_Checked;
                    mapraycheckBox.Checked += mapraycheckBox_Checked;
                    mapsilcheckBox.Checked += mapsilcheckBox_Checked;
                    filtermap();
                    if (!(bool)mapmoreparam.IsChecked)
                    {
                        maptabfil.Visibility = Visibility.Collapsed;
                    }
                    if (!(bool)contrmoreparam.IsChecked)
                    {
                        contrtabfil.Visibility = Visibility.Collapsed;
                    }
                    if (!(bool)contrmoreparam.IsChecked && !(bool)mapmoreparam.IsChecked)
                    {
                        filGroup.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        Thread psqlthread;
        private void psqlcleardb()
        {
            ApplyEffect(this);
            MessageBoxResult messresult = new MessageBoxResult();
            if (Application.Current.Dispatcher.CheckAccess())
            {
                messresult = MessageBox.Show((string)Application.Current.Resources["m_confirmcleardb"],
                (string)Application.Current.Resources["m_confirm"], MessageBoxButton.YesNo, MessageBoxImage.Question);
            }
            else
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                    messresult = MessageBox.Show((string)Application.Current.Resources["m_confirmcleardb"],
                (string)Application.Current.Resources["m_confirm"], MessageBoxButton.YesNo, MessageBoxImage.Question);
                }));
            }
            if (messresult == MessageBoxResult.Yes)
            {
                try
                {
                    using (var conn = new NpgsqlConnection("Server=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server") + ";Port=" +
                        IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port") + ";User Id=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user") +
                        ";Password=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "pass") + ";Database=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "database") + ";"))
                    {
                        conn.Open();
                        using (var cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = "truncate contracts CASCADE;";
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }
                catch (Exception e)
                {
                    if (Application.Current.Dispatcher.CheckAccess())
                    {
                        MessageBox.Show(Application.Current.MainWindow, e.Message, "Errr on truncate contracts CASCADE!");
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                            MessageBox.Show(Application.Current.MainWindow, e.Message, "Errr on truncate contracts CASCADE!");
                        }));
                    }
                }
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    refreshwin();
                }));
            }
            ClearEffect(this);
        }
        private void psqlbackupdb()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".sql";
            dlg.Filter = "SQL Files (*.sql)|*.sql";
            dlg.FileName = IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server") + "_" +
                        IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "database") + "_" +
                        DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".sql";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string batFilePath = AppDomain.CurrentDomain.BaseDirectory + @"psqlexe.bat";
                if (!File.Exists(batFilePath))
                {
                    using (FileStream fs = File.Create(batFilePath))
                    {
                        fs.Close();
                    }
                }
                using (StreamWriter sw = new StreamWriter(batFilePath))
                {
                    sw.WriteLine('@' + "chcp 65001");
                    sw.WriteLine('@' + "echo Wait for backup database ...");
                    sw.WriteLine('@' + AppDomain.CurrentDomain.BaseDirectory + @"psql_utils\pg_dump.exe -h " +
                        IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server")
                        + " -p " + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port") +
                        "-C -U " + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user") +
                        " " + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "database") + " > "
                        + '"' + dlg.FileName + '"');
                    sw.WriteLine('@' + "echo Done!");
                    sw.WriteLine('@' + "pause");
                }
                Process process = Process.Start(batFilePath);
                process.WaitForExit();
                process.Close();
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"psqlexe.bat"))
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"psqlexe.bat");
                }
            }
        }
        private void psqlrestoredb()
        {
            ApplyEffect(this);
            MessageBoxResult messresult = new MessageBoxResult();
            if (Application.Current.Dispatcher.CheckAccess())
            {
                messresult = MessageBox.Show(Application.Current.MainWindow, (string)Application.Current.Resources["m_confirmrestoredb"],
                   (string)Application.Current.Resources["m_confirm"], MessageBoxButton.YesNo, MessageBoxImage.Question);
            }
            else
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                    messresult = MessageBox.Show(Application.Current.MainWindow, (string)Application.Current.Resources["m_confirmrestoredb"],
(string)Application.Current.Resources["m_confirm"], MessageBoxButton.YesNo, MessageBoxImage.Question);
                }));
            }
            if (messresult == MessageBoxResult.Yes)
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.DefaultExt = ".sql";
                dlg.Filter = "SQL Files (*.sql)|*.sql";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    string batFilePath = AppDomain.CurrentDomain.BaseDirectory + @"psqlexe.bat";
                    if (!File.Exists(batFilePath))
                    {
                        using (FileStream fs = File.Create(batFilePath))
                        {
                            fs.Close();
                        }
                    }
                    using (StreamWriter sw = new StreamWriter(batFilePath))
                    {
                        sw.WriteLine('@' + "chcp 65001");
                        sw.WriteLine('@' + "echo Get all connected users ...");
                        sw.WriteLine('@' + AppDomain.CurrentDomain.BaseDirectory + @"psql_utils\psql.exe -h " +
                            IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server")
                            + " -p " + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port") +
                            " -U " + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user") +
                            " -c " + '"' + "SELECT  * FROM pg_stat_activity WHERE datname = '" +
                            IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "database") + "';" + '"');
                        sw.WriteLine('@' + "echo Done!");
                        sw.WriteLine('@' + "echo Wait for disconnect all users from database ...");
                        sw.WriteLine('@' + AppDomain.CurrentDomain.BaseDirectory + @"psql_utils\psql.exe -h " +
                            IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server")
                            + " -p " + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port") +
                            " -U " + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user") +
                            " -c " + '"' + "SELECT pg_terminate_backend (pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = '" +
                            IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "database") + "';" + '"');
                        sw.WriteLine('@' + "echo Done!");
                        sw.WriteLine('@' + "echo Wait for drop database ...");
                        sw.WriteLine('@' + AppDomain.CurrentDomain.BaseDirectory + @"psql_utils\psql.exe -h " +
                            IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server")
                            + " -p " + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port") +
                            " -U " + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user") +
                            " -c " + '"' + "DROP DATABASE " + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "database") + ";" + '"');
                        sw.WriteLine('@' + "echo Done!");
                        sw.WriteLine('@' + "echo Wait for create database ...");
                        sw.WriteLine('@' + AppDomain.CurrentDomain.BaseDirectory + @"psql_utils\psql.exe -h " +
                            IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server")
                            + " -p " + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port") +
                            " -U " + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user") +
                            " -c " + '"' + "CREATE DATABASE " + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "database") +
                            " WITH ENCODING='UTF8' CONNECTION LIMIT=-1;" + '"');
                        sw.WriteLine('@' + "echo Done!");
                        sw.WriteLine('@' + "echo Wait for restore database ...");
                        sw.WriteLine('@' + AppDomain.CurrentDomain.BaseDirectory + @"psql_utils\psql.exe -h " +
                            IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server")
                            + " -p " + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port") +
                            " -U " + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user") +
                            " -d " + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "database") + " < "
                            + '"' + dlg.FileName + '"');
                        sw.WriteLine('@' + "echo Done!");
                        sw.WriteLine('@' + "pause");
                    }
                    Process process = Process.Start(batFilePath);
                    process.WaitForExit();
                    process.Close();
                    try
                    {
                        using (var conn = new NpgsqlConnection("Server=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server") + ";Port=" +
                            IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port") + ";User Id=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user") +
                            ";Password=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "pass") + ";Database=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "database") + ";"))
                        {
                            conn.Open();
                            using (var cmd = new NpgsqlCommand())
                            {
                                cmd.Connection = conn;
                                cmd.CommandText = "select version();";
                                cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string exp = ex.Message;
                    }
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"psqlexe.bat"))
                    {
                        File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"psqlexe.bat");
                    }
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        refreshwin();
                    }));
                }
            }
            ClearEffect(this);
        }
        private void cleardbbutton_Click(object sender, RoutedEventArgs e)
        {
            psqlthread = new Thread(psqlcleardb);
            psqlthread.Start();
        }

        private void backupdbbutton_Click(object sender, RoutedEventArgs e)
        {
            psqlthread = new Thread(psqlbackupdb);
            psqlthread.Start();
        }

        private void restoredbbutton_Click(object sender, RoutedEventArgs e)
        {
            psqlthread = new Thread(psqlrestoredb);
            psqlthread.Start();
        }
        private void rinshfirsttime()
        {
            NpgsqlConnection conn = new NpgsqlConnection("Server=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server") +
                ";Port=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port") +
                ";User Id=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user") +
                ";Password=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "pass") +
                ";Database=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "database") + ";");
            try
            {
                string serverinfo = "";
                string taberr = "";
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select version();";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                serverinfo = reader[i].ToString();
                            }
                        }
                    }
                    //проверка бд
                    try
                    {
                        cmd.CommandText = "select count(1) from ck63;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            }
                        }
                    }
                    catch (PostgresException ex)
                    {
                        taberr = taberr + ex.Message + "\r\n";
                    }
                    try
                    {
                        cmd.CommandText = "select count(1) from contracts;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            }
                        }
                    }
                    catch (PostgresException ex)
                    {
                        taberr = taberr + ex.Message + "\r\n";
                    }
                    try
                    {
                        cmd.CommandText = "select count(1) from leaserfiz;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            }
                        }
                    }
                    catch (PostgresException ex)
                    {
                        taberr = taberr + ex.Message + "\r\n";
                    }
                    try
                    {
                        cmd.CommandText = "select count(1) from leaseryur;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            }
                        }
                    }
                    catch (PostgresException ex)
                    {
                        taberr = taberr + ex.Message + "\r\n";
                    }
                    try
                    {
                        cmd.CommandText = "select count(1) from lessorfiz;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            }
                        }
                    }
                    catch (PostgresException ex)
                    {
                        taberr = taberr + ex.Message + "\r\n";
                    }
                    try
                    {
                        cmd.CommandText = "select count(1) from lessoryur;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            }
                        }
                    }
                    catch (PostgresException ex)
                    {
                        taberr = taberr + ex.Message + "\r\n";
                    }
                    try
                    {
                        cmd.CommandText = "select count(1) from obl;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (Convert.ToInt32(reader[i]) != 25)
                                    {
                                        taberr = taberr + (string)Application.Current.Resources["m_badtabl"] + "obl!" + "\r\n";
                                    }
                                }
                            }
                        }
                    }
                    catch (PostgresException ex)
                    {
                        taberr = taberr + ex.Message + "\r\n";
                    }
                    try
                    {
                        cmd.CommandText = "select count(1) from rayon;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (Convert.ToInt32(reader[i]) != 746)
                                    {
                                        taberr = taberr + (string)Application.Current.Resources["m_badtabl"] + "rayon!" + "\r\n";
                                    }
                                }
                            }
                        }
                    }
                    catch (PostgresException ex)
                    {
                        taberr = taberr + ex.Message + "\r\n";
                    }
                    try
                    {
                        cmd.CommandText = "select count(1) from sil;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (Convert.ToInt32(reader[i]) != 14852)
                                    {
                                        taberr = taberr + (string)Application.Current.Resources["m_badtabl"] + "sil!" + "\r\n";
                                    }
                                }
                            }
                        }
                    }
                    catch (PostgresException ex)
                    {
                        taberr = taberr + ex.Message + "\r\n";
                    }
                    try
                    {
                        cmd.CommandText = "select count(1) from wgs84;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            }
                        }
                    }
                    catch (PostgresException ex)
                    {
                        taberr = taberr + ex.Message + "\r\n";
                    }
                    finally
                    {
                        conn.Close();
                    }
                    if (taberr.Length > 0)
                    {
                        ApplyEffect(this);
                        if (Application.Current.Dispatcher.CheckAccess())
                        {
                            MessageBox.Show(Application.Current.MainWindow, (string)FindResource("m_badstruktdb") + "\r\nErrorInfo:\r\n" + taberr, (string)FindResource("m_Error"));
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                                MessageBox.Show(Application.Current.MainWindow, (string)FindResource("m_badstruktdb") + "\r\nErrorInfo:\r\n" + taberr, (string)FindResource("m_Error"));
                            }));
                        }
                        ClearEffect(this);
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(new Action(() => {
                            //загрузка данных бд
                            contractsList = bdfunc.selcontr();
                            contractsEntities.Reset(contractsList);
                            contractsView = (ListCollectionView)CollectionViewSource.GetDefaultView(contractsEntities);

                            if (leasercomboBox.Items.Count > 0 && mapleasercomboBox.Items.Count > 0)
                            {
                                string selleaser = leasercomboBox.SelectedItem.ToString();
                                string selmapleaser = mapleasercomboBox.SelectedItem.ToString();
                                foreach (contract contr in contractsList)
                                {
                                    if (!leasercomboBox.Items.Contains(contr.leaser.ToLower()))
                                    {
                                        leasercomboBox.Items.Add(contr.leaser.ToLower());
                                        mapleasercomboBox.Items.Add(contr.leaser.ToLower());
                                    }
                                }
                                int idselcb = leasercomboBox.Items.IndexOf(selleaser);
                                int idselmapcb = mapleasercomboBox.Items.IndexOf(selmapleaser);
                                if (idselcb != -1)
                                {
                                    leasercomboBox.SelectedIndex = idselcb;
                                }
                                else
                                {
                                    leasercomboBox.SelectedIndex = 0;
                                }
                                if (idselmapcb != -1)
                                {
                                    mapleasercomboBox.SelectedIndex = idselmapcb;
                                }
                                else
                                {
                                    mapleasercomboBox.SelectedIndex = 0;
                                }
                            }
                            else
                            {
                                foreach (contract contr in contractsList)
                                {
                                    if (!leasercomboBox.Items.Contains(contr.leaser.ToLower()))
                                    {
                                        leasercomboBox.Items.Add(contr.leaser.ToLower());
                                        mapleasercomboBox.Items.Add(contr.leaser.ToLower());
                                    }
                                }
                                leasercomboBox.SelectedIndex = 0;
                                mapleasercomboBox.SelectedIndex = 0;
                            }
                            filtercontr();
                            if (contractsList.Count > 0)
                            {
                                List<string> listid = new List<string>();
                                foreach (contract contr in contractsList)
                                {
                                    listid.Add(contr.id.ToString());
                                }
                                allcoords = bdfunc.selcoords(listid, "wgs84");
                            }
                            webControl1.ExecuteJavascript("savemapstate()");
                            manualref = true;
                            webView1.Reload(true);
                            webControl1.Reload(true);
                        }));
                    }
                }
            }
            catch (PostgresException ex)
            {
                ApplyEffect(this);
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    MessageBox.Show(Application.Current.MainWindow, (string)FindResource("m_bdless") + "\r\nErrorInfo:\r\n" + ex.Message, (string)FindResource("m_Error"));
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                        MessageBox.Show(Application.Current.MainWindow, (string)FindResource("m_bdless") + "\r\nErrorInfo:\r\n" + ex.Message, (string)FindResource("m_Error"));
                    }));
                }
                ClearEffect(this);
            }
            catch (TimeoutException ex)
            {
                ApplyEffect(this);
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    MessageBox.Show(Application.Current.MainWindow, (string)FindResource("m_bdless") + "\r\nErrorInfo:\r\n" + ex.Message, (string)FindResource("m_Error"));
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                        MessageBox.Show(Application.Current.MainWindow, (string)FindResource("m_bdless") + "\r\nErrorInfo:\r\n" + ex.Message, (string)FindResource("m_Error"));
                    }));
                }
                ClearEffect(this);
            }
            catch (SocketException ex)
            {
                ApplyEffect(this);
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    MessageBox.Show(Application.Current.MainWindow, (string)FindResource("m_bdless") + "\r\nErrorInfo:\r\n" + ex.Message, (string)FindResource("m_Error"));
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                        MessageBox.Show(Application.Current.MainWindow, (string)FindResource("m_bdless") + "\r\nErrorInfo:\r\n" + ex.Message, (string)FindResource("m_Error"));
                    }));
                }
                ClearEffect(this);
            }
            finally
            {
                conn.Close();
            }
        }
        private void rinsh()
        {
            NpgsqlConnection conn = new NpgsqlConnection("Server=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server") +
                ";Port=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port") +
                ";User Id=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user") +
                ";Password=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "pass") +
                ";Database=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "database") + ";");
            try
            {
                string serverinfo = "";
                string taberr = "";
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select version();";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                serverinfo = reader[i].ToString();
                            }
                        }
                    }
                    if (Application.Current.Dispatcher.CheckAccess())
                    {
                        MessageBox.Show(Application.Current.MainWindow, (string)Application.Current.Resources["m_connserversucss"] + "\r\nInfo: " + serverinfo);
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                            MessageBox.Show(Application.Current.MainWindow, (string)Application.Current.Resources["m_connserversucss"] + "\r\nInfo: " + serverinfo);
                        }));
                    }
                    //проверка бд
                    try
                    {
                        cmd.CommandText = "select count(1) from ck63;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            }
                        }
                    }
                    catch (PostgresException ex)
                    {
                        taberr = taberr + ex.Message + "\r\n";
                    }
                    try
                    {
                        cmd.CommandText = "select count(1) from contracts;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            }
                        }
                    }
                    catch (PostgresException ex)
                    {
                        taberr = taberr + ex.Message + "\r\n";
                    }
                    try
                    {
                        cmd.CommandText = "select count(1) from leaserfiz;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            }
                        }
                    }
                    catch (PostgresException ex)
                    {
                        taberr = taberr + ex.Message + "\r\n";
                    }
                    try
                    {
                        cmd.CommandText = "select count(1) from leaseryur;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            }
                        }
                    }
                    catch (PostgresException ex)
                    {
                        taberr = taberr + ex.Message + "\r\n";
                    }
                    try
                    {
                        cmd.CommandText = "select count(1) from lessorfiz;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            }
                        }
                    }
                    catch (PostgresException ex)
                    {
                        taberr = taberr + ex.Message + "\r\n";
                    }
                    try
                    {
                        cmd.CommandText = "select count(1) from lessoryur;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            }
                        }
                    }
                    catch (PostgresException ex)
                    {
                        taberr = taberr + ex.Message + "\r\n";
                    }
                    try
                    {
                        cmd.CommandText = "select count(1) from obl;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (Convert.ToInt32(reader[i]) != 25)
                                    {
                                        taberr = taberr + (string)FindResource("m_badobldata") + "\r\n";
                                    }
                                }
                            }
                        }
                    }
                    catch (PostgresException ex)
                    {
                        taberr = taberr + ex.Message + "\r\n";
                    }
                    try
                    {
                        cmd.CommandText = "select count(1) from rayon;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (Convert.ToInt32(reader[i]) != 746)
                                    {
                                        taberr = taberr + (string)FindResource("m_badraydata") + "\r\n";
                                    }
                                }
                            }
                        }
                    }
                    catch (PostgresException ex)
                    {
                        taberr = taberr + ex.Message + "\r\n";
                    }
                    try
                    {
                        cmd.CommandText = "select count(1) from sil;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (Convert.ToInt32(reader[i]) != 14852)
                                    {
                                        taberr = taberr + (string)FindResource("m_badsildata") + "\r\n";
                                    }
                                }
                            }
                        }
                    }
                    catch (PostgresException ex)
                    {
                        taberr = taberr + ex.Message + "\r\n";
                    }
                    try
                    {
                        cmd.CommandText = "select count(1) from wgs84;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            }
                        }
                    }
                    catch (PostgresException ex)
                    {
                        taberr = taberr + ex.Message + "\r\n";
                    }
                    finally
                    {
                        conn.Close();
                    }
                    if (taberr.Length > 0)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            contractsList.Clear();
                            contractsEntities.Reset(contractsList);
                            contractsView = (ListCollectionView)CollectionViewSource.GetDefaultView(contractsEntities);
                            webControl1.Reload(true);
                            webView1.Reload(true);
                        }));
                        MessageBox.Show((string)Application.Current.Resources["m_struktdbnotpass"] + "\r\nErrorInfo:\r\n" + taberr, (string)FindResource("m_Error"));
                    }
                    else
                    {
                        MessageBox.Show((string)Application.Current.Resources["m_struktdbpass"]);
                        Dispatcher.BeginInvoke(new Action(() => {

                            contractsList = bdfunc.selcontr();
                            contractsEntities.Reset(contractsList);
                            contractsView = (ListCollectionView)CollectionViewSource.GetDefaultView(contractsEntities);

                            if (leasercomboBox.Items.Count > 0 && mapleasercomboBox.Items.Count > 0)
                            {
                                string selleaser = leasercomboBox.SelectedItem.ToString();
                                string selmapleaser = mapleasercomboBox.SelectedItem.ToString();
                                foreach (contract contr in contractsList)
                                {
                                    if (!leasercomboBox.Items.Contains(contr.leaser.ToLower()))
                                    {
                                        leasercomboBox.Items.Add(contr.leaser.ToLower());
                                        mapleasercomboBox.Items.Add(contr.leaser.ToLower());
                                    }
                                }
                                int idselcb = leasercomboBox.Items.IndexOf(selleaser);
                                int idselmapcb = mapleasercomboBox.Items.IndexOf(selmapleaser);
                                if (idselcb != -1)
                                {
                                    leasercomboBox.SelectedIndex = idselcb;
                                }
                                else
                                {
                                    leasercomboBox.SelectedIndex = 0;
                                }
                                if (idselmapcb != -1)
                                {
                                    mapleasercomboBox.SelectedIndex = idselmapcb;
                                }
                                else
                                {
                                    mapleasercomboBox.SelectedIndex = 0;
                                }
                            }
                            else
                            {
                                foreach (contract contr in contractsList)
                                {
                                    if (!leasercomboBox.Items.Contains(contr.leaser.ToLower()))
                                    {
                                        leasercomboBox.Items.Add(contr.leaser.ToLower());
                                        mapleasercomboBox.Items.Add(contr.leaser.ToLower());
                                    }
                                }
                                leasercomboBox.SelectedIndex = 0;
                                mapleasercomboBox.SelectedIndex = 0;
                            }
                            filtercontr();
                            if (contractsList.Count > 0)
                            {
                                List<string> listid = new List<string>();
                                foreach (contract contr in contractsList)
                                {
                                    listid.Add(contr.id.ToString());
                                }
                                allcoords = bdfunc.selcoords(listid, "wgs84");
                            }
                            webControl1.ExecuteJavascript("savemapstate()");
                            manualref = true;
                            webView1.Reload(true);
                            webControl1.Reload(true);
                        }));
                    }
                }
            }
            catch (PostgresException ex)
            {
                ApplyEffect(this);
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    contractsList.Clear();
                    contractsEntities.Reset(contractsList);
                    contractsView = (ListCollectionView)CollectionViewSource.GetDefaultView(contractsEntities);
                    webControl1.Reload(true);
                }));
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    MessageBox.Show(Application.Current.MainWindow, (string)FindResource("m_bdless") + "\r\nErrorInfo:\r\n" + ex.Message, (string)FindResource("m_Error"));
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                        MessageBox.Show(Application.Current.MainWindow, (string)FindResource("m_bdless") + "\r\nErrorInfo:\r\n" + ex.Message, (string)FindResource("m_Error"));
                    }));
                }
                ClearEffect(this);
            }
            catch (TimeoutException ex)
            {
                ApplyEffect(this);
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    contractsList.Clear();
                    contractsEntities.Reset(contractsList);
                    contractsView = (ListCollectionView)CollectionViewSource.GetDefaultView(contractsEntities);
                    webControl1.Reload(true);
                }));
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    MessageBox.Show(Application.Current.MainWindow, (string)FindResource("m_bdless") + "\r\nErrorInfo:\r\n" + ex.Message, (string)FindResource("m_Error"));
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                        MessageBox.Show(Application.Current.MainWindow, (string)FindResource("m_bdless") + "\r\nErrorInfo:\r\n" + ex.Message, (string)FindResource("m_Error"));
                    }));
                }
                ClearEffect(this);
            }
            catch (SocketException ex)
            {
                ApplyEffect(this);
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    contractsList.Clear();
                    contractsEntities.Reset(contractsList);
                    contractsView = (ListCollectionView)CollectionViewSource.GetDefaultView(contractsEntities);
                    webControl1.Reload(true);
                }));
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    MessageBox.Show(Application.Current.MainWindow, (string)FindResource("m_bdless") + "\r\nErrorInfo:\r\n" + ex.Message, (string)FindResource("m_Error"));
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                        MessageBox.Show(Application.Current.MainWindow, (string)FindResource("m_bdless") + "\r\nErrorInfo:\r\n" + ex.Message, (string)FindResource("m_Error"));
                    }));
                }
                ClearEffect(this);
            }
            finally
            {
                conn.Close();
            }
            ClearEffect(this);
        }
        private void testbutton_Click(object sender, RoutedEventArgs e)
        {
            IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server", serveriptextBox.Text);
            IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port", serverporttextBox.Text);
            IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "database", dbtextBox.Text);
            IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user", usertextBox.Text);
            IniFile.Write(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "pass", passtextBox.Text);
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\postgresql\"))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\postgresql\");
            }
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\postgresql\pgpass.conf"))
            {
                List<string> pass = new List<string>();
                using (var streamReader = File.OpenText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\postgresql\pgpass.conf"))
                {
                    var lines = streamReader.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        string[] param = line.Split(':');
                        if (!param[0].Contains(IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server")))
                        {
                            pass.Add(line);
                        }
                        else if (!param[3].Contains(IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user")))
                        {
                            pass.Add(line);
                        }
                    }
                }
                using (StreamWriter sw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\postgresql\pgpass.conf"))
                {
                    foreach (string lin in pass)
                    {
                        sw.WriteLine(lin);
                    }
                    sw.WriteLine(IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server") + ':' +
                        IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port") + ':' +
                        '*' + ':' +
                        IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user") + ':' +
                        IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "pass"));
                }
            }
            else
            {
                using (FileStream fs = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\postgresql\pgpass.conf"))
                {
                    fs.Close();
                }
                using (StreamWriter sw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\postgresql\pgpass.conf"))
                {
                    sw.WriteLine(IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server") +
                        IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port") +
                        '*' + ':' +
                        IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user") +
                        IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "pass"));
                }
            }
            ApplyEffect(this);
            testservThread = new Task(rinsh);
            testservThread.Start();
        }

        private void enablepasscheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ApplyEffect(this);
            pass.nowchangepass = true;
            pass.oldpasstrue = false;
            passwindow oldw = new passwindow();
            oldw.ShowDialog();
            pass.nowchangepass = false;
            if (pass.oldpasstrue)
            {
                if ((bool)enablepasscheckBox.IsChecked)
                {
                    farmer.Properties.Settings.Default.passen = true;
                    farmer.Properties.Settings.Default.Save();
                }
                else
                {
                    farmer.Properties.Settings.Default.passen = false;
                    farmer.Properties.Settings.Default.Save();
                }
            }
            else
            {
                enablepasscheckBox.Checked -= enablepasscheckBox_Checked;
                enablepasscheckBox.IsChecked = !(bool)enablepasscheckBox.IsChecked;
                enablepasscheckBox.Checked += enablepasscheckBox_Checked;
            }
            ClearEffect(this);
        }

        private void langcomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.Language = langcomboBox.SelectedValue as CultureInfo;
            cccombo.typecomboBoxitems = new List<KeyValuePair<string, string>>{
            new KeyValuePair<string, string>("0", (string)Application.Current.Resources["m_contrlease"]),
            new KeyValuePair<string, string>("1", (string)Application.Current.Resources["m_contrsublease"]),
            new KeyValuePair<string, string>("2", (string)Application.Current.Resources["m_contrjoint"]),
            new KeyValuePair<string, string>("3", (string)Application.Current.Resources["m_contrcommun"])
        };
            cccombo.statuscomboBoxitems = new List<KeyValuePair<string, string>>{
            new KeyValuePair<string, string>("0", (string)Application.Current.Resources["m_registered"]),
            new KeyValuePair<string, string>("1", (string)Application.Current.Resources["m_noregistered"])
        };
            cccombo.formownercomboBoxitems = new List<KeyValuePair<string, string>>{
            new KeyValuePair<string, string>("0", (string)Application.Current.Resources["m_privt"]),
            new KeyValuePair<string, string>("1", (string)Application.Current.Resources["m_gov"]),
            new KeyValuePair<string, string>("2", (string)Application.Current.Resources["m_commun"])
        };
            cccombo.formunitcomboBoxitems = new List<KeyValuePair<string, string>>{
            new KeyValuePair<string, string>("0", (string)Application.Current.Resources["m_arable"]),
            new KeyValuePair<string, string>("1", (string)Application.Current.Resources["m_haymaking"]),
            new KeyValuePair<string, string>("2", (string)Application.Current.Resources["m_pasture"]),
            new KeyValuePair<string, string>("3", (string)Application.Current.Resources["m_perennials"]),
            new KeyValuePair<string, string>("4", (string)Application.Current.Resources["m_farmstead"]),
            new KeyValuePair<string, string>("5", (string)Application.Current.Resources["m_industrial"])
        };
            cccombo.formusecomboBoxitems = new List<KeyValuePair<string, string>>{
            new KeyValuePair<string, string>("0", (string)Application.Current.Resources["m_commagr"]),
            new KeyValuePair<string, string>("1", (string)Application.Current.Resources["m_farm"]),
            new KeyValuePair<string, string>("2", (string)Application.Current.Resources["m_privtfarm"]),
            new KeyValuePair<string, string>("3", (string)Application.Current.Resources["m_industrial"])
        };
            int currsel = typecomboBox.SelectedIndex;
            typecomboBox.ItemsSource = cccombo.typecomboBoxitems;
            typecomboBox.SelectedIndex = currsel;
            currsel = statuscomboBox.SelectedIndex;
            statuscomboBox.ItemsSource = cccombo.statuscomboBoxitems;
            statuscomboBox.SelectedIndex = currsel;
            currsel = formownercomboBox.SelectedIndex;
            formownercomboBox.ItemsSource = cccombo.formownercomboBoxitems;
            formownercomboBox.SelectedIndex = currsel;
            currsel = formunitcomboBox.SelectedIndex;
            formunitcomboBox.ItemsSource = cccombo.formunitcomboBoxitems;
            formunitcomboBox.SelectedIndex = currsel;
            currsel = formusecomboBox.SelectedIndex;
            formusecomboBox.ItemsSource = cccombo.formusecomboBoxitems;
            formusecomboBox.SelectedIndex = currsel;
            currsel = maptypecomboBox.SelectedIndex;
            maptypecomboBox.ItemsSource = cccombo.typecomboBoxitems;
            maptypecomboBox.SelectedIndex = currsel;
            currsel = mapstatuscomboBox.SelectedIndex;
            mapstatuscomboBox.ItemsSource = cccombo.statuscomboBoxitems;
            mapstatuscomboBox.SelectedIndex = currsel;
            currsel = mapformownercomboBox.SelectedIndex;
            mapformownercomboBox.ItemsSource = cccombo.formownercomboBoxitems;
            mapformownercomboBox.SelectedIndex = currsel;
            currsel = mapformunitcomboBox.SelectedIndex;
            mapformunitcomboBox.ItemsSource = cccombo.formunitcomboBoxitems;
            mapformunitcomboBox.SelectedIndex = currsel;
            currsel = mapformusecomboBox.SelectedIndex;
            mapformusecomboBox.ItemsSource = cccombo.formusecomboBoxitems;
            mapformusecomboBox.SelectedIndex = currsel;
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ApplyEffect(this);
            pass.nowchangepass = true;
            pass.oldpasstrue = false;
            passwindow oldw = new passwindow();
            oldw.ShowDialog();
            pass.nowchangepass = false;
            if (pass.oldpasstrue)
            {
                newpasswin nw = new newpasswin();
                nw.ShowDialog();
            }
            ClearEffect(this);
        }
        Task webvwldfrcomplthr;
        private void webView1_LoadingFrameCompletefunc()
        {
            foreach (contract row in contractsList)
            {
                geojsonFeature lay = new geojsonFeature();
                lay.type = "Feature";
                lay.properties = new geojsonFeature.propertiesFeature();
                lay.geometry = new geojsonFeature.geometryFeature();
                lay.properties.color = "DeepSkyBlue";
                lay.properties.weight = "2";
                lay.properties.areanum = row.area_number;//m_leaser
                string description = @"<div id=" + '"' + "index" + '"' + " style=" + '"' + "display:none" + '"' + ">" + row.id + "</div>" +
                    "<div id=" + '"' + "cadnum" + '"' + ">" + (string)FindResource("m_cadnum") + ": " + row.cadastral_number + "</div>" +
                    "<div id=" + '"' + "owner" + '"' + ">" + (string)FindResource("m_lessor") + ": " + row.lessor + "</div>" +
                    "<div id=" + '"' + "leaser" + '"' + ">" + (string)FindResource("m_leaser") + ": " + row.leaser + "</div>" +
                    "<div id=" + '"' + "square" + '"' + ">" + (string)FindResource("m_square") + ": " + row.square + "</div>" +
                    "<div id=" + '"' + "field" + '"' + ">" + (string)FindResource("m_field") +
                    ": <input id=" + '"' + "area_input" + '"' + " type=" + '"' + "text" + '"' + " style=" + '"' + "width: 60px" + '"' + " value=" + '"' + row.area_number + '"' + "/>" +
                    "<button id=" + '"' + "savebtn" + '"' + " style=" + '"' + "visibility: visible" + '"' + " onclick=" + '"' + "updatefdb()" + '"' + ">" + (string)FindResource("m_save") + "</button></div>" +
                    "<div id=" + '"' + "unit_number" + '"' + ">" + (string)FindResource("m_unit_number") + ": " + row.unit_number + "</div>";
                string newdescrp = "";
                for (int i = 0; i < description.Length; i++)
                {
                    if (description[i] == Char.Parse("'"))
                    {
                        newdescrp = newdescrp + "&#39;";
                    }
                    else
                    {
                        newdescrp = newdescrp + description[i];
                    }
                }
                List<coordsformat> currcoords = new List<coordsformat>();
                foreach (coordsformat coord in allcoords)
                {
                    if (coord.id == row.id)
                    {
                        currcoords.Add(coord);
                    }
                }
                List<int> listrank = new List<int>();
                foreach (coordsformat coord in currcoords)
                {
                    if (!listrank.Contains(coord.rank))
                    {
                        listrank.Add(coord.rank);
                    }
                }
                lay.geometry.type = "Polygon";
                lay.geometry.coordinates = new string[listrank.Count()][][];
                for (int l = 0; l < listrank.Count(); l++)
                {
                    List<coordsformat> coordss = new List<coordsformat>();
                    foreach (coordsformat coord in currcoords)
                    {
                        if (coord.rank == listrank[l])
                        {
                            coordss.Add(coord);
                        }
                    }
                    lay.geometry.coordinates[l] = new string[coordss.Count()][];
                    int i = 0;
                    foreach (var item in coordss)
                    {
                        lay.geometry.coordinates[l][i] = new string[2];
                        lay.geometry.coordinates[l][i][0] = item.x.ToString();
                        lay.geometry.coordinates[l][i][1] = item.y.ToString();
                        i++;
                    }
                }

                //string jsonlay = Serializejson(lay);
                Dispatcher.BeginInvoke(new Action(() => webView1.ExecuteJavascript("loadarea('" + Serializejson(lay) + "'" + "," + "'" + newdescrp + "')")));
            }
            Dispatcher.BeginInvoke(new Action(() => {
                webView1.ExecuteJavascript("init()");
                filtermap();
            }));
            ClearEffect(this);
        }
        private void webView1_LoadingFrameComplete(object sender, FrameEventArgs e)
        {
            if (contractsList.Count > 0 && e.IsMainFrame)
            {
                ApplyEffect(this);
                webvwldfrcomplthr = new Task(webView1_LoadingFrameCompletefunc);
                webvwldfrcomplthr.Start();
            }

        }
        Task renderpdfthr;
        private async void renderpdf()
        {
            await Task.Delay(6000);
            int bmpHeight;
            int bmpWidth;
            this.Dispatcher.Invoke(() =>
            {
                using (var bitmap = new Bitmap(webView1.Width, webView1.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    BitmapSurface surface = (BitmapSurface)webView1.Surface;
                    BitmapData bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                    surface.CopyTo(bitmapData.Scan0, surface.RowSpan, 4, false, false);
                    bitmap.UnlockBits(bitmapData);

                    bmpHeight = bitmap.Height;
                    bmpWidth = bitmap.Width;
                    bitmap.Save("temp.png", ImageFormat.Png);
                }
            });
            using (var imageStream = File.OpenRead("temp.png"))
            {
                var decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.IgnoreColorProfile,
                    BitmapCacheOption.Default);
                bmpHeight = decoder.Frames[0].PixelHeight;
                bmpWidth = decoder.Frames[0].PixelWidth;
            }
            string filename = "";
            using (PdfDocument document = new PdfDocument())
            {
                PdfPage page = document.AddPage();
                this.Dispatcher.Invoke(() =>
                {
                    switch (selpapertypestr)
                    {
                        case "A4":
                            page.Size = PageSize.A4;
                            break;
                        case "A3":
                            page.Size = PageSize.A3;
                            break;
                        case "A2":
                            page.Size = PageSize.A2;
                            break;
                        case "A1":
                            page.Size = PageSize.A1;
                            break;
                        case "A0":
                            page.Size = PageSize.A0;
                            break;
                    }
                });
                page.Orientation = PageOrientation.Landscape;
                XGraphics gfx = XGraphics.FromPdfPage(page);
                Document doc = new Document();
                MigraDoc.DocumentObjectModel.Section sec = doc.AddSection();
                PageSetup p = new PageSetup();
                MigraDoc.DocumentObjectModel.Paragraph para = sec.AddParagraph();
                para.AddImage("temp.png");
                MigraDoc.Rendering.DocumentRenderer docRenderer = new DocumentRenderer(doc);
                docRenderer.PrepareDocument();
                docRenderer.RenderObject(gfx, XUnit.FromCentimeter(0), XUnit.FromCentimeter(0), "12cm", para);
                filename = "map_" + DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss") + ".pdf";
                document.Save(filename);
                docRenderer = null;
            }
            File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\temp.png");
            this.Dispatcher.Invoke(() =>
            {
                webControl1.Visibility = Visibility.Visible;
                webView1.Resize(4494, 3179);
            });
            GC.Collect();
            ClearEffect(this);
            Process.Start(filename);
        }

        public static string selpapertypestr { get; set; }
        public static int[] selpapertypeint { get; set; }
        private async void executejsc()
        {
            webControl1.Visibility = Visibility.Hidden;
            webView1.Resize((int)((selpapertypeint[0] / 25.4) * 96), (int)((selpapertypeint[1] / 25.4) * 96));
            await Task.Delay(1000);
            var resNorthEastlat = webControl1.ExecuteJavascriptWithResult("rect.getBounds().getNorthEast().lat");
            var resNorthEastlng = webControl1.ExecuteJavascriptWithResult("rect.getBounds().getNorthEast().lng");
            var resSouthWestlat = webControl1.ExecuteJavascriptWithResult("rect.getBounds().getSouthWest().lat");
            var resSouthWestlng = webControl1.ExecuteJavascriptWithResult("rect.getBounds().getSouthWest().lng");
            webView1.ExecuteJavascript("map.fitBounds([[" + resNorthEastlat.ToString() + ", " + resNorthEastlng.ToString() + 
                "],[" + resSouthWestlat.ToString() + "," + resSouthWestlng.ToString() + "]]);");
        }
        private  JSValue returnvalue(JSValue[] result)
        {
            if (result[0])
            {
                ApplyEffect(this);
                executejsc();
                renderpdfthr = new Task(renderpdf);
                renderpdfthr.Start();
            }
            printmenuItem.Visibility = Visibility.Visible;
            return null;
        }
        private void printmenuItem_Click(object sender, RoutedEventArgs e)
        {
            printmenuItem.Visibility = Visibility.Collapsed;
            ApplyEffect(this);
            selpaper mdw = new selpaper();
            mdw.ShowDialog();
            webControl1.ExecuteJavascript("enselrect()");
            ClearEffect(this);
        }

        private void RowDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listView.SelectedItem != null)
            {
                contractwindow objModal = new contractwindow();
                objModal.Owner = this;
                ApplyEffect(this);
                contract row = (contract)listView.SelectedItem;
                objModal.loadcontract(row.id);
                objModal.ShowDialog();

                refreshwin();
                ClearEffect(this);
            }
        }
        private void addkmlbutton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.DefaultExt = ".json";
            //dlg.Filter = "json (.json)|*.json";
            dlg.DefaultExt = ".kml";
            dlg.Filter = "KML (*.kml)|*.kml";
            dlg.Multiselect = true;
            Nullable<bool> result = dlg.ShowDialog(Window.GetWindow(this));
            if (result == true && dlg.FileNames.Length > 0)
            {
                foreach (string flname in dlg.FileNames)
                {
                    string xmlstr = "";
                    foreach (string kmlstr in System.IO.File.ReadAllLines(flname))
                    {
                        if (!kmlstr.Contains("<kml") && !kmlstr.Contains("</kml>"))
                        {
                            xmlstr = xmlstr + kmlstr;
                        }
                    }
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xmlstr);
                    XDocument xdoc = XDocument.Load(new XmlNodeReader(doc));
                    geojsonFeature lay = new geojsonFeature();
                    lay.type = "Feature";
                    lay.properties = new geojsonFeature.propertiesFeature();
                    lay.geometry = new geojsonFeature.geometryFeature();
                    lay.properties.color = "DarkRed";
                    lay.properties.weight = "2";
                    string description = "";
                    string outercoords = "";
                    XElement Folder = xdoc.XPathSelectElement("//Folder");
                    if (Folder != null)
                    {
                        XElement name = Folder.XPathSelectElement("//name");
                        XElement Placemark = Folder.XPathSelectElement("//Placemark");
                        if (Placemark != null)
                        {
                            XElement Polygon = Folder.XPathSelectElement("//Polygon");
                            if (Polygon != null)
                            {
                                XElement outerBoundaryIs = Folder.XPathSelectElement("//outerBoundaryIs");
                                if (Placemark != null)
                                {
                                    XElement coordinates = Folder.XPathSelectElement("//coordinates");
                                    if (coordinates != null)
                                    {
                                        outercoords = coordinates.FirstNode.ToString();
                                    }
                                }
                            }
                        }
                    }
                    description = flname;
                    string newdescrp = "";
                    for (int i = 0; i < description.Length; i++)
                    {
                        if (description[i] == Char.Parse("'"))
                        {
                            newdescrp = newdescrp + "&#39;";
                        }
                        else
                        {
                            newdescrp = newdescrp + description[i];
                        }
                    }
                    
                    lay.geometry.type = "Polygon";
                    lay.geometry.coordinates = new string[1][][];
                    string normoutercoords = "";
                    foreach (char chr in outercoords)
                    {
                        if (Char.IsDigit(chr) | chr == ',' | chr == '.' | chr == '-')
                        {
                            normoutercoords = normoutercoords + chr;
                        }
                        else
                        {
                            normoutercoords = normoutercoords + "\r\n";
                        }
                    }
                    string[] outercoordsstr = normoutercoords.Split(new string[] { "\r\n"}, StringSplitOptions.None);
                    List<string> clearoutercoordsstr = new List<string>();
                    foreach (string outercoord in outercoordsstr)
                    {
                        if (outercoord != "")
                        {
                            clearoutercoordsstr.Add(outercoord);
                        }
                    }
                    lay.geometry.coordinates[0] = new string[clearoutercoordsstr.Count()][];
                    for(int l = 0; l < clearoutercoordsstr.Count; l++)
                    {
                        string[] coords = clearoutercoordsstr[l].Split(',');
                        lay.geometry.coordinates[0][l] = new string[2];
                        lay.geometry.coordinates[0][l][0] = coords[0];
                        lay.geometry.coordinates[0][l][1] = coords[1];
                    }
                    
                    webControl1.ExecuteJavascript("loadkmlf('" + Serializejson(lay) + "'" + "," + "'" + Path.GetFileName(newdescrp).Replace(".kml","") + "')");
                    webView1.ExecuteJavascript("loadkmlf('" + Serializejson(lay) + "'" + "," + "'" + Path.GetFileName(newdescrp).Replace(".kml", "") + "')");
                }
            }
        }

        private void enableeditcheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if ((bool)enableeditcheckBox.IsChecked)
            {
                farmer.Properties.Settings.Default.editdbenable = true;
                farmer.Properties.Settings.Default.Save();
            }
            else
            {
                farmer.Properties.Settings.Default.editdbenable = false;
                farmer.Properties.Settings.Default.Save();
            }
        }
    }
}
