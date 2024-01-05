namespace FEZRepacker.Converter.Definitions.MicrosoftXna
{
    [XnbType("Microsoft.Xna.Framework.Audio.SoundEffect")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.SoundEffectReader")]
    internal class SoundEffect
    {
        // Definition of this class is nothing alike what's actually in the game.
        // Stored data is extremely similar to WAV format and its chunks,
        // so I'm just storing raw byte arrays of chunks in most cases.

        [XnbProperty]
        public int FormatChunkSize { get; set; }

        [XnbProperty]
        public short FormatType { get; set; }

        [XnbProperty]
        public short ChannelCount { get; set; }

        [XnbProperty]
        public int SampleFrequency { get; set; }

        [XnbProperty]
        public int BytesPerSecond { get; set; }

        [XnbProperty]
        public short BlockAlignment { get; set; }

        [XnbProperty]
        public short BitsPerSample { get; set; }

        // normally this does not occur in PCM, but XNA's reader expects it.
        [XnbProperty]
        public short ExtraParameter { get; set; }

        [XnbProperty(UseConverter = true, SkipIdentifier = true)]
        public byte[] DataChunk { get; set; }

        [XnbProperty]
        public int LoopStart { get; set; }

        [XnbProperty]
        public int LoopLength { get; set; }

        [XnbProperty]
        public int UnknownValue { get; set; }


        public SoundEffect()
        {
            DataChunk = new byte[0];
        }
    }
}
