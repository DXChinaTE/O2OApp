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
using PancakeCooker.Common.ItemTemplatePanel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PancakeCooker.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Store : Page
    {      
        private Dictionary<CheckBox, GoodsInfo> selectedGoods = new Dictionary<CheckBox, GoodsInfo>();
        private Dictionary<int, GoodsPanel> goodsPanels = new Dictionary<int, GoodsPanel>();
        private BitmapImage source = null;
        private bool isInkPic = false;
        private string imgPath = String.Empty;
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
        private string imageName = String.Empty;
        private const string BUTTON = "Windows.UI.Xaml.Controls.Button";
        private const string CHECKBOX = "Windows.UI.Xaml.Controls.CheckBox";
        private const string TEXTBLOCK = "Windows.UI.Xaml.Controls.TextBlock";
        private const string GRID = "Windows.UI.Xaml.Controls.Grid"; 
        public Store()
        {
            this.InitializeComponent();
            InitParam();                  
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;               
            }
        }
        
        private void InitParam()
        {
            refreshStep = 10;
            PictureMargin = 15;            
            preWidth = Window.Current.Bounds.Width - goodsScrollViewr.Margin.Left - goodsScrollViewr.Margin.Right;
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.Equals("Windows.Mobile"))
            {                
                pictureColumn = 2;
                PicWidth = MinPictureWith = MaxPictureWith = preWidth / pictureColumn - PictureMargin*2;                              
            }
            else
            {
                PicWidth = MinPictureWith = 200;
                MaxPictureWith = 250;
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
                ResourceDictionary dic = Application.Current.Resources;
                IList<ResourceDictionary> dics = dic.MergedDictionaries;
                ResourceDictionary resource = null;
                foreach(var item in dics)
                {
                    if(item.Source.AbsolutePath.Contains("/Files/Styles/Styles.xaml"))
                    {
                        resource = item;
                        break;
                    }

                }
                object style = null;
                if(null != resource)
                {
                                       
                    resource.TryGetValue("CheckBoxStyle", out style);
                }               

                ImageBrush brush = new ImageBrush();
                brush.ImageSource = source;
                //item 1
                GoodsPanel panel1 = new GoodsPanel();
                panel1.PictureWith = PicWidth;
                CheckBox check1 = new CheckBox();
                //check1.SetValue(StyleProperty,);
                check1.Checked += CheckBox_Checked;
                check1.Unchecked += CheckBox_Unchecked;
                if((style as Style) != null)
                {
                    check1.SetValue(StyleProperty, style);
                }
                panel1.Check = check1;
                panel1.PictureBrush = brush;
                panel1.GoodsName = "白杯子";
                panel1.Price = "￥35";
                panel1.PicturePath = imgPath;
                goodsPanels.Add(0,panel1);
                panel1.SetValue(RelativePanel.AlignLeftWithPanelProperty,true);
                panel1.SetValue(RelativePanel.AlignTopWithPanelProperty,true);
                goodsPanel.Children.Add(panel1);

                //item 2
                GoodsPanel panel2 = new GoodsPanel();
                panel2.PictureWith = PicWidth;
                CheckBox check2 = new CheckBox();
                //check1.SetValue(StyleProperty,);
                check2.Checked += CheckBox_Checked;
                check2.Unchecked += CheckBox_Unchecked;
                panel2.Check = check2;
                if ((style as Style) != null)
                {
                    check2.SetValue(StyleProperty, style);
                }
                panel2.PictureBrush = brush;
                panel2.GoodsName = "黑杯子";
                panel2.Price = "￥35";
                panel2.PicturePath = imgPath;
                goodsPanels.Add(1, panel2);
                panel2.SetValue(RelativePanel.RightOfProperty, panel1.Name);
                panel2.SetValue(RelativePanel.AlignTopWithPanelProperty, true);
                goodsPanel.Children.Add(panel2);

                //item 3
                GoodsPanel panel3 = new GoodsPanel();
                panel3.PictureWith = PicWidth;
                CheckBox check3 = new CheckBox();
                //check1.SetValue(StyleProperty,);
                check3.Checked += CheckBox_Checked;
                check3.Unchecked += CheckBox_Unchecked;
                panel3.Check = check3;
                if ((style as Style) != null)
                {
                    check3.SetValue(StyleProperty, style);
                }
                panel3.PictureBrush = brush;
                panel3.GoodsName = "白T恤";
                panel3.Price = "￥86";
                panel3.PicturePath = imgPath;
                goodsPanels.Add(2, panel3);
                panel3.SetValue(RelativePanel.RightOfProperty, panel2.Name);
                panel3.SetValue(RelativePanel.AlignTopWithPanelProperty, true);
                goodsPanel.Children.Add(panel3);

                //item 4
                GoodsPanel panel4 = new GoodsPanel();
                panel4.PictureWith = PicWidth;
                CheckBox check4 = new CheckBox();
                //check1.SetValue(StyleProperty,);
                check4.Checked += CheckBox_Checked;
                check4.Unchecked += CheckBox_Unchecked;
                panel4.Check = check4;
                if ((style as Style) != null)
                {
                    check4.SetValue(StyleProperty, style);
                }
                panel4.PictureBrush = brush;
                panel4.GoodsName = "黑T恤";
                panel4.Price = "￥86";
                panel4.PicturePath = imgPath;
                goodsPanels.Add(3, panel4);
                panel4.SetValue(RelativePanel.RightOfProperty, panel3.Name);
                panel4.SetValue(RelativePanel.AlignTopWithPanelProperty, true);
                goodsPanel.Children.Add(panel4);
                ResetPicturePanelPositon();
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
            goodsScrollViewr.Width =  Window.Current.Bounds.Width - goodsScrollViewr.Margin.Left - goodsScrollViewr.Margin.Right;
            goodsScrollViewr.Height =  Window.Current.Bounds.Height - topPannel.Height;
            double currentWith = goodsScrollViewr.Width;
            if (Math.Abs(currentWith - preWidth) > refreshStep)
            {
                preWidth = currentWith;
                ResetPicturePanelPositon();
            }            
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox check = sender as CheckBox;
            if(null == sender)
            {
                return;
            }
            var parent = VisualTreeHelper.GetParent(check);
            if(null == parent)
            {
                return;
            }
            var grid = VisualTreeHelper.GetParent(parent);
            if(null == grid)
            {
                return;
            }
            var panel = VisualTreeHelper.GetParent(grid);
            if (null != panel && panel.ToString().Equals(typeof(GoodsPanel).ToString()))
            {
                GoodsPanel goods = panel as GoodsPanel;
                GoodsInfo info = new GoodsInfo();
                info.img = source;
                info.imgUri = imgPath;
                info.count = 1;
                info.imageName = imageName;
                info.name = goods.GoodsName;
                info.price = goods.Price.Substring(goods.Price.IndexOf('￥')+1);
                selectedGoods.Add(check, info);
            }            
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
