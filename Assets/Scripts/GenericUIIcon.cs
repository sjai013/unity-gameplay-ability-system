using UnityEngine;
using UnityEngine.UI;

public class GenericUIIcon : MonoBehaviour
{
    public Image ImageIcon;
    public RectTransform CooldownOverlay;

    public void SetCooldownRemainingPercent(float cooldownPercentRemaining)
    {
        var oldScale = this.CooldownOverlay.localScale;
        this.CooldownOverlay.localScale = new Vector3(oldScale.x, Mathf.Clamp01(1 - cooldownPercentRemaining), oldScale.y);
    }

}
