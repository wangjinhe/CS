using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CS.StringEx
{
    /// <summary>
    /// 封装常见的正则表达式
    /// </summary>
    public  class RegexEx
    {
        /// <summary>
        /// 判断是否为有效的ip
        /// </summary>
        /// <param name="input"></param>
        /// <returns>是返回true,否返回false</returns>
        public static bool IsIP(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length > 15)
            {
                return false;
            }
            //  ((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)
            string pattern = "((2[0-4]\\d|25[0-5]|[01]?\\d\\d?)\\.){3}(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)";
            Match m = Regex.Match(input, pattern);
            if (m != null && m.Success && m.Value == input) // 为避免包含，将匹配的值判断是否相等
            {
                return true;
            }
            return false;
        
        }


    }
}
