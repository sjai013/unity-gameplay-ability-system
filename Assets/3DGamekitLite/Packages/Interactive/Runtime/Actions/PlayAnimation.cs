using UnityEngine;


namespace Gamekit3D.GameCommands
{
    public class PlayAnimation : GameCommandHandler
    {
        public Animation[] animations;

        public override void PerformInteraction()
        {
            foreach (var a in animations)
                a.Play();
        }
    }
}
