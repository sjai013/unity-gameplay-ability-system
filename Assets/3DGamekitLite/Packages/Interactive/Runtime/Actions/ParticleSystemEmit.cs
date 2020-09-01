using UnityEngine;


namespace Gamekit3D.GameCommands
{
    public class ParticleSystemEmit : GameCommandHandler
    {
        public ParticleSystem[] particleSystems;
        public int count;

        public override void PerformInteraction()
        {
            foreach (var ps in particleSystems)
            {
                ps.Emit(count);
            }
        }
    }
}
