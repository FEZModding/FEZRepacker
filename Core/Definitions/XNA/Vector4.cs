namespace FEZRepacker.Converter.Definitions.MicrosoftXna
{
    [XnbType("Microsoft.Xna.Framework.Vector4")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.Vector4Reader")]
    internal class Vector4
    {
        [XnbProperty]
        public float W { get; set; }

        [XnbProperty]
        public float X { get; set; }

        [XnbProperty]
        public float Y { get; set; }

        [XnbProperty]
        public float Z { get; set; }
    }
}
