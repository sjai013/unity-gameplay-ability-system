using System;
using System.Collections;
using System.Collections.Generic;
using GameplayAbilitySystem.Attributes.Components;
using UnityEngine;

namespace AbilitySystemDemo
{

    [CreateAssetMenu(fileName = "DemoStatChangeHandler", menuName = "Ability System Demo/Demo Stat Change Handler")]
    public class StatChangeAnimationHandler : AbstractStatChangeHandler
    {
        public AttributeType HealthAttribute;
        public AttributeType SpeedAttribute;

        public override void StatChanged(AttributeChangeData Change)
        {
            // If health was reduced, play a damage taken animation.  We could be fancy here and do
            // a check on the actual gameplay effect.  For example, this could have been a self-cast
            // ability that reduced health for increasing some other stat

            /* 
            if (Change.Modifier.Attribute == HealthAttribute && Change.NewValue < Change.OldValue)
            {
                Change.Target.GetActor().GetComponent<Animator>().SetTrigger("Take Damage");
                return;
            }
            */

        }

    }
}