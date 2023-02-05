namespace FEZRepacker.Converter.Definitions.MicrosoftXna
{
    [XnbType("Microsoft.Xna.Framework.Audio.SoundEffect")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.SoundEffectReader")]
    internal class SoundEffect
    {
        // Definition of this class is nothing alike what's actually in the game.
        // Stored data is extremely similar to WAV format and its chunks,
        // so I'm just storing raw byte arrays of chunks in most cases.

        [XnbProperty(UseConverter = true, SkipIdentifier = true)]
        public byte[] FormatChunk { get; set; }

        [XnbProperty(UseConverter = true, SkipIdentifier = true)]
        public byte[] DataChunk { get; set; }

        [XnbProperty]
        public int LoopStart { get; set; }

        [XnbProperty]
        public int LoopLength { get; set; }

        [XnbProperty]
        public int UnknownValue { get; set; }


        public SoundEffect(){
            FormatChunk = new byte[0];
            DataChunk = new byte[0];
        }
    }
}
