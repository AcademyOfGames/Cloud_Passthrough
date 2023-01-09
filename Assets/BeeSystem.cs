using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeSystem : MonoBehaviour
{
    HeroBeeBehavior bee;
    public GameObject[] littleBees;
    // Start is called before the first frame update
    void Start()
    {
    }

    public IEnumerator WaitAndGoAway()
    {
        yield return new WaitForSeconds(10);
        print("bee going away");
        bee = FindObjectOfType<HeroBeeBehavior>();

        bee.SwitchStates(HeroBeeBehavior.BeeState.GoAway);
        yield return new WaitForSeconds(3);
        foreach (GameObject go in littleBees)
        {
            go.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }


}
