using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace CS.Http
{
    /// <summary>
    /// 访问https
    /// </summary>
    public class DownLoadSSL
    {


        public static string Get(string url, string referer, string encode, CookieContainer cookie)
        {

            System.Net.ServicePointManager.Expect100Continue = false;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);

            Uri responseUri = null;
            HttpWebResponse response = null;
            Stream stream = null;
            StreamReader streamReader = null;

            string source = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.CookieContainer = cookie;
                request.Method = "GET";
                //request.Host = "mdskip.taobao.com";
                request.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, application/x-shockwave-flash, */*";
                request.Referer = referer;
                request.Headers.Add("Accept-Language:zh-cn");
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
                request.KeepAlive = true;

                response = (HttpWebResponse)request.GetResponse();
                responseUri = response.ResponseUri;

                stream = response.GetResponseStream();
                streamReader = new StreamReader(stream, Encoding.GetEncoding(encode));
                source = streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (streamReader != null)
                {
                    streamReader.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
            }

            return source;
        }


        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

    }

}
