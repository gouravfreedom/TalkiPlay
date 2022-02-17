using System;
using System.Collections.Generic;
using System.Windows.Input;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms;

namespace TalkiPlay
{
    public interface ISvgImageButtonController : IButtonController
    {
        bool IsEnabled { get; }
        void SendEnabled(bool enabled);
    }
    
    public class SvgImageButton : ContentView, ISvgImageButtonController
    {
        private SvgCachedImage _buttonImage;
    
        public SvgImageButton()
        {
            BuildContent();
        }

        /// <summary>
        /// Backing store for the <c>Command</c> bindable property.
        /// </summary>
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(SvgImageButton), default(ICommand));

        public ICommand Command
        {
            get => (ICommand) this.GetValue(CommandProperty);
            set => this.SetValue(CommandProperty, value);
        }
        
        /// <summary>
        /// Backing store for the <c>CommandParameter</c> bindable property.
        /// </summary>
        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(SvgImageButton));

        public object CommandParameter
        {
            get => (object) this.GetValue(CommandParameterProperty);
            set => this.SetValue(CommandParameterProperty, value);
        }

        public void SendEnabled(bool enabled)
        {
            this.IsEnabled = enabled;
        }

        public event EventHandler Clicked;

        public static BindableProperty SourceProperty =
            BindableProperty.Create(nameof(Source), typeof(ImageSource), typeof(SvgImageButton), propertyChanged: (
                bindable, value, newValue) =>
            {
                var view = (SvgImageButton) bindable;
                var source = newValue as Xamarin.Forms.ImageSource;
                view._buttonImage.Source = source;
            });

        [TypeConverter(typeof (ImageSourceConverter))]
        public ImageSource Source
        {
            get => (ImageSource) this.GetValue(SourceProperty);
            set => this.SetValue(SourceProperty, value);
        }
          
        public static BindableProperty PressedSourceProperty =
            BindableProperty.Create(nameof(PressedSource), typeof(ImageSource), typeof(SvgImageButton));

        [TypeConverter(typeof (ImageSourceConverter))]
        public ImageSource PressedSource
        {
            get => (ImageSource) this.GetValue(PressedSourceProperty);
            set => this.SetValue(PressedSourceProperty, value);
        }
        
        public static BindableProperty DisabledSourceProperty =
            BindableProperty.Create(nameof(DisabledSource), typeof(ImageSource), typeof(SvgImageButton));

        
        [TypeConverter(typeof (ImageSourceConverter))]
        public ImageSource DisabledSource
        {
            get => (ImageSource) this.GetValue(DisabledSourceProperty);
            set => this.SetValue(DisabledSourceProperty, value);
        }

        public Aspect Aspect
        {
            get => _buttonImage.Aspect;
            set => _buttonImage.Aspect = value;
        }

        public static BindableProperty ImageColorProperty =
         BindableProperty.Create(nameof(ImageColorProperty), typeof(Color), typeof(SvgImageButton), defaultValue: Color.Transparent, propertyChanged: (
               bindable, value, newValue) =>
         {
             var view = (SvgImageButton)bindable;
             var source = (Xamarin.Forms.Color) newValue; 

             if(source != Color.Transparent)
             {
                 var map = view._buttonImage.ReplaceStringMap ?? new Dictionary<string, string>();
                 map["fill: rgb(0, 0, 0);"] = GetRGBFill(source);
             }

         });

        public Color ImageColor
        {
            get => (Color) this.GetValue(ImageColorProperty);
            set => this.SetValue(ImageColorProperty, value);
        }


        /// <summary>
        /// Gets or sets the requested width of the image. If less than or equal to zero than a 
        /// width of 50 will be used.
        /// </summary>
        /// <value>
        /// The ImageWidthRequest property gets/sets the value of the backing field, ImageWidthRequestProperty.
        /// </value> 
        public double ImageWidthRequest
        {
            get => _buttonImage.WidthRequest;
            set => _buttonImage.WidthRequest = value;
        }

        public double ImageScale
        {
            get => _buttonImage.Scale;
            set => _buttonImage.Scale = value;
        }

        public double ImageHeightRequest
        {
            get => _buttonImage.HeightRequest;
            set => _buttonImage.HeightRequest = value;
        }

        public bool DownsampleToViewSize
        {
            get => _buttonImage.DownsampleToViewSize;
            set => _buttonImage.DownsampleToViewSize = value;
        }
        
        public double DownsampleHeight
        {
            get => _buttonImage.DownsampleHeight;
            set => _buttonImage.DownsampleHeight = value;
        }
        
        public double DownsampleWidth
        {
            get => _buttonImage.DownsampleWidth;
            set => _buttonImage.DownsampleWidth = value;
        }
        
        public new bool IsEnabled
        {
            get => (bool) this.GetValue(IsEnabledProperty);
            set => this.SetValue(IsEnabledProperty, value);
        }

        private void BuildContent()
        {
            _buttonImage = new SvgCachedImage();
             Content = _buttonImage;
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(IsEnabled):
                    _buttonImage.Source = IsEnabled ? Source : DisabledSource ?? Source;
                    Opacity = !IsEnabled && DisabledSource == null ? 0.5 : 1.0;
                    break;
            }
        }


        public void SendClicked()
        {
            if (!IsEnabled) return;
            Clicked?.Invoke(this, EventArgs.Empty);
            Command?.Execute(null);
        }

        public void SendPressed()
        {
            if (PressedSource == null)
            {
                Opacity = 0.5;
                return;
            }
            
            _buttonImage.Source = PressedSource;
        }

        public void SendReleased()
        {
            if (PressedSource == null)
            {
                Opacity = 1.0;
                return;
            }
            
            _buttonImage.Source = Source;
        }

        public static string GetRGBFill(Color color)
        {
            var red = (int)(color.R * 255);
            var green = (int)(color.G * 255);
            var blue = (int)(color.B * 255);
            var rgbFill = $"fill: rgb({red},{green},{blue});";
            return rgbFill;
        }
    }

  
}