using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MenuLevelEnvironments
{
    public MenuLevelEnvironments()
    {
        blocked = true;
    }

    public string levelName;
    public Transform area;
    public bool blocked = true;
}

public class MenuSystem : MonoBehaviour
{
    [Header("Menu Components")]
    [SerializeField] Transform parentObjects;
    [SerializeField] Transform triggerParent;

    [SerializeField] List<MenuLevelEnvironments> levelEnvironments = new List<MenuLevelEnvironments>();

    [Header("Switch")]
    [SerializeField] MenuSwitch menuSwitch;

    FireBehaviour bonfire;
    PlayerProgression playerProgress;

    ExitCanvas exitCanvas;

    bool isDisplayed = false;

    private void Awake()
    {
        bonfire = FindObjectOfType<FireBehaviour>();
        exitCanvas = GetComponentInChildren<ExitCanvas>();
        playerProgress = GetComponent<PlayerProgression>();
    }

    public void DisplayMenu()
    {
        if (isDisplayed) return;
        StartCoroutine(MenuSequence(menuSwitch.SwitchOn, 0.05f, null));
        bonfire.TurnFireOnOff(menuSwitch.SwitchOn);
        isDisplayed = true;
    }

    public void HideMenu()
    {
        StartCoroutine(MenuSequence(false, 0.05f, null));
        bonfire.TurnFireOnOff(menuSwitch.SwitchOn);
        isDisplayed = false;
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
        // Display Environments.
        foreach (MenuLevelEnvironments environment in levelEnvironments)
        {
            if (environment.blocked) continue;
            foreach (Transform t in environment.area)
            {
                if (IsInList(t, exceptions)) continue;
                //t.gameObject.SetActive(true);
                Dissolve[] dissolves = t.GetComponentsInChildren<Dissolve>();
                foreach (Dissolve d in dissolves)
                {
                    if (d == null) continue;
                    if (show)
                    {
                        StartCoroutine(d.FadeIn(0.5f));
                    }

                    else
                    {
                        //Debug.Log("Fade Out" + d.transform.name);
                        StartCoroutine(d.FadeOut(0.2f));
                    }
                }
                yield return new WaitForSeconds(0.05f);
            }
        }
        triggerParent.gameObject.SetActive(show);

        yield return new WaitForSeconds(5f);
        ShowAllMenuObjects(show);
    }

    public void SetMenuAtStart()
    {
        //bonfire.gameObject.SetActive();
        //menuSwitch.TurnSwitchOnOff(false);
        bonfire.TurnFireOnOff(menuSwitch.SwitchOn);
        triggerParent.gameObject.SetActive(menuSwitch.SwitchOn);
        ShowAllMenuObjects(menuSwitch.SwitchOn);
        exitCanvas.OpenExitCanvas(false);
        exitCanvas.SetExitCanvasActive(false);
    }

    /// <summary>
    /// Sets active or inactive all GameObjects in menuObjects list.
    /// </summary>
    /// <param name="display"></param>
    private void ShowAllMenuObjects(bool display)
    {
        foreach (MenuLevelEnvironments env in levelEnvironments)
        {
            if (env.blocked) continue;
            env.area.gameObject.SetActive(display);
        }
    }

    private void SetAllInvisible()
    {        
        foreach(MenuLevelEnvironments env in levelEnvironments)
        {
            if (env.blocked) continue;
            foreach(Transform t in env.area)
            {
                foreach (Dissolve d in t.GetComponentsInChildren<Dissolve>(true))
                {
                    if (d == null) continue;
                    d.SetInvisible();
                }
            }
            env.area.gameObject.SetActive(true);
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

    public void SetBlockedEnvironments()
    {
        // only load the levels the player has unblocked.
        // Check Level progression to decide how many levels to display
        int maxIndex = playerProgress.GetMaxCompletedIndex();
        for (int i = 0; i < levelEnvironments.Count; i++)
        {
            // Ex. Max completed is eagle. maxIndex = 0, unblock all up to next level -> maxIndex + 1
            if (i <= maxIndex + 1)
            {
                levelEnvironments[i].blocked = false;
                continue;
            }

            levelEnvironments[i].area.gameObject.SetActive(false);
        }
    }

    public ExitCanvas GetExitCanvas()
    {
        return exitCanvas;
    }

}
