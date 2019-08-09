using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro
{

    /// <summary>
    /// 导出的配置文件格式
    /// </summary>
    [System.Flags]
    public enum ExportFormatEnum
    {
        Csv = 1,
        Json = 2,
        Xml = 4,
    }
}