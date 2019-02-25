using System;
using System.Runtime.InteropServices;

namespace Binkalore
{
    /// <summary>
    /// Utility class for logging to console and marshalling pointers.
    /// </summary>
    internal static class Utilities
    {
        internal static void PrintUsage()
        {
            Console.WriteLine("Binkalore - Bink Audio format converter");
            Console.WriteLine("Usage: Binkalore.exe [inputPath] (outputPath) (options)");
            Console.WriteLine("Options:");
            Console.WriteLine("- Logging");
            Console.WriteLine("\t--help\tPrint command line help information");
            Console.WriteLine("\t--silent\tOnly print minimal debug information");
            Console.WriteLine("\t--verbose\tLog all available debug information");
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

        /// <summary>
        /// Marshals a pointer to a string.
        /// </summary>
        /// <param name="intPtr">The pointer to be marshalled.</param>
        /// <returns>The string pointed by the pointer.</returns>
        internal static string ToStringAnsi(this IntPtr intPtr)
        {
            return Marshal.PtrToStringAnsi(intPtr);
        }
    }
}
