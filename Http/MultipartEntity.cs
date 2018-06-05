/*  针对multipart/form-data提交方式，模拟java中的MultipartEntity类写的。
 * 使用方法如下：

MultipartEntity post = new MultipartEntity();
post.AddItem("w", "17ckd");
post.AddItem("cno", "211097368892");
post.AddItem("cemskind", "百世物流");
post.AddItem("ccode", code);
postdata = post.GetPostData();
ContentType = post.ContentType;

 
 * 
 * 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CS.Http
{
    public class MultipartEntity
    {

        private Dictionary<string, string> dic = new Dictionary<string, string>();
        public string Boundary = "";
        public string PostDataBoundary = "";
        public string ContentType = "";

        public MultipartEntity(string boundary = "")
        {
            if (string.IsNullOrEmpty(boundary))
            {
                Boundary = "---------------------------282512662830282";
            }
            else
            {
                this.Boundary = boundary;
            }

            this.PostDataBoundary = "--" + Boundary;
            this.ContentType = "multipart/form-data; boundary=" + Boundary;
        }

        public void AddItem(string name, string value)
        {
            if (!string.IsNullOrEmpty(name) && !dic.ContainsKey(name))
            {
                dic.Add(name, value);
            }
        }

        public string GetPostData()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in this.dic)
            {
                sb.AppendFormat("{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", this.PostDataBoundary, kv.Key, kv.Value);
            }
            sb.AppendFormat("{0}--\r\n", this.PostDataBoundary);
            return sb.ToString();
        }


    }


}
