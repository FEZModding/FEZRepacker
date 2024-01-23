
using FEZRepacker.Core.Definitions.Game.Level;
using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Definitions.Json
{
    public class TrileInstanceJsonModel : JsonModel<TrileInstance>
    {
        public TrileEmplacement Emplacement { get; set; }
        public Vector3 Position { get; set; }
        public byte Phi { get; set; }
        public int Id { get; set; }
        public TrileInstanceActorSettings? ActorSettings { get; set; }

        public TrileInstanceJsonModel()
        {

        }

        public TrileInstanceJsonModel(TrileEmplacement emplacement) : this()
        {
            Emplacement = emplacement;
        }

        public TrileInstanceJsonModel(TrileEmplacement emplacement, TrileInstance instance) : this(emplacement)
        { 
            SerializeFrom(instance);
        }

        public TrileInstance Deserialize()
        {
            return new()
            {
                Position = Position,
                PhiLight = Phi,
                TrileId = Id,
                ActorSettings = ActorSettings
            };
        }

        public void SerializeFrom(TrileInstance data)
        {
            Position = data.Position;
            Phi = data.PhiLight;
            Id = data.TrileId;
            ActorSettings = data.ActorSettings;
        }
    }
}
