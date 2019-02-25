using System;
using System.IO;

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

        private static void Main(string[] args)
        {
            CheckDependencies();

            if (args.Length == 0)
            {
                PrintUsage();
            }

            // Expect a valid file path as the first argument
            if (File.Exists(args[0]))
            {
                // Set default output path name
                OutputPath = Path.GetFileNameWithoutExtension(args[0]);
            }
            else
            {
                PrintError($"Specified input file \"{args[0]}\" does not exist.");
            }

            // Check if an output path is specified as the second argument
            if (args.Length > 1 && !args[1].StartsWith("--"))
            {
                // The second argument is not a switch
                try
                {
                    Directory.CreateDirectory(args[1]);
                    OutputPath = args[1];
                    PrintDebug($"Using output path \"{OutputPath}\".");
                }
                catch (Exception e)
                {
                    PrintError($"Failed to create output directory \"{args[1]}\": {e.Message}");
                }
            }

            // Ensure creation of output directory
            try
            {
                Directory.CreateDirectory(OutputPath);
            }
            catch (Exception e)
            {
                PrintError($"Failed to create output directory \"{OutputPath}\": {e.Message}");
            }

            PrintInfo("Initializing MSS...");
            Initialize();
            ConvertFiles(args[0]);
        }

        private static void Initialize()
        {
            AilSetRedistDirectory(".");
            AilStartup();
        }

        private static void ConvertFiles(string inputPath)
        {
            // var files = Directory.GetFiles(inputPath);
            // PrintDebug($"Found {files.Length} files in target directory. Beginning conversion.");
            PrintInfo($"Processing {inputPath}...");
            var result = AilDecompressAsi(File.ReadAllBytes(inputPath), out byte[] wavData);
            if (String.IsNullOrEmpty(result))
            {
                // No error occured
                var outputFilePath = Path.Combine(
                    OutputPath, $"{Path.GetFileNameWithoutExtension(inputPath)}.wav");
                File.WriteAllBytes(outputFilePath, wavData);
            }
            else
            {
                PrintInfo($"Conversion error on file {Path.GetFileName(inputPath)}: {result}");
            }

            AilShutdown();
            PrintError("Conversion completed.");
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
                    default:
                        // Invalid switch, ignore
                        break;
                }
            }
        }
    }
}
