using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleSheets : MonoBehaviour
{
    
    string BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSdHkniLGcPDOwky78XShFmCSTerlW_yZrQsb_En0tyVrBGj0w/formResponse";
    public string userName = "Guest";

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

   IEnumerator Post(string data) {
       
       print("Sending data " + data);
       WWWForm form = new WWWForm();
       form.AddField("entry.1758749926", data);
       form.AddField("entry.1173467535", userName);
   
       byte[] rawData = form.data;
       string url = BASE_URL;
   
       // Post a request to an URL with our custom headers
       WWW www = new WWW(url, rawData);
       yield return www;
        print(www);
   }
   
   public void AddEventData(string data, string user)
   {
       userName = user;
       StartCoroutine(Post(data));
   }
}
