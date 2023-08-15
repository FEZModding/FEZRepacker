using System.Runtime.CompilerServices;

namespace FEZRepacker.Converter.Definitions
{
    /// <summary>
    /// Used to store information about type readers declared in XNB files.
    /// XNB files use reader type assembly qualifers to identify types which
    /// aren't handled by generic readers (like enums).
    /// </summary>
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
