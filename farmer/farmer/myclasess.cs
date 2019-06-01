using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace farmer
{
    public class pass
    {
        public static bool nowchangepass { set; get; }
        public static bool oldpasstrue { set; get; }
    }
    public class myvariable
    {
        public static int errcount { set; get; }
        public static string errstr { set; get; }
    }
    public class ulidpoint
    {
        public string ulid { set; get; }
        public string point { set; get; }
    }
    public class ulidpoints
    {
        public string ulid { set; get; }
        public string fp { set; get; }
        public string sp { set; get; }
    }

    public class cccombo
    {
        public static List<KeyValuePair<string, string>> oblcomboBoxitems = new List<KeyValuePair<string, string>>{
            new KeyValuePair<string, string>("100000000", "АВТОНОМНА РЕСПУБЛІКА КРИМ"),
            new KeyValuePair<string, string>("500000000", "ВІННИЦЬКА"),
            new KeyValuePair<string, string>("700000000", "ВОЛИНСЬКА"),
            new KeyValuePair<string, string>("1200000000", "ДНІПРОПЕТРОВСЬКА"),
            new KeyValuePair<string, string>("1400000000", "ДОНЕЦЬКА"),
            new KeyValuePair<string, string>("1800000000", "ЖИТОМИРСЬКА"),
            new KeyValuePair<string, string>("2100000000", "ЗАКАРПАТСЬКА"),
            new KeyValuePair<string, string>("2300000000", "ЗАПОРІЗЬКА"),
            new KeyValuePair<string, string>("2600000000", "ІВАНО-ФРАНКІВСЬКА"),
            new KeyValuePair<string, string>("3200000000", "КИЇВСЬКА"),
            new KeyValuePair<string, string>("3500000000", "КІРОВОГРАДСЬКА"),
            new KeyValuePair<string, string>("4400000000", "ЛУГАНСЬКА"),
            new KeyValuePair<string, string>("4600000000", "ЛЬВІВСЬКА"),
            new KeyValuePair<string, string>("4800000000", "МИКОЛАЇВСЬКА"),
            new KeyValuePair<string, string>("5100000000", "ОДЕСЬКА"),
            new KeyValuePair<string, string>("5300000000", "ПОЛТАВСЬКА"),
            new KeyValuePair<string, string>("5600000000", "РІВНЕНСЬКА"),
            new KeyValuePair<string, string>("5900000000", "СУМСЬКА"),
            new KeyValuePair<string, string>("6100000000", "ТЕРНОПІЛЬСЬКА"),
            new KeyValuePair<string, string>("6300000000", "ХАРКІВСЬКА"),
            new KeyValuePair<string, string>("6500000000", "ХЕРСОНСЬКА"),
            new KeyValuePair<string, string>("6800000000", "ХМЕЛЬНИЦЬКА"),
            new KeyValuePair<string, string>("7100000000", "ЧЕРКАСЬКА"),
            new KeyValuePair<string, string>("7300000000", "ЧЕРНІВЕЦЬКА"),
            new KeyValuePair<string, string>("7400000000", "ЧЕРНІГІВСЬКА")
        };
        public static List<KeyValuePair<int[], string>> typepapercomboBoxitems = new List<KeyValuePair<int[], string>>{
            new KeyValuePair<int[], string>(new int[] {297,210}, "A4"),
            new KeyValuePair<int[], string>(new int[] {420,297}, "A3"),
            new KeyValuePair<int[], string>(new int[] {594,420}, "A2"),
            new KeyValuePair<int[], string>(new int[] {841,594}, "A1"),
            new KeyValuePair<int[], string>(new int[] {1189,841}, "A0")
        };
        public static List<KeyValuePair<string, string>> typecomboBoxitems = new List<KeyValuePair<string, string>>{
            new KeyValuePair<string, string>("0", (string)Application.Current.Resources["m_contrlease"]),
            new KeyValuePair<string, string>("1", (string)Application.Current.Resources["m_contrsublease"]),
            new KeyValuePair<string, string>("2", (string)Application.Current.Resources["m_contrjoint"]),
            new KeyValuePair<string, string>("3", (string)Application.Current.Resources["m_contrcommun"])
        };
        public static List<KeyValuePair<string, string>> statuscomboBoxitems = new List<KeyValuePair<string, string>>{
            new KeyValuePair<string, string>("0", (string)Application.Current.Resources["m_registered"]),
            new KeyValuePair<string, string>("1", (string)Application.Current.Resources["m_noregistered"])
        };
        public static List<KeyValuePair<string, string>> formownercomboBoxitems = new List<KeyValuePair<string, string>>{
            new KeyValuePair<string, string>("0", (string)Application.Current.Resources["m_privt"]),
            new KeyValuePair<string, string>("1", (string)Application.Current.Resources["m_gov"]),
            new KeyValuePair<string, string>("2", (string)Application.Current.Resources["m_commun"])
        };
        public static List<KeyValuePair<string, string>> formunitcomboBoxitems = new List<KeyValuePair<string, string>>{
            new KeyValuePair<string, string>("0", (string)Application.Current.Resources["m_arable"]),
            new KeyValuePair<string, string>("1", (string)Application.Current.Resources["m_haymaking"]),
            new KeyValuePair<string, string>("2", (string)Application.Current.Resources["m_pasture"]),
            new KeyValuePair<string, string>("3", (string)Application.Current.Resources["m_perennials"]),
            new KeyValuePair<string, string>("4", (string)Application.Current.Resources["m_farmstead"]),
            new KeyValuePair<string, string>("5", (string)Application.Current.Resources["m_industrial"])
        };
        public static List<KeyValuePair<string, string>> formusecomboBoxitems = new List<KeyValuePair<string, string>>{
            new KeyValuePair<string, string>("0", (string)Application.Current.Resources["m_commagr"]),
            new KeyValuePair<string, string>("1", (string)Application.Current.Resources["m_farm"]),
            new KeyValuePair<string, string>("2", (string)Application.Current.Resources["m_privtfarm"]),
            new KeyValuePair<string, string>("3", (string)Application.Current.Resources["m_industrial"])
        };
    }
    public class coordsformat
    {
        public int id { set; get; }
        public string x { set; get; }
        public string y { set; get; }
        public int rank { set; get; }
    }
    public class geojsonFeature
    {
        public string type { set; get; }
        public propertiesFeature properties { set; get; }
        public geometryFeature geometry { set; get; }
        public class propertiesFeature
        {
            public string name { set; get; }
            public string color { set; get; }
            public string weight { set; get; }
            public string areanum { set; get; }
            public string opacity { set; get; }
            public string description { set; get; }
        }
        public class geometryFeature
        {
            public string type { set; get; }
            public string[][][] coordinates { set; get; }
        }
    }
    public class contract : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _id;
        public int id
        {
            get { return _id; }
            set { _id = value; NotifyPropertyChanged("id"); }
        }
        private string _contract_number;
        public string contract_number
        {
            get { return _contract_number; }
            set { _contract_number = value; NotifyPropertyChanged("contract_number"); }
        }

        private string _date_contract;
        public string date_contract
        {
            get { return _date_contract; }
            set { _date_contract = value; NotifyPropertyChanged("date_contract"); }
        }
        private string _lessor;
        public string lessor
        {
            get { return _lessor; }
            set { _lessor = value; NotifyPropertyChanged("lessor"); }
        }
        private string _leaser;
        public string leaser
        {
            get { return _leaser; }
            set { _leaser = value; NotifyPropertyChanged("lessor"); }
        }
        private string _cadastral_number;
        public string cadastral_number
        {
            get { return _cadastral_number; }
            set { _cadastral_number = value; NotifyPropertyChanged("cadastral_number"); }
        }
        private string _square;
        public string square
        {
            get { return _square; }
            set { _square = value; NotifyPropertyChanged("square"); }
        }
        private string _term_lease;
        public string term_lease
        {
            get { return _term_lease; }
            set { _term_lease = value; NotifyPropertyChanged("term_lease"); }
        }
        private string _registration_date;
        public string registration_date
        {
            get { return _registration_date; }
            set { _registration_date = value; NotifyPropertyChanged("registration_date"); }
        }
        private string _contract_status;
        public string contract_status
        {
            get { return _contract_status; }
            set { _contract_status = value; NotifyPropertyChanged("contract_status"); }
        }
        private string _unit_number;
        public string unit_number
        {
            get { return _unit_number; }
            set { _unit_number = value; NotifyPropertyChanged("unit_number"); }
        }
        private string _notes;
        public string notes
        {
            get { return _notes; }
            set { _notes = value; NotifyPropertyChanged("notes"); }
        }
        private string _type_contract;
        public string type_contract
        {
            get { return _type_contract; }
            set { _type_contract = value; NotifyPropertyChanged("type_contract"); }
        }
        private string _area_number;
        public string area_number
        {
            get { return _area_number; }
            set { _area_number = value; NotifyPropertyChanged("area_number"); }
        }
        private string _form_owner;
        public string form_owner
        {
            get { return _form_owner; }
            set { _form_owner = value; NotifyPropertyChanged("form_owner"); }
        }
        private string _contractortype;
        public string contractortype
        {
            get { return _contractortype; }
            set { _contractortype = value; NotifyPropertyChanged("fizoryur"); }
        }
        private string _form_use;
        public string form_use
        {
            get { return _form_use; }
            set { _form_use = value; NotifyPropertyChanged("form_use"); }
        }
        private string _form_unit;
        public string form_unit
        {
            get { return _form_unit; }
            set { _form_unit = value; NotifyPropertyChanged("form_unit"); }
        }
        private string _koatuu;
        public string koatuu
        {
            get { return _koatuu; }
            set { _koatuu = value; NotifyPropertyChanged("koatuu"); }
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class DocumentPaginatorWrapper : DocumentPaginator

    {

        Size m_PageSize;

        Size m_Margin;

        DocumentPaginator m_Paginator;

        Typeface m_Typeface;



        public DocumentPaginatorWrapper(DocumentPaginator paginator, Size pageSize, Size margin)

        {

            m_PageSize = pageSize;

            m_Margin = margin;

            m_Paginator = paginator;



            m_Paginator.PageSize = new Size(m_PageSize.Width - margin.Width * 2,

                                            m_PageSize.Height - margin.Height * 2);

        }



        Rect Move(Rect rect)

        {

            if (rect.IsEmpty)

            {

                return rect;

            }

            else

            {

                return new Rect(rect.Left + m_Margin.Width, rect.Top + m_Margin.Height,

                                rect.Width, rect.Height);

            }

        }



        public override DocumentPage GetPage(int pageNumber)

        {

            DocumentPage page = m_Paginator.GetPage(pageNumber);



            // Create a wrapper visual for transformation and add extras

            ContainerVisual newpage = new ContainerVisual();



            DrawingVisual title = new DrawingVisual();



            using (DrawingContext ctx = title.RenderOpen())

            {

                if (m_Typeface == null)

                {

                    m_Typeface = new Typeface("Times New Roman");

                }



                FormattedText text = new FormattedText("Page "+(pageNumber + 1),

                    System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,

                    m_Typeface, 14, Brushes.Black);



                ctx.DrawText(text, new Point(0, -96 / 4)); // 1/4 inch above page content

            }



            DrawingVisual background = new DrawingVisual();



            using (DrawingContext ctx = background.RenderOpen())

            {

                ctx.DrawRectangle(new SolidColorBrush(Color.FromRgb(240, 240, 240)), null, page.ContentBox);

            }



            newpage.Children.Add(background); // Scale down page and center



            ContainerVisual smallerPage = new ContainerVisual();

            smallerPage.Children.Add(page.Visual);

            smallerPage.Transform = new MatrixTransform(0.95, 0, 0, 0.95,

                0.025 * page.ContentBox.Width, 0.025 * page.ContentBox.Height);



            newpage.Children.Add(smallerPage);

            newpage.Children.Add(title);



            newpage.Transform = new TranslateTransform(m_Margin.Width, m_Margin.Height);



            return new DocumentPage(newpage, m_PageSize, Move(page.BleedBox), Move(page.ContentBox));

        }



        public override bool IsPageCountValid

        {

            get

            {

                return m_Paginator.IsPageCountValid;

            }

        }



        public override int PageCount

        {

            get

            {

                return m_Paginator.PageCount;

            }

        }



        public override Size PageSize

        {

            get

            {

                return m_Paginator.PageSize;

            }



            set

            {

                m_Paginator.PageSize = value;

            }

        }



        public override IDocumentPaginatorSource Source

        {

            get

            {

                return m_Paginator.Source;

            }

        }

    }
}
