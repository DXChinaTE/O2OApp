using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml.Media.Imaging;
using PancakeCooker.Views;
using PancakeCooker.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PancakeCooker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage _CurrentHandle = null;
        public double _TopPenelHeight = 0;
        private List<RelativePanel> topMenuPanels = new List<RelativePanel>();
        private Dictionary<Type, Visibility> needShowTopPanel = new Dictionary<Type, Visibility>();
        private List<GoodsInfo> goodsList = new List<GoodsInfo>();
        public MainPage()
        {
            this.InitializeComponent();
            _CurrentHandle = this;
            _TopPenelHeight = TopPanel.Height;
            mainFrame.Navigate(typeof(Custom));
            topMenuPanels.Add(galleryPanel1);
            topMenuPanels.Add(myPanel1);
            topMenuPanels.Add(customPanel1);
            topMenuPanels.Add(latestNewsPenel1);
            topMenuPanels.Add(myPanel);
            topMenuPanels.Add(latestNewsPanel);
            topMenuPanels.Add(customPanel);
            topMenuPanels.Add(galleryPanel);
            needShowTopPanel.Add(typeof(Custom),Visibility.Visible);
            needShowTopPanel.Add(typeof(Ink),Visibility.Visible);
            needShowTopPanel.Add(typeof(Orders),Visibility.Visible);
            needShowTopPanel.Add(typeof(Store),Visibility.Collapsed);
            needShowTopPanel.Add(typeof(PicStroe), Visibility.Collapsed);
            needShowTopPanel.Add(typeof(LatestNews),Visibility.Visible);
            needShowTopPanel.Add(typeof(ShoppingCart),Visibility.Collapsed);
            //if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            //{
            //    Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            //}
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if(menuPanel.Visibility == Visibility.Visible)
            {
                menuPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                menuPanel.Visibility = Visibility.Visible;
            }
        }

        private void gallery_Click(object sender, RoutedEventArgs e)
        {
            menuPanel.Visibility = Visibility.Collapsed;
            mainFrame.Navigate(typeof(Custom));            
            UIElement panel = (UIElement)VisualTreeHelper.GetParent(gallery);
            UIElement panel1 = (UIElement)VisualTreeHelper.GetParent(gallery1);
            if(null==panel || panel1 == null)
            {
                return;
            }
            foreach(var item in topMenuPanels)
            {
                if(item.Equals(panel) || item.Equals(panel1))
                {
                    item.Opacity = 1;
                }
                else
                {
                    item.Opacity = 0.6;
                }
            }
        }

        private void latestNews_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(typeof(LatestNews));
            menuPanel.Visibility = Visibility.Collapsed;
            UIElement panel = (UIElement)VisualTreeHelper.GetParent(latestNews);
            UIElement panel1 = (UIElement)VisualTreeHelper.GetParent(latestNews1);
            if (null == panel || panel1 == null)
            {
                return;
            }
            foreach (var item in topMenuPanels)
            {
                if (item.Equals(panel) || item.Equals(panel1))
                {
                    item.Opacity = 1;
                }
                else
                {
                    item.Opacity = 0.6;
                }
            }
        }

        private void custom_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(typeof(Ink));
            menuPanel.Visibility = Visibility.Collapsed;
            UIElement panel = (UIElement)VisualTreeHelper.GetParent(custom);
            UIElement panel1 = (UIElement)VisualTreeHelper.GetParent(custom1);
            if (null == panel || panel1 == null)
            {
                return;
            }
            foreach (var item in topMenuPanels)
            {
                if (item.Equals(panel) || item.Equals(panel1))
                {
                    item.Opacity = 1;
                }
                else
                {
                    item.Opacity = 0.6;
                }
            }
        }

        private void my_Click(object sender, RoutedEventArgs e)
        {
            menuPanel.Visibility = Visibility.Collapsed;
            mainFrame.Navigate(typeof(Orders));
            UIElement panel = (UIElement)VisualTreeHelper.GetParent(my);
            UIElement panel1 = (UIElement)VisualTreeHelper.GetParent(my1);
            if (null == panel || panel1 == null)
            {
                return;
            }
            foreach (var item in topMenuPanels)
            {
                if (item.Equals(panel) || item.Equals(panel1))
                {
                    item.Opacity = 1;
                }
                else
                {
                    item.Opacity = 0.6;
                }
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeControl();
        }

        private void ResizeControl()
        {
            menuPanel.Height = Window.Current.Bounds.Height - TopPanel.Height;
            TopPanel.Width = Window.Current.Bounds.Width;
            mainPanel.Width = mainFrame.Width = Window.Current.Bounds.Width;
            if (TopPanel.Visibility == Visibility.Visible)
            {
                mainPanel.Height = mainFrame.Height = Window.Current.Bounds.Height - TopPanel.Height;
            }
            else
            {
                mainPanel.Height = mainFrame.Height = Window.Current.Bounds.Height;
            }            
        }

        public void GoBack( bool showTopPanel = false)
        {
            if(mainFrame.BackStack.Count > 0)
            {
                mainFrame.GoBack();
                if(showTopPanel)
                {
                    TopPanel.Visibility = Visibility.Visible;
                    ResizeControl();
                }
            }            
        }
        public  void Navigate(Type t, Object param = null)
        {
            if(t != null)
            {             
                if (typeof(Ink) == t && null != param)
                {                  
                    string path = param as string; 
                    if(needShowTopPanel.ContainsKey(t))
                    {
                        TopPanel.Visibility = needShowTopPanel[t];
                    }                
                    ResizeControl();
                    if(!String.IsNullOrEmpty(path))
                    {
                        mainFrame.Navigate(t, path);
                    }                  
                }
                else if(typeof(Ink) == t && null == param)
                {
                    if (needShowTopPanel.ContainsKey(t))
                    {
                        TopPanel.Visibility = needShowTopPanel[t];
                    }
                    mainFrame.Navigate(t);
                }
                else if(typeof(ShoppingCart) == t && null != param)
                {
                    if (needShowTopPanel.ContainsKey(t))
                    {
                        TopPanel.Visibility = needShowTopPanel[t];
                    }
                    ResizeControl();
                    mainFrame.Navigate(t,param);                   
                }
                else if(typeof(Orders) == t)
                {
                    if (needShowTopPanel.ContainsKey(t))
                    {
                        TopPanel.Visibility = needShowTopPanel[t];
                    }
                    ResizeControl();
                    mainFrame.Navigate(t);
                }
                else if(typeof(Custom) == t)
                {
                    if (needShowTopPanel.ContainsKey(t))
                    {
                        TopPanel.Visibility = needShowTopPanel[t];
                    }
                    ResizeControl();
                    mainFrame.Navigate(t);
                }
                else
                {
                    if (needShowTopPanel.ContainsKey(t))
                    {
                        TopPanel.Visibility = needShowTopPanel[t];
                    }
                    ResizeControl();                  
                    mainFrame.Navigate(t, param);
                }              
            }          
        }

        private void MainWindow_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Input.PointerPoint ptr = e.GetCurrentPoint(sender as UIElement);
            if(ptr.Position.X > menuPanel.Width)
            {
                menuPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void GoToShopCart_Click(object sender, RoutedEventArgs e)
        {
            if (needShowTopPanel.ContainsKey(typeof(ShoppingCart)))
            {
                TopPanel.Visibility = needShowTopPanel[typeof(ShoppingCart)];
            }
            ResizeControl();
            
            mainFrame.Navigate(typeof(ShoppingCart), null);
        }

        private async Task<bool> GetShopCartInfo()
        {     
            try
            {
                Windows.Storage.StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile shoppingCartStorageFile = await storageFolder.GetFileAsync("ShoppingCart.xml");
                //string rootPath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
                //rootPath = rootPath + @"\Data";
                //StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(rootPath);
                //Windows.Storage.StorageFile shoppingCartStorageFile = await folder.GetFileAsync("ShoppingCart.xml");
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
                int goodsCount = 0;
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
                            goodsList.Add(info); goodsCount += Convert.ToInt32(count);
                        }
                    }
                }
                if (goodsCount > 0)
                {
                    shopcartGoodsNumber.Text = Convert.ToString(goodsCount);
                    shopcartGoodsNumber.Visibility = Visibility.Visible;
                    shopcartEllipse.Visibility = Visibility.Visible;
                }
                else
                {
                    shopcartGoodsNumber.Text = "0";
                    shopcartGoodsNumber.Visibility = Visibility.Collapsed;
                    shopcartEllipse.Visibility = Visibility.Collapsed;
                }
                return true;
            }   
            catch(Exception e)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(e.Message);                
#endif
                return false;
            }    
            
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            bool success = await GetShopCartInfo();
        }
        
        public async void ShopCartNotify()
        {
            bool success = await GetShopCartInfo();
        }

    }
}
