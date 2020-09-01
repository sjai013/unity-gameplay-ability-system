using UnityEngine;

namespace Gamekit3D.WorldBuilding
{
    [System.Serializable]
    public class SmartParts
    {
        public Transform topLeft;
        public Transform top;
        public Transform topRight;
        public Transform left;
        public Transform center;
        public Transform right;
        public Transform bottomLeft;
        public Transform bottom;
        public Transform bottomRight;
        public Transform[] alternateCenters = new Transform[0];
        public void SetActive(bool active)
        {
            topLeft.gameObject.SetActive(active);
            top.gameObject.SetActive(active);
            topRight.gameObject.SetActive(active);
            left.gameObject.SetActive(active);
            center.gameObject.SetActive(active);
            right.gameObject.SetActive(active);
            bottomLeft.gameObject.SetActive(active);
            bottom.gameObject.SetActive(active);
            bottomRight.gameObject.SetActive(active);
            foreach (var i in alternateCenters)
                i.gameObject.SetActive(active);
        }
    }

    [ExecuteInEditMode]
    public class MegaSmartObject : MonoBehaviour
    {
        public Bounds originBounds;
        public Bounds bounds;
        public SmartParts parts;
        public Transform generated;

        void Reset()
        {
            parts = new SmartParts();
            parts.topLeft = transform.Find("TL");
            parts.top = transform.Find("T");
            parts.topRight = transform.Find("TR");
            parts.left = transform.Find("L");
            parts.center = transform.Find("C");
            parts.right = transform.Find("R");
            parts.bottomLeft = transform.Find("BL");
            parts.bottom = transform.Find("B");
            parts.bottomRight = transform.Find("BR");
            if (parts.center != null)
            {
                parts.alternateCenters = new[] { parts.center };
            }
            generated = transform.Find("Generated");
            if (generated == null)
            {
                generated = new GameObject("Generated").transform;
                generated.parent = transform;
                generated.localPosition = Vector3.zero;
                generated.rotation = Quaternion.identity;
            }
            var rb = parts.center.gameObject.GetLocalBounds();
            bounds = new Bounds(rb.center, rb.size);
            bounds.center += parts.center.localPosition;
            originBounds = bounds;
        }

    }
}
