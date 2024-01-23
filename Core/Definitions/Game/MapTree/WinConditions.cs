namespace FEZRepacker.Core.Definitions.Game.MapTree
{
    [XnbType("FezEngine.Structure.WinConditions")]
    [XnbReaderType("FezEngine.Readers.WinConditionsReader")]
    internal class WinConditions
    {
        [XnbProperty]
        public int ChestCount { get; set; }

        [XnbProperty]
        public int LockedDoorCount { get; set; }

        [XnbProperty]
        public int UnlockedDoorCount { get; set; }

        [XnbProperty(UseConverter = true)]
        public List<int> ScriptIds { get; set; }

        [XnbProperty]
        public int CubeShardCount { get; set; }

        [XnbProperty]
        public int OtherCollectibleCount { get; set; }

        [XnbProperty]
        public int SplitUpCount { get; set; }

        [XnbProperty]
        public int SecretCount { get; set; }


        public WinConditions()
        {
            ScriptIds = new List<int>();
        }
    }
}
