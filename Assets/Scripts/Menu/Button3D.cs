using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Button3D : MonoBehaviour
{
    public UnityEvent onPressed;
    [SerializeField] float timePressing = 2.0f;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(PressingButtonSequence());
    }

    private void OnTriggerExit(Collider other)
    {
        StopCoroutine(PressingButtonSequence());
        Debug.Log("Canceling onPressed");
    }

    private IEnumerator PressingButtonSequence()
    {
        Debug.Log("Start pressing button");
        float remainingTime = timePressing;
        while(remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            Debug.Log(remainingTime);
            yield return null;
        }

        //yield return new WaitForSeconds(timePressing);
        Debug.Log("Invoking onPressed");
        onPressed.Invoke();
    }

}
