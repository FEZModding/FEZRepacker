using System.Runtime.CompilerServices;

namespace FEZRepacker.Core.Definitions.Game
{
    /// <summary>
    /// Used to assign conversion method for content type property.
    /// For more details, see <c>GenericContentType</c> implementation.
    /// </summary>
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
