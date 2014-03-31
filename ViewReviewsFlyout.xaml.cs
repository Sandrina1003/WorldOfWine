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
    public sealed partial class ViewReviewsFlyout : UserControl
    {
       
       
        public ViewReviewsFlyout(){}
        public ViewReviewsFlyout(double x, double y, List<Review> ReviewsList)
        {
            this.InitializeComponent();
            
            //Rating.Value=Convert.ToDouble( rating);
            //Review.Text = review;
            //Date.Text = date;
            for (int i = 0; i < ReviewsList.Count; i++)
                ReviewsList[i].index = i + 1;
            ReviewsLView.ItemsSource = ReviewsList;

            if (!ViewReviewsPopup.IsOpen)
            {
                ViewReviewsPopup.HorizontalOffset = x;
                ViewReviewsPopup.VerticalOffset = y;
                ViewReviewsPopup.IsOpen = true;
            }
            else
            {
                ViewReviewsPopup.IsOpen = false;
            }
        }
        public void Close()
        {
            ViewReviewsPopup.IsOpen = false;
        }

        public void Open()
        {
            ViewReviewsPopup.IsOpen = true;
           
        }

        public bool IsOpen()
        {
            return ViewReviewsPopup.IsOpen;
        }
        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            if (ViewReviewsPopup.IsOpen)
                ViewReviewsPopup.IsOpen = false;
        }
    }
}
