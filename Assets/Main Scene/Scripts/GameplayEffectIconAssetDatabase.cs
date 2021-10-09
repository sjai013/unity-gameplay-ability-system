using System;
using System.Collections.Generic;
using GameplayTag.Authoring;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay Ability System/Gameplay Effect/Icon Asset Database")]
public class GameplayEffectIconAssetDatabase : ScriptableObject
{
    [SerializeField] private GameplayEffectIconMap[] m_IconMap;
    private Dictionary<GameplayTagScriptableObject.GameplayTag, Sprite> m_Cache;


    public void Reset()
    {
        m_Cache = BuildLookupTable();
    }

    public GameplayEffectIconMap[] ListIconMap()
    {
        return m_IconMap;
    }

    public Sprite Get(GameplayTagScriptableObject.GameplayTag tag)
    {
        return m_Cache[tag];
    }

    public bool HasIcon(GameplayTagScriptableObject.GameplayTag tag, out Sprite sprite)
    {
        // If cache hasn't been built yet, build it
        if (m_Cache == null) Reset();

        if (m_Cache.TryGetValue(tag, out sprite))
        {
            return true;
        }

        return false;
    }

    private Dictionary<GameplayTagScriptableObject.GameplayTag, Sprite> BuildLookupTable()
    {
        Dictionary<GameplayTagScriptableObject.GameplayTag, Sprite> dict = new Dictionary<GameplayTagScriptableObject.GameplayTag, Sprite>();
        for (var i = 0; i < m_IconMap.Length; i++)
        {
            dict.TryAdd(m_IconMap[i].Tag.TagData, m_IconMap[i].Icon);
        }

        return dict;
    }
}

[Serializable]
public struct GameplayEffectIconMap
{
    public Sprite Icon;
    public GameplayTagScriptableObject Tag;
}