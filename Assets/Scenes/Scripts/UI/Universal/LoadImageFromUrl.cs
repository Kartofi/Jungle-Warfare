using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadImageFromUrl : MonoBehaviour
{
    //Image Cache
    public static Dictionary<long, Texture2D> imageStorage = new Dictionary<long, Texture2D>();

    public string url;

    public RawImage image;

    public Texture defaultImage;


    public void UpdateImage(string url)
    {
        this.url = url;
        StartCoroutine(DownloadAndSetImage(url));
    }
    public static void RemoveImageFromStorage(long id)
    {
        if (imageStorage.ContainsKey(id)) 
        {
            imageStorage.Remove(id);
        }
    }
    public void UpdateImageUsingId(long id = -1)
    {
        if (image == null)
        {
            return;
        }
        if (id == -1)
        {
            if (defaultImage != null)
            {
                image.texture = defaultImage;
            }else
            {
                image.texture = null;
            }
            
            return;
        }
        if (imageStorage.ContainsKey(id))
        {
           image.texture = imageStorage.GetValueOrDefault(id);
           if (image.texture == null && defaultImage != null)
           {
               image.texture = defaultImage;
           }
        }
        else
        {
            this.url = NetworkingCredentials.rootUrl + "images/users/" + id;
            if (gameObject.activeSelf == true)
            {
                StartCoroutine(DownloadAndSetImage(url, id));
            }
            
        }
    }
    public static void DownloadProfileImageStart(long id = -1)
    {
        DownloadImage(NetworkingCredentials.rootUrl + "images/users/", id);
    }
    static IEnumerator DownloadImage(string MediaUrl, long id = -1)
    { 
        
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success && request.result != UnityWebRequest.Result.InProgress)
        {
            Debug.Log(request.error);
            Debug.Log(MediaUrl);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            imageStorage.Add(id, texture);
        }
    }
    IEnumerator DownloadAndSetImage(string MediaUrl, long id = -1)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success && request.result != UnityWebRequest.Result.InProgress)
        {
            Debug.Log(request.error);
            Debug.Log(MediaUrl);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            image.texture = texture;
            if (!imageStorage.ContainsKey(id))
            {
                imageStorage.Add(id, texture);
            }
        }
            
    }
}
