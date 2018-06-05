using System;
using System.Web;
using System.Net;
using System.IO;
using System.Text;

namespace CS.Http
{
	/// <summary>
	/// Request������
	/// </summary>
	public class RequestEx
	{
		/// <summary>
		/// �жϵ�ǰҳ���Ƿ���յ���Post����
		/// </summary>
		/// <returns>�Ƿ���յ���Post����</returns>
		public static bool IsPost()
		{
			return HttpContext.Current.Request.HttpMethod.Equals("POST");
		}
		/// <summary>
		/// �жϵ�ǰҳ���Ƿ���յ���Get����
		/// </summary>
		/// <returns>�Ƿ���յ���Get����</returns>
		public static bool IsGet()
		{
			return HttpContext.Current.Request.HttpMethod.Equals("GET");
		}

        /// <summary>
        /// ��ȡ������Ϣ��form��get�ύ��
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
        /// ��ȡ����������Ϣ��form��get�ύ��
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
		/// ����ָ���ķ�����������Ϣ
		/// </summary>
		/// <param name="strName">������������</param>
		/// <returns>������������Ϣ</returns>
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
		/// ������һ��ҳ��ĵ�ַ
		/// </summary>
		/// <returns>��һ��ҳ��ĵ�ַ</returns>
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
		/// �õ���ǰ��������ͷ
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
		/// �õ�����ͷ
		/// </summary>
		/// <returns></returns>
		public static string GetHost()
		{
			return HttpContext.Current.Request.Url.Host;
		}


		/// <summary>
		/// ��ȡ��ǰ�����ԭʼ URL(URL ������Ϣ֮��Ĳ���,������ѯ�ַ���(�������))
		/// </summary>
		/// <returns>ԭʼ URL</returns>
		public static string GetRawUrl()
		{
			return HttpContext.Current.Request.RawUrl;
		}

		/// <summary>
		/// �жϵ�ǰ�����Ƿ�������������
		/// </summary>
		/// <returns>��ǰ�����Ƿ�������������</returns>
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
		/// �ж��Ƿ�����������������
		/// </summary>
		/// <returns>�Ƿ�����������������</returns>
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
		/// ��õ�ǰ����Url��ַ
		/// </summary>
		/// <returns>��ǰ����Url��ַ</returns>
		public static string GetUrl()
		{
			return HttpContext.Current.Request.Url.ToString();
		}
		

		/// <summary>
		/// ���ָ��Url������ֵ
		/// </summary>
		/// <param name="strName">Url����</param>
		/// <returns>Url������ֵ</returns>
		public static string GetQueryString(string strName)
		{
			if (HttpContext.Current.Request.QueryString[strName] == null)
			{
				return "";
			}
			return HttpContext.Current.Request.QueryString[strName];
		}

        /// <summary>
        /// ��ȡָ����URL��������ֵ
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
		/// ��õ�ǰҳ�������
		/// </summary>
		/// <returns>��ǰҳ�������</returns>
		public static string GetPageName()
		{
			string [] urlArr = HttpContext.Current.Request.Url.AbsolutePath.Split('/');
			return urlArr[urlArr.Length - 1].ToLower();
		}

		/// <summary>
		/// ���ر���Url�������ܸ���
		/// </summary>
		/// <returns></returns>
		public static int GetParamCount()
		{
			return HttpContext.Current.Request.Form.Count + HttpContext.Current.Request.QueryString.Count;
		}


		/// <summary>
		/// ���ָ����������ֵ
		/// </summary>
		/// <param name="strName">������</param>
		/// <returns>��������ֵ</returns>
		public static string GetFormString(string strName)
		{
			return HttpContext.Current.Request.Form[strName];
		}

        /// <summary>
        /// ���ָ��������������ֵ���Ϊ���ⷵ��-1
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
		/// ���Url���������ֵ, ���ж�Url�����Ƿ�Ϊ���ַ���, ��ΪTrue�򷵻ر�������ֵ
		/// </summary>
		/// <param name="strName">����</param>
		/// <returns>Url���������ֵ</returns>
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
		/// �����û��ϴ����ļ�
		/// </summary>
		/// <param name="path">����·��</param>
		public static void SaveRequestFile(string path)
		{
			if (HttpContext.Current.Request.Files.Count > 0)
			{
				HttpContext.Current.Request.Files[0].SaveAs(path);
			}
		}

        /// <summary>
        /// ��ȡ�û��ͻ��˵�Ip
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
        /// ��ȡ����IP��ַ��Ϣ
        /// ���ͬʱ����������ͱ������磬�Ȼ�ȡǰ�����������
        /// </summary>
        public static string GetLocalIP()
        {
            ///��ȡ���ص�IP��ַ
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


        #region �������󷽷�
        
       

        /// <summary>
        /// Put������Ҫ���ڴ�����Դ
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
                return "�쳣��" + ex.ToString();
            }
        }

        /// <summary>
        /// delete������Ҫ����ɾ������
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
                return "�쳣��" + ex.ToString();
            }
        }

        #endregion
    
    }
}
