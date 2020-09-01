using UnityEngine;
using UnityEngine.Playables;

public class MaterialSwitcherMixerBehaviour : PlayableBehaviour
{
    bool m_FirstFrameHappened;
    Material[] m_OriginalSharedMaterials;
    Material[] m_DefaultMaterials;
    Renderer m_TrackBinding;
    int m_InputCount = -1;

    public override void OnPlayableCreate (Playable playable)
    {
        m_InputCount = playable.GetInputCount();
    }

    bool Setup (Playable playable)
    {
        m_OriginalSharedMaterials = m_TrackBinding.sharedMaterials;
        m_DefaultMaterials = new Material[m_OriginalSharedMaterials.Length];
        for (int i = 0; i < m_OriginalSharedMaterials.Length; i++)
        {
            m_DefaultMaterials[i] = new Material(m_OriginalSharedMaterials[i]);
        }

        if (m_InputCount > 0)
        {
            for (int i = 0; i < m_InputCount; i++)
            {
                ScriptPlayable<MaterialSwitcherBehaviour> inputPlayable = (ScriptPlayable<MaterialSwitcherBehaviour>)playable.GetInput(i);
                MaterialSwitcherBehaviour input = inputPlayable.GetBehaviour();
                if (!input.SetMaterials (m_DefaultMaterials))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_TrackBinding = playerData as Renderer;

        if (m_TrackBinding == null)
            return;

        if (!m_FirstFrameHappened)
        {
            m_FirstFrameHappened = m_DefaultMaterials != null;
            m_FirstFrameHappened &= Setup (playable);
        }

        if (!m_FirstFrameHappened)
            return;

        m_TrackBinding.materials = m_DefaultMaterials;

        for (int i = 0; i < m_InputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<MaterialSwitcherBehaviour> inputPlayable = (ScriptPlayable<MaterialSwitcherBehaviour>)playable.GetInput(i);
            MaterialSwitcherBehaviour input = inputPlayable.GetBehaviour ();

            if (inputWeight > 0 && input.setupCorrectly)
            {
                m_TrackBinding.materials = input.materials;
            }
        }
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        if (m_FirstFrameHappened)
        {
            m_FirstFrameHappened = false;

            if (m_DefaultMaterials != null)
            {
                if(m_TrackBinding != null)
                    m_TrackBinding.sharedMaterials = m_OriginalSharedMaterials;

                m_DefaultMaterials = null;
            }
        }
    }
}
