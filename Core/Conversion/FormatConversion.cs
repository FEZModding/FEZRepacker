using FEZRepacker.Core.FileSystem;

namespace FEZRepacker.Core.Conversion
{
    public static class FormatConversion
    {
        public static FileBundle Convert(object? data)
        {
            if(data == null)
            {
                throw new NullReferenceException(nameof(data));
            }

            var converter = FormatConverters.FindForType(data.GetType());

            if(converter == null)
            {
                throw new FormatConversionException($"Type {data.GetType()} is not supported for conversion.");
            }

            return converter.Convert(data);
        }

        public static object? Deconvert(FileBundle bundle)
        {
            var converter = FormatConverters.FindForFileBundle(bundle);

            if (converter == null)
            {
                throw new FormatConversionException($"File bundle type {bundle.MainExtension} is not supported for conversion.");
            }

            return converter.Deconvert(bundle);
        }
    }
}
