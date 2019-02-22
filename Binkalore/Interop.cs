using System;
using System.Runtime.InteropServices;

namespace Binkalore
{
    /// <summary>
    /// Compatibility notes:
    /// <see cref="CharSet.Unicode"/> is not compatible with MSS library.
    /// </summary>
    internal static class Interop
    {        
        internal delegate int AilLengthyCallback(MilesLengthyState state, uint value);

        internal delegate int AilCodecSetPreferences(string preference, int value);

        [DllImport("mss32.dll", EntryPoint = "AIL_set_redist_directory")]
        internal static extern IntPtr _AilSetRedistDirectory(string directory);

        [DllImport("mss32.dll", EntryPoint = "AIL_startup")]
        internal static extern int _AilStartup();

        [DllImport("mss32.dll", EntryPoint = "AIL_shutdown")]
        internal static extern void _AilShutdown();

        [DllImport("mss32.dll", EntryPoint = "AIL_decompress_ASI")]
        internal static extern int _AilDecompressAsi(
            [MarshalAs(UnmanagedType.LPArray)]
            byte[] inputData,
            uint inputDataSize,
            string inputFileExtension,
            ref IntPtr wavPointer,
            ref uint wavSize,
            [MarshalAs(UnmanagedType.FunctionPtr)]
            AilLengthyCallback callback);

        [DllImport("mss32.dll", EntryPoint = "AIL_last_error")]
        internal static extern IntPtr _AilLastError();

        [DllImport("mss32.dll", EntryPoint = "AIL_mem_free_lock")]
        internal static extern void _AilMemFreeLock(IntPtr pointer);

        internal enum MilesLengthyState : uint
        {
            AilLengthyInit,
            AilLengthySetPreference,
            AilLengthyUpdate,
            AilLengthyDone
        }
    }
}
