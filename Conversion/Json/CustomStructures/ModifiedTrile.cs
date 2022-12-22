using FezEngine;
using FezEngine.Structure;
using FezEngine.Structure.Scripting;
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
        public ModifiedTrileInstanceSettings? Settings { get; set; }

        private static FaceOrientation[] OrientationLookup = new FaceOrientation[4]
		{
			FaceOrientation.Back,
			FaceOrientation.Left,
			FaceOrientation.Front,
			FaceOrientation.Right
        };

        public ModifiedTrile()
        {
            Position = new();
        }

        public ModifiedTrile(TrileEmplacement position, TrileInstance instance)
        {
            Position = position;
            Id = instance.TrileId;
            Orientation = OrientationLookup[instance.PhiLight];
            Settings = new ModifiedTrileInstanceSettings(position, instance);
            if (Settings.IsUnnecessary()) Settings = null;
        }

        public TrileInstance ToOriginal()
        {
            TrileInstance instance = new TrileInstance();

            instance.Position = new Vector3(Position.X, Position.Y, Position.Z);
            instance.PhiLight = (byte)Array.IndexOf(OrientationLookup, Orientation);
            instance.TrileId = Id;

            if (Settings != null)
            {
                instance.Position += Settings.Offset;
                instance.ActorSettings = Settings.ToOriginal();
            }

            return instance;
        }
    }

    class ModifiedTrileInstanceSettings
    {
        public Vector3 Offset { get; set; }

        // Assuming it's unused, haven't seen it used anywhere in the
        // decompiled source code, and in all maps it has a value of null.
        //public int? ContainedTrile { get; set; } 

        public string SignText { get; set; }
        public bool[] Sequence { get; set; }
        public string SequenceSampleName { get; set; }
        public string SequenceAlternateSampleName { get; set; }
        public int? HostVolume { get; set; }

        public ModifiedTrileInstanceSettings()
        {
            SignText = "";
            Sequence = new bool[0];
            SequenceSampleName = "";
            SequenceAlternateSampleName = "";
        }

        public ModifiedTrileInstanceSettings(TrileEmplacement position, TrileInstance instance)
        {
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
