using System.Data;
using System.IO;
using System.Text;


namespace GameFramePro.EditorEx
{
    /// <summary>
    /// 转化成xml
    /// </summary>
    public class ExcelToXmlUtility
    {
        /// <summary>
        /// Excel 转成 Xml
        /// </summary>
        /// <param name="dataSources"></param>
        /// <param name="xmlPath">导出后的文件存储路径</param>
        /// <param name="exportColumn">导出哪些列</param>
        public static void ConvertExcelToXml(DataTable dataSources, string xmlPath, int[] exportColumn)
        {
            if (exportColumn == null || exportColumn.Length == 0)
            {
                Debug.LogError("ConvertExcelToXml Fail,需要给定导出哪些列的数据");
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

            //创建Xml文件头
            stringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            stringBuilder.Append("\r\n");
            //创建根节点
            stringBuilder.Append("<Table>");
            stringBuilder.Append("\r\n");
            //读取数据
            for (int row = 2; row < rowCount; row++)
            {
                //创建子节点
                stringBuilder.Append("  <Row>");
                stringBuilder.Append("\r\n");
                for (int dex = 0; dex < exportColumn.Length; dex++)
                {
                    if (dex >= colCount) continue;
                    int column = exportColumn[dex];

                    stringBuilder.Append("   <" + fileInfor[column].ToString() + ">");
                    stringBuilder.Append(dataSources.Rows[row][column].ToString());
                    stringBuilder.Append("</" + fileInfor[column].ToString() + ">");
                    stringBuilder.Append("\r\n");
                }
                //使用换行符分割每一行
                stringBuilder.Append("  </Row>");
                stringBuilder.Append("\r\n");
            }

            #endregion
            stringBuilder.Append("</Table>");
            //写入文件
            using (FileStream fileStream = new FileStream(xmlPath, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    textWriter.Write(stringBuilder.ToString());
                }
                Debug.Log("Excel 导出xml到路径" + xmlPath);
            }
        }
    }
}