using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.UnityConverters;
using Newtonsoft.Json.UnityConverters.Math;
using System;
using Newtonsoft.Json.Converters;
using System.Text;

public static class JsonParser
{
    public static string ToJson(object data)
    {

        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.NullValueHandling = NullValueHandling.Ignore;
      
        try
        {
            var myJson = JsonConvert.SerializeObject(data, settings);

            return myJson;
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            return null;
        }
           
    }
    public static byte[] ToJsonByteArray(object data)
    {
        return Encoding.UTF8.GetBytes(ToJson(data));
    }
    public static T FromJson<T>(string json)
    {
        
        try 
        {
            T deserialized = JsonUtility.FromJson<T>(json);
            return deserialized;
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            return default(T);
        }
        
        
    }

    public static T FromCompressedJson<T>(string json)
    {
        
        string unCompressed = GZipManager.Decompress(json);
        T data = FromJson<T>(unCompressed);
        return data;
    }

    public static string ToCompressedJson(object data)
    {
        string json = ToJson(data);
        string compressed = GZipManager.Compress(json);
        return compressed;
    }
    public static byte[] ToCompressedJsonByteArray(object data)
    {
        return Convert.FromBase64String(ToCompressedJson(data));
    }
}
