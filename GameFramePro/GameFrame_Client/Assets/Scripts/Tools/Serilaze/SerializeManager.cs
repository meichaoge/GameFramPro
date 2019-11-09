using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro
{
    /// <summary>/// 使用的Json 解析器类型/// </summary>
    public enum JsonParserEnum
    {
        None = -1, //不可用初始状态
        NewtonJson,
        LitJson
    }

    /// <summary>/// 统一的序列化管理器 /// </summary>
    public static class SerializeManager
    {
        public static JsonParserEnum S_AppJsonParserEnum { get; set; } = JsonParserEnum.LitJson;


        #region 反序列化

        public static object DeserializeObject(string value)
        {
            try
            {
                //if (S_AppJsonParserEnum == JsonParserEnum.NewtonJson)
                //    return JsonConvert.DeserializeObject(value);
                if (S_AppJsonParserEnum == JsonParserEnum.LitJson)
                    return LitJson.JsonMapper.ToObject(value);
                Debug.LogError("Not Support " + S_AppJsonParserEnum);
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                global::Debug.LogError($"反序列化失败 {e}");
                return null;
            }
        }


        /// <summary>/// 反序列化为指定的类型 toType/// </summary>
        public static object DeserializeObject(string value, Type toType)
        {
            try
            {
                //if (S_AppJsonParserEnum == JsonParserEnum.NewtonJson)
                //    return JsonConvert.DeserializeObject(value, toType);
                if (S_AppJsonParserEnum == JsonParserEnum.LitJson)
                    return LitJson.JsonMapper.ToObject(toType, value);
                Debug.LogError("Not Support " + S_AppJsonParserEnum);
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                Debug.LogError($"反序列化指定的类型{toType}失败 {e}");
                return null;
            }
        }

        public static T DeserializeObject<T>(string value)
        {
            try
            {
                //if (S_AppJsonParserEnum == JsonParserEnum.NewtonJson)
                //    return JsonConvert.DeserializeObject<T>(value);
                if (S_AppJsonParserEnum == JsonParserEnum.LitJson)
                    return LitJson.JsonMapper.ToObject<T>(value);
                Debug.LogError("Not Support " + S_AppJsonParserEnum);
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                Debug.LogError($"反序列化指定的类型{typeof(T)}失败 {e}");
                return default;
            }
        }

        #endregion

        #region 序列化

        /// <summary>/// 序列化/// </summary>
        public static string SerializeObject(object value)
        {
            try
            {
                //if (S_AppJsonParserEnum == JsonParserEnum.NewtonJson)
                //    return JsonConvert.SerializeObject(value);
                if (S_AppJsonParserEnum == JsonParserEnum.LitJson)
                    return LitJson.JsonMapper.ToJson(value);
                Debug.LogError("Not Support " + S_AppJsonParserEnum);
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                Debug.LogError($"序列化失败 {e}");
                return default;
            }
        }

        #endregion
    }
}