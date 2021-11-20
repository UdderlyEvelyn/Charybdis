using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Diagnostics.CodeAnalysis;

namespace Charybdis.Library.Core
{
    /// <summary>
    /// (Secure) Random Number Generatorbefore moment
    /// Research: http://stackoverflow.com/questions/14983336/guid-newguid-vs-a-random-string-generator-from-random-next
    /// </summary>
    //No examples here, it's too straightforward to need them.
    [ExcludeFromCodeCoverage] //Randomness can't really be verified via AUT.
    public static class RNG
    {
        /// <summary>
        /// Generates a random sbyte (8-bit integer) value (convenience function since bytes and sbytes are equivalent in the case of random data).
        /// </summary>
        /// <returns></returns>
        public static sbyte GetSByte()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[1];
            rng.GetBytes(bytes);
            return (sbyte)bytes[0];
        }

        /// <summary>
        /// Generates a random short (16-bit integer) value.
        /// </summary>
        /// <returns></returns>
        public static short GetShort()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[2];
            rng.GetBytes(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }

        /// <summary>
        /// Generates a random int (32-bit integer) value.
        /// </summary>
        /// <returns></returns>
        public static int GetInt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[4];
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// Generates a random long (64-bit integer) value.
        /// </summary>
        /// <returns></returns>
        public static long GetLong()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[8];
            rng.GetBytes(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }

        /// <summary>
        /// Generates a random byte (8-bit unsigned integer) value.
        /// </summary>
        /// <returns></returns>
        public static byte GetByte()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[1];
            rng.GetBytes(bytes);
            return bytes[0];
        }

        /// <summary>
        /// Generates a byte array of random byte (8-bit unsigned integer) values.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static byte[] GetBytes(int count)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[count];
            rng.GetBytes(bytes);
            return bytes;
        }

        /// <summary>
        /// Generates a random ushort (16-bit unsigned integer) value.
        /// </summary>
        /// <returns></returns>
        public static ushort GetUShort()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[2];
            rng.GetBytes(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }

        /// <summary>
        /// Generates a random uint (32-bit unsigned integer) value.
        /// </summary>
        /// <returns></returns>
        public static uint GetUInt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[4];
            rng.GetBytes(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        /// <summary>
        /// Generates a random ulong (64-bit unsigned integer) value.
        /// </summary>
        /// <returns></returns>
        public static ulong GetULong()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[8];
            rng.GetBytes(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }
    }
}
