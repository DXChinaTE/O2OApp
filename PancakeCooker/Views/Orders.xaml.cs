using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.Storage.Streams;
using PancakeCooker.Views;
using PancakeCooker.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PancakeCooker.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Orders : Page
    {
        private const string RELATIVEPANEL = "Windows.UI.Xaml.Controls.RelativePanel";
        private const string GRID = "Windows.UI.Xaml.Controls.Grid";
        private const string IMAGE = "Windows.UI.Xaml.Controls.Image";
        private const string SLIDEPANEL = "PancakeCooker.Common.SlidePanel";
        private List<OrderInfo> payedOrders = new List<OrderInfo>();
        private List<OrderInfo> notPayOrders = new List<OrderInfo>();
        private List<OrderInfo> completedOrders = new List<OrderInfo>();
        private Button preClickedButton = null; 
        private enum OrderType{ NOT_PAY, PAYED, COMPLETED };      
        private OrderGridSizeInfo orderGridSizeInfo = new OrderGridSizeInfo();
        private bool isMobile = false;
        public Orders()
        {
            this.InitializeComponent();
            InitControl();
            InitOrderInfo();
            preClickedButton = payedBtn;           
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
                isMobile = true;
            }
        }

        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            if(infoPanel.Visibility == Visibility.Visible || myPicsPanel.Visibility == Visibility.Visible || addAddressPanel.Visibility == Visibility.Visible)
            {
                addAddressPanel.Visibility = myPicsPanel.Visibility = infoPanel.Visibility = Visibility.Collapsed;
                menuPanel.Visibility = Visibility.Visible;
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
   
        private void ResizeControl()
        {
            if (Window.Current.Bounds.Width > 640)
            {
                menuPanel.Width = 300;
                //contractRectangle.Height = 150;
                userPic.Width = 80;
                userPic.Height = 80;
                userPic.Margin = new Thickness(110, 110, 0, 0);
                //infoPanel.Visibility = Visibility.Visible;
                menuPanel.Visibility = Visibility.Visible;
                //menuPanel.Width = 200;
                orderScrollViewer.Width = orderGrid.Width = addAddressPanel.Width = myPicsPanel.Width =  infoPanel.Width = infoGrid.Width = Window.Current.Bounds.Width - menuPanel.Width;
                addAddressPanel.Height = myPicsPanel.Height = infoPanel.Height = infoGrid.Height = menuPanel.Height = Window.Current.Bounds.Height - MainPage._CurrentHandle._TopPenelHeight;

                orderGridSizeInfo.infoPanelHeight = 30;              
                orderGridSizeInfo.goodsPicHeight = 150;
                orderGridSizeInfo.goodsPicWidth = 150;
                orderGridSizeInfo.margin = 10;
            }
            else
            {
                if(isMobile)
                {
                    menuPanel.Visibility = Visibility.Visible;
                    infoPanel.Visibility = Visibility.Collapsed;
                    menuPanel.Width = Window.Current.Bounds.Width;
                    userPic.Margin = new Thickness((menuPanel.Width-userPic.Width)/2,(300-userPic.Height)/2,0,0);
                    orderScrollViewer.Width = orderGrid.Width = addAddressPanel.Width = myPicsPanel.Width = infoPanel.Width = infoGrid.Width = Window.Current.Bounds.Width;
                    addAddressPanel.Height = myPicsPanel.Height = infoPanel.Height = infoGrid.Height = menuPanel.Height = Window.Current.Bounds.Height - MainPage._CurrentHandle._TopPenelHeight;

                    orderGridSizeInfo.infoPanelHeight = 30;                   
                    orderGridSizeInfo.goodsPicHeight = 150;
                    orderGridSizeInfo.goodsPicWidth = 150;
                    orderGridSizeInfo.margin = 10;
                }
                else
                {
                    //menuPanel.Visibility = Visibility.Collapsed;
                    menuPanel.Width = 200;
                    menuPanel.Height = 300;
                    //contractRectangle.Height = 100;
                    userPic.Width = 60;
                    userPic.Height = 60;
                    userPic.Margin = new Thickness(70,120,0,0);
                    orderScrollViewer.Width = orderGrid.Width = addAddressPanel.Width = myPicsPanel.Width = infoPanel.Width = infoGrid.Width = Window.Current.Bounds.Width - menuPanel.Width;
                    addAddressPanel.Height = myPicsPanel.Height = infoPanel.Height = infoGrid.Height = menuPanel.Height = Window.Current.Bounds.Height - MainPage._CurrentHandle._TopPenelHeight;

                    orderGridSizeInfo.infoPanelHeight = 30;
                    //orderGridSizeInfo.orderInfoPanelHeight = 20;
                    //orderGridSizeInfo.goodsGridHeight = 250;
                    orderGridSizeInfo.goodsPicHeight = 150;
                    orderGridSizeInfo.goodsPicWidth = 150;
                    orderGridSizeInfo.margin = 10;
                }                
            }
        }
        private void InitControl()
        {
            if(Window.Current.Bounds.Width > 640)
            {
                myInfo.SelectedIndex = 0;
            }
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/sample.jpg"));
            userPic.Fill = brush;          
        }
    
        private async void InitOrderInfo()
        {
            bool success = await GetOrderInfo();
            int totalOrderCount = notPayOrders.Count + payedOrders.Count + completedOrders.Count;
            if(totalOrderCount>0)
            {
                myordersNumber.Text = Convert.ToString(totalOrderCount);
                myordersNumber.Visibility = Visibility.Visible;
                myordersEllipse.Visibility = Visibility.Visible;
            }            
            if(notPayOrders.Count > 0)
            {
                notPayEllipse.Visibility = Visibility.Visible;
                notPayNumber.Visibility = Visibility.Visible;
                notPayNumber.Text = Convert.ToString(notPayOrders.Count);
            } 
            if(payedOrders.Count > 0)
            {
                payedEllipse.Visibility = Visibility.Visible;
                payedNumber.Visibility = Visibility.Visible;
                payedNumber.Text = Convert.ToString(payedOrders.Count);
            } 
            if(completedOrders.Count>0)
            {
                completedEllipse.Visibility = Visibility.Visible;
                completedNumber.Visibility = Visibility.Visible;
                completedNumber.Text = Convert.ToString(completedOrders.Count);
            }                                
            InsertOrders(OrderType.PAYED);
        }

        private async Task<bool> GetOrderInfo()
        {
            notPayOrders.Clear();
            payedOrders.Clear();
            completedOrders.Clear();
            List<IXmlNode> orderNodes = new List<IXmlNode>();
            //Windows.Storage.StorageFolder storageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Data");
            Windows.Storage.StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile storageFile = await storageFolder.GetFileAsync("Goods.xml");
            XmlDocument doc   = await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(storageFile);
            IXmlNode root = null;
            //遍历订单
            foreach(var item in doc.ChildNodes)
            {
                var name = item.NodeName;
               if(name.Equals("orders"))
                {
                    root = item;
                    break;
                }
            }
            if(null == root)
            {
                return false;
            }
            foreach(var item in root.ChildNodes)
            {              
                var name = item.NodeName;
                if(name.Equals("order"))
                {
                    orderNodes.Add(item);         
                }            
            }
            
            foreach(var item in orderNodes)
            {
                string customer = item.SelectSingleNode("name").InnerText;
                string mobile = item.SelectSingleNode("mobile").InnerText;
                string id = item.SelectSingleNode("id").InnerText;
                string price = item.SelectSingleNode("totalPrice").InnerText;
                string payed = item.SelectSingleNode("payed").InnerText;
                string completed = item.SelectSingleNode("completed").InnerText;
                string address = item.SelectSingleNode("address").InnerText;
                IXmlNode goodsNode = item.SelectSingleNode("goods");
                List<GoodsInfo> goodsList = new List<GoodsInfo>();
                foreach(var goods in goodsNode.ChildNodes)
                {
                    if(goods.NodeName.Equals("item"))
                    {
                        string img = goods.SelectSingleNode("img").InnerText;
                        string singlePrice = goods.SelectSingleNode("price").InnerText;
                        string name = goods.SelectSingleNode("productName").InnerText;
                        string count = goods.SelectSingleNode("count").InnerText;
                        string imagename = goods.SelectSingleNode("imageName").InnerText;                       
                        GoodsInfo info = new GoodsInfo();
                        StorageFile file = await StorageFile.GetFileFromPathAsync(img);
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
                        goodsList.Add(info);
                    }
                }
                OrderInfo orderInfo = new OrderInfo();
                orderInfo.address = address;
                orderInfo.id = id;
                orderInfo.price = price;
                orderInfo.goodsList = goodsList;
                orderInfo.mobile = mobile;
                orderInfo.payed = payed;
                orderInfo.completed = completed;
                orderInfo.customerName = customer;
                if(!payed.Equals("true"))
                {
                    notPayOrders.Add(orderInfo);
                }
                else if (payed.Equals("true") && completed.Equals("false"))
                {
                    payedOrders.Add(orderInfo);
                }
                else
                {
                    completedOrders.Add(orderInfo);
                }
            }

            return true;
        }
     
        private void InsertOrders(OrderType type)
        {
            orderGrid.Children.Clear();
            orderGrid.RowDefinitions.Clear();
            orderGrid.ColumnDefinitions.Clear();
            List<OrderInfo> orders = null;
            switch(type)
            {
                case OrderType.NOT_PAY:
                    orders = notPayOrders;
                    break;
                case OrderType.PAYED:
                    orders = payedOrders;
                    break;
                case OrderType.COMPLETED:
                    orders = completedOrders;
                    break;
                default:
                    break;
            }
            if(null == orders)
            {
                return;
            }
            int count = orders.Count;
            for(int i=0; i<count; i++)
            {
                RowDefinition row = new RowDefinition();
                //row.Height = new GridLength(orderGridSizeInfo.orderGridHeight);
                orderGrid.RowDefinitions.Add(row);
            }
            int nRow = 0;
            foreach(var item in orders)
            {
                Grid oneOrderGrid = new Grid();
                oneOrderGrid.Background = new SolidColorBrush(Color.FromArgb(255,235,235,235));                          
                            
                //Init four panels
                RelativePanel topPanel = new RelativePanel();
                topPanel.Background = new SolidColorBrush(Color.FromArgb(255,241,241,241));
                topPanel.Height = orderGridSizeInfo.infoPanelHeight + orderGridSizeInfo.margin;
                topPanel.Width = infoPanel.Width;
                Grid.SetColumn(topPanel, 0);
                Grid.SetRow(topPanel, 0);
                oneOrderGrid.Children.Add(topPanel);

                RelativePanel addressPanel = new RelativePanel();
                addressPanel.Height = orderGridSizeInfo.infoPanelHeight + orderGridSizeInfo.margin;
                addressPanel.Width = infoPanel.Width;
                addressPanel.Background = new SolidColorBrush(Color.FromArgb(255, 241, 241, 241));
                Grid.SetColumn(addressPanel, 0);
                Grid.SetRow(addressPanel, 2);
                oneOrderGrid.Children.Add(addressPanel);

                //goods panel
                RelativePanel goodsGridPanel = new RelativePanel();              
                goodsGridPanel.Background = new SolidColorBrush(Color.FromArgb(255, 235, 235, 235));
                //goodsGridPanel.Width = infoPanel.Width;
                goodsGridPanel.Height = orderGridSizeInfo.goodsPicHeight + 4 * orderGridSizeInfo.margin + orderGridSizeInfo.infoPanelHeight*2;

                //SlidePanel slidePanel = new SlidePanel(infoPanel.Width);
                //slidePanel.Add(goodsGridPanel,orderGridSizeInfo.goodsPicWidth);
                //Grid.SetColumn(slidePanel, 0);
                //Grid.SetRow(slidePanel, 1);
                //oneOrderGrid.Children.Add(slidePanel);

                //Grid.SetColumn(goodsGridPanel, 0);
                //Grid.SetRow(goodsGridPanel, 1);
                //oneOrderGrid.Children.Add(goodsGridPanel);

                RelativePanel buttonPanel = new RelativePanel();
                buttonPanel.Background = new SolidColorBrush(Color.FromArgb(255, 241, 241, 241));
                buttonPanel.Width = infoPanel.Width;
                buttonPanel.Height = orderGridSizeInfo.infoPanelHeight + orderGridSizeInfo.margin*2;
                Grid.SetColumn(buttonPanel, 0);
                Grid.SetRow(buttonPanel, 3);
                buttonPanel.VerticalAlignment = VerticalAlignment.Top;
                oneOrderGrid.Children.Add(buttonPanel);

                RowDefinition topRow = new RowDefinition();
                topRow.Height = new GridLength(topPanel.Height);
                oneOrderGrid.RowDefinitions.Add(topRow);
                RowDefinition goodsGridRow = new RowDefinition();
                goodsGridRow.Height = new GridLength(goodsGridPanel.Height);
                oneOrderGrid.RowDefinitions.Add(goodsGridRow);
                RowDefinition addressRow = new RowDefinition();
                addressRow.Height = new GridLength(addressPanel.Height);
                oneOrderGrid.RowDefinitions.Add(addressRow);
                RowDefinition buttonRow = new RowDefinition();
                buttonRow.Height = new GridLength(buttonPanel.Height + orderGridSizeInfo.margin);
                oneOrderGrid.RowDefinitions.Add(buttonRow);

                #region order info panel items
                TextBlock topLeft = new TextBlock();
                topLeft.Width = 80;
                topLeft.Height = orderGridSizeInfo.infoPanelHeight;
                topLeft.Text = "订单号";
                topLeft.Foreground = new SolidColorBrush(Color.FromArgb(255,51,51,51));
                topLeft.Margin = new Thickness(20, 0, 0, 0);
                topLeft.SetValue(RelativePanel.AlignLeftWithPanelProperty,true);
                topLeft.SetValue(RelativePanel.AlignBottomWithPanelProperty, true);
                topPanel.Children.Add(topLeft);

                TextBlock topRight = new TextBlock();
                topRight.Width = 150;
                topRight.Height = orderGridSizeInfo.infoPanelHeight;
                topRight.Text = item.id;
                topRight.Foreground = new SolidColorBrush(Color.FromArgb(255, 51, 51, 51));
                topRight.Margin = new Thickness(0, 0, 20, 0);
                topRight.SetValue(RelativePanel.AlignRightWithPanelProperty, true);
                topRight.SetValue(RelativePanel.AlignBottomWithPanelProperty, true);
                topRight.TextAlignment = TextAlignment.Right;
                topPanel.Children.Add(topRight);
                #endregion

                #region address panel item
                TextBlock addressLeft = new TextBlock();
                addressLeft.Width = topPanel.Width / 2;
                addressLeft.Height = orderGridSizeInfo.infoPanelHeight;
                addressLeft.Margin = new Thickness(20,0,0,0);
                addressLeft.Text = "配送地址";
                addressLeft.Foreground = new SolidColorBrush(Color.FromArgb(255, 51, 51, 51));
                addressLeft.SetValue(RelativePanel.AlignLeftWithPanelProperty, true);
                addressLeft.SetValue(RelativePanel.AlignBottomWithPanelProperty, true);
                addressPanel.Children.Add(addressLeft);
               
                TextBlock addressRight = new TextBlock();
                addressRight.Width = 150;
                addressRight.Height = orderGridSizeInfo.infoPanelHeight;
                addressRight.Text = item.address;
                addressRight.Foreground = new SolidColorBrush(Color.FromArgb(255, 51, 51, 51));
                addressRight.Margin = new Thickness(0,0,20,0);
                addressRight.TextAlignment = TextAlignment.Right;
                addressRight.SetValue(RelativePanel.AlignRightWithPanelProperty, true);
                addressRight.SetValue(RelativePanel.AlignBottomWithPanelProperty, true);
                addressPanel.Children.Add(addressRight);
                #endregion

                #region button panel item
                TextBlock priceText = new TextBlock();
                priceText.Width = 12;
                priceText.Height = 20;
                priceText.Text = "￥";
                priceText.Name = "priceText";
                priceText.Foreground = new SolidColorBrush(Color.FromArgb(255,253,40,3));
                priceText.TextAlignment = TextAlignment.Left;
                priceText.SetValue(RelativePanel.AlignLeftWithPanelProperty, true);
                priceText.SetValue(RelativePanel.AlignHorizontalCenterWithPanelProperty, true);
                priceText.Margin = new Thickness(17, 0, 0, -32);
                buttonPanel.Children.Add(priceText);

                TextBlock priceNumberText = new TextBlock();
                priceNumberText.Width = 70;
                priceNumberText.Height = orderGridSizeInfo.infoPanelHeight;
                priceNumberText.Text = item.price;
                priceNumberText.Foreground = new SolidColorBrush(Colors.Red);
                priceNumberText.FontSize = 20;
                priceNumberText.TextAlignment = TextAlignment.Left;
                priceNumberText.SetValue(RelativePanel.RightOfProperty, "priceText");
                priceNumberText.SetValue(RelativePanel.AlignHorizontalCenterWithPanelProperty, true);
                priceNumberText.Margin = new Thickness(3, 0, 0, 0);
                buttonPanel.Children.Add(priceNumberText);

                Button btnPay = new Button();
                if (item.payed.Equals("true"))
                {
                    btnPay.Visibility = Visibility.Collapsed;
                }
                btnPay.Width = 100;
                btnPay.Height = orderGridSizeInfo.infoPanelHeight;
                btnPay.VerticalContentAlignment = VerticalAlignment.Center;
                btnPay.HorizontalContentAlignment = HorizontalAlignment.Center;
                btnPay.BorderThickness = new Thickness(0);
                btnPay.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 78, 0));
                btnPay.Name = "btnPay";
                TextBlock btnPayText = new TextBlock();
                btnPayText.Text = "进行付款";               
                btnPayText.Foreground = new SolidColorBrush(Colors.White);                
                btnPay.Content = btnPayText;
                //btnPay.Foreground = new SolidColorBrush(Colors.White);
                btnPay.Background = new SolidColorBrush(Color.FromArgb(255,255,78,0));
                btnPay.SetValue(RelativePanel.AlignRightWithPanelProperty,true);
                btnPay.SetValue(RelativePanel.AlignHorizontalCenterWithPanelProperty, true);
                btnPay.Margin  = new Thickness(0, 0, 20, 0);
                buttonPanel.Children.Add(btnPay);           

                Button btnCancel = new Button();
                btnCancel.Width = 100;
                btnCancel.Height = orderGridSizeInfo.infoPanelHeight;
                btnCancel.VerticalContentAlignment = VerticalAlignment.Center;
                btnCancel.HorizontalContentAlignment = HorizontalAlignment.Center;
                //btnCancel.Content = "取消订单";
                TextBlock btnCancelText = new TextBlock();
                btnCancelText.Text = "取消订单";
                btnCancelText.Foreground = new SolidColorBrush(Colors.White);                
                btnCancel.Content = btnCancelText;
                btnCancel.BorderThickness = new Thickness(0);
                btnCancel.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 148, 148, 148));
                //btnCancel.Foreground = new SolidColorBrush(Colors.White);
                btnCancel.Background = new SolidColorBrush(Color.FromArgb(255,148,148,148));
                btnCancel.Margin = new Thickness(0, 0, 5, 0);
                btnCancel.SetValue(RelativePanel.LeftOfProperty, "btnPay");
                btnCancel.SetValue(RelativePanel.AlignHorizontalCenterWithPanelProperty, true);
                buttonPanel.Children.Add(btnCancel);
                #endregion

                #region goods detail panel item                            
                int goodsCount = item.goodsList.Count;            
                double goodsGridPanelWidth = 0;
                for(int i=0; i< goodsCount; i++)
                {
                    RelativePanel goodsItemPanel = new RelativePanel();
                    goodsItemPanel.Background = new SolidColorBrush(Color.FromArgb(255,224,224,224));
                    goodsItemPanel.SetValue(RelativePanel.AlignLeftWithPanelProperty,true);
                    goodsItemPanel.Margin = new Thickness(goodsGridPanelWidth + orderGridSizeInfo.margin,0,0,0);
                    //goods name
                    TextBlock goodsNameText = new TextBlock();
                    goodsNameText.Width = 60;
                    goodsNameText.Height = orderGridSizeInfo.infoPanelHeight - 5;
                    goodsNameText.Text = item.goodsList[i].name;
                    goodsNameText.TextAlignment = TextAlignment.Left;
                    goodsNameText.Foreground = new SolidColorBrush(Color.FromArgb(255,51,51,51));
                    goodsNameText.SetValue(RelativePanel.AlignLeftWithPanelProperty,true);
                    goodsNameText.SetValue(RelativePanel.AlignBottomWithPanelProperty,true);
                    goodsNameText.Margin = new Thickness(orderGridSizeInfo.margin,0,0,orderGridSizeInfo.margin);
                    goodsItemPanel.Children.Add(goodsNameText);

                    //goods number
                    TextBlock numberText = new TextBlock();
                    numberText.Height = orderGridSizeInfo.infoPanelHeight - 5;
                    numberText.Width = 25;
                    numberText.Name = "number";
                    numberText.TextAlignment = TextAlignment.Right;
                    numberText.Foreground = new SolidColorBrush(Color.FromArgb(255,51,51,51));
                    numberText.Text = "x" + Convert.ToString(item.goodsList[i].count);
                    numberText.SetValue(RelativePanel.AlignRightWithPanelProperty,true);
                    numberText.SetValue(RelativePanel.AlignBottomWithPanelProperty,true);
                    numberText.Margin = new Thickness(0,0, orderGridSizeInfo.margin, orderGridSizeInfo.margin);
                    goodsItemPanel.Children.Add(numberText);

                    //goods price
                    TextBlock singlePriceText = new TextBlock();
                    singlePriceText.Height = orderGridSizeInfo.infoPanelHeight;
                    singlePriceText.Width = 30;
                    singlePriceText.Name = "singlePriceText";
                    singlePriceText.TextAlignment = TextAlignment.Left;
                    singlePriceText.FontSize = 20;
                    singlePriceText.Foreground = new SolidColorBrush(Color.FromArgb(255, 253, 40, 3));
                    singlePriceText.Text = item.goodsList[i].price;
                    singlePriceText.SetValue(RelativePanel.AlignBottomWithPanelProperty,true);
                    singlePriceText.SetValue(RelativePanel.LeftOfProperty, "number");
                    singlePriceText.Margin = new Thickness(0,0,0, orderGridSizeInfo.margin);
                    goodsItemPanel.Children.Add(singlePriceText);

                    TextBlock priceSymbol = new TextBlock();
                    priceSymbol.Height = orderGridSizeInfo.infoPanelHeight - 5;
                    priceSymbol.Width = 20;
                    priceSymbol.TextAlignment = TextAlignment.Right;
                    priceSymbol.FontSize = 16;
                    priceSymbol.Foreground = new SolidColorBrush(Color.FromArgb(255, 253, 40, 3));
                    priceSymbol.Text = "￥";
                    priceSymbol.SetValue(RelativePanel.AlignBottomWithPanelProperty,true);
                    priceSymbol.SetValue(RelativePanel.LeftOfProperty, "singlePriceText");
                    priceSymbol.Margin = new Thickness(0, 0, 0, orderGridSizeInfo.margin);
                    goodsItemPanel.Children.Add(priceSymbol);

                    //pictrue name
                    TextBlock picNameText = new TextBlock();
                    picNameText.Name = "picNameText";
                    picNameText.Height = orderGridSizeInfo.infoPanelHeight;
                    picNameText.Width = orderGridSizeInfo.goodsPicWidth;
                    picNameText.Text = item.goodsList[i].imageName;
                    picNameText.TextAlignment = TextAlignment.Left;
                    picNameText.Foreground = new SolidColorBrush(Color.FromArgb(255, 51, 51, 51));
                    picNameText.FontSize = 18;
                    picNameText.SetValue(RelativePanel.AlignLeftWithPanelProperty,true);
                    picNameText.SetValue(RelativePanel.AboveProperty, "singlePriceText");
                    picNameText.Margin = new Thickness(orderGridSizeInfo.margin, 0, 0, orderGridSizeInfo.margin);
                    goodsItemPanel.Children.Add(picNameText);

                    //goods picture
                    Grid picGrid = new Grid();
                    RowDefinition row = new RowDefinition();
                    row.Height = new GridLength(orderGridSizeInfo.goodsPicHeight);
                    ColumnDefinition col = new ColumnDefinition();
                    col.Width = new GridLength(orderGridSizeInfo.goodsPicWidth);
                    picGrid.RowDefinitions.Add(row);
                    picGrid.ColumnDefinitions.Add(col);
                    Border border = new Border();
                    border.BorderBrush = new SolidColorBrush(Colors.White);
                    border.BorderThickness = new Thickness(1);
                    border.CornerRadius = new CornerRadius(3, 3, 3, 3);
                    border.Background = item.goodsList[i].brush;
                    picGrid.Children.Add(border);
                    picGrid.SetValue(RelativePanel.AlignLeftWithPanelProperty, true);
                    picGrid.SetValue(RelativePanel.AlignRightWithPanelProperty,true);
                    picGrid.SetValue(RelativePanel.AlignTopWithPanelProperty,true);
                    picGrid.SetValue(RelativePanel.AboveProperty, "picNameText");
                    picGrid.Margin = new Thickness(orderGridSizeInfo.margin, orderGridSizeInfo.margin, orderGridSizeInfo.margin, orderGridSizeInfo.margin);
                    goodsItemPanel.Children.Add(picGrid);

                    goodsGridPanelWidth += (orderGridSizeInfo.goodsPicWidth + 3*orderGridSizeInfo.margin);
                    goodsGridPanel.Children.Add(goodsItemPanel);
                }              
                SlidePanel slidePanel = new SlidePanel(infoPanel.Width);
                goodsGridPanel.Width = goodsGridPanelWidth;
                slidePanel.Add(goodsGridPanel, orderGridSizeInfo.goodsPicWidth + orderGridSizeInfo.margin);
                Grid.SetColumn(slidePanel, 0);
                Grid.SetRow(slidePanel, 1);
                oneOrderGrid.Children.Add(slidePanel);
                #endregion



                Grid.SetColumn(oneOrderGrid, 0);
                Grid.SetRow(oneOrderGrid, nRow);
                orderGrid.Children.Add(oneOrderGrid);
                nRow++;
            }
        }

        private void notPayBtn_Click(object sender, RoutedEventArgs e)
        {          
            InsertOrders(OrderType.NOT_PAY);
            if(null != preClickedButton)
            {
                preClickedButton.Opacity = 0.6;
            }
            preClickedButton = sender as Button;
            preClickedButton.Opacity = 1;
        }

        private void payedBtn_Click(object sender, RoutedEventArgs e)
        {
            InsertOrders(OrderType.PAYED);
            if (null != preClickedButton)
            {
                preClickedButton.Opacity = 0.6;
            }
            preClickedButton = sender as Button;
            preClickedButton.Opacity = 1;
        }

        private void completBtn_Click(object sender, RoutedEventArgs e)
        {         
            InsertOrders(OrderType.COMPLETED);
            if (null != preClickedButton)
            {
                preClickedButton.Opacity = 0.6;
            }
            preClickedButton = sender as Button;
            preClickedButton.Opacity = 1;
        }

        private void myInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(isMobile)
            {
                menuPanel.Visibility = Visibility.Collapsed;
            }
            ListBox listBox = sender as ListBox;
            if(null == listBox)
            {
                return;
            }
            ListBoxItem selectedItem = (ListBoxItem)listBox.SelectedItem;
            if(null != selectedItem)
            {
                RelativePanel panel = (RelativePanel)selectedItem.Content;
                if(null == panel)
                {
                    return;
                }
                if(panel.Equals(myorderPanel))
                {
                    infoPanel.Visibility = Visibility.Visible;
                    addAddressPanel.Visibility = myPicsPanel.Visibility = Visibility.Collapsed;
                    orderPic.Source = new BitmapImage(new Uri("ms-appx:///Assets/myorder-selected.png"));
                    addressPic.Source = new BitmapImage(new Uri("ms-appx:///Assets/adress.png"));
                    mypicPic.Source = new BitmapImage(new Uri("ms-appx:///Assets/myPic.png"));
                }
                else if(panel.Equals(adressPanel))
                {
                    myPicsPanel.Visibility = infoPanel.Visibility = Visibility.Collapsed;
                    addAddressPanel.Visibility = Visibility.Visible;
                    orderPic.Source = new BitmapImage(new Uri("ms-appx:///Assets/myorder.png"));
                    addressPic.Source = new BitmapImage(new Uri("ms-appx:///Assets/adress-selected.png"));
                    mypicPic.Source = new BitmapImage(new Uri("ms-appx:///Assets/myPic.png"));
                }
                else
                {
                    myPicsPanel.Visibility = Visibility.Visible;
                    infoPanel.Visibility = addAddressPanel.Visibility = Visibility.Collapsed;
                    orderPic.Source = new BitmapImage(new Uri("ms-appx:///Assets/myorder.png"));
                    addressPic.Source = new BitmapImage(new Uri("ms-appx:///Assets/adress.png"));
                    mypicPic.Source = new BitmapImage(new Uri("ms-appx:///Assets/myPic-selected.png"));
                }
            }          
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeControl();       
            int count = VisualTreeHelper.GetChildrenCount(orderGrid);
            for (int i = 0; i < count; i++)
            {
                UIElement ele = (UIElement)VisualTreeHelper.GetChild(orderGrid, i);
                string type = ele.ToString();
                if (type.Equals(GRID))
                {
                    Grid grid = (Grid)ele;                  
                    grid.Width = orderGrid.Width;
                    int childCountOfGrid = VisualTreeHelper.GetChildrenCount(grid);
                    for(int j=0; j< childCountOfGrid; j++)
                    {
                        UIElement gridChild = (UIElement)VisualTreeHelper.GetChild(grid,j);
                        if(gridChild.ToString().Equals(RELATIVEPANEL))
                        {
                            RelativePanel panel =  (RelativePanel)gridChild;                       
                            panel.Width = orderGrid.Width;                               
                        }
                        else if (gridChild.ToString().Equals(SLIDEPANEL))
                        {
                            RelativePanel panel = (RelativePanel)gridChild;                          
                            panel.Width = orderGrid.Width;
                        }
                    }                     
                }
                else if(type.Equals(RELATIVEPANEL))
                {
                    RelativePanel panel = (RelativePanel)ele;                 
                    panel.Width = orderGrid.Width;
                }
            }
        }
    }
}
