using System;

namespace TalkiPlay.Shared
{
    public struct AudioPlayerSetting
    {
        public AudioPlayerSetting(string filePath, float volume = 0.5f, int numberOfLoops = 0)
        {

            Ensure.ArgumentNotNull(filePath, nameof(filePath));

            FilePath = filePath;
            Volume = volume;
            NumberOfLoops = numberOfLoops;
        }

        public string FilePath { get; }
        public float Volume { get; }
        public int NumberOfLoops { get; }
        public bool IsEmpty => String.IsNullOrWhiteSpace(FilePath);
        public static AudioPlayerSetting Empty => new AudioPlayerSetting("");
    }
}