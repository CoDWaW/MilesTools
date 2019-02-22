using System;
using System.Collections.Generic;
using System.IO;

using static Mileslice.Bank;
using static Mileslice.Utilities;

namespace Mileslice
{
    /// <summary>
    /// Class for slicing files.
    /// </summary>
    internal static class Slicing
    {
        /// <summary>
        /// Slice a list of files.
        /// </summary>
        /// <param name="inputFile">The list of path to files to be sliced.</param>
        /// <param name="outputPath">The output path to save all slices.</param>
        internal static void SliceFile(string[] inputFile, string outputPath)
        {
            foreach (var file in inputFile)
            {
                var extractPath = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(file));
                // Create separate path for each file
                try
                {
                    Directory.CreateDirectory(extractPath);
                }
                catch (Exception e)
                {
                    PrintError($"Failed to create output directory \"{extractPath}\": {e.Message}");
                }

                SliceFile(file, extractPath);
            }
        }

        /// <summary>
        /// Slice a file.
        /// </summary>
        /// <param name="inputFile">The path to a file to be sliced.</param>
        /// <param name="outputPath">The output path to save all slices.</param>
        internal static void SliceFile(string inputFile, string outputPath)
        {
            var bankFile = File.OpenRead(inputFile);
            var listFile = new StreamWriter(Path.Combine(outputPath, ".list.csv"));

            PrintInfo($"Processing {inputFile}...");
            PrintDebug("Searching for headers...");
            var headerOffsets = GetAllHeaders(bankFile);
            PrintDebug("Calculating file sizes...");
            var fileSizes = GetAllFileSizes(bankFile.Length, headerOffsets);
            PrintDebug($"Found {headerOffsets.Count} file headers.");

            // Export file list and write to console if not silent
            listFile.WriteLine("Count,Offset,Length");
            PrintDebug("Count\tOffset\tLength");
            for (int i = 0; i < headerOffsets.Count; i++)
            {
                listFile.WriteLine($"{i},{headerOffsets[i]},{fileSizes[i]}");
                if (!Program.IsSilent)
                {
                    PrintDebug($"{i}\t{headerOffsets[i]}\t{fileSizes[i]}");
                }
            }

            // Save file slices if not list only
            if (!Program.IsListOnly)
            {
                PrintDebug("Saving file slices...");
                SaveFileSlices(bankFile, headerOffsets, fileSizes, outputPath);
            }

            listFile.Close();
            bankFile.Close();
        }

        /// <summary>
        /// Slices a stream bank file into bink audio files with an offset list and a length list.
        /// </summary>
        /// <param name="bankFile">A stream of the stream bank file.</param>
        /// <param name="headerOffsets">A list of all header offset in the file.</param>
        /// <param name="fileSizes">A list of file lengths coresponding to the provided offset list.</param>
        /// <param name="outputPath">Path to save file slices to.</param>
        internal static void SaveFileSlices(FileStream bankFile, List<uint> headerOffsets, List<uint> fileSizes, string outputPath)
        {
            if (headerOffsets.Count != fileSizes.Count)
            {
                // Offset count mismatch with length count, abort
                PrintError("The count of provided offsets does not match with the count of lengths.");
            }

            for (int i = 0; i < headerOffsets.Count; i++)
            {
                var fileSize = (int)fileSizes[i];
                var buffer = new byte[fileSize];
                using (var output = File.OpenWrite(
                    Path.Combine(outputPath,
                    $"{i}.{Program.FileExtension}")))
                {
                    // Write bytes to file destination in output directory
                    bankFile.Seek(headerOffsets[i], SeekOrigin.Begin);
                    bankFile.Read(buffer, 0, fileSize);
                    output.Write(buffer, 0, fileSize);
                }
            }

            PrintInfo($"Saved {headerOffsets.Count} file slices.");
        }
    }
}
