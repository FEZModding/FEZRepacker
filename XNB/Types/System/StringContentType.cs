using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEZRepacker.XNB.Types
{
    class StringContentType : XNBContentType<string>
    {
        public StringContentType(XNBContentConverter converter) : base(converter){}
        public override TypeAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.StringReader";

        public override object Read(BinaryReader reader)
        {
            return reader.ReadString();
        }

        public override void Write(object data, BinaryWriter writer)
        {
            writer.Write((string)data);
        }
    }
}
