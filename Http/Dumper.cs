using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CS.Http
{
    /// <summary>
    /// 解析Request.InputStream为字符串
    /// </summary>
    public class Dumper
    {
        public static byte[] Dump(Stream stream)
        {
            byte[] buffer = new byte[0];
            MemoryStream stream2 = new MemoryStream();
            try
            {
                int count = 0x400;
                byte[] buffer2 = new byte[count];
                while (count > 0)
                {
                    int num2 = stream.Read(buffer2, 0, count);
                    stream2.Write(buffer2, 0, num2);
                    count = num2;
                }
                buffer = stream2.ToArray();
            }
            catch
            {
            }
            finally
            {
                stream2.Close();
            }
            return buffer;
        }


        public static string Dump(Stream stream, Encoding encoding)
        {
            byte[] bytes = Dump(stream);
            return encoding.GetString(bytes);
        }

 


    }
}
