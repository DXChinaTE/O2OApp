using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;

namespace PancakeCooker.Common
{
    public class GoodsInfo
    {
        private BitmapImage _goodsImg;
        private int _count;
        private string _countString;
        public BitmapImage img
        {
            get
            {
                return _goodsImg;
            }
            set
            {
                _goodsImg = value;
                _brush.ImageSource = _goodsImg;
            }
        }
        public string imgUri { get; set; }
        public string name { get; set; }
        public string imageName { get; set; }
        public string price { get; set; }
        public int count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
                _countString = "x" + Convert.ToString(_count);
            }
        }
        public string countString
        {
            get
            {
                return _countString;
            }
        }
        private ImageBrush _brush = new ImageBrush();
        public ImageBrush brush
        {
            get
            {
                return _brush;
            }
        }
    }

    public class OrderInfo
    {
        public string id { get; set; }
        public string customerName { get; set; }
        public string mobile { get; set; }
        public string price { get; set; }
        public string address { get; set; }
        public string payed { get; set; }
        public string completed { get; set; }

        public List<GoodsInfo> goodsList = new List<GoodsInfo>();
    }

    public class OrderGridSizeInfo
    {
        public double infoPanelHeight { get; set; }
        //public double goodsGridHeight { get; set; }
        //public double orderInfoPanelHeight { get; set; }
        public double goodsPicHeight { get; set; }
        public double goodsPicWidth { get; set; }
        public double margin { get; set; }
    }

}
