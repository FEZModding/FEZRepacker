using System.Numerics;

using FEZRepacker.Converter.Definitions.MicrosoftXna;

namespace FEZRepacker.Converter.XNB.Formats.Json.CustomStructures
{
    internal class SpriteFontJsonData
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

        public SpriteFontJsonData()
        {
            Characters = new List<CharacterData>();
        }

        public static SpriteFontJsonData FromSpriteFont(SpriteFont font)
        {
            var data = new SpriteFontJsonData();
            
            data.LineSpacing = font.LineSpacing;
            data.Spacing = font.Spacing;

            for(int i=0;i<font.Characters.Count;i++)
            {
                var character = new CharacterData();
                character.Character = font.Characters[i];
                character.GlyphBounds = font.GlyphBounds[i];
                character.Cropping = font.Cropping[i];
                character.KerningData = font.KerningData[i];

                data.Characters.Add(character);
            }

            return data;
        }

        public void ToSpriteFont(ref SpriteFont font)
        {
            font.LineSpacing = this.LineSpacing;
            font.Spacing = this.Spacing;

            foreach(var character in this.Characters)
            {
                font.Characters.Add(character.Character);
                font.GlyphBounds.Add(character.GlyphBounds);
                font.Cropping.Add(character.Cropping);
                font.KerningData.Add(character.KerningData);
            }
        }
    }
}
