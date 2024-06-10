using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wms.Controls
{
    /// <summary>
    /// Interaction logic for ucDateTimePicker.xaml
    /// </summary>
    public partial class ucDateTimePicker : UserControl
    {
        public static readonly DependencyProperty DateFormatProperty =
                    DependencyProperty.Register("DateFormat", typeof(string), typeof(ucDateTimePicker), new FrameworkPropertyMetadata("MM/dd/yyyy hh:mm tt", OnDateFormatChanged));

        public static readonly DependencyProperty MaximumDateProperty =
            DependencyProperty.Register("MaximumDate", typeof(DateTime), typeof(ucDateTimePicker), new FrameworkPropertyMetadata(Convert.ToDateTime("01/01/3000 00:00:00 AM"), null));

        public static readonly DependencyProperty MinimumDateProperty =
            DependencyProperty.Register("MinimumDate", typeof(DateTime), typeof(ucDateTimePicker), new FrameworkPropertyMetadata(DateTime.UtcNow, null));

        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register("SelectedDate", typeof(DateTime), typeof(ucDateTimePicker), new FrameworkPropertyMetadata(DateTime.Now, OnSelectedDateChanged));

        public static readonly DependencyProperty DateTextIsWrongProperty =
            DependencyProperty.Register("DateTextIsWrong", typeof(bool), typeof(ucDateTimePicker), new FrameworkPropertyMetadata(false));

        public static readonly RoutedEvent DateFormatChangedEvent =
            EventManager.RegisterRoutedEvent("DateFormatChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucDateTimePicker));

        public static readonly RoutedEvent DateChangedEvent =
            EventManager.RegisterRoutedEvent("DateChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucDateTimePicker));

        private string inputDateFormat;
        private bool forceTextUpdateNow = true;

        public ucDateTimePicker()
        {
            InitializeComponent();
            MinimumDate = DateTime.Parse("1999-01-01");
            CalDisplay.SelectedDatesChanged += CalDisplay_SelectedDatesChanged;
            DateDisplay.PreviewMouseUp += DateDisplay_PreviewMouseUp;
            DateDisplay.LostFocus += DateDisplay_LostFocus;
            DateDisplay.TextChanged += DateDisplay_TextChanged;
        }

        public DateTime SelectedDate
        {
            get { return (DateTime)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        public string DateFormat
        {
            get { return Convert.ToString(GetValue(DateFormatProperty)); }
            set { SetValue(DateFormatProperty, value); }
        }

        public bool ShowCalendarButton
        {
            get { return PopUpCalendarButton.Visibility == Visibility.Visible; }
            set { PopUpCalendarButton.Visibility = (value ? Visibility.Visible : Visibility.Collapsed); }
        }

        public DateTime MinimumDate
        {
            get { return Convert.ToDateTime(GetValue(MinimumDateProperty)); }
            set { SetValue(MinimumDateProperty, value); }
        }

        public DateTime MaximumDate
        {
            get { return Convert.ToDateTime(GetValue(MaximumDateProperty)); }
            set { SetValue(MaximumDateProperty, value); }
        }

        protected bool DateTextIsWrong
        {
            get { return (bool)GetValue(DateTextIsWrongProperty); }
            set { SetValue(DateTextIsWrongProperty, value); }
        }

        public event RoutedEventHandler DateChanged
        {
            add { AddHandler(DateChangedEvent, value); }
            remove { RemoveHandler(DateChangedEvent, value); }
        }

        public event RoutedEventHandler DateFormatChanged
        {
            add { AddHandler(DateFormatChangedEvent, value); }
            remove { RemoveHandler(DateFormatChangedEvent, value); }
        }

        public string InputDateFormat()
        {
            if (inputDateFormat == null)
            {
                var df = DateFormat;

                if (!df.Contains("MMM"))
                {
                    df = df.Replace("MM", "M");
                }

                if (!df.Contains("ddd"))
                {
                    df = df.Replace("dd", "d");
                }

                inputDateFormat = df.Replace("hh", "h").Replace("HH", "H").Replace("mm", "m").Replace("ss", "s");
            }

            return inputDateFormat;
        }



        private void CalDisplay_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            PopUpCalendarButton.IsChecked = false;
            var timeOfDay = SelectedDate.TimeOfDay;

            if (CalDisplay.SelectedDate != null)
            {
                SelectedDate = CalDisplay.SelectedDate.Value.Date + timeOfDay;
            }
        }

        private void DateDisplay_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (DateDisplay.SelectionLength == 0)
            {
                FocusOnDatePart(DateDisplay.SelectionStart);
            }
        }

        private DateTime? ParseDateText(bool flexible)
        {
            DateTime selectedDate;

            if (!DateTime.TryParseExact(DateDisplay.Text, InputDateFormat(), null, DateTimeStyles.AllowWhiteSpaces, out selectedDate)
                && (!flexible || !DateTime.TryParse(DateDisplay.Text, out selectedDate)))
            {
                return null;
            }

            return selectedDate;
        }

        private void ReformatDateText()
        {
            var date = ParseDateText(true);

            if (date != null)
            {
                var newText = date.Value.ToString(DateFormat);

                if (DateDisplay.Text != newText)
                {
                    DateDisplay.Text = newText;
                }
            }
        }

        private void DateDisplay_LostFocus(object sender, RoutedEventArgs e)
        {
            DateDisplay.Text = SelectedDate.ToString(DateFormat);

            try
            {
                DateDisplay.SelectionLength = 0;
            }
            catch (NullReferenceException) { }
        }

        void DateDisplay_TextChanged(object sender, TextChangedEventArgs e)
        {
            var date = ParseDateText(true);

            if (date != null)
            {
                SelectedDate = date.Value;
            }
        }

        public static void OnDateFormatChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var dateTimePicker = (ucDateTimePicker)obj;
            dateTimePicker.inputDateFormat = null;
            dateTimePicker.DateDisplay.Text = dateTimePicker.SelectedDate.ToString(dateTimePicker.DateFormat);
        }

        public static void OnSelectedDateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var dateTimePicker = (ucDateTimePicker)obj;

            var date = (DateTime)args.NewValue;
            dateTimePicker.CalDisplay.SelectedDate = date;
            dateTimePicker.CalDisplay.DisplayDate = date;

            if (dateTimePicker.DateDisplay.IsFocused && !dateTimePicker.forceTextUpdateNow)
            {
                var oldDate = dateTimePicker.ParseDateText(true);

                if (oldDate != null)
                {
                    dateTimePicker.DateTextIsWrong = date != oldDate.Value;
                }
            }
            else
            {
                dateTimePicker.DateTextIsWrong = false;
                dateTimePicker.forceTextUpdateNow = false;

                dateTimePicker.DateDisplay.Text = date >= dateTimePicker.MinimumDate ? date.ToString(dateTimePicker.DateFormat) : dateTimePicker.MinimumDate.ToString(dateTimePicker.DateFormat);
            }
        }

        private void FocusOnDatePart(int selStart)
        {
            ReformatDateText();

            var df = DateFormat;

            if (selStart > df.Length - 1)
            {
                selStart = df.Length - 1;
            }

            var firstchar = df[selStart];

            while (!char.IsLetter(firstchar) && selStart + 1 < df.Length)
            {
                selStart++;
                firstchar = df[selStart];
            }

            while (selStart > 0 && df[selStart - 1] == firstchar)
            {
                selStart--;
            }

            var selLength = 1;

            while (selStart + selLength < df.Length && df[selStart + selLength] == firstchar)
            {
                selLength++;
            }

            if (firstchar == 't')
            {
                return;
            }

            DateDisplay.Focus();
            DateDisplay.Select(selStart, selLength);
        }

        private void DateDisplay_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (SelectedDate < MinimumDate)
            {
                SelectedDate = MinimumDate;
            }
        }
    }
}
