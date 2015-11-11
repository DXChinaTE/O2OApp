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
    public class PicturePanel : RelativePanel
    {
        private Grid picGrid = new Grid();
        private Border picBorder = new Border();
        private Button _picButton = null;
        public double ButtonWidth
        {
            get
            {
                if(null != _picButton)
                {
                    return _picButton.Width;
                }
                return 1;
            }
            set
            {
                if (null != _picButton)
                {
                    _picButton.Width = _picButton.Height = value;
                }
            }
        }
        public string PicturePath
        {          
            set
            {
                if(null != _picButton)
                {
                    _picButton.Name = value;
                }
            }
        }
        public Button PicButton
        {
            set
            {
                if(_picButton != null)
                {
                    picGrid.Children.Remove(_picButton);
                }
                _picButton = value;
                picGrid.Children.Add(_picButton);
                //this.Width = this.Height = 2 * picGrid.Margin.Left + picButton.Width;
                //picBorder.Width = picBorder.Height = picButton.Width;
            }
        } 
        public double _gridBorder
        {
            set
            {
                picGrid.Margin = new Thickness(value,value,value,value);
            }
            get
            {
                return picGrid.Margin.Left;
            }
        }
        public Brush _BackGroud
        {
            set
            {
                this.Background = value;
            }
            get
            {
                return this.Background;
            }
        }
        public Brush _gridBackgroud
        {
            get
            {
                return picBorder.Background;
            }
            set
            {
                picBorder.Background = value;
            }
        }
        public string _Name
        {
            get
            {
                return this.Name;
            }
            set
            {
                this.Name = value;
            }
        }
        public PicturePanel()
        {
            picGrid.Margin = new Thickness(10, 10, 10, 10);
            RowDefinition row = new RowDefinition();
            picGrid.RowDefinitions.Add(row);
            ColumnDefinition col = new ColumnDefinition();
            picGrid.ColumnDefinitions.Add(col);
            picBorder.BorderThickness = new Thickness(1);
            picBorder.BorderBrush = new SolidColorBrush(Colors.White);
            picBorder.CornerRadius = new CornerRadius(5,5,5,5);
            picGrid.Children.Add(picBorder);
            this.Children.Add(picGrid);
        }
    }
}
