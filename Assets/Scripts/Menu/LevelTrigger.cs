using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class LevelEvent : UnityEvent<LevelController.Level>{}

public class LevelTrigger : MonoBehaviour
{
    public LevelEvent onPressed;
    //[SerializeField] float timePressing = 2.0f;
    [SerializeField] LevelController.Level levelToLoad = LevelController.Level.eagle;

    private void OnTriggerEnter(Collider other)
    {
        print("trigger entered " + name + " and " + other.name);
        if(other.CompareTag("Player"))
            onPressed.Invoke(levelToLoad);
        //StartCoroutine(PressingButtonSequence());
    }

    /*private void OnTriggerExit(Collider other)
    {
        StopCoroutine(PressingButtonSequence());
        Debug.Log("Canceling onPressed");
    }*/

    /*private IEnumerator PressingButtonSequence()
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
        
    }*/

}
