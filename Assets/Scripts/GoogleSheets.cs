using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleSheets : MonoBehaviour
{
    string BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSc9T5FuV5aLl-1RUsM9mVFmUBMWojrGIoYivfs3SIQQmvMq8Q/formResponse";
    public string userName = "Guest";
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
   IEnumerator Post(string data) {
       WWWForm form = new WWWForm();
       form.AddField("entry.781808147", data);
       form.AddField("entry.593286627", userName);
   
       byte[] rawData = form.data;
       string url = BASE_URL;
   
       // Post a request to an URL with our custom headers
       WWW www = new WWW(url, rawData);
       yield return www;
   }
   
   public void AddEventData(string data) {
       StartCoroutine(Post(data));
   }
}
