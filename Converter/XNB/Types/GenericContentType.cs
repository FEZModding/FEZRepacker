using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

using FEZRepacker.Converter.Definitions;
using FEZRepacker.Converter.Helpers;
using FEZRepacker.Converter.XNB.Formats;

namespace FEZRepacker.Converter.XNB.Types
{
    /// <summary>
    /// Generalizes content type creation by using reflection on given type containing <c>XnbTypeAttribute</c>
    /// by parsing each public property with <c>XnbPropertyAttribute</c> based on parameters contained in it.
    /// This replaces the need of creating a content type for each custom asset type.
    /// </summary>
    /// <typeparam name="T">Class type with <c>XnbTypeAttribute</c> to form content type from.</typeparam>
    internal class GenericContentType<T> : XnbContentType<T> where T : class
    {
        private XnbAssemblyQualifier _name;
        public GenericContentType(XnbFormatConverter converter) : base(converter)
        {
            var qualifier = GetXnbReaderTypeFor(typeof(T));
            if (qualifier.HasValue) _name = qualifier.Value;
            PopulateReflectionMaps();
            _typeBuilder = CreateContainedTypeConstructor();
        }
        public override XnbAssemblyQualifier Name => _name;

        private Dictionary<PropertyInfo, XnbPropertyAttribute> _propertyMap = new();
        private Dictionary<PropertyInfo, Type> _underlyingTypeMap = new();


        private void PopulateReflectionMaps()
        {
            _propertyMap = typeof(T).GetProperties()
                .Where(property => Attribute.IsDefined(property, typeof(XnbPropertyAttribute)))
                .ToDictionary(
                    property => property,
                    property => (property.GetCustomAttributes(typeof(XnbPropertyAttribute), false).Single() as XnbPropertyAttribute)!
                 ).OrderBy(pair => pair.Value.Order)
                 .ToDictionary(pair => pair.Key, pair => pair.Value);

            _underlyingTypeMap = _propertyMap.ToDictionary(
                pair => pair.Key,
                pair => Nullable.GetUnderlyingType(pair.Key.PropertyType) ?? pair.Key.PropertyType
            );
        }


        private Func<T> _typeBuilder;
        private Func<T> CreateContainedTypeConstructor()
        {
            var t = typeof(T);
            var ex = new Expression[] { Expression.New(typeof(T)) };
            var block = Expression.Block(t, ex);
            return Expression.Lambda<Func<T>>(block).Compile();
        }


        public override object Read(BinaryReader reader)
        {
            T content = _typeBuilder();

            foreach (var propertyMapRecord in _propertyMap)
            {
                var property = propertyMapRecord.Key;
                var attribute = propertyMapRecord.Value;

                Type propertyType = _underlyingTypeMap[property];

                if (attribute.Optional)
                {
                    if (!reader.ReadBoolean()) continue;
                }

                object? readValue = null;

                if (attribute.UseConverter)
                {
                    readValue = Converter.ReadType(propertyType, reader, attribute.SkipIdentifier);
                }
                else if (propertyType == typeof(bool)) readValue = reader.ReadBoolean();
                else if (propertyType == typeof(int)) readValue = reader.ReadInt32();
                else if (propertyType == typeof(byte)) readValue = reader.ReadByte();
                else if (propertyType == typeof(short)) readValue = reader.ReadInt16();
                else if (propertyType == typeof(float)) readValue = reader.ReadSingle();
                else if (propertyType == typeof(char)) readValue = reader.ReadChar();
                else if (propertyType == typeof(string)) readValue = reader.ReadString();
                else if (propertyType == typeof(Vector2)) readValue = reader.ReadVector2();
                else if (propertyType == typeof(Vector3)) readValue = reader.ReadVector3();
                else if (propertyType == typeof(Quaternion)) readValue = reader.ReadQuaternion();
                else if (propertyType == typeof(Color)) readValue = reader.ReadColor();
                else if (propertyType == typeof(TimeSpan)) readValue = new TimeSpan(reader.ReadInt64());
                else throw new NotSupportedException($"Type {propertyType.FullName} is not supported");

                if (readValue != null) property.SetValue(content, readValue);
            }

            return content;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            T content = (T)data;

            foreach (var propertyMapRecord in _propertyMap)
            {
                var property = propertyMapRecord.Key;
                var attribute = propertyMapRecord.Value;

                Type propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                object? writeValue = property.GetValue(content);

                if (attribute.Optional)
                {
                    int typeID = Converter.ContentTypes.ToList().FindIndex(t => t.BasicType == propertyType) + 1;
                    if (writeValue != null)
                    {
                        if (typeID > 0) writer.Write7BitEncodedInt(typeID);
                        else writer.Write(true);
                    }
                    else writer.Write(false);

                }

                if (writeValue != null)
                {
                    if (attribute.UseConverter)
                    {
                        Converter.WriteType(propertyType, writeValue, writer, attribute.SkipIdentifier);
                    }
                    else if (propertyType == typeof(bool)) writer.Write((bool)writeValue!);
                    else if (propertyType == typeof(int)) writer.Write((int)writeValue!);
                    else if (propertyType == typeof(byte)) writer.Write((byte)writeValue!);
                    else if (propertyType == typeof(short)) writer.Write((short)writeValue!);
                    else if (propertyType == typeof(float)) writer.Write((float)writeValue!);
                    else if (propertyType == typeof(char)) writer.Write(((char)writeValue!));
                    else if (propertyType == typeof(string)) writer.Write((string)writeValue!);
                    else if (propertyType == typeof(Vector2)) writer.Write((Vector2)writeValue!);
                    else if (propertyType == typeof(Vector3)) writer.Write((Vector3)writeValue!);
                    else if (propertyType == typeof(Quaternion)) writer.Write((Quaternion)writeValue!);
                    else if (propertyType == typeof(Color)) writer.Write((Color)writeValue!);
                    else if (propertyType == typeof(TimeSpan)) writer.Write(((TimeSpan)writeValue!).Ticks);
                    else throw new NotSupportedException($"Type {propertyType.FullName} is not supported");
                }
            }
        }
    }
}
