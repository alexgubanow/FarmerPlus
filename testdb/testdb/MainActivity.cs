using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MySql.Data.MySqlClient;
using Android.Webkit;

namespace testdb
{
    [Activity(Label = "testdb", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        WebView web_view;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            web_view = FindViewById<WebView>(Resource.Id.webview);
            web_view.Settings.JavaScriptEnabled = true;
            web_view.SetWebViewClient(new HelloWebViewClient());
            web_view.LoadUrl("file:///android_asset/index.html");

            string strProvider = "Data Source=159.224.55.85; Database=c4barabandb;User ID=c4c4;Password=sad15forever;";
            MySqlConnection cnt = new MySqlConnection(strProvider);
            try
            {
                cnt.Open();
                MySqlCommand cmd = new MySqlCommand("select version();", cnt);
                //cmd.ExecuteNonQuery();
                String result = cmd.ExecuteScalar().ToString();
                EditText edtext = FindViewById<EditText>(Resource.Id.editText1);
                edtext.Text = result;
                cnt.Close();
            }
            //try
            //{
            //    MySqlConnection sqlconn;
            //    string connsqlstring = string.Format("Server=159.224.55.85;Port=3306;database=c4barabandb;User Id=c4c4;Password=sad15forever;charset=utf8");
            //    sqlconn = new MySqlConnection(connsqlstring);
            //    sqlconn.Open();
            //    string queryString = "select version();";
            //    MySqlCommand sqlcmd = new MySqlCommand(queryString, sqlconn);
            //    String result = sqlcmd.ExecuteScalar().ToString();
            //    EditText edtext = FindViewById<EditText>(Resource.Id.editText1);
            //    edtext.Text = result;
            //    sqlconn.Close();
            //}
            catch (Exception ex)
            {
                EditText edtext = FindViewById<EditText>(Resource.Id.editText1);
                edtext.Text = ex.Message;
            }
        }
        public class HelloWebViewClient : WebViewClient
        {
            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                view.LoadUrl(url);
                return true;
            }
        }
    }
}

