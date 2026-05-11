using System.Numerics;

namespace DirectX
{
    internal static class DirectXTexCompress
    {
        public static byte[] DecompressDxt1(byte[] imageData, int width, int height)
        {
            return DecompressDxtBlocks(imageData, width, height, (reader, pixels) => 
                Bc.D3DXDecodeBC1(pixels.AsSpan(), Bc.ReadBC1(reader))
            );
        }

        public static byte[] CompressDxt1(byte[] imageData, int width, int height)
        {
            var bc = new Bc.D3DX_BC1();
            return CompressDxtBlocks(imageData, width, height, (writer, pixels) =>
            {
                Bc.D3DXEncodeBC1(ref bc, pixels.AsSpan(), 0.5f, 0);
                Bc.WriteBC1(writer, bc);
            });
        }
        
        public static byte[] DecompressDxt3(byte[] imageData, int width, int height)
        {
            return DecompressDxtBlocks(imageData, width, height, (reader, pixels) => 
                Bc.D3DXDecodeBC2(pixels.AsSpan(), Bc.ReadBC2(reader))
            );
        }

        public static byte[] CompressDxt3(byte[] imageData, int width, int height)
        {
            var bc = new Bc.D3DX_BC2();
            return CompressDxtBlocks(imageData, width, height, (writer, pixels) =>
            {
                Bc.D3DXEncodeBC2(ref bc, pixels.AsSpan(), 0);
                Bc.WriteBC2(writer, bc);
            });
        }

        public static byte[] DecompressDxt5(byte[] imageData, int width, int height)
        {
            return DecompressDxtBlocks(imageData, width, height, (reader, pixels) =>
                Bc.D3DXDecodeBC3(pixels.AsSpan(), Bc.ReadBC3(reader))
            );
        }

        public static byte[] CompressDxt5(byte[] imageData, int width, int height)
        {
            var bc = new Bc.D3DX_BC3();
            return CompressDxtBlocks(imageData, width, height, (writer, pixels) =>
            {
                Bc.D3DXEncodeBC3(ref bc, pixels.AsSpan(), 0);
                Bc.WriteBC3(writer, bc);
            });
        }
        
        private static byte[] DecompressDxtBlocks(
            byte[] imageData, int width, int height, Action<BinaryReader, Vector4[]> decodeMethod)
        {
            int blocksX = (width + 3) / 4;
            int blocksY = (height + 3) / 4;
            var output = new byte[width * height * 4];
            var pixels = new Vector4[16];
            
            using var reader = new BinaryReader(new MemoryStream(imageData));

            for (int by = 0; by < blocksY; by++)
            {
                for (int bx = 0; bx < blocksX; bx++)
                {
                    decodeMethod.Invoke(reader, pixels);
                    WriteBlock(output, pixels, bx * 4, by * 4, width, height);
                }
            }

            return output;
        }

        private static byte[] CompressDxtBlocks(
            byte[] imageData, int width, int height, Action<BinaryWriter, Vector4[]> encodeMethod)
        {
            int blocksX = (width + 3) / 4;
            int blocksY = (height + 3) / 4;
            using var output = new MemoryStream();
            using var writer = new BinaryWriter(output);
            var pixels = new Vector4[16];

            for (int by = 0; by < blocksY; by++)
            {
                for (int bx = 0; bx < blocksX; bx++)
                {
                    ReadBlock(imageData, pixels, bx * 4, by * 4, width, height);
                    encodeMethod.Invoke(writer, pixels);
                }
            }

            return output.ToArray();
        }
        
        private static void ReadBlock(byte[] imageData, Vector4[] pixels, int startX, int startY, int width, int height)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    int px = startX + x;
                    int py = startY + y;
                    int i = y * 4 + x;

                    if (px < width && py < height)
                    {
                        int src = (py * width + px) * 4;
                        pixels[i] = new Vector4(
                            imageData[src + 0] / 255f,
                            imageData[src + 1] / 255f,
                            imageData[src + 2] / 255f,
                            imageData[src + 3] / 255f
                        );
                    }
                    else
                    {
                        pixels[i] = Vector4.Zero;
                    }
                }
            }
        }
        
        private static void WriteBlock(byte[] output, Vector4[] pixels, int startX, int startY, int width, int height)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    int px = startX + x;
                    int py = startY + y;

                    if (px >= width || py >= height)
                        continue;

                    int dst = (py * width + px) * 4;
                    var p = pixels[y * 4 + x];
                    output[dst + 0] = (byte)(int)(Math.Min(Math.Max(p.X, 0f), 1f) * 255f + 0.5f);
                    output[dst + 1] = (byte)(int)(Math.Min(Math.Max(p.Y, 0f), 1f) * 255f + 0.5f);
                    output[dst + 2] = (byte)(int)(Math.Min(Math.Max(p.Z, 0f), 1f) * 255f + 0.5f);
                    output[dst + 3] = (byte)(int)(Math.Min(Math.Max(p.W, 0f), 1f) * 255f + 0.5f);
                }
            }
        }
    }
}
