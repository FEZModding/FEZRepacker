using System.Runtime.CompilerServices;

namespace FEZRepacker.Converter.Definitions
{
    [AttributeUsage(AttributeTargets.Enum, Inherited = false, AllowMultiple = false)]
    internal sealed class XnbEnumTypeAttribute : Attribute
    {
        public XnbAssemblyQualifier Qualifier { get; set; }

        public XnbEnumTypeAttribute(string _qualifier){
            Qualifier = _qualifier;
        }

    }
}
