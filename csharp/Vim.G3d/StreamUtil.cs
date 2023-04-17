using System.IO;

namespace Vim.G3d
{
    public static class StreamUtil
    {
        /// <summary>
        /// Updates the seek head to consume the stream and returns false.
        /// </summary>
        public static bool ReadFailure(this Stream stream, long size)
        {
            // Update the seek head to consume the stream and return false.
            stream.Seek((int)size, SeekOrigin.Current);
            return false;
        }
    }
}
