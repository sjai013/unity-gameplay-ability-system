using System.Collections;
using UnityEngine;

namespace Gamekit3D.WorldBuilding
{

    [ExecuteInEditMode]
    public class InstancePainter : MonoBehaviour
    {
        public enum CollisionTest
        {
            RendererBounds,
            ColliderBounds
        }

        public LayerMask layerMask;
        public Transform rootTransform;
        public float brushRadius = 5;
        public float brushHeight = 10;
        public float brushDensity = 0.25f;
        [Range(0, 360)]
        public float maxRandomRotation = 360f;
        [Range(0, 360)]
        public float rotationStep = 90f;
        public CollisionTest collisionTest;
        [Range(0, 1)]
        public float maxIntersectionVolume = 0;
        [Range(0, 360)]
        public float maxSlope = 45;

        [HideInInspector] public bool randomizeAfterStamp = true;
        [HideInInspector] public bool alignToNormal = true;
        [HideInInspector] public bool followOnSurface = true;
        [HideInInspector] public int selectedPrefabIndex = 0;

        public GameObject[] prefabPallete;

        public GameObject SelectedPrefab
        {
            get
            {
                return prefabPallete == null || prefabPallete.Length == 0 ? null : prefabPallete[selectedPrefabIndex];
            }
        }

        [ContextMenu("Delete Children")]
        void DeleteChildren()
        {
            while (transform.childCount > 0) DestroyImmediate(transform.GetChild(0).gameObject);
        }


    }
}