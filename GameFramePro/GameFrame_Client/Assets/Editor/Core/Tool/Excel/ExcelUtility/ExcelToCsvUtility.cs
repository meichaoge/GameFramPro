using System.Data;
using System.IO;
using System.Text;


namespace GameFramePro.EditorEx
{
    /// <summary>
    /// 将Excel 转换成CSV
    /// </summary>
    public class ExcelToCsvUtility
    {
        /// <summary>
        /// Excel 转成Json
        /// </summary>
        /// <param name="dataSources"></param>
        /// <param name="csvPath">导出后的文件存储路径</param>
        /// <param name="exportColumn">导出哪些列</param>
        public static void ConvertExcelToCsv(DataTable dataSources, string csvPath,int[] exportColumn)
        {
            if(exportColumn==null|| exportColumn.Length == 0)
            {
                Debug.LogError("ConvertExcelToCsv Fail,需要给定导出哪些列的数据");
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


            #region 读取数据
            DataRow fileInfor = dataSources.Rows[0];   //第一行字段名称
            DataRow formationInfor = dataSources.Rows[1];  //第二行字段类型

            //创建一个StringBuilder存储数据
            StringBuilder stringBuilder = new StringBuilder();

            for (int row = 2; row < rowCount; ++row)
            {
                for (int dex = 0; dex < exportColumn.Length; dex++)
                {
                    if(dex< colCount)
                    {
                        int column = exportColumn[dex];

                        //读取第1行数据作为表头字段
                        string field = fileInfor[column].ToString();
                        string fileFormation = formationInfor[column].ToString();
                        //Key-Value对应
                        stringBuilder.Append(dataSources.Rows[row][column] + ",");
                    }
                }
               
                //使用换行符分割每一行
                stringBuilder.Append("\r\n");
                //添加到表数据中
            }

            #endregion


            //写入文件
            using (FileStream fileStream = new FileStream(csvPath, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    textWriter.Write(stringBuilder.ToString());
                }
                Debug.Log("Excel 导出CSV到路径" + csvPath);
            }


        }
    }
}