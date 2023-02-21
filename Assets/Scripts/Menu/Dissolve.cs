using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    // Requires the object to contain a material with Dissolve Shader.
    Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    public IEnumerator FadeOut(float speed = 1f)
    {
        // 1 is invisible // 0 is visible
        foreach(Material m in rend.materials)
        {
            if (!m.HasProperty("_Dissolve")) yield break;

            float newVal = 0.001f;
            while (newVal < 1)
            {
                SetDissolve(m, newVal);
                newVal += Time.deltaTime * speed;
                yield return null;
            }
        }
    }

    public IEnumerator FadeIn(float speed = 1f)
    {
        Debug.Log("Fade In" + transform.name);      
        foreach (Material m in rend.materials)
        {
            if (!m.HasProperty("_Dissolve")) break;

            float newVal = 1f;
            while (newVal > 0)
            {
                SetDissolve(m, newVal);
                newVal -= Time.deltaTime * speed;
                yield return null;
            }
        }
        yield return null;
    }

    //Helper function.
    private void SetDissolve(Material mat, float visibility)
    {
        if (!mat.HasProperty("_Dissolve")) return;
        mat.SetFloat("_Dissolve",visibility);
    }

    public void SetInvisible()
    {
        foreach (Material m in rend.materials)
        {
            if (!m.HasProperty("_Dissolve")) break;
            SetDissolve(m, 1f);
        }
    }
}
