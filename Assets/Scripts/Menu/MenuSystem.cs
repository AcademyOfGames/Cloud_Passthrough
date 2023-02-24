using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*[System.Serializable]
public class MenuLevelComponents
{
    public string levelName;
    public Transform storyPartParent;
    public Transform triggerLoader;
    public Transform animal;
    public Transform area;
}*/


public class MenuSystem : MonoBehaviour
{
    [Header("Menu Components")]
    [SerializeField] List<Transform> menuObjects = new List<Transform>();
    [SerializeField] Transform parentObjects;
    [SerializeField] Transform triggerParent;

    [Header("Switch")]
    [SerializeField] MenuSwitch menuSwitch;

    FireBehaviour bonfire;
    LevelController levelController;

    ExitCanvas exitCanvas;

    //[SerializeField] List<MenuLevelComponents> levelsComponents = new List<MenuLevelComponents>();
    //int maxLevelIndex = 0;

    private void Awake()
    {
        bonfire = menuSwitch.GetComponent<FireBehaviour>();
        levelController = FindObjectOfType<LevelController>();
        exitCanvas = GetComponentInChildren<ExitCanvas>();
    }

    private void OnEnable()
    {
        menuSwitch.onMenuSwitched.AddListener(DisplayMenu);
    }

    private void OnDisable()
    {
        menuSwitch.onMenuSwitched.RemoveAllListeners();
    }

    private void DisplayMenu()
    {
        /*if(levelController.currentLevel != LevelController.Level.menu ||
            levelController.currentLevel != LevelController.Level.start)
        {
            // Toggle ExitCanvas
            exitCanvas.OpenExitCanvas(menuSwitch.SwitchOn);
            bonfire.TurnFireOnOff(menuSwitch.SwitchOn);
            return;
        }*/
        if(levelController.currentLevel == LevelController.Level.eagle || levelController.currentLevel == LevelController.Level.bee)
        {
            exitCanvas.OpenExitCanvas(menuSwitch.SwitchOn);
        }
        else
        {
            StartCoroutine(MenuSequence(menuSwitch.SwitchOn, 0.05f, null));
        }

        bonfire.TurnFireOnOff(menuSwitch.SwitchOn);

        if (levelController.currentLevel == LevelController.Level.start)
            levelController.ChangeLevel(LevelController.Level.menu);
    }

    /// <summary>
    /// Sequence to hide or display Menu Game Objects.
    /// </summary>
    /// <param name="show"></param>
    /// <returns></returns>
    private IEnumerator MenuSequence(bool show, float delay, List<Transform> exceptions)
    {
        yield return new WaitForSeconds(delay);
        if(show)
            SetAllInvisible();
        foreach (Transform t in menuObjects)
        {
            if (IsInList(t, exceptions)) continue;
            t.gameObject.SetActive(true);
            Dissolve[] dissolves = t.GetComponentsInChildren<Dissolve>(true);
            foreach(Dissolve d in dissolves)
            {
                if (show)
                {
                    //Debug.Log("Fade In" + d.transform.name);
                    StartCoroutine(d.FadeIn(0.5f));
                    //if (!isEagleOn) eagle.gameObject.SetActive(true); // only turn on eagle the first time the menu displays.
                }

                else
                {
                    //Debug.Log("Fade Out" + d.transform.name);
                    StartCoroutine(d.FadeOut(0.2f));
                }
            }
            yield return new WaitForFixedUpdate();
        }
        triggerParent.gameObject.SetActive(show);
        yield return new WaitForSeconds(2f);
        parentObjects.gameObject.SetActive(show);
        
    }

    public void SetMenuAtStart()
    {
        //bonfire.gameObject.SetActive();
        //menuSwitch.TurnSwitchOnOff(false);
        bonfire.TurnFireOnOff(menuSwitch.SwitchOn);
        triggerParent.gameObject.SetActive(menuSwitch.SwitchOn);
        ShowAllMenuObjects(menuSwitch.SwitchOn);
        exitCanvas.OpenExitCanvas(false);
    }

    /// <summary>
    /// Sets active or inactive all GameObjects in menuObjects list.
    /// </summary>
    /// <param name="display"></param>
    private void ShowAllMenuObjects(bool display)
    {
        Debug.Log("Set all inactive");
        //parentObjects.gameObject.SetActive(display);
        foreach(Transform t in menuObjects)
        {
            t.gameObject.SetActive(display);
        }
    }

    private void SetAllInvisible()
    {
        // Helper function.
        foreach (Transform t in menuObjects)
        {
            foreach (Dissolve d in t.GetComponentsInChildren<Dissolve>())
            {
                d.SetInvisible();
            }
        }
    }

    private bool IsInList(Transform transform, List<Transform> list)
    {
        if (list == null) return false;
        foreach(Transform t in list)
        {
            if (t == transform)
            {
                return true;
            }
                
        }
        return false;
    }

    /// <summary>
    /// Turns the Menu On or Off. The bonfire stays active.
    /// </summary>
    /// <param name="onOff"></param>
    public void SetSwitchOnOff(bool onOff)
    {
        menuSwitch.TurnSwitchOnOff(onOff);
    }

    /// <summary>
    /// Sets the switch active or inactive
    /// </summary>
    /// <param name="isActive">If false, the menu cannot toggle.</param>
    public void SetSwitchActive(bool isActive)
    {
        menuSwitch.SwitchActive = isActive;
    }

    private void LoadUnblockedLevels()
    {
        // todo only load the levels the player has unblocked.

    }

}
