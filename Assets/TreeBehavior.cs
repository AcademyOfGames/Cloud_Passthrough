using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehavior : MonoBehaviour
{


  public void CrashThroughBricks()
  {
    GetComponent<AudioManager>().PlaySound("bricksCrashing");
  }
}
