using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramePro.EditorEx
{
    public class ExcelToLuaUtility
    {
        /// <summary>
        /// Excel 转成Lua
        /// </summary>
        /// <param name="dataSources"></param>
        /// <param name="luaPath">导出后的文件存储路径</param>
        /// <param name="exportColumn">导出哪些列</param>
        public static void ConvertExcelToLua(DataTable dataSources, string luaPath, int[] exportColumn)
        {
            if (exportColumn == null || exportColumn.Length == 0)
            {
                Debug.LogError("ConvertExcelToLua Fail,需要给定导出哪些列的数据");
                return;
            }
            //判断数据表内是否存在数据
            if (dataSources.Rows.Count < 2)
            {
                Debug.LogError("当前读取的表中不包含任何数据" + dataSources.TableName);
                return;
            }

            #region 读取表格数据

            //读取数据表行数和列数
            int rowCount = dataSources.Rows.Count;
            int colCount = dataSources.Columns.Count;


            DataRow fileInfor = dataSources.Rows[0];   //第一行字段名称
            DataRow formationInfor = dataSources.Rows[1];  //第二行字段类型
                                                           //准备一个列表存储整个表的数据
            List<Dictionary<string, object>> tableInfor = new List<Dictionary<string, object>>();

            for (int row = 2; row < rowCount; ++row)
            {
                for (int dex = 0; dex < exportColumn.Length; dex++)
                {
                    if (dex < colCount)
                    {
                        int column = exportColumn[dex];
                        //准备一个字典存储每一行的数据
                        Dictionary<string, object> rowData = new Dictionary<string, object>();

                        //读取第1行数据作为表头字段
                        string field = fileInfor[column].ToString();
                        string fileFormation = formationInfor[column].ToString();
                        //Key-Value对应
                        rowData[field] = dataSources.Rows[row][column];
                        //添加到表数据中
                        tableInfor.Add(rowData);
                    }
                }
            }
            #endregion


            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("local datas = {");
            stringBuilder.Append("\r\n");

            stringBuilder.Append(string.Format("\t\"{0}\" = ", dataSources.TableName));
            stringBuilder.Append("{\r\n");
            foreach (Dictionary<string, object> dic in tableInfor)
            {
                stringBuilder.Append("\t\t{\r\n");
                foreach (string key in dic.Keys)
                {
                    if (dic[key].GetType().Name == "String")
                        stringBuilder.Append(string.Format("\t\t\t\"{0}\" = \"{1}\",\r\n", key, dic[key]));
                    else
                        stringBuilder.Append(string.Format("\t\t\t\"{0}\" = {1},\r\n", key, dic[key]));
                }
                stringBuilder.Append("\t\t},\r\n");
            }
            stringBuilder.Append("\t}\r\n");



            stringBuilder.Append("}\r\n");
            stringBuilder.Append("return datas");

            //写入文件
            using (FileStream fileStream = new FileStream(luaPath, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    textWriter.Write(stringBuilder.ToString());
                }
                Debug.Log("Excel 导出Lua到路径" + luaPath);
            }
        }

    }
}
