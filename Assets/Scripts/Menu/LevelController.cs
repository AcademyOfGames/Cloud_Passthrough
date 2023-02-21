using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public enum Level { start, eagle, bee };
    public Level currentLevel = Level.start;

    // todo? enum for state playing, pause

    [SerializeField] List<LevelTrigger> levelTriggers;
    [SerializeField] Transform eagleStory;

    StoryParts storyParts;
    MenuSystem menu;
    Wind wind;
    Transform eagle;
    FireBehaviour bonfire;

    private void Start()
    {
        storyParts = FindObjectOfType<StoryParts>();
        bonfire = FindObjectOfType<FireBehaviour>();
        
        menu = GetComponent<MenuSystem>();
        wind = FindObjectOfType<Wind>(true);
        ChangeLevel(Level.start);
    }

    private void OnEnable()
    {
        foreach (LevelTrigger trigger in levelTriggers)
        {
            // connect onPressed with StartIntroSequence.
            trigger.onPressed.AddListener(ChangeLevel);
        }
    }

    private void OnDisable()
    {
        foreach (LevelTrigger trigger in levelTriggers)
        {
            trigger.onPressed.RemoveAllListeners();
        }
    }

    private void ChangeLevel(Level level)
    {
        // Change Level
        Debug.Log("Changing Level");
        List<Transform> toIgnore = new List<Transform>();
        switch (level)
        {
            case Level.start:
                // initial state
                StopAllCoroutines();
                Debug.Log("Initial Game State");

                eagleStory.gameObject.SetActive(false);
                wind.SetDarkSky(); // passtrough dark.
                menu.StartFire(); // only fire is active.

                currentLevel = Level.start;
                break;

            case Level.eagle:
                StopAllCoroutines();
                Debug.Log("Starting Eagle Level");
                storyParts.StartIntroSequence();
                eagleStory.gameObject.SetActive(true);

                // menuSequence() out;
                bonfire.TurnFireOnOff(false);

                // change passtrough layer.
                wind.ResetSky();

                currentLevel = Level.eagle;
                break;

            case Level.bee:
                StopAllCoroutines();
                Debug.Log("Starting Bee Level");

                // todo menuSequence() out;
 
                currentLevel = Level.bee;
                break;
        }
    }
}
