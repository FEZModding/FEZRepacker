using System.Runtime.CompilerServices;

namespace FEZRepacker.Converter.Definitions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum, Inherited = false, AllowMultiple = false)]
    internal sealed class XnbTypeAttribute : Attribute
    {
        public XnbAssemblyQualifier Qualifier { get; set; }

        public XnbTypeAttribute(string _qualifier)
        {
            Qualifier = _qualifier;
        }

    }
}