using System.Runtime.CompilerServices;

namespace FEZRepacker.Core.Definitions.Game
{
    /// <summary>
    /// Used to declare custom properties for a content type conversion.
    /// Instead of matching namespaces and specification, each content type has an assembly
    /// qualifier structure containing name actually used within the XNB file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    internal sealed class XnbTypeAttribute : Attribute
    {
        public XnbAssemblyQualifier Qualifier { get; set; }

        public XnbTypeAttribute(string _qualifier)
        {
            Qualifier = _qualifier;
        }

    }
}
