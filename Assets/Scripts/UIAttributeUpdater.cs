using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Attributes;
using UnityEngine;

public class UIAttributeUpdater : MonoBehaviour {
    public AttributeSet AttributeSet;
    public AttributeType AttributeType;
    public AttributeType MaxAttributeType;

    private Attribute Attribute;
    private Attribute MaxAttribute;

    public RectTransform AttributeBar;

    public float LerpSpeed;

    private float maxWidth;
    // Start is called before the first frame update
    void Start() {
        this.Attribute = this.AttributeSet.Attributes.FirstOrDefault(x => x.AttributeType == AttributeType);
        this.MaxAttribute = this.AttributeSet.Attributes.FirstOrDefault(x => x.AttributeType == MaxAttributeType);
        this.maxWidth = AttributeBar.rect.width;
    }

    // Update is called once per frame
    void Update() {
        var rect = this.AttributeBar.rect;
        var width = (this.Attribute.CurrentValue / this.MaxAttribute.CurrentValue) * this.maxWidth;
        var lerpedWidth = Mathf.Lerp(rect.width, width, Time.deltaTime * LerpSpeed);
        lerpedWidth = Mathf.Min(lerpedWidth, this.maxWidth);
        lerpedWidth = Mathf.Max(lerpedWidth, 0);
        this.AttributeBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lerpedWidth);
        this.AttributeBar.ForceUpdateRectTransforms();
    }
}
