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
    public sealed partial class GalleryFlyout : UserControl
    {
        public GalleryFlyout() {}

        int i = 0;
        

        public double x;
        public double y;

        //public string username;
        //public string password;
        //public string rating;
        //public string review;
        //public string ID;
        List<Photo> im = new List<Photo>();

        public  GalleryFlyout(double x, double y, List<Photo> pictures)
        {
            this.InitializeComponent();
            this.x = x;
            this.y = y;
            im = pictures;

            if (!GalleryPopup.IsOpen)
            {
                GalleryPopup.HorizontalOffset = 100 ;
                GalleryPopup.VerticalOffset = 100;
                Image.Source = new BitmapImage(new Uri(pictures[0].photo, UriKind.Absolute));
                GalleryPopup.IsOpen = true;
            }
            else
            {
                GalleryPopup.IsOpen = false;
            }
        }
        public void Close()
        {
            GalleryPopup.IsOpen = false;
        }

        public void Open()
        {
            GalleryPopup.IsOpen = true;
           
        }

        public bool IsOpen()
        {
            return GalleryPopup.IsOpen;
        }
        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            if (GalleryPopup.IsOpen)
                GalleryPopup.IsOpen = false;
        }
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (i == 0)
                i = im.Count;
                i--;
            
            Image.Source = new BitmapImage(new Uri(im[i].photo, UriKind.Absolute));
            
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            i++;
            
                if (i == im.Count)
                {
                    i = 0;
                    Image.Source = new BitmapImage(new Uri(im[i].photo, UriKind.Absolute));
                    
                }
                else
                {
                    Image.Source = new BitmapImage(new Uri(im[i].photo, UriKind.Absolute));
                    
                }
        }
    }
}
