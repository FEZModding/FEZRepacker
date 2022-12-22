using FEZRepacker.XNB.Attributes;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace FEZRepacker.XNB.Types
{
    class GenericContentType<T> : XNBContentType<T> where T : class
    {
        private FEZAssemblyQualifier _name;
        public GenericContentType(XNBContentConverter converter) : base(converter) {
            var attribute = (typeof(T).GetCustomAttributes(typeof(XNBTypeAttribute), false).Single() as XNBTypeAttribute);
            if (attribute != null) _name = attribute.Qualifier;
            PopulateReflectionMaps();
            CreateContainedTypeConstructor();
        }
        public override FEZAssemblyQualifier Name => _name;

        /* 
         * Greetings, dear traveller! 
         * 
         * Before you lies what I personally consider to be the most cursed
         * code that I have ever created, and it hold this entire project together!
         * You see, this piece of junk replaced a need for having a separate
         * converter for each data type. HOWEVER, in a place of like 30 files
         * comes one, probably very unsafe one. But should I care?! NAH!
         * 
         * If you're reading this it means it worked at least once and I proceeded
         * onward with my life, cursing everyone working on this project in the future.
         */

        private Dictionary<PropertyInfo, XNBPropertyAttribute> _propertyMap = new();
        private Dictionary<PropertyInfo, Type> _underlyingTypeMap = new();
        

        private void PopulateReflectionMaps()
        {
            _propertyMap = typeof(T).GetProperties()
                .Where(property => Attribute.IsDefined(property, typeof(XNBPropertyAttribute)))
                .ToDictionary(
                    property => property, 
                    property => (property.GetCustomAttributes(typeof(XNBPropertyAttribute), false).Single() as XNBPropertyAttribute)!
                 ).OrderBy(pair => pair.Value.Order)
                 .ToDictionary(pair => pair.Key, pair => pair.Value);

            _underlyingTypeMap = _propertyMap.ToDictionary(
                pair => pair.Key,
                pair => Nullable.GetUnderlyingType(pair.Key.PropertyType) ?? pair.Key.PropertyType
            );
        }


        private Func<T> _typeBuilder;
        private void CreateContainedTypeConstructor()
        {
            var t = typeof(T);
            var ex = new Expression[] { Expression.New(typeof(T)) };
            var block = Expression.Block(t, ex);
            _typeBuilder = Expression.Lambda<Func<T>>(block).Compile();
        }


        public override object Read(BinaryReader reader)
        {
            T content = _typeBuilder();

            foreach ((var property, var attribute) in _propertyMap)
            {
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
                else if (propertyType == typeof(float)) readValue = reader.ReadSingle();
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

            foreach ((var property, var attribute) in _propertyMap)
            {
                Type propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                object? writeValue = property.GetValue(content);

                if (attribute.Optional)
                {
                    int typeID = Converter.Types.ToList().FindIndex(t => t.BasicType == propertyType) + 1;
                    if(writeValue != null)
                    {
                        if(typeID > 0) writer.Write7BitEncodedInt(typeID);
                        else writer.Write(true);
                    }
                    else writer.Write(false);

                }

                if(writeValue != null)
                {
                    if (attribute.UseConverter)
                    {
                        Converter.WriteType(propertyType, writeValue, writer, attribute.SkipIdentifier);
                    }
                    else if (propertyType == typeof(bool)) writer.Write((bool)writeValue!);
                    else if (propertyType == typeof(int)) writer.Write((int)writeValue!);
                    else if (propertyType == typeof(byte)) writer.Write((byte)writeValue!);
                    else if (propertyType == typeof(float)) writer.Write((float)writeValue!);
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
