using System;
using System.IO;

using static Binkalore.Conversion;
using static Binkalore.InteropWrapper;
using static Binkalore.Utilities;

namespace Binkalore
{
    /// <summary>
    /// Compatibility notes:
    /// This will only work under .NET Framework 3.5.
    /// Any version higher will result in MSS failing to start up.
    /// </summary>
    internal class Program
    {
        // File search buffer size
        internal const int BufferSize = 4096;
        // Tail audio chunk magic bytes
        internal static readonly int[] ChunkMagic = { 0x99, 0x99 };

        // Global switches
        internal static string OutputPath = "";
        internal static bool IsHelp = false;
        internal static bool IsSilent = false;
        internal static bool IsVerbose = false;
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

            // Initialize MSS
            CheckDependencies();
            PrintInfo("Initializing MSS...");
            Initialize();

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
                ConvertFiles(files, outputPath);
            }
            else
            {
                // Slicing a single file
                ConvertFile(args[0], outputPath);
            }

            AilShutdown();
            PrintError("Conversion completed.");
        }

        private static void Initialize()
        {
            AilSetRedistDirectory(".");
            AilStartup();
        }

        private static void CheckDependencies()
        {
            if (!File.Exists("mss32.dll"))
            {
                PrintError("mss32.dll (Miles Sound System library) is not found.");
            }
            if (!File.Exists("binkawin.asi"))
            {
                PrintError("binkawin.asi (Bink Audio ASI Codec) is not found.");
            }
        }

        private static void ParseSwitches(string[] args)
        {
            foreach (var arg in args)
            {
                switch (arg)
                {
                    case "--help":
                        IsHelp = true;
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
