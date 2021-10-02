using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{

    [SerializeField] private float m_TimeToDie;

    private float m_TimeElapsed;

    // Update is called once per frame
    void Update()
    {
        m_TimeElapsed += Time.deltaTime;
        if (m_TimeElapsed >= m_TimeToDie) {
            Destroy(this.gameObject);
        }
    }
}
