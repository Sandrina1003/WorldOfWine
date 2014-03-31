using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;
using System.Diagnostics;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wines.Controls;
using Windows.Storage;


// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Wines.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ReturnedWines : Wines.Common.LayoutAwarePage
    {
        public LoginFlyout LoginFly = null;
        public GetWinesFlyout GetWinesFly = null;
        ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
        public ReturnedWines()
        {
            this.InitializeComponent();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (roamingSettings.Values["username"] != null && roamingSettings.Values["password"] != null)
                Login.IsEnabled = false;
            string url = e.Parameter as string;
            List<WineItem> lw = await GetWebResponse(url);
            FillWines(lw);
            
        }
        void Wine_Click(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var Item = ((WineSearchItem)e.ClickedItem);
            this.Frame.Navigate(typeof(RetWineDetails), Item);
        }

        private void HomeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {


            if (roamingSettings.Values["username"] == null || roamingSettings.Values["password"] == null)
            {
                var transform = Login.TransformToVisual(MainGrid);
                Point AbsolutePosition = transform.TransformPoint(new Point(0, 0));
                if (LoginFly == null)
                {

                    Rect bounds = Window.Current.Bounds;
                    double height = bounds.Height;
                    double width = bounds.Width;

                    LoginFly = new LoginFlyout(AbsolutePosition.X * 2, AbsolutePosition.Y - 310, null);

                }
                else
                {
                    if (LoginFly.IsOpen())
                    {
                        LoginFly.Close();
                    }
                    else
                    {
                        LoginFly.Open();

                    }
                }
            }

        }

        public ObservableCollection<WineSearchItem> Winelist;
        public async void FillWines(List<WineItem> wineL)
        {
            Winelist = new ObservableCollection<WineSearchItem>();
            foreach (WineItem v in wineL)
            {
                Winelist.Add(new WineSearchItem()
                {
                    ID=v.id,
                    Name = v.name,
                    Code = v.code,
                    Available = v.available,
                    Link = v.link,
                    Num_merchants = v.num_merchants,
                    Num_reviews = v.num_reviews,
                    Price = v.price,
                    Region = v.region,
                    Snoothrank = v.snoothrank,
                    Type = v.type,
                    Varietal = v.varietal,
                    Vintage = v.vintage,
                    Winery = v.winery,
                    Winery_ID = v.winery_id,
                    Image = v.image,
                    Recipes=v.recipes
                    
                });
            }
            itemListView.ItemsSource = Winelist;
            //WinesGridView.ItemsSource = Winelist;
        }
        List<WineItem> listW = new List<WineItem>();
        private async Task<List<WineItem>> GetWebResponse(string url)
        {

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
            HttpClient httpClient = new HttpClient();


            try
            {
                HttpResponseMessage httpResponse = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
                HttpStatusCode statusCode = httpResponse.StatusCode;
                HttpContent responseContent = httpResponse.Content;

                if (responseContent != null)
                {
                    string results = await responseContent.ReadAsStringAsync();
                    // txtResponse.Text = results;
                    JObject result = JObject.Parse(results);
                    JArray wines = (JArray)result["wines"];



                    foreach (JObject wine in wines)
                    {
                        WineItem w = new WineItem();
                       
                        
                        w.name = wine["name"].ToString();
                        w.code = wine["code"].ToString();
                        w.region = wine["region"].ToString();
                        w.winery = wine["winery"].ToString();
                        w.winery_id = wine["winery_id"].ToString();
                        w.varietal = wine["varietal"].ToString();
                        w.price = wine["price"].ToString();
                        w.vintage = wine["vintage"].ToString();
                        w.type = wine["type"].ToString();
                        w.link = wine["link"].ToString();
                        w.image = wine["image"].ToString();
                        w.snoothrank = wine["snoothrank"].ToString();
                        w.available = wine["available"].ToString();
                        w.num_merchants = wine["num_merchants"].ToString();
                        w.num_reviews = wine["num_reviews"].ToString();
                        JArray recipes = (JArray)wine["recipes"];
                        w.recipes = new List<Recipe>();
                        if (recipes != null)
                        {
                            foreach (JObject rec in recipes)
                            {

                                Recipe rc = new Recipe();
                                rc.name = rec["name"].ToString();
                                rc.link = rec["link"].ToString();
                                rc.source_link = rec["source_link"].ToString();
                                rc.image = rec["image"].ToString();
                                w.recipes.Add(rc);

                            }
                        }
                        listW.Add(w);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return listW;
        }
        //public class ResWine
        //{
        //    public string id { get; set; }
        //    public string name { get; set; }
        //    public string region { get; set; }
        //    public string winery { get; set; }
        //    public string winery_id { get; set; }
        //    public string varietal { get; set; }
        //    public string code { get; set; }
        //    public string price { get; set; }
        //    public string vintage { get; set; }
        //    public string type { get; set; }
        //    public string link { get; set; }
        //    public string image { get; set; }
        //    public string snoothrank { get; set; }
        //    public string available { get; set; }
        //    public string num_merchants { get; set; }
        //    public string num_reviews { get; set; }
        //    // public List<Review> reviews { get; set; }
        //}
        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        private void GetWines_Click(object sender, RoutedEventArgs e)
        {
            var transform = BtnGetWines.TransformToVisual(MainGrid);
            Point AbsolutePosition = transform.TransformPoint(new Point(0, 0));
            if (GetWinesFly == null)
            {
                Rect bounds = Window.Current.Bounds;
                double height = bounds.Height;
                double width = bounds.Width;
                //GetWinesFly.Width = bounds.Width;

                GetWinesFly = new GetWinesFlyout(AbsolutePosition.X - BtnGetWines.ActualWidth * 2 - 60, this.ActualHeight);

            }
            else
            {
                if (GetWinesFly.IsOpen())
                {
                    GetWinesFly.Close();
                }
                else
                {
                    GetWinesFly.Open();
                }
            }
        }

     

        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }
    }
}
