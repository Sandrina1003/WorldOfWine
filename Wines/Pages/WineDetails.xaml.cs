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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Wines.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class WineDetails : Wines.Common.LayoutAwarePage
    {
        ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
        public LoginFlyout LoginFly = null;
        public ViewReviewsFlyout ViewReviewsFly = null;
        public GalleryFlyout GalleryFly = null;
        public RateWineFlyout RateWineFly = null;
        public AlreadyRatedFlyout AlreadyRatedFly = null;
        public GetWinesFlyout GetWinesFly = null;
        public WineryFlyout WineryFly = null;
        public string rating;
        public string review;
        public string date;
        WineSearchItem wine;

        public bool rated = false;
        
        public WineDetails()
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
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            wine = e.Parameter as WineSearchItem;
            if (wine.Photos == null || wine.Photos.Count==0)
                btnGallery.IsEnabled = false;
            FillDetails(wine);

        }

        private void HomeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        } 

        public async void FillDetails(WineSearchItem wine)
        {

            List<MyRating> RWines = await AllRated();
            foreach (MyRating rw in RWines)
            {
                if (rw.code == wine.Code)
                {
                    rated = true;
                    review = rw.review;
                    rating = rw.rating;
                    date = rw.date;
                    break;
                }

            }
            
           // string im = "../Assets/WhiteWine1.jpg";
            string im = wine.Image.ToString();
            //BitmapImage bi3 = new BitmapImage();
            //bi3.UriSource = new Uri(im, UriKind.RelativeOrAbsolute);
            if (im != "ms-appx:///Assets/MyWine.png" && im != "ms-appx:///Assets/RedWine.png" && im != "ms-appx:///Assets/WhiteWine.png")
           {
                Uri uri = new Uri(im, UriKind.RelativeOrAbsolute);

                ImageSource imgSource = new BitmapImage(uri);

                WineImage.Source = imgSource;
            }
            else
            {

                Uri uri = new Uri(im);
                ImageSource imgSource = new BitmapImage(uri);
                WineImage.Source = imgSource;
            }
            if(wine.Recipes !=null)
            FoodLView.ItemsSource = wine.Recipes;
            try { AvRating.Value = Convert.ToDouble(wine.Snoothrank); }
            
                catch (Exception ex)
            {
                AvRating.Value = 0;
            }
            
            
            if (wine.Name != null)
            pageTitle.Text = wine.Name.ToString();
            //WineImage.Source = bi3;
            if (wine.Price != null)
            WinePrice.Text = wine.Price.ToString();
            if (wine.Region != null)
            WineRegion.Text = wine.Region.ToString();
            if (wine.Link != null)
            WineMore.Text = wine.Link;
            if (wine.Type != null)
            WineType.Text = wine.Type;
            if (wine.Winery != null)
            WineWinery.Text = wine.Winery;
            if (wine.Vintage != null)
            WineVintage.Text = wine.Vintage;
            if (wine.Varietal != null)
            WineVarietal.Text = wine.Varietal;
            //wine.WineDetails =await ReadData(wine.ID);
            roamingSettings.Values["ID"] = wine.ID;
           // roamingSettings.Values["ID"] = "1480";
            //WineDet wineDet = await ReadData("1480");
            alcohol.Text = wine.WineDetails.Alcohol;
            alcalinity_of_ash.Text= wine.WineDetails.Alcalinity;
            ash.Text= wine.WineDetails.Ash;
            flavanoids.Text=wine.WineDetails.Flavanoids;
            hue.Text= wine.WineDetails.Hue;
            magnesium.Text= wine.WineDetails.Magnesium;
            malic_acid.Text= wine.WineDetails.Malic_acid;
            nonflavanoids_phenols.Text= wine.WineDetails.Nonflavanoid_phenols;
            proanthocyanins.Text=wine.WineDetails.Proanthocyanins;
            proline.Text= wine.WineDetails.Proline;
            total_phenols.Text= wine.WineDetails.Total_phenols;
            color_intensity.Text= wine.WineDetails.Color_intensity;
        }



        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        private async void BtnWinery_Click(object sender, RoutedEventArgs e)
        {

            var transform = BtnWinery.TransformToVisual(MainGrid);
            Point AbsolutePosition = transform.TransformPoint(new Point(0, 0));
            if (WineryFly == null)
            {
                Rect bounds = Window.Current.Bounds;
                double height = bounds.Height;
                double width = bounds.Width;
                //GetWinesFly.Width = bounds.Width;
                Winery winery = await GetWineryResponse(wine.Winery_ID);
                WineryFly = new WineryFlyout(AbsolutePosition.X-290, 0,winery);
            }
            else
            {
                if (WineryFly.IsOpen())
                {
                    WineryFly.Close();
                }
                else
                {
                    WineryFly.Open();
                }
            }
        }

        private void Gallery_Click(object sender, RoutedEventArgs e)
        {
            
            var transform = btnGallery.TransformToVisual(MainGrid);
            Point AbsolutePosition = transform.TransformPoint(new Point(0, 0));
            if (GalleryFly == null)
            {
                Rect bounds = Window.Current.Bounds;
                double height = bounds.Height;
                double width = bounds.Width;
                //GetWinesFly.Width = bounds.Width;

                GalleryFly = new GalleryFlyout(100, 100 , wine.Photos);
            }
            else
            {
                if (GalleryFly.IsOpen())
                {
                    GalleryFly.Close();
                }
                else
                {
                    GalleryFly.Open();
                }
            }
        }
        
        private void Rating_Click(object sender, RoutedEventArgs e)
        {
            

            if (roamingSettings.Values["username"] == null || roamingSettings.Values["password"] == null)
            {
                var transform = Rating.TransformToVisual(MainGrid);
                Point AbsolutePosition = transform.TransformPoint(new Point(0, 0));
                if (LoginFly == null)
                {

                    Rect bounds = Window.Current.Bounds;
                    double height = bounds.Height;
                    double width = bounds.Width;

                    LoginFly = new LoginFlyout(AbsolutePosition.X * 2, AbsolutePosition.Y - 310,wine);
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


            else
              if (rated)
                {
                    var transform = Rating.TransformToVisual(MainGrid);
                    Point AbsolutePosition = transform.TransformPoint(new Point(0, 0));
                    if (AlreadyRatedFly == null)
                    {
                        Rect bounds = Window.Current.Bounds;
                        double height = bounds.Height;
                        double width = bounds.Width;
                        //GetWinesFly.Width = bounds.Width;

                        AlreadyRatedFly = new AlreadyRatedFlyout(AbsolutePosition.X * 2, AbsolutePosition.Y-290,review,date,rating);
                    }
                    else
                    {
                        if (AlreadyRatedFly.IsOpen())
                        {
                            AlreadyRatedFly.Close();
                        }
                        else
                        {
                            AlreadyRatedFly.Open();
                        }
                    }
                }
                else
                {
                    var transform = Rating.TransformToVisual(MainGrid);
                    Point AbsolutePosition = transform.TransformPoint(new Point(0, 0));
                    if (RateWineFly == null)
                    {
                         Rect bounds = Window.Current.Bounds;
                         double height = bounds.Height;
                        double width = bounds.Width;
                        //GetWinesFly.Width = bounds.Width;

                        RateWineFly = new RateWineFlyout(AbsolutePosition.X * 2, AbsolutePosition.Y - Rating.ActualHeight - 130,wine);
                    }
                    else
                    {
                        if (RateWineFly.IsOpen())
                        {
                            RateWineFly.Close();
                        }
                        else
                        {
                            RateWineFly.Open();
                        }
                    }
                }
           }
        
        public async Task<List<MyRating>> AllRated()
        {
            List<MyRating> wList = new List<MyRating>();
            string uri = "http://api.snooth.com/my-wines/?akey=nxe7rzjlof7xl0t2d6z9axbncz26du2jz84i8hw06ec9x9z7&u=sandrina.filipovska@gmail.com&p=sanibani";
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
                        MyRating w = new MyRating();
                        w.id = roamingSettings.Values["ID"].ToString();
                        w.name = wine["name"].ToString();
                        w.code = wine["code"].ToString();
                        w.rating = wine["my-rating"].ToString();
                        w.review = wine["my-review"].ToString();
                        w.date = wine["date_activity"].ToString();
                        wList.Add(w);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return wList;
        }

        public async Task<Winery> GetWineryResponse(string id)
        {
            Winery w = new Winery();
            string uri = "http://api.snooth.com/winery/?akey=nxe7rzjlof7xl0t2d6z9axbncz26du2jz84i8hw06ec9x9z7&id=" + id;
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
                    JObject winery = (JObject)result["winery"];



                    
                        w.id = id;

                        w.name = winery["name"].ToString();
                        w.address = winery["address"].ToString();
                        w.city = winery["city"].ToString();
                        w.state = winery["state"].ToString();
                        w.zip = winery["zip"].ToString();
                        w.country = winery["country"].ToString();
                        w.phone = winery["phone"].ToString();
                        w.url = winery["url"].ToString();
                        w.email = winery["email"].ToString();
                        w.latitude = winery["lat"].ToString();
                        w.longitude = winery["lng"].ToString();
                        w.image = winery["image"].ToString();
                        w.description = winery["description"].ToString();

                    
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return w;
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
        private void BtnViewReviews_Click(object sender, RoutedEventArgs e)
        {
            var transform = BtnViewReviews.TransformToVisual(MainGrid);
            Point AbsolutePosition = transform.TransformPoint(new Point(0, 0));
            if (ViewReviewsFly == null)
            {
                Rect bounds = Window.Current.Bounds;
                //GetWinesFly.Width = bounds.Width;

                ViewReviewsFly = new ViewReviewsFlyout(AbsolutePosition.X - BtnViewReviews.ActualWidth * 2-40, 100, wine.Reviews);

            }
            else
            {
                if (ViewReviewsFly.IsOpen())
                {
                    ViewReviewsFly.Close();
                }
                else
                {
                    ViewReviewsFly.Open();
                }
            }
        }
     

        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        //public class WineDet
        //{
        //   public string Alcohol {get; set;}
        //   public string Malic_acid { get; set; }
        //   public string Ash { get; set; }
        //   public string Alcalinity { get; set; }
        //   public string Magnesium { get; set; }
        //   public string Total_phenols { get; set; }
        //   public string Flavanoids { get; set; }
        //   public string Nonflavanoid_phenols { get; set; }
        //   public string Proanthocyanins { get; set; }
        //   public string Color_intensity { get; set; }
        //   public string Hue { get; set; }
        //   public string Proline { get; set; }
        //}
        //public class RatedWine
        //{
        //    public string id { get; set; }
        //    public string name { get; set; }
        //    public string code { get; set; }
        //    public string rating { get; set; }
        //    public string review { get; set; }
        //    public string date { get; set; }
        //}
    }
}
