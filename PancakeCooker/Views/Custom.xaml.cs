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
using PancakeCooker.Views;
using PancakeCooker.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PancakeCooker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Custom : Page
    {     
        private Dictionary<ImageBrush, string> picPaths = new Dictionary<ImageBrush, string>();
        private List<string> imageList = new List<string>();
        private const string BORDER = "Windows.UI.Xaml.Controls.Border";
        private double _picWidth = 200;
        public Custom()
        {
            
            this.InitializeComponent();
            ContactsCVS.Source = new ObservableCollection<PicInfo>();
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.Equals("Windows.Mobile"))
            {                
                _picWidth = (Window.Current.Bounds.Width - 48) / 2;
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
            picGrid.Width = Window.Current.Bounds.Width;
        }     

        private void Btn_Click(object sender, RoutedEventArgs e)
        {  
            Button btn = sender as Button;
            if(null == btn)
            {
                return;
            }
            var parent = VisualTreeHelper.GetParent(btn);
            if(null != parent)
            {
                int count = VisualTreeHelper.GetChildrenCount(parent);
                for(int i=0; i< count; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent,i);
                    if(null != child && child.ToString().Equals(BORDER))
                    {
                        ImageBrush brush = (ImageBrush)((Border)child).Background;
                        //selectImag.Source = brush.ImageSource;
                        selectImag.Background = brush;
                        hidePanel.Visibility = Visibility.Visible;
                        break;
                    }
                }
            }
        }

        private void select_Click(object sender, RoutedEventArgs e)
        {         
            string path = String.Empty;
            foreach(var item in picPaths)
            {
                if (item.Key.Equals(selectImag.Background))
                {
                    path = item.Value;
                }
            }
            if(!String.IsNullOrEmpty(path))
            {
                MainPage._CurrentHandle.Navigate(typeof(Store), path);
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
            picPaths.Clear();
            string root  = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
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
            ObservableCollection<PicInfo> piclist = (ObservableCollection<PicInfo>)ContactsCVS.Source;
            foreach (var item in imageList)
            {
                try
                {
                    PicInfo info = new PicInfo(item);
                    info.picWidth = _picWidth;
                    ImageBrush brush = await info.InitPic();
                    if (brush != null)
                    {
                        piclist.Add(info);
                        picPaths.Add(info.brush, info.picPath);
                    }
                }
                catch(Exception e)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(e.Message);
#endif
                    continue;
                }                             
            }
            return;
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
