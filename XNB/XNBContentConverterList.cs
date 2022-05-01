using FEZRepacker.XNB.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEZRepacker.XNB
{
    static class XNBContentConverterList
    {
        private static Dictionary<TypeAssemblyQualifier, XNBContentConverter> _list = new();
        static XNBContentConverterList()
        {
            Add(TextStorageContentConverter.Instance);
            Add(LevelContentConverter.Instance);
        }

        private static void Add(XNBContentConverter converter)
        {
            if (converter.DataTypes.Length == 0) return;
            _list.Add(converter.DataTypes[0], converter);
        }

        public static XNBContentConverter Get(TypeAssemblyQualifier typeName)
        {
            if(_list.ContainsKey(typeName)) return _list[typeName];
            else return null;
        }
    }
}
