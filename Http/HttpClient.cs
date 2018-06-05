/*

HttpClient client=new HttpClient(url);
string html=client.GetString();

GetString()函数内部会查找Http Headers, 以及HTML的Meta标签,试图找出获取的内容的编码信息.如果都找不到,它会使用client.DefaultEncoding, 这个属性默认为utf-8, 也可以手动设置.

自动保持Cookie, Referer

HttpClient client=new HttpClient(url1, null, true);
string html1=client.GetString();
client.Url=url2;
string html2=client.GetString();

这里HttpClient的第三个参数,keepContext设置为真时,HttpClient会自动记录每次交互时服务器对Cookies进行的操作,同时会以前一次请求的Url为Referer.在这个例子里,获取html2时,会把url1作为Referer, 同时会向服务器传递在获取html1时服务器设置的Cookies. 当然,你也可以在构造HttpClient时直接提供第一次请求要发出的Cookies与Referer:

HttpClient client=new HttpClient(url, new WebContext(cookies, referer), true);

或者,在使用过程中随时修改这些信息:

client.Context.Cookies=cookies;
client.Context.referer=referer;

模拟HTML表单提交

HttpClient client=new HttpClient(url);
client.PostingData.Add(fieldName1, filedValue1);
client.PostingData.Add(fieldName2, fieldValue2);
string html=client.GetString();

上面的代码相当于提交了一个有两个input的表单. 在PostingData非空,或者附加了要上传的文件时(请看下面的上传和文件), HttpClient会自动把HttpVerb改成POST, 并将相应的信息附加到Request上.

向服务器上传文件

HttpClient client=new HttpClient(url);
client.AttachFile(fileName, fieldName);
client.AttachFile(byteArray, fileName, fieldName);
string html=client.GetString();

这里面的fieldName相当于<input type="file" name="fieldName" />里的fieldName. fileName当然就是你想要上传的文件路径了. 你也可以直接提供一个byte[] 作为文件内容, 但即使如此,你也必须提供一个文件名,以满足HTTP规范的要求.

不同的返回形式


字符串: string html = client.GetString();
流: Stream stream = client.GetStream();
字节数组: byte[] data = client.GetBytes();
保存到文件:  client.SaveAsFile(fileName);
或者,你也可以直接操作HttpWebResponse: HttpWebResponse res = client.GetResponse();

每调用一次上述任何一个方法,都会导致发出一个HTTP Request, 也就是说,你不能同时得到某个Response的两种返回形式.
另外,调用后它们任意一个之后,你可以通过client.ResponseHeaders来获取服务器返回的HTTP头.
下载资源的指定部分(用于断点续传,多线程下载)

HttpClient client=new HttpClient(url);
//发出HEAD请求,获取资源长度
int length=client.HeadContentLength();

//只获取后一半内容

client.StartPoint=length/2;
byte[] data=client.GetBytes();

HeadContentLength()只会发出HTTP HEAD请求.根据HTTP协议, HEAD与GET的作用等同, 但是,只返回HTTP头,而不返回资源主体内容. 也就是说,用这个方法,你没法获取一个需要通过POST才能得到的资源的长度,如果你确实有这样的需求,建议你可以通过GetResponse(),然后从ResponseHeader里获取Content-Length.

 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using System.Web;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace CS.Http
{
    public class HttpClient
    {
        #region deerchao
        #region fields
        private bool keepContext;
        private string defaultLanguage = "zh-CN";
        private Encoding defaultEncoding = Encoding.UTF8;
        private string accept = "*/*";
        private string userAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        private HttpVerb verb = HttpVerb.GET;
        private HttpClientContext context;
        private readonly List<HttpUploadingFile> files = new List<HttpUploadingFile>();
        private readonly PostDataList postingData = new PostDataList();
        private string url;
        private WebHeaderCollection responseHeaders;
        private int startPoint;
        private int endPoint;
        private bool keepAlive = false;
        private string defalutdomain = "eeeff.com";
        #endregion

        #region events
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        private void OnStatusUpdate(StatusUpdateEventArgs e)
        {
            EventHandler<StatusUpdateEventArgs> temp = StatusUpdate;

            if (temp != null)
                temp(this, e);
        }
        #endregion

        #region properties

        public string Defalutdomain
        {
            set { defalutdomain = value; }
        }
        /// <summary>
        /// 是否自动在不同的请求间保留Cookie, Referer
        /// </summary>
        public bool KeepContext
        {
            get { return keepContext; }
            set { keepContext = value; }
        }

        /// <summary>
        /// 期望的回应的语言
        /// </summary>
        public string DefaultLanguage
        {
            get { return defaultLanguage; }
            set { defaultLanguage = value; }
        }

        /// <summary>
        /// GetString()如果不能从HTTP头或Meta标签中获取编码信息,则使用此编码来获取字符串
        /// </summary>
        public Encoding DefaultEncoding
        {
            get { return defaultEncoding; }
            set { defaultEncoding = value; }
        }

        /// <summary>
        /// 指示发出Get请求还是Post请求
        /// </summary>
        public HttpVerb Verb
        {
            get { return verb; }
            set { verb = value; }
        }

        /// <summary>
        /// 要上传的文件.如果不为空则自动转为Post请求
        /// </summary>
        public List<HttpUploadingFile> Files
        {
            get { return files; }
        }

        /// <summary>
        /// 要发送的Form表单信息
        /// </summary>
        public PostDataList PostingData
        {
            get { return postingData; }
        }

        /// <summary>
        /// 获取或设置请求资源的地址
        /// </summary>
        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        /// <summary>
        /// 用于在获取回应后,暂时记录回应的HTTP头
        /// </summary>
        public WebHeaderCollection ResponseHeaders
        {
            get { return responseHeaders; }
        }

        /// <summary>
        /// 获取或设置期望的资源类型
        /// </summary>
        public string Accept
        {
            get { return accept; }
            set { accept = value; }
        }

        /// <summary>
        /// 获取或设置请求中的Http头User-Agent的值
        /// </summary>
        public string UserAgent
        {
            get { return userAgent; }
            set { userAgent = value; }
        }

        /// <summary>
        /// 获取或设置Cookie及Referer
        /// </summary>
        public HttpClientContext Context
        {
            get { return context; }
            set { context = value; }
        }

        /// <summary>
        /// 获取或设置获取内容的起始点,用于断点续传,多线程下载等
        /// </summary>
        public int StartPoint
        {
            get { return startPoint; }
            set { startPoint = value; }
        }

        /// <summary>
        /// 获取或设置获取内容的结束点,用于断点续传,多下程下载等.
        /// 如果为0,表示获取资源从StartPoint开始的剩余内容
        /// </summary>
        public int EndPoint
        {
            get { return endPoint; }
            set { endPoint = value; }
        }

        public bool KeepAlive
        {
            get { return keepAlive; }
            set { keepAlive = value; }
        }

        #endregion

        #region constructors
        /// <summary>
        /// 构造新的HttpClient实例
        /// </summary>
        public HttpClient()
            : this(null)
        {
        }

        /// <summary>
        /// 构造新的HttpClient实例
        /// </summary>
        /// <param name="url">要获取的资源的地址</param>
        public HttpClient(string url)
            : this(url, null)
        {
        }

        /// <summary>
        /// 构造新的HttpClient实例
        /// </summary>
        /// <param name="url">要获取的资源的地址</param>
        /// <param name="context">Cookie及Referer</param>
        public HttpClient(string url, HttpClientContext context)
            : this(url, context, true)
        {
        }

        /// <summary>
        /// 构造新的HttpClient实例
        /// </summary>
        /// <param name="url">要获取的资源的地址</param>
        /// <param name="context">Cookie及Referer</param>
        /// <param name="keepContext">是否自动在不同的请求间保留Cookie, Referer</param>
        public HttpClient(string url, HttpClientContext context, bool keepContext)
        {
            this.url = url;
            this.context = context;
            this.keepContext = keepContext;
            if (this.context == null)
                this.context = new HttpClientContext();
        }
        #endregion

        #region AttachFile
        /// <summary>
        /// 在请求中添加要上传的文件
        /// </summary>
        /// <param name="fileName">要上传的文件路径</param>
        /// <param name="fieldName">文件字段的名称(相当于&lt;input type=file name=fieldName&gt;)里的fieldName)</param>
        public void AttachFile(string fileName, string fieldName)
        {
            HttpUploadingFile file = new HttpUploadingFile(fileName, fieldName);
            files.Add(file);
        }

        /// <summary>
        /// 在请求中添加要上传的文件
        /// </summary>
        /// <param name="data">要上传的文件内容</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fieldName">文件字段的名称(相当于&lt;input type=file name=fieldName&gt;)里的fieldName)</param>
        public void AttachFile(byte[] data, string fileName, string fieldName, string contentType)
        {
            HttpUploadingFile file = new HttpUploadingFile(data, fileName, fieldName, contentType);
            files.Add(file);
        }
        #endregion

        /// <summary>
        /// 清空PostingData, Files, StartPoint, EndPoint, ResponseHeaders, 并把Verb设置为Get.
        /// 在发出一个包含上述信息的请求后,必须调用此方法或手工设置相应属性以使下一次请求不会受到影响.
        /// </summary>
        public void Reset()
        {
            verb = HttpVerb.GET;
            files.Clear();
            postingData.Clear();
            responseHeaders = null;
            keepAlive = false;
            startPoint = 0;
            endPoint = 0;
        }

        private HttpWebRequest CreateRequest()
        {
            if (url.Contains("https://"))
            {
                ServicePointManager.CertificatePolicy = new AcceptAllCertificatePolicy();
            }
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            
            //req.ReadWriteTimeout = 3000000;

            //req.Headers.Add("X-Real-IP", "123.6.98.238");
            //req.Headers.Add("REMOTE-HOST", "123.6.98.238");
            //req.Headers.Add("X-Forwarded-For", "123.6.98.238");

            req.AllowAutoRedirect = false;
            req.CookieContainer = new CookieContainer();
            req.Headers.Add("Accept-Language", defaultLanguage);
            req.Accept = accept;
            req.UserAgent = userAgent;
            req.KeepAlive = keepAlive;
            req.ServicePoint.Expect100Continue = false;

            if (context.Cookies != null)
                req.CookieContainer.Add(context.Cookies);
            if (!string.IsNullOrEmpty(context.Referer))
                req.Referer = context.Referer;

            if (verb == HttpVerb.HEAD)
            {
                req.Method = "HEAD";
                return req;
            }

            if (postingData.Count > 0 || files.Count > 0)
                verb = HttpVerb.POST;

            if (verb == HttpVerb.POST)
            {
                req.Method = "POST";

                MemoryStream memoryStream = new MemoryStream();
                StreamWriter writer = new StreamWriter(memoryStream);

                if (files.Count > 0)
                {
                    string newLine = "\r\n";
                    string boundary = Guid.NewGuid().ToString().Replace("-", "");
                    req.ContentType = "multipart/form-data; boundary=" + boundary;

                    foreach (var item in postingData)
                    {
                        writer.Write("--" + boundary + newLine);
                        writer.Write("Content-Disposition: form-data; name=\"{0}\"{1}{1}", item.name, newLine);
                        writer.Write(item.value + newLine);
                    }
                    int i = 0;
                    foreach (HttpUploadingFile file in files)
                    {
                        i++;
                        writer.Write("--" + boundary + newLine);
                        writer.Write(
                            "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}",
                            file.FieldName,
                            file.ShortName,
                            newLine
                            );
                        //writer.Write("Content-Type: " + file.ContentType + "" + newLine + newLine);
                        writer.Write("Content-Type: application/octet-stream" + newLine + newLine);
                        writer.Flush();
                        memoryStream.Write(file.Data, 0, file.Data.Length);
                        writer.Write(newLine);

                        if (i == files.Count)
                        {
                            writer.Write("--" + boundary + "--" + newLine);
                        }
                        else
                        {
                            writer.Write("--" + boundary + newLine);
                        }
                    }
                }
                else
                {
                    req.ContentType = "application/x-www-form-urlencoded";
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in postingData)
                    {
                        sb.AppendFormat("{0}={1}&", HttpUtility.UrlEncode(item.name, defaultEncoding), HttpUtility.UrlEncode(item.value, defaultEncoding));
                    }
                    if (sb.Length > 0)
                        sb.Length--;
                    writer.Write(sb.ToString());
                }

                writer.Flush();

                using (Stream stream = req.GetRequestStream())
                {
                    memoryStream.WriteTo(stream);
                }
            }

            if (startPoint != 0 && endPoint != 0)
                req.AddRange(startPoint, endPoint);
            else if (startPoint != 0 && endPoint == 0)
                req.AddRange(startPoint);

            return req;
        }

        /// <summary>
        /// 发出一次新的请求,并返回获得的回应
        /// 调用此方法永远不会触发StatusUpdate事件.
        /// </summary>
        /// <returns>相应的HttpWebResponse</returns>
        public HttpWebResponse GetResponse()
        {
            HttpWebRequest req = CreateRequest();
           // req.ReadWriteTimeout = 1000 ;
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            responseHeaders = res.Headers;

            string cookiestr = responseHeaders.Get("Set-Cookie");
            if (keepContext)
            {
                if (context.Cookies == null)
                {
                    context.Cookies = new CookieCollection();
                }
                if (!string.IsNullOrEmpty(cookiestr))
                {
                    SetCookie(cookiestr);
                }
                //context.Cookies = res.Cookies;
                context.Referer = url;
            }
            return res;
        }

        private void SetCookie(string cookiestr)
        {
            string[] cookies = cookiestr.Replace(", ", "!").Split(',');

            foreach (string cookie in cookies)
            {
                if (cookie.StartsWith(" "))
                {
                    continue;
                }
                string[] cookieitems = cookie.Replace("!", ", ").Split(';');

                string cookie1 = cookieitems[0];
                string name = cookie1.Split('=')[0];
                string value = cookie1.Substring(cookie1.IndexOf('=') + 1).Replace("\"", "");
                string domain = "";
                string path = "";

                string pathitem = (from p in cookieitems where p.ToLower().Contains("path") select p).SingleOrDefault();
                if (!string.IsNullOrEmpty(pathitem))
                {
                    path = pathitem.Split('=')[1];
                }
                string domainitem = (from d in cookieitems where d.ToLower().Contains("domain") && d.ToLower().StartsWith(" d") select d).SingleOrDefault();
                if (!string.IsNullOrEmpty(domainitem))
                {
                    domain = domainitem.Split('=')[1];
                }
                if (string.IsNullOrEmpty(domain))
                {
                    domain = defalutdomain;
                }
                //if (cookieitems.Length > 3)
                //{
                //    path = cookieitems[2].Split('=')[1];
                //    domain = cookieitems[3].Split('=')[1];
                //}
                //else if (cookieitems.Length > 2)
                //{
                //    path = cookieitems[1].Split('=')[1];
                //    domain = cookieitems[2].Split('=')[1];
                //}
                //else
                //{
                //    path = cookieitems[1].Split('=')[1];
                //}
                context.Cookies.Add(new Cookie(name, value, path, domain));
            }
        }

        /// <summary>
        /// 发出一次新的请求,并返回回应内容的流

        /// 调用此方法永远不会触发StatusUpdate事件.
        /// </summary>
        /// <returns>包含回应主体内容的流</returns>
        public Stream GetStream()
        {
            return GetResponse().GetResponseStream();
        }

        /// <summary>
        /// 发出一次新的请求,并以字节数组形式返回回应的内容

        /// 调用此方法会触发StatusUpdate事件
        /// </summary>
        /// <returns>包含回应主体内容的字节数组</returns>
        public byte[] GetBytes()
        {
            HttpWebResponse res = GetResponse();
            int length = (int)res.ContentLength;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[0x100];
                using (Stream rs = res.GetResponseStream())
                {
                    rs.ReadTimeout = 5000;
                    for (int i = rs.Read(buffer, 0, buffer.Length); i > 0; i = rs.Read(buffer, 0, buffer.Length))
                    {
                        memoryStream.Write(buffer, 0, i);
                        //OnStatusUpdate(new StatusUpdateEventArgs((int)memoryStream.Length, length));
                    }
                    rs.Close();
                }

                return memoryStream.ToArray();
            }
            
        }

     

        /// <summary>
        /// 发出一次新的请求,对回应的主体内容以指定的编码进行解码
        /// 调用此方法会触发StatusUpdate事件
        /// </summary>
        /// <param name="encoding">指定的编码</param>
        /// <returns>解码后的字符串</returns>
        public string GetString(Encoding encoding)
        {
            byte[] data = GetBytes();
            if (data == null)
            {
                return "";
            }
            return encoding.GetString(data);
        }


        /// <summary>
        /// 发出一次新的请求,以Http头,或Html Meta标签,或DefaultEncoding指示的编码信息对回应主体解码
        /// 调用此方法会触发StatusUpdate事件
        /// </summary>
        /// <returns>解码后的字符串</returns>
        public string GetString()
        {
            try
            {
                var stream = GetStream();

                string encodingName = GetEncodingFromHeaders();

                if (encodingName != null && encodingName.ToLower().Contains("gzip"))
                {
                    stream = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Decompress);
                    string source = "";
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        source = reader.ReadToEnd();
                    }
                    return source;
                }

                byte[] data = ConvertStreamToByteBuffer(stream);

                if (encodingName == null)
                {
                    encodingName = GetEncodingFromBody(data);
                }

                Encoding encoding;
                if (encodingName == null)
                {
                    encoding = defaultEncoding;
                }
                else
                {
                    try
                    {
                        encoding = Encoding.GetEncoding(encodingName);
                    }
                    catch (ArgumentException)
                    {
                        encoding = defaultEncoding;
                    }
                }
                return encoding.GetString(data);

            }
            catch
            {
                return "";
            }
        }

        #region ConvertStreamToByteBuffer：把给定的文件流转换为二进制字节数组
        /// <summary>
        /// ConvertStreamToByteBuffer：把给定的文件流转换为二进制字节数组。

        /// </summary>
        /// <param name="theStream"></param>
        /// <returns></returns>
        public static byte[] ConvertStreamToByteBuffer(System.IO.Stream theStream)
        {
            int b1;
            System.IO.MemoryStream tempStream = new System.IO.MemoryStream();
            while ((b1 = theStream.ReadByte()) != -1)
            {
                tempStream.WriteByte(((byte)b1));
            }
            return tempStream.ToArray();
        }
        #endregion


        private string GetEncodingFromHeaders()
        {
            string encoding = null;
            if (responseHeaders.AllKeys.Contains("Content-Encoding"))
            {
                return responseHeaders["Content-Encoding"];
            }
            string contentType = responseHeaders["Content-Type"];
            if (contentType != null)
            {
                int i = contentType.IndexOf("charset=");
                if (i != -1)
                {
                    encoding = contentType.Substring(i + 8);
                }
            }
            return encoding;
        }

        private string GetEncodingFromBody(byte[] data)
        {
            string encodingName = null;
            string dataAsAscii = Encoding.ASCII.GetString(data);
            if (dataAsAscii != null)
            {
                int i = dataAsAscii.IndexOf("charset=");
                if (i != -1)
                {
                    int j = dataAsAscii.IndexOf("\"", i);
                    if (j != -1)
                    {
                        int k = i + 8;
                        encodingName = dataAsAscii.Substring(k, (j - k) + 1);
                        char[] chArray = new char[2] { '>', '"' };
                        encodingName = encodingName.TrimEnd(chArray);
                    }
                }
            }
            return encodingName;
        }

        /// <summary>
        /// 发出一次新的Head请求,获取资源的长度

        /// 此请求会忽略PostingData, Files, StartPoint, EndPoint, Verb
        /// </summary>
        /// <returns>返回的资源长度</returns>
        public int HeadContentLength()
        {
            Reset();
            HttpVerb lastVerb = verb;
            verb = HttpVerb.HEAD;
            using (HttpWebResponse res = GetResponse())
            {
                verb = lastVerb;
                return (int)res.ContentLength;
            }
        }

        /// <summary>
        /// 发出一次新的请求,把回应的主体内容保存到文件

        /// 调用此方法会触发StatusUpdate事件
        /// 如果指定的文件存在,它会被覆盖

        /// </summary>
        /// <param name="fileName">要保存的文件路径</param>
        public void SaveAsFile(string fileName)
        {
            SaveAsFile(fileName, FileExistsAction.Overwrite);
        }

        /// <summary>
        /// 发出一次新的请求,把回应的主体内容保存到文件

        /// 调用此方法会触发StatusUpdate事件
        /// </summary>
        /// <param name="fileName">要保存的文件路径</param>
        /// <param name="existsAction">指定的文件存在时的选项</param>
        /// <returns>是否向目标文件写入了数据</returns>
        public bool SaveAsFile(string fileName, FileExistsAction existsAction)
        {
            byte[] data = GetBytes();
            switch (existsAction)
            {
                case FileExistsAction.Overwrite:
                    using (BinaryWriter writer = new BinaryWriter(new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write)))
                        writer.Write(data);
                    return true;

                case FileExistsAction.Append:
                    using (BinaryWriter writer = new BinaryWriter(new FileStream(fileName, FileMode.Append, FileAccess.Write)))
                        writer.Write(data);
                    return true;

                default:
                    if (!File.Exists(fileName))
                    {
                        using (
                            BinaryWriter writer =
                                new BinaryWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write)))
                            writer.Write(data);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }
        }
        #endregion

        #region wangjun

        /// <summary>
        /// 发送post请求
        /// </summary>
        /// <param name="Url">提交的地址</param>
        /// <param name="postDataStr">表单对象</param>
        /// <returns>服务器返回值</returns>
        public static string SendDataByPost(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.CookieContainer = new CookieContainer();
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postDataStr.Length;
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.Default);
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.Default);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();

            return retString;
        }



        /// <summary> 下载文件</summary>        
        /// <param name="URL">下载文件地址</param>        
        /// <param name="Filename">下载后的存放地址</param>              
        public static void DownloadFile(string URL, string filename)
        {
            try
            {
                System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
                System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;

                System.IO.Stream st = myrp.GetResponseStream();
                System.IO.Stream so = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                long totalDownloadedByte = 0;
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, (int)by.Length);
                while (osize > 0)
                {
                    totalDownloadedByte = osize + totalDownloadedByte;

                    so.Write(by, 0, osize);

                    osize = st.Read(by, 0, (int)by.Length);
                }
                so.Close();
                st.Close();
            }
            catch
            {

            }
        }

        /// <summary> 下载文件</summary>        
        /// <param name="URL">下载文件地址</param>        
        /// <param name="Filename">下载后的存放地址</param>        
        /// <param name="Prog">用于显示的进度条</param>        
        public static void DownloadFile(string URL, string filename, System.Windows.Forms.ProgressBar prog)
        {
            try
            {
                System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
                System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
                if (prog != null)
                {
                    prog.Maximum = (int)totalBytes;
                }
                System.IO.Stream st = myrp.GetResponseStream();
                System.IO.Stream so = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                long totalDownloadedByte = 0;
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, (int)by.Length);
                while (osize > 0)
                {
                    totalDownloadedByte = osize + totalDownloadedByte;
                    System.Windows.Forms.Application.DoEvents();
                    so.Write(by, 0, osize);
                    if (prog != null)
                    {
                        prog.Value = (int)totalDownloadedByte;
                    }
                    osize = st.Read(by, 0, (int)by.Length);
                }
                so.Close();
                st.Close();
            }
            catch
            {

            }
        }
        /// <summary>
        /// 发送post请求上传文件
        /// </summary>
        /// <param name="address">url地址</param>
        /// <param name="fileNamePath">本机文件路径</param>
        /// <param name="saveName">上传保存的文件名</param>
        /// <returns>1请求成功0失败</returns>
        public static int Upload_Request(string address, string fileNamePath, string saveName)
        {
            int returnValue = 0;
            // 要上传的文件          
            FileStream fs = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read);
            BinaryReader r = new BinaryReader(fs);
            //时间戳           
            string strBoundary = "----------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + strBoundary + "\r\n");
            //请求头部信息             
            StringBuilder sb = new StringBuilder();
            sb.Append("--");
            sb.Append(strBoundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"");
            sb.Append("file");
            sb.Append("\"; filename=\"");
            sb.Append(saveName);
            sb.Append("\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: ");
            sb.Append("application/octet-stream");
            sb.Append("\r\n");
            sb.Append("\r\n");
            string strPostHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(strPostHeader);
            // 根据uri创建HttpWebRequest对象        
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(address));
            httpReq.Method = "POST";
            //对发送的数据不使用缓存           
            httpReq.AllowWriteStreamBuffering = false;
            //设置获得响应的超时时间（300秒）         
            httpReq.Timeout = 300000;
            httpReq.ContentType = "multipart/form-data; boundary=" + strBoundary;
            long length = fs.Length + postHeaderBytes.Length + boundaryBytes.Length;
            long fileLength = fs.Length;
            httpReq.ContentLength = length;
            try
            {

                //每次上传4k               
                int bufferLength = 4096;
                byte[] buffer = new byte[bufferLength];
                //已上传的字节数 
                long offset = 0;
                //开始上传时间 
                DateTime startTime = DateTime.Now;
                int size = r.Read(buffer, 0, bufferLength);
                Stream postStream = httpReq.GetRequestStream();
                //发送请求头部消息               
                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                while (size > 0)
                {
                    postStream.Write(buffer, 0, size);
                    offset += size;

                    TimeSpan span = DateTime.Now - startTime;
                    double second = span.TotalSeconds;

                    size = r.Read(buffer, 0, bufferLength);
                }
                //添加尾部的时间戳            
                postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                postStream.Close();
                //获取服务器端的响应 
                WebResponse webRespon = httpReq.GetResponse();
                Stream s = webRespon.GetResponseStream();
                StreamReader sr = new StreamReader(s);
                //读取服务器端返回的消息 
                String sReturnString = sr.ReadLine();
                s.Close();
                sr.Close();
                if (sReturnString == "Success")
                {
                    returnValue = 1;
                }
                else if (sReturnString == "Error")
                {
                    returnValue = 0;
                }
            }
            catch
            {
                returnValue = 0;
            }
            finally
            {
                fs.Close();
                r.Close();
            }
            return returnValue;
        }

        // <summary>
        /// 将本地文件上传到指定的服务器(HttpWebRequest方法)
        /// </summary>
        /// <param name="address">文件上传到的服务器</param>
        /// <param name="fileNamePath">要上传的本地文件（全路径）</param>
        /// <param name="saveName">文件上传后的名称</param>
        /// <param name="progressBar">上传进度条</param>
        /// <returns>成功返回1，失败返回0</returns>
        public static int Upload_Request(string address, string fileNamePath, string saveName, ProgressBar progressBar)
        {
            int returnValue = 0;

            // 要上传的文件
            FileStream fs = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read);
            BinaryReader r = new BinaryReader(fs);

            //时间戳

            string strBoundary = "----------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + strBoundary + "\r\n");

            //请求头部信息
            StringBuilder sb = new StringBuilder();
            sb.Append("--");
            sb.Append(strBoundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"");
            sb.Append("file");
            sb.Append("\"; filename=\"");
            sb.Append(saveName);
            sb.Append("\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: ");
            sb.Append("application/octet-stream");
            sb.Append("\r\n");
            sb.Append("\r\n");
            string strPostHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(strPostHeader);

            // 根据uri创建HttpWebRequest对象
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(address));
            httpReq.Method = "POST";

            //对发送的数据不使用缓存

            httpReq.AllowWriteStreamBuffering = false;

            //设置获得响应的超时时间（300秒）
            httpReq.Timeout = 300000;
            httpReq.ContentType = "multipart/form-data; boundary=" + strBoundary;
            long length = fs.Length + postHeaderBytes.Length + boundaryBytes.Length;
            long fileLength = fs.Length;
            httpReq.ContentLength = length;
            try
            {
                progressBar.Maximum = int.MaxValue;
                progressBar.Minimum = 0;
                progressBar.Value = 0;

                //每次上传4k
                int bufferLength = 4096;
                byte[] buffer = new byte[bufferLength];

                //已上传的字节数

                long offset = 0;

                //开始上传时间

                DateTime startTime = DateTime.Now;
                int size = r.Read(buffer, 0, bufferLength);
                Stream postStream = httpReq.GetRequestStream();

                //发送请求头部消息

                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                while (size > 0)
                {
                    postStream.Write(buffer, 0, size);
                    offset += size;
                    progressBar.Value = (int)(offset * (int.MaxValue / length));
                    TimeSpan span = DateTime.Now - startTime;
                    double second = span.TotalSeconds;
                    //lblTime.Text = "已用时：" + second.ToString("F2") + "秒";
                    //if (second > 0.001)
                    //{
                    //    lblSpeed.Text = " 平均速度：" + (offset / 1024 / second).ToString("0.00") + "KB/秒";
                    //}
                    //else
                    //{
                    //    lblSpeed.Text = " 正在连接…";
                    //}
                    //lblState.Text = "已上传：" + (offset * 100.0 / length).ToString("F2") + "%";
                    //lblSize.Text = (offset / 1048576.0).ToString("F2") + "M/" + (fileLength / 1048576.0).ToString("F2") + "M";
                    //Application.DoEvents();
                    size = r.Read(buffer, 0, bufferLength);
                }
                //添加尾部的时间戳
                postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                postStream.Close();

                //获取服务器端的响应

                WebResponse webRespon = httpReq.GetResponse();
                Stream s = webRespon.GetResponseStream();
                StreamReader sr = new StreamReader(s);

                //读取服务器端返回的消息

                String sReturnString = sr.ReadLine();
                s.Close();
                sr.Close();
                if (sReturnString == "Success")
                {
                    returnValue = 1;
                }
                else if (sReturnString == "Error")
                {
                    returnValue = 0;
                }

            }
            catch
            {
                returnValue = 0;
            }
            finally
            {
                fs.Close();
                r.Close();
            }

            return returnValue;
        }
        #endregion
    }

    //ServicePointManager.CertificatePolicy = new AcceptAllCertificatePolicy();
    internal class AcceptAllCertificatePolicy : ICertificatePolicy
    {
        public AcceptAllCertificatePolicy()
        {
        }

        public bool CheckValidationResult(ServicePoint sPoint,

           X509Certificate cert, WebRequest wRequest, int certProb)
        {

            // Always accept 

            return true;

        }

    }


    public class HttpClientContext
    {
        private CookieCollection cookies;
        private string referer;

        public CookieCollection Cookies
        {
            get { return cookies; }
            set { cookies = value; }
        }

        public string Referer
        {
            get { return referer; }
            set { referer = value; }
        }
    }

    public class PostData
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class PostDataList : List<PostData>
    {
        public void Add(string name, string value)
        {
            base.Add(new PostData { name = name, value = value });
        }
    }

    public enum HttpVerb
    {
        GET,
        POST,
        HEAD,
    }

    public enum FileExistsAction
    {
        Overwrite,
        Append,
        Cancel,
    }

    public class HttpUploadingFile
    {
        private string fileName;
        private string fieldName;
        private string contentType;
        private byte[] data;
        private string shortName;

        public string ShortName
        {
            get { return shortName; }
            set { shortName = value; }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public string FieldName
        {
            get { return fieldName; }
            set { fieldName = value; }
        }

        public string ContentType
        {
            get { return contentType; }
            set { contentType = value; }
        }

        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }

        public HttpUploadingFile(string fileName, string fieldName)
        {
            this.fileName = fileName;
            this.fieldName = fieldName;

            using (FileStream stream = new FileStream(fileName, FileMode.Open))
            {
                byte[] inBytes = new byte[stream.Length];
                stream.Read(inBytes, 0, inBytes.Length);
                data = inBytes;
            }
            shortName = System.IO.Path.GetFileName(fileName);
        }

        public HttpUploadingFile(byte[] data, string fileName, string fieldName, string contentType)
        {
            this.data = data;
            this.fileName = fileName;
            this.fieldName = fieldName;
            this.contentType = contentType;
        }
    }

    public class StatusUpdateEventArgs : EventArgs
    {
        private readonly int bytesGot;
        private readonly int bytesTotal;

        public StatusUpdateEventArgs(int got, int total)
        {
            bytesGot = got;
            bytesTotal = total;
        }

        /// <summary>
        /// 已经下载的字节数
        /// </summary>
        public int BytesGot
        {
            get { return bytesGot; }
        }

        /// <summary>
        /// 资源的总字节数
        /// </summary>
        public int BytesTotal
        {
            get { return bytesTotal; }
        }
    }




}
