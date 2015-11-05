using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Data.Xml.Dom;
using PancakeCooker.Common;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace PancakeCooker.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ShoppingCart : Page
    {
        private Dictionary<CheckBox, GoodsInfo> selectedGoods = new Dictionary<CheckBox, GoodsInfo>();
        private const string CHECKBOX = "Windows.UI.Xaml.Controls.CheckBox";
        private const string TEXTBLOCK = "Windows.UI.Xaml.Controls.TextBlock";
        private const string IMAGE = "Windows.UI.Xaml.Controls.Image";
        private const string BUTTON = "Windows.UI.Xaml.Controls.Button";
        private List<GoodsInfo> goodsList = new List<GoodsInfo>();

        public ShoppingCart()
        {
            this.InitializeComponent();
            GoodsCVS.Source = new ObservableCollection<GoodsInfo>();
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.Equals("Windows.Mobile"))
            {
                this.Margin = new Thickness(0, -8, 0, 0);
            }
            string platform = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;//Windows.Mobile Windows.Desktop
            if(platform.Equals("Windows.Mobile"))
            {
                priceText.Margin = new Thickness(20, 0, 0, 0);
                price.Margin = new Thickness(0, 0, 20, 0);
                priceSymbol.Margin = new Thickness(0, 0, 0, 0);
            }
            else
            {
                priceText.Margin = new Thickness(20, 10, 0, 0);
                price.Margin = new Thickness(0, 10, 0, 0);
                priceSymbol.Margin = new Thickness(0, 0, 0, 10);
            }

        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            }
        }
        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {          
            e.Handled = true;
            if (HidePanel.Visibility == Visibility.Visible)
            {
                HidePanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                returnBack_Click(returnBack, null);
            }           
        }

        private void ResizeControl()
        {
            topPanel.Width = buttomPanel.Width = mainPanel.Width = HidePanel.Width = mainGrid.Width = Window.Current.Bounds.Width;
            topPanel.Height = buttomPanel.Height = 40;
            mainPanel.Height = HidePanel.Height = mainGrid.Height = Window.Current.Bounds.Height;
            goods.Width = Window.Current.Bounds.Width;           
            goods.Height = Window.Current.Bounds.Height - 80;           
            goodsGrid.Width = Window.Current.Bounds.Width;
        }

        private void Decrease_Click(object sender, RoutedEventArgs e)
        {          
            Button add = sender as Button;
            if (null == add)
            {
                return;
            }
            UIElement parent = (UIElement)VisualTreeHelper.GetParent(add);
            if (null == parent)
            {
                return;
            }
            int count = VisualTreeHelper.GetChildrenCount(parent);
            double price = 0;
            bool bChecked = false;
            int currentNumber = 0;
            CheckBox check = null;
            GoodsInfo info = new GoodsInfo();
            for (int i = 0; i < count; i++)
            {
                UIElement child = (UIElement)VisualTreeHelper.GetChild(parent, i);
                if (null == child)
                {
                    continue;
                }
                string strType = child.ToString();
                if (strType.Equals(TEXTBLOCK))
                {
                    TextBlock text = (TextBlock)child;
                    if (text.Name.Equals("goodsNumber"))
                    {
                        int number = Convert.ToInt32(text.Text);
                        if( number > 1)
                        {
                            text.Text = Convert.ToString(number - 1);
                            currentNumber = number - 1;
                            info.count = currentNumber;
                        }                      
                    }
                    else if (text.Name.Equals("goodsNumber2"))
                    {
                        int number = Convert.ToInt32(text.Text.Substring(text.Text.IndexOf('x')+1));
                        if (number > 1)
                        {
                            text.Text = "x" + Convert.ToString(number - 1);
                        }
                    }
                    else if (text.Name.Equals("priceValue"))
                    {
                        price = Convert.ToDouble(text.Text);
                    }
                    else if(text.Name.Equals("goodsName"))
                    {
                        info.name = text.Text;
                    }
                    else if (text.Name.Equals("imageName"))
                    {
                        info.imageName = text.Text;
                    }
                }
                else if (strType.Equals(CHECKBOX))
                {
                    check = (CheckBox)child;
                    if (null == check)
                    {
                        continue;
                    }
                    bChecked = (bool)check.IsChecked;
                }
            }
            foreach(var item in goodsList)
            {
                if(item.imageName.Equals(info.imageName) && item.name.Equals(info.name))
                {
                    item.count = info.count;
                    break;
                }
            }
            if (bChecked)
            {
                double oldPrice = Convert.ToDouble(this.price.Text);
                oldPrice -= price;
                this.price.Text = Convert.ToString(oldPrice);
                foreach (var item in selectedGoods)
                {
                    if (item.Key.Equals(check))
                    {
                        item.Value.count = currentNumber;
                    }
                }
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Button add = sender as Button;            
            if (null == add)
            {
                return;
            }
            UIElement parent = (UIElement)VisualTreeHelper.GetParent(add);
            if (null == parent)
            {
                return;
            }
            int count = VisualTreeHelper.GetChildrenCount(parent);
            double price = 0;   
            bool bChecked = false;
            int currentNumber = 0;
            CheckBox check = null;
            GoodsInfo info = new GoodsInfo();
            for (int i = 0; i < count; i++)
            {
                UIElement child = (UIElement)VisualTreeHelper.GetChild(parent, i);
                if(null == child)
                {
                    continue;
                }
                string strType = child.ToString();            
                if (strType.Equals(TEXTBLOCK))
                {
                    TextBlock text = (TextBlock)child;
                    if (text.Name.Equals("goodsNumber"))
                    {
                        int number = Convert.ToInt32(text.Text);
                        text.Text = Convert.ToString(number + 1);
                        currentNumber = number + 1;
                        info.count = currentNumber;
                    } 
                    else if(text.Name.Equals("goodsNumber2"))
                    {
                        int number = Convert.ToInt32(text.Text.Substring(text.Text.IndexOf('x') + 1));                       
                        text.Text = "x" + Convert.ToString(number + 1);
                    }
                    else if(text.Name.Equals("priceValue"))
                    {
                        price = Convert.ToDouble(text.Text.Substring(text.Text.IndexOf('￥') + 1));
                    }
                    else if (text.Name.Equals("goodsName"))
                    {
                        info.name = text.Text;
                    }
                    else if (text.Name.Equals("imageName"))
                    {
                        info.imageName = text.Text;
                    }
                }
                else if(strType.Equals(CHECKBOX))
                {
                    check = (CheckBox)child;
                    if(null == check)
                    {
                        continue;
                    }
                    bChecked = (bool)check.IsChecked;
                }
            }
            foreach (var item in goodsList)
            {
                if (item.imageName.Equals(info.imageName) && item.name.Equals(info.name))
                {
                    item.count = info.count;
                    break;
                }
            }
            if (bChecked)
            {               
                double oldPrice = Convert.ToDouble(this.price.Text);
                oldPrice += price;
                this.price.Text = Convert.ToString(oldPrice);
                foreach(var item in selectedGoods)
                {
                    if(item.Key.Equals(check))
                    {
                        item.Value.count = currentNumber;
                    }
                }
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox check = sender as CheckBox;
            if (null == check)
            {
                return;
            }
            selectedGoods.Remove(check);
            GoodsInfo info = new GoodsInfo();         
            UIElement parent = (UIElement)VisualTreeHelper.GetParent(check);
            if (null == parent)
            {
                return;
            }
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                UIElement child = (UIElement)VisualTreeHelper.GetChild(parent, i);
                string strType = child.ToString();
                if (strType.Equals(CHECKBOX))
                {
                    continue;
                }
                else if (strType.Equals(TEXTBLOCK))
                {
                    TextBlock text = (TextBlock)child;
                    if (text.Name.Equals("priceValue"))
                    {
                        info.price = text.Text;
                    }
                    else if (text.Name.Equals("goodsName"))
                    {
                        foreach (var item in goodsList)
                        {
                            if (item.name.Equals(text.Text))
                            {
                                info.imgUri = item.imgUri;
                                break;
                            }
                        }
                        info.name = text.Text;
                    }
                    else if (text.Name.Equals("goodsNumber"))
                    {
                        info.count = Convert.ToInt32(text.Text);
                    }
                }
            }
            int number = info.count;
            double price = Convert.ToDouble(info.price.Substring(info.price.IndexOf('￥') + 1));
            double oldPrice = Convert.ToDouble(this.price.Text);
            oldPrice -= (price * number);
            this.price.Text = Convert.ToString(oldPrice);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox check = sender as CheckBox;
            if (null == check)
            {
                return;
            }
            UIElement parent = (UIElement)VisualTreeHelper.GetParent(check);
            if (null == parent)
            {
                return;
            }
            int count = VisualTreeHelper.GetChildrenCount(parent);
            GoodsInfo info = new GoodsInfo();
            //info.check = check;
            for(int i=0; i<count; i++)
            {
                UIElement child = (UIElement)VisualTreeHelper.GetChild(parent, i);
                string strType = child.ToString();
                if (strType.Equals(CHECKBOX))
                {
                    continue;
                }
                else if (strType.Equals(TEXTBLOCK))
                {
                    TextBlock text = (TextBlock)child;
                    if (text.Name.Equals("priceValue"))
                    {
                        info.price = text.Text;
                    }
                    else if(text.Name.Equals("goodsName"))
                    {                       
                        info.name = text.Text;
                    }
                    else if(text.Name.Equals("goodsNumber"))
                    {
                        info.count = Convert.ToInt32(text.Text);
                    }
                    else if(text.Name.Equals("imageName"))
                    {
                        info.imageName = text.Text;
                    }
                }               
            }
            foreach (var item in goodsList)
            {
                if (item.name.Equals(info.name) && item.imageName.Equals(info.imageName))
                {
                    info.imgUri = item.imgUri;
                    info.img = item.img;
                    break;
                }
            }
            selectedGoods.Add(check,info);
            int number = info.count;
            double price = Convert.ToDouble(info.price.Substring(info.price.IndexOf('￥') + 1));
            double oldPrice = Convert.ToDouble(this.price.Text);
            oldPrice += (price * number);
            this.price.Text = Convert.ToString(oldPrice);       
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {            
            if(selectedGoods.Count == 0 )
            {
                return;
            }
            confirmPayPanel.Visibility = Visibility.Collapsed;
            confirmDeletePanel.Visibility = Visibility.Visible;
            payPanel.Visibility = Visibility.Collapsed;
            HidePanel.Visibility = Visibility.Visible;
            return;        
        }

        private void selectBtn_Click(object sender, RoutedEventArgs e)
        {          
            if(selectedGoods.Count == 0)
            {
                return;
            }
            confirmDeletePanel.Visibility = Visibility.Collapsed;
            confirmPayPanel.Visibility = Visibility.Collapsed;
            payPanel.Visibility = Visibility.Visible;
            HidePanel.Visibility = Visibility.Visible;          
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            goodsList.Clear();
            try
            {
                List<GoodsInfo> list = e.Parameter as List<GoodsInfo>;
                if (null != list)
                {
                    goodsList = list;
                }
                ObservableCollection<GoodsInfo> infos = (ObservableCollection<GoodsInfo>)GoodsCVS.Source;
                Windows.Storage.StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile shoppingCartStorageFile = await storageFolder.GetFileAsync("ShoppingCart.xml");                
                XmlDocument doc = await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(shoppingCartStorageFile);
                IXmlNode root = null;
                foreach (var item in doc.ChildNodes)
                {
                    if (item.NodeName.Equals("goods"))
                    {
                        root = item;
                        break;
                    }
                }
                if (null != root)
                {
                    foreach (var goods in root.ChildNodes)
                    {
                        if (goods.NodeName.Equals("item"))
                        {
                            string imgUrl = goods.SelectSingleNode("img").InnerText;
                            string singlePrice = goods.SelectSingleNode("price").InnerText;
                            string name = goods.SelectSingleNode("productName").InnerText;
                            string count = goods.SelectSingleNode("count").InnerText;
                            string imagename = goods.SelectSingleNode("imageName").InnerText;
                            bool exist = false;
                            foreach (var item in goodsList)
                            {
                                if (item.name.Equals(name) && item.imgUri.Equals(imgUrl))
                                {
                                    item.count = item.count + Convert.ToInt32(count);
                                    exist = true;
                                    break;
                                }
                            }
                            if (!exist)
                            {
                                GoodsInfo info = new GoodsInfo();
                                StorageFile file = await StorageFile.GetFileFromPathAsync(imgUrl);
                                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                                {
                                    BitmapImage image = new BitmapImage();
                                    image.SetSource(fileStream);
                                    info.img = image;
                                }
                                info.name = name;
                                info.price = singlePrice;
                                info.count = Convert.ToInt32(count);
                                info.imageName = imagename;
                                info.imgUri = imgUrl;
                                goodsList.Add(info);
                            }

                        }
                    }
                }
                foreach (var item in goodsList)
                {
                    infos.Add(item);
                }
            }
            catch(Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
            }            
        }

        private  void payBtn_Click(object sender, RoutedEventArgs e)
        {
            payPanel.Visibility = Visibility.Collapsed;
            confirmDeletePanel.Visibility = Visibility.Collapsed;
            confirmPayPanel.Visibility = Visibility.Visible;
            double price = 0;
            foreach(var item in selectedGoods)
            {
                price += (Convert.ToDouble(item.Value.price) * item.Value.count);
            }
            totalPrice.Text = "￥" + Convert.ToString(price);
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            HidePanel.Visibility = Visibility.Collapsed;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeControl();
        }

        private async void returnBack_Click(object sender, RoutedEventArgs e)
        {
            bool success = await WriteShorppingcart();            
            MainPage._CurrentHandle.Navigate(typeof(Custom));
        }

        private void HidePanel_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if(payPanel.Visibility == Visibility.Collapsed)
            {
                return;
            }
            e.Handled = true;
            Windows.UI.Input.PointerPoint ptr = e.GetCurrentPoint(sender as UIElement);
            Point pt = ptr.Position;
            if (pt.X >= (HidePanel.ActualWidth - payPanel.ActualWidth) / 2 && pt.X <= (HidePanel.ActualWidth - payPanel.ActualWidth) / 2 + payPanel.ActualWidth && pt.Y >= (HidePanel.ActualHeight - payPanel.ActualHeight) / 2 && pt.Y <= (HidePanel.ActualHeight - payPanel.ActualHeight) / 2 + payPanel.ActualHeight)
            {
                return;
            }
            else
            {
                HidePanel.Visibility = Visibility.Collapsed;
            }
        }

        private async Task<bool> WriteShorppingcart()
        {
            Windows.Storage.StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile shoppingCartStorageFile = await storageFolder.GetFileAsync("ShoppingCart.xml");
            XmlDocument doc = await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(shoppingCartStorageFile);
            IXmlNode root = null;
            foreach (var item in doc.ChildNodes)
            {
                if (item.NodeName.Equals("goods"))
                {
                    root = item;
                    break;
                }
            }
            if (null != root)
            {
                root.InnerText = "";
                foreach (var item in goodsList)
                {
                    XmlElement eleInner = doc.CreateElement("item");
                    XmlElement eleInnerChild = doc.CreateElement("img");
                    eleInnerChild.InnerText = item.imgUri;
                    eleInner.AppendChild(eleInnerChild);
                    eleInnerChild = doc.CreateElement("count");
                    eleInnerChild.InnerText = Convert.ToString(item.count);
                    eleInner.AppendChild(eleInnerChild);
                    eleInnerChild = doc.CreateElement("price");
                    eleInnerChild.InnerText = item.price;
                    eleInner.AppendChild(eleInnerChild);
                    eleInnerChild = doc.CreateElement("productName");
                    eleInnerChild.InnerText = item.name;
                    eleInner.AppendChild(eleInnerChild);
                    eleInnerChild = doc.CreateElement("imageName");
                    eleInnerChild.InnerText = item.imageName;
                    eleInner.AppendChild(eleInnerChild);
                    root.AppendChild(eleInner);
                }
            }
            await doc.SaveToFileAsync(shoppingCartStorageFile);
            MainPage._CurrentHandle.ShopCartNotify();
            return true;
        }


        private async void ConfirmDelete_Click(object sender, RoutedEventArgs e)
        {
            List<CheckBox> checkboxs = new List<CheckBox>();
            HidePanel.Visibility = Visibility.Collapsed;
            foreach(var item in selectedGoods)
            {               
                GoodsInfo info = item.Value;
                checkboxs.Add(item.Key);
                foreach(var goods in goodsList)
                {
                    if(goods.imgUri.Equals(info.imgUri) && goods.name.Equals(info.name))
                    {
                        goodsList.Remove(goods);
                        break;
                    }
                }
            }
            for(int i=0; i < checkboxs.Count; i++)
            {
                checkboxs[i].IsChecked = false;
            }
            selectedGoods.Clear();
            await WriteShorppingcart();
            ObservableCollection<GoodsInfo> infos = (ObservableCollection<GoodsInfo>)GoodsCVS.Source;
            infos.Clear();
            foreach(var item in goodsList)
            {
                infos.Add(item);
            }

        }

        private void CancelDelete_Click(object sender, RoutedEventArgs e)
        {
            HidePanel.Visibility = Visibility.Collapsed;
        }

        private async void CancelPay_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGoods.Count > 0)
            {
                OrderInfo orderList = new OrderInfo();
                string id = DateTime.Now.ToString("yyyyMMddHHmmss");
                string name = customerNameText.Text;
                string mobile = mobileText.Text;
                string totalPrice;
                string address = addressText.Text;
                double price = 0;
                foreach (var item in selectedGoods)
                {
                    double singlePrice = Convert.ToDouble(item.Value.price.Substring(item.Value.price.IndexOf('￥') + 1));
                    price += (singlePrice * item.Value.count);
                }
                totalPrice = Convert.ToString(price);
                Windows.Storage.StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile storageFile = await storageFolder.GetFileAsync("Goods.xml");
                XmlDocument doc = await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(storageFile);
                IXmlNode root = null;
                foreach (var item in doc.ChildNodes)
                {
                    if (item.NodeName.Equals("orders"))
                    {
                        root = item;
                        break;
                    }
                }
                if (null == root)
                {
                    return;
                }
                try
                {
                    XmlElement orderEle = doc.CreateElement("order");
                    XmlElement ele = doc.CreateElement("id");
                    ele.InnerText = id;
                    orderEle.AppendChild(ele);
                    ele = doc.CreateElement("name");
                    ele.InnerText = name;
                    orderEle.AppendChild(ele);
                    ele = doc.CreateElement("mobile");
                    ele.InnerText = mobile;
                    orderEle.AppendChild(ele);
                    ele = doc.CreateElement("totalPrice");
                    ele.InnerText = totalPrice;
                    orderEle.AppendChild(ele);
                    ele = doc.CreateElement("address");
                    ele.InnerText = address;
                    orderEle.AppendChild(ele);
                    ele = doc.CreateElement("payed");
                    ele.InnerText = "false";
                    orderEle.AppendChild(ele);
                    ele = doc.CreateElement("completed");
                    ele.InnerText = "false";
                    orderEle.AppendChild(ele);
                    ele = doc.CreateElement("goods");
                    foreach (var item in selectedGoods)
                    {
                        XmlElement eleInner = doc.CreateElement("item");
                        XmlElement eleInnerChild = doc.CreateElement("img");
                        eleInnerChild.InnerText = item.Value.imgUri;
                        eleInner.AppendChild(eleInnerChild);
                        eleInnerChild = doc.CreateElement("count");
                        eleInnerChild.InnerText = Convert.ToString(item.Value.count);
                        eleInner.AppendChild(eleInnerChild);
                        eleInnerChild = doc.CreateElement("price");
                        eleInnerChild.InnerText = item.Value.price;
                        eleInner.AppendChild(eleInnerChild);
                        eleInnerChild = doc.CreateElement("productName");
                        eleInnerChild.InnerText = item.Value.name;
                        eleInner.AppendChild(eleInnerChild);
                        eleInnerChild = doc.CreateElement("imageName");
                        eleInnerChild.InnerText = item.Value.imageName;
                        eleInner.AppendChild(eleInnerChild);
                        ele.AppendChild(eleInner);
                    }
                    orderEle.AppendChild(ele);
                    root.AppendChild(orderEle);
                    await doc.SaveToFileAsync(storageFile);
                    MainPage._CurrentHandle.Navigate(typeof(Store));
                }
                catch (Exception ex)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
                }
            }
            foreach (var item in selectedGoods)
            {
                GoodsInfo info = item.Value;
                foreach (var goods in goodsList)
                {
                    if (goods.imgUri.Equals(info.imgUri) && goods.name.Equals(info.name))
                    {
                        goodsList.Remove(goods);
                        break;
                    }
                }
            }
            bool success = await WriteShorppingcart();
            MainPage._CurrentHandle.Navigate(typeof(Custom));
        }

        private async void ConfirmPay_Click(object sender, RoutedEventArgs e)
        {
            OrderInfo orderList = new OrderInfo();
            string id = DateTime.Now.ToString("yyyyMMddHHmmss");
            string name = customerNameText.Text;
            string mobile = mobileText.Text;
            string totalPrice;
            string address = addressText.Text;
            double price = 0;
            foreach (var item in selectedGoods)
            {
                double singlePrice = Convert.ToDouble(item.Value.price.Substring(item.Value.price.IndexOf('￥') + 1));
                price += (singlePrice * item.Value.count);
            }
            totalPrice = Convert.ToString(price);
            Windows.Storage.StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile storageFile = await storageFolder.GetFileAsync("Goods.xml");
            XmlDocument doc = await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(storageFile);
            IXmlNode root = null;
            foreach (var item in doc.ChildNodes)
            {
                if (item.NodeName.Equals("orders"))
                {
                    root = item;
                    break;
                }
            }
            if (null == root)
            {
                return;
            }
            try
            {
                XmlElement orderEle = doc.CreateElement("order");
                XmlElement ele = doc.CreateElement("id");
                ele.InnerText = id;
                orderEle.AppendChild(ele);
                ele = doc.CreateElement("name");
                ele.InnerText = name;
                orderEle.AppendChild(ele);
                ele = doc.CreateElement("mobile");
                ele.InnerText = mobile;
                orderEle.AppendChild(ele);
                ele = doc.CreateElement("totalPrice");
                ele.InnerText = totalPrice;
                orderEle.AppendChild(ele);
                ele = doc.CreateElement("address");
                ele.InnerText = address;
                orderEle.AppendChild(ele);
                ele = doc.CreateElement("payed");
                ele.InnerText = "true";
                orderEle.AppendChild(ele);
                ele = doc.CreateElement("completed");
                ele.InnerText = "false";
                orderEle.AppendChild(ele);
                ele = doc.CreateElement("goods");
                foreach (var item in selectedGoods)
                {
                    XmlElement eleInner = doc.CreateElement("item");
                    XmlElement eleInnerChild = doc.CreateElement("img");
                    eleInnerChild.InnerText = item.Value.imgUri;
                    eleInner.AppendChild(eleInnerChild);
                    eleInnerChild = doc.CreateElement("count");
                    eleInnerChild.InnerText = Convert.ToString(item.Value.count);
                    eleInner.AppendChild(eleInnerChild);
                    eleInnerChild = doc.CreateElement("price");
                    eleInnerChild.InnerText = item.Value.price;
                    eleInner.AppendChild(eleInnerChild);
                    eleInnerChild = doc.CreateElement("productName");
                    eleInnerChild.InnerText = item.Value.name;
                    eleInner.AppendChild(eleInnerChild);
                    eleInnerChild = doc.CreateElement("imageName");
                    eleInnerChild.InnerText = item.Value.imageName;
                    eleInner.AppendChild(eleInnerChild);
                    ele.AppendChild(eleInner);
                }
                orderEle.AppendChild(ele);
                root.AppendChild(orderEle);
                await doc.SaveToFileAsync(storageFile);
                foreach (var item in selectedGoods)
                {
                    GoodsInfo info = item.Value;
                    foreach (var goods in goodsList)
                    {
                        if (goods.imgUri.Equals(info.imgUri) && goods.name.Equals(info.name))
                        {
                            goodsList.Remove(goods);
                            break;
                        }
                    }
                }
                bool success = await WriteShorppingcart();
                MainPage._CurrentHandle.Navigate(typeof(Orders));
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
            }
        }
    }
}
