using System.Collections.ObjectModel;

using FEZRepacker.Core.FileSystem;

namespace FEZRepacker.Core.Conversion
{
    /// <summary>
    /// Defines methods of convertion between specific object types 
    /// and a <see cref="FileBundle"/>, along with information about
    /// object types and file extension supported by this converter.
    /// </summary>
    public abstract class FormatConverter
    {
        public abstract string FileFormat { get; }
        public abstract Type FormatType { get; }

        public abstract FileBundle Convert(object? data);
        public abstract object? Deconvert(FileBundle bundle);

        public IDictionary<string, object> Configuration { get; } = new Dictionary<string, object>();

        public FileBundle Convert<T>(T data)
        {
            return Convert((object?)data);
        }
        public T? Deconvert<T>(FileBundle bundle)
        {
            return (T?)Deconvert(bundle);
        }
    }

    /// <summary>
    /// Generic type which automatically supplies format type property of <see cref="FormatConverter"/>
    /// </summary>
    /// <typeparam name="ConvertedType">A type to support for this converter</typeparam>
    public abstract class FormatConverter<ConvertedType> : FormatConverter
    {
        public override Type FormatType => typeof(ConvertedType);
        public abstract FileBundle ConvertTyped(ConvertedType data);
        public abstract ConvertedType DeconvertTyped(FileBundle bundle);

        public override FileBundle Convert(object? data)
        {
            return ConvertTyped((ConvertedType?)data!);
        }

        public override object? Deconvert(FileBundle bundle)
        {
            return DeconvertTyped(bundle);
        }
    }
}
