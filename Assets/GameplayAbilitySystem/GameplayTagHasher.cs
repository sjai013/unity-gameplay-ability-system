using GameplayAbilitySystem;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

public class GameplayTagHasher : MonoBehaviour, IConvertGameObjectToEntity {

    [SerializeField]
    private List<GameplayTag> Tags;
    readonly Dictionary<GameplayTag, List<GameplayTag>> TagHierarchy = new Dictionary<GameplayTag, List<GameplayTag>>();

    private void BuildHash() {
        //GameplayTagHashList.TagHash.TryAdd(Tags[0])
        Dictionary<string, GameplayTag> GameplayTagList = new Dictionary<string, GameplayTag>();
        for (var i = 0; i < Tags.Count; i++) {
            var tag = Tags[i];
            var tagName = tag.name;
            tag.SetUniqueId(i + 1);
            if (!(GameplayTagList.TryGetValue(tagName, out var gameplayTag))) {
                GameplayTagList.Add(tagName, tag);
                //// Split the tag by the "."
                //int lastIdx = tagName.LastIndexOf('.');
                //if (lastIdx != 1) {
                //    parentTag = tagName.Substring(0, lastIdx);
                //}
            } else {
                // Object exists
                Debug.LogWarning("The tag [" + tagName + "] has been defined multiple times in the GameplayTagHasher script. " +
                                  "This duplicate was at index [" + i + "]. Please remove the duplicate references. ");
            }
        }

        // Create ancestory hierarchy
        foreach (var gameplayTagKvp in GameplayTagList) {
            var gameplayTag = gameplayTagKvp.Value;
            var gameplayTagName = gameplayTagKvp.Key;
            var parentTagName = "";

            // Recursively try to get all available ancestor
            var ancestorFoundOrNonExistent = false;
            List<GameplayTag> Parent = new List<GameplayTag>(6);
            while (!ancestorFoundOrNonExistent) {
                int lastIdx = gameplayTagName.LastIndexOf('.');
                if (lastIdx <= 0) { // No "." found - we're done
                    ancestorFoundOrNonExistent = true;
                } else {
                    // "." was found, so we need to get parent name
                    parentTagName = gameplayTagName.Substring(0, lastIdx);
                    // Check to see if there is a GameplayTag for this parent
                    if ((GameplayTagList.TryGetValue(parentTagName, out var parentTag))) {
                        Parent.Add(parentTag);
                    }
                }
                gameplayTagName = parentTagName;
            }

            TagHierarchy.Add(gameplayTagKvp.Value, Parent);
        }

    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        BuildHash();
        for (int i = 0; i < TagHierarchy.Count; i++) {
            var item = TagHierarchy.ElementAt(i);
            var tag = item.Key.GetHashCode();
            var parent1 = item.Value.Count > 0 ? item.Value[0].GetHashCode() : -1;
            var parent2 = item.Value.Count > 1 ? item.Value[1].GetHashCode() : -1;
            var parent3 = item.Value.Count > 2 ? item.Value[2].GetHashCode() : -1;
            var parent4 = item.Value.Count > 3 ? item.Value[3].GetHashCode() : -1;
            var parent5 = item.Value.Count > 4 ? item.Value[4].GetHashCode() : -1;
            var parent6 = item.Value.Count > 5 ? item.Value[5].GetHashCode() : -1;
            var data = new GameplayTagComponent {
                Tag = tag,
                Parent1Tag = parent1,
                Parent2Tag = parent2,
                Parent3Tag = parent3,
                Parent4Tag = parent4,
                Parent5Tag = parent5,
                Parent6Tag = parent6,
            };
            var newEntity = dstManager.CreateEntity(typeof(GameplayTagComponent));
            dstManager.SetComponentData(newEntity, data);
        }
    }
}