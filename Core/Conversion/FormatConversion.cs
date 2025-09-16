using FEZRepacker.Core.FileSystem;

namespace FEZRepacker.Core.Conversion
{
    /// <summary>
    /// Main format conversion handling logic. Contains methods for converting
    /// and deconverting objects to and from easily readable file formats.
    /// </summary>
    /// <remarks>
    /// A <see cref="FormatConverter"/> specific to to given object type is used
    /// in order to convert and deconvert it. A list of format converters defining
    /// supported object types is stored in <see cref="FormatConverters"/>.
    /// </remarks>
    public static class FormatConversion
    {
        /// <summary>
        /// Finds converter for a type of given object, then attempts to
        /// convert it and store in a <see cref="FileBundle"/>.
        /// </summary>
        /// <param name="data">A reference to object to convert</param>
        /// <param name="settings">A configuration to alter converter behaviour</param>
        /// <returns>
        /// A <see cref="FileBundle"/> containing file or files converted from given object.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Thrown when null object was given
        /// </exception>
        /// <exception cref="FormatConversionException">
        /// Thrown when a type of given object is not supported by Repacker
        /// </exception>
        public static FileBundle Convert(object? data, FormatConverterSettings? settings = null)
        {
            if(data == null)
            {
                throw new NullReferenceException(nameof(data));
            }

            var converter = FormatConverters.FindForType(data.GetType());

            if (converter == null)
            {
                throw new FormatConversionException($"Type {data.GetType()} is not supported for conversion.");
            }

            if (settings.HasValue)
            {
                converter.Settings = settings.Value;
            }

            return converter.Convert(data);
        }

        /// <summary>
        /// Finds converter for main extension of given <see cref="FileBundle"/>,
        /// then attempts to deconvert it back to an object with a type assigned to this converter.
        /// </summary>
        /// <param name="bundle">A <see cref="FileBundle"/> containing files to convert.</param>
        /// <param name="settings">A configuration to alter converter behaviour</param>
        /// <returns>
        /// An object deconverted from files contained in given <see cref="FileBundle"/>.
        /// </returns>
        /// <exception cref="FormatConversionException">
        /// Thrown when main extension of given <see cref="FileBundle"/> is not supported by Repacker
        /// </exception>
        public static object? Deconvert(FileBundle bundle, FormatConverterSettings? settings = null)
        {
            var converter = FormatConverters.FindForFileBundle(bundle);

            if (converter == null)
            {
                throw new FormatConversionException($"File bundle type {bundle.MainExtension} is not supported for conversion.");
            }
            
            if (settings.HasValue)
            {
                converter.Settings = settings.Value;
            }

            return converter.Deconvert(bundle);
        }
    }
}
