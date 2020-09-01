using UnityEngine;


namespace Gamekit3D.GameCommands
{
    public class SwitchMaterial : GameCommandHandler
    {
        public Renderer target;
        public Material[] materials;
        int count;

        public override void PerformInteraction()
        {
            count++;
            target.material = materials[count % materials.Length];
        }
    }
}
