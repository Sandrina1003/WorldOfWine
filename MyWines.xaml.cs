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
    public sealed partial class MyWines : Wines.Common.LayoutAwarePage
    {
        ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
        public GetWinesFlyout GetWinesFly = null;
        public ObservableCollection<WineSearchItem> MyWinesList;
        public ObservableCollection<WineSearchItem> MyRecommendedList;
        WinesGroup wg = new WinesGroup();
        List<WineItem> Wines = new List<WineItem>();
        List<WineItem> recWines = new List<WineItem>();
        List<WineItem> test = new List<WineItem>();
        // MainPage mp = new MainPage();
        //List<WineItem> MyWi = new List<WineItem>();

        public MyWines()
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

            await FillData();

            await GetRecommendatons(MyWinesList);

            WinesLView.ItemsSource = MyWinesList;
            RecWinesLView.ItemsSource = MyRecommendedList;
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

        //public async Task<List<MyRating>> AllRated()
        //{
        //    List<MyRating> wList = new List<MyRating>();
        //    string uri = "http://api.snooth.com/my-wines/?akey=nxe7rzjlof7xl0t2d6z9axbncz26du2jz84i8hw06ec9x9z7&u=sandrina.filipovska@gmail.com&p=sanibani";
        //    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(uri));
        //    HttpClient httpClient = new HttpClient();

        //    try
        //    {
        //        HttpResponseMessage httpResponse = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
        //        HttpStatusCode statusCode = httpResponse.StatusCode;
        //        HttpContent responseContent = httpResponse.Content;

        //        if (responseContent != null)
        //        {
        //            string results = await responseContent.ReadAsStringAsync();
        //            // txtResponse.Text = results;
        //            JObject result = JObject.Parse(results);
        //            JArray wines = (JArray)result["wines"];



        //            foreach (JObject wine in wines)
        //            {
        //                MyRating w = new MyRating();
        //                //w.id = roamingSettings.Values["ID"].ToString();
        //                w.name = wine["name"].ToString();
        //                w.code = wine["code"].ToString();
        //                w.rating = wine["my-rating"].ToString();
        //                w.review = wine["my-review"].ToString();
        //                w.date = wine["date_activity"].ToString();
        //                wList.Add(w);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.Message);
        //    }
        //    return wList;
        //}

        public async Task FillRecommended(List<WineItem> recWines)
        {

            MyRecommendedList = new ObservableCollection<WineSearchItem>();

            RemoveDup(recWines);
            for (int i = 0; i < MyWinesList.Count; i++)
            {
                for (int j = 0; j < recWines.Count; j++)
                    if (MyWinesList[i].ID == recWines[j].id)
                        recWines.Remove(recWines[j]);
            }
            QuickSort(recWines, 0, Wines.Count - 1);
            List<string> ids = await wg.ReadId();
            int br = 0;
            foreach (WineItem v in recWines)
            {
                //string id;

                for (int i = 0; i < Wines.Count; i++)
                {
                    if (v.code == Wines[i].code)
                    {
                        if (v.image == "")
                            v.image = "ms-appx:///Assets/MyWine.png";
                        v.id = Wines[i].id;
                        MyRecommendedList.Add(new WineSearchItem()
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
                            Reviews = Wines[i].reviews,
                            ID = v.id,
                            Recipes = Wines[i].recipes,
                            MyRating = v.myRating,
                            Distance = v.distance,
                            WineDetails = await wg.ReadData(v.id)

                        });
                    }
                }
                br++;
                if (br == 15)
                    break;
            }

        }



        public async Task FillData()
        {

            //List<MyRating> RWines = await AllRated();
            //foreach (MyRating rw in RWines)
            //{
            //    if (rw.code == wine.Code)
            //    {
            //        rated = true;
            //        review = rw.review;
            //        rating = rw.rating;
            //        date = rw.date;
            //        break;
            //    }

            //}

            LoadingTextWines.Visibility = Visibility.Visible;
            LoadingWines.IsActive = true;
            LoadingTextRecWines.Visibility = Visibility.Visible;
            LoadingRecWines.IsActive = true;
            List<WineItem> MyWs = await wg.MyWinesResponse();
            if (MyWs.Count == 0)
            {

                string res = "You haven't rated any wine yet...";
                MessageDialog MBox = new MessageDialog(res);
                await MBox.ShowAsync();
                Frame.Navigate(typeof(MainPage));

            }
            MyWinesList = new ObservableCollection<WineSearchItem>();
            List<string> ids = await wg.ReadId();


            Wines = await wg.GetWines();
            foreach (WineItem v in MyWs)
            {
                //string id;

                for (int i = 0; i < Wines.Count; i++)
                {

                    if (v.code == Wines[i].code)
                    {
                        if (v.image == "")
                            v.image = "ms-appx:///Assets/MyWine.png";
                        v.id = Wines[i].id;
                        MyWinesList.Add(new WineSearchItem()
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
                            Reviews = Wines[i].reviews,
                            ID = v.id,
                            Recipes = Wines[i].recipes,
                            MyRating = v.myRating,
                            Distance = v.distance,
                            Photos=Wines[i].photos,
                            WineDetails = await wg.ReadData(v.id)

                        });
                    }
                }


                LoadingTextWines.Visibility = Visibility.Collapsed;
                LoadingWines.IsActive = false;
                LoadingTextRecWines.Visibility = Visibility.Collapsed;
                LoadingRecWines.IsActive = false;
            }
            RemoveDuplicate(MyWinesList);
            //RedWinesGView.ItemsSource = RedWinesList;
            //WhiteWinesGView.ItemsSource = WhiteWinesList;
            //SparklingWinesGView.ItemsSource = SparklingWines;
        }

        public async Task GetRecommendatons(ObservableCollection<WineSearchItem> MyList)
        {
            foreach (WineSearchItem ws in MyList)
            {

                KNN(ws);

            }

            //test = recWines;

            await FillRecommended(Wines);

        }

        public void KNN(WineSearchItem ws)
        {

            for (int i = 0; i < Wines.Count; i++)
            {


                double x1 = Convert.ToDouble(ws.WineDetails.Alcalinity);
                double x2 = Convert.ToDouble(Wines[i].wineDet.Alcalinity);
                double dist = Math.Sqrt(Math.Pow(Convert.ToDouble(ws.WineDetails.Alcalinity) - Convert.ToDouble(Wines[i].wineDet.Alcalinity), 2) +
                      Math.Pow(Convert.ToDouble(ws.WineDetails.Alcohol) - Convert.ToDouble(Wines[i].wineDet.Alcohol), 2) +
                      Math.Pow(Convert.ToDouble(ws.WineDetails.Ash) - Convert.ToDouble(Wines[i].wineDet.Ash), 2) +
                      Math.Pow(Convert.ToDouble(ws.WineDetails.Color_intensity) - Convert.ToDouble(Wines[i].wineDet.Color_intensity), 2) +
                      Math.Pow(Convert.ToDouble(ws.WineDetails.Flavanoids) - Convert.ToDouble(Wines[i].wineDet.Flavanoids), 2) +
                      Math.Pow(Convert.ToDouble(ws.WineDetails.Hue) - Convert.ToDouble(Wines[i].wineDet.Hue), 2) +
                      Math.Pow(Convert.ToDouble(ws.WineDetails.Magnesium) - Convert.ToDouble(Wines[i].wineDet.Magnesium), 2) +
                      Math.Pow(Convert.ToDouble(ws.WineDetails.Malic_acid) - Convert.ToDouble(Wines[i].wineDet.Malic_acid), 2) +
                      Math.Pow(Convert.ToDouble(ws.WineDetails.Nonflavanoid_phenols) - Convert.ToDouble(Wines[i].wineDet.Nonflavanoid_phenols), 2) +
                      Math.Pow(Convert.ToDouble(ws.WineDetails.Proanthocyanins) - Convert.ToDouble(Wines[i].wineDet.Proanthocyanins), 2) +
                      Math.Pow(Convert.ToDouble(ws.WineDetails.Proline) - Convert.ToDouble(Wines[i].wineDet.Proline), 2) +
                      Math.Pow(Convert.ToDouble(ws.WineDetails.Total_phenols) - Convert.ToDouble(Wines[i].wineDet.Total_phenols), 2) +
                      Math.Pow(Convert.ToDouble(ws.WineDetails.Class) - Convert.ToDouble(Wines[i].wineDet.Class), 2) +
                      Math.Pow(Convert.ToDouble(ws.MyRating.rating) - 5, 2));


                if (Wines[i].distance > dist || Wines[i].distance == 0)
                    Wines[i].distance = dist;

            }

            //recWines.Add(TwoMin[0]);
            //recWines.Add(TwoMin[1]);

        }

        public static void QuickSort(List<WineItem> lwi, int left, int right)
        {
            if (left < right)
            {
                int q = Partition(lwi, left, right);
                QuickSort(lwi, left, q - 1);
                QuickSort(lwi, q + 1, right);
            }
        }

        private static int Partition(List<WineItem> lwi, int left, int right)
        {
            double pivot = lwi[right].distance;
            double temp;

            int i = left;
            for (int j = left; j < right; j++)
            {
                if (lwi[j].distance <= pivot)
                {
                    temp = lwi[j].distance;
                    lwi[j].distance = lwi[i].distance;
                    lwi[i].distance = temp;
                    i++;
                }
            }

            lwi[right].distance = lwi[i].distance;
            lwi[i].distance = pivot;

            return i;
        }



        //public List<WineItem> FindTwoMin(List<WineItem> lwi, WineSearchItem ws)
        //{
        //    double minDist1 = lwi[0].distance;
        //    WineItem minWine1=lwi[0];
        //    double minDist2 = lwi[0].distance;
        //    WineItem minWine2 = lwi[0];
        //    List<WineItem> TwoRet=new List<WineItem>();
        //    for (int i = 0; i < lwi.Count; i++)
        //    {
        //        if (lwi[i].distance < minDist1 && lwi[i].id!=ws.ID)
        //        {
        //            minDist1 = lwi[i].distance;
        //            minWine1 = lwi[i];
        //            TwoRet.Add(lwi[i]);
        //        }
        //    }
        //    for (int i = 0; i < lwi.Count; i++)
        //    {
        //        if (lwi[i].distance < minDist2 && lwi[i].distance > minDist1 && lwi[i].id!=ws.ID)
        //            minDist2 = lwi[i].distance;
        //        minWine2 = lwi[i];
        //        TwoRet.Add(lwi[i]);
        //    }
        //        return TwoRet;
        //}
        public void RemoveDuplicate(ObservableCollection<WineSearchItem> ldwi)
        {
            for (int i = 0; i < ldwi.Count; i++)
            {
                for (int j = 0; j < ldwi.Count; j++)
                {
                    if (ldwi[i].ID == ldwi[j].ID && ldwi[i].Distance <= ldwi[j].Distance && i != j)
                        ldwi.Remove(ldwi[j]);
                    else
                        if (ldwi[i].ID == ldwi[j].ID && ldwi[i].Distance > ldwi[j].Distance && i != j)
                            ldwi.Remove(ldwi[i]);
                }
            }
        }

        public void RemoveDup(List<WineItem> ldwi)
        {
            for (int i = 0; i < ldwi.Count; i++)
            {
                for (int j = 0; j < ldwi.Count; j++)
                {
                    if (ldwi[i].id == ldwi[j].id && ldwi[i].distance <= ldwi[j].distance && i != j)
                        ldwi.Remove(ldwi[j]);
                    else
                        if (ldwi[i].id == ldwi[j].id && ldwi[i].distance > ldwi[j].distance && i != j)
                            ldwi.Remove(ldwi[i]);
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
