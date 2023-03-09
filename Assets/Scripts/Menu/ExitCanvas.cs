using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitCanvas : MonoBehaviour
{
    [SerializeField] Transform folder;
    public bool isOpen = true;
    public bool isActive = true;

    [Header("Exit Canvas")]
    [SerializeField] Transform exitCanvas;
    [SerializeField] CustomButton exitYesButton;
    [SerializeField] CustomButton exitNoButton;
    [SerializeField] CustomButton backToMenuButton;
    //List<OVRGrabbableExtended> grabbables = new List<OVRGrabbableExtended>();

    //Dictionary<CustomButton, Vector3> positions = new Dictionary<CustomButton, Vector3>();

    LevelController levelController;
    MenuSwitch menuSwitch;

    private void Awake()
    {
        levelController = FindObjectOfType<LevelController>();
        menuSwitch = FindObjectOfType<MenuSwitch>();
    }

    private void Start()
    {
        // todo add grabbables to dictionary to reset positions when user is not grabbing a log.

    }

    private void OnEnable()
    {
        /*
        exitYesButton.OnGrabBegin.AddListener(levelController.ExitApplication);
        exitNoButton.OnGrabBegin.AddListener(CloseExitCanvas);
        backToMenuButton.OnGrabBegin.AddListener(BackToMainMenu);
        */
        menuSwitch.onMenuSwitched.AddListener(() => OpenExitCanvas(menuSwitch.SwitchOn));

        exitYesButton.onTriggered.AddListener(levelController.ExitApplication);
        exitNoButton.onTriggered.AddListener(() => OpenExitCanvas(false));
        backToMenuButton.onTriggered.AddListener(BackToMainMenu);
    }

    private void OnDisable()
    {
        menuSwitch.onMenuSwitched.RemoveAllListeners();

        exitYesButton.onTriggered.RemoveAllListeners();
        exitNoButton.onTriggered.RemoveAllListeners();
        backToMenuButton.onTriggered.RemoveAllListeners();
    }

    private void BackToMainMenu()
    {
        Debug.Log("Back to Main Menu");
        OpenExitCanvas(false);
        levelController.ChangeLevel(LevelController.Level.menu);
        menuSwitch.TurnSwitchOnOff(false);
    }

    public void OpenExitCanvas(bool open)
    {
        Debug.Log("Trying to open or close menu");
        if (!isActive) return;
        exitCanvas.gameObject.SetActive(open);
        /*
        if (open)
        {
            menuSwitch.SwitchActive = false;
            return;
        }
        menuSwitch.SwitchActive = true;*/
        isOpen = open;
    }

    public void SetExitCanvasActive(bool active)
    {
        isActive = active;
    }

    private void ResetCanvas()
    {
        // todo Reset Positions? Or Instantiate a new Exit Canvas?
    }
}
