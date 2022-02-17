using ChilliSource.Mobile.UI;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
  

    public static class Fonts
    {
        #region Fonts
#if __IOS__

        public const string FontFamilyRoundedBold = "ArialRoundedMTBold";
        public const string FontFamilyRounded = "ArialRoundedMT";
        public const string FontFamilyRegular = "Avenir-Roman";
        public const string FontFamilyBlack = "Avenir-Black";
        public const string FontFamilyRegularLight = "HelveticaNeue";
        public const string FontFamilyMedium = "Avenir-Medium";
        
        public static ExtendedFont ButtonTextLargeFont = new ExtendedFont(FontFamilyRegular, size:28, color: Colors.WhiteColor);
        public static ExtendedFont ButtonTextFont = new ExtendedFont(FontFamilyRegular, size:20, color: Colors.WhiteColor);
        public static ExtendedFont ButtonTextFontBold = new ExtendedFont(FontFamilyRoundedBold, size: 20, color: Colors.WhiteColor);
        public static ExtendedFont ButtonTextFontBoldBig = new ExtendedFont(FontFamilyRoundedBold, size: 30, color: Colors.WhiteColor);
        public static ExtendedFont ButtonTealTextFont = new ExtendedFont(FontFamilyRegular, size:20, color: Colors.TealColor);
        public static ExtendedFont ButtonTextRedFont = new ExtendedFont(FontFamilyRegular, size:20, color: Colors.Red);
        public static ExtendedFont ButtonTextGreyFont = new ExtendedFont(FontFamilyRegular, size:20, color: Colors.WarmGrey);
        public static ExtendedFont ButtonTextDarkGreyFont = new ExtendedFont(FontFamilyRegular, size: 20, color: Colors.SlateGrey);

#endif

#if __ANDROID__
        public const string FontFamilyRoundedBold = "Roboto-Bold";
        public const string FontFamilyRounded = "Roboto-Regular";
        public const string FontFamilyRegular = "Roboto-Regular";
        public const string FontFamilyBlack = "Roboto-Black";
        public const string FontFamilyRegularLight = "Roboto-Light";
        public const string FontFamilyMedium = "Roboto-Medium";
        
        public static ExtendedFont ButtonTextLargeFont = new ExtendedFont(FontFamilyRegular, size:20, color: Colors.WhiteColor);
        public static ExtendedFont ButtonTextFont = new ExtendedFont(FontFamilyRegular, size:17, color: Colors.WhiteColor);
        public static ExtendedFont ButtonTextFontBold = new ExtendedFont(FontFamilyRoundedBold, size:17, color: Colors.WhiteColor);
        public static ExtendedFont ButtonTextFontBoldBig = new ExtendedFont(FontFamilyRoundedBold, size: 24, color: Colors.WhiteColor);
        public static ExtendedFont ButtonTextRedFont = new ExtendedFont(FontFamilyRegular, size:17, color: Colors.Red);
        public static ExtendedFont ButtonTextGreyFont = new ExtendedFont(FontFamilyRegular, size:17, color: Colors.WarmGrey);
        public static ExtendedFont ButtonTextDarkGreyFont = new ExtendedFont(FontFamilyRegular, size: 17, color: Colors.SlateGrey);
       public static ExtendedFont ButtonTealTextFont = new ExtendedFont(FontFamilyRegular, size:17, color: Colors.TealColor);

#endif

        public static ExtendedFont HeroTitleFont = new ExtendedFont(FontFamilyRoundedBold, size: 34, color: Colors.WhiteColor);
        public static ExtendedFont HeroTitle1Font = new ExtendedFont(FontFamilyRoundedBold, size: 23, color: Colors.WhiteColor);
        public static ExtendedFont TitleRegularFontBig = new ExtendedFont(FontFamilyRegular, size: 25, color: Colors.WhiteColor);
        public static ExtendedFont Underline1FontBig = new ExtendedFont(FontFamilyRegular, size: 24, color: Colors.WhiteColor, 0, 0, true);
        public static ExtendedFont TitleFont = new ExtendedFont(FontFamilyRegular, size:13, color: Colors.WhiteColor);
        public static ExtendedFont HeaderFont = new ExtendedFont(FontFamilyRoundedBold, size:27, color: Colors.WhiteColor);
        public static ExtendedFont Header1Font = new ExtendedFont(FontFamilyRoundedBold, size:20, color: Colors.WhiteColor);
        public static ExtendedFont Header1FontBig = new ExtendedFont(FontFamilyRoundedBold, size: 24, color: Colors.WhiteColor);
        public static ExtendedFont Header1WhiteRegularFont = new ExtendedFont(FontFamilyRegular, size:20, color: Colors.WhiteColor);
        public static ExtendedFont Header1BlackFont = new ExtendedFont(FontFamilyRoundedBold, size:20, color:Colors.Black);
        public static ExtendedFont Header1BlueFont = new ExtendedFont(FontFamilyRoundedBold, size: 20, color: Colors.Blue2Color);
        public static ExtendedFont Header2Font = new ExtendedFont(FontFamilyRoundedBold, size:17, color:Colors.WhiteColor);
        public static ExtendedFont Header2BlueFont = new ExtendedFont(FontFamilyRoundedBold, size: 17, color: Colors.Blue2Color);
        public static ExtendedFont Header2DarkTealFont = new ExtendedFont(FontFamilyRoundedBold, size: 17, color: Colors.DarkTealColor);
        public static ExtendedFont Header3Font = new ExtendedFont(FontFamilyRoundedBold, size: 14, color: Colors.WhiteColor);
        public static ExtendedFont Header3BlueFont = new ExtendedFont(FontFamilyRoundedBold, size: 14, color: Colors.Blue2Color);
        public static ExtendedFont HeaderYellowFont = new ExtendedFont(FontFamilyRoundedBold, size: 27, color: Colors.Yellow2Color);
        public static ExtendedFont ItalicWhiteFontSmall = new ExtendedFont(size: 14, color: Color.FromHex("#BCE4F6"), fontAttributes: FontAttributes.Italic);
        public static ExtendedFont ItalicWhiteFontBig = new ExtendedFont(size: 20, color: Color.FromHex("#BCE4F6"), fontAttributes: FontAttributes.Italic);
        public static ExtendedFont ItalicWhiteFontBigger = new ExtendedFont(size: 27, color: Color.FromHex("#BCE4F6"), fontAttributes: FontAttributes.Italic);

        //public static ExtendedFont QuestionBlackFont = new ExtendedFont(FontFamilyRoundedBold, size:25, color:Colors.Black);

        public static ExtendedFont LabelWhiteFont = new ExtendedFont(FontFamilyRegular, size:17, color:Colors.WhiteColor);
        public static ExtendedFont LabelBlackFont = new ExtendedFont(FontFamilyRegular, size:17, color:Colors.Black);
      
        public static ExtendedFont NavigationTitleFont  = new ExtendedFont(FontFamilyBlack, size:17, color:Colors.WhiteColor);
        //public static ExtendedFont NavigationTitleBlackFont  = new ExtendedFont(FontFamilyBlack, size:13, color:Colors.Black);
        //public static ExtendedFont BadgeTextBlackFont  = new ExtendedFont(FontFamilyBlack, size:13, color:Colors.Black);
        public static ExtendedFont BadgeTextFont  = new ExtendedFont(FontFamilyBlack, size:13, color:Colors.WhiteColor);

        //public static ExtendedFont DialogHeaderFont = new ExtendedFont(FontFamilyRoundedBold, size:17, color: Colors.WarmGrey);
        //public static ExtendedFont DialogSubtitleFont = new ExtendedFont(FontFamilyRegular, size: 13, color: Colors.SlateGrey, kerning: -0.1f);


        //public static ExtendedFont BodyStrongReverseFont = new ExtendedFont(FontFamilyMedium, size:17, color:Colors.WhiteColor);
        //public static ExtendedFont BodyStrongFont = new ExtendedFont(FontFamilyMedium, size:17, color:Colors.WarmGrey);
        public static ExtendedFont BodyLinkFont = new ExtendedFont(FontFamilyRegular, size:17, color:Colors.Black);
        public static ExtendedFont BodyLinkReverseFont = new ExtendedFont(FontFamilyRegular, size:17, color:Colors.WhiteColor);
        public static ExtendedFont BodyFont = new ExtendedFont(FontFamilyRegularLight, size:17, color:Colors.Black);
        public static ExtendedFont BodyBlueRoundedFont = new ExtendedFont(FontFamilyRounded, size: 17, color: Colors.Blue2Color);
        public static ExtendedFont BodyWhiteFont = new ExtendedFont(FontFamilyRegularLight, size:17, color:Colors.WhiteColor);
        public static ExtendedFont BodyPlaceHolderFont = new ExtendedFont(FontFamilyRegularLight, size:17, color:Colors.PinkishGrey);
        //public static ExtendedFont BodyPlaceHolderDarkFont = new ExtendedFont(FontFamilyRegularLight, size: 17, color: Colors.WarmGrey);
        //public static ExtendedFont BodyPlaceHolderWhiteFont = new ExtendedFont(FontFamilyRegularLight, size:17, color:Colors.WhiteColor);
        //public static ExtendedFont BodyRedFont = new ExtendedFont(FontFamilyRegularLight, size:17, color:Colors.Red);
        public static readonly ExtendedFont LinkWhiteFont =
            new ExtendedFont(Fonts.FontFamilyRegular, size: 16, color: Colors.WhiteColor,
                isUnderlined: true);

        
        public static ExtendedFont MetaLinkBlackFont = new ExtendedFont(FontFamilyRegular, size: 13, color: Colors.Black);
        //public static ExtendedFont MetaStrongFont  = new ExtendedFont(FontFamilyRoundedBold, size:13, color:Colors.SlateGrey);
        public static ExtendedFont MetaLinkFont  = new ExtendedFont(FontFamilyRegular, size:13, color:Colors.Black);
        public static ExtendedFont MetaLinkWhiteFont = new ExtendedFont(FontFamilyRegular, size: 13, color: Colors.WhiteColor);        
        public static ExtendedFont MetaLinkFusiaFont  = new ExtendedFont(FontFamilyRegular, size:13, color:Colors.Fuschia);        
        //public static ExtendedFont MetaFont  = new ExtendedFont(FontFamilyRegularLight, size:13, color:Colors.SlateGrey);
        //public static ExtendedFont MetaGreyFont  = new ExtendedFont(FontFamilyRegularLight, size:13, color:Colors.WarmGrey);
        public static ExtendedFont MetaErrorFont  = new ExtendedFont(FontFamilyRegularLight, size:13, color:Colors.Red);
        public static ExtendedFont MetaSuccessFont  = new ExtendedFont(FontFamilyRegularLight, size:13, color:Colors.SuccessColor);
        
        
        #endregion
    }

    public static class Icons
    {
        // public static readonly Icon SquareIcon = new Icon("far-square", Color.White, 24);
        // public static readonly Icon CheckedSquareIcon = new Icon("far-check-square", Color.White, 24);
        //
        public static readonly FontImageSource SquareIcon = new FontImageSource
        {
            Glyph = "\uf0c8", FontFamily = "FontAwesomeRegular", Color = Color.White, Size = 24
        };
        
        public static readonly FontImageSource CheckedSquareIcon = new FontImageSource
        {
            Glyph = "\uf14a", FontFamily = "FontAwesomeRegular", Color = Color.White, Size = 24
        };
        
        public static readonly FontImageSource LargeGoldStarIcon = new FontImageSource
        {
            Glyph = "\uf005", FontFamily = "FontAwesomeSolid", Color = Color.FromHex("F5C342"), Size = 60
        };
        
        public static readonly FontImageSource SmallGoldStarIcon = new FontImageSource
        {
            Glyph = "\uf005", FontFamily = "FontAwesomeSolid", Color = Color.FromHex("F5C342"), Size = 30
        };
        
        public static readonly FontImageSource SmallGrayStarIcon = new FontImageSource
        {
            Glyph = "\uf005", FontFamily = "FontAwesomeSolid", Color = Color.FromHex("D3CFD0"), Size = 30
        };
    }

    public static class Converters
    {
        public static BooleanToObjectConverter<ExtendedFont> BooleanToFontConverter = new BooleanToObjectConverter<ExtendedFont>()
        {
            TrueObject = Fonts.MetaErrorFont,
            FalseObject = Fonts.MetaSuccessFont
        };

        public static BooleanToObjectConverter<Color> BooleanToColorConverter = new BooleanToObjectConverter<Color>()
        {
            TrueObject = Colors.SuccessColor,
            FalseObject = Colors.WhiteColor
        };

     
        public static BooleanToObjectConverter<Color> BooleanToAvatarSelectionColorConverter = new BooleanToObjectConverter<Color>()
        {
            TrueObject = Color.Black,
            FalseObject = Color.White
        };

        public static BooleanToObjectConverter<Color> BooleanToAvatarSelectionNavColorConverter = new BooleanToObjectConverter<Color>()
        {
            TrueObject = Color.Transparent,
            FalseObject = Colors.DarkTealColor
        };
        
        public static BooleanToObjectConverter<FontImageSource> BooleanToStarIconConverter = new BooleanToObjectConverter<FontImageSource>()
        {
            TrueObject = Icons.SmallGoldStarIcon,
            FalseObject = Icons.SmallGrayStarIcon
        };
        
        public static InvertedBooleanConverter InvertedBooleanConverter = new InvertedBooleanConverter();

        public static EmbRsrcToImageConverter EmbRsrc2ImageConverter = new EmbRsrcToImageConverter();

        public static WidthToHeightByImageRatio Width2HeightByImageRatio = new WidthToHeightByImageRatio();
        public static ChildToAvatarConverter Child2AvatarConverter = new ChildToAvatarConverter();        
    }
}