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
using Windows.UI.Popups;
using System.Threading.Tasks;
using Wines.Pages;




// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Wines.Controls
{
    public sealed partial class GetWinesFlyout : UserControl
    {
        public string colorP;
        public string typeP;
        public string countryP;
        public string minPriceP;
        public string maxPriceP;
        public string sortP;
        public string minRankP;
        public string maxRankP;

        public double x;
        public double y;

        public GetWinesFlyout() { }
        public GetWinesFlyout(double x, double y)
        {
            this.InitializeComponent();
            GetWinesPopup.Height = y;
            this.x = x;
            this.y = y;


            if (!GetWinesPopup.IsOpen)
            {
                GetWinesPopup.HorizontalOffset = x;
                GetWinesPopup.VerticalOffset = 0;
                
                GetWinesPopup.IsOpen = true;
            }
            else
            {
                GetWinesPopup.IsOpen = false;
            }
        }

        public void Close()
        {
            GetWinesPopup.IsOpen = false;
        }

        public void Open()
        {
            GetWinesPopup.IsOpen = true;
        }

        public bool IsOpen()
        {
            return GetWinesPopup.IsOpen;
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            if (GetWinesPopup.IsOpen)
                GetWinesPopup.IsOpen = false;
        }
        string url;
        public async void Get_Click(object sender, RoutedEventArgs e)
        {
            colorP = Get_Color();
            typeP = Get_Type();
            countryP = Get_Country();
            minPriceP = Get_MinPrice();
            maxPriceP = Get_MaxPrice();
            sortP = Get_Sort();
            minRankP = Get_MinRank();
            maxRankP = Get_MaxRank();
            if (Convert.ToInt16(minRankP) > Convert.ToInt16(maxRankP))
            {
                MessageDialog MBox = new MessageDialog("Minimum Rank must be smaller than Maximum Rank");
                await MBox.ShowAsync();
            }
            else
                if (minPriceP != "" && maxPriceP != "")
                {
                    if (Convert.ToInt16(minPriceP) > Convert.ToInt16(maxPriceP))
                    {
                        MessageDialog MBox = new MessageDialog("Minimum Price must be smaller than Maximum Price");
                        await MBox.ShowAsync();
                    }
                }
                else
                {
                    //url = " http://api.snooth.com/wine?akey=nxe7rzjlof7xl0t2d6z9axbncz26du2jz84i8hw06ec9x9z7&id=1480&food=1";
                    url = "http://api.snooth.com/wines/?akey=nxe7rzjlof7xl0t2d6z9axbncz26du2jz84i8hw06ec9x9z7&q=wine&a=0";
                    if (typeP != "")
                        url += "&t=" + typeP;
                    if (colorP != "")
                        url += "&color=" + colorP;
                    if (countryP != "")
                        url += "&c=" + countryP;
                    if (sortP != "")
                        url += "&s=" + sortP;
                    if (minPriceP != "")
                        url += "&mp=" + minPriceP;
                    if (maxPriceP != "")
                        url += "&xp=" + maxPriceP;
                    if (minRankP != "")
                        url += "&mr=" + minRankP;
                    if (maxRankP != "")
                        url += "&xr=" + maxRankP;
                    //return url;
                    Frame frame = Window.Current.Content as Frame;
                    GetWinesPopup.IsOpen = false;
                    frame.Navigate(typeof(ReturnedWines), url);
                }
        }
        private string Get_Color()
        {
            RadioButton radioButton = null;
            radioButton = GetCheckedRadioButton(ColorGrid.Children, "ColorGroup");
            if (radioButton != null)

                return radioButton.Content.ToString();
            else
                return "";
        }
        private string Get_Type()
        {
            RadioButton radioButton = null;
            radioButton = GetCheckedRadioButton(TypeGrid.Children, "TypeGroup");
            if (radioButton != null)

                return radioButton.Content.ToString();
            else
                return "";
        }

        private string Get_Country()
        {
            Grid gr = null;
            if (gr != null)
            {
                gr = (Grid)cmbCountry.SelectedItem;
                return gr.Name;
            }
            else
                return "";
        }

        private string Get_MinPrice()
        {

            return minPrice.Text;
        }

        private string Get_MaxPrice()
        {
            return maxPrice.Text;
        }
        private string Get_Sort()
        {
            RadioButton radioButton = null;
            radioButton = GetCheckedRadioButton(SortGrid.Children, "PriceGroup");
            if (radioButton != null)

                return "price+" + radioButton.Name;
            else
                return "";
        }
        private string Get_MinRank()
        {
            if (sldMin.Value == 0)
                return "1";
            return sldMin.Value.ToString();
        }
        private string Get_MaxRank()
        {
            if (sldMax.Value == 0)
                return "5";
            return sldMax.Value.ToString();
        }
        private RadioButton GetCheckedRadioButton(UIElementCollection children, String groupName)
        {
            return children.OfType<RadioButton>().FirstOrDefault(rb => rb.IsChecked == true && rb.GroupName == groupName);
        }
    }
}
