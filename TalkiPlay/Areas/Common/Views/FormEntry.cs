//using System;
//using ChilliSource.Mobile.UI;
//using FFImageLoading.Svg.Forms;
//using TalkiPlay.Shared;
//using Xamarin.Forms;

//namespace TalkiPlay
//{
//	public class FormEntry : Grid
//	{
//		private ExtendedEntry _entry;
//		private SvgCachedImage _icon;
//		private BoxView _underline;
//		private ColumnDefinition _iconColumn;
//		private const double IconColumnWidth = 24;
//		private const double ColumnPadding = 16;
//		public FormEntry()
//		{
//			BuildContent();
//		}


//		private void BuildContent()
//		{
//			_iconColumn = new ColumnDefinition() { Width = 24 };
//			this.ColumnDefinitions.Add(_iconColumn);
//			this.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });

//			_iconColumn.Width = !String.IsNullOrWhiteSpace(Icon) ? IconColumnWidth : 0;
//			this.ColumnSpacing = !String.IsNullOrWhiteSpace(Icon) ? ColumnPadding : 0;
//			this.RowSpacing = 8;


//			this.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
//			this.RowDefinitions.Add(new RowDefinition() { Height = 1 });

//			_entry = new ExtendedEntry { HasBorder = false, HorizontalContentPadding = 0};
//			_entry.Effects.Add(new BorderlessEffect());

//			_entry.Focused += (sender, args) => { UpdateUnderline(); };
//			_entry.Unfocused += (sender, args) => { UpdateUnderline(); };
//			_icon = new SvgCachedImage() { Source = Icon };
//			_underline = new BoxView() { BackgroundColor = UnderlineColor };
//			this.Children.Add(_icon, 0, 0);
//			this.Children.Add(_entry, 1, 0);
//			this.Children.Add(_underline, 0, 1);

//			SetColumnSpan(_underline, 2);

//		}

//		private void UpdateUnderline()
//		{
//			if (!IsValid)
//			{
//				_underline.BackgroundColor = ErrorUnderlineColor;
//			}
//			else if (_entry.IsFocused)
//			{
//				_underline.BackgroundColor = FocusUnderlineColor;
//			}
//			else
//			{
//				_underline.BackgroundColor = UnderlineColor;
//			}

//		}

//		public static BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(string), typeof(FormEntry), propertyChanged:
//			(bindable, value, newValue) =>
//			{
//				if (bindable is FormEntry view)
//				{
//					var val = (string)newValue;
//					view._iconColumn.Width = !String.IsNullOrWhiteSpace(val) ? IconColumnWidth : 0;
//					view.ColumnSpacing = !String.IsNullOrWhiteSpace(val) ? ColumnPadding : 0;
//					view._icon.Source = val;
//				}
//			});

//		public string Icon
//		{
//			get => (string)GetValue(IconProperty);
//			set => SetValue(IconProperty, value);
//		}

//		public ExtendedEntry Entry => _entry;

//		public static BindableProperty UnderlineColorProperty = BindableProperty.Create(nameof(UnderlineColor),
//			typeof(Color), typeof(FormEntry), Colors.LightGreyColor, propertyChanged:
//			(bindable, value, newValue) =>
//			{
//				if (bindable is FormEntry view)
//				{
//					view._underline.BackgroundColor = (Color)newValue;
//				}
//			});


//		public Color UnderlineColor
//		{
//			get => (Color)GetValue(UnderlineColorProperty);
//			set => SetValue(UnderlineColorProperty, value);
//		}

//		public static BindableProperty FocusUnderlineColorProperty = BindableProperty.Create(nameof(FocusUnderlineColor),
//			typeof(Color), typeof(FormEntry), Colors.WarmGrey);

//		public Color FocusUnderlineColor
//		{
//			get => (Color)GetValue(FocusUnderlineColorProperty);
//			set => SetValue(FocusUnderlineColorProperty, value);
//		}

//		public static BindableProperty ErrorUnderlineColorProperty = BindableProperty.Create(nameof(ErrorUnderlineColor),
//			typeof(Color), typeof(FormEntry), Colors.Red);

//		public Color ErrorUnderlineColor
//		{
//			get => (Color)GetValue(ErrorUnderlineColorProperty);
//			set => SetValue(ErrorUnderlineColorProperty, value);
//		}

//		public static BindableProperty PlaceHolderTextProperty = BindableProperty.Create(nameof(PlaceHolderText),
//			typeof(string), typeof(FormEntry), propertyChanged: (bindable, value, newValue) =>
//			{
//				if (bindable is FormEntry view)
//				{
//					view._entry.Placeholder = (string)newValue;
//				}
//			});


//		public string PlaceHolderText
//		{
//			get => (string)GetValue(PlaceHolderTextProperty);
//			set => SetValue(PlaceHolderTextProperty, value);
//		}

//		public ExtendedFont EntryFont
//		{
//			get => _entry.CustomFont;
//			set => _entry.CustomFont = value;
//		}

//		public ExtendedFont PlaceHolderFont
//		{
//			get => _entry.CustomPlaceholderFont;
//			set => _entry.CustomPlaceholderFont = value;
//		}

//		public bool IsPassword
//		{
//			get => _entry.IsPassword;
//			set => _entry.IsPassword = value;
//		}

//		public bool IsSpellCheckEnabled
//		{
//			get => _entry.IsSpellCheckEnabled;
//			set => _entry.IsSpellCheckEnabled = value;
//		}

//		public static BindableProperty IsValidProperty = BindableProperty.Create(nameof(IsValid),
//			typeof(bool), typeof(FormEntry), true, propertyChanged: (bindable, value, newValue) =>
//			{
//				if (bindable is FormEntry view)
//				{
//					var b = (bool)newValue;
//					view.UpdateUnderline();
//				}

//			});


//		public bool IsValid
//		{
//			get => (bool)GetValue(IsValidProperty);
//			set => SetValue(IsValidProperty, value);
//		}

//		public Color EntryBackgroundColor
//		{
//			get => _entry.NormalBackgroundColor;
//			set => _entry.NormalBackgroundColor = value;
//		}

//		public float HorizontalContentPadding
//		{
//			get => _entry.HorizontalContentPadding;
//			set => _entry.HorizontalContentPadding = value;
//		}

		
//		public KeyboardReturnKeyType KeyboardReturnKeyType
//		{
//			get => _entry.KeyboardReturnType;
//			set
//			{
//				_entry.KeyboardReturnType = value;

//				switch (value)
//				{
//					case KeyboardReturnKeyType.Next:
//						_entry.ReturnType = ReturnType.Next;
//						break;
//					case KeyboardReturnKeyType.Done:
//						_entry.ReturnType = ReturnType.Done;
//						break;
//					case KeyboardReturnKeyType.Go:
//						_entry.ReturnType = ReturnType.Go;
//						break;
//					default:
//						_entry.ReturnType = ReturnType.Default;
//						break;
//				};
//			}
//		}

//		public Keyboard Keyboard
//		{
//			get => _entry.Keyboard;
//			set => _entry.Keyboard = value;
//		}

//		public bool IsEntryFocused => _entry.IsFocused;

//		public static BindableProperty NextEntryProperty = BindableProperty.Create(nameof(NextEntry), typeof(FormEntry), typeof(FormEntry),
//			propertyChanged: (bindable, oldValue, newValue) =>
//			{
//				if (bindable is FormEntry view)
//				{
//					if (newValue != null)
//					{
//						var f = newValue as FormEntry;
//						view._entry.ReturnCommand = new Command(() => { f?._entry?.Focus(); });
//					}

//				}

//			});

//		public FormEntry NextEntry
//		{
//			get => (FormEntry)GetValue(NextEntryProperty);
//			set => SetValue(NextEntryProperty, value);
//		}
//	}
//}

