using System;
using ChilliSource.Mobile.UI;
using Xamarin.Forms;
using MagicGradients;
using TalkiPlay.Shared;

namespace TalkiPlay
{
    public static class Styles
    {
        
        public static readonly Style PrimaryButtonStyle = new Style(typeof(Button))
        {
            Setters =
            {
                new Setter { Property = Button.FontFamilyProperty, Value = Fonts.ButtonTextFont.Family },
                new Setter { Property = Button.FontSizeProperty, Value = Fonts.ButtonTextFont.Size },
                new Setter { Property = Button.TextColorProperty, Value = Fonts.ButtonTextFont.Color },
                new Setter { Property = Button.CornerRadiusProperty, Value = Dimensions.DefaultCornerRadius  },
                new Setter { Property = VisualElement.BackgroundColorProperty, Value = Colors.TealColor  },
                new Setter { Property = VisualElement.HeightRequestProperty, Value = Dimensions.DefaultButtonHeight  },
            }
        };

        public static readonly Style NewPrimaryButtonStyle = new Style(typeof(Button))
        {
            Setters =
            {
                new Setter { Property = Button.FontFamilyProperty, Value = Fonts.ButtonTextFont.Family },
                new Setter { Property = Button.FontSizeProperty, Value = 19 },
                new Setter { Property = Button.TextColorProperty, Value = Colors.WhiteColor },
                new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.Transparent  },
                
            }
        };

        public static GradientView BuildMainGradient()
        {
            return new GradientView()
            {
                GradientSource = BuildMainGradientSource()
            };
         }

        public static GradientCollection BuildMainGradientSource()
        {
            
            return new GradientCollection
            {
                Gradients =
                {
                    new LinearGradient()
                    {
                        Angle = 0,
                        Stops =
                        {
                            new MagicGradients.GradientStop()
                            {
                                Color = Color.FromHex("18EAD9"),
                                Offset = new Offset(0, OffsetType.Proportional)
                                //Offset = 0
                            },
                            new MagicGradients.GradientStop()
                            {
                                Color = Color.FromHex("5e7aea"),
                                Offset = new Offset(1, OffsetType.Proportional)
                                //Offset = 1
                            }
                        }
                    },
                    new LinearGradient()
                    {
                        Angle = -45,
                        Stops =
                        {
                            new MagicGradients.GradientStop()
                            {
                                Color = Color.FromHex("3f18EAD9"),// "43A6E3"),
                                Offset = new Offset(0, OffsetType.Proportional)
                                //Offset = 0f
                            },
                            new MagicGradients.GradientStop()
                            {
                                Color = Color.FromHex("3f5e7aea"),
                                Offset = new Offset(1, OffsetType.Proportional)
                                //Offset = 1
                            }
                        }
                    }
                         
                }
            };
        }               
    }
}
