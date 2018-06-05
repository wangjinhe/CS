using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using CS.Http;
using CS.ProcessEx;

namespace CS.Img
{
   
    /// <summary>
    /// 验证码处理类
    /// </summary>
    public class CheckCodeBase
    {
        /// <summary>
        /// 验证码的地址
        /// </summary>
        public string ImgUrl { get; set; }
        /// <summary>
        /// 图片保存路径
        /// </summary>
        protected string ImgSavePath { get; set; }

        /// <summary>
        /// 处理后的图片保存路径
        /// 如果不处理图片，就是原来图片的路径
        /// </summary>
        protected string ImgHandleSavePath { get; set; }

        /// <summary>
        /// 验证码保存路径
        /// </summary>
        protected string CodeSavePath { get; set; }
        private int m_MaxTryCount = 10; // 默认为10
        /// <summary>
        /// 如果验证码不合格，最长尝试下载验证码的次数
        /// </summary>
        public int MaxTryCount
        {
            get
            {
                return m_MaxTryCount;
            }
            set
            {
                if (value < 1 || value > 10000)
                {
                    m_MaxTryCount = 10;
                }
                else
                {
                    m_MaxTryCount = value;
                }
            }
        }

        private string m_TesseractOCRPath = @"C:\Tesseract-OCR";
        /// <summary>
        /// Tesseract-OCR的路径，默认为C:\Tesseract-OCR
        /// </summary>
        public string TesseractOCRPath
        {
            get
            {
                return m_TesseractOCRPath;
            }
            set
            {
                if (IsTesseractOcrExist(value))
                {
                    m_TesseractOCRPath = value.Trim('/').Trim('\\');
                }
                else
                {
                    throw new Exception(value + "下载不存在tesseract.exe");
                }
            }
        }

        /// <summary>
        /// 图片Url的hash值，它和线程id组合，默认为图片和验证码的文件名
        /// </summary>
        private string m_urlHashCode = "";
        /// <summary>
        /// 图片的验证码的CookieContainer对象
        /// </summary>
        private CookieContainer m_cookieContainer;
        /// <summary>
        /// cookies的字符串
        /// </summary>
        private string m_cookies;

        private bool m_hasPreHandle; // 是否需要预处理

        private string m_font; // 使用的训练的字体

        public string Font
        {
            get
            {
                return m_font;
            }
            set
            {
                m_font = value;
            }
        }


        private string m_ocrArgs; 
        /// <summary>
        ///  tesseract使用的其他参数，比如 -psm 7
        /// </summary>
        public string OcrArgs
        {
            get
            {
                return m_ocrArgs;
            }
            set
            {
                m_ocrArgs = value;
            }
        }


        private int m_sleepTime = 0;
        public int SleepTime
        {
            get
            {
                return m_sleepTime;
            }
            set
            {
                m_sleepTime = value;
            }
        }



        /// <summary>
        /// 验证码
        /// </summary>
        private string m_code;
        /// <summary>
        /// 运行tesseract的命令，比如：  "C:\Tesseract-OCR\tesseract C:/rand.jpg  C:/rand"
        /// 注意：第1个参数是tesseract.exe的路径；第2个参数是图片路径；第3个参数是保存验证码的路径
        /// </summary>
        private string m_cmd;//    ;  // 后面的会自动添加txt来保存验证码
        protected string CMD
        {
            get
            {
                if (string.IsNullOrEmpty(m_cmd))
                {
                    if (m_hasPreHandle == false) // 不需要预处理
                    {
                        m_cmd = string.Format(@"{0}\tesseract {1}  C:/{2}", this.TesseractOCRPath, this.ImgSavePath, m_urlHashCode);
                    }
                    else
                    {
                        m_cmd = string.Format(@"{0}\tesseract {1}  C:/{2}", this.TesseractOCRPath, this.ImgHandleSavePath, m_urlHashCode);
                    }

                    // 加上字体
                    if (!string.IsNullOrEmpty(Font))
                    {
                        m_cmd += string.Format(" -l {0}", m_font);
                    }
                    if (!string.IsNullOrEmpty(OcrArgs))
                    {
                        m_cmd += " " + OcrArgs;
                    }
                }

                return m_cmd;
            }


        }

        public CheckCodeBase(string imgUrl)
            : this(imgUrl, false)
        {
        }

        /// <summary>
        /// 构造函数
        /// 默认图片和验证码文件名为url的哈希码，后缀分别是jpg和txt;
        /// 都保存路径在C:/下面
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <param name="hasPreHandle">是否需要预处理</param>
        public CheckCodeBase(string imgUrl, bool hasPreHandle)
        {
            if (string.IsNullOrEmpty(imgUrl))
            {
                throw new Exception("验证码地址不能为空");
            }
            this.ImgUrl = imgUrl;
            this.m_hasPreHandle = hasPreHandle;
            Init();
        }



        private void Init()
        {
            m_urlHashCode = this.ImgUrl.GetHashCode().ToString(); // 可能为负值
            this.ImgSavePath = "C:/" + m_urlHashCode + ".jpg";
            this.CodeSavePath = "C:/" + m_urlHashCode + ".txt";
            if (this.m_hasPreHandle == true)
            {
                this.ImgHandleSavePath = "C:/" + m_urlHashCode + "_handle.jpg";
            }

        }


        private bool IsTesseractOcrExist(string path)
        {
            try
            {
                // 判断tesseract.exe是否存在
                if (string.IsNullOrEmpty(path))
                {
                    Console.WriteLine("Tesseract-OCR的路径不能为空");
                    return false;
                }
                string exePath = path.Trim('/').Trim('\\') + "/tesseract.exe";
                if (!File.Exists(exePath))
                {
                    Console.WriteLine(exePath + "不存在");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool GetCode(out string code, CookieContainer cookieContainer)
        {

            code = "";

            for (int i = 0; i < MaxTryCount; i++)
            {
                try
                {
                    m_cookieContainer = cookieContainer; // 初始化
                    bool flag = SaveImg();
                    cookieContainer = m_cookieContainer;

                    if (flag == true) // 保存成功
                    {
                        if (m_hasPreHandle == true)
                        {
                            HandleImage(); // 处理验证码
                        }

                        if (m_sleepTime != 0)
                        {
                            Thread.Sleep(m_sleepTime);
                        }

                        string msg = "";
                        CmdHelper.RunCmd(this.CMD, out msg);
                        Console.WriteLine("cmd为:{0}\r\n", this.CMD);
                       
                        if (File.Exists(this.CodeSavePath))
                        {
                            code = File.ReadAllText(this.CodeSavePath).Trim();
                        }
                        else
                        {
                            continue;
                        }

                        code = HandleCode(code);
                        bool ok = IsCodeOk(code);
                        if (ok == true)
                        {
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return false;
        }



        public bool GetCode(out string code, out string cookies)
        {

            code = "";
            cookies = "";

            for (int i = 0; i < MaxTryCount; i++)
            {
                try
                {
                    m_cookieContainer = new CookieContainer(); // 初始化
                    bool flag = SaveImg();


                    if (flag == true) // 保存成功
                    {
                        if (m_hasPreHandle == true)
                        {
                            HandleImage(); // 处理验证码
                        }

                        cookies = CookieHelper.ToCookie(m_cookieContainer);

                        string msg = "";
                        CmdHelper.RunCmd(this.CMD, out msg);
                        Console.WriteLine("cmd为:{0} \r\n", this.CMD);
                        //Console.WriteLine("cmd为:{0} 返回如下:\r\n{1}", this.CMD, msg);
                        code = File.ReadAllText(this.CodeSavePath).Trim();

                        code = HandleCode(code);
                        bool ok = IsCodeOk(code);
                        if (ok == true)
                        {
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return false;
        }

        /// <summary>
        /// 处理验证码，比如去噪点、二值化等等
        /// 这里仅仅去掉边框
        /// </summary>
        public virtual void HandleImage()
        {
            try
            {

                using (Bitmap map = new Bitmap(this.ImgSavePath))
                {
                    using (Bitmap newMap = new Bitmap(map.Width, map.Height)) // 得到一个新图片处理
                    {
                        for (int i = 0; i < map.Width; i++)
                        {
                            for (int j = 0; j < map.Height; j++)
                            {
                                // 去掉边框
                                if (i == 0 || j == 0 || i == map.Width - 1 || j == map.Height - 1)
                                {
                                    newMap.SetPixel(i, j, Color.White);
                                }
                                else
                                {
                                    Color c = map.GetPixel(i, j);
                                    newMap.SetPixel(i, j, c);
                                }
                            }
                        }
                        // 处理图片，就把图片存在另外一个路径                       
                        newMap.Save(this.ImgHandleSavePath);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// 判断两个颜色是否相近
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static bool IsSimilarColor(Color A, Color B, int threshold)
        {
            try
            {
                // 计算方差
                double fx = Math.Sqrt(Math.Pow(A.R - B.R, 2) + Math.Pow(A.G - B.G, 2) + Math.Pow(A.B - B.B, 2));
                if (fx <= threshold)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        /// <summary>
        /// 处理验证码,处理之后只留下字母和数字
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public virtual string HandleCode(string code)
        {
            try
            {
                code = Regex.Replace(code, "[^0-9a-zA-Z]", "");
            }
            catch (Exception ex)
            {
            }
            return code;
        }

        /// <summary>
        /// 这里只做简单的判断，特殊的重载该函数实现。
        /// 一般的验证码至少4位
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public virtual bool IsCodeOk(string code)
        {
            if (!string.IsNullOrEmpty(code) && code.Length > 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private bool SaveImg()
        {

            HttpWebRequest req;
            HttpWebResponse res = null;
            try
            {
                System.Uri httpUrl = new System.Uri(this.ImgUrl);
                req = (HttpWebRequest)(WebRequest.Create(httpUrl));
                req.Timeout = 180000; //设置超时值10秒
                req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.132 Safari/537.36";
                req.Accept = "image/webp,*/*;q=0.8";

                req.CookieContainer = m_cookieContainer;
                req.KeepAlive = true;
                res = (HttpWebResponse)(req.GetResponse());
                //获取图片流  
                using (Bitmap img = new Bitmap(res.GetResponseStream()))
                {
                    img.Save(this.ImgSavePath);//随机名
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}下载图片异常:{1}",ImgUrl, ex.Message);
                return false;
            }
            finally
            {
                res.Close();
            }
        }

    }
}
