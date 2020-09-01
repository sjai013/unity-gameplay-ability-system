using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamekit3D
{
    public class UIAutoscroll : MonoBehaviour
    {

        public ScrollRect scrollRect;
        public Scrollbar scrollbar;
        public float scrollValue;
        public float duration = 30.0f;

        void OnEnable()
        {
            StartCoroutine(Scroller());
        }

        IEnumerator Scroller()
        {
            var t = 0.0f;
            while (true)
            {
                t += Time.deltaTime / duration;
                scrollRect.verticalNormalizedPosition = 1 - Mathf.PingPong(t, 1);
                yield return null;
            }
        }
    } 
}
