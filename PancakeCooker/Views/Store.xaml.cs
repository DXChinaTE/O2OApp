/*
Demo只hardcode 4个商品
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using PancakeCooker.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PancakeCooker.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Store : Page
    {      
        private Dictionary<CheckBox, GoodsInfo> selectedGoods = new Dictionary<CheckBox, GoodsInfo>();
        private BitmapImage source = null;
        private bool isInkPic = false;
        private string imgPath = String.Empty;
        private double _picWidth = 220;
        private Thickness _margin = new Thickness(10, 0, 0, 265);
        private string imageName = String.Empty;
        private const string BUTTON = "Windows.UI.Xaml.Controls.Button";
        private const string CHECKBOX = "Windows.UI.Xaml.Controls.CheckBox";
        private const string TEXTBLOCK = "Windows.UI.Xaml.Controls.TextBlock";
        private const string GRID = "Windows.UI.Xaml.Controls.Grid"; 
        public Store()
        {
            this.InitializeComponent();
            double width = this.Height;
            width = Window.Current.Bounds.Height;
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.Equals("Windows.Mobile"))
            {
                this.Margin = new Thickness(0, 0, 0, 0);
                _picWidth = (Window.Current.Bounds.Width - 90) / 2;
                _margin = new Thickness(10, 0, 0, 35 + _picWidth);
            }
            GoodsCVS.Source = new ObservableCollection<Carrier>();
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }
       
        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            MainPage._CurrentHandle.GoBack();            
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            }
        }
       
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            object obj = e.Parameter;
            if (null != obj)
            {
                string path = obj as string;
                imgPath = path;
                int index = 0;
                while (index >= 0)
                {
                    index = path.IndexOf('\\');
                    if (index >= 0)
                    {
                        path = path.Substring(index + 1);
                    }
                }
                index = path.IndexOf('.');
                if(index < 0)
                {
                    imageName = path;
                }
                else
                {
                    imageName = path.Substring(0, index);
                }
                
                
                StorageFile file = await StorageFile.GetFileFromPathAsync(imgPath);
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapImage image = new BitmapImage();
                    image.SetSource(fileStream);
                    source = image;
                }
                InkCanvas inkCanvas = new InkCanvas();
                inkCanvas.InkPresenter.StrokeContainer.Clear();
                var stream = await file.OpenSequentialReadAsync();
                if (null != stream)
                {
                    try
                    {
                        await inkCanvas.InkPresenter.StrokeContainer.LoadAsync(stream);
                        if (inkCanvas.InkPresenter.StrokeContainer.GetStrokes().Count != 0)
                        {
                            isInkPic = true;
                        }
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
                        isInkPic = false;
                    }
                }
            }
            if (null != source)
            {
                ObservableCollection<Carrier> list = (ObservableCollection<Carrier>)GoodsCVS.Source;
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = source;
                //item 1
                Carrier carrier1 = new Carrier();
                carrier1.Name = "白杯子";
                carrier1.price = "￥35";
                carrier1.picWidth = _picWidth;
                carrier1.checkBorder = _margin;
                carrier1.backGround = brush;
                list.Add(carrier1);

                //item 2
                Carrier carrier2 = new Carrier();
                carrier2.Name = "黑杯子";
                carrier2.price = "￥35";
                carrier2.picWidth = _picWidth;
                carrier2.checkBorder = _margin;
                carrier2.backGround = brush;
                list.Add(carrier2);

                //item 3
                Carrier carrier3 = new Carrier();
                carrier3.Name = "白T恤";
                carrier3.price = "￥86";
                carrier3.picWidth = _picWidth;
                carrier3.checkBorder = _margin;
                carrier3.backGround = brush;
                list.Add(carrier3);

                //item 4
                Carrier carrier4 = new Carrier();
                carrier4.Name = "黑T恤";
                carrier4.price = "￥86";
                carrier4.picWidth = _picWidth;
                carrier4.checkBorder = _margin;
                carrier4.backGround = brush;
                list.Add(carrier4);
            }
        }

        private void returnBack_Click(object sender, RoutedEventArgs e)
        {
            if(isInkPic)
            {                
                MainPage._CurrentHandle.Navigate(typeof(Ink), imgPath);
            }
            else
            {
                MainPage._CurrentHandle.Navigate(typeof(Custom));
            }            
        }

        private void select_Click(object sender, RoutedEventArgs e)
        {
            List<GoodsInfo> list = new List<GoodsInfo>();         
            foreach(var item in selectedGoods)
            {
                list.Add(item.Value);
            }
            if (list.Count > 0)
            {
                MainPage._CurrentHandle.Navigate(typeof(ShoppingCart), list);
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            topPannel.Width = Window.Current.Bounds.Width;
            goodsGrid.Width = buttom.Width =  Window.Current.Bounds.Width;
            buttom.Height =  Window.Current.Bounds.Height - topPannel.Height;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox check = sender as CheckBox;
            if(null == sender)
            {
                return;
            }
            UIElement parent = (UIElement)VisualTreeHelper.GetParent(check);
            if(null == parent)
            {
                return;
            }
            int count = VisualTreeHelper.GetChildrenCount(parent);
            GoodsInfo info = new GoodsInfo();
            //info.check = check;
            for(int i=0; i<count; i++)
            {
                UIElement child = (UIElement)VisualTreeHelper.GetChild(parent,i);
                string strType = child.ToString();
                if(strType.Equals(CHECKBOX))
                {
                    continue;
                }
                else if(strType.Equals(TEXTBLOCK))
                {
                    TextBlock text = (TextBlock)child;
                    if(text.Text.StartsWith("￥"))
                    {
                        info.price = text.Text.Substring(text.Text.IndexOf('￥') + 1);
                    }
                    else
                    {
                        info.name = text.Text;
                    }
                }               
            }
            info.img = source;
            info.imgUri = imgPath;
            info.count = 1;
            info.imageName = imageName;
            selectedGoods.Add(check,info);
            return;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox check = sender as CheckBox;
            if (null == sender)
            {
                return;
            }
            selectedGoods.Remove(check);
        }
    }

    public class Carrier
    {
        public string Name { get; set; }
        public string price { get; set; }
        public Thickness checkBorder { get; set; }
        public double picWidth { get; set; }
        public ImageBrush backGround { get; set; }      
    }
}
