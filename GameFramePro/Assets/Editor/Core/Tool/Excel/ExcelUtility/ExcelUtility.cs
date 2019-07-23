using Excel;
using System.Collections;
using System.Data;
using System.IO;


namespace GameFramePro.EditorEx
{
    /// <summary>
    /// 读取Excel 并转换为其他格式
    /// Excel 格式要求：
    /// (1)第一行为字段名称   
    /// (2)第二行为数据类型(支持类型String,Int,Float,Double,)
    /// </summary>
    public static class ExcelUtility
    {


        /// <summary>
        /// 读取Excel 表格数据
        /// </summary>
        /// <param name="excelFile"></param>
        public static DataSet GetExcelData(string excelFile)
        {
            if (string.IsNullOrEmpty(excelFile))
            {
                Debug.LogError("GetExcelDataFail, 指定的目录为null");
                return null;
            }

            if (System.IO.File.Exists(excelFile) == false)
            {
                Debug.LogError("GetExcelDataFail,指定路径的文件不存在" + excelFile);
                return null;
            }


            FileStream mStream = null;
            DataSet mResultSet = null;
            try
            {
                mStream = File.Open(excelFile, FileMode.Open, FileAccess.Read);
                IExcelDataReader mExcelReader = ExcelReaderFactory.CreateOpenXmlReader(mStream);
                mResultSet = mExcelReader.AsDataSet();
                return mResultSet;
            }
            catch (System.Exception e)
            {
                Debug.LogError("数据读取异常" + e);
                return null;
            }
            finally
            {
                if (mStream != null)
                    mStream.Close();
            }

        }

    }
}