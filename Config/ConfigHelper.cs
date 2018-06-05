using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CS.Config
{
    public class ConfigHelper
    {
        /// <summary>
        /// 返回AppSetting中key对应的值
        /// 不存在返回""
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSettingValue(string key)
        {
            string value = System.Configuration.ConfigurationManager.AppSettings[key];
            return value == null ? "" : value;
        }

    }
}
