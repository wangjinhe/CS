using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace CS.Office
{
    public class CSVHelper
    {

        private static Char GetSeparator(string[] arrLines)
        {
            Char ch = '\t'; // 默认
            if (arrLines.Length < 1)
            {
                return ch;
            }
            else //if (arrLines.Length == 1) //只有一行
            {
                string line = arrLines[0];
                int count1 = Regex.Matches(line, ",").Count;
                int count2 = Regex.Matches(line, "\\t").Count;
                if (count1 > count2)
                {
                    ch = ',';
                }
                else
                {
                    ch = '\t';
                }
                return ch;
            }
        }


        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// 列以\t分割
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        public static DataTable ReadCSV(string filePath, bool hasHeader)
        {
            try
            {
                DataTable dt = new DataTable();
                string[] arrLines = File.ReadAllLines(filePath,Encoding.Default);

                Char ch = GetSeparator(arrLines);

                string[] row0 = arrLines[0].Split(ch);
                int columnCount = row0.Length;

                for (int i = 0; i < columnCount; i++)
                {
                    string col = "";
                    // 第1行是列名称
                    if (hasHeader == true)
                    {
                        col = row0[i];
                    }
                    else
                    {
                        col = i.ToString();
                    }
                    DataColumn dc = new DataColumn(col);
                    dt.Columns.Add(dc);
                }

                int index = 0;
                foreach (string line in arrLines)
                {
                    if (hasHeader == true && index++ == 0)
                    {
                        continue;
                    }
                    string[] arr = line.Split(ch);
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = arr[j];
                    }
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }





        /// <summary>
        /// 将DataTable中数据写入到CSV文件中
        /// </summary>
        /// <param name="dt">提供保存数据的DataTable</param>
        /// <param name="fileName">CSV的文件路径</param>
        /// <param name="isShowHeader">是否显示头部</param>
        public static void SaveCSV(DataTable dt, string fullPath,bool isShowHeader)
        {

            try
            {
                FileInfo fi = new FileInfo(fullPath);

                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                FileStream fs = new FileStream(fullPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);

                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                string data = "";

                #region  ////写出列名称
                if (isShowHeader == true)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        data += dt.Columns[i].ColumnName.ToString();
                        if (i < dt.Columns.Count - 1)
                        {
                            data += ',';
                        }
                    }
                    sw.WriteLine(data);
                }
                #endregion

                //写出各行数据
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    data = "";
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string str = dt.Rows[i][j].ToString();
                        str = str.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
                        if (str.Contains(',') || str.Contains('"')
                            || str.Contains('\r') || str.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
                        {
                            str = string.Format("\"{0}\"", str);
                        }

                        data += str;
                        if (j < dt.Columns.Count - 1)
                        {
                            data += ",";
                        }
                    }
                    sw.WriteLine(data);
                }
                sw.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        // 针对淘宝标题裂变
        // 复制第3行数据，修改标题和商家编号
        public static void UpdateTable(DataTable dt, List<string> listTitle)
        {
            try
            {
                if (listTitle == null || listTitle.Count == 0)
                {
                    return;
                }

                int columnCount = dt.Columns.Count;
                int rowCount = dt.Rows.Count;

                DataRow row1 = dt.Rows[1]; // 第1列是标题
                DataRow row3 = dt.Rows[3]; // 第3列是原始数据
                // 标题是  title
                // 商家编码要加下划线 "outer_id";
                int titleIndex = 0;
                int sjbmIndex = 0;
                for (int i = 0; i < columnCount; i++)
                {
                    string field = Convert.ToString(row1[i]);
                    if (field == "title")
                    {
                        titleIndex = i;
                    }
                    else if (field == "outer_id")
                    {
                        sjbmIndex = i;
                    }
                }

                DataRow row = null;
                // 复制n行
                for (int n = 0, i = 4; n < listTitle.Count; n++, i++)
                {

                    if (rowCount <= i)
                    {
                        row = dt.NewRow();
                        dt.Rows.Add(row);
                    }
                    else
                    {
                        row = dt.Rows[i];
                    }
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j == titleIndex) // 标题
                        {
                            row[j] = listTitle[n];
                        }
                        else if (j == sjbmIndex)  // 商家编码
                        {
                            row[j] = row3[j] + "_" + (n + 1);
                        }
                        else
                        {
                            row[j] = row3[j];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



    }

}
