using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Web.Security;
using System.IO;

namespace CS.Security
{
    public class Password
    {
        #region 不可逆的C#加密函数参考

        /// <summary>
        /// md5加密，不可逆
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string md5(string password)
        {
            return md5(password, 32);
        }


        /// <summary>
        /// 加密用户密码
        /// </summary>
        /// <param name="password">密码</param>
        /// <param name="codeLength">多少位</param>
        /// <returns>加密密码</returns>
        public static string md5(string password, int codeLength)
        {
            if (!string.IsNullOrEmpty(password))
            {
                // 16位MD5加密（取32位加密的9~25字符）  
                if (codeLength == 16)
                {
                    return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5").ToLower().Substring(8, 16);
                }

                // 32位加密
                if (codeLength == 32)
                {
                    return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5").ToLower();
                }
            }
            return string.Empty;
        }

        #endregion

        #region C#可加密解密的函数参考。

        /// <summary>
        /// DES数据加密
        /// </summary>
        /// <param name="targetValue">目标字段</param>
        /// <returns>加密</returns>
        public static string Encrypt(string targetValue)
        {
            return Encrypt(targetValue, "Project");
        }



        /// <summary>
        /// DES数据解密
        /// </summary>
        /// <param name="targetValue">目标字段</param>
        /// <returns>解密</returns>
        public static string Decrypt(string targetValue)
        {
            return Decrypt(targetValue, "Project");
        }
        

        /// <summary>
        /// DES数据加密
        /// </summary>
        /// <param name="targetValue">目标值</param>
        /// <param name="key">密钥</param>
        /// <returns>加密值</returns>
        public static string Encrypt(string targetValue, string key)
        {
            if (string.IsNullOrEmpty(targetValue))
            {
                return string.Empty;
            }

            var returnValue = new StringBuilder();
            var des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(targetValue);
            // 通过两次哈希密码设置对称算法的初始化向量   
            des.Key = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile
                                                  (FormsAuthentication.HashPasswordForStoringInConfigFile(key, "md5").
                                                       Substring(0, 8), "sha1").Substring(0, 8));
            // 通过两次哈希密码设置算法的机密密钥   
            des.IV = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile
                                                 (FormsAuthentication.HashPasswordForStoringInConfigFile(key, "md5")
                                                      .Substring(0, 8), "md5").Substring(0, 8));
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            foreach (byte b in ms.ToArray())
            {
                returnValue.AppendFormat("{0:X2}", b);
            }
            return returnValue.ToString();
        }



        /// <summary>
        /// DES数据解密
        /// </summary>
        /// <param name="targetValue"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(string targetValue, string key)
        {
            if (string.IsNullOrEmpty(targetValue))
            {
                return string.Empty;
            }
            // 定义DES加密对象
            var des = new DESCryptoServiceProvider();
            int len = targetValue.Length / 2;
            var inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(targetValue.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            // 通过两次哈希密码设置对称算法的初始化向量   
            des.Key = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile
                                                  (FormsAuthentication.HashPasswordForStoringInConfigFile(key, "md5").
                                                       Substring(0, 8), "sha1").Substring(0, 8));
            // 通过两次哈希密码设置算法的机密密钥   
            des.IV = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile
                                                 (FormsAuthentication.HashPasswordForStoringInConfigFile(key, "md5")
                                                      .Substring(0, 8), "md5").Substring(0, 8));
            // 定义内存流
            var ms = new MemoryStream();
            // 定义加密流
            var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        #endregion

    }
}
