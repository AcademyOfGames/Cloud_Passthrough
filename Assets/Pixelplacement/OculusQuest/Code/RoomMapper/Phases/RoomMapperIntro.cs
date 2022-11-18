using System;
using UnityEngine.UI;

namespace Pixelplacement.XRTools
{
    public class RoomMapperIntro : RoomMapperPhase
    {
        private void Start()
        {
            foreach (var hand in FindObjectsOfType<UICustomInteraction>())
            {
                hand.ToggleLaser(true);
            }
        }

        //Event Handlers:
        public void HandleGo()
        {
            Next();
            foreach (var hand in FindObjectsOfType<UICustomInteraction>())
            {
                hand.ToggleLaser(false);
            }
        }
    }
}