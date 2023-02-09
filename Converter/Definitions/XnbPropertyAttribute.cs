using System.Runtime.CompilerServices;

namespace FEZRepacker.Converter.Definitions
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    internal sealed class XnbPropertyAttribute : Attribute
    {
        public int Order { get; private set; }
        public bool UseConverter { get; set; }
        public bool Optional { get; set; }
        public bool SkipIdentifier { get; set; }

        public XnbPropertyAttribute(
            [CallerLineNumber] int _order = 0
        )
        {
            Order = _order;
        }

    }
}