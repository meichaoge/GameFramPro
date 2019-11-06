using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace GameFramePro
{
    /// <summary>
    /// 协助处理CSV文件的读写
    /// </summary>
    public class CsvUtility : Single<CsvUtility>
    {
        /// <summary>
        /// ** 避免不同平台对换行符的界定不一致 (System.Environment.NewLine 在不同平台下解析同一个文件得到的结果不一致) 
        /// </summary>
        public static readonly string CSVNewline = "\n";
        public static readonly char CsvSeparatorChar = ','; //csv 文件中分割两个数据的标识符
        public static readonly char CsvVectorSeparatorChar = '&'; //csv 文件中分割两个数据的标识符

        //public delegate void CsvReadLineHandler<T>(string line, out T data1);
        //public delegate void CsvReadLineHandler<T,V>(string line, out T data1,out V data2);
        //public delegate void CsvReadLineHandler<T, V,W>(string line, out T data1, out V data2,out W data3);
        //public delegate void CsvReadLineHandler<T, V, W,X>(string line, out T data1, out V data2, out W data3,out X data4);
        //public delegate void CsvReadLineHandler<T, V, W, X,Y>(string line, out T data1, out V data2, out W data3, out X data4,out Y data5);


        private static bool s_IsInitialed = false;

        private static void RegistFormatCallback()
        {
            if (s_IsInitialed) return;
            FormatUtility<string, string>.RegisterHandler(S_Instance, typeof(string), ReadCsv_String);
            FormatUtility<bool, string>.RegisterHandler(S_Instance, typeof(bool), ReadCsv_bool);
            FormatUtility<int, string>.RegisterHandler(S_Instance, typeof(int), ReadCsv_Int);
            FormatUtility<uint, string>.RegisterHandler(S_Instance, typeof(uint), ReadCsv_Uint);
            FormatUtility<long, string>.RegisterHandler(S_Instance, typeof(long), ReadCsv_Long);
            FormatUtility<ulong, string>.RegisterHandler(S_Instance, typeof(ulong), ReadCsv_Ulong);
            FormatUtility<float, string>.RegisterHandler(S_Instance, typeof(float), ReadCsv_Float);
            FormatUtility<double, string>.RegisterHandler(S_Instance, typeof(double), ReadCsv_Double);
            FormatUtility<decimal, string>.RegisterHandler(S_Instance, typeof(decimal), ReadCsv_Decimal);
            FormatUtility<Vector2, string>.RegisterHandler(S_Instance, typeof(Vector2), ReadCsv_Vector2);
            FormatUtility<Vector3, string>.RegisterHandler(S_Instance, typeof(Vector3), ReadCsv_Vector3);
            s_IsInitialed = true;
        }


        #region 写数据



        public static bool WriteCsv_NewLine(ref StringBuilder builder)
        {
            Debug.Assert(builder != null);
            //   builder.Append(System.Environment.NewLine);
            builder.Append(CSVNewline);
            return true;
        }

        /// <summary>
        /// 写CSV 数据，如果字符串中包含 csv 分隔符会报错
        /// </summary>
        /// <param name="data"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static bool WriteCsv(string data, ref StringBuilder builder)
        {
            Debug.Assert(builder != null);
            if (string.IsNullOrEmpty(data) == false && data.IndexOf(CsvSeparatorChar) != -1)
            {
                Debug.LogError(string.Format("WriteCsv Fail, parameter data={0} Format Error, Can't Contain char= {1}", data, CsvSeparatorChar));
                builder.Append(CsvSeparatorChar);
                return false;
            }

            builder.Append(data).Append(CsvSeparatorChar);
            return true;
        }


        public static bool WriteCsv(bool data, ref StringBuilder builder)
        {
            Debug.Assert(builder != null);
            builder.Append(data).Append(CsvSeparatorChar);
            return true;
        }

        public static bool WriteCsv(int data, ref StringBuilder builder)
        {
            Debug.Assert(builder != null);
            builder.Append(data).Append(CsvSeparatorChar);
            return true;
        }

        public static bool WriteCsv(uint data, ref StringBuilder builder)
        {
            Debug.Assert(builder != null);
            builder.Append(data).Append(CsvSeparatorChar);
            return true;
        }


        public static bool WriteCsv(long data, ref StringBuilder builder)
        {
            Debug.Assert(builder != null);
            builder.Append(data).Append(CsvSeparatorChar);
            return true;
        }

        public static bool WriteCsv(ulong data, ref StringBuilder builder)
        {
            Debug.Assert(builder != null);
            builder.Append(data).Append(CsvSeparatorChar);
            return true;
        }

        public static bool WriteCsv(float data, ref StringBuilder builder)
        {
            Debug.Assert(builder != null);
            builder.Append(data).Append(CsvSeparatorChar);
            return true;
        }

        public static bool WriteCsv(double data, ref StringBuilder builder)
        {
            Debug.Assert(builder != null);
            builder.Append(data).Append(CsvSeparatorChar);
            return true;
        }

        public static bool WriteCsv(decimal data, ref StringBuilder builder)
        {
            Debug.Assert(builder != null);
            builder.Append(data).Append(CsvSeparatorChar);
            return true;
        }


        /// <summary>
        /// Vector 每个分量之间使用 CsvVectorSeparatorChar 符号区分
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static bool WriteCsv(Vector2 vector, ref StringBuilder builder)
        {
            Debug.Assert(builder != null);
            builder.Append(vector.x).Append(CsvVectorSeparatorChar).Append(vector.y).Append(CsvSeparatorChar);
            return true;
        }

        /// <summary>
        /// Vector 每个分量之间使用 CsvVectorSeparatorChar 符号区分
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static bool WriteCsv(Vector3 vector, ref StringBuilder builder)
        {
            Debug.Assert(builder != null);
            builder.Append(vector.x).Append(CsvVectorSeparatorChar).Append(vector.y).Append(CsvVectorSeparatorChar).Append(vector.z).Append(CsvSeparatorChar);
            return true;
        }

        #endregion

        #region 读数据 (目前不能是静态的 否则无法注册处理回调)

        public static string ReadCsv_String(string csvData)
        {
            return ReadCsv_String(csvData, out var isSuccess);
        }

        public static string ReadCsv_String(string csvData, out bool isSuccess)
        {
            RegistFormatCallback();
            isSuccess = true;
            return csvData;
        }


        public static bool ReadCsv_bool(string csvData)
        {
            return ReadCsv_bool(csvData, out var isSuccess);
        }

        public static bool ReadCsv_bool(string csvData, out bool isSuccess)
        {
            RegistFormatCallback();
            if (string.IsNullOrEmpty(csvData))
            {
                Debug.LogError("ReadCsv Fail,Parameter is null Or empty");
                isSuccess = false;
                return false;
            }

            isSuccess = true;
            return bool.Parse(csvData);
        }


        public static int ReadCsv_Int(string csvData)
        {
            return ReadCsv_Int(csvData, out var isSuccess);
        }

        public static int ReadCsv_Int(string csvData, out bool isSuccess)
        {
            RegistFormatCallback();
            if (string.IsNullOrEmpty(csvData))
            {
                Debug.LogError("ReadCsv Fail,Parameter is null Or empty");
                isSuccess = false;
                return 0;
            }

            isSuccess = true;
            return int.Parse(csvData);
        }


        public static uint ReadCsv_Uint(string csvData)
        {
            return ReadCsv_Uint(csvData, out var isSuccess);
        }

        public static uint ReadCsv_Uint(string csvData, out bool isSuccess)
        {
            RegistFormatCallback();
            if (string.IsNullOrEmpty(csvData))
            {
                Debug.LogError("ReadCsv Fail,Parameter is null Or empty");
                isSuccess = false;
                return 0;
            }

            isSuccess = true;
            return uint.Parse(csvData);
        }


        public static long ReadCsv_Long(string csvData)
        {
            return ReadCsv_Long(csvData, out var isSuccess);
        }

        public static long ReadCsv_Long(string csvData, out bool isSuccess)
        {
            RegistFormatCallback();
            if (string.IsNullOrEmpty(csvData))
            {
                Debug.LogError("ReadCsv Fail,Parameter is null Or empty");
                isSuccess = false;
                return 0L;
            }

            isSuccess = true;
            return long.Parse(csvData);
        }


        public static ulong ReadCsv_Ulong(string csvData)
        {
            return ReadCsv_Ulong(csvData, out var isSuccess);
        }

        public static ulong ReadCsv_Ulong(string csvData, out bool isSuccess)
        {
            RegistFormatCallback();
            if (string.IsNullOrEmpty(csvData))
            {
                Debug.LogError("ReadCsv Fail,Parameter is null Or empty");
                isSuccess = false;
                return 0L;
            }

            isSuccess = true;
            return ulong.Parse(csvData);
        }

        public static float ReadCsv_Float(string csvData)
        {
            return ReadCsv_Float(csvData, out var isSuccess);
        }

        public static float ReadCsv_Float(string csvData, out bool isSuccess)
        {
            RegistFormatCallback();
            if (string.IsNullOrEmpty(csvData))
            {
                Debug.LogError("ReadCsv Fail,Parameter is null Or empty");
                isSuccess = false;
                return 0f;
            }

            isSuccess = true;
            return float.Parse(csvData);
        }

        public static double ReadCsv_Double(string csvData)
        {
            return ReadCsv_Double(csvData, out var isSuccess);
        }

        public static double ReadCsv_Double(string csvData, out bool isSuccess)
        {
            RegistFormatCallback();
            if (string.IsNullOrEmpty(csvData))
            {
                Debug.LogError("ReadCsv Fail,Parameter is null Or empty");
                isSuccess = false;
                return 0d;
            }

            isSuccess = true;
            return double.Parse(csvData);
        }


        public static decimal ReadCsv_Decimal(string csvData)
        {
            return ReadCsv_Decimal(csvData, out var isSuccess);
        }

        public static decimal ReadCsv_Decimal(string csvData, out bool isSuccess)
        {
            RegistFormatCallback();
            if (string.IsNullOrEmpty(csvData))
            {
                Debug.LogError("ReadCsv Fail,Parameter is null Or empty");
                isSuccess = false;
                return 0;
            }

            isSuccess = true;
            return decimal.Parse(csvData);
        }


        public static Vector2 ReadCsv_Vector2(string csvData)
        {
            return ReadCsv_Vector2(csvData, out var isSuccess);
        }

        public static Vector2 ReadCsv_Vector2(string csvData, out bool isSuccess)
        {
            RegistFormatCallback();
            if (string.IsNullOrEmpty(csvData))
            {
                Debug.LogError("ReadCsv Fail,Parameter is null Or empty");
                isSuccess = false;
                return Vector2.zero;
            }

            Vector2 result = Vector2.zero;
            string[] vectorDatas = csvData.Split(CsvVectorSeparatorChar);
            for (int dex = 0; dex < vectorDatas.Length; dex++)
            {
                if (dex == 0)
                    result.x = float.Parse(vectorDatas[dex]);
                else if (dex == 1)
                {
                    result.y = float.Parse(vectorDatas[dex]);
                    break;
                }
            }

            isSuccess = true;
            return result;
        }


        public static Vector3 ReadCsv_Vector3(string csvData)
        {
            return ReadCsv_Vector3(csvData, out var isSuccess);
        }

        public static Vector3 ReadCsv_Vector3(string csvData, out bool isSuccess)
        {
            RegistFormatCallback();
            if (string.IsNullOrEmpty(csvData))
            {
                Debug.LogError("ReadCsv Fail,Parameter is null Or empty");
                isSuccess = false;
                return Vector3.zero;
            }

            Vector3 result = Vector3.zero;
            string[] vectorDatas = csvData.Split(CsvVectorSeparatorChar);
            for (int dex = 0; dex < vectorDatas.Length; dex++)
            {
                if (dex == 0)
                    result.x = float.Parse(vectorDatas[dex]);
                else if (dex == 1)
                    result.y = float.Parse(vectorDatas[dex]);
                else if (dex == 2)
                {
                    result.z = float.Parse(vectorDatas[dex]);
                    break;
                }
            }

            isSuccess = true;
            return result;
        }

        #endregion


        #region 读CSV 扩展

        public static bool ReadCsvEx<T>(string csvData, out T data)
        {
            RegistFormatCallback();
            return FormatUtility<T, string>.FormatProcess(S_Instance, typeof(T), csvData, out data);
        }


        public static bool ReadLineCsv<T>(string csvData, out T data1)
        {
            RegistFormatCallback();
            bool isSuccess1 = FormatUtility<T, string>.FormatProcess(S_Instance, typeof(T), csvData, out data1);
            return isSuccess1;
        }

        public static bool ReadLineCsv<T, V>(string csvData, out T data1, out V data2)
        {
            RegistFormatCallback();
            string[] csvContent = StringUtility.SplitString(csvData, CsvSeparatorChar, 2, true);

            bool isSuccess1 = FormatUtility<T, string>.FormatProcess(S_Instance, typeof(T), csvContent[0], out data1);
            bool isSuccess2 = FormatUtility<V, string>.FormatProcess(S_Instance, typeof(V), csvContent[1], out data2);

            return isSuccess1 && isSuccess2;
        }

        public static bool ReadLineCsv<T, V, W>(string csvData, out T data1, out V data2, out W data3)
        {
            RegistFormatCallback();
            string[] csvContent = StringUtility.SplitString(csvData, CsvSeparatorChar, 2, true);

            bool isSuccess1 = FormatUtility<T, string>.FormatProcess(S_Instance, typeof(T), csvContent[0], out data1);
            bool isSuccess2 = FormatUtility<V, string>.FormatProcess(S_Instance, typeof(V), csvContent[1], out data2);
            bool isSuccess3 = FormatUtility<W, string>.FormatProcess(S_Instance, typeof(W), csvContent[2], out data3);

            return isSuccess1 && isSuccess2 && isSuccess3;
        }

        public static bool ReadLineCsv<T, V, W, X>(string csvData, out T data1, out V data2, out W data3, out X data4)
        {
            RegistFormatCallback();
            string[] csvContent = StringUtility.SplitString(csvData, CsvSeparatorChar, 3, true);

            bool isSuccess1 = FormatUtility<T, string>.FormatProcess(S_Instance, typeof(T), csvContent[0], out data1);
            bool isSuccess2 = FormatUtility<V, string>.FormatProcess(S_Instance, typeof(V), csvContent[1], out data2);
            bool isSuccess3 = FormatUtility<W, string>.FormatProcess(S_Instance, typeof(W), csvContent[2], out data3);
            bool isSuccess4 = FormatUtility<X, string>.FormatProcess(S_Instance, typeof(X), csvContent[3], out data4);

            return isSuccess1 && isSuccess2 && isSuccess3 && isSuccess4;
        }

        public static bool ReadLineCsv<T, V, W, X, Y>(string csvData, out T data1, out V data2, out W data3, out X data4, out Y data5)
        {
            RegistFormatCallback();
            string[] csvContent = StringUtility.SplitString(csvData, CsvSeparatorChar, 4, true);

            bool isSuccess1 = FormatUtility<T, string>.FormatProcess(S_Instance, typeof(T), csvContent[0], out data1);
            bool isSuccess2 = FormatUtility<V, string>.FormatProcess(S_Instance, typeof(V), csvContent[1], out data2);
            bool isSuccess3 = FormatUtility<W, string>.FormatProcess(S_Instance, typeof(W), csvContent[2], out data3);
            bool isSuccess4 = FormatUtility<X, string>.FormatProcess(S_Instance, typeof(X), csvContent[3], out data4);
            bool isSuccess5 = FormatUtility<Y, string>.FormatProcess(S_Instance, typeof(Y), csvContent[4], out data5);

            return isSuccess1 && isSuccess2 && isSuccess3 && isSuccess4 && isSuccess5;
        }

        #endregion 

        /// <summary>
        /// 将csv 字符串按照分隔符分割成一个个元素
        /// </summary>
        /// <param name="csvData"></param>
        /// <returns></returns>
        public static string[] SplitCsvStrings(string csvData)
        {
            if (string.IsNullOrEmpty(csvData))
            {
                Debug.LogError($"给定的csv 字符串不可分割");
                return null;
            }

            return csvData.Split(CsvSeparatorChar);
        }


        /// <summary>
        /// 将CSV 文件字符串按照换行符切割
        /// </summary>
        /// <returns></returns>
        public static string[] SplitCSVStringsToLines(string targetStr)
        {
            if (string.IsNullOrEmpty(targetStr))
            {
                Debug.LogError($"给定的字符串为null");
                return new string[0];
            }
            return targetStr.Split(CSVNewline.ToCharArray());
        }
    }
}