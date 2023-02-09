using FEZRepacker.Converter.Definitions.FezEngine;
using FEZRepacker.Converter.Definitions.FezEngine.Structure;
using System.Numerics;

namespace FEZRepacker.Converter.XNB.Formats.Json.CustomStructures
{
    // Isolate actor settings and provide only Id and Phi value,
    // since in most cases only these are actually changing
    internal class ModifiedTrile
    {
        public TrileEmplacement Emplacement { get; set; }
        public Vector3 Position { get; set; }
        public byte Phi { get; set; }
        public int Id { get; set; }
        public TrileInstanceActorSettings? ActorSettings { get; set; }

        public ModifiedTrile()
        {
            Position = new();
        }

        public ModifiedTrile(TrileEmplacement position, TrileInstance instance)
        {
            Emplacement = position;
            Position = instance.Position;
            Id = instance.TrileId;
            Phi = instance.PhiLight;
            ActorSettings = instance.ActorSettings;
        }

        public TrileInstance ToOriginal()
        {
            TrileInstance instance = new TrileInstance();

            instance.Position = Position;
            instance.PhiLight = Phi;
            instance.TrileId = Id;
            instance.ActorSettings = ActorSettings;

            return instance;
        }
    }
}
