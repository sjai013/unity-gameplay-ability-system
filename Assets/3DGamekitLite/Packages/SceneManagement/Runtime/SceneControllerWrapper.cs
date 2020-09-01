using System;
using UnityEngine;

namespace Gamekit3D
{
    public class SceneControllerWrapper : MonoBehaviour
    {
        public void RestartZone(bool resetHealth)
        {
            SceneController.RestartZone(resetHealth);
        }

        public void TransitionToScene(TransitionPoint transitionPoint)
        {
            SceneController.TransitionToScene(transitionPoint);
        }

        public void RestartZoneWithDelay(float delay)
        {
            SceneController.RestartZoneWithDelay(delay, false);
        }

        public void RestartZoneWithDelayAndHealthReset(float delay)
        {
            SceneController.RestartZoneWithDelay(delay, true);
        }
    }
}