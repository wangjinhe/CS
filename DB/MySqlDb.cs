using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections;

namespace CS.DB
{
    /// <summary>
    /// MySql数据库操作类
    /// </summary>
    public class MySqlDb
    {


        /// <summary>
        /// 获取连接字符串 返回如下：
        /// Server=;port=3306;User ID=;password=;database=;charset=utf8
        /// </summary>
        /// <param name="server">服务ip</param>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="database">数据库名</param>
        /// <param name="port">端口号</param>
        /// <param name="charset">字符码</param>
        /// <returns>返回连接字符串</returns>
        public static string GetConnString(string server, string user, string password, string database, string port = "3306", string charset = "utf8")
        {
            string sConn = string.Format("Server={0};port={1};User ID={2};password={3};database={4};charset={5};Allow User Variables=True", server, port, user, password, database, charset);
            return sConn;
        }

        /// <summary>
        /// 执行sql语句，并自动关闭
        /// </summary>
        /// <param name="connstr"></param>
        /// <param name="sSql"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connstr, string sSql)
        {
            using (MySqlConnection conn = new MySqlConnection(connstr))
            {
                conn.Open();
                using (MySqlCommand comm = new MySqlCommand(sSql, conn))
                {
                    comm.CommandTimeout = 0; // 秒，10分钟
                    int iFlag = comm.ExecuteNonQuery();
                    conn.Close();
                    return iFlag;
                }
            }

        }


        /// <summary>
        /// 获取数据集，并自动关闭连接
        /// </summary>
        /// <param name="connstr"></param>
        /// <param name="sSql"></param>
        /// <returns></returns>
        public static DataSet GetDateSet(string connstr, string sSql)
        {
            using (MySqlConnection conn = new MySqlConnection(connstr))
            {
                conn.Open();
                DataSet ds = new DataSet();
                using (MySqlCommand comm = new MySqlCommand(sSql, conn))
                {
                    comm.CommandTimeout = 0; // 10分钟               
                    MySqlDataAdapter adapter = new MySqlDataAdapter(comm);
                    adapter.Fill(ds);
                    conn.Close();
                    return ds;
                }
            }
        }

        public static bool IsExist(string connstr, string sTableName, string sFilter)
        {
            string sSql = string.Format("select 1 from {0} where 1=1 {1}", sTableName, sFilter);
            object o = ExecuteScalar(connstr, sSql);
            if (o != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static object ExecuteScalar(string connstr, string sSql)
        {
            using (MySqlConnection conn = new MySqlConnection(connstr))
            {
                conn.Open();
                using (MySqlCommand comm = new MySqlCommand(sSql, conn))
                {
                    comm.CommandTimeout = 0; // 秒，5分钟
                    object o = comm.ExecuteScalar();
                    conn.Close();
                    return o;
                }
            }
        }


        /// <summary>
        /// 插入数据
        ///二进制blob类型的值类型为 byte []
        /// </summary>
        /// <param name="connstr"></param>
        /// <param name="table">表名称</param>
        /// <param name="ht">字段为key,值为value</param>
        /// <returns></returns>
        public static int AddData(string connstr, string table, Hashtable ht)
        {
            using (MySqlConnection conn = new MySqlConnection(connstr))
            {
                conn.Open();
                MySqlParameter[] param = new MySqlParameter[ht.Count];
                StringBuilder sbField = new StringBuilder(); // 字段
                StringBuilder sbParam = new StringBuilder(); //参数

                int i = 0;
                foreach (DictionaryEntry de in ht)
                {
                    if (i < ht.Count - 1)
                    {
                        sbField.Append(de.Key + ",");
                        sbParam.Append("?" + de.Key + ",");
                    }
                    else
                    {
                        sbField.Append(de.Key);
                        sbParam.Append("?" + de.Key);
                    }
                    param[i++] = new MySqlParameter("?" + de.Key, de.Value);
                }

                string sSql = string.Format("insert into {0}({1}) values({2})", table, sbField.ToString(), sbParam.ToString());

                using (MySqlCommand comm = new MySqlCommand(sSql, conn))
                {
                    comm.CommandTimeout = 0; // 10分钟
                    comm.Parameters.AddRange(param);
                    int iFlag = comm.ExecuteNonQuery();
                    conn.Close();
                    return iFlag;
                }
            }
        }

        public static int UpdateData(string connstr, string table, string filter, Hashtable ht)
        {
            using (MySqlConnection conn = new MySqlConnection(connstr))
            {
                conn.Open();
                MySqlParameter[] param = new MySqlParameter[ht.Count];
                StringBuilder sbField = new StringBuilder(); // 字段
                StringBuilder sbParam = new StringBuilder(); //参数
                int i = 0;
                foreach (DictionaryEntry de in ht)
                {
                    if (i < ht.Count - 1)
                    {
                        sbParam.Append(de.Key + "=?" + de.Key + ",");
                    }
                    else
                    {
                        sbParam.Append(de.Key + "=?" + de.Key);
                    }
                    param[i++] = new MySqlParameter("?" + de.Key, de.Value);
                }
                string sSql = string.Format("update {0} set {1} where 1=1 {2};", table, sbParam.ToString(), filter);
                using (MySqlCommand comm = new MySqlCommand(sSql, conn))
                {
                    comm.CommandTimeout = 0; // 10分钟
                    comm.Parameters.AddRange(param);
                    int iFlag = comm.ExecuteNonQuery();
                    conn.Close();
                    return iFlag;
                }
            }

        }

        public static int SaveData(string connstr, string table, string filter, Hashtable ht)
        {
            bool isExist = IsExist(connstr, table, filter);
            int iFlag = 0;

            if (isExist)
            {
                iFlag = UpdateData(connstr, table, filter, ht);
            }
            else
            {
                iFlag = AddData(connstr, table, ht);
            }
            return iFlag;
        }

    }
}
