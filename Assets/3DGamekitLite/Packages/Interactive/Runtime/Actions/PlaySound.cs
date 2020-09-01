using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gamekit3D.GameCommands
{
    public class PlaySound : GameCommandHandler
    {
        public AudioSource[] audioSources;

        public override void PerformInteraction()
        {
            foreach (var a in audioSources)
                a.Play();
        }

    }
}
