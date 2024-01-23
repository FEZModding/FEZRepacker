
using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.MapTree;

namespace FEZRepacker.Core.Definitions.Json
{
    public class MapNodeConnectionJsonModel : JsonModel<MapNodeConnection>
    {
        public FaceOrientation Face { get; set; }
        public int Node { get; set; }
        public float BranchOversize { get; set; }

        public MapNodeConnectionJsonModel()
        {

        }

        public MapNodeConnectionJsonModel(MapNodeConnection data) : this()
        {
            SerializeFrom(data);
        }

        public MapNodeConnection Deserialize()
        {
            return new()
            {
                Face = Face,
                BranchOversize = BranchOversize
            };
        }

        public void SerializeFrom(MapNodeConnection data)
        {
            Face = data.Face;
            BranchOversize = data.BranchOversize;
        }
    }
}
