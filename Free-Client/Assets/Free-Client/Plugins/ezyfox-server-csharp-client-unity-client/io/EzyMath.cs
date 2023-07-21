﻿namespace com.tvd12.ezyfoxserver.client.io
{
	public sealed class EzyMath
	{
		private EzyMath()
		{
		}

		public static int bin2int(int length)
		{
			return (int)bin2long(length);
		}

		public static int bin2uint(byte[] bytes)
		{
			return (int)bin2ulong(bytes);
		}

		public static int bin2int(byte[] bytes)
		{
			return (int)bin2long(bytes);
		}

		public static long bin2long(int length)
		{
			long result = 0;
			for (int i = 0; i < length; ++i)
				result |= 1L << i;
			return result;
		}

		public static long bin2ulong(byte[] bytes)
		{
			return bin2long(bytes, true);
		}

		public static long bin2long(byte[] bytes)
		{
			return bin2long(bytes, false);
		}

		private static long bin2long(byte[] bytes, bool unsigned)
		{
			int len = bytes.Length - 1;
			long result = (long)bytes[0];
			result = unsigned ? result & 0xff : result;
			result = result << (len * 8);
			for (int i = 1; i <= len; ++i)
				result |= ((long)bytes[i] & 0xff) << ((len - i) * 8);
			return result;
		}

		public static void xor(byte[] bytes)
		{
			for (int i = 0; i < bytes.Length; ++i)
				bytes[i] ^= 0xff;
		}
	}
}