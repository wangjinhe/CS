/*

Base64编码说明
　　Base64编码要求把3个8位字节（3*8=24）转化为4个6位的字节（4*6=24），
    之后在6位的前面补两个0，形成8位一个字节的形式。 
    如果剩下的字符不足3个字节，则用0填充，在最后编码完成后在结尾添加1到2个 “=”。
　　为了保证所输出的编码位可读字符，Base64制定了一个编码表，以便进行统一转换。
  编码表的大小为2^6=64，这也是Base64名称的由来。
  注：BASE64字符表：ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using CS.Img;

namespace CS.Encode
{
    /// <summary>
    /// Base64操作
    /// </summary>
    public class Base64
    {


        /// <summary>
        /// 将图片进行base64位编码
        /// </summary>
        /// <param name="path">图片路径</param>
        /// <returns>返回图片编码后的字符串</returns>
        public static string EncodeImg(string path)
        {
            MemoryStream ms = new MemoryStream();
            Bitmap bitmap = new Bitmap(path);
            
            bitmap.Save(ms, ImgEx.GetImgFormat(path));
            byte [] bytes = ms.GetBuffer();
            string sResult = Convert.ToBase64String(bytes);
            return sResult;
        }

        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="str">要编码的字符串</param>
        /// <returns>返回编码后的字符串</returns>
        public static string Encode(string str)
        {
            if (str == null)
            {
                return "";
            }
            byte[] bytes = Encoding.Default.GetBytes(str);
            string sResult = Convert.ToBase64String(bytes);
            return sResult;

           
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="str">要解码的字符串</param>
        /// <returns>返回解码后的字符串</returns>
        public static string Decode(string str)
        {
            if (str == null)
            {
                return "";
            }
            byte[] bytes = Convert.FromBase64String(str);
            string sResult = Encoding.Default.GetString(bytes);
            return sResult;
        }

        /// <summary>
        /// 解析64位编码的图片，保存到filename
        /// </summary>
        /// <param name="sBase64Img">64位编码的图片</param>
        /// <param name="filename">要保存的图片路径</param>
        public static void DecodeImg(string sBase64Img, string filename)
        {
            byte[] bytes = Convert.FromBase64String(sBase64Img);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(bytes);
            Bitmap bitmap = new Bitmap(stream);
            bitmap.Save(filename);            
        }


    }
}
