using UnityEngine;
using UnityEngine.Playables;

public class StandardMaterialEmissionMixerBehaviour : PlayableBehaviour
{
    Color m_DefaultColor;

    const string k_EmissionColorName = "_EmissionColor";
    int m_EmissionColorId;

    bool m_FirstFrameHappened;
    Renderer m_TrackBinding;
    Material[] m_TrackBindingMaterials;
    int m_MaterialIndex = -1;
    bool m_IndicesMatch = true;

    public override void OnGraphStart (Playable playable)
    {
        m_EmissionColorId = Shader.PropertyToID (k_EmissionColorName);
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_TrackBinding = playerData as Renderer;
        
        if (m_TrackBinding == null)
            return;

        int inputCount = playable.GetInputCount();

        if (!m_FirstFrameHappened)
        {
            for (int i = 0; i < inputCount; i++)
            {
                ScriptPlayable<StandardMaterialEmissionBehaviour> inputPlayable = (ScriptPlayable<StandardMaterialEmissionBehaviour>)playable.GetInput(i);
                StandardMaterialEmissionBehaviour input = inputPlayable.GetBehaviour();
                if (i == 0)
                    m_MaterialIndex = input.materialIndex;
                else if (m_MaterialIndex < 0 || m_MaterialIndex != input.materialIndex)
                {
                    m_IndicesMatch = false;
                    for (int j = 0; j < inputCount; j++)
                    {
                        ScriptPlayable<StandardMaterialEmissionBehaviour> checkedInputPlayable = (ScriptPlayable<StandardMaterialEmissionBehaviour>)playable.GetInput(j);
                        StandardMaterialEmissionBehaviour checkedInput = checkedInputPlayable.GetBehaviour();
                        checkedInput.materialIndicesMatch = false;
                    }
                    break;
                }
            }
        }

        if(!m_IndicesMatch)
            return;

        if (!m_FirstFrameHappened)
        {
            m_TrackBindingMaterials = new Material[m_TrackBinding.sharedMaterials.Length];
            m_TrackBindingMaterials[m_MaterialIndex] = new Material(m_TrackBinding.sharedMaterials[m_MaterialIndex]);
            m_TrackBinding.materials = m_TrackBindingMaterials;
            m_DefaultColor = m_TrackBindingMaterials[m_MaterialIndex].GetColor(m_EmissionColorId);
            m_FirstFrameHappened = true;
        }

        Color blendedColor = Color.clear;

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<StandardMaterialEmissionBehaviour> inputPlayable = (ScriptPlayable<StandardMaterialEmissionBehaviour>)playable.GetInput(i);
            StandardMaterialEmissionBehaviour input = inputPlayable.GetBehaviour ();

            blendedColor += input.color * inputWeight;
        }

        m_TrackBindingMaterials[m_MaterialIndex].SetColor(m_EmissionColorId, blendedColor);
    }

    public override void OnGraphStop (Playable playable)
    {
        m_TrackBindingMaterials[m_MaterialIndex].SetColor (m_EmissionColorId, m_DefaultColor);
        Object.Destroy (m_TrackBindingMaterials[m_MaterialIndex]);
        m_FirstFrameHappened = false;
    }
}
