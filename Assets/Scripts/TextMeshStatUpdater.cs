using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameplayAbilitySystem.Attributes;
using TMPro;

namespace AbilitySystemDemo
{
    public class TextMeshStatUpdater : MonoBehaviour
    {
        public AttributeType Stat;
        public AttributeSet AttributeSet;
        public TextMeshPro Text;
        // Update is called once per frame
        void Update()
        {
            Text.text = AttributeSet.Attributes.Find(x => x.AttributeType == Stat).CurrentValue.ToString("000");
        }
    }
}