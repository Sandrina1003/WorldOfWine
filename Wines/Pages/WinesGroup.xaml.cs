using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Wines.Controls;
using Wines.Pages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;
using System.Diagnostics;
using Windows.Storage;
using Windows.UI.Popups;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Wines.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class WinesGroup : Wines.Common.LayoutAwarePage
    {
        ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
        public GetWinesFlyout GetWinesFly = null;
        public LoginFlyout LoginFly = null;
        List<WineItem> RedWines = new List<WineItem>();
        List<WineItem> WhiteWines = new List<WineItem>();
        List<WineItem> RoseWines = new List<WineItem>();
        List<WineItem> AmberWines = new List<WineItem>();
        List<WineItem> ClearWines = new List<WineItem>();
        List<WineItem> AllWines = new List<WineItem>();
        List<WineItem> SparklingWines = new List<WineItem>();
        List<WineItem> MyWines = new List<WineItem>();
        //ObservableCollection<WineSearchItem> proba = new ObservableCollection<WineSearchItem>();
        //MainPage mp = new MainPage();

       

        public WinesGroup()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            if (roamingSettings.Values["username"] != null && roamingSettings.Values["password"] != null)
                Login.IsEnabled = false;
            LoadingTextWines.Visibility = Visibility.Visible;
            LoadingWines.IsActive = true;

            await FillData();
            //this.DefaultViewModel["Red"] = RedWinesList;
            //this.DefaultViewModel["White"] = WhiteWinesList;
            //this.DefaultViewModel["My"] = MyWinesList;
            //ObservableCollection<WineSearchItem> proba = new ObservableCollection<WineSearchItem>();
            //proba = (ObservableCollection<WineSearchItem>)this.DefaultViewModel["My"];
            LoadingTextWines.Visibility = Visibility.Collapsed;
            LoadingWines.IsActive = false;
            
            
            
            
            if (navigationParameter.ToString() == "Red")
            {
                //WinesGView.ItemsSource = this.DefaultViewModel["Red"];
                WinesGView.ItemsSource = RedWinesList;
                
            }
            if (navigationParameter.ToString() == "White")
            {
                ////ObservableCollection<WineSearchItem> proba = new ObservableCollection<WineSearchItem>();
                //proba = (ObservableCollection<WineSearchItem>)DefaultViewModel["My"];
                //ObservableCollection<WineSearchItem> listicka = proba;
                //// WinesGView.ItemsSource = this.DefaultViewModel["White"];
                WinesGView.ItemsSource =WhiteWinesList;
            }
            //if (navigationParameter.ToString() == "My")
            //{
                
                
            //    WinesGView.ItemsSource = MyWinesList;
            //}
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void HomeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        } 

        void Wine_Click(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var Item = ((WineSearchItem)e.ClickedItem);
            this.Frame.Navigate(typeof(WineDetails), Item);
        }
        public ObservableCollection<WineSearchItem> RedWinesList;
        public ObservableCollection<WineSearchItem> WhiteWinesList;
        public ObservableCollection<WineSearchItem> MyWinesList;

        public async Task FillData()
        {
            
            
            List<WineItem> Wines = await GetWines();
            //ObservableCollection<WineItem> Wines = new ObservableCollection<WineItem>();
            //Wines = (ObservableCollection<WineItem>)DefaultViewModel["White"];
            foreach (WineItem v in Wines)
            {
                if (v.type == "Red Wine")
                {
                    if (v.image == "")
                        v.image = "ms-appx:///Assets/RedWine.png";
                    RedWines.Add(v);
                }
                if (v.type == "White Wine")
                {
                    if (v.image == "")
                        v.image = "ms-appx:///Assets/WhiteWine.png";
                    WhiteWines.Add(v);
                }

            }

            //List<WineItem> MyWs = await MyWinesResponse();
            RedWinesList = new ObservableCollection<WineSearchItem>();
            WhiteWinesList = new ObservableCollection<WineSearchItem>();
            MyWinesList = new ObservableCollection<WineSearchItem>();
            foreach (WineItem v in RedWines)
            {

                RedWinesList.Add(new WineSearchItem()
                {
                    ID = v.id,
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
                    Reviews = v.reviews,
                    Recipes=v.recipes,
                    WineDetails =v.wineDet,
                    Photos=v.photos

                });
            }
           
            foreach (WineItem v in WhiteWines)
            {
                WhiteWinesList.Add(new WineSearchItem()
                {
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
                    Reviews = v.reviews,
                    ID = v.id,
                    Recipes = v.recipes,
                    WineDetails = v.wineDet,
                    Photos=v.photos

                });


            }
            //this.DefaultViewModel["White"] = RedWinesList;
            //foreach (WineItem v in MyWs)
            //{
            //    //string id;
            //    List<string> ids = await ReadId();
            //    for (int i = 0; i < Wines.Count; i++)
            //    {
            //        if (v.code == Wines[i].code)
            //            v.id = Wines[i].id;
            //    }
            //    MyWinesList.Add(new WineSearchItem()
            //    {
            //        Name = v.name,
            //        Code = v.code,
            //        Available = v.available,
            //        Link = v.link,
            //        Num_merchants = v.num_merchants,
            //        Num_reviews = v.num_reviews,
            //        Price = v.price,
            //        Region = v.region,
            //        Snoothrank = v.snoothrank,
            //        Type = v.type,
            //        Varietal = v.varietal,
            //        Vintage = v.vintage,
            //        Winery = v.winery,
            //        Winery_ID = v.winery_id,
            //        Image = v.image,
            //        Reviews = v.reviews,
            //        ID = v.id,
            //        Recipes = v.recipes,
            //        WineDetails = await ReadData(v.id)

            //    });


            //}
           
          
           //proba = (ObservableCollection<WineSearchItem>)DefaultViewModel["White"];
            
           
        }

        public async Task<WineDet> ReadData(string ID)
        {
            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("Assets\\wine.data.txt");

            var stream = await file.OpenReadAsync();

            var rdr = new StreamReader(stream.AsStream());

            var contents = await rdr.ReadToEndAsync();

            WineDet wd = new WineDet();

            var lines = contents.Split('|');
            for (int i = 0; i < lines.Length; i++)
            {
                var res = lines[i].Split(',');
                if (res[13] == ID)
                {
                    wd.Alcohol = res[1];
                    wd.Malic_acid = res[2];
                    wd.Ash = res[3];
                    wd.Alcalinity = res[4];
                    wd.Magnesium = res[5];
                    wd.Total_phenols = res[6];
                    wd.Flavanoids = res[7];
                    wd.Nonflavanoid_phenols = res[8];
                    wd.Proanthocyanins = res[9];
                    wd.Color_intensity = res[10];
                    wd.Hue = res[11];
                    wd.Proline = res[12];
                    wd.ID = ID;
                    wd.Class = res[0];
                }

            }
            return wd;
        }

        public async Task<WineItem> GetWebResponse(string id)
        {
            WineItem w = new WineItem();
            string uri = "http://api.snooth.com/wine?akey=nxe7rzjlof7xl0t2d6z9axbncz26du2jz84i8hw06ec9x9z7&id=" + id + "&food=1&photos=1";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(uri));
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
                        w.id = id;
                        w.wineDet = await ReadData(id);
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
                        JArray reviews = (JArray)wine["reviews"];
                        JArray photos = (JArray)wine["photos"];
                        w.reviews = new List<Review>();
                        w.photos = new List<Photo>();
                        if (reviews != null)
                        {
                            foreach (JObject rev in reviews)
                            {

                                Review r = new Review();
                                r.name = rev["name"].ToString();
                                r.rating = rev["rating"].ToString();
                                r.date = rev["date"].ToString();
                                r.lang = rev["lang"].ToString();
                                r.body = rev["body"].ToString();
                                w.reviews.Add(r);

                            }
                        }
                        if (photos != null)
                        {
                            foreach (JObject ph in photos)
                            {

                                Photo p = new Photo();
                                p.photo = ph["full"].ToString();
                                 w.photos.Add(p);

                            }
                        }
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
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return w;
        }

        public async Task<List<WineItem>> MyWinesResponse()
        {
            List<WineItem> MyRetWines = new List<WineItem>();
           
            string uri = "http://api.snooth.com/my-wines/?akey=nxe7rzjlof7xl0t2d6z9axbncz26du2jz84i8hw06ec9x9z7&u="+roamingSettings.Values["username"]+"&p="+roamingSettings.Values["password"];
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(uri));
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


                    if (wines != null)
                    {
                        foreach (JObject wine in wines)
                        {
                            WineItem w = new WineItem();
                            //w.id = id;
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
                            if (w.image == "")
                                w.image = "ms-appx:///Assets/MyWine.png";
                            w.snoothrank = wine["snoothrank"].ToString();
                            MyRating mr = new MyRating();
                            mr.rated = true;
                            mr.rating = wine["my-rating"].ToString();
                            mr.date = wine["date_activity"].ToString();
                            mr.review = wine["my-review"].ToString();
                            w.myRating = mr;
                            //WineDet wd = new WineDet();

                            // w.available = wine["available"].ToString();
                            // w.num_merchants = wine["num_merchants"].ToString();
                            //w.num_reviews = wine["num_reviews"].ToString();

                            MyRetWines.Add(w);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return MyRetWines;
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

        public async Task<List<string>> ReadId()
        {
            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("Assets\\wine.data.txt");

            var stream = await file.OpenReadAsync();

            var rdr = new StreamReader(stream.AsStream());

            var contents = await rdr.ReadToEndAsync();

            var lines = contents.Split('|');
            List<string> ids = new List<string>();
            for (int i = 0; i < lines.Length; i++)
            {
                var res = lines[i].Split(',');
                ids.Add(res[13]);

            }
            return ids;
        }

        public async Task<List<WineItem>> GetWines()
        {

            List<string> ids = await ReadId();


            foreach (string id in ids)
            {
                WineItem wine = await GetWebResponse(id);
                AllWines.Add(wine);
            }
            return AllWines;
        }
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
                
                GetWinesFly = new GetWinesFlyout(AbsolutePosition.X - BtnGetWines.ActualWidth * 2-60, this.ActualHeight);
                
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

        
    }
    }


