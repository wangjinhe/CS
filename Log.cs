/*LogLevel 配置为：
 * -1：不显示日志
 * 0：显示info和error
 * 1：显示error;
 * 默认显示Error和info
 * 
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CS
{

    public class Log
    {
        /// <summary>
        /// 日志级别
        /// 
        ///  -1 不显示日志
        ///  0 info
        ///  1 error 
        ///  
        ///         
        /// </summary>
        private static int LogLevel;

        private static string dir = AppDomain.CurrentDomain.BaseDirectory + "log\\";       

        static Log()
        {
            string sLogLevel = System.Configuration.ConfigurationManager.AppSettings["LogLevel"];
            if (string.IsNullOrEmpty(sLogLevel)) // 默认不显示日志
            {
                LogLevel = 0;
            }
            else
            {
                LogLevel = Convert.ToInt32(sLogLevel);
            }
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

        }


        /// <summary>
        /// 记录普通的日志信息保存到年月日命名的文件中
        /// </summary>
        /// <param name="sInfo"></param>
        public static void Info(string sInfo)
        {
            if (LogLevel <= -1)
            {
                return;
            }
            if (LogLevel > 0)
            {
                return;
            }
            string path =dir + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            using (StreamWriter writer = File.AppendText(path))
            {
                lock (writer)
                {
                    writer.WriteLine(DateTime.Now.ToLocalTime() + " Info:\r\n  " + sInfo);
                }
            }

        }

        /// <summary>
        /// 保存错误信息到年月日命名的文件中
        /// </summary>
        /// <param name="sError"></param>
        public static void Error(string sError)
        {
            if (LogLevel <= -1)
            {
                return;
            }
            if (LogLevel > 1)
            {
                return;
            }

            string path = dir + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            using (StreamWriter writer = File.AppendText(path))
            {
                lock (writer)
                {
                    writer.WriteLine(DateTime.Now.ToLocalTime() + "  Error:\r\n  " + sError);
                }
            }

        }





    }
}

