using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    LevelController.Level maxCompleted = LevelController.Level.start;

    void CheckMaxCompleted(LevelController.Level currentlyPlaying)
    {
        int indexCurrent = Convert.ToInt32(currentlyPlaying);
        int indexMaxPlayed = Convert.ToInt32(maxCompleted);

        if (indexCurrent > indexMaxPlayed)
            maxCompleted = currentlyPlaying;
    }

    // todo save this settings
}
