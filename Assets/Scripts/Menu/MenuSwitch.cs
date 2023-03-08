using UnityEngine;
using UnityEngine.Events;
using Cognitive3D;


public class MenuSwitch : MonoBehaviour
{
    bool switchActive = true;
    public bool SwitchActive //Controls if it can be switched
    {
        get { return switchActive; }
        set { switchActive = value; }
    }
    bool switchOn = false;
    public bool SwitchOn // Checks if it is on or off
    {
        get { return switchOn;}
    }

    public UnityEvent onMenuSwitched;

    public void TurnSwitchOnOff(bool onOff)
    {
        switchOn = onOff;
        onMenuSwitched.Invoke();

        new Cognitive3D.CustomEvent("Fire Invoked").Send();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!switchActive) return;

        if (other.CompareTag("Player"))
        {
            TurnSwitchOnOff(!switchOn);
        }
    }
}
