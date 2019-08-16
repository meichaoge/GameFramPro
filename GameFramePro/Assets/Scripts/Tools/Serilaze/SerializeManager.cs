using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft;
using LitJson;
using Newtonsoft.Json;

namespace GameFramePro
{
    /// <summary>
    /// 使用的Json 解析器类型
    /// </summary>
    public enum JsonParserEnum
    {
        None = -1, //不可用初始状态
        NewtonJson,
        LitJson
    }

    /// <summary>
    /// 序列化
    /// </summary>
    public class SerializeManager : Single<SerializeManager>
    {
        public static JsonParserEnum S_AppJsonParserEnum { get; set; } = JsonParserEnum.NewtonJson;



        #region 反序列化

        public static object DeserializeObject(string value)
        {
            if (S_AppJsonParserEnum == JsonParserEnum.NewtonJson)
                return JsonConvert.DeserializeObject(value);
            if (S_AppJsonParserEnum == JsonParserEnum.LitJson)
                return JsonMapper.ToObject(value);
            Debug.LogError("Not Support " + S_AppJsonParserEnum);
            throw new NotImplementedException();
        }

        public static T DeserializeObject<T>(string value)
        {
            if (S_AppJsonParserEnum == JsonParserEnum.NewtonJson)
                return JsonConvert.DeserializeObject<T>(value);
            if (S_AppJsonParserEnum == JsonParserEnum.LitJson)
                return JsonMapper.ToObject<T>(value);
            Debug.LogError("Not Support " + S_AppJsonParserEnum);
            throw new NotImplementedException();
        }
        #endregion

        #region 序列化

        public static string SerializeObject(object value)
        {
            if (S_AppJsonParserEnum == JsonParserEnum.NewtonJson)
                return JsonConvert.SerializeObject(value);
            if (S_AppJsonParserEnum == JsonParserEnum.LitJson)
                return JsonMapper.ToJson(value);
            Debug.LogError("Not Support " + S_AppJsonParserEnum);
            throw new NotImplementedException();
        }





        #endregion

    }
}