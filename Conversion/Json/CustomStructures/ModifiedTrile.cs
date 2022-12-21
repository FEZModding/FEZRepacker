using FEZEngine;
using FEZEngine.Structure;
using FEZEngine.Structure.Scripting;
using System.Numerics;

namespace FEZRepacker.Conversion.Json.CustomStructures
{
    // Isolate actor settings and provide only Id and Phi value,
    // since in most cases only these are actually changing
    class ModifiedTrile
    {
        public TrileEmplacement Position { get; set; }
        public FaceOrientation Orientation { get; set; }
        public int Id { get; set; }

        private static FaceOrientation[] OrientationLookup = new FaceOrientation[4]
		{
			FaceOrientation.Back,
			FaceOrientation.Left,
			FaceOrientation.Front,
			FaceOrientation.Right
        };

        public ModifiedTrile(TrileEmplacement position, TrileInstance instance)
        {
            Position = position;
            Id = instance.TrileId;
            Orientation = OrientationLookup[instance.PhiLight];
        }

        public TrileInstance ToOriginal(ModifiedTrileInstanceSettings? settings)
        {
            TrileInstance instance = new TrileInstance();

            instance.Position = new Vector3(Position.X, Position.Y, Position.Z);
            instance.PhiLight = (byte)Orientation;
            instance.TrileId = Id;

            if (settings != null)
            {
                instance.Position += settings.Offset;
                instance.ActorSettings = settings.ToOriginal();
            }

            return instance;
        }
    }

    class ModifiedTrileInstanceSettings
    {
        public TrileEmplacement Position { get; set; }
        public Vector3 Offset { get; set; }

        // Assuming it's unused, haven't seen it used anywhere in the
        // decompiled source code, and in all maps it has a value of null.
        //public int? ContainedTrile { get; set; } 

        public string SignText { get; set; }
        public bool[] Sequence { get; set; }
        public string SequenceSampleName { get; set; }
        public string SequenceAlternateSampleName { get; set; }
        public int? HostVolume { get; set; }

        public ModifiedTrileInstanceSettings(TrileEmplacement position, TrileInstance instance)
        {
            Position = position;
            Vector3 trilePos = new Vector3(position.X, position.Y, position.Z);
            Offset = instance.Position - trilePos;

            if (instance.ActorSettings != null)
            {
                SignText = instance.ActorSettings.SignText;
                Sequence = instance.ActorSettings.Sequence;
                SequenceSampleName = instance.ActorSettings.SequenceSampleName;
                SequenceAlternateSampleName = instance.ActorSettings.SequenceAlternateSampleName;
            }
            else
            {
                SignText = "";
                Sequence = new bool[0];
                SequenceSampleName = "";
                SequenceAlternateSampleName = "";
            }
        }

        public bool IsUnnecessary(bool includingOffset = false)
        {
            return 
                (Offset.Length() == 0 || !includingOffset) && 
                SignText.Length == 0 &&
                Sequence.Length == 0 &&
                SequenceSampleName.Length == 0 &&
                SequenceAlternateSampleName.Length == 0 &&
                !HostVolume.HasValue;
        }

        public TrileInstanceActorSettings? ToOriginal()
        {
            if (!IsUnnecessary())
            {
                var actorSettings = new TrileInstanceActorSettings();

                actorSettings.SignText = SignText;
                actorSettings.Sequence = Sequence;
                actorSettings.SequenceSampleName = SequenceSampleName;
                actorSettings.SequenceAlternateSampleName = SequenceAlternateSampleName;
                actorSettings.HostVolume = HostVolume;

                return actorSettings;
            }
            else return null;
        }
    }
}
