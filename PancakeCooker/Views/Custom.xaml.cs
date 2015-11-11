using System;
using System.Threading.Tasks;
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
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using PancakeCooker.Common;
using PancakeCooker.Common.ItemTemplatePanel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PancakeCooker.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Custom : Page
    {     
        private Dictionary<int, PicturePanel> picturePanels = new Dictionary<int, PicturePanel>();
        private List<string> imageList = new List<string>();
        private string selectedPicPath = String.Empty;
        private const string BORDER = "Windows.UI.Xaml.Controls.Border";
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
                picPanelWidth = _picWidth + 2 * PictureMargin;
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
                picPanelWidth = _picWidth + 2 * _pictureMargin;
            }
        }
        private double picPanelWidth { get; set; }

        public Custom()
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
            PictureMargin = 10;
            preWidth = Window.Current.Bounds.Width - picsScrollViewer.Margin.Left - picsScrollViewer.Margin.Right;
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.Equals("Windows.Mobile"))
            {
                pictureColumn = 2;
                PicWidth = MinPictureWith = MaxPictureWith = preWidth / 2 - PictureMargin*2;
            }
            else
            {
                PicWidth = MinPictureWith = 200;
                MaxPictureWith = 250;             
            }            
        }
        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {           
            e.Handled = true;
            if (hidePanel.Visibility == Visibility.Visible)
            {
                hidePanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainPage._CurrentHandle.GoBack();
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        private void InitControl()
        {
            hidePanel.Height = Window.Current.Bounds.Height - MainPage._CurrentHandle._TopPenelHeight;
            hidePanel.Width = Window.Current.Bounds.Width;
            picsScrollViewer.Width = Window.Current.Bounds.Width - picsScrollViewer.Margin.Left - picsScrollViewer.Margin.Right;
            picsScrollViewer.Height = Window.Current.Bounds.Height - MainPage._CurrentHandle._TopPenelHeight;
            double currentWith = picsScrollViewer.Width;
            if(System.Math.Abs(currentWith - preWidth) > refreshStep)
            {
                preWidth = currentWith;
                ResetPicturePanelPositon();
            }
        }     

        private void ResetPicturePanelPositon()
        {
            pictureColumn = Convert.ToInt32(Math.Floor(preWidth / (MinPictureWith + 2*PictureMargin)));
            PicWidth = preWidth / pictureColumn - 2 * PictureMargin;
            PicWidth = PicWidth > MaxPictureWith ? MaxPictureWith : PicWidth;
            foreach(var item in picturePanels)
            {
                item.Value.ClearValue(RelativePanel.BelowProperty);
                item.Value.ClearValue(RelativePanel.AlignLeftWithPanelProperty);
                item.Value.ClearValue(RelativePanel.RightOfProperty);
                item.Value.ClearValue(RelativePanel.AlignTopWithPanelProperty);
                item.Value.ButtonWidth = PicWidth;
                //set left
                if(item.Key % pictureColumn == 0)
                {
                    item.Value.SetValue(RelativePanel.AlignLeftWithPanelProperty,true);
                }
                else
                {
                    item.Value.SetValue(RelativePanel.RightOfProperty,picturePanels[item.Key -1].Name);
                }
                //set top
                if(item.Key / pictureColumn == 0)
                {
                    item.Value.SetValue(RelativePanel.AlignTopWithPanelProperty,true);
                }
                else
                {
                    item.Value.SetValue(RelativePanel.BelowProperty,picturePanels[item.Key - pictureColumn].Name);
                }
            }
        }
       
        private void select_Click(object sender, RoutedEventArgs e)
        {                    
            if(!String.IsNullOrEmpty(selectedPicPath))
            {
                MainPage._CurrentHandle.Navigate(typeof(Store), selectedPicPath);
            }           
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {                  
            InitControl();                       
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {            
            return;
        }

        private async void Page_Loading(FrameworkElement sender, object args)
        {
            try
            {
                string root = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
                string path = root + @"\Images";

                // Get the folder object that corresponds to
                // this absolute path in the file system.
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path);


                //StorageFolder storageFolder = KnownFolders.PicturesLibrary;
                //IReadOnlyList<StorageFile> files = await storageFolder.GetFilesAsync();
                IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
                imageList.Clear();
                int count = files.Count;
                if (count == 0)
                {
                    return;
                }
                foreach (var file in files)
                {
                    imageList.Add(file.Path);
                }
                if (imageList.Count == 0)
                {
                    return;
                }
                int index = 0;
                foreach (var item in imageList)
                {
                    try
                    {
                        StorageFile file = await StorageFile.GetFileFromPathAsync(item);
                        using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                        {
                            BitmapImage image = new BitmapImage();
                            image.SetSource(fileStream);
                            ImageBrush brush = new ImageBrush();
                            brush.ImageSource = image;
                            PicturePanel panel = new PicturePanel();
                            panel._gridBackgroud = brush;
                            Button btn = new Button();
                            btn.Width = btn.Height = MinPictureWith;
                            btn.BorderThickness = new Thickness(0);
                            btn.Background = new SolidColorBrush(Color.FromArgb(0,255,255,255));
                            btn.Click += Btn_Click;
                            //btn.Name = item;
                            panel.Name = item;
                            panel.PicButton = btn;
                            if(index > 0 && picturePanels.ContainsKey(index-1))
                            {
                                panel.SetValue(RelativePanel.RightOfProperty, picturePanels[index-1].Name);
                            }
                            else
                            {
                                panel.SetValue(RelativePanel.AlignLeftWithPanelProperty, true);
                            }                            
                            panel.SetValue(RelativePanel.AlignTopWithPanelProperty,true);
                            picturePanels.Add(index++,panel);
                            picsPanel.Children.Add(panel);
                        }
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine(e.Message);
#endif
                        continue;
                    }
                }
                ResetPicturePanelPositon();
            }
            catch(Exception e)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(e.Message);
#endif
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (null == btn)
            {
                return;
            }
            var grid = VisualTreeHelper.GetParent(btn);
            if (null != grid)
            {
                var panel = VisualTreeHelper.GetParent(grid);
                if(null != panel && panel.ToString().Equals(typeof(PicturePanel).ToString()))
                {
                    ImageBrush brush =(ImageBrush)(panel as PicturePanel)._gridBackgroud;
                    selectImag.Background = brush;
                    selectedPicPath = (panel as PicturePanel).Name;
                    hidePanel.Visibility = Visibility.Visible;
                }
            }
        }

        private void hidePanel_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            Windows.UI.Input.PointerPoint ptr = e.GetCurrentPoint(sender as UIElement);
            Point pt = ptr.Position;
            if(pt.X>= (hidePanel.ActualWidth - operatePanel.ActualWidth)/2 && pt.X <= (hidePanel.ActualWidth - operatePanel.ActualWidth) / 2 + operatePanel.ActualWidth && pt.Y >= (hidePanel.ActualHeight - operatePanel.ActualHeight) / 2 && pt.Y <= (hidePanel.ActualHeight - operatePanel.ActualHeight) / 2 + operatePanel.ActualHeight)
            {
                return;
            }
            else
            {
                hidePanel.Visibility = Visibility.Collapsed;
                selectedPicPath = String.Empty;
            }            
        }
       
    }

    public class PicInfo
    {
        public ImageBrush brush;
        public string picPath = String.Empty;
        public double picWidth = 0;
        public PicInfo(string path)
        {
            picPath = path;
        }

        public async Task<ImageBrush> InitPic()
        {         
            try
            {               
                StorageFile file = await StorageFile.GetFileFromPathAsync(picPath);
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapImage image = new BitmapImage();
                    image.SetSource(fileStream);
                    brush = new ImageBrush();
                    brush.ImageSource = image;
                    return brush;
                }                  
            }
            catch(Exception e)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(e.Message);
#endif
                return null;
            }
           
        }
    }
}
