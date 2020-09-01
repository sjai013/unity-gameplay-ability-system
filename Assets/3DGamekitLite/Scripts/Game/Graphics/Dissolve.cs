using UnityEngine;

namespace Gamekit3D
{
    public class Dissolve : MonoBehaviour
    {
        public float minStartTime = 2f;
        public float maxStartTime = 6f;
        public float dissolveTime = 3f;
        public AnimationCurve curve;

        float m_Timer;
        float m_EmissionRate;
        Renderer[] m_Renderer;
        MaterialPropertyBlock m_PropertyBlock;
        ParticleSystem m_ParticleSystem;
        ParticleSystem.EmissionModule m_Emission;
        float m_StartTime;
        float m_EndTime;

        const string k_CutoffName = "_Cutoff";

        void Awake()
        {

            m_PropertyBlock = new MaterialPropertyBlock();
            m_Renderer = GetComponentsInChildren<Renderer>();

            m_ParticleSystem = GetComponentInChildren<ParticleSystem>();

            m_Emission = m_ParticleSystem.emission;

            m_EmissionRate = m_Emission.rateOverTime.constant;
            m_Emission.rateOverTimeMultiplier = 0;


            m_Timer = 0;

            m_StartTime = Time.time + Random.Range(minStartTime, maxStartTime);
            m_EndTime = m_StartTime + dissolveTime + m_ParticleSystem.main.startLifetime.constant;
        }

        void Update()
        {
            if (Time.time >= m_StartTime)
            {
                float cutoff = 0;

                for (int i = 0; i < m_Renderer.Length; i++)
                {
                    m_Renderer[i].GetPropertyBlock(m_PropertyBlock);
                    cutoff = Mathf.Clamp01(m_Timer / dissolveTime);
                    m_PropertyBlock.SetFloat(k_CutoffName, cutoff);
                    m_Renderer[i].SetPropertyBlock(m_PropertyBlock);
                }


                m_Emission.rateOverTimeMultiplier = curve.Evaluate(cutoff) * m_EmissionRate;


                m_Timer += Time.deltaTime;
            }

            if (Time.time >= m_EndTime)
            {
                Destroy(gameObject);
            }
        }
    }

}