using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Http
{
    public static async Task<Httpdata> PostAsync(string url,object postData, int timeOut=5)
    {
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Jungle Warfare/Windows");
        client.Timeout = TimeSpan.FromSeconds(timeOut);
        try
        {
            string postDataString = JsonParser.ToJson(postData);

            StringContent content = new StringContent(postDataString, Encoding.UTF8, "application/json");

            var data = await client.PostAsync(url, content);
            string response = await data.Content.ReadAsStringAsync();
            Httpdata httpData = JsonParser.FromJson<Httpdata>(response);

            return httpData;
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public static async Task<Httpdata> GetAsync(string url, int timeOut = 5)
    {
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Jungle Warfare/Windows");
        client.Timeout = TimeSpan.FromSeconds(timeOut);
        try
        {
            string response = await client.GetStringAsync(url);
            Httpdata httpData = JsonParser.FromJson<Httpdata>(response);

            return httpData;
        }
        catch (Exception e)
        {
            throw e;
        }
    }
}
public class LoginHttp
{
    public long id;
    public string name;
    public string password;
    public string loginSessionId;

    public string versionHash;
    public LoginHttp(long id, string password)
    {
        this.id = id;
        this.versionHash = AntiCheatRules.versionHash;
        this.password = password;
    }
    public LoginHttp(string name, string password)
    {
        this.name = name;
        this.versionHash = AntiCheatRules.versionHash;
        this.password = password;
    }
    public LoginHttp()
    {
        this.versionHash = AntiCheatRules.versionHash;
    }
}