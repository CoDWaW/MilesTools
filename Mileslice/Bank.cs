using System.Collections.Generic;
using System.IO;

using static Mileslice.Utilities;

namespace Mileslice
{
    /// <summary>
    /// Utility class for gathering information from a stream bank.
    /// </summary>
    internal static class Bank
    {
        /// <summary>
        /// Gets a list of all Bink Audio header offsets from a stream bank file.
        /// </summary>
        /// <param name="bankFile">A stream of the stream bank file.</param>
        /// <returns>A list of all header offsets in the file.</returns>
        internal static List<uint> GetAllHeaders(FileStream bankFile)
        {
            uint currentOffset = 0;
            var bytesRead = 0;
            var buffer = new byte[Program.BufferSize];
            var headerOffsets = new List<uint>();

            uint headerOffset = 0;
            int bytesMatched = 0;

            do
            {
                // Read fresh buffer
                bytesRead = bankFile.Read(buffer, 0, Program.BufferSize);
                if (bytesRead == 0)
                {
                    // This can happen when the previous {BufferSize} bytes exactly ends the file
                    break;
                }

                // Buffer filled, now search for headers
                for (int i = 0; i < bytesRead; i++)
                {
                    if (buffer[i] == Program.HeaderMagic[0])
                    {
                        // First byte matched, note the offset
                        headerOffset = currentOffset + (uint)i;
                        bytesMatched = 1;
                        continue;
                    }
                    if (bytesMatched > 0)
                    {
                        // A header match is in progress
                        if (buffer[i] == Program.HeaderMagic[bytesMatched])
                        {
                            // And another byte matches
                            bytesMatched++;
                        }
                        else
                        {
                            // Byte match failure, reset count
                            bytesMatched = 0;
                            continue;
                        }

                        if (bytesMatched == Program.HeaderMagic.Length)
                        {
                            // And a header match is found
                            headerOffsets.Add(headerOffset);
                            bytesMatched = 0;
                            continue;
                        }
                    }
                }

                currentOffset += (uint)bytesRead;
            }
            while (bytesRead == Program.BufferSize);

            return headerOffsets;
        }

        /// <summary>
        /// Calculates a list of Bink Audio file sizes from a list of header offsets.
        /// </summary>
        /// <param name="bankFileSize">The size of the stream bank file.</param>
        /// <param name="headerOffsets">A list of all header offsets in the file.</param>
        /// <returns>A list of file lengths corresponding to the provided offset list.</returns>
        internal static List<uint> GetAllFileSizes(long bankFileSize, List<uint> headerOffsets)
        {
            var fileSizes = new List<uint>();

            for (int i = 0; i < headerOffsets.Count; i++)
            {
                uint fileSize = 0;
                if (i < headerOffsets.Count - 1)
                {
                    // Not the last slice
                    fileSize = headerOffsets[i + 1] - headerOffsets[i];
                }
                else
                {
                    // The last slice
                    fileSize = (uint)(bankFileSize - headerOffsets[i]);
                    PrintDebug($"Last file slice has a length of {fileSize}.");
                }

                fileSizes.Add(fileSize);
            }

            return fileSizes;
        }

    }
}
