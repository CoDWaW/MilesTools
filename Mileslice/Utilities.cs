using System;

namespace Mileslice
{
    /// <summary>
    /// Utility class for logging to console.
    /// </summary>
    internal static class Utilities
    {
        internal static void PrintUsage()
        {
            Console.WriteLine("Mileslice - Miles Sound System 10 Bank File Slicer");
            Console.WriteLine("Usage: Mileslice.exe [inputPath] (outputPath) (options)");
            Console.WriteLine("Input path can either be a single file or a directory.");
            Console.WriteLine("Options:");
            Console.WriteLine("- Logging");
            Console.WriteLine("\t--help\tPrint command line help information");
            Console.WriteLine("\t--silent\tSuppress most console logging");
            Console.WriteLine("\t--verbose\tLog all available debug information");
            Console.WriteLine("- Extraction");
            Console.WriteLine("\t--list-only\tSkip file extraction, only create file list");
            Console.WriteLine("\t--recursive\tIf a directory is specified as input, all files under it will be processed");
            PrintError("(End of help)");
        }

        /// <summary>
        /// Prints a debug message to the console if not in silent mode.
        /// </summary>
        /// <param name="message">The message to be printed.</param>
        internal static void PrintDebug(string message)
        {
            if (Program.IsSilent || !Program.IsVerbose)
            {
                return;
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"[-] {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        internal static void PrintInfo(string message)
        {
            if (Program.IsSilent)
            {
                return;
            }

            Console.WriteLine($"[*] {message}");
        }

        /// <summary>
        /// Prints an error message in red to the console and terminate the app.
        /// </summary>
        /// <param name="message">The message to be printed.</param>
        internal static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[!] {message}");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
