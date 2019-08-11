using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Attributes;
using UnityEngine;

public class UIAttributeUpdater : MonoBehaviour, IUpdateAttributeValue {
    public RectTransform AttributeBar;
    public float LerpSpeed;
    private float maxWidth;
    // Start is called before the first frame update
    void Start() {
        this.maxWidth = AttributeBar.rect.width;
    }


    public void UpdateAttributeValue(float current, float max) {
        var rect = this.AttributeBar.rect;
        var width = (current / max) * this.maxWidth;
        var lerpedWidth = Mathf.Lerp(rect.width, width, Time.deltaTime * LerpSpeed);
        lerpedWidth = Mathf.Min(lerpedWidth, this.maxWidth);
        lerpedWidth = Mathf.Max(lerpedWidth, 0);
        this.AttributeBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lerpedWidth);
        this.AttributeBar.ForceUpdateRectTransforms();
    }
}

public interface IUpdateAttributeValue {
    void UpdateAttributeValue(float current, float max);
}