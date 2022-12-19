using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehavior : MonoBehaviour
{


  public void CrashThroughBricks()
  {
    FindObjectOfType<AudioManager>().PlaySound("bricksCrashing");
  }

  public void TreeGrowingSound()
  {
    FindObjectOfType<AudioManager>().PlaySound("treeGrowingTrimmed");

  }
}
