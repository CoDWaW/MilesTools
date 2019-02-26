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
            var successCount = 0;
            var failureCount = 0;

            foreach (var file in inputFiles)
            {
                if (ConvertFile(file, outputPath))
                {
                    successCount++;
                }
                else
                {
                    failureCount++;
                }
            }

            PrintInfo($"{successCount} files succeeded and {failureCount} files failed.");
        }

        internal static bool ConvertFile(string inputFile, string outputPath)
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
                return true;
            }
            else
            {
                PrintInfo($"Conversion error on file {Path.GetFileName(inputFile)}: {result}");
                return false;
            }
        }
    }
}
