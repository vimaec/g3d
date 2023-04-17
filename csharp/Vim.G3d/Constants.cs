namespace Vim.G3d
{
    public static class Constants
    {
        public const string G3dPrefix = "g3d";
        public const string Separator = ":";
        public const char SeparatorChar = ':';

        public const string MetaHeaderSegmentName = "meta";
        public const long MetaHeaderSegmentNumBytes = 8; // The header is 7 bytes + 1 bytes padding.
        public const byte MetaHeaderMagicA = 0x63;
        public const byte MetaHeaderMagicB = 0xD0;

        public static readonly string[] MetaHeaderSupportedUnits = { "mm", "cm", "m\0", "km", "in", "ft", "yd", "mi" };
    }
}
