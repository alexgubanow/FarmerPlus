using Awesomium.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace farmer
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Initialization must be performed here,
            // before creating a WebControl.
            if (!WebCore.IsInitialized)
            {
                WebCore.Initialize(new WebConfig()
                {
                    HomeURL = new Uri("file:///" + System.AppDomain.CurrentDomain.BaseDirectory + "index.html"),
                    LogPath = System.AppDomain.CurrentDomain.BaseDirectory + "web.log",
                    LogLevel = LogLevel.Verbose,
                    RemoteDebuggingHost = "127.0.0.1",
                    RemoteDebuggingPort = 9033
                });
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Make sure we shutdown the core last.
            if (WebCore.IsInitialized)
                WebCore.Shutdown();

            base.OnExit(e);
        }
        private static List<CultureInfo> m_Languages = new List<CultureInfo>();

        public static List<CultureInfo> Languages
        {
            get
            {
                return m_Languages;
            }
        }

        public App()
        {
            App.LanguageChanged += App_LanguageChanged;
            m_Languages.Clear();
            m_Languages.Add(new CultureInfo("ru-RU"));
            m_Languages.Add(new CultureInfo("uk-UA"));
            m_Languages.Add(new CultureInfo("en-US"));
        }

        public static event EventHandler LanguageChanged;
        private void App_LanguageChanged(Object sender, EventArgs e)
        {
            farmer.Properties.Settings.Default.DefaultLanguage = Language;
            farmer.Properties.Settings.Default.Save();
        }
        public static CultureInfo Language
        {
            get
            {
                return System.Threading.Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                if (value == System.Threading.Thread.CurrentThread.CurrentUICulture) return;

                //1. Меняем язык приложения:
                System.Threading.Thread.CurrentThread.CurrentUICulture = value;

                //2. Создаём ResourceDictionary для новой культуры
                ResourceDictionary dict = new ResourceDictionary();
                switch (value.Name)
                {
                    case "ru-RU":
                        dict.Source = new Uri(String.Format("Resources/lang.ru-RU.xaml", value.Name), UriKind.Relative);
                        break;
                    case "uk-UA":
                        dict.Source = new Uri(String.Format("Resources/lang.uk-UA.xaml", value.Name), UriKind.Relative);
                        break;
                    case "en-US":
                        dict.Source = new Uri("Resources/lang.xaml", UriKind.Relative);
                        break;
                    default:
                        dict.Source = new Uri(String.Format("Resources/lang.xaml", value.Name), UriKind.Relative);
                        break;
                }

                //3. Находим старую ResourceDictionary и удаляем его и добавляем новую ResourceDictionary
                ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                              where d.Source != null && d.Source.OriginalString.StartsWith("Resources/lang.")
                                              select d).First();
                if (oldDict != null)
                {
                    int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                    Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                    Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
                }
                else
                {
                    Application.Current.Resources.MergedDictionaries.Add(dict);
                }

                //4. Вызываем евент для оповещения всех окон.
                LanguageChanged(Application.Current, new EventArgs());
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Language = farmer.Properties.Settings.Default.DefaultLanguage;
        }
    }
}
