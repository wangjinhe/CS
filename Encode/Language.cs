using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace CS.Encode
{
    /// <summary>
    /// 语言类编码
    /// </summary>
    public class Language
    {
        /// <summary>
        /// 将C#中特殊字符转义
        /// 特殊字符包括（\、"、'）
        /// </summary>
        /// <param name="sSrc">要转义的字符串</param>
        /// <returns>转义后的字符串</returns>
        public static string EncodeCSharp(string sSrc)
        {
            if (string.IsNullOrEmpty(sSrc))
            {
                return "";
            }
            string sDes = sSrc.Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\'", "\\\'");

            return sDes;
        }

        /// <summary>
        /// 将C#中特殊字符反转义
        /// 特殊字符包括（\、"、'）
        /// </summary>
        /// <param name="sSrc">要反转义的字符串</param>
        /// <returns>反转义后的字符串</returns>
        public static string DecodeCSharp(string sSrc)
        {
            if (string.IsNullOrEmpty(sSrc))
            {
                return "";
            }
            string sDes = sSrc.Replace( "\\\\","\\")
                .Replace("\\\"","\"")
                .Replace( "\\\'","\'");
            return sDes;
        }


        /// <summary>
        /// 将中文字符串转为Unicode编码
        /// </summary>
        /// <param name="str">中文字符串</param>
        /// <returns>返回Unicode编码</returns>
        public static string EncodeUnicode(string str)
        {
            StringBuilder strResult = new StringBuilder();
            if (!string.IsNullOrEmpty(str))
            {
                for (int i = 0; i < str.Length; i++)
                {
                    strResult.Append("\\u");
                    strResult.Append(((int)str[i]).ToString("x")); // 将每一个字符转为16进制数字
                }
            }
            return strResult.ToString();
        }

         /// <summary>
        /// 对Unicode字符解码
        /// </summary>
        /// <param name="str">Unicode编码的字符</param>
        /// <returns>返回解码后的字符</returns>
        public static string DecodeUnicode(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }

            StringBuilder strResult = new StringBuilder(str);


            try
            {
                //找出所有的中文
                MatchCollection coll = Regex.Matches(str, "\\\\u[0-9a-zA-Z]{4}");
                Dictionary<string, string> dicWords = new Dictionary<string, string>();
                for (int i = 0; i < coll.Count; i++)
                {
                    string key = coll[i].Value;
                    if (!dicWords.ContainsKey(key))
                    {
                        string value = Regex.Unescape(key);
                        dicWords.Add(key, value);
                        strResult.Replace(key, value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return strResult.ToString();

        }

        /// <summary>
        /// 将xml中特殊字符转义
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncodeXml(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            str = str.Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace(" ", "&nbsp;")
                .Replace("©", "&copy;") // 版权符号
                .Replace("®", "&reg;"); // 注册符号

            return str;
        }
        /// <summary>
        /// 将xml中特殊字符反转义
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DecodeXml(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            str = str.Replace("&amp;","&" )
                .Replace("&lt;","<")
                .Replace("&gt;",">")
                .Replace( "&quot;","\"")
                .Replace( "&nbsp;"," ")
                .Replace("&copy;","©" ) // 版权符号
                .Replace("&reg;","®"); // 注册符号

            return str;
        }


    }
}
