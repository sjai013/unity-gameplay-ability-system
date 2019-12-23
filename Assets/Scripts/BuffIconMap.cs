using UnityEngine;

[CreateAssetMenu(fileName = "BuffIconMap", menuName = "Ability System Demo/Buff Icon Map")]
public class BuffIconMap : ScriptableObject {
    public int BuffIdentifier;
    public Sprite Sprite;
    public Color SpriteColor;
}