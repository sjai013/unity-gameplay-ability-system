using UnityEngine;
using UnityEngine.UI;

public class UIBuffElement : MonoBehaviour
{
    [SerializeField] Image m_Icon;
    [SerializeField] Image m_Progress;
    public void Initialise(Sprite sprite, float durationRemainPercent)
    {
        m_Icon.sprite = sprite;
        m_Progress.fillAmount = durationRemainPercent;
    }
}
