using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;

namespace CS.Time
{
    public class TimeHelper
    {
        #region unix时间   　
        //时间戳是自 1970 年 1 月 1 日（00:00:00 GMT）以来的秒数。它也被称为 Unix 时间戳（Unix Timestamp）。

        /// <summary>
        /// 将unix时间戳转为时间
        /// 单位是毫秒
        /// </summary>
        /// <param name="timeStamp">单位是毫秒</param>
        /// <returns>转换失败返回时间最小值</returns>
        public static DateTime GetTimeFromUnixMilliseconds(string milliseconds)
        {
            try
            {
                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                double dTime = double.Parse(milliseconds);
                return dtStart.AddMilliseconds(dTime);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// 将unix时间戳转为时间
        /// 单位是秒
        /// </summary>
        /// <param name="timeStamp">单位是秒</param>
        /// <returns>转换失败返回时间最小值</returns>
        public static DateTime GetTimeFromUnixSeconds(string seconds)
        {
            try
            {
                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                double dTime = double.Parse(seconds);
                return dtStart.AddSeconds(dTime);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// 将时间转为unix时间戳
        /// 单位是秒
        /// </summary>
        /// <param name="time"></param>
        /// <returns>转换失败返回0</returns>
        public static long GetUnixSecondsFromTime(DateTime time)
        {
            try
            {
                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                long t = (long)(time - dtStart).TotalSeconds;
                return t;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }
        }

        /// <summary>
        /// 将时间转为unix时间戳
        /// 单位是毫秒
        /// </summary>
        /// <param name="time"></param>
        /// <returns>转换失败返回0</returns>
        public static long GetUnixMillisecondsFromTime(DateTime time)
        {
            try
            {
                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                long t = (long)(time - dtStart).TotalMilliseconds;
                return t;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }
        }

        #endregion

        #region 获取网络时间

        public static DateTime GetNowTime()
        {
            WebClient client = new WebClient();
            string url = "http://www.hko.gov.hk/cgi-bin/gts/time5a.pr?a=2";
            string source =  client.DownloadString(url);
           
            Regex regex = new Regex(@"0=(?<timestamp>\d{10})\d+");
            Match match = regex.Match(source);
            if (match.Success)
            {
                return GetTime(match.Groups["timestamp"].Value);
            }
            return DateTime.Now;
        }

        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name=”timeStamp”></param>
        /// <returns></returns>
        public static DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime); 
            return dtStart.Add(toNow);
        }


        #endregion



    }
}
