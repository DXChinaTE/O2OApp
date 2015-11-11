using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PancakeCooker.Common.ItemTemplatePanel
{
     public class GoodsPanel : RelativePanel
    {
        private Grid goodsGrid = new Grid();
        private Grid picGrid = new Grid();
        private RelativePanel goodsInfoPanel = new RelativePanel();
        private Border goodsBorder = new Border();
        private Border picBorder = new Border();
        private TextBlock nameText = new TextBlock();
        private TextBlock priceText = new TextBlock();
        private CheckBox _check = null;
        private double _pictureWidth = 0;
        public string PicturePath { get; set; }
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
        public CheckBox Check
        {
            set
            {
                if(null != _check)
                {
                    goodsInfoPanel.Children.Remove(_check);                    
                }
                _check = value;
                _check.SetValue(RelativePanel.AlignLeftWithPanelProperty,true);
                _check.SetValue(RelativePanel.AboveProperty,picGrid.Name);
                _check.Margin = new Thickness(10,0,0,0);
                goodsInfoPanel.Children.Add(_check);
            }
            get
            {
                return _check;
            }
        }
        public string GoodsName
        {
            set
            {
                nameText.Text = value;
                this.Name = value;
            }
            get
            {
                return nameText.Text;
            }
        }
        public string Price
        {
            set
            {
                priceText.Text = value;
            }
            get
            {
                return priceText.Text;
            }
        }
        public Brush PictureBrush
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
       
        public GoodsPanel()
        {
            this.Margin = new Thickness(5,5,5,5);
            RowDefinition goodsRow = new RowDefinition();
            ColumnDefinition goodsColumn = new ColumnDefinition();
            goodsGrid.RowDefinitions.Add(goodsRow);
            goodsGrid.ColumnDefinitions.Add(goodsColumn);
            goodsGrid.Background = new SolidColorBrush(Color.FromArgb(255,235,235,235));          
            goodsBorder.BorderThickness = new Thickness(1);
            goodsBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255,255,255,255));
            goodsBorder.CornerRadius = new CornerRadius(5,5,5,5);
            goodsGrid.Children.Add(goodsBorder);

            //init goods info panel    
            goodsInfoPanel.Name = "goodsInfoPanel";      
            nameText.Width = 50;
            nameText.Height = 25;
            nameText.Foreground = new SolidColorBrush(Color.FromArgb(255,51,51,51));
            nameText.Name = "nameText";
            nameText.SetValue(RelativePanel.AlignLeftWithPanelProperty,true);
            nameText.SetValue(RelativePanel.AlignBottomWithPanelProperty,true);
            nameText.Margin = new Thickness(10,0,0,10);
            nameText.TextAlignment = TextAlignment.Left;
            goodsInfoPanel.Children.Add(nameText);

            priceText.Width = 50;
            priceText.Height = 25;
            priceText.Foreground = new SolidColorBrush(Color.FromArgb(255,253,40,57));
            priceText.SetValue(RelativePanel.AlignRightWithPanelProperty,true);
            priceText.SetValue(RelativePanel.AlignBottomWithPanelProperty,true);
            priceText.Margin = new Thickness(0,0,10,10);
            priceText.TextAlignment = TextAlignment.Right;
            goodsInfoPanel.Children.Add(priceText);

            RowDefinition picRow = new RowDefinition();
            ColumnDefinition picColumn = new ColumnDefinition();
            picGrid.RowDefinitions.Add(picRow);
            picGrid.ColumnDefinitions.Add(picColumn);
            picGrid.Name = "PictureGrid";
            picGrid.SetValue(RelativePanel.AlignLeftWithPanelProperty, true);
            picGrid.SetValue(RelativePanel.AboveProperty,nameText.Name);
            picGrid.Margin = new Thickness(10,0,10,10);
            picBorder.BorderThickness = new Thickness(1);
            picBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255,255,255,255));
            picBorder.CornerRadius = new CornerRadius(5,5,5,5);
            picGrid.Children.Add(picBorder);
            goodsInfoPanel.Children.Add(picGrid);

            goodsGrid.Children.Add(goodsInfoPanel);
            this.Children.Add(goodsGrid);
        }
    }
}
