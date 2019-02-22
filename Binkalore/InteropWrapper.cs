using System;
using System.Runtime.InteropServices;

using static Binkalore.Interop;
using static Binkalore.Utilities;

namespace Binkalore
{
    internal static class InteropWrapper
    {
        private const string BinkAudioExtension = ".binka";

        internal static AilLengthyCallback _callback = Callback;

        private static int Callback(MilesLengthyState state, uint value)
        {
            switch (state)
            {
                case MilesLengthyState.AilLengthyDone:
                    // PrintDebug("Conversion complete.");
                    break;
                case MilesLengthyState.AilLengthyInit:
                    // PrintDebug("Conversion initialized.");
                    break;
                case MilesLengthyState.AilLengthySetPreference:
                    // PrintDebug("Conversion codec preference setter received.");
                    break;
                case MilesLengthyState.AilLengthyUpdate:
                    // PrintDebug($"Conversion percentage updated.");
                    break;
            }
            return 1;
        }

        public static void AilSetRedistDirectory(string directory)
        {
            var currentDirectory = _AilSetRedistDirectory(directory).ToStringAnsi();
            PrintDebug($"Miles directory is set to \"{currentDirectory}\".");
        }

        public static void AilStartup()
        {
            var result = _AilStartup();
            if (result == 0)
            {
                PrintDebug("Miles Sound System is already initialized.");
            }
            else
            {
                PrintDebug($"Miles Sound System is successfully initialized.");
            }
        }

        public static void AilShutdown()
        {
            _AilShutdown();
            PrintDebug("Miles Sound System is now shut down.");
        }

        public static string AilDecompressAsi(byte[] inputData, out byte[] wavData)
        {
            wavData = null;
            var wavPointer = new IntPtr();
            uint wavSize = 0;

            var result = _AilDecompressAsi(
                inputData,
                (uint)inputData.Length,
                BinkAudioExtension,
                ref wavPointer,
                ref wavSize,
                _callback);

            if (result == 1)
            {
                PrintDebug("ASI decompression successful.");
                wavData = new byte[wavSize];
                Marshal.Copy(wavPointer, wavData, 0, (int)wavSize);
                AilMemFreeLock(wavPointer);
                return String.Empty;
            }
            else
            {
                PrintDebug("ASI decompression failed.");
                return AilLastError();
            }
        }

        public static string AilLastError()
        {
            var lastError = _AilLastError();
            return lastError.ToStringAnsi();
        }

        private static void AilMemFreeLock(IntPtr pointer)
        {
            _AilMemFreeLock(pointer);
        }
    }
}
