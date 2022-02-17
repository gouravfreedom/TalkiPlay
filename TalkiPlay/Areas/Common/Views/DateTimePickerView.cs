//using System;
//using System.Collections.Generic;
//using ChilliSource.Mobile.UI;
//using Xamarin.Forms;

//namespace TalkiPlay
//{
//    public enum PickerMode
//    {
//        DateTime,
//        Date,
//        Time
//    }

//    public class DateTimePickerView : View
//    {
//        public static readonly BindableProperty MinimumDateTimeProperty =
//            BindableProperty.Create(nameof(MinimumDateTime), typeof(DateTime), typeof(DateTimePickerView), DateTime.MinValue);

//        public DateTime MinimumDateTime
//        {
//            get { return (DateTime)GetValue(MinimumDateTimeProperty); }
//            set { SetValue(MinimumDateTimeProperty, value); }
//        }

//        public static readonly BindableProperty MaximumDateTimeProperty =
//            BindableProperty.Create(nameof(MaximumDateTime), typeof(DateTime), typeof(DateTimePickerView), DateTime.MaxValue);

//        public DateTime MaximumDateTime
//        {
//            get { return (DateTime)GetValue(MaximumDateTimeProperty); }
//            set { SetValue(MaximumDateTimeProperty, value); }
//        }

//        public static readonly BindableProperty SelectedDateTimeProperty =
//            BindableProperty.Create(nameof(SelectedDateTime), typeof(DateTime), typeof(DateTimePickerView), DateTime.Now);

//        public DateTime SelectedDateTime
//        {
//            get { return (DateTime)GetValue(SelectedDateTimeProperty); }
//            set { SetValue(SelectedDateTimeProperty, value); }
//        }

//        public static readonly BindableProperty PickerModeProperty =
//            BindableProperty.Create(nameof(PickerMode), typeof(PickerMode), typeof(DateTimePickerView), PickerMode.DateTime);

//        public PickerMode PickerMode
//        {
//            get { return (PickerMode)GetValue(PickerModeProperty); }
//            set { SetValue(PickerModeProperty, value); }
//        }

//        public static readonly BindableProperty CustomFontProperty =
//           BindableProperty.Create(nameof(CustomFont), typeof(ExtendedFont), typeof(DateTimePickerView), null);

//        public ExtendedFont CustomFont
//        {
//            get { return (ExtendedFont)GetValue(CustomFontProperty); }
//            set { SetValue(CustomFontProperty, value); }
//        }

//        public static readonly BindableProperty CustomSelectedFontProperty =
//            BindableProperty.Create(nameof(CustomSelectedFont), typeof(ExtendedFont), typeof(DateTimePickerView), null);

//        public ExtendedFont CustomSelectedFont
//        {
//            get { return (ExtendedFont)GetValue(CustomSelectedFontProperty); }
//            set { SetValue(CustomSelectedFontProperty, value); }
//        }


//        public static readonly BindableProperty MinuteIntervalProperty =
//            BindableProperty.Create(nameof(MinuteInterval), typeof(int), typeof(DateTimePickerView), 1);

//        public int MinuteInterval
//        {
//            get { return (int)GetValue(MinuteIntervalProperty); }
//            set { SetValue(MinuteIntervalProperty, value); }
//        }

//    }
//}

