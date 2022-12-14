using System.Runtime.CompilerServices;

namespace FEZRepacker.XNB.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class XNBTypeAttribute : Attribute
    {
        public FEZAssemblyQualifier Qualifier { get; set; }

        public XNBTypeAttribute(string _qualifier){
            Qualifier = _qualifier;
        }

    }
}
