namespace Gamekit3D.GameCommands
{
    public class RespawnPlayer : GameCommandHandler
    {
        public Gamekit3D.PlayerController player;

        public override void PerformInteraction()
        {
            player.Respawn();
        }
    }
}
