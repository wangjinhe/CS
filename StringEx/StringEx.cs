using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CS.StringEx
{
   public   class StringEx
    {

       public static string RemoveSpecialCharacter(string str)
       {
           if (string.IsNullOrEmpty(str))
           {
               return "";
           }
           str = str.Replace("&nbsp;", "");
           str = System.Text.RegularExpressions.Regex.Replace(str, "[^a-zA-Z0-9\\u4e00-\\u9fa5\\s]", "");

           return str;
       }

       /// <summary>
       /// mysql中字符串中含有单引号转义
       /// 
       /// </summary>
       /// <param name="fieldvalue"></param>
       /// <returns></returns>
       public static string SqlZhuanYi(string fieldvalue)
       {
           fieldvalue = fieldvalue.Replace("'", "\\\'");  // 单引号转义
           return fieldvalue;
       }


       /// <summary>

       /// 去除HTML标记

       /// </summary>

       /// <param name="NoHTML">包括HTML的源码 </param>

       /// <returns>已经去除后的文字</returns>

       public static string NoHTML(string Htmlstring)
       {

           //删除脚本

           Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);

           //删除HTML

           Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);

           Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);

           Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);

           Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

           Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);

           Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);

           Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);

           Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);

           Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);

           Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);

           Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);

           Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);

           Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);

           Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

           Htmlstring.Replace("<", "");

           Htmlstring.Replace(">", "");

           Htmlstring.Replace("\r\n", "");

           //Htmlstring=HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();

           return Htmlstring;

       }



       /// <summary>
       /// 移除空格、换行、tab
       /// </summary>
       /// <param name="str"></param>
       /// <returns></returns>
       public static string RemoveEmpty(string str)
       {
           try
           {
               if (string.IsNullOrEmpty(str))
               {
                   return "";
               }
               str = str.Replace("&nbsp;", "");
               str = System.Text.RegularExpressions.Regex.Replace(str, "\\s+", "");
               return str;
           }
           catch (Exception ex)
           {
               return str;
           }

       }



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
