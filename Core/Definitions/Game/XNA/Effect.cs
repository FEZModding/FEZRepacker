namespace FEZRepacker.Core.Definitions.Game.XNA
{
    [XnbType("Microsoft.Xna.Framework.Graphics.Effect, Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.EffectReader, Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553")]
    public class Effect
    {
        // Definition of this class is nothing alike what's actually in the game.
        // Stored data is simply a raw FXB file.

        [XnbProperty(UseConverter = true, SkipIdentifier = true)]
        public byte[] Data { get; set; } = { };
    }
}
