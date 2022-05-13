namespace FEZRepacker
{
    static class XNBConstants
    {
        public const string IDENTIFIER = "XNB";
        public const byte VERSION = 5;

        public const byte FLAG_HIDEF = 0x01;
        public const byte FLAG_COMPRESSED = 0x80;

        public const char PLATFORM_WINDOWS = 'w';
        public const char PLATFORM_MOBILE = 'm';
        public const char PLATFORM_XBOX = 'x';

        public const int HEADER_SIZE = 10;

        public const int COMPRESSED_PROLOGUE_SIZE = 14;
    }
}
