using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CS.Serializer
{
    public class JsonHelper
    {

        /// <summary>
        /// 从json对象中获取value
        /// </summary>
        /// <param name="json">json对象</param>
        /// <param name="key">关键词</param>
        /// <param name="type">将value转换的目标数据类型</param>
        /// <returns></returns>
        public static object GetJsonValue(dynamic json, string key, Type type)
        {
            object o = null;

            try
            {

                if (json != null && json.ContainsKey(key))
                {
                    o = json[key];                   
                }

                if (type == typeof(String))
                {
                    o = Convert.ToString(o);
                }
                else
                {
                    if (o == null || o == "")
                    {
                        o = "0";
                    }
                    if (type == typeof(Int32))
                    {
                        o = Convert.ToInt32(o);
                    }
                    else if (type == typeof(Double))
                    {
                        o = Convert.ToDouble(o);
                    }
                    else if (type == typeof(Boolean))
                    {
                        o = Convert.ToBoolean(o);
                    }
                    else if (type == typeof(long))
                    {
                        o = (long)o;
                    }
                    else if (type == typeof(float))
                    {
                        o = (float)o;
                    }
                }              

               
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取{0}异常:{1}", key, ex.Message);
            }
            return o;
        }
    }
}
