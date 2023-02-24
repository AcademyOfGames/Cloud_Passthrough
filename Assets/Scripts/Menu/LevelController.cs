using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public enum Level {start, menu, eagle, bee , after};
    public Level currentLevel = Level.start;

    // todo? enum for state playing, pause

    [SerializeField] List<LevelTrigger> levelTriggers;

    [Header("Eagle")]
    [SerializeField] Transform eagleStory;
    [SerializeField] Transform eagle;

    [Header("Bee")]
    [SerializeField] Transform beeStory;
    [SerializeField] Transform bee;

    StoryParts storyParts;
    MenuSystem menu;
    Wind wind;
    

    private void Start()
    {
        
        storyParts = FindObjectOfType<StoryParts>();
        menu = GetComponent<MenuSystem>();
        wind = FindObjectOfType<Wind>(true);

        if (eagle == null) eagle = FindObjectOfType<BirdMovement>().transform;

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

    public void ChangeLevel(Level level)
    {
        // Change Level
        Debug.Log("Changing Level");
        //List<Transform> toIgnore = new List<Transform>();
        switch (level)
        {
            case Level.start:
                // initial state
                StopAllCoroutines();
                Debug.Log("Initial Game State");

                //Eagle settings.
                eagle.gameObject.SetActive(false);
                eagleStory.gameObject.SetActive(false);

                // Sky 
                wind.SetDarkSky(); // passtrough dark.

                // Menu
                menu.SetMenuAtStart(); // configuration set by MenuSwitch
                menu.SetSwitchActive(true);
                currentLevel = Level.start;
                break;

            case Level.menu:
                // todo Check Level progression to decide how many levels to display
                
                currentLevel = Level.menu;
                // Eagle.
                eagle.gameObject.SetActive(true);
                // todo set eagle to land to stick when going back to menu
                eagleStory.gameObject.SetActive(false);
                // Menu.
                menu.SetSwitchActive(false);
                break;

            case Level.eagle:
                
                StopAllCoroutines();
                Debug.Log("Starting Eagle Level");
                // Eagle
                storyParts.StartIntroSequence();
                eagleStory.gameObject.SetActive(true);

                // Menu
                menu.SetSwitchActive(true);
                menu.SetSwitchOnOff(false);

                //Sky
                wind.ResetSky(); // change passtrough layer.
                currentLevel = Level.eagle;
                break;

            case Level.bee:
                
                StopAllCoroutines();
                Debug.Log("Starting Bee Level");

                // Eagle
                eagleStory.gameObject.SetActive(false);
                eagle.gameObject.SetActive(false);

                // Bee
                beeStory.gameObject.SetActive(true);

                // Menu
                menu.SetSwitchActive(true);
                menu.SetSwitchOnOff(false);
                currentLevel = Level.bee;
                break;

            case Level.after:
                currentLevel = Level.after;
                break;
        }
    }

    public void ExitApplication()
    {
        Debug.Log("Exit Application");
        Application.Quit();
    }
}
