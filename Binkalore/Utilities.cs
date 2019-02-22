using System;
using System.Runtime.InteropServices;

namespace Binkalore
{
    /// <summary>
    /// Utility class for logging to console and marshalling pointers.
    /// </summary>
    internal static class Utilities
    {
        /// <summary>
        /// Prints a debug message to the console if not in silent mode.
        /// </summary>
        /// <param name="message">The message to be printed.</param>
        internal static void PrintDebug(string message)
        {
            if (Program.IsSilent)
            {
                return;
            }
            Console.WriteLine(message);
        }

        /// <summary>
        /// Prints an error message to the console and terminate the app.
        /// </summary>
        /// <param name="message">The message to be printed.</param>
        internal static void PrintError(string message)
        {
            Console.WriteLine(message);
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
