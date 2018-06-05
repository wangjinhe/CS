using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace CS.Http
{
   public class HttpsHelper
    {


       public static string GetTls(String Url, CookieContainer cookieContainer)
        {
            try
            {             
                HttpWebRequest request = null;
                Uri uri = new Uri(Url);
                request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "GET";
                request.Timeout = 100000;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:53.0) Gecko/20100101 Firefox/53.0";
            
                request.CookieContainer = cookieContainer;


                // server2003机器：The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                request.ProtocolVersion = HttpVersion.Version11;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType.Tls);

                string result = string.Empty;
                Encoding encode = Encoding.UTF8;
              
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, encode))
                        {
                            result = readStream.ReadToEnd();
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("get异常：" + ex.Message);
                return ex.Message;
            }
        }


       public static string PostTls(string Url, string postDataStr, CookieContainer cookieContainer)
        {
            try
            {

                HttpWebRequest request = null;
                Uri uri = new Uri(Url);
                request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "POST";
                request.Timeout = 15000;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
                // server2003机器：The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                request.CookieContainer = cookieContainer;
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                
                request.ProtocolVersion = HttpVersion.Version11;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType.Tls);
                           

                UTF8Encoding encoding = new UTF8Encoding();
                byte[] bytes = encoding.GetBytes(postDataStr);
                request.ContentLength = bytes.Length;
                Stream writeStream = request.GetRequestStream();
                writeStream.Write(bytes, 0, bytes.Length);

                string result = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            result = readStream.ReadToEnd();
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Post异常：" + ex.Message);
                return ex.Message;
            }
        }



       public static string SendRequest(HttpItem item)
       {
           try
           {

               HttpWebRequest request = null;
               Uri uri = new Uri(item.URL);
               request = (HttpWebRequest)WebRequest.Create(uri);

               // 这一句是赋值，要放在最前面，否则会覆盖前面添加的header
               if (item.Header != null && item.Header.Count > 0)
               {
                   request.Headers = item.Header;
               }

               request.Method = item.Method; // "POST";
               request.Timeout = item.ReadWriteTimeout;// 15000;
               if (!string.IsNullOrEmpty(item.UserAgent))
               {
                   request.UserAgent = item.UserAgent;
               }
               else
               {
                   request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
               }
               request.ServicePoint.Expect100Continue = item.Expect100Continue;

               if (!string.IsNullOrEmpty(item.ContentType))
               {
                   request.ContentType = item.ContentType;
               }
               else
               {
                   request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
               }

               request.ServicePoint.Expect100Continue = item.Expect100Continue; 

               


             


               if (item.CookieContainer != null)
               {
                   request.CookieContainer = item.CookieContainer;
               }
               if (!string.IsNullOrEmpty(item.Cookie))
               {
                   request.Headers[HttpRequestHeader.Cookie] = item.Cookie;
               }

               if (!string.IsNullOrEmpty(item.Referer))
               {
                   request.Referer = item.Referer;
               }
               if (!string.IsNullOrEmpty(item.Host))
               {
                   request.Host =item.Host;                       
               }

               

               // server2003机器：The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel
               ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;


               request.AllowAutoRedirect = item.Allowautoredirect;
              

               request.ProtocolVersion = HttpVersion.Version11;
               ServicePointManager.SecurityProtocol = (SecurityProtocolType.Tls);

               if (item.Method.ToUpper() == "POST")
               {
                   UTF8Encoding encoding = new UTF8Encoding();
                   byte[] bytes = encoding.GetBytes(item.Postdata);
                   request.ContentLength = bytes.Length;
                   Stream writeStream = request.GetRequestStream();
                   writeStream.Write(bytes, 0, bytes.Length);
               }

               Encoding encode = Encoding.UTF8;
               if (item.Encoding != null)
               {
                   encode = item.Encoding;
               }

               string result = string.Empty;
               using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
               {
                   using (Stream responseStream = response.GetResponseStream())
                   {
                       using (StreamReader readStream = new StreamReader(responseStream, encode))
                       {
                           result = readStream.ReadToEnd();
                       }
                   }
               }
               return result;
           }
           catch (Exception ex)
           {
               Console.WriteLine("异常：" + ex.Message);
               return "异常：" + ex.Message;
           }
       }


    }
}
