using Cinemachine;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Gamekit3D
{
    [RequireComponent(typeof(Collider))]
    public class TransitionPoint : MonoBehaviour
    {
        public enum TransitionType
        {
            DifferentZone, DifferentNonGameplayScene, SameScene,
        }


        public enum TransitionWhen
        {
            ExternalCall, OnTriggerEnter,
        }


        [Tooltip("This is the gameobject that will transition.  For example, the player.")]
        public GameObject transitioningGameObject;
        [Tooltip("Whether the transition will be within this scene, to a different zone or a non-gameplay scene.")]
        public TransitionType transitionType;
        [SceneName]
        public string newSceneName;
        [Tooltip("The tag of the SceneTransitionDestination script in the scene being transitioned to.")]
        public SceneTransitionDestination.DestinationTag transitionDestinationTag;
        [Tooltip("The destination in this scene that the transitioning gameobject will be teleported.")]
        public TransitionPoint destinationTransform;
        [Tooltip("What should trigger the transition to start.")]
        public TransitionWhen transitionWhen;
        [Tooltip("Is this transition only possible with specific items in the inventory?")]
        public bool requiresInventoryCheck;
        [Tooltip("The inventory to be checked.")]
        public Gamekit3D.InventoryController inventoryController;
        [Tooltip("The required items.")]
        public Gamekit3D.InventoryController.InventoryChecker inventoryCheck;

        bool m_TransitioningGameObjectPresent;

        void Start()
        {
            if (transitionWhen == TransitionWhen.ExternalCall)
                m_TransitioningGameObjectPresent = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == transitioningGameObject)
            {
                m_TransitioningGameObjectPresent = true;

                if (ScreenFader.IsFading || SceneController.Transitioning)
                    return;

                if (transitionWhen == TransitionWhen.OnTriggerEnter)
                    TransitionInternal();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject == transitioningGameObject)
            {
                m_TransitioningGameObjectPresent = false;
            }
        }

        protected void TransitionInternal()
        {
            if (requiresInventoryCheck)
            {
                if (!inventoryCheck.CheckInventory(inventoryController))
                    return;
            }

            if (transitionType == TransitionType.SameScene)
            {
                GameObjectTeleporter.Teleport(transitioningGameObject, destinationTransform.transform);
            }
            else
            {
                SceneController.TransitionToScene(this);
            }
        }

        public void Transition()
        {
            if (m_TransitioningGameObjectPresent)
                if (transitionWhen == TransitionWhen.ExternalCall)
                    TransitionInternal();
        }
    }
}