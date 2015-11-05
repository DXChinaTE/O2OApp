/*
picture library
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
using Windows.Storage.Pickers;
using Windows.Devices.Input;
using PancakeCooker.Common;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PancakeCooker.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PicStroe : Page
    {
     
        private Dictionary<ImageBrush, string> selectImagePaths = new Dictionary<ImageBrush, string>();
        private ImageBrush selectedImageBrush = null;
        private bool add = true;
        public PicStroe()
        {            
            this.InitializeComponent();           
            PicsCVS.Source = new ObservableCollection<PicInfo>();
            InsertButton();
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }

        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            if(picDetail.Visibility == Visibility.Visible)
            {
                picDetail.Visibility = Visibility.Collapsed;
                picList.Visibility = Visibility.Visible;
            }
            else
            {
                MainPage._CurrentHandle.GoBack(true);
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

        /// <summary>
        /// add pic button
        /// </summary>
        private void InsertButton()
        {        
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/pic.png"));
            AddPics.Background = ib;
        }

       
        /// <summary>
        /// add pic button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnPic_Click(object sender, RoutedEventArgs e)
        {
            selectImagePaths.Clear(); 
            FileOpenPicker picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".gif");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");

            IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync();
            if (files != null)
            {
                int count = files.Count;
                if(count > 0)
                {
                    AddPics.Visibility = Visibility.Collapsed;
                    picGrid.Visibility = Visibility.Visible;
                    ObservableCollection<PicInfo> piclist = (ObservableCollection<PicInfo>)PicsCVS.Source;
                    piclist.Clear();
                    foreach (var file in files)
                    {                                               
                        try
                        {
                            PicInfo info = new PicInfo(file.Path);
                            ImageBrush brush = await info.InitPic();
                            if (brush != null)
                            {
                                piclist.Add(info);
                                selectImagePaths.Add(info.brush, info.picPath);
                            }
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
                            continue;
                        }                  
                   }                
                }
            }
        }

        /// <summary>
        /// pic clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Click(object sender, RoutedEventArgs e)
        {           
            Button btn = sender as Button;
            ImageBrush brush = null;
            if(null != btn)
            {
                var parent = VisualTreeHelper.GetParent(btn);
                if(null != parent)
                {
                    int count = VisualTreeHelper.GetChildrenCount(parent);
                    for(int i=0; i< count; i++)
                    {
                        var child = VisualTreeHelper.GetChild(parent,i);
                        if(null != child && child.ToString().Equals("Windows.UI.Xaml.Controls.Border"))
                        {
                            brush = (ImageBrush)((Border)child).Background;
                        }
                    }
                }
            }
            if(null != brush)
            {
                imgDetail.Source = brush.ImageSource;
                selectedImageBrush = brush;
                picList.Visibility = Visibility.Collapsed;
                picDetail.Visibility = Visibility.Visible;
                right.Visibility = Visibility.Visible;
                double width = picDetail.Width > picDetail.Height ? picDetail.Height - 10 : picDetail.Width - 10;
                picDetailGrid.Width = picDetailGrid.Height = width / 5;
            }
        }
     
        private void select_Click(object sender, RoutedEventArgs e)
        {           
            foreach(var item in selectImagePaths)
            {
                if(item.Key.Equals(selectedImageBrush))
                {
                    MainPage._CurrentHandle.Navigate(typeof(Ink), item.Value);
                }
            }           
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {         
            picDetail.Visibility = Visibility.Collapsed;
            picList.Visibility = Visibility.Visible;
            right.Visibility = Visibility.Collapsed;
            selectedImageBrush = null;
        }

        private void zoom_Click(object sender, RoutedEventArgs e)
        {
            double width = picDetail.Width > picDetail.Height ? picDetail.Height - 10 : picDetail.Width - 10;
            if(add)
            {
                if (picDetailGrid.Width < width*4/5)
                {
                    picDetailGrid.Width += width / 5;
                    picDetailGrid.Height += width / 5;
                }
                else
                {
                    add = false;
                    picDetailGrid.Width -= width / 5;
                    picDetailGrid.Height -= width / 5;
                }
            } 
            else
            {
                if (picDetailGrid.Width > width*2 / 5)
                {
                    picDetailGrid.Width -= width / 5;
                    picDetailGrid.Height -= width / 5;
                }
                else
                {
                    add = true;
                    picDetailGrid.Width += width / 5;
                    picDetailGrid.Height += width / 5;
                }
            }          
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            picDetail.Width = picList.Width = topPanel.Width = Window.Current.Bounds.Width;
            picDetail.Height = picList.Height = Window.Current.Bounds.Height - topPanel.Height;
            
        }

        private void returnBack_Click(object sender, RoutedEventArgs e)
        {
            MainPage._CurrentHandle.Navigate(typeof(Ink));
        }
       
    }
}
