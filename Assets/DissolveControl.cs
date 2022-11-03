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

    public GameObject groundCollider1;
    public GameObject groundCollider2;
    public GameObject brickWalls;

    public GameObject deer;
    public ParticleSystem mist;


    private float completeDissolve = 250f;
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
            transition += Time.deltaTime * .24f;
            newDissolve = Mathf.Lerp(0, completeDissolve,transition);
            m.SetFloat("dissolve", newDissolve);
            yield return null;
        }
        deer.SetActive(false);
        groundCollider1.SetActive(false);
        brickWalls.SetActive(false);
        groundCollider2.SetActive(false);
        mist.gravityModifier = .5f;

        Invoke("hidePassthroughObjects", 1);
        SceneManager.LoadSceneAsync("GrassyHlls",LoadSceneMode.Additive);
        yield return new WaitForSeconds(.1f);
        transition = 0;
        float transparency = m.GetFloat("transparency");
        while (transition <= 1)
        {
            transition += Time.deltaTime * .01f;
            transparency = Mathf.Lerp(transparency, 0,transition);
            m.SetFloat("transparency", transparency);
            yield return null;
        }


    }

    void hidePassthroughObjects()
    {
        mist.gameObject.SetActive(false);
        foreach(GameObject b in GameObject.FindGameObjectsWithTag("Brick"))
        {
            b.SetActive(false);
        }
    }


    void Update(){
    }
}

