using System;
using System.Collections.Generic;
using ReactiveUI;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class SpacerCell : ReactiveBaseViewCell<IEmptyItemViewModel>
    {
        public SpacerCell()
        {
            InitializeComponent();

        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if(BindingContext is IEmptyItemViewModel vm)
            {
                EmptyBox.HeightRequest = vm.Height;
                Height = vm.Height;
            }
        }
    }
}
