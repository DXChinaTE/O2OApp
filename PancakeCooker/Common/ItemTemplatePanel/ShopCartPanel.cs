using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls;

namespace PancakeCooker.Common.ItemTemplatePanel
{
    public class ShopCartPanel :RelativePanel
    {
        private Grid picGrid = new Grid();
        private Grid goodsGrid = new Grid();
        private Border picBorder = new Border();
        private Border goodsBorder = new Border();
        private RelativePanel goodsInfoPanel = new RelativePanel();
        //price and count info part
        private TextBlock priceSymbolText = new TextBlock();
        private TextBlock priceText = new TextBlock();
        private TextBlock goodsCountWithSymbolText = new TextBlock();
        //count add and decrease part
        private SymbolIcon symbolAdd = new SymbolIcon();
        private SymbolIcon symbolDecrease = new SymbolIcon();
        private Rectangle rectangleAdd = new Rectangle();
        private Rectangle rectangleDecrease = new Rectangle();
        private Rectangle rectangleNumber = new Rectangle();
        private TextBlock goodsCountText = new TextBlock();
        private Button btnAdd = null;
        public Button ButtonAdd
        {
            set
            {
                if(null == btnAdd)
                {
                    btnAdd = value;
                    btnAdd.Width = rectangleAdd.Width;
                    btnAdd.Height = rectangleAdd.Height;
                    btnAdd.BorderThickness = new Thickness(0);
                    btnAdd.Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
                    btnAdd.SetValue(RelativePanel.AlignBottomWithPanelProperty,true);
                    btnAdd.SetValue(RelativePanel.RightOfProperty, rectangleNumber.Name);
                    btnAdd.Margin = rectangleAdd.Margin;
                    goodsInfoPanel.Children.Add(btnAdd);
                }
            }
        }
        private double _pictureWidth = 0;
        public double PictureWith
        {
            set
            {
                _pictureWidth = value;
                picGrid.Width = picGrid.Height = _pictureWidth;
            }
            get
            {
                return _pictureWidth;
            }
        }
        private Button btnDecrease = null;
        public Button ButtonDecrease
        {
            set
            {
                if(null == btnDecrease)
                {
                    btnDecrease = value;
                    btnDecrease.Width = rectangleDecrease.Width;
                    btnDecrease.Height = rectangleDecrease.Height;
                    btnDecrease.BorderThickness = new Thickness(0);
                    btnDecrease.Background = new SolidColorBrush(Color.FromArgb(0,255,255,255));
                    btnDecrease.SetValue(RelativePanel.AlignLeftWithPanelProperty,true);
                    btnDecrease.SetValue(RelativePanel.AlignBottomWithPanelProperty,true);
                    btnDecrease.Margin = rectangleDecrease.Margin;
                    goodsInfoPanel.Children.Add(btnDecrease);
                }
            }
        }
        private string pictureName;
        public string PictureName
        {
            get
            {
                return pictureName;
            }
            set
            {
                pictureName = value;
                pictureNameText.Text = pictureName;
            }
        }
        private string goodsName;
        public string GoodsName
        {
            set
            {
                goodsName = value;
                goodsNameText.Text = goodsName;
            }
            get
            {
                return goodsName;
            }
        }
        private int goodsCount;
        public int GoodsCount
        {
            set
            {
                goodsCount = value;
                goodsCountWithSymbolText.Text = "x" + Convert.ToString(goodsCount);
                goodsCountText.Text = Convert.ToString(goodsCount);
            }
            get
            {
                return goodsCount;
            }
        }
        public Brush Picture
        {
            set
            {
                picBorder.Background = value;
            }
            get
            {
                return picBorder.Background;
            }
        }
        private string price;
        public string Price
        {
            set
            {
                price = value;
                priceText.Text = price;
            }
            get
            {
                return price;
            }
        }
        private CheckBox check = null;
        public CheckBox Check
        {
            set
            {
                if(null == check)
                {
                    check = value;
                    check.SetValue(RelativePanel.AlignLeftWithPanelProperty,true);
                    check.SetValue(RelativePanel.AboveProperty,picGrid.Name);
                    check.Margin = new Thickness(10,0,0,0);
                    goodsInfoPanel.Children.Add(check);
                }
            }
            get
            {
                return check;
            }
        }
        private string pictureURL;
        public string PictureURL
        {
            get
            {
                return pictureURL;
            }
            set
            {
                pictureURL = value;
            }
        }
        //goods name part
        private TextBlock goodsNameInfoText = new TextBlock();
        private TextBlock goodsNameText = new TextBlock();
        //picture name part
        private TextBlock pictureNameText = new TextBlock();
        public double GetItemTotalPrice()
        {

            return Convert.ToDouble(price)*goodsCount;
        }
        public double AddClick()
        {
            GoodsCount++;
            return Convert.ToDouble(price);
        }
        public double DecreaseClick()
        {
            if(GoodsCount > 1)
            {
                GoodsCount--;
                return Convert.ToDouble(price);
            }
            return 0;
        }
        public ShopCartPanel()
        {
            //init price and count info part                    
            goodsCountWithSymbolText.Name = "goodsCountWithSymbolText";
            goodsCountWithSymbolText.Width = 20;
            goodsCountWithSymbolText.Height = 20;
            goodsCountWithSymbolText.FontSize = 13;
            goodsCountWithSymbolText.TextAlignment = TextAlignment.Right;
            goodsCountWithSymbolText.Foreground = new SolidColorBrush(Color.FromArgb(255,51,51,51));
            goodsCountWithSymbolText.SetValue(RelativePanel.AlignRightWithPanelProperty,true);
            goodsCountWithSymbolText.SetValue(RelativePanel.AlignBottomWithPanelProperty,true);
            goodsCountWithSymbolText.Margin = new Thickness(0,0,10,10);
            goodsInfoPanel.Children.Add(goodsCountWithSymbolText);
            priceText.Width = 40;
            priceText.Height = 25;
            priceText.FontSize = 20;
            priceText.TextAlignment = TextAlignment.Left;
            priceText.Foreground = new SolidColorBrush(Color.FromArgb(255, 253, 40, 57));
            priceText.Name = "priceText";
            priceText.SetValue(RelativePanel.AlignBottomWithPanelProperty,true);
            priceText.SetValue(RelativePanel.LeftOfProperty,goodsCountWithSymbolText.Name);
            priceText.Margin = new Thickness(0,0,0,10);
            goodsInfoPanel.Children.Add(priceText);
            priceSymbolText.Text = "￥";
            priceSymbolText.TextAlignment = TextAlignment.Right;
            priceSymbolText.Foreground = new SolidColorBrush(Color.FromArgb(255, 253, 40, 57));
            priceSymbolText.Width = 15;
            priceSymbolText.Height = 20;
            priceSymbolText.FontSize = 14;
            priceSymbolText.SetValue(RelativePanel.AlignBottomWithPanelProperty,true);
            priceSymbolText.SetValue(RelativePanel.LeftOfProperty,priceText.Name);
            priceSymbolText.Margin = new Thickness(0,0,0,8);
            goodsInfoPanel.Children.Add(priceSymbolText);

            //init count add and decrease part
            rectangleDecrease.Width = rectangleDecrease.Height = 22;
            rectangleDecrease.StrokeThickness = 1;
            rectangleDecrease.Stroke = new SolidColorBrush(Colors.Gray);
            rectangleDecrease.Name = "rectangleDecrease";
            rectangleDecrease.SetValue(RelativePanel.AlignLeftWithPanelProperty,true);
            rectangleDecrease.SetValue(RelativePanel.AlignBottomWithPanelProperty,true);
            rectangleDecrease.Margin = new Thickness(10,0,0,10);
            goodsInfoPanel.Children.Add(rectangleDecrease);
            rectangleNumber.Width = 40;
            rectangleNumber.Height = 22;
            rectangleNumber.StrokeThickness = 1;
            rectangleNumber.Stroke = new SolidColorBrush(Colors.Gray);
            rectangleNumber.Name = "rectangleNumber";
            rectangleNumber.SetValue(RelativePanel.AlignBottomWithPanelProperty,true);
            rectangleNumber.SetValue(RelativePanel.RightOfProperty,rectangleDecrease.Name);
            rectangleNumber.Margin = new Thickness(-1,0,0,10);
            goodsInfoPanel.Children.Add(rectangleNumber);
            goodsCountText.Width = 40;
            goodsCountText.Height = 22;
            goodsCountText.TextAlignment = TextAlignment.Center;
            goodsCountText.Foreground = new SolidColorBrush(Color.FromArgb(255, 51, 51, 51));
            goodsCountText.SetValue(RelativePanel.AlignBottomWithPanelProperty, true);
            goodsCountText.SetValue(RelativePanel.RightOfProperty, rectangleDecrease.Name);
            goodsCountText.Margin = new Thickness(-1, 0, 0, 10);
            goodsInfoPanel.Children.Add(goodsCountText);

            rectangleAdd.Width = rectangleAdd.Height = 22;
            rectangleAdd.StrokeThickness = 1;
            rectangleAdd.Stroke = new SolidColorBrush(Colors.Gray);
            rectangleAdd.Name = "rectangleAdd";
            rectangleAdd.SetValue(RelativePanel.AlignBottomWithPanelProperty,true);
            rectangleAdd.SetValue(RelativePanel.RightOfProperty,rectangleNumber.Name);
            rectangleAdd.Margin = new Thickness(-1,0,0,10);
            goodsInfoPanel.Children.Add(rectangleAdd);

            //add and decrease symbol
            symbolAdd.Width = symbolAdd.Height = 22;
            symbolAdd.Symbol = Symbol.Add;
            symbolAdd.SetValue(RelativePanel.AlignBottomWithPanelProperty, true);
            symbolAdd.SetValue(RelativePanel.RightOfProperty, rectangleNumber.Name);
            symbolAdd.RenderTransformOrigin = new Windows.Foundation.Point(0.5,0.5);
            CompositeTransform transform = new CompositeTransform();
            transform.ScaleX = 0.6;
            transform.ScaleY = 0.6;
            symbolAdd.SetValue(SymbolIcon.RenderTransformProperty, transform);
            symbolAdd.Margin = new Thickness(-1, 0, 0, 10);
            goodsInfoPanel.Children.Add(symbolAdd);

            symbolDecrease.Width = symbolAdd.Height = 22;
            symbolDecrease.Symbol = Symbol.Remove;
            symbolDecrease.SetValue(RelativePanel.AlignLeftWithPanelProperty, true);
            symbolDecrease.SetValue(RelativePanel.AlignBottomWithPanelProperty, true);
            symbolDecrease.Margin = new Thickness(10, 0, 0, 10);
            symbolDecrease.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
            CompositeTransform transform1 = new CompositeTransform();
            transform1.ScaleX = 0.6;
            transform1.ScaleY = 0.6;
            symbolDecrease.SetValue(SymbolIcon.RenderTransformProperty, transform1);
            goodsInfoPanel.Children.Add(symbolDecrease);
            //goods name part
            goodsNameInfoText.Width = 80;
            goodsNameInfoText.Height = 25;
            goodsNameInfoText.TextAlignment = TextAlignment.Left;
            goodsNameInfoText.Text = "商品信息";
            goodsNameInfoText.Foreground = new SolidColorBrush(Color.FromArgb(255, 51, 51, 51));
            goodsNameInfoText.Name = "goodsNameInfoText";
            goodsNameInfoText.SetValue(RelativePanel.AlignLeftWithPanelProperty,true);
            goodsNameInfoText.SetValue(RelativePanel.AboveProperty,priceText.Name);
            goodsNameInfoText.Margin = new Thickness(10,0,0,10);
            goodsInfoPanel.Children.Add(goodsNameInfoText);
            goodsNameText.Width = 80;
            goodsNameText.Height = 25;
            goodsNameText.TextAlignment = TextAlignment.Right;
            goodsNameText.Foreground = new SolidColorBrush(Color.FromArgb(255, 51, 51, 51));
            goodsNameText.SetValue(RelativePanel.AlignRightWithPanelProperty,true);
            goodsNameText.SetValue(RelativePanel.AboveProperty,priceText.Name);
            goodsNameText.Margin = new Thickness(0,0,10,10);
            goodsInfoPanel.Children.Add(goodsNameText);

            //picture name part
            pictureNameText.Width = 180;
            pictureNameText.Height = 30;
            pictureNameText.FontSize = 16;
            pictureNameText.TextAlignment = TextAlignment.Left;
            pictureNameText.Foreground = new SolidColorBrush(Color.FromArgb(255, 51, 51, 51));
            pictureNameText.Name = "pictureNameText";
            pictureNameText.SetValue(RelativePanel.AlignLeftWithPanelProperty,true);
            pictureNameText.SetValue(RelativePanel.AboveProperty,goodsNameInfoText.Name);
            pictureNameText.Margin = new Thickness(10,0,0,10);
            goodsInfoPanel.Children.Add(pictureNameText);

            RowDefinition picRow = new RowDefinition();
            ColumnDefinition picCol = new ColumnDefinition();
            picGrid.RowDefinitions.Add(picRow);
            picGrid.ColumnDefinitions.Add(picCol);
            picBorder.BorderBrush = new SolidColorBrush(Colors.White);
            picBorder.BorderThickness = new Thickness(1);
            picBorder.CornerRadius = new CornerRadius(5,5,5,5);
            picGrid.Children.Add(picBorder);
            picGrid.Name = "picGrid";
            picGrid.SetValue(RelativePanel.AlignLeftWithPanelProperty,true);
            picGrid.SetValue(RelativePanel.AboveProperty,pictureNameText.Name);
            picGrid.Margin = new Thickness(10,0,10,10);
            goodsInfoPanel.Children.Add(picGrid);

            RowDefinition goodsRow = new RowDefinition();
            ColumnDefinition goodsCol = new ColumnDefinition();
            goodsGrid.RowDefinitions.Add(goodsRow);
            goodsGrid.ColumnDefinitions.Add(goodsCol);
            goodsBorder.BorderBrush = new SolidColorBrush(Colors.White);
            goodsBorder.BorderThickness = new Thickness(1);
            goodsBorder.CornerRadius = new CornerRadius(5, 5, 5, 5);
            goodsGrid.Children.Add(goodsBorder);
            goodsGrid.Children.Add(goodsInfoPanel);
            goodsGrid.Margin = new Thickness(10,10,10,10);
            goodsGrid.Background = new SolidColorBrush(Color.FromArgb(255,235,235,235));
            this.Children.Add(goodsGrid);
        }
    }
}
