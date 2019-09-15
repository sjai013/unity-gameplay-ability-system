using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilitySystem {

    /// <summary>
    /// Animation event
    /// </summary>
    /// <typeparam name="AnimationEvent"></typeparam>
    [AddComponentMenu("Gameplay Ability System/Animation Event System")]
    [System.Serializable]
    public class CustomAnimationUnityEvent : UnityEvent<AnimationEvent> {
        
    }

    /// <summary>
    /// An Animation Event System for managing animation on a component and returning animation events
    /// </summary>
    public class AnimationEventSystem : MonoBehaviour {

        public AnimationEvent PreviousAnimationEvent;

        /// <summary>
        /// Delegate to execute when custom animation events are triggered on the character
        /// </summary>
        public CustomAnimationUnityEvent CustomAnimationEvent = new CustomAnimationUnityEvent();
        /// <summary>
        /// Delegate to execute when the player animation is complete
        /// </summary>
        public UnityEvent AnimationComplete;
        /// <summary>
        /// Delegate to execute when the player animation starts
        /// </summary>
        public UnityEvent AnimationStarted;

        /// <summary>
        /// Run when an animation event occurs
        /// </summary>
        /// <param name="eventName">Name of the animation event that occured</param>
        public void OnAnimationEvent(AnimationEvent evt) {
            CustomAnimationEvent?.Invoke(evt);
            this.PreviousAnimationEvent = evt;
        }

        /// <summary>
        /// Run when an animation completes
        /// </summary>
        public void OnAnimationComplete() {
            AnimationComplete?.Invoke();
        }

        /// <summary>
        /// Run when an animation starts
        /// </summary>
        public void OnAnimationStarted() {
            AnimationStarted?.Invoke();
        }

    }
}