using System;
using System.IO;

using static Mileslice.Slicing;
using static Mileslice.Utilities;

namespace Mileslice
{
    internal class Program
    {
        // BinkA header magic bytes
        internal static readonly int[] HeaderMagic = { 0x31, 0x46, 0x43, 0x42 };
        // File slicing extension
        internal const string FileExtension = "binka";
        // File search buffer size
        internal const int BufferSize = 4096;

        // Global switches
        internal static bool IsSilent = false;
        internal static bool IsVerbose = false;
        internal static bool IsListOnly = false;
        internal static bool IsRecursive = false;

        private static void Main(string[] args)
        {
            // Check arguments
            if (args.Length == 0)
            {
                PrintUsage();
            }
            ParseSwitches(args);

            // Check paths
            var outputPath = "";
            var isDirectory = false;
            if (File.Exists(args[0]))
            {
                // A single file is specified
                outputPath = Path.GetFileNameWithoutExtension(args[0]);
            }
            else if (Directory.Exists(args[0]))
            {
                // A directory is specified
                isDirectory = true;
            }
            else
            {
                // Path is invalid
                PrintError($"Specified input path \"{args[0]}\" is neither a file nor a directory.");
            }

            // Check if an output path is specified as the second argument
            if (args.Length > 1 && !args[1].StartsWith("--"))
            {
                // The second argument is not a switch
                outputPath = args[1];
            }

            // Ensure creation of output directory
            try
            {
                Directory.CreateDirectory(outputPath);
            }
            catch (Exception e)
            {
                PrintError($"Failed to create output directory \"{outputPath}\": {e.Message}");
            }

            PrintDebug($"Using input path \"{args[0]}\".");
            PrintDebug($"Using output path \"{outputPath}\".");

            if (isDirectory)
            {
                // Slicing all files in a directory
                string[] files;
                if (IsRecursive)
                {
                    // Recursive listing
                    files = Directory.GetFiles(args[0], "*", SearchOption.AllDirectories);
                }
                else
                {
                    // Only top directory
                    files = Directory.GetFiles(args[0], "*", SearchOption.TopDirectoryOnly);
                }
                SliceFile(files, outputPath);
            }
            else
            {
                // Slicing a single file
                SliceFile(args[0], outputPath);
            }

            PrintError("Parsing complete.");
        }

        /// <summary>
        /// Parse command line arguments and toggle respective switches.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        private static void ParseSwitches(string[] args)
        {
            foreach (var arg in args)
            {
                switch (arg)
                {
                    case "--help":
                        PrintUsage();
                        break;
                    case "--silent":
                        IsSilent = true;
                        PrintInfo("Silent: most logging is disabled.");
                        break;
                    case "--verbose":
                        IsVerbose = true;
                        PrintInfo("Verbose: detailed logging is enabled.");
                        break;
                    case "--list-only":
                        IsListOnly = true;
                        PrintInfo("List only: files will not be sliced.");
                        break;
                    case "--recursive":
                        IsRecursive = true;
                        PrintInfo("Recursive: all files under input directory will be processed.");
                        break;
                    default:
                        // Invalid switch, ignore
                        break;
                }
            }
        }
    }
}
