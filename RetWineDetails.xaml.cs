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
    public sealed partial class RetWineDetails : Wines.Common.LayoutAwarePage
    {
        ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
        public ViewReviewsFlyout ViewReviewsFly = null;
        public GalleryFlyout GalleryFly = null;
        public GetWinesFlyout GetWinesFly = null;
        public WineryFlyout WineryFly = null;
        public LoginFlyout LoginFly = null;
        public string rating;
        public string review;
        public string date;
        WineSearchItem wine;
        WinesGroup wg = new WinesGroup();
        WineItem v = new WineItem();
       public ObservableCollection<WineSearchItem> WineIt;

        public RetWineDetails()
        {
            this.InitializeComponent();
        }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            WineIt = new ObservableCollection<WineSearchItem>();
            
            if (roamingSettings.Values["username"] != null && roamingSettings.Values["password"] != null)
                Login.IsEnabled = false;
            wine = e.Parameter as WineSearchItem;
            v=await wg.GetWebResponse(wine.Code);
            if (v.photos == null || v.photos.Count == 0)
                btnGallery.IsEnabled = false;
            else
                btnGallery.IsEnabled = true;
                WineIt.Add(new WineSearchItem()
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

            FillDetails(WineIt[0]);

        }

        

        public async void FillDetails(WineSearchItem wine)
        {

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
            if (wine.Recipes != null)
                FoodLView.ItemsSource = wine.Recipes;
            if (wine.Snoothrank != null)
                AvRating.Value = Convert.ToDouble(wine.Snoothrank);

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
            pageTitle.Text = wine.Name;
        }

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
                WineDetails wd = new WineDetails();
                Winery winery = await wd.GetWineryResponse(wine.Winery_ID);
                WineryFly = new WineryFlyout(AbsolutePosition.X - 290, 0, winery);
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

        private void BtnViewReviews_Click(object sender, RoutedEventArgs e)
        {
            var transform = BtnViewReviews.TransformToVisual(MainGrid);
            Point AbsolutePosition = transform.TransformPoint(new Point(0, 0));
            if (ViewReviewsFly == null)
            {
                Rect bounds = Window.Current.Bounds;
                //GetWinesFly.Width = bounds.Width;

                ViewReviewsFly = new ViewReviewsFlyout(AbsolutePosition.X - BtnViewReviews.ActualWidth * 2 - 40, 100, wine.Reviews);

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

                GalleryFly = new GalleryFlyout(100, 100, WineIt[0].Photos);
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

        private void HomeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
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

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }
    }
}
