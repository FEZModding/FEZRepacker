using System.Runtime.CompilerServices;

namespace FEZRepacker;

/// <summary>
/// Extensions for polyfilling functionality not available in .NET Standard 2.0.
/// </summary>
public static class PolyfillExtensions
{
    public static void Deconstruct<TKey, TValue>(
        this KeyValuePair<TKey, TValue> kvp,
        out TKey key,
        out TValue value)
    {
        key = kvp.Key;
        value = kvp.Value;
    }

    public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int count)
    {
        // This is a terrible implementation but it'll probably do for now.
        var array = source.ToArray();
        return array.Take(Math.Max(0, array.Length - count));
    }

    // Source: hhttps://source.dot.net/#Microsoft.Build.Framework/BinaryReaderExtensions.cs,20
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Read7BitEncodedInt(this BinaryReader reader)
    {
        // Read out an Int32 7 bits at a time.  The high bit
        // of the byte when on means to continue reading more bytes.
        int count = 0;
        int shift = 0;
        byte b;
        do
        {
            // Check for a corrupted stream.  Read a max of 5 bytes.
            // In a future version, add a DataFormatException.
            if (shift == 5 * 7)  // 5 bytes max per Int32, shift += 7
            {
                throw new FormatException();
            }
 
            // ReadByte handles end of stream cases for us.
            b = reader.ReadByte();
            count |= (b & 0x7F) << shift;
            shift += 7;
        } while ((b & 0x80) != 0);
        return count;
    }

    // Source: https://source.dot.net/#Microsoft.Build.Framework/BinaryWriterExtensions.cs,35
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write7BitEncodedInt(this BinaryWriter writer, int value)
    {
        // Write out an int 7 bits at a time.  The high bit of the byte,
        // when on, tells reader to continue reading more bytes.
        uint v = (uint)value;   // support negative numbers
        while (v >= 0x80)
        {
            writer.Write((byte)(v | 0x80));
            v >>= 7;
        }
 
        writer.Write((byte)v);
    }
}
