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
using Windows.Storage;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Windows.UI.Popups;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Wines.Controls
{
    public sealed partial class AlreadyRatedFlyout : UserControl
    {
        public AlreadyRatedFlyout() {}
        ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;

        

        public double x;
        public double y;

        //public string username;
        //public string password;
        //public string rating;
        //public string review;
        //public string ID;


        public  AlreadyRatedFlyout(double x, double y, string review, string date, string rating)
        {
            this.InitializeComponent();
            this.x = x;
            this.y = y;
            UserRating.Value=Convert.ToDouble( rating);
            Review.Text = review;
            Date.Text = date;


            if (!AlreadyRatedPopup.IsOpen)
            {
                AlreadyRatedPopup.HorizontalOffset = x;
                AlreadyRatedPopup.VerticalOffset = y;

                AlreadyRatedPopup.IsOpen = true;
            }
            else
            {
                AlreadyRatedPopup.IsOpen = false;
            }
        }
        public void Close()
        {
            AlreadyRatedPopup.IsOpen = false;
        }

        public void Open()
        {
            AlreadyRatedPopup.IsOpen = true;
           
        }

        public bool IsOpen()
        {
            return AlreadyRatedPopup.IsOpen;
        }
        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            if (AlreadyRatedPopup.IsOpen)
                AlreadyRatedPopup.IsOpen = false;
        }
    }
}
