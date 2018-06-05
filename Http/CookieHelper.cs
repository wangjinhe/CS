/*1，使用httpfox抓包，headers中的cookie可以直接复制
 *2，cookie的domain很关键，要设置为httpfox抓到的域 
 * 
 */ 


using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CS.Http
{
    public static class CookieHelper
    {

        #region 静态方法



        #region Cookie简化去除重复数据
        #region 正则提取Cookie
        public static string LiteCookies(string Cookies)
        {
            try
            {
                string rStr = "";
                Cookies = Cookies.Replace("HttpOnly", "").Replace(";", "; ");
                Regex r = new Regex("(?<=,|^)(?<cookie>[^ ]+=[\\s|\"]?(?![\"]?deleted[\"]?)[^;]+)[\"]?;");
                Match m = r.Match(Cookies);
                while (m.Success)
                {
                    rStr = GetCleanCookie(rStr, m.Groups["cookie"].Value);
                    m = m.NextMatch();
                }
                return rStr;
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region 获取一次请求中的无重复Cookie
        private static string GetCleanCookie(string source, string inStr)
        {
            if (source != "" && inStr != "")
            {
                bool changed = false;
                string[] tem = source.Split(';');
                for (int i = 0; i < tem.Length; i++)
                {
                    if (tem[i].Split('=')[0] == inStr.Split('=')[0])
                    {
                        source = source.Replace(tem[i], inStr);
                        changed = true;
                    }
                }
                if (!changed) source += ";" + inStr;
                return source;
            }
            else if (inStr != "") return inStr;
            else if (source != "") return source;
            else return "";
        }
        #endregion

        #region 合并多次请求的Cookie,去掉重复部分
        public static string MergerCookies(string OldCookie, string NewCookie)
        {
            if (!string.IsNullOrEmpty(OldCookie) && !string.IsNullOrEmpty(NewCookie))
            {
                if (OldCookie == NewCookie) return OldCookie;
                else
                {
                    OldCookie = OldCookie.Replace(" ", "").Replace("path=/;", "").Replace("HttpOnly",""); // 去掉空格
                    NewCookie = NewCookie.Replace(" ", "").Replace("path=/;", "").Replace("HttpOnly", "");
                    List<string> Old = new List<String>(OldCookie.Split(';'));
                    List<string> New =  new List<String>(NewCookie.Split(';'));
                  
                    foreach (string n in New)
                    {
                        foreach (string o in Old)
                        {
                            if (o == n || o.Split('=')[0] == n.Split('=')[0])
                            {
                                Old.Remove(o);
                                break;
                            }
                        }
                    }
                    List<string> list = new List<string>(Old);
                    list.AddRange(New);
                    return string.Join(";", list.ToArray());
                }
            }
            else if (!string.IsNullOrEmpty(OldCookie)) return OldCookie;
            else if (!string.IsNullOrEmpty(NewCookie)) return NewCookie;
            else return "";
        }

       
        #endregion

        #endregion

        #endregion




        /// <summary>
        /// 把文本格式Cookie转换为CookieContainer
        /// </summary>
        /// <param name="cookie">cookie</param>
        /// <param name="url">url</param>
        /// <returns></returns>
        public static CookieContainer ToCookieContainer(this string cookie, string domain)
        {
            CookieContainer credence = new CookieContainer();
            cookie = cookie.Trim();
            cookie = cookie.EndsWith(";") ? cookie : string.Concat(cookie, ";");

            var kv = cookie.Split(';');

            foreach (var item in kv)
            {
                try
                {
                    var value = item.Split('=');

                    var c = new Cookie()
                    {
                        Name = value[0].Trim(),
                        Value = value.Length.Equals(2) ? value[1] : string.Empty,
                        Domain = domain,
                        Path = "/",
                        Expires = DateTime.Now.AddDays(1)
                    };

                    credence.Add(c);
                }
                catch (Exception ex)
                {
                    Console.WriteLine( ex.Message);
                }
            }
            return credence;
        }

        /// <summary>
        /// 把文本格式Cookie转换为CookieCollection
        /// </summary>
        /// <param name="cookie">cookie</param>
        /// <param name="url">url</param>
        /// <returns></returns>
        public static CookieCollection ToCookieCollection(this string cookie, string domain)
        {
            CookieCollection coll = new CookieCollection();
            // CookieContainer credence = new CookieContainer();
            cookie = cookie.Trim();
            cookie = cookie.EndsWith(";") ? cookie : string.Concat(cookie, ";");

            var kv = cookie.Split(';');

            foreach (var item in kv)
            {
                try
                {
                    var value = item.Split('=');

                    var c = new Cookie()
                    {
                        Name = value[0].Trim(),
                        Value = value.Length.Equals(2) ? value[1] : string.Empty,
                        Domain = domain,
                        Path = "/",
                        //Expires = DateTime.Now
                    };
                    coll.Add(c);
                    //credence.Add(c);
                }
                catch { }
            }
            return coll;
        }


        public static void AddCookie(this CookieContainer credence, string key, string value, string domain)
        {
            var c = new Cookie()
            {
                Name = key,
                Value = value,
                Domain = domain,
                Path = "/",
                Expires = DateTime.Now
            };

            credence.Add(c);
        }

        public static CookieContainer ToCookieContainer(this Cookie[] cookies, string domain)
        {
            if (cookies == null || cookies.Length == 0)
                return new CookieContainer();

            var cookieStrs = cookies.Select(c => c.ToString()).ToArray();
            var sb = new StringBuilder();
            foreach (var cookie in cookies)
            {
                sb.Append(string.Concat(cookie, ";"));
            }
            return sb.ToString().ToCookieContainer(domain);
        }

        /// <summary>
        /// 把CookieContainer转换为文本格式的Cookie
        /// </summary>
        /// <param name="credence"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ToCookie(this CookieContainer credence, string url)
        {
            return credence.GetCookieHeader(new Uri(url));
        }

        /// <summary>
        /// 把CookieContainer转换为文本格式的Cookie
        /// </summary>
        /// <param name="credence"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ToCookie(this CookieContainer credence)
        {
            var cookies = GetCookies(credence).Select(c => c.ToString()).ToArray();
            if (cookies == null || cookies.Length == 0)
                return string.Empty;

            var sb = new StringBuilder();
            foreach (var cookie in cookies)
            {
                sb.Append(string.Concat(cookie, ";"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 更新cookie域
        /// </summary>
        /// <param name="credence"></param>
        /// <param name="domain"></param>
        public static CookieContainer UpdateDomain(this CookieContainer credence, string domain)
        {
            var cookie = credence.ToCookie();
            return cookie.ToCookieContainer(domain);
        }

        /// <summary>
        /// 获取CookieContainer里的所有Cookie
        /// </summary>
        /// <param name="credence"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Cookie[] GetCookies(this CookieContainer credence)
        {
            var cookies = new List<Cookie>();

            var domains = (Hashtable)credence.GetType().InvokeMember(
                "m_domainTable",
                BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null,
                credence,
                new object[] { });

            foreach (object pathList in domains.Values)
            {
                var lstCookieCol = (SortedList)pathList.GetType().InvokeMember(
                    "m_list",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance,
                    null,
                    pathList,
                    new object[] { });

                foreach (CookieCollection colCookies in lstCookieCol.Values)
                {
                    foreach (Cookie c in colCookies)
                    {
                        cookies.Add(c);
                    }
                }
            }

            return cookies.ToArray();
        }

        /// <summary>
        /// 获取cookie值
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetCookieValue(string cookie, string key)
        {
            var d = cookie.Split(';');
            for (var b = 0; b < d.Length; b++)
            {
                if (d[b].IndexOf(key) > -1)
                {
                    var a = d[b].Split('=');
                    if (a[0].Trim() == key)
                    {
                        return a[1];
                    }
                }
            }
            return string.Empty;
        }
    }
}
