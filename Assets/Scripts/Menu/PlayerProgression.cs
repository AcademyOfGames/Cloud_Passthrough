using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    public LevelController.Level maxCompleted = LevelController.Level.eagle;

    /// <summary>
    /// Compares the highest level completed with the current level the player is playing.
    /// Updates the highest level completed if currentlyPlaying is higher than current progress.
    /// </summary>
    /// <param name="nameLevelCompleted"></param>
    public void UpdateProgress(LevelController.Level nameLevelCompleted)
    {
        int indexCurrent = Convert.ToInt32(nameLevelCompleted);
        int indexMaxPlayed = Convert.ToInt32(maxCompleted);

        if (indexCurrent > indexMaxPlayed)
            maxCompleted = nameLevelCompleted;
    }

    public int GetMaxCompletedIndex()
    {
        // -2 is the offset. This function does not consider the start and menu as levels.
        return Convert.ToInt32(maxCompleted) - 2;
    }

    // todo save this settings
}
