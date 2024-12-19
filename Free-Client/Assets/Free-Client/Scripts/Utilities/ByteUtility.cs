using System;

namespace Assambra.FreeClient.Utilities
{
    public static class ByteUtility
    {
        /// <summary>
        /// Modifies a specific bit in a byte.
        /// </summary>
        /// <param name="originalByte">The original byte to modify.</param>
        /// <param name="bitPosition">The position of the bit to modify (0-7).</param>
        /// <param name="setValue">True to set the bit, false to clear it.</param>
        /// <returns>The modified byte.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if bitPosition is not in the range 0-7.</exception>
        public static byte ModifyBit(byte originalByte, int bitPosition, bool setValue)
        {
            if (bitPosition < 0 || bitPosition > 7)
            {
                throw new ArgumentOutOfRangeException(nameof(bitPosition), "Bit position must be between 0 and 7.");
            }

            return setValue
                ? (byte)(originalByte | (1 << bitPosition))  // Set bit
                : (byte)(originalByte & ~(1 << bitPosition)); // Clear bit
        }

        /// <summary>
        /// Converts a boolean array to a byte.
        /// </summary>
        /// <param name="boolArray">An array of exactly 8 booleans representing bits (true = 1, false = 0).</param>
        /// <returns>A byte where each bit corresponds to the boolean array values.</returns>
        /// <exception cref="ArgumentNullException">Thrown if boolArray is null.</exception>
        /// <exception cref="ArgumentException">Thrown if boolArray does not have exactly 8 elements.</exception>
        public static byte BoolArrayToByte(bool[] boolArray)
        {
            if (boolArray == null)
            {
                throw new ArgumentNullException(nameof(boolArray), "Boolean array cannot be null.");
            }

            if (boolArray.Length != 8)
            {
                throw new ArgumentException("Boolean array must have exactly 8 elements.", nameof(boolArray));
            }

            byte result = 0;
            for (int i = 0; i < 8; i++)
            {
                if (boolArray[i])
                {
                    result |= (byte)(1 << i);
                }
            }
            return result;
        }

        /// <summary>
        /// Converts a byte to a boolean array.
        /// </summary>
        /// <param name="originalByte">The byte to convert.</param>
        /// <returns>An array of 8 booleans, each representing a bit in the byte.</returns>
        public static bool[] ByteToBoolArray(byte originalByte)
        {
            var result = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                result[i] = (originalByte & (1 << i)) != 0;
            }
            return result;
        }
    }
}

