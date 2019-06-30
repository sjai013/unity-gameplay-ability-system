using UnityEngine;
using UnityEngine.UI;

public class GenericUIIcon : MonoBehaviour {
    public Image ImageIcon;
    public Image CooldownOverlay;
    

    public void SetCooldownRemainingPercent(float cooldownPercentRemaining) {
        this.CooldownOverlay.fillAmount = Mathf.Clamp01(1 - cooldownPercentRemaining);
    }

}
