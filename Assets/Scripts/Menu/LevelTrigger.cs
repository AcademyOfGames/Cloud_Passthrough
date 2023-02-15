using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class LevelEvent : UnityEvent<MenuSystem.Level>{}

public class LevelTrigger : MonoBehaviour
{
    public LevelEvent onPressed;
    [SerializeField] float timePressing = 2.0f;
    [SerializeField] MenuSystem.Level levelToLoad = MenuSystem.Level.eagle;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
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
        onPressed.Invoke(levelToLoad);
    }

}
