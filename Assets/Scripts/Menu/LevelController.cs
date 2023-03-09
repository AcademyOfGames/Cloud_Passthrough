using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public enum Level {start, menu, eagle, bee , after};
    public Level currentLevel = Level.eagle;

    // todo? enum for state playing, pause

    [SerializeField] List<LevelTrigger> levelTriggers;

    [Header("Eagle")]
    [SerializeField] Transform eagleStory;
    [SerializeField] BirdMovement eagle;
    [SerializeField] Transform tree;

    [Header("Bee")]
    [SerializeField] Transform beeStory;
    [SerializeField] Transform bee;

    [SerializeField] Transform bucket;


    MenuSystem menu;
    Wind wind; // for sky
    StumpBehavior stump; // bucket
    MenuSwitch menuSwitch;

    public TutorialHand hand;
    private void Start()
    {
        menuSwitch = FindObjectOfType<MenuSwitch>();
        stump = FindObjectOfType<StumpBehavior>(true);
        menu = GetComponent<MenuSystem>();
        wind = FindObjectOfType<Wind>(true);

        if (eagle == null) eagle = FindObjectOfType<BirdMovement>();

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
        if (level == currentLevel) return;

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
                menuSwitch.onMenuSwitched.AddListener(DisplayLevelEnviroments);

                currentLevel = Level.start;
                break;

            case Level.menu:
                // Reset
                menu.DisplayMenu();
                menu.GetExitCanvas().SetExitCanvasActive(true);
                bucket.gameObject.SetActive(false);

                // Check level progression
                menu.SetBlockedEnvironments();

                currentLevel = Level.menu;

                // Eagle.
                eagle.GoToStick();
                eagle.gameObject.SetActive(true);
                // todo set eagle to face correct angle.
                eagleStory.gameObject.SetActive(false);

                // Bee
                bee.gameObject.SetActive(false);
                beeStory.gameObject.SetActive(false);
                
                break;

            case Level.eagle:
                // Menu.
                menuSwitch.TurnSwitchOnOff(false);
                menu.HideMenu();
                
                StopAllCoroutines();
                Debug.Log("Starting Eagle Level");

                // Eagle
                eagleStory.gameObject.SetActive(true);
                eagle.GoToNest();

                foreach (var item in FindObjectsOfType<BrickLogic>())
                {
                    item.ActivateBrick();
                }
                //storyParts.StartIntroSequence();

                //Sky
                IEnumerator resetSky = wind.ResetSky();
                StartCoroutine(resetSky);// change passtrough layer.
                currentLevel = Level.eagle;
                break;

            case Level.bee:
                Debug.Log("Starting Bee Level");
                // Menu.
                menuSwitch.TurnSwitchOnOff(false);
                menu.HideMenu();

                StopAllCoroutines();
                
                // Eagle
                //eagleStory.gameObject.SetActive(false);
                eagle.gameObject.SetActive(false);

                // Bee
                hand.EquipGlove();
                stump.ActivateBeeSystem();

                currentLevel = Level.bee;
                break;

            case Level.after:
                menu.HideMenu();
                currentLevel = Level.after;
                break;
        }
    }

    public void ExitApplication()
    {
        Debug.Log("Exit Application");
        Application.Quit();
    }

    private void DisplayLevelEnviroments()
    {
        ChangeLevel(Level.menu);
        menuSwitch.onMenuSwitched.RemoveListener(DisplayLevelEnviroments);
    }
}
