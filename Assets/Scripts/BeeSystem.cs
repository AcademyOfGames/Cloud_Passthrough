using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeSystem : MonoBehaviour
{
    HeroBeeBehavior bee;
    public GameObject[] littleBees;
    public FlowerBehavior[] flowers;
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

    public void ResetFlowers()
    {
        foreach(FlowerBehavior flower in flowers)
        {
            if (flower == this) continue;
            flower.gameObject.SetActive(false);
            flower.DeactivateSecondFlowers();
        }
    }

}
