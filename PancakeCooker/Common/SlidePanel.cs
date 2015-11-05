using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;

namespace PancakeCooker.Common
{
    public class SlidePanel : RelativePanel
    {
        private Button _leftButton = new Button();
        private double _slideStepWidth = 0;
        private StackPanel _panelLeft = new StackPanel();
        private StackPanel _panelRight = new StackPanel();
        private Button _buttonLeft = new Button();
        private Button _buttonRight = new Button();
        private SolidColorBrush __buttonPanelBackground = new SolidColorBrush(Color.FromArgb(0,255,255,255));
        private SolidColorBrush _buttonBackground = new SolidColorBrush(Color.FromArgb(153, 255, 255, 255));
        private SolidColorBrush _buttonSymbolForeground = new SolidColorBrush(Color.FromArgb(255,51,51,51));
        private RelativePanel _child = null;
        private Point _prePoint;

        public SlidePanel(double width)
        {
            this.Width = width;
            _panelLeft.Width = _panelRight.Width = 30;
            _buttonLeft.Background = _buttonRight.Background = _buttonBackground;
            _panelLeft.Background = _panelRight.Background = __buttonPanelBackground;
            _buttonLeft.BorderThickness = _buttonRight.BorderThickness = new Thickness(0);
            _buttonLeft.Foreground = _buttonRight.Foreground = _buttonSymbolForeground;
            _buttonRight.Content = ">";
            _buttonLeft.Content = "<";
            _buttonLeft.FontSize = _buttonRight.FontSize = 20;
            _panelLeft.SetValue(RelativePanel.AlignLeftWithPanelProperty,true);
            _panelRight.SetValue(RelativePanel.AlignRightWithPanelProperty,true);            
            this.SizeChanged += SlidePanel_SizeChanged;
            this.PointerMoved += SlidePanel_PointerMoved;
            //this.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.TranslateX | Windows.UI.Xaml.Input.ManipulationModes.TranslateY;
            //this.ManipulationStarted += SlidePanel_ManipulationStarted;
            //this.ManipulationCompleted += SlidePanel_ManipulationCompleted;
        }

        private void SlidePanel_ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
            if(e.Position.X > _prePoint.X && _child.Margin.Left < 0)
            {
                e.Handled = true;
                if(System.Math.Abs(_child.Margin.Right) > e.Position.X - _prePoint.X)
                {
                    _child.Margin = new Thickness(_child.Margin.Left +(e.Position.X - _prePoint.X), 0, _child.Margin.Right -(e.Position.X - _prePoint.X), 0);
                }
                else
                {
                    _child.Margin = new Thickness(0, 0, _child.Margin.Right - System.Math.Abs(_child.Margin.Left), 0);
                }
            }
            else if(e.Position.X < _prePoint.X && _child.Margin.Right < 0)
            {
                e.Handled = true;
                if (e.Position.X - _prePoint.X < _child.Margin.Right)
                {
                    _child.Margin = new Thickness(_child.Margin.Left + (e.Position.X - _prePoint.X), 0, _child.Margin.Right - (e.Position.X - _prePoint.X), 0);
                }
                else
                {
                    _child.Margin = new Thickness(_child.Margin.Left + _child.Margin.Right, 0, 0, 0);
                }               
            }
        }

        private void SlidePanel_ManipulationStarted(object sender, Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        {
            _prePoint = e.Position;
        }

        private void SlidePanel_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Windows.UI.Input.PointerPoint ptr = e.GetCurrentPoint(sender as UIElement);
            Point pt = ptr.Position;
            if(pt.X <= _panelLeft.Width)
            {
                _panelRight.Visibility = Visibility.Collapsed;
                if (_child.Margin.Left < 0)
                {
                    _panelLeft.Visibility = Visibility.Visible;
                }
            }
            else if(pt.X >= this.Width - _panelRight.Width)
            {                               
                _panelLeft.Visibility = Visibility.Collapsed;
                if (_child.Margin.Right < 0)
                {
                    _panelRight.Visibility = Visibility.Visible;
                }
            }
            else
            {
                _panelRight.Visibility = _panelLeft.Visibility = Visibility.Collapsed;
            }
        }

        private void SlidePanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(null != _child)
            {
                _child.Margin = new Thickness(0,0, this.Width - _child.Width, 0);
                _panelLeft.Visibility = Visibility.Collapsed;
                _panelRight.Visibility = Visibility.Collapsed;              
            }
        }

        public void Add(RelativePanel child,double itemWidth)
        {
            _child = child;
            child.SetValue(RelativePanel.AlignLeftWithPanelProperty,true);
            child.SetValue(RelativePanel.AlignTopWithPanelProperty,true);
            child.SetValue(RelativePanel.AlignBottomWithPanelProperty,true);
            child.SetValue(RelativePanel.AlignRightWithPanelProperty,true);
            this._slideStepWidth = itemWidth;
            this.Background = child.Background;
            _buttonLeft.Height = _buttonRight.Height = _panelLeft.Height = _panelRight.Height = this.Height = child.Height;
            _buttonLeft.Width = _buttonRight.Width = 30;
            _buttonLeft.Click += _buttonLeft_Click;
            _buttonRight.Click += _buttonRight_Click;       
            child.SetValue(RelativePanel.AlignTopWithPanelProperty,true);
            _panelLeft.Children.Add(_buttonLeft);
            _panelRight.Children.Add(_buttonRight);
            this.Children.Add(child);
            this.Children.Add(_panelLeft);
            this.Children.Add(_panelRight);
        }

        private void _buttonRight_Click(object sender, RoutedEventArgs e)
        {
            if (null != _child)
            {
                if (_child.Margin.Right < 0)
                {
                    double stepwidth = System.Math.Abs(_child.Margin.Right % _slideStepWidth);
                    if(stepwidth != 0)
                    {
                        _child.Margin = new Thickness(_child.Margin.Left - stepwidth, 0, _child.Margin.Right +  stepwidth, 0);
                    }
                    else
                    {
                        _child.Margin = new Thickness(_child.Margin.Left - _slideStepWidth, 0, _child.Margin.Right + _slideStepWidth, 0);
                    }                  
                }
                return;
            }
        }

        private void _buttonLeft_Click(object sender, RoutedEventArgs e)
        {
            if (null != _child)
            {
                if (_child.Margin.Left < 0)
                {
                    double stepwidth = System.Math.Abs(_child.Margin.Left % _slideStepWidth);
                    if(stepwidth != 0)
                    {
                        _child.Margin = new Thickness(_child.Margin.Left + stepwidth, 0, _child.Margin.Right  - stepwidth, 0);
                    }
                    else
                    {
                        _child.Margin = new Thickness(_child.Margin.Left + _slideStepWidth, 0, _child.Margin.Right - _slideStepWidth, 0);
                    }                    
                }
                return;
            }           
        }
    }
}
