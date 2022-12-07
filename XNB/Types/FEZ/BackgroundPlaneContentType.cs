using FEZEngine;
using FEZEngine.Structure;
using FEZRepacker.Dependencies;
using System.Numerics;

namespace FEZRepacker.XNB.Types.FEZ
{
    class BackgroundPlaneContentType : XNBContentType<BackgroundPlane>
    {
        public BackgroundPlaneContentType(XNBContentConverter converter) : base(converter) { }

        public override FEZAssemblyQualifier Name => "FezEngine.Readers.BackgroundPlaneReader";

        public override object Read(BinaryReader reader)
        {
            BackgroundPlane bgPlane = new BackgroundPlane();

            bgPlane.Position = reader.ReadVector3();
            bgPlane.Rotation = reader.ReadQuaternion();
            bgPlane.Scale = reader.ReadVector3();
            bgPlane.Size = reader.ReadVector3();
            bgPlane.TextureName = reader.ReadString();
            bgPlane.LightMap = reader.ReadBoolean();
            bgPlane.AllowOverbrightness = reader.ReadBoolean();
            bgPlane.Filter = reader.ReadColor();
            bgPlane.Animated = reader.ReadBoolean();
            bgPlane.Doublesided = reader.ReadBoolean();
            bgPlane.Opacity = reader.ReadSingle();
            if(reader.ReadBoolean()) bgPlane.AttachedGroup = reader.ReadInt32();
            bgPlane.Billboard = reader.ReadBoolean();
            bgPlane.SyncWithSamples = reader.ReadBoolean();
            bgPlane.Crosshatch = reader.ReadBoolean();
            // no idea what it's for. It's in the original reader, so I'm leaving it here
            // perhaps a deleted flag?
            reader.ReadBoolean(); 
            bgPlane.AlwaysOnTop = reader.ReadBoolean();
            bgPlane.Fullbright = reader.ReadBoolean();
            bgPlane.PixelatedLightmap = reader.ReadBoolean();
            bgPlane.XTextureRepeat = reader.ReadBoolean();
            bgPlane.YTextureRepeat = reader.ReadBoolean();
            bgPlane.ClampTexture = reader.ReadBoolean();
            bgPlane.ActorType = Converter.ReadType<ActorType>(reader);
            if (reader.ReadBoolean()) bgPlane.AttachedPlane = reader.ReadInt32();
            bgPlane.ParallaxFactor = reader.ReadSingle();

            return bgPlane;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            BackgroundPlane bgPlane = (BackgroundPlane)data;

            writer.Write(bgPlane.Position);
            writer.Write(bgPlane.Rotation);
            writer.Write(bgPlane.Scale);
            writer.Write(bgPlane.Size);
            writer.Write(bgPlane.TextureName);
            writer.Write(bgPlane.LightMap);
            writer.Write(bgPlane.AllowOverbrightness);
            writer.Write(bgPlane.Filter);
            writer.Write(bgPlane.Animated);
            writer.Write(bgPlane.Doublesided);
            writer.Write(bgPlane.Opacity);
            writer.Write(bgPlane.AttachedGroup.HasValue);
            if (bgPlane.AttachedGroup.HasValue) writer.Write(bgPlane.AttachedGroup.GetValueOrDefault());
            writer.Write(bgPlane.Billboard);
            writer.Write(bgPlane.SyncWithSamples);
            writer.Write(bgPlane.Crosshatch);
            // writing the unused field for data continuity
            // it's normally boolean but since it occupies one byte anyway
            // we can do a little bit of trolling
            writer.Write((byte)0x69);
            writer.Write(bgPlane.AlwaysOnTop);
            writer.Write(bgPlane.Fullbright);
            writer.Write(bgPlane.PixelatedLightmap);
            writer.Write(bgPlane.XTextureRepeat);
            writer.Write(bgPlane.YTextureRepeat);
            writer.Write(bgPlane.ClampTexture);
            Converter.WriteType(bgPlane.ActorType, writer);

            writer.Write(bgPlane.AttachedPlane.HasValue);
            if (bgPlane.AttachedPlane.HasValue) writer.Write(bgPlane.AttachedPlane.GetValueOrDefault());
            writer.Write(bgPlane.ParallaxFactor);
        }
    }
}
