using System;
using System.Web;
using System.Net;
using System.IO;
using System.Text;

namespace CS.Http
{
	/// <summary>
	/// Request操作类
	/// </summary>
	public class RequestEx
	{
		/// <summary>
		/// 判断当前页面是否接收到了Post请求
		/// </summary>
		/// <returns>是否接收到了Post请求</returns>
		public static bool IsPost()
		{
			return HttpContext.Current.Request.HttpMethod.Equals("POST");
		}
		/// <summary>
		/// 判断当前页面是否接收到了Get请求
		/// </summary>
		/// <returns>是否接收到了Get请求</returns>
		public static bool IsGet()
		{
			return HttpContext.Current.Request.HttpMethod.Equals("GET");
		}

        /// <summary>
        /// 获取参数信息（form或get提交）
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static string GetParamString(string Param)
        {
            if (IsPost())
            {
                return GetFormString(Param);
            }
            else if (IsGet())
            {
                return GetQueryString(Param);
            }
            return "";
        }

        /// <summary>
        /// 获取参数数字信息（form或get提交）
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static string GetParamNumber(string Param)
        {
            if (IsPost())
            {
                return GetFormNumber(Param);
            }
            else if (IsGet())
            {
                return GetQueryNumber(Param);
            }
            return "-1";
        }
		/// <summary>
		/// 返回指定的服务器变量信息
		/// </summary>
		/// <param name="strName">服务器变量名</param>
		/// <returns>服务器变量信息</returns>
		public static string GetServerString(string strName)
		{
			//
			if (HttpContext.Current.Request.ServerVariables[strName] == null)
			{
				return "";
			}
			return HttpContext.Current.Request.ServerVariables[strName].ToString();
		}

		/// <summary>
		/// 返回上一个页面的地址
		/// </summary>
		/// <returns>上一个页面的地址</returns>
		public static string GetUrlReferrer()
		{
			string retVal = null;
    
			try
			{
				retVal = HttpContext.Current.Request.UrlReferrer.ToString();
			}
			catch{}
			
			if (retVal == null)
				return "";
    
			return retVal;

		}
		
		/// <summary>
		/// 得到当前完整主机头
		/// </summary>
		/// <returns></returns>
		public static string GetCurrentFullHost()
		{
			HttpRequest request = System.Web.HttpContext.Current.Request;
			if (!request.Url.IsDefaultPort)
			{
				return string.Format("{0}:{1}", request.Url.Host, request.Url.Port.ToString());
			}
			return request.Url.Host;
		}

		/// <summary>
		/// 得到主机头
		/// </summary>
		/// <returns></returns>
		public static string GetHost()
		{
			return HttpContext.Current.Request.Url.Host;
		}


		/// <summary>
		/// 获取当前请求的原始 URL(URL 中域信息之后的部分,包括查询字符串(如果存在))
		/// </summary>
		/// <returns>原始 URL</returns>
		public static string GetRawUrl()
		{
			return HttpContext.Current.Request.RawUrl;
		}

		/// <summary>
		/// 判断当前访问是否来自浏览器软件
		/// </summary>
		/// <returns>当前访问是否来自浏览器软件</returns>
		public static bool IsBrowserGet()
		{
			string[] BrowserName = {"ie", "opera", "netscape", "mozilla", "konqueror", "firefox"};
			string curBrowser = HttpContext.Current.Request.Browser.Type.ToLower();
			for (int i = 0; i < BrowserName.Length; i++)
			{
				if (curBrowser.IndexOf(BrowserName[i]) >= 0)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 判断是否来自搜索引擎链接
		/// </summary>
		/// <returns>是否来自搜索引擎链接</returns>
		public static bool IsSearchEnginesGet()
		{
            if (HttpContext.Current.Request.UrlReferrer == null)
            {
                return false;
            }
		    string[] SearchEngine = {"google", "yahoo", "msn", "baidu", "sogou", "sohu", "sina", "163", "lycos", "tom", "yisou", "iask", "soso", "gougou", "zhongsou"};
			string tmpReferrer = HttpContext.Current.Request.UrlReferrer.ToString().ToLower();
			for (int i = 0; i < SearchEngine.Length; i++)
			{
				if (tmpReferrer.IndexOf(SearchEngine[i]) >= 0)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 获得当前完整Url地址
		/// </summary>
		/// <returns>当前完整Url地址</returns>
		public static string GetUrl()
		{
			return HttpContext.Current.Request.Url.ToString();
		}
		

		/// <summary>
		/// 获得指定Url参数的值
		/// </summary>
		/// <param name="strName">Url参数</param>
		/// <returns>Url参数的值</returns>
		public static string GetQueryString(string strName)
		{
			if (HttpContext.Current.Request.QueryString[strName] == null)
			{
				return "";
			}
			return HttpContext.Current.Request.QueryString[strName];
		}

        /// <summary>
        /// 获取指定的URL参数数字值
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
        public static string GetQueryNumber(string strName)
        {
            if (string.IsNullOrEmpty(HttpContext.Current.Request.QueryString[strName]))
            {
                return "-1";
            }
            return HttpContext.Current.Request.QueryString[strName];
        }

		/// <summary>
		/// 获得当前页面的名称
		/// </summary>
		/// <returns>当前页面的名称</returns>
		public static string GetPageName()
		{
			string [] urlArr = HttpContext.Current.Request.Url.AbsolutePath.Split('/');
			return urlArr[urlArr.Length - 1].ToLower();
		}

		/// <summary>
		/// 返回表单或Url参数的总个数
		/// </summary>
		/// <returns></returns>
		public static int GetParamCount()
		{
			return HttpContext.Current.Request.Form.Count + HttpContext.Current.Request.QueryString.Count;
		}


		/// <summary>
		/// 获得指定表单参数的值
		/// </summary>
		/// <param name="strName">表单参数</param>
		/// <returns>表单参数的值</returns>
		public static string GetFormString(string strName)
		{
			return HttpContext.Current.Request.Form[strName];
		}

        /// <summary>
        /// 获得指定表单参数的数字值如果为空这返回-1
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
        public static string GetFormNumber(string strName)
        {
            if (string.IsNullOrEmpty(HttpContext.Current.Request.Form[strName]))
            {
                return "-1";
            }
            return HttpContext.Current.Request.Form[strName];
        }

		/// <summary>
		/// 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
		/// </summary>
		/// <param name="strName">参数</param>
		/// <returns>Url或表单参数的值</returns>
		public static string GetString(string strName)
		{
			if ("".Equals(GetQueryString(strName)))
			{
				return GetFormString(strName);
			}
			else
			{
				return GetQueryString(strName);
			}
		}


	

		/// <summary>
		/// 保存用户上传的文件
		/// </summary>
		/// <param name="path">保存路径</param>
		public static void SaveRequestFile(string path)
		{
			if (HttpContext.Current.Request.Files.Count > 0)
			{
				HttpContext.Current.Request.Files[0].SaveAs(path);
			}
		}

        /// <summary>
        /// 获取用户客户端的Ip
        /// </summary>
        /// <returns></returns>
        public static string GetUserIP()
        {
            string userIP = "";
            try
            {
                userIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (userIP == null || userIP == "")
                {
                    userIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return userIP;
        }

        /// <summary>
        /// 获取本地IP地址信息
        /// 如果同时有无线网络和本地网络，先获取前面的无线网络
        /// </summary>
        public static string GetLocalIP()
        {
            ///获取本地的IP地址
            string AddressIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }
            return AddressIP;
        }


        #region 扩张请求方法
        
       

        /// <summary>
        /// Put请求，主要用于创建资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Put(string url, string data)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "PUT";
                request.ContentType = "text/plain";
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.WriteLine(data);
                }

                WebResponse response = request.GetResponse();
                StringBuilder sb = new StringBuilder();

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    while (reader.Peek() != -1)
                    {
                        sb.Append(reader.ReadLine());
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return "异常：" + ex.ToString();
            }
        }

        /// <summary>
        /// delete请求，主要用于删除请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Delete(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "DELETE";
                request.ContentType = "text/plain";

                WebResponse response = request.GetResponse();
                StringBuilder sb = new StringBuilder();

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    while (reader.Peek() != -1)
                    {
                        sb.Append(reader.ReadLine());
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return "异常：" + ex.ToString();
            }
        }

        #endregion
    
    }
}
