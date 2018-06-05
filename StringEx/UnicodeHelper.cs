using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CS.StringEx
{
    public class UnicodeHelper
    {
        /// <summary>
        /// 判断是否含有中文
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool HasChinese(string text)
        {
            try
            {
                if (Regex.Match(text, "[\\u4e00-\\u9fa5]").Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
