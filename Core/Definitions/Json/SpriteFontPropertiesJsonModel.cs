

using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Definitions.Json
{
    internal class SpriteFontPropertiesJsonModel : JsonModel<SpriteFont>
    {
        public class CharacterData
        {
            public char Character;
            public Rectangle GlyphBounds;
            public Rectangle Cropping;
            public Vector3 KerningData;
        }

        public int LineSpacing { get; set; }
        public float Spacing { get; set; }
        public List<CharacterData> Characters { get; set; }

        public SpriteFontPropertiesJsonModel()
        {
            Characters = new();
        }

        public SpriteFontPropertiesJsonModel(SpriteFont spriteFont) : this()
        {
            SerializeFrom(spriteFont);
        }

        public SpriteFont Deserialize()
        {
            var font = new SpriteFont();

            font.LineSpacing = this.LineSpacing;
            font.Spacing = this.Spacing;

            foreach (var character in this.Characters)
            {
                font.Characters.Add(character.Character);
                font.GlyphBounds.Add(character.GlyphBounds);
                font.Cropping.Add(character.Cropping);
                font.KerningData.Add(character.KerningData);
            }

            return font;
        }

        public void SerializeFrom(SpriteFont font)
        {
            LineSpacing = font.LineSpacing;
            Spacing = font.Spacing;

            for (int i = 0; i < font.Characters.Count; i++)
            {
                var character = new CharacterData();
                character.Character = font.Characters[i];
                character.GlyphBounds = font.GlyphBounds[i];
                character.Cropping = font.Cropping[i];
                character.KerningData = font.KerningData[i];

                Characters.Add(character);
            }
        }
    }
}
