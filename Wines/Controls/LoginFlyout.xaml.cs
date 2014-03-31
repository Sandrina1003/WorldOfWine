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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.Storage;
// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Wines.Controls
{
    public sealed partial class LoginFlyout : UserControl
    {
        ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
        public SignUpFlyout SignUpFly = null;
        public RateWineFlyout RateWineFly = null;

        public LoginFlyout(){}
         public double x;
        public double y;

        public string username;
        public string password;
        public string rating;
        public string review;
        public WineSearchItem wine;

         public LoginFlyout(double x, double y, WineSearchItem wsi)
        {
            this.InitializeComponent();
             
            this.x = x;
            this.y = y;
            wine = wsi;

            if (!LoginPopup.IsOpen)
            {
                LoginPopup.HorizontalOffset = x;
                LoginPopup.VerticalOffset = y;
                LoginPopup.IsOpen = true;
            }
            else
            {
                LoginPopup.IsOpen = false;
            }
        }
        public void Close()
        {
            LoginPopup.IsOpen = false;
        }

        public void Open()
        {
            LoginPopup.IsOpen = true;
        }

        public bool IsOpen()
        {
            return LoginPopup.IsOpen;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (LoginPopup.IsOpen)
                LoginPopup.IsOpen = false;

           
        }
        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            if (LoginPopup.IsOpen)
                LoginPopup.IsOpen = false;
        }
        private async void SignIn_Click(object sender, RoutedEventArgs e)
        {
            if (LoginPopup.IsOpen)
                LoginPopup.IsOpen = false;

            string res = await GetLoginResponse();
            if (res == "Successfull")
            {
                roamingSettings.Values["username"] = username;
                roamingSettings.Values["password"] = password;
                if (wine == null)
                {
                        string r = "You are Loged In...";
                        MessageDialog MBox = new MessageDialog(r);
                        await MBox.ShowAsync();
                        Frame frame = Window.Current.Content as Frame;
                        LoginPopup.IsOpen = false;
                        frame.Navigate(typeof(MainPage));
                }
                else
                {
                    var transform = SignIn.TransformToVisual(MainGrid);
                    Point AbsolutePosition = transform.TransformPoint(new Point(0, 0));
                    if (RateWineFly == null)
                    {

                        Rect bounds = Window.Current.Bounds;
                        double height = bounds.Height;
                        double width = bounds.Width;

                        RateWineFly = new RateWineFlyout(AbsolutePosition.X, 480, wine);
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
            else
            {
                MessageDialog MBox = new MessageDialog("Username or password incorect !");
                await MBox.ShowAsync();
            }
        }
        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            if (LoginPopup.IsOpen)
                LoginPopup.IsOpen = false;
            var transform = SignUp.TransformToVisual(MainGrid);
            Point AbsolutePosition = transform.TransformPoint(new Point(0, 0));
            if (SignUpFly == null)
            {

                Rect bounds = Window.Current.Bounds;
                double height = bounds.Height;
                double width = bounds.Width;

                SignUpFly = new SignUpFlyout(AbsolutePosition.X-10 , AbsolutePosition.Y+290,wine);
            }
            else
            {
                if (SignUpFly.IsOpen())
                {
                    SignUpFly.Close();
                }
                else
                {
                    SignUpFly.Open();

                }
            }
        }
        private async Task<string> GetLoginResponse()
        {
            username = Username.Text;
            password = Password.Password;
            string resp="";
            string uri = "https://api.snooth.com/rate/?akey=nxe7rzjlof7xl0t2d6z9axbncz26du2jz84i8hw06ec9x9z7&u="+username+"&p="+password;
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
                    string msg = res["errmsg"].ToString();
                    string status = res["status"].ToString();

                    if (msg != "invalid user")
                    {
                        resp = "Successfull";
                        MainPage mp = new MainPage();
                        mp.LogoutButton.Visibility = Visibility.Visible;
                    }
                    else
                        resp = "Unsuccessfull";
                    }
                }
            
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return resp;
        }
    }
    }


