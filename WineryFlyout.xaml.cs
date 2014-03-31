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
using System.Threading.Tasks;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Wines.Controls
{
    public sealed partial class WineryFlyout : UserControl
    {
        public double x;
        public double y;
        public WineryFlyout() { }

        public  WineryFlyout(double x, double y, Winery winery)
        {
            this.InitializeComponent();
            this.x = x;
            this.y = y;
            
            


            if (!WineryPopup.IsOpen)
            {
                WineryPopup.HorizontalOffset = x;
                WineryPopup.VerticalOffset = y;
                FillDetails(winery);

                
                WineryPopup.IsOpen = true;
            }
            else
            {
                WineryPopup.IsOpen = false;
            }
        }
        public void Close()
        {
            WineryPopup.IsOpen = false;
        }

        public void Open()
        {
            WineryPopup.IsOpen = true;
           
        }

        public bool IsOpen()
        {
            return WineryPopup.IsOpen;
        }
        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            if (WineryPopup.IsOpen)
                WineryPopup.IsOpen = false;
        }

        public void FillDetails(Winery winery)
        {
            if (winery.image != null)
                Image.Source = new BitmapImage(new Uri(winery.image, UriKind.Absolute));
            else
            {
                Uri uri = new Uri("ms-appx:///Assets/Winery.jpg");
                ImageSource imgSource = new BitmapImage(uri);
                Image.Source = imgSource;
            }
            if(winery.description!=null)
            Description.Text = winery.description;
            if(winery.name!=null)
            Title.Text = winery.name;
            if(winery.url!=null)
            Url.Text = winery.url;
            if(winery.phone !=null)
            Phone.Text = winery.phone;
            if(winery.address !=null)
            Street.Text = winery.address;
            if(winery.city !=null)
            City.Text = winery.city;
            if(winery.country!=null)
            Country.Text = winery.country;
            if (winery.email != null) 
            Email.Text= winery.email;
           

        }
        
    }
}
