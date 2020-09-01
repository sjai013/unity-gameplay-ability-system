using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gamekit3D
{
    public class ContactDamager : MonoBehaviour
    {
        [HelpBox] public string helpString = @"
Remember to have a collider set to trigger on this object or one of its children!
Also Remember to place that object in a layer that collide with what you want to damage 
(e.g. the Enemy layer does not collide with the Player layer, so add it to a child in a different layer)
";

        public int amount;
        public LayerMask damagedLayers;

        private void OnTriggerStay(Collider other)
        {
            if ((damagedLayers.value & 1 << other.gameObject.layer) == 0)
                return;

            Damageable d = other.GetComponentInChildren<Damageable>();

            if (d != null && !d.isInvulnerable)
            {
                Damageable.DamageMessage message = new Damageable.DamageMessage
                {
                    damageSource = transform.position,
                    damager = this,
                    amount = amount,
                    direction = (other.transform.position - transform.position).normalized,
                    throwing = false
                };

                d.ApplyDamage(message);
            }
        }
    }

    public class HelpBoxAttribute : PropertyAttribute
    {

    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Mathf.Max(EditorGUIUtility.singleLineHeight * 2,
                EditorStyles.helpBox.CalcHeight(new GUIContent(property.stringValue), Screen.width) +
                EditorGUIUtility.singleLineHeight);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.HelpBox(position, property.stringValue, MessageType.Info);
        }
    }
#endif
}