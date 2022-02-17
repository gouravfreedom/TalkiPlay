using System;

namespace TalkiPlay.Shared
{
    public class SoftwareVersion : IComparable<SoftwareVersion>
    {
        
        public SoftwareVersion(int major, int minor, int patch, int revision)
        {
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
            this.Revision = revision;
        }
        
        public int Major { get; set; }
        
        public int Minor { get; set; }
        
        public int Patch { get; set; }
        
        public int Revision { get; set; }

        public int CompareTo(SoftwareVersion other)
        {
            if (Major == other.Major)
            {
                if (Minor == other.Minor)
                {
                    if (Patch == other.Patch)
                    {
                        return Revision.CompareTo(other.Revision);
                    }

                    return Patch.CompareTo(other.Patch);
                }

                return Minor.CompareTo(other.Minor);
            }

            return Major.CompareTo(other.Major);
        }
    }
    
    public static class VersionStringExtensions
    {

        public static SoftwareVersion ToSoftwareVersion(this string version)
        {
            if(!String.IsNullOrWhiteSpace(version))
            {
                var versions = version.Split('.');
                return new SoftwareVersion(versions[0].ToNumber(), versions[1].ToNumber(), versions[2].ToNumber(), versions[3].ToNumber());
            }

            return new SoftwareVersion(0, 0, 0, 0);
        }

        public static int ToNumber(this string source)
        {
            int i = 0;

            if (int.TryParse(source, out i))
            {
                return i;
            }
            return i;
        }
    }
}