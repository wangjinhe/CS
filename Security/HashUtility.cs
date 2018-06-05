using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CS.Security
{
	/// <summary>
	/// MD5 哈希辅助类
	/// </summary>
	public static class HashUtility
	{
        /// <summary>
        /// 转换为16进制的字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		public static string ToHexString(byte[] data)
		{
			var sb = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
			{
				sb.Append(data[i].ToString("X2"));
			}

			return sb.ToString();
		}

        /// <summary>
        /// 转换为字节
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
		public static byte[] ToBytes(string hex)
		{
			var list = new List<byte>();

			for (int i = 0; i < hex.Length; i += 2)
			{
				var b = Convert.ToByte(hex.Substring(i, 2), 16);
				list.Add(b);
			}

			return list.ToArray();
		}

		/// <summary>
		/// 获取哈希码
		/// </summary>
		public static string Hash(byte[] data)
		{
			using (var hasher = global::System.Security.Cryptography.MD5.Create())
			{
				return ToHexString(hasher.ComputeHash(data));
			}
		}

		/// <summary>
		/// 获取哈希码
		/// </summary>
		public static string Hash(string s)
		{
			var data = Encoding.UTF8.GetBytes(s);
			return Hash(data);
		}

		/// <summary>
		/// 获取文件哈希码
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static string HashFile(string filename)
		{
			return Hash(File.ReadAllBytes(filename));
		}

		/// <summary>
		/// 校验哈希码
		/// </summary>
		public static bool Verify(string s, string hash)
		{
			string hashOfInput = Hash(s);
			return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, hash) == 0;
		}
	}
}
