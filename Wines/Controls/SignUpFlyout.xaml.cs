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
    public sealed partial class SignUpFlyout : UserControl
    {
        ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
        public SignUpFlyout SignUpFly = null;
        public LoginFlyout LoginFly = null;
        public RateWineFlyout RateWineFly = null;

        public SignUpFlyout(){}
        public double x;
        public double y;

        public string username;
        public string password;
        public string email;
        public WineSearchItem wine;

        public SignUpFlyout(double x, double y, WineSearchItem wsi)
        {
            this.InitializeComponent();
             
            this.x = x;
            this.y = y;
            wine = wsi;


            if (!SignUpPopup.IsOpen)
            {
                SignUpPopup.HorizontalOffset = x;
                SignUpPopup.VerticalOffset = y;
                SignUpPopup.IsOpen = true;
            }
            else
            {
                SignUpPopup.IsOpen = false;
            }
        }
        public void Close()
        {
            SignUpPopup.IsOpen = false;
        }

        public void Open()
        {
            SignUpPopup.IsOpen = true;
        }

        public bool IsOpen()
        {
            return SignUpPopup.IsOpen;
        }
        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            if (SignUpPopup.IsOpen)
                SignUpPopup.IsOpen = false;
        }
        private async void SignUp_Click(object sender, RoutedEventArgs e)
        {
            if (SignUpPopup.IsOpen)
                SignUpPopup.IsOpen = false;
            string res = await GetSignUpResponse();
            if (res == "Registration successfull")
            {
                if (wine == null)
                {
                    roamingSettings.Values["username"] = username;
                    roamingSettings.Values["password"] = password;
                    Frame frame = Window.Current.Content as Frame;
                    SignUpPopup.IsOpen = false;
                    frame.Navigate(typeof(MainPage));
                }
                else
                {
                    MessageDialog MBox = new MessageDialog(res);
                    await MBox.ShowAsync();
                    roamingSettings.Values["username"] = username;
                    roamingSettings.Values["password"] = password;
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
                MessageDialog MBox = new MessageDialog(res);
                await MBox.ShowAsync();
            }

        }
        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            if (SignUpPopup.IsOpen)
                SignUpPopup.IsOpen = false;
            var transform = SignUp.TransformToVisual(MainGrid);
            Point AbsolutePosition = transform.TransformPoint(new Point(0, 0));
            if (LoginFly == null)
            {

                Rect bounds = Window.Current.Bounds;
                double height = bounds.Height;
                double width = bounds.Width;

                LoginFly = new LoginFlyout(AbsolutePosition.X+40 , AbsolutePosition.Y+360,wine);
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
        private async Task<string> GetSignUpResponse()
        {
            username = Username.Text;
            password = Password.Password;
            email = Mail.Text;
            string resp = "";
            string uri = "http://api.snooth.com/create-account/?akey=nxe7rzjlof7xl0t2d6z9axbncz26du2jz84i8hw06ec9x9z7&e="+email+"&p="+password+"&s="+username;
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

                    if (msg != "")
                        resp = msg;
                    if (msg == "")
                    {
                        if (status == "1")
                            resp = "Registration successfull";
                        else
                            resp = "Unknown Error.... Please try again...";
                    }
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
