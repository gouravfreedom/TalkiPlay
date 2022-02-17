using Xamarin.Forms;

namespace TalkiPlay
{
    public class PressableView : ContentView
    {        
        public virtual void OnPressed(bool pressed)
        {
            if (pressed)
            {
                Opacity = 0.5;
            }
            else
            {
                this.FadeTo(1);
            }
        }
    }
}