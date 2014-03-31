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
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Wines
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public LoginFlyout LoginFly = null;
        ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
        string Red = "There are a countless number of red grape varieties in the world,some able to make wine, others best suited for Welch's grape juice. Right now, the world wine market focuses on about 40 to 50 different red wine grape varieties, the most widely recognized and used listed below." +
                             "\n\n What differentiates red wine from white is first, the skin color of the grape, and second, the amount of time the grape juice has with its skins. After picking, red grapes are put into tanks or barrels where they soak with their skins, absorbing pigments and other" +
                             "aspects of the grape skin, such as tannins. This is how red wine gets its red color. The exact color, which can range from light red to almost purple, depends on both the color of the particular grape skin and the amount of time spent on the skins. Remember, the inside of almost all grapes is a light, golden color – it's the skins that have the pigment. For example, much of" +
                             "Champagne is made from Pinot Noir and/or Pinot Meunier, both red grapes. Yet because the juice for Champagne is pressed quickly, with little time on the skins, the color of Champagne is often white.";
                             
        string White = "White wine differs from red wine in, first and most obviously, color. Under that skin, the pulpy part of a white grape is the same color as that of a red grape. The skin dictates the end color for red wine, which differs from the white's color determinates." +
                           "<\n\nThis is mainly due to the pressing of the grapes. When white grapes are picked, they are immediately pressed and the juice is removed from the skins with little contact." +
                           "\n\nColor in white wine does vary, often from the type of grape, occasionally from the use of wood. Listed below are a few of the most common white varieties in the world wine market and of wine.com. They are listed from lighter bodied, and lighter colored, to fuller bodied with deeper colors.";
        string UserName;
        string Password;
        public GetWinesFlyout GetWinesFly = null;
        public GalleryFlyout GalleryFly = null;
        List<Photo> photos = new List<Photo>();
       

        public MainPage()
        {
            this.InitializeComponent();

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

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RedDescription.Text = Red;
            WhiteDescription.Text = White;
            if (roamingSettings.Values["username"] == null || roamingSettings.Values["password"] == null)
            {

                LogoutButton.Visibility = Visibility.Collapsed;
                Login.IsEnabled = true;
            }
            else
            {
                Login.IsEnabled = false;
                UserName = roamingSettings.Values["username"] as String;
                Password = roamingSettings.Values["password"] as String;
                User.Text = UserName;
                LogoutButton.Visibility = Visibility.Visible;
            }
            
        }
        
        

        private void Red_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(WinesGroup), "Red");
        }

        private void White_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(WinesGroup), "White");
        }

        private async void My_Click(object sender, RoutedEventArgs e)
        {
            if (roamingSettings.Values["username"] == null || roamingSettings.Values["password"] == null)
            {

                string res = "You have to sign in in order to access this group...";
                MessageDialog MBox = new MessageDialog(res);
                await MBox.ShowAsync();
                

            }
            else
            this.Frame.Navigate(typeof(MyWines), "My");
        }

        private void LogoutButton_Click(object sender, TappedRoutedEventArgs e)
        {
            roamingSettings.Values["username"] = null;
            roamingSettings.Values["password"] = null;
            LogoutButton.Visibility = Visibility.Collapsed;
            Login.IsEnabled = true;
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

    public class MyRating
    {
        public string id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string rating { get; set; }
        public string review { get; set; }
        public string date { get; set; }
        public bool rated { get; set; }
    }

    public class Photo
    {
        public string photo { get; set; }
    }

    public class Recipe
    {
        public string name { get; set; }
        public string link { get; set;  }
        public string source_link { get; set; }
        public string image { get; set; }
    }
    public class WineSearchItem
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Region { get; set; }
        public string Winery { get; set; }
        public string Winery_ID { get; set; }
        public string Varietal { get; set; }
        public string Image { get; set; }
        public string Price { get; set; }
        public string Vintage { get; set; }
        public string Type { get; set; }
        public string Link { get; set; }
        public string Snoothrank { get; set; }
        public string Available { get; set; }
        public string Num_merchants { get; set; }
        public string Num_reviews { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Recipe> Recipes { get; set; }
        public WineDet WineDetails { get; set; }
        public MyRating MyRating { get; set; }
        public double Distance { get; set; }
        public List<Photo> Photos { get; set; }
    }

    public class Winery
    {
        public string id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public string phone { get; set; }
        public string url { get; set; }
        public string email { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string image { get; set; }
        public string description { get; set; }
    }

    public class WineItem
    {
        public string id { get; set; }
        public string name { get; set; }
        public string region { get; set; }
        public string winery { get; set; }
        public string winery_id { get; set; }
        public string varietal { get; set; }
        public string code { get; set; }
        public string price { get; set; }
        public string vintage { get; set; }
        public string type { get; set; }
        public string link { get; set; }
        public string image { get; set; }
        public string snoothrank { get; set; }
        public string available { get; set; }
        public string num_merchants { get; set; }
        public string num_reviews { get; set; }
        public List<Review> reviews { get; set; }
        public List<Recipe> recipes { get; set; }
        public WineDet wineDet { get; set; }
        public MyRating myRating { get; set; }
        public double distance { get; set; }
        public List<Photo> photos { get; set; }

    }

    public class WineDet
    {
        public string Alcohol { get; set; }
        public string Malic_acid { get; set; }
        public string Ash { get; set; }
        public string Alcalinity { get; set; }
        public string Magnesium { get; set; }
        public string Total_phenols { get; set; }
        public string Flavanoids { get; set; }
        public string Nonflavanoid_phenols { get; set; }
        public string Proanthocyanins { get; set; }
        public string Color_intensity { get; set; }
        public string Hue { get; set; }
        public string Proline { get; set; }
        public string ID { get; set; }
        public string Class { get; set; }
    }

    public class Review
    {
        public string name { get; set; }
        public string rating { get; set; }
        public string date { get; set; }
        public string lang { get; set; }
        public string body { get; set; }
        public int index { get; set; }
    }
}
