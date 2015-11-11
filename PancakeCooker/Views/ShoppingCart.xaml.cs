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
using Windows.UI.ViewManagement;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Data.Xml.Dom;
using PancakeCooker.Common;
using PancakeCooker.Common.ItemTemplatePanel;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace PancakeCooker.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ShoppingCart : Page
    {
        private Dictionary<ShopCartPanel, GoodsInfo> selectedGoods = new Dictionary<ShopCartPanel, GoodsInfo>();
        private List<GoodsInfo> goodsList = new List<GoodsInfo>();
        private Dictionary<int, ShopCartPanel> goodsPanels = new Dictionary<int, ShopCartPanel>();
        private Style checkboxStyle = null;
        private double MinPictureWith { get; set; }
        private double MaxPictureWith { get; set; }
        private double _picWidth { get; set; }
        private double PicWidth
        {
            get
            {
                return _picWidth;
            }
            set
            {
                _picWidth = value;
                goodsPanelWidth = _picWidth + 2 * PictureMargin;
            }
        }
        private double preWidth { get; set; }
        private double refreshStep { get; set; }
        private int pictureColumn { get; set; }
        private double _pictureMargin;
        private double PictureMargin
        {
            get
            {
                return _pictureMargin;
            }
            set
            {
                _pictureMargin = value;
                goodsPanelWidth = _picWidth + 2 * _pictureMargin;
            }
        }
        private double goodsPanelWidth { get; set; }

        public ShoppingCart()
        {
            this.InitializeComponent();
            InitParam();            
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
                TranslateTransform trans = new TranslateTransform();
                trans.X = 0;
                trans.Y = 7;
                priceSymbol.SetValue(RenderTransformProperty,trans);
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

        private void InitParam()
        {
            ResourceDictionary dic = Application.Current.Resources;
            IList<ResourceDictionary> dics = dic.MergedDictionaries;
            ResourceDictionary resource = null;
            foreach (var item in dics)
            {
                if (item.Source.AbsolutePath.Contains("/Files/Styles/Styles.xaml"))
                {
                    resource = item;
                    break;
                }

            }
            object style = null;
            if (null != resource)
            {
                resource.TryGetValue("CheckBoxStyle", out style);
                if (null != style)
                {
                    checkboxStyle = style as Style;
                }
            }                   
            refreshStep = 10;
            PictureMargin = 20;
            preWidth = Window.Current.Bounds.Width - 20; ;
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.Equals("Windows.Mobile"))
            {
                pictureColumn = 1;
                PicWidth = MinPictureWith = MaxPictureWith = preWidth / pictureColumn - 2*PictureMargin;
            }
            else
            {
                PicWidth = MinPictureWith = 200;
                MaxPictureWith = 250;
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
            topPanel.Width = buttomPanel.Width = mainPanel.Width = HidePanel.Width  = Window.Current.Bounds.Width;
            topPanel.Height = buttomPanel.Height = 40;
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.Equals("Windows.Mobile"))
            {
                StatusBar bar = StatusBar.GetForCurrentView();
                double height = bar.OccludedRect.Height;              
                mainPanel.Height = HidePanel.Height  = Window.Current.Bounds.Height - height;
                goodsScrollView.Height = mainPanel.Height - topPanel.Height - buttomPanel.Height;
            }
            else
            {
                mainPanel.Height = HidePanel.Height = Window.Current.Bounds.Height;
                goodsScrollView.Height = Window.Current.Bounds.Height - topPanel.Height - buttomPanel.Height;
            }          
            goods.Width = goodsScrollView.Width = Window.Current.Bounds.Width - goodsScrollView.Margin.Left - goodsScrollView.Margin.Right;          
            double currentWidth = goodsScrollView.Width;
            if(Math.Abs(currentWidth - preWidth) > refreshStep)
            {
                preWidth = currentWidth;
                ResetPicturePanelPositon();
            } 
        }

        private void ResetPicturePanelPositon()
        {
            pictureColumn = Convert.ToInt32(Math.Floor(preWidth / (MinPictureWith + 2 * PictureMargin)));
            PicWidth = preWidth / pictureColumn - 2 * PictureMargin;
            PicWidth = PicWidth > MaxPictureWith ? MaxPictureWith : PicWidth;
            foreach (var item in goodsPanels)
            {
                item.Value.ClearValue(RelativePanel.BelowProperty);
                item.Value.ClearValue(RelativePanel.AlignLeftWithPanelProperty);
                item.Value.ClearValue(RelativePanel.RightOfProperty);
                item.Value.ClearValue(RelativePanel.AlignTopWithPanelProperty);
                item.Value.PictureWith = PicWidth;
                //set left
                if (item.Key % pictureColumn == 0)
                {
                    item.Value.SetValue(RelativePanel.AlignLeftWithPanelProperty, true);
                }
                else
                {
                    item.Value.SetValue(RelativePanel.RightOfProperty, goodsPanels[item.Key - 1].Name);
                }
                //set top
                if (item.Key / pictureColumn == 0)
                {
                    item.Value.SetValue(RelativePanel.AlignTopWithPanelProperty, true);
                }
                else
                {
                    item.Value.SetValue(RelativePanel.BelowProperty, goodsPanels[item.Key - pictureColumn].Name);
                }
            }
        }

        private void Decrease_Click(object sender, RoutedEventArgs e)
        {
            Button add = sender as Button;
            if (null == add)
            {
                return;
            }
            var panel = VisualTreeHelper.GetParent(add);
            if (null == panel)
            {
                return;
            }
            var grid = VisualTreeHelper.GetParent(panel);
            if (null == grid)
            {
                return;
            }
            var shopcartPanel = VisualTreeHelper.GetParent(grid);
            if (null != shopcartPanel && shopcartPanel.ToString().Equals(typeof(ShopCartPanel).ToString()))
            {
                ShopCartPanel itemPanel = shopcartPanel as ShopCartPanel;
                double price = itemPanel.DecreaseClick();
                bool bCheck = (bool)itemPanel.Check.IsChecked;
                if (bCheck)
                {
                    double oldPrice = Convert.ToDouble(this.price.Text);
                    oldPrice -= price;
                    this.price.Text = Convert.ToString(oldPrice);
                    foreach (var item in selectedGoods)
                    {
                        if (item.Key.Equals(itemPanel.Check))
                        {
                            item.Value.count = itemPanel.GoodsCount;
                        }
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
            var panel = VisualTreeHelper.GetParent(add);
            if (null == panel)
            {
                return;
            }
            var grid = VisualTreeHelper.GetParent(panel);
            if (null == grid)
            {
                return;
            }
            var shopcartPanel = VisualTreeHelper.GetParent(grid);
            if (null != shopcartPanel && shopcartPanel.ToString().Equals(typeof(ShopCartPanel).ToString()))
            {
                ShopCartPanel itemPanel = shopcartPanel as ShopCartPanel;
                double price = itemPanel.AddClick();
                bool bCheck = (bool)itemPanel.Check.IsChecked;
                if(bCheck)
                {
                    double oldPrice = Convert.ToDouble(this.price.Text);
                    oldPrice += price;
                    this.price.Text = Convert.ToString(oldPrice);
                    foreach (var item in selectedGoods)
                    {
                        if (item.Key.Equals(itemPanel.Check))
                        {
                            item.Value.count = itemPanel.GoodsCount;
                        }
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
            var panel = VisualTreeHelper.GetParent(check);
            if (null == panel)
            {
                return;
            }
            var grid = VisualTreeHelper.GetParent(panel);
            if (null == grid)
            {
                return;
            }
            var shopcartPanel = VisualTreeHelper.GetParent(grid);
            if (null != shopcartPanel && shopcartPanel.ToString().Equals(typeof(ShopCartPanel).ToString()))
            {
                ShopCartPanel itemPanel = shopcartPanel as ShopCartPanel;
                double totalPrice = itemPanel.GetItemTotalPrice();
                double oldPrice = Convert.ToDouble(this.price.Text);
                oldPrice -= totalPrice;
                this.price.Text = Convert.ToString(oldPrice);
                selectedGoods.Remove(itemPanel);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox check = sender as CheckBox;
            if (null == check)
            {
                return;
            }
            var panel = VisualTreeHelper.GetParent(check);
            if (null == panel)
            {
                return;
            }
            var grid = VisualTreeHelper.GetParent(panel);
            if(null == grid)
            {
                return;
            }
            var shopcartPanel = VisualTreeHelper.GetParent(grid);
            if(null != shopcartPanel && shopcartPanel.ToString().Equals(typeof(ShopCartPanel).ToString()))
            {
                ShopCartPanel itemPanel = shopcartPanel as ShopCartPanel;
                double totalPrice = itemPanel.GetItemTotalPrice();
                double oldPrice = Convert.ToDouble(this.price.Text);
                oldPrice += totalPrice;
                this.price.Text = Convert.ToString(oldPrice);
                GoodsInfo info = new GoodsInfo();
                info.name = itemPanel.GoodsName;
                info.price = itemPanel.Price;
                info.count = itemPanel.GoodsCount;
                info.imageName = itemPanel.PictureName;
                info.imgUri = itemPanel.PictureURL;
                selectedGoods.Add(itemPanel, info);
            }             
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
                int index = 0;
                foreach (var item in goodsList)
                {
                    ShopCartPanel panel = new ShopCartPanel();
                    panel.Picture = item.brush;
                    panel.GoodsName = item.name;
                    panel.PictureName = item.imageName;
                    panel.GoodsCount = item.count;
                    panel.Price = item.price;
                    panel.Name = item.imgUri + item.name;
                    panel.PictureURL = item.imgUri;
                    CheckBox check = new CheckBox();
                    if(null != checkboxStyle)
                    {
                        check.SetValue(StyleProperty,checkboxStyle);
                    }
                    check.Checked += CheckBox_Checked;
                    check.Unchecked += CheckBox_Unchecked;
                    panel.Check = check;
                    Button btnAdd = new Button();
                    btnAdd.Click += Add_Click;
                    Button btnDecrease = new Button();
                    btnDecrease.Click += Decrease_Click;
                    panel.ButtonAdd = btnAdd;
                    panel.ButtonDecrease = btnDecrease;
                    panel.PictureWith = PicWidth;
                    if(index > 0 && goodsPanels.ContainsKey(index - 1))
                    {
                        panel.SetValue(RelativePanel.RightOfProperty, goodsPanels[index - 1].Name);                        
                    }
                    else
                    {
                        panel.SetValue(RelativePanel.AlignLeftWithPanelProperty, true);
                    }
                    panel.SetValue(RelativePanel.AlignTopWithPanelProperty, true);
                    goods.Children.Add(panel);
                    goodsPanels.Add(index++,panel);
                }
                ResetPicturePanelPositon();
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
            HidePanel.Visibility = Visibility.Collapsed;
            foreach(var item in selectedGoods)
            {               
                GoodsInfo info = item.Value;
                foreach(var goods in goodsList)
                {
                    if(goods.imgUri.Equals(info.imgUri) && goods.name.Equals(info.name))
                    {
                        goodsList.Remove(goods);
                        break;
                    }
                }
                foreach(var panel in goodsPanels)
                {
                    if(panel.Value.Equals(item.Key))
                    {
                        goodsPanels.Remove(panel.Key);
                        break;
                    }
                }
            }           
            selectedGoods.Clear();
            await WriteShorppingcart();
            List<ShopCartPanel> panelTemp = new List<ShopCartPanel>();
            int index = 0;
            foreach(var item in goodsPanels)
            {
                panelTemp.Add(item.Value);
            }
            goodsPanels.Clear();
            foreach(var item in panelTemp)
            {
                goodsPanels.Add(index++,item);
            }
            ResetPicturePanelPositon();
                       
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
