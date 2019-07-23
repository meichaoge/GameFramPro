using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;


namespace GameFramePro.EditorEx
{
    /// <summary>
    /// 将Excel 数据转化成json
    /// </summary>
    public class ExcelToJsonUtility
    {
        /// <summary>
        /// 将制定的表格数据转化成Json 数据
        /// </summary>
        /// <param name="dataSources"></param>
        /// <param name="JsonPath">导出后的文件存储路径</param>
        /// <param name="exportColumn">导出哪些列</param>
        public static void ConvertExcelToJson(DataTable dataSources, string JsonPath, int[] exportColumn)
        {
            if (exportColumn == null || exportColumn.Length == 0)
            {
                Debug.LogError("ConvertExcelToJson Fail,需要给定导出哪些列的数据");
                return;
            }
            //判断数据表内是否存在数据
            if (dataSources.Rows.Count < 2)
            {
                Debug.LogError("当前读取的表中不包含任何数据" + dataSources.TableName);
                return;
            }
            //读取数据表行数和列数
            int rowCount = dataSources.Rows.Count;
            int colCount = dataSources.Columns.Count;

            //准备一个列表存储整个表的数据
            List<Dictionary<string, object>> tableInfor = new List<Dictionary<string, object>>();

            #region 读取数据
            DataRow fileInfor = dataSources.Rows[0];   //第一行字段名称
            DataRow formationInfor = dataSources.Rows[1];  //第二行字段类型

            for (int row = 2; row < rowCount; ++row)
            {
                //准备一个字典存储每一行的数据
                Dictionary<string, object> rowData = new Dictionary<string, object>();
                for (int dex = 0; dex < exportColumn.Length; dex++)
                {
                    if (dex < colCount)
                    {
                        int column = exportColumn[dex];

                        //读取第1行数据作为表头字段
                        string field = fileInfor[column].ToString();
                        string fileFormation = formationInfor[column].ToString();
                        //Key-Value对应
                        rowData[field] = dataSources.Rows[row][column];
                    }
                }
                //添加到表数据中
                tableInfor.Add(rowData);
            }

            #endregion


            //生成Json字符串
            string json = SerilazeManager.SerializeObject(tableInfor);
            //写入文件
            using (FileStream fileStream = new FileStream(JsonPath, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    textWriter.Write(json);
                }
                Debug.Log("Excel 导出Json 到路径" + JsonPath);
            }
        }


        /// <summary>
        /// 检测指定的字段格式是否正确
        /// </summary>
        /// <param name="fileName">字段名</param>
        /// <param name="fileType">字段类型</param>
        /// <param name="fileData">字段数据</param>
        /// <returns></returns>
        private static bool CheckDataFormat(string fileName, string fileType, string fileData)
        {
            Type type = Type.GetType(fileType);
            if (type == null)
            {
                Debug.LogError(string.Format("FileName={0} Type Define Error,Not Exit Type:{1}", fileName, fileType));
                return false;
            }
            return true;
        }


    }
}