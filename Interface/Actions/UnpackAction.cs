﻿using System;

using FEZRepacker.Core.Conversion;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.XNB;


namespace FEZRepacker.Interface.Actions
{
    internal abstract class UnpackAction : CommandLineAction
    {
        public enum UnpackingMode
        {
            Raw,
            DecompressedXNB,
            Converted
        }
        protected abstract UnpackingMode Mode { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string[] Aliases { get; }

        public CommandLineArgument[] Arguments => new[] {
            new CommandLineArgument("pak-path"),
            new CommandLineArgument("destination-folder")
        };

        public void Execute(string[] args)
        {
            var pakPath = args[0];
            var outputDir = args[1];

            UnpackPackage(pakPath, outputDir, Mode);
        }

        public static FileBundle UnpackFile(string extension, Stream data, UnpackingMode mode)
        {
            if(extension != ".xnb" || mode == UnpackingMode.Raw)
            {
                return FileBundle.Single(data, extension);
            }
            else if (mode == UnpackingMode.DecompressedXNB)
            {
                return FileBundle.Single(XnbCompressor.Decompress(data), ".xnb");
            }
            else if (mode == UnpackingMode.Converted)
            {
                FileBundle outputBundle;
                try
                {
                    var outputData = XnbSerializer.Deserialize(data)!;
                    outputBundle = FormatConversion.Convert(outputData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Cannot deserialize XNB file: {ex.Message}. Saving raw file instead.");
                    return FileBundle.Single(data, extension);
                }

                return outputBundle;
            }
            else
            {
                return new FileBundle();
            }
        }

        public static void UnpackPackage(string pakPath, string outputDir, UnpackingMode mode)
        {
            if (Path.GetExtension(pakPath) != ".pak")
            {
                throw new Exception("A path must lead to a .PAK file.");
            }

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            using var pakStream = File.OpenRead(pakPath);
            using var pakReader = new PakReader(pakStream);

            Console.WriteLine($"Unpacking archive {pakPath} containing {pakReader.FileCount} files...");

            int filesDone = 0;
            foreach (var pakFile in pakReader.ReadFiles())
            {
                var extension = pakFile.DetectedFileExtension;

                Console.WriteLine(
                    $"({filesDone + 1}/{pakReader.FileCount})" +
                    $"{pakFile.Path} ({(extension.Length == 0 ? "unknown" : extension)} file," +
                    $"size: {pakFile.Size} bytes)"
                );

                try
                {
                    using var outputBundle = UnpackFile(extension, pakFile.Data, mode);

                    outputBundle.BundlePath = Path.Combine(outputDir, pakFile.Path);
                    Directory.CreateDirectory(Path.GetDirectoryName(outputBundle.BundlePath) ?? "");

                    foreach (var outputFile in outputBundle.Files)
                    {
                        var fileName = outputBundle.BundlePath + outputBundle.MainExtension + outputFile.Extension;
                        using var fileOutputStream = File.Open(fileName, FileMode.Create);
                        outputFile.Data.CopyTo(fileOutputStream);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Unable to unpack {pakFile.Path} - {ex.Message}");
                }
                filesDone++;
            }
        }
    }
}
