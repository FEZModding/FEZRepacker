using FEZRepacker.Core.FileSystem;

namespace FEZRepacker.Core.Conversion
{
    public abstract class FormatConverter
    {
        public abstract string FileFormat { get; }
        public abstract Type FormatType { get; }

        public abstract FileBundle Convert(object? data);
        public abstract object? Deconvert(FileBundle bundle);

        public FileBundle Convert<T>(T data)
        {
            return Convert((object?)data);
        }
        public T? Deconvert<T>(FileBundle bundle)
        {
            return (T?)Deconvert(bundle);
        }
    }

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
