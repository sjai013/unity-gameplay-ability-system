using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamekit3D
{
    public class MenuActivityController : MonoBehaviour
    {
        Canvas[] m_Canvases = new Canvas[0];
        GraphicRaycaster[] m_Raycasters = new GraphicRaycaster[0];

        void Awake ()
        {
            m_Canvases = GetComponentsInChildren<Canvas> (true);
            m_Raycasters = GetComponentsInChildren<GraphicRaycaster> (true);
        }

        public void SetActive (bool active)
        {
            for (int i = 0; i < m_Canvases.Length; i++)
            {
                m_Canvases[i].enabled = active;
            }

            for (int i = 0; i < m_Raycasters.Length; i++)
            {
                m_Raycasters[i].enabled = active;
            }
        }
    }
}