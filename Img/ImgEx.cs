using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Net;

namespace CS.Img
{
    /// <summary>
    /// 图片扩展类
    /// </summary>
    public  class ImgEx
    {
        /// <summary>
        /// 根据图片路径，返回ImageFormat
        /// </summary>
        /// <param name="path">图片路径</param>
        /// <returns>返回ImageFormat</returns>
        public static ImageFormat GetImgFormat(string path)
        {
            string sExt = Path.GetExtension(path);
            ImageFormat imgformat = null;
            
            switch (sExt.ToLower())
            {
                case ".gif":
                    imgformat = ImageFormat.Gif;
                    break;
                case ".png":
                    imgformat = ImageFormat.Png;
                    break;
                case ".jpg":
                case ".jpeg":
                    imgformat = ImageFormat.Jpeg;
                    break;                    
                case ".bmp":
                    imgformat = ImageFormat.Bmp;
                    break;
                case ".icon":
                    imgformat = ImageFormat.Icon;
                    break;
                default:
                    imgformat = ImageFormat.Jpeg;
                    break;
            }
            return imgformat;
        }

        /// <summary>
        /// 下载图片到本地
        /// </summary>
        /// <param name="url">图片网址</param>
        /// <param name="path">本地路径</param>
        public static void DownloadImg(string url,string path)
        {
            WebClient client = new WebClient();
            byte[] bytes = client.DownloadData(new Uri(url));
            MemoryStream ms = new MemoryStream(bytes);
            ms.Seek(0, SeekOrigin.Begin);
            ms.WriteTo(new FileStream(path, FileMode.OpenOrCreate));
        }


        public static Bitmap Base64StringToImage(string base64)
        {
            Bitmap bmp = null;
            try
            {
                if (string.IsNullOrEmpty(base64))
                {
                    return null;
                }
                byte[] arr = Convert.FromBase64String(base64);
                MemoryStream ms = new MemoryStream(arr);
                bmp = new Bitmap(ms);
                ms.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return bmp;
        }



    }
}
