using System.Runtime.CompilerServices;

namespace FEZRepacker.Converter.Definitions
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class XnbReaderTypeAttribute : Attribute
    {
        public XnbAssemblyQualifier Qualifier { get; set; }

        public XnbReaderTypeAttribute(string _qualifier)
        {
            Qualifier = _qualifier;
        }

    }
}