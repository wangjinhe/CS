/*创建日期：2014-5-7
 *创建人：王金河
 *内容：使用NPOI插件操作Excel
 * 
 *说明：
 *(1)NPOI是一组开源的组件,包括：NPOI、NPOI.HPSF、NPOI.HSSF、NPOI.HSSF.UserModel、NPOI.POIFS、NPOI.Util.
 *(2)优点：读取Excel速度较快，读取方式操作灵活性
 *(3)缺点：只支持03的Excel，xlsx的无法读取(2012年)。03版Excel对于行数还有限制，只支持65536行。
 *(4)本类暂时只提供读取操作(2014-5-7)。
 * 
 * 修改日期：2016-6-14
 * 修改内容：增加xlsx的支持；可以写入excel
 * 修改人：一休
 * 参考：http://www.cnblogs.com/luxiaoxun/p/3374992.html
 * 说明：暂时不管用，写入请使用ExcelHelper.cs
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace CS.Office
{
    /// <summary>
    /// 使用NPOI插件操作Excel
    /// </summary>
    public class Excel
    {
        #region 变量和构造函数

        /// <summary>
        /// Excel文件路径,必须是服务器上的物理路径
        /// </summary>
        private string m_filePath;

        /// <summary>
        /// 当前工作簿，就是当前的Excel文档
        /// </summary>
        private IWorkbook m_book;

        /// <summary>
        /// 当前工作表。
        /// </summary>
        private ISheet m_sheet;

        /// <summary>
        /// 构造函数，初始化工作表sheet,默认为第一个
        /// </summary>
        /// <param name="sFilePath">文件路径，必须是服务器上的物理路径,浏览器上的文件必须先保存到服务器</param>
        public Excel(string sFilePath)
        {
            m_filePath = sFilePath;

            if (!File.Exists(m_filePath))
            {
                throw new FileNotFoundException("文件不存在");
            }
            FileStream fs = File.OpenRead(m_filePath);
            // m_book = new HSSFWorkbook(fs);
            if (m_filePath.IndexOf(".xlsx") > 0) // 2007版本
            {
                m_book = new XSSFWorkbook(fs);
            }
            else //  if (m_filePath.IndexOf(".xls") > 0) // 2003版本
            {
                m_book = new HSSFWorkbook(fs);
            }
          
            m_sheet = m_book.GetSheetAt(0);

            fs.Close();

        }

        #endregion

        #region 设置当前sheet

        /// <summary>
        /// 设置当前sheet
        /// </summary>
        /// <param name="i">sheet序号，从0开始</param>
        /// <returns>设置成功返回true,设置失败返回false,同时sheet设置为null</returns>
        public bool SetCurrentSheet(int i)
        {
            if (i >= m_book.NumberOfSheets)
            {
                m_sheet = null;
                return false;
            }
            else
            {
                m_sheet = m_book.GetSheetAt(i);
                return true;
            }
        }

        #endregion

        #region 获取单元格的值
        /// <summary>
        /// 获取单元格的内容
        /// </summary>      
        /// <param name="iRow">行数,从0开始</param>
        /// <param name="iCol">列数，从0开始</param>
        /// <returns>返回单元格的值，sheet为空返回""</returns>
        public string GetValue(int iRow, int iCol)
        {
            try
            {
                if (m_sheet == null)
                {
                    return "";
                }
                IRow row =  m_sheet.GetRow(iRow);
                if (row == null)
                {
                    return "";
                }
                ICell cell = row.GetCell(iCol);
                string sValue = "";
                sValue = cell == null ? "" : cell.ToString();
                #region
                if (cell != null && sValue != "" && cell.CellType == CellType.Numeric)
                {
                    short format = cell.CellStyle.DataFormat;
                    if (format == 14 || format == 31 || format == 57 || format == 58)
                    {
                        DateTime date = cell.DateCellValue;
                        sValue = date.ToString("yyyy-MM-dd");
                    }
                }
                #endregion


                return sValue;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        #endregion


        #region 设置单元格的值
        /// <summary>
        /// 设置单元格的内容
        /// 说明：暂时不管用，写入请使用ExcelHelper.cs
        /// </summary>      
        /// <param name="iRow">行数,从0开始</param>
        /// <param name="iCol">列数，从0开始</param>
        /// <param name="value">内容</param>
        /// <returns>返回单元格的值，sheet为空返回""</returns>
        public bool SetValue(int iRow, int iCol, string value)
        {
            bool ok = true;
            try
            {
                if (m_sheet == null)
                {
                    return false;
                }

               
                //IRow row = m_sheet.GetRow(iRow);
                //if (row == null)
                //{
                //    return false;
                //}
                //ICell cell = row.GetCell(iCol);
                //cell.SetCellValue(value);
                IRow row = m_sheet.CreateRow(iRow);
                row.CreateCell(iCol).SetCellValue(value);
                
                return ok;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion

        #region 获取单元格的内容
        /// <summary>
        /// 获取单元格的内容
        /// </summary>     
        /// <param name="sCellName">单元格名称，如A1</param>    
        /// <returns>单元格内容</returns>
        public string GetValue(string sCellName)
        {
            if (m_sheet == null)
            {
                return "";
            }
            int iRow, iCol;
            GetRowCol(sCellName, out iRow, out iCol);
            return GetValue(iRow, iCol);
        }
        #endregion

        #region 根据单元格的名称获取行数和列数
        /// <summary>
        /// 根据单元格的名称获取行数和列数
        /// </summary>
        /// <param name="sCellName">单元格的名称,如 A1，调用者必须保证参数的正确，此处不做判断，以提高速度</param>
        /// <param name="iRow">单元格的行数,如 A1 返回 0 </param>
        /// <param name="iCol">单元格的列数,如 A1 返回 0 </param>
        public void GetRowCol(string sCellName, out int iRow, out int iCol)
        {
            iRow = iCol = 0;
            for (int i = 0, size = sCellName.Length; i < size; i++)
            {
                if (sCellName[i] >= 'A' && sCellName[i] <= 'Z') // 如果是字母，代表 列
                {
                    iCol += iCol * 26 + sCellName[i] - 'A';
                }
                else  // 处理 数字
                {
                    iRow = Convert.ToInt32(sCellName.Substring(i)) - 1;
                    return; // 对于遇到数字，直接截取转换
                }
            }

        }
        #endregion


    }
}
