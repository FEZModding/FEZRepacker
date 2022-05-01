using FEZRepacker.XNB.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEZRepacker.XNB
{
    abstract class XNBContent
    {
        public abstract XNBContentConverter Converter { get; }
    }

    abstract class XNBContentConverter
    {
        public abstract TypeAssemblyQualifier[] DataTypes { get; }
        public abstract string FileFormat { get; }

        public abstract void Write(XNBContent data, BinaryWriter writer);
        public abstract XNBContent Read(BinaryReader reader);
        public abstract void WriteUnpacked(XNBContent data, BinaryWriter writer);
        public abstract XNBContent ReadUnpacked(BinaryReader reader);
    }
}
