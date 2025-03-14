using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

public static class GZipManager
{
    public static string Decompress(string input)
    {
        try
        {
            byte[] inputBytes = Convert.FromBase64String(input);
            byte[] decompressed = Decompress(inputBytes);
            string output = Encoding.UTF8.GetString(decompressed);
            return output;
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            return null;
        }
       
    }

    public static string Compress(string input)
    {
        byte[] encoded = Encoding.UTF8.GetBytes(input);
        byte[] compressed = Compress(encoded);
        return Convert.ToBase64String(compressed);
    }

    static byte[] Compress(byte[] data)
    {
        using (MemoryStream output = new MemoryStream())
        {
            using (GZipStream gzip = new GZipStream(output, CompressionMode.Compress))
            {
                gzip.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }
    }

    public static byte[] Decompress(byte[] bytes)
    {
        using (var memoryStream = new MemoryStream(bytes))
        {

            using (var outputStream = new MemoryStream())
            {
                using (var decompressStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                  decompressStream.CopyTo(outputStream);
                }
                return outputStream.ToArray();
            }
        }
    }
}
