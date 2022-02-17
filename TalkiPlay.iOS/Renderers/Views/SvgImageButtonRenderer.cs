using System;
using TalkiPlay;
using TalkiPlay.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SvgImageButton), typeof(SvgImageButtonRenderer))]
namespace TalkiPlay.iOS
{
    public class SvgImageButtonRenderer : ViewRenderer<SvgImageButton, CustomButtonView> , IControlRenderer
    {
        private ISvgImageButtonController SvgImageButtonController => Element;

        protected override void OnElementChanged(ElementChangedEventArgs<SvgImageButton> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (this.Control == null)
                {
                    this.SetNativeControl(CreateNativeControl());   
                }

                 this.Control.TouchUpInside += ControlOnTouchUpInside;
                this.Control.TouchUpOutside += ControlOnTouchUpOutside;
                 this.Control.TouchDown += ControlOnTouchDown;
                 this.Control.OnEnabled += ControlOnOnEnabled;
          }

        }

   
        private void ControlOnOnEnabled(object sender, bool e)
        {
            this.SvgImageButtonController.SendEnabled(e);
        }

        protected override CustomButtonView CreateNativeControl()
        {
           var button = new CustomButtonView();
           return button;
        }

        private void ControlOnTouchDown(object sender, EventArgs e)
        {
            SvgImageButtonController.SendPressed();            
            SvgImageButtonController?.SendClicked();
        }

        private void ControlOnTouchUpInside(object sender, EventArgs e)
        {
            SvgImageButtonController?.SendReleased();
        }

        private void ControlOnTouchUpOutside(object sender, EventArgs e)
        {
            SvgImageButtonController?.SendReleased();
        }

        protected override void Dispose(bool disposing)
        {

            if (disposing)
            {
                if (Control != null)
                {
                    this.Control.TouchUpInside -= ControlOnTouchUpInside;
                    this.Control.TouchDown -= ControlOnTouchDown;
                    this.Control.TouchUpOutside -= ControlOnTouchUpOutside;
                    this.Control.OnEnabled -= ControlOnOnEnabled;

                }
            }
          
            
            base.Dispose(disposing);
        }

        public UIView NativeControl => Control;
    }

    public interface IControlRenderer
    {
        UIView NativeControl { get; }
    }
    
    public interface IButtonView
    {
        event EventHandler<bool> OnEnabled;
        event EventHandler<bool> OnSelected;
        event EventHandler<bool> OnHilighted;
    }
    
    public class CustomButtonView : UIButton, IButtonView
    {
        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                SendEnabled(value);
            }
        }
        
        public override bool Selected 
        { 
            get => base.Selected;
            set 
            {
                base.Enabled = value;
                SendSelected(value);
            }
        }
    
        public override bool Highlighted 
        { 
            get => base.Highlighted;
            set 
            {
                base.Highlighted = value;
                SendHilighted(value);
            }
        }
        
        public event EventHandler<bool> OnEnabled;
        public event EventHandler<bool> OnSelected;
        public event EventHandler<bool> OnHilighted;

        private void SendEnabled(bool enabled)
        {
            OnEnabled?.Invoke(this, enabled);
        }
        
        private void SendSelected(bool selected)
        {
            OnSelected?.Invoke(this, selected);
        }
        
        private void SendHilighted(bool highlighted)
        {
            OnHilighted?.Invoke(this, highlighted);
        }
    }

}