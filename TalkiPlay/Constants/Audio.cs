namespace TalkiPlay.Shared
{
    public static class Audio
    {
        public const string AudioFolder = "audio/";
        public const string HuntMusic = AudioFolder + "hunt.mp3";
        public const string ExploreMusic = AudioFolder + "explore.mp3";
        
        #if __IOS__
        public const string ConfettiSound = AudioFolder + "confetti.mp3";
        public const string CupSound = AudioFolder + "cup.mp3";
        public const string PuzzlePieceSound = AudioFolder + "puzzle_piece.mp3";
        #endif
        
        #if __ANDROID__
        public static readonly string ConfettiSound = Resource.Raw.confetti.ToString();
        public static readonly string CupSound = Resource.Raw.cup.ToString();
        public static readonly string PuzzlePieceSound = Resource.Raw.puzzle_piece.ToString();
        #endif
        
    }
}