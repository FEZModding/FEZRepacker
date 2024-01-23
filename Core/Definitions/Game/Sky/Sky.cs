namespace FEZRepacker.Core.Definitions.Game.Sky 
{ 
    [XnbType("FezEngine.Structure.Sky")]
    [XnbReaderType("FezEngine.Readers.SkyReader")]
    public class Sky
    {
        [XnbProperty]
        public string Name { get; set; } = "";

        [XnbProperty]
        public string Background { get; set; } = "";

        [XnbProperty]
        public float WindSpeed { get; set; }

        [XnbProperty]
        public float Density { get; set; }

        [XnbProperty]
        public float FogDensity { get; set; }

        [XnbProperty(UseConverter = true)]
        public List<SkyLayer> Layers { get; set; } = new();

        [XnbProperty(UseConverter = true)]
        public List<string> Clouds { get; set; } = new();

        [XnbProperty(UseConverter = true)]
        public string Shadows { get; set; } = "";

        [XnbProperty(UseConverter = true)]
        public string Stars { get; set; } = "";

        [XnbProperty(UseConverter = true)]
        public string CloudTint { get; set; } = "";

        [XnbProperty]
        public bool VerticalTiling { get; set; }

        [XnbProperty]
        public bool HorizontalScrolling { get; set; }

        [XnbProperty]
        public float LayerBaseHeight { get; set; }

        [XnbProperty]
        public float InterLayerVerticalDistance { get; set; }

        [XnbProperty]
        public float InterLayerHorizontalDistance { get; set; }

        [XnbProperty]
        public float HorizontalDistance { get; set; }

        [XnbProperty]
        public float VerticalDistance { get; set; }

        [XnbProperty]
        public float LayerBaseSpacing { get; set; }

        [XnbProperty]
        public float WindParallax { get; set; }

        [XnbProperty]
        public float WindDistance { get; set; }

        [XnbProperty]
        public float CloudsParallax { get; set; }

        [XnbProperty]
        public float ShadowOpacity { get; set; }

        [XnbProperty]
        public bool FoliageShadows { get; set; }

        [XnbProperty]
        public bool NoPerFaceLayerXOffset { get; set; }

        [XnbProperty]
        public float LayerBaseXOffset { get; set; }
    }
}
