namespace FEZRepacker.Core.Definitions.Game.XNA
{
    [XnbType("Microsoft.Xna.Framework.Graphics.Effect")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.EffectReader")]
    internal class Effect
    {
        // Definition of this class is nothing alike what's actually in the game.
        // Stored data is simply a raw FXB file.

        [XnbProperty(UseConverter = true, SkipIdentifier = true)]
        public byte[] Data { get; set; }

        public Effect()
        {
            Data = new byte[0];
        }
    }
}
