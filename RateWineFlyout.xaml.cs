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
using System.Threading.Tasks;
using Windows.Storage;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using Windows.UI.Popups;
// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Wines.Controls
{
    public sealed partial class RateWineFlyout : UserControl
    {
        ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;

        

        public double x;
        public double y;

        public string username;
        public string password;
        public string rating;
        public string review;
        public string ID;
        public WineSearchItem wine;
        

        public RateWineFlyout(){}
         public RateWineFlyout(double x, double y, WineSearchItem wsi)
        {
            this.InitializeComponent();
            this.x = x;
            this.y = y;
            wine = wsi;
           // roamingSettings.Values["username"] = null;
           // roamingSettings.Values["password"] = null;


            if (!RateWinePopup.IsOpen)
            {
                RateWinePopup.HorizontalOffset = x;
                RateWinePopup.VerticalOffset = y;
                RateWinePopup.IsOpen = true;
            }
            else
            {
                RateWinePopup.IsOpen = false;
            }
        }
        public void Close()
        {
            RateWinePopup.IsOpen = false;
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            if (RateWinePopup.IsOpen)
                RateWinePopup.IsOpen = false;
        }

        public void Open()
        {
            RateWinePopup.IsOpen = true;
        }

        public bool IsOpen()
        {
            return RateWinePopup.IsOpen;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (RateWinePopup.IsOpen)
                RateWinePopup.IsOpen = false;
        }
        private async void Post_Click(object sender, RoutedEventArgs e)
        {

            string response=await RateWineResponse();
            if (response == "1")
            {
                string res = "Your Rating is posted successfully. Chears ! ! !";
                MessageDialog MBox = new MessageDialog(res);
                await MBox.ShowAsync();
            }
            else
            {
                MessageDialog MBox = new MessageDialog(response);
                await MBox.ShowAsync();
            }
        }

        private async Task<string> RateWineResponse()
        {
            ID = wine.Code;
            rating = UserRating.Value.ToString();
            review = Review.Text;
            username = roamingSettings.Values["username"].ToString();
            password = roamingSettings.Values["password"].ToString();
            string status = "";
            string msg="";
            string uri = "https://api.snooth.com/rate/?akey=nxe7rzjlof7xl0t2d6z9axbncz26du2jz84i8hw06ec9x9z7&u="+username+"&p="+password+"&id="+ID+"&r="+rating+"&b="+review;
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
                    JObject res = (JObject)result["meta"];
                     msg = res["errmsg"].ToString();
                    status = res["status"].ToString();

                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            if (status == "1")
                return status;
            else
                return msg;
        }
    }
}
