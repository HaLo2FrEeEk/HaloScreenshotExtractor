using System.IO;

namespace HaloScreenshots
{
    internal static class BRExtensions
    {
        /// <summary>
        /// Allows me to read Big Endian int32 values
        /// </summary>
        /// <returns>Big Endian int32</returns>
        public static int ReadBigInt32(this BinaryReader x)
        {
            byte[] buff = x.ReadBytes(4);
            return (buff[0] << 24 | buff[1] << 16 | buff[2] << 8 | buff[3]);
        }

        /// <summary>
        /// Allows me to read Big Endian int16 values
        /// </summary>
        /// <returns>Big Endian int16</returns>
        public static short ReadBigInt16(this BinaryReader x)
        {
            byte[] buff = x.ReadBytes(2);
            return (short)(buff[0] << 8 | buff[1]);
        }

        /// <summary>
        /// Allows me to read Big Endian int24 values
        /// </summary>
        /// <returns>Big Endian int24</returns>
        public static int ReadBigInt24(this BinaryReader x)
        {
            byte[] buff = x.ReadBytes(3);
            return (buff[0] << 16 | buff[1] << 8 | buff[2]);
        }

        /// <summary>
        /// Allows me to read Little Endian int24 values
        /// </summary>
        /// <returns>Little Endian int24</returns>
        public static int ReadInt24(this BinaryReader x)
        {
            byte[] buff = x.ReadBytes(3);
            return (buff[2] << 16 | buff[1] << 8 | buff[0]);
        }
    }
}