using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DissolveControl : MonoBehaviour
{
    public Transform startPos;
    private Transform objPos;
    private Material m;

    private float completeDissolve = 2000f;
    void Start()
    {
        m = GetComponent<MeshRenderer>().material;
        m.SetVector("startPos", startPos.position);
    }
    
    public void StartDissolve()
    {
        StartCoroutine("DissolveOverTime");
    }

    IEnumerator DissolveOverTime()
    {
        float transition = 0f;
        float newDissolve = 0;
        
        while (transition <= 1)
        {
            transition += Time.deltaTime * .08f;
            newDissolve = Mathf.Lerp(0, completeDissolve,transition);
            m.SetFloat("dissolve", newDissolve);
            yield return null;
        }
        SceneManager.LoadSceneAsync("GrassyHlls",LoadSceneMode.Additive);
        yield return new WaitForSeconds(1f);
        transition = 0;
        float transparency = m.GetFloat("transparency");
        while (transition <= 1)
        {
            transition += Time.deltaTime * .005f;
            transparency = Mathf.Lerp(transparency, 0,transition);
            m.SetFloat("transparency", transparency);
            yield return null;
        }


    }
 
    void Update(){
    }
}

