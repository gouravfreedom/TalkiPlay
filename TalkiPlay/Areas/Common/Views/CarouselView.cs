//#region License

///*
//Licensed to Blue Chilli Technology Pty Ltd and the contributors under the MIT License (the "License").
//You may not use this file except in compliance with the License.
//See the LICENSE file in the project root for more information.
//*/

//#endregion


//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Timers;
//using Xamarin.Forms;
//using ChilliSource.Mobile.UI;

//namespace TalkiPlay
//{
//    /// <summary>
//    /// Allows linear sliding of child views
//    /// </summary>
//    public class CarouselView : ScrollView
//    {
//        private readonly StackLayout _stack;
//        private Timer _selectedItemTimer;
//        private bool _layingOutChildren;
//        private int _selectedIndex;

//        /// <summary>
//        /// Initializes a new instance of this <c>CarouselView</c> class.
//        /// </summary>
//        public CarouselView()
//        {
//            Orientation = ScrollOrientation.Horizontal;

//            _stack = new StackLayout
//            {
//                Orientation = StackOrientation.Horizontal,
//                Spacing = 0
//            };

//            Content = _stack;

//            _selectedItemTimer = new Timer
//            {
//                AutoReset = false,
//                Interval = 300
//            };

//            _selectedItemTimer.Elapsed += SelectedItemTimerElapsed;
//        }

//        public delegate void ScrollRequestedDelegate(double animationDuration);
//        public event ScrollRequestedDelegate ScrollRequested;

//        public void ScrollToNextPage(double animationDuration = 0.5)
//        {
//            ScrollRequested?.Invoke(animationDuration);
//        }


//        #region Properties

//        /// <summary>
//        /// Gets the list of child items for the carousel view.
//        /// </summary>
//        /// <value>A <see cref="IList{View}"/> of the children.</value>
//        public IList<View> Children
//        {
//            get
//            {
//                return _stack.Children;
//            }
//        }

//        /// <summary>
//        /// Gets or sets the item template for the carousel view.
//        /// </summary>
//        /// <value>A <see cref="DataTemplate"/> representing the item template for the carousel view.</value>
//        public DataTemplate ItemTemplate
//        {
//            get;
//            set;
//        }


//        /// <summary>
//        /// Backing store for the <c>SelectedIndex</c> bindable property.
//        /// </summary>
//        public static readonly BindableProperty SelectedIndexProperty =
//            BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(CarouselView), 0, BindingMode.TwoWay,
//                                    propertyChanged: (bindable, oldValue, newValue) =>
//                                    {
//                                        ((CarouselView)bindable).UpdateSelectedItem();
//                                    });

//        /// <summary>
//        /// Gets or sets the index of the selected view. This is a bindable property.
//        /// </summary>
//        /// <value>The index of the selected view; the default value is 0.</value>
//        public int SelectedIndex
//        {
//            get
//            {
//                return (int)GetValue(SelectedIndexProperty);
//            }
//            set
//            {
//                SetValue(SelectedIndexProperty, value);
//            }
//        }

//        /// <summary>
//        /// Backing store for the <c>ItemsSource</c> bindable property.
//        /// </summary>
//        public static readonly BindableProperty ItemsSourceProperty =
//            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(CarouselView), null,
//                      propertyChanging: (bindableObject, oldValue, newValue) =>
//                      {
//                          ((CarouselView)bindableObject).ItemsSourceChanging();
//                      },
//                      propertyChanged: (bindableObject, oldValue, newValue) =>
//                      {
//                          ((CarouselView)bindableObject).ItemsSourceChanged();
//                      });

//        /// <summary>
//        /// Gets or sets the source of the items for the carousel view. This is a bindable property.
//        /// </summary>
//        /// <value>A <see cref="IEnumerable"/> value that provides the source of the items for the carousel view.</value>
//        public IEnumerable ItemsSource
//        {
//            get
//            {
//                return (IEnumerable)GetValue(ItemsSourceProperty);
//            }
//            set
//            {
//                SetValue(ItemsSourceProperty, value);
//            }
//        }


//        /// <summary>
//        /// Backing store for the <c>SelectedItem</c> bindable property.
//        /// </summary>
//        public static readonly BindableProperty SelectedItemProperty =
//            BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(CarouselView), null, BindingMode.TwoWay,
//                propertyChanged: (bindable, oldValue, newValue) =>
//                {
//                    ((CarouselView)bindable).UpdateSelectedIndex();
//                });

//        /// <summary>
//        /// Gets or sets the selected view. This is a bindbale property.
//        /// </summary>
//        public object SelectedItem
//        {
//            get
//            {
//                return GetValue(SelectedItemProperty);
//            }
//            set
//            {
//                SetValue(SelectedItemProperty, value);
//            }
//        }

//        #endregion


//        #region Overrides

//        protected override void LayoutChildren(double x, double y, double width, double height)
//        {
//            base.LayoutChildren(x, y, width, height);
//            if (_layingOutChildren)
//            {
//                return;
//            }

//            _layingOutChildren = true;
//            foreach (var child in Children)
//            {
//                child.WidthRequest = width;
//            }
//            _layingOutChildren = false;
//        }

//        #endregion

//        #region Private Methods

//        private void UpdateSelectedItem()
//        {
//            _selectedItemTimer.Stop();
//            _selectedItemTimer.Start();
//        }

//        private void SelectedItemTimerElapsed(object sender, ElapsedEventArgs e)
//        {
//            Device.BeginInvokeOnMainThread(() =>
//            {

//                SelectedItem = Children.Count > 0 && SelectedIndex > -1 ? Children[SelectedIndex].BindingContext : null;

//            });
//        }

//        private void ItemsSourceChanging()
//        {
//            if (ItemsSource == null)
//            {
//                return;
//            }
//            _selectedIndex = (ItemsSource as IList)?.IndexOf(SelectedItem) ?? 0;
//        }

//        private void ItemsSourceChanged()
//        {
//            _stack.Children.Clear();

//            foreach (var item in ItemsSource)
//            {
//                View view = null;

//                if (ItemTemplate is DataTemplateSelector)
//                {
//                    var selector = ItemTemplate as DataTemplateSelector;
//                    var template = selector?.SelectTemplate(item, this);
//                    view = template?.CreateContent() as View;
//                }
//                else
//                {
//                    view = ItemTemplate.CreateContent() as View;
//                }

//                if (view is BindableObject bindableObject)
//                {
//                    bindableObject.BindingContext = item;
//                }
//                _stack.Children.Add(view);
//            }

//            if (_selectedIndex >= 0)
//            {
//                SelectedIndex = _selectedIndex;
//            }
//        }

//        private void UpdateSelectedIndex()
//        {
//            if (SelectedItem == BindingContext && Children?.Count > 0)
//            {
//                return;
//            }

//            SelectedIndex = Children
//                .Select(c => c.BindingContext)
//                .ToList()
//                .IndexOf(SelectedItem);
//        }

//        #endregion
//    }
//}

