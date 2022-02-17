using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Shapes;
using Xamarin.Forms.Xaml;

namespace TalkiPlay
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StarView : ContentView
    {
        public StarView()
        {
            InitializeComponent();
            UpdateProgress();
            imgOn.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Width" || e.PropertyName == "Height")
                {
                    UpdateProgress();
                }
            };
            TapGesture.Tapped += OnViewTapped;
        }
        public TapGestureRecognizer TapGesture { get; } = new TapGestureRecognizer();
        //public RatingView RatingView { get; internal set; }

        public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create(nameof(IsReadOnly), typeof(bool), typeof(StarView), true);
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public static readonly BindableProperty ProgressValueProperty = BindableProperty.Create(nameof(ProgressValue), typeof(double), typeof(StarView), 0d, BindingMode.TwoWay);
        public double ProgressValue
        {
            get => (double)GetValue(ProgressValueProperty);
            set => SetValue(ProgressValueProperty, value);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case "ProgressValue":
                    UpdateProgress();
                    break;
                case "IsReadOnly":
                    UpdateGesture();
                    break;
                default: break;
            }
        }

        internal void UpdateGesture()
        {
            if (IsReadOnly)
            {
                if (rootGrid.GestureRecognizers.Contains(TapGesture))
                {
                    rootGrid.GestureRecognizers.Remove(TapGesture);
                }
            }
            else
            {
                if (!rootGrid.GestureRecognizers.Contains(TapGesture))
                {
                    rootGrid.GestureRecognizers.Add(TapGesture);
                }
            }
        }

        private void UpdateProgress()
        {
            if (imgOn.Width != double.NaN && imgOn.Width > 0 && imgOn.Height != double.NaN && imgOn.Height > 0)
            {
                var rect = new Xamarin.Forms.Rectangle();
                rect.Width = imgOn.Width * Math.Min(ProgressValue, 1d);
                rect.Height = imgOn.Height;
                imgOn.Clip = new RectangleGeometry() { Rect = rect };
            }
        }

        private void OnViewTapped(object sender, EventArgs e)
        {
            //if (RatingView.IsReadOnly == false)
            {
                //RatingView.HandleStarClicked(this);
            }
        }
    }
}