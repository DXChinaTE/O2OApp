using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Input.Inking;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PancakeCooker.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Ink : Page
    {
        const int penSize = 2;
        bool saved = false;
        string filePath = String.Empty;
        private const string IMAGE = "Windows.UI.Xaml.Controls.Image";
        private const string ELLIPSE = "Windows.UI.Xaml.Shapes.Ellipse";       
        private Button preClickedButton = null;
        private List<Button> linkageButtons = new List<Button>();
        public Ink()
        {
            this.InitializeComponent();
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
            InkDrawingAttributes drawingAttributes = new InkDrawingAttributes();
            drawingAttributes.Color = Color.FromArgb(255,243,156,17);
            drawingAttributes.Size = new Size(penSize, penSize);
            drawingAttributes.IgnorePressure = false;
            drawingAttributes.FitToCurve = true;

            pancakeInk.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
            pancakeInk.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse | Windows.UI.Core.CoreInputDeviceTypes.Pen | Windows.UI.Core.CoreInputDeviceTypes.Touch;           
            pancakeInk.InkPresenter.StrokesCollected += InkPresenter_StrokesCollected;
            pancakeInk.InkPresenter.StrokesErased += InkPresenter_StrokesErased;
            preClickedButton = pen;
            linkageButtons.Add(pen);
            linkageButtons.Add(penWP);
            linkageButtons.Add(eraser);
            linkageButtons.Add(eraserWP);       
        }

        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            }
        }

        private void InkPresenter_StrokesErased(InkPresenter sender, InkStrokesErasedEventArgs args)
        {
            saved = false;
        }

        private void InkPresenter_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {            
            saved = false;
        }

        private void CollapseColorButton(SolidColorBrush brush = null)
        {
            colorButtonsWp.Visibility = Visibility.Collapsed;
            colorBtnBorderEllipseWp.Visibility = Visibility.Collapsed;
            colorBtnBorderRectangleWp.Visibility = Visibility.Collapsed;
            colorButtons.Visibility = Visibility.Collapsed;
            colorBtnBorderEllipse.Visibility = Visibility.Collapsed;
            colorBtnBorderRectangle.Visibility = Visibility.Collapsed;
            if(null != brush)
            {
                colorBtnInnerEllipseWp.Fill = brush;
                colorBtnInnerEllipse.Fill = brush;
            }
        }
       
        private void ResetButtonBackgroudAndPic(Button currentClicked)
        {           
            if(preClickedButton.Equals(pen) || preClickedButton.Equals(penWP))
            {
                var parent = VisualTreeHelper.GetParent(pen);
                if (null == parent)
                    return;
                int count = VisualTreeHelper.GetChildrenCount(parent);
                double width = pen.Width;
                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    if (child != null && child.ToString().Equals(ELLIPSE) && ((Ellipse)child).Width == width)
                    {
                        ((Ellipse)child).Fill = new SolidColorBrush(Color.FromArgb(255, 224, 224, 224));
                    }
                    else if (child != null && child.ToString().Equals(IMAGE))
                    {
                        Image img = (Image)child;
                        BitmapImage bitmap = (BitmapImage)img.Source;
                        bitmap = new BitmapImage(new Uri("ms-appx:///Assets/pen.PNG"));
                        img.Source = bitmap;
                    }
                }
                if(!linkageButtons.Contains(currentClicked))
                {
                    parent = VisualTreeHelper.GetParent(wpPenBtn);
                    if (null == parent)
                        return;
                    count = VisualTreeHelper.GetChildrenCount(parent);
                    width = wpPenBtn.Width;
                    for (int i = 0; i < count; i++)
                    {
                        var child = VisualTreeHelper.GetChild(parent, i);
                        if (child != null && child.ToString().Equals(ELLIPSE) && ((Ellipse)child).Width == width)
                        {
                            ((Ellipse)child).Fill = new SolidColorBrush(Color.FromArgb(255, 224, 224, 224));
                        }
                        else if (child != null && child.ToString().Equals(IMAGE))
                        {
                            Image img = (Image)child;
                            BitmapImage bitmap = (BitmapImage)img.Source;
                            bitmap = new BitmapImage(new Uri("ms-appx:///Assets/pen.PNG"));
                            img.Source = bitmap;
                        }
                    }
                }                
            }
            else if(preClickedButton.Equals(eraser) || preClickedButton.Equals(eraserWP))
            {
                var parent = VisualTreeHelper.GetParent(eraser);
                if (null == parent)
                    return;
                int count = VisualTreeHelper.GetChildrenCount(parent);
                double width = eraser.Width;
                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    if (child != null && child.ToString().Equals(ELLIPSE) && ((Ellipse)child).Width == width)
                    {
                        ((Ellipse)child).Fill = new SolidColorBrush(Color.FromArgb(255, 224, 224, 224));
                    }
                    else if (child != null && child.ToString().Equals(IMAGE))
                    {
                        Image img = (Image)child;
                        BitmapImage bitmap = (BitmapImage)img.Source;
                        bitmap = new BitmapImage(new Uri("ms-appx:///Assets/eraser.PNG"));
                        img.Source = bitmap;
                    }
                }
                if(!linkageButtons.Contains(currentClicked))
                {
                    parent = VisualTreeHelper.GetParent(wpPenBtn);
                    if (null == parent)
                        return;
                    count = VisualTreeHelper.GetChildrenCount(parent);
                    width = wpPenBtn.Width;
                    for (int i = 0; i < count; i++)
                    {
                        var child = VisualTreeHelper.GetChild(parent, i);
                        if (child != null && child.ToString().Equals(ELLIPSE) && ((Ellipse)child).Width == width)
                        {
                            ((Ellipse)child).Fill = new SolidColorBrush(Color.FromArgb(255, 224, 224, 224));
                        }
                        else if (child != null && child.ToString().Equals(IMAGE))
                        {
                            Image img = (Image)child;
                            BitmapImage bitmap = (BitmapImage)img.Source;
                            bitmap = new BitmapImage(new Uri("ms-appx:///Assets/eraser.PNG"));
                            img.Source = bitmap;
                        }
                    }
                }               
            }
            else if(preClickedButton.Equals(colorBtn) || preClickedButton.Equals(colorBtnWp))
            {
                var parent = VisualTreeHelper.GetParent(colorBtn);
                if (null == parent)
                    return;
                int count = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    if (child != null && child.ToString().Equals(ELLIPSE) && ((Ellipse)child).Name.Equals("colorBtnOuterEllipse"))
                    {
                        ((Ellipse)child).Fill = new SolidColorBrush(Color.FromArgb(255, 224, 224, 224));
                        break;
                    }
                }
                parent = VisualTreeHelper.GetParent(colorBtnWp);
                if (null == parent)
                    return;
                count = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    if (child != null && child.ToString().Equals(ELLIPSE) && ((Ellipse)child).Name.Equals("colorBtnOuterEllipseWp"))
                    {
                        ((Ellipse)child).Fill = new SolidColorBrush(Color.FromArgb(255, 224, 224, 224));
                        break;
                    }
                }
            }
            else
            {
                var parent = VisualTreeHelper.GetParent(preClickedButton);
                if (null == parent)
                    return;
                int count = VisualTreeHelper.GetChildrenCount(parent);
                double width = preClickedButton.Width;
                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    if (child != null && child.ToString().Equals(ELLIPSE) && ((Ellipse)child).Width == width)
                    {
                        ((Ellipse)child).Fill = new SolidColorBrush(Color.FromArgb(255, 224, 224, 224));
                        break;
                    }
                }
            }
        }
        private void ResetButtonState(Button btn)
        {
            if(null == btn || preClickedButton == null || preClickedButton.Equals(btn))
            {
                return;
            }            
            ResetButtonBackgroudAndPic(btn);
            preClickedButton = btn;
            if (!btn.Equals(colorBtn) && !btn.Equals(colorBtnWp))
            {
                CollapseColorButton();
            }
        }     

        public async void SetInkData(string path)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(path);
            using (var stream = await file.OpenSequentialReadAsync())
            {
                try
                {
                    await pancakeInk.InkPresenter.StrokeContainer.LoadAsync(stream);
                }
                catch(Exception e)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(e.Message);
#endif
                }
            }
        }

        private void shoppingCart_Click(object sender, RoutedEventArgs e)
        {                      
            if (!saved)
            {
                return;
            }
            Button btn = sender as Button;
            if (null == btn)
            {
                return;
            }
            ResetButtonState(btn);
            MainPage._CurrentHandle.Navigate(typeof(Store),filePath);                    
        }

        private async void save_Click(object sender, RoutedEventArgs e)
        {
            if (pancakeInk.InkPresenter.StrokeContainer.GetStrokes().Count > 0)
            {
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                savePicker.FileTypeChoices.Add("Gif with embedded ISF", new System.Collections.Generic.List<string> { ".gif" });

                Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
                if (null != file)
                {
                    try
                    {
                        using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            await pancakeInk.InkPresenter.StrokeContainer.SaveAsync(stream);
                        }
                        saved = true;
                        filePath = file.Path;
                        Button btn = sender as Button;
                        if (null == btn)
                        {
                            return;
                        }
                        var parent = VisualTreeHelper.GetParent(btn);
                        if (null != parent)
                        {
                            int count = VisualTreeHelper.GetChildrenCount(parent);
                            for (int i = 0; i < count; i++)
                            {
                                var child = VisualTreeHelper.GetChild(parent, i);
                                if (child != null && child.ToString().Equals(ELLIPSE) && ((Ellipse)child).Width == btn.Width)
                                {
                                    ((Ellipse)child).Fill = new SolidColorBrush(Color.FromArgb(255, 88, 91, 144));
                                    break;
                                }
                            }
                        }
                        ResetButtonState(btn);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
                        saved = false;
                    }
                }
            }
            else
            {
            }
        }

        private void addPicBtn_Click(object sender, RoutedEventArgs e)
        {
            MainPage._CurrentHandle.Navigate(typeof(PicStroe));         
        }

        protected  override void OnNavigatedTo(NavigationEventArgs e)
        {
            object obj = e.Parameter;
            if(null != obj)
            {
                string path = obj as string;
                if(!path.Equals(String.Empty))
                {
                    saved = true;
                    filePath = path;
                    SetInkData(path);
                }
                
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            object obj = e.Parameter;
        }

        private void eraser_Click(object sender, RoutedEventArgs e)
        {
            pancakeInk.InkPresenter.InputProcessingConfiguration.Mode = InkInputProcessingMode.Erasing;
            Button btn = sender as Button;
            if (null != btn && btn.Equals(eraserWP))
            {
                wppenPanel.Visibility = Visibility.Collapsed;
                //btn = wpPenBtn;                
            }
            var parent = VisualTreeHelper.GetParent(eraser);
            if (null == parent)
                return;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            double width = eraser.Width;
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child.ToString().Equals(ELLIPSE) && ((Ellipse)child).Width == width)
                {
                    ((Ellipse)child).Fill = new SolidColorBrush(Color.FromArgb(255, 88, 91, 144));
                }
                else if (child != null && child.ToString().Equals(IMAGE))
                {
                    Image img = (Image)child;
                    BitmapImage bitmap = (BitmapImage)img.Source;
                    bitmap = new BitmapImage(new Uri("ms-appx:///Assets/eraser-clicked.PNG"));
                    img.Source = bitmap;
                }
            }
            parent = VisualTreeHelper.GetParent(wpPenBtn);
            if (null == parent)
                return;
            count = VisualTreeHelper.GetChildrenCount(parent);
            width = wpPenBtn.Width;
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child.ToString().Equals(ELLIPSE) && ((Ellipse)child).Width == width)
                {
                    ((Ellipse)child).Fill = new SolidColorBrush(Color.FromArgb(255, 88, 91, 144));
                }
                else if (child != null && child.ToString().Equals(IMAGE))
                {
                    Image img = (Image)child;
                    BitmapImage bitmap = (BitmapImage)img.Source;
                    bitmap = new BitmapImage(new Uri("ms-appx:///Assets/eraser-clicked.PNG"));
                    img.Source = bitmap;
                }
            }           
            ResetButtonState(btn);
        }

        private void pen_Click(object sender, RoutedEventArgs e)
        {
            pancakeInk.InkPresenter.InputProcessingConfiguration.Mode = InkInputProcessingMode.Inking;
            Button btn = sender as Button;
            if(null != btn && btn.Equals(penWP))
            {
                wppenPanel.Visibility = Visibility.Collapsed;
               // btn = wpPenBtn;               
            }
            var parent = VisualTreeHelper.GetParent(pen);
            if (null == parent)
                return;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            double width = pen.Width;
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child.ToString().Equals(ELLIPSE) && ((Ellipse)child).Width == width)
                {
                    ((Ellipse)child).Fill = new SolidColorBrush(Color.FromArgb(255, 88, 91, 144));
                }
                else if (child != null && child.ToString().Equals(IMAGE))
                {
                    Image img = (Image)child;
                    BitmapImage bitmap = (BitmapImage)img.Source;
                    bitmap = new BitmapImage(new Uri("ms-appx:///Assets/pen-clicked.PNG"));
                    img.Source = bitmap;
                }
            }
            parent = VisualTreeHelper.GetParent(wpPenBtn);
            if (null == parent)
                return;
            count = VisualTreeHelper.GetChildrenCount(parent);
            width = wpPenBtn.Width;
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child.ToString().Equals(ELLIPSE) && ((Ellipse)child).Width == width)
                {
                    ((Ellipse)child).Fill = new SolidColorBrush(Color.FromArgb(255, 88, 91, 144));
                }
                else if (child != null && child.ToString().Equals(IMAGE))
                {
                    Image img = (Image)child;
                    BitmapImage bitmap = (BitmapImage)img.Source;
                    bitmap = new BitmapImage(new Uri("ms-appx:///Assets/pen-clicked.PNG"));
                    img.Source = bitmap;
                }
            }           
            ResetButtonState(btn);
        }

        private void color_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;  
            if(colorButtonsWp.Visibility == Visibility.Visible || colorButtons.Visibility == Visibility.Visible)
            {
                return;
            }        
            if(null != btn && btn.Name.Equals("colorBtnWp"))
            {              
                colorButtonsWp.Visibility = Visibility.Visible;         
                colorBtnBorderEllipseWp.Visibility = Visibility.Visible;
                colorBtnBorderRectangleWp.Visibility = Visibility.Visible;              
            }
            else
            {              
                colorButtons.Visibility = Visibility.Visible;
                colorBtnBorderEllipse.Visibility = Visibility.Visible;
                colorBtnBorderRectangle.Visibility = Visibility.Visible;
            }
            var parent = VisualTreeHelper.GetParent(colorBtn);
            if (null != parent)
            {
                int count = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);                    
                    if (child != null && child.ToString().Equals(ELLIPSE) && ((Ellipse)child).Width == colorBtn.Width)
                    {
                        ((Ellipse)child).Fill = new SolidColorBrush(Color.FromArgb(255, 88, 91, 144));
                    }
                }
            }
            parent = VisualTreeHelper.GetParent(colorBtnWp);
            if (null != parent)
            {
                int count = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    if (child != null && child.ToString().Equals(ELLIPSE) && ((Ellipse)child).Width == colorBtnWp.Width)
                    {
                        ((Ellipse)child).Fill = new SolidColorBrush(Color.FromArgb(255, 88, 91, 144));
                    }
                }
            }
            ResetButtonState(btn);
        }

        private void blue_Click(object sender, RoutedEventArgs e)
        {
            if (pancakeInk != null)
            {
                InkDrawingAttributes drawingAttributes = pancakeInk.InkPresenter.CopyDefaultDrawingAttributes();
                drawingAttributes.Color = Color.FromArgb(255,49,90,190);
                pancakeInk.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
            }           
            CollapseColorButton(new SolidColorBrush(Color.FromArgb(255, 49, 90, 190)));
            SetToInkMode();
        }

        private void yellow_Click(object sender, RoutedEventArgs e)
        {
            if (pancakeInk != null)
            {
                InkDrawingAttributes drawingAttributes = pancakeInk.InkPresenter.CopyDefaultDrawingAttributes();
                drawingAttributes.Color = Color.FromArgb(255,254,97,0);
                pancakeInk.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
            }          
            CollapseColorButton(new SolidColorBrush(Color.FromArgb(255, 254, 97, 0)));
            SetToInkMode();
        }

        private void green_Click(object sender, RoutedEventArgs e)
        {
            if (pancakeInk != null)
            {
                InkDrawingAttributes drawingAttributes = pancakeInk.InkPresenter.CopyDefaultDrawingAttributes();

                drawingAttributes.Color = Color.FromArgb(255,36,153,91);
                pancakeInk.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
            }
            CollapseColorButton(new SolidColorBrush(Color.FromArgb(255, 36, 153, 91)));
            SetToInkMode();
        }

        private void black_Click(object sender, RoutedEventArgs e)
        {
            if (pancakeInk != null)
            {
                InkDrawingAttributes drawingAttributes = pancakeInk.InkPresenter.CopyDefaultDrawingAttributes();

                drawingAttributes.Color = Colors.Black;
                pancakeInk.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
            }         
            CollapseColorButton(new SolidColorBrush(Colors.Black));
            SetToInkMode();
        }

        private void red_Click(object sender, RoutedEventArgs e)
        {
            if (pancakeInk != null)
            {
                InkDrawingAttributes drawingAttributes = pancakeInk.InkPresenter.CopyDefaultDrawingAttributes();

                drawingAttributes.Color = Color.FromArgb(255,233,43,42);
                pancakeInk.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
            }           
            CollapseColorButton(new SolidColorBrush(Color.FromArgb(255, 233, 43, 42)));
            SetToInkMode();
        }

        private void orange_Click(object sender, RoutedEventArgs e)
        {
            if (pancakeInk != null)
            {
                InkDrawingAttributes drawingAttributes = pancakeInk.InkPresenter.CopyDefaultDrawingAttributes();

                drawingAttributes.Color = Color.FromArgb(255,243,156,17);
                pancakeInk.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
            }         
            CollapseColorButton(new SolidColorBrush(Color.FromArgb(255, 243, 156, 17)));
            SetToInkMode();
        }

        private void white_Click(object sender, RoutedEventArgs e)
        {
            if (pancakeInk != null)
            {
                InkDrawingAttributes drawingAttributes = pancakeInk.InkPresenter.CopyDefaultDrawingAttributes();

                drawingAttributes.Color = Color.FromArgb(255, 255, 255, 255);
                pancakeInk.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
            }          
            CollapseColorButton(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)));
            SetToInkMode();
        }

        private void violet_Click(object sender, RoutedEventArgs e)
        {
            if (pancakeInk != null)
            {
                InkDrawingAttributes drawingAttributes = pancakeInk.InkPresenter.CopyDefaultDrawingAttributes();

                drawingAttributes.Color = Color.FromArgb(255, 162, 25, 194);
                pancakeInk.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
            }         
            CollapseColorButton(new SolidColorBrush(Color.FromArgb(255, 162, 25, 194)));
            SetToInkMode();
        }
       
        private void SetToInkMode()
        {
            preClickedButton = colorBtn;
            pancakeInk.InkPresenter.InputProcessingConfiguration.Mode = InkInputProcessingMode.Inking;
            pen_Click(pen,null);
        }
        private void wpPenBtn_Click(object sender, RoutedEventArgs e)
        {
            if(wppenPanel.Visibility == Visibility.Visible)
            {
                wppenPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                wppenPanel.Visibility = Visibility.Visible;
            }
            CollapseColorButton();
            preClickedButton = colorBtnWp;
            ResetButtonState(pen);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            workPanel.Width = pancakeInk.Width = Window.Current.Bounds.Width;
            workPanel.Height = Window.Current.Bounds.Height - MainPage._CurrentHandle._TopPenelHeight;
            pancakeInk.Height = workPanel.Height - tabletBrush.ActualHeight - tabletBrush.Margin.Bottom;          
        }       
    }
}
