using System;
using System.IO;

using static Binkalore.InteropWrapper;
using static Binkalore.Utilities;

namespace Binkalore
{
    internal static class Conversion
    {
        internal static void ConvertFiles(string[] inputFiles, string outputPath)
        {
            foreach (var file in inputFiles)
            {
                ConvertFile(file, outputPath);
            }
        }

        internal static void ConvertFile(string inputFile, string outputPath)
        {
            PrintInfo($"Processing {inputFile}...");
            var result = AilDecompressAsi(File.ReadAllBytes(inputFile), out byte[] wavData);
            if (String.IsNullOrEmpty(result) && wavData != null)
            {
                // No error occured
                var outputFilePath = Path.Combine(
                    outputPath,
                    $"{Path.GetFileNameWithoutExtension(inputFile)}.wav");
                File.WriteAllBytes(outputFilePath, wavData);
            }
            else
            {
                PrintInfo($"Conversion error on file {Path.GetFileName(inputFile)}: {result}");
            }
        }
    }
}
