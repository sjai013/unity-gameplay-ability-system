using UnityEngine.Events;
using UnityEngine.Playables;

namespace Gamekit3D.GameCommands
{
    public class StartPlayableDirector : GameCommandHandler
    {
        public PlayableDirector director;
        public UnityEvent OnDirectorPlay;
        public UnityEvent OnDirectorFinish;

        void Reset()
        {
            director = GetComponent<PlayableDirector>();
        }

        public override void PerformInteraction()
        {
            OnDirectorPlay.Invoke ();
            
            if (director)
                director.Play();
            
            Invoke ("FinishInvoke", (float)director.duration);
        }

        void FinishInvoke ()
        {
            OnDirectorFinish.Invoke ();
        }
    }
}
