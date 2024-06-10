using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace wms.Controls
{
    /// <summary>
    /// 悬浮按钮
    /// </summary>
    public partial class FloatButton : Button
    {
        public new event EventHandler ClickEvent;

        private bool _move = false;
        double _distance = 200;
        double _distanceNew = 5;
        private Point _lastPos;
        private Point _newPos;
        private Point _oldPos;

        public FloatButton()
        {
            InitializeComponent();
        }

        private void btn_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Parent != null && this.Parent is FrameworkElement)
            {
                FrameworkElement parent = this.Parent as FrameworkElement;
                double left1 = parent.ActualWidth - this.ActualWidth - this._distanceNew;
                double top1 = parent.ActualHeight - this.ActualHeight - this._distanceNew;
                this.Margin = new Thickness(left1, top1, 0, 0);

                parent.PreviewMouseMove += (s, ee) =>
                {
                    if (_move)
                    {
                        Point pos = ee.GetPosition(parent);
                        double left = this.Margin.Left + pos.X - this._lastPos.X;
                        double top = this.Margin.Top + pos.Y - this._lastPos.Y;
                        this.Margin = new Thickness(left, top, 0, 0);

                        _lastPos = ee.GetPosition(parent);
                    }
                };

                parent.PreviewMouseUp += (s, ee) =>
                {
                    if (_move)
                    {
                        _move = false;

                        Point pos = ee.GetPosition(parent);
                        _newPos = pos;
                        double left = this.Margin.Left + pos.X - this._lastPos.X;
                        double top = this.Margin.Top + pos.Y - this._lastPos.Y;
                        double right = parent.ActualWidth - left - this.ActualWidth;
                        double bottom = parent.ActualHeight - top - this.ActualHeight;

                        if (left < _distance && top < _distance) //左上
                        {
                            left = this._distanceNew;
                            top = this._distanceNew;
                        }
                        else if (left < _distance && bottom < _distance) //左下
                        {
                            left = this._distanceNew;
                            top = parent.ActualHeight - this.ActualHeight - this._distanceNew;
                        }
                        else if (right < _distance && top < _distance) //右上
                        {
                            left = parent.ActualWidth - this.ActualWidth - this._distanceNew;
                            top = this._distanceNew;
                        }
                        else if (right < _distance && bottom < _distance) //右下
                        {
                            left = parent.ActualWidth - this.ActualWidth - this._distanceNew;
                            top = parent.ActualHeight - this.ActualHeight - this._distanceNew;
                        }
                        else if (left < _distance && top > _distance && bottom > _distance) //左
                        {
                            left = this._distanceNew;
                            top = this.Margin.Top;
                        }
                        else if (right < _distance && top > _distance && bottom > _distance) //右
                        {
                            left = parent.ActualWidth - this.ActualWidth - this._distanceNew;
                            top = this.Margin.Top;
                        }
                        else if (top < _distance && left > _distance && right > _distance) //上
                        {
                            left = this.Margin.Left;
                            top = this._distanceNew;
                        }
                        else if (bottom < _distance && left > _distance && right > _distance) //下
                        {
                            left = this.Margin.Left;
                            top = parent.ActualHeight - this.ActualHeight - this._distanceNew;
                        }

                        ThicknessAnimation marginAnimation = new ThicknessAnimation();
                        marginAnimation.From = this.Margin;
                        marginAnimation.To = new Thickness(left, top, 0, 0);
                        marginAnimation.Duration = TimeSpan.FromMilliseconds(200);

                        Storyboard story = new Storyboard();
                        story.FillBehavior = FillBehavior.Stop;
                        story.Children.Add(marginAnimation);
                        Storyboard.SetTargetName(marginAnimation, "btn");
                        Storyboard.SetTargetProperty(marginAnimation, new PropertyPath("(0)", Border.MarginProperty));

                        story.Begin(this);

                        this.Margin = new Thickness(left, top, 0, 0);
                    }
                };
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.Parent != null && this.Parent is FrameworkElement)
            {
                FrameworkElement parent = this.Parent as FrameworkElement;
                _move = true;
                _lastPos = e.GetPosition(parent);
                _oldPos = _lastPos;
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            if (_newPos.Equals(_oldPos))
            {
                if (ClickEvent != null)
                {
                    ClickEvent(sender, e);
                }
            }
        }
    }
}
