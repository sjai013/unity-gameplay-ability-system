using GameplayAbilitySystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class GameplayTagHasher : MonoBehaviour, IConvertGameObjectToEntity {

    [SerializeField]
    private List<GameplayTag> Tags;

    private GameplayTagCache TagHash;
    private void BuildHash() {
        //GameplayTagHashList.TagHash.TryAdd(Tags[0])
        Dictionary<GameplayTag, GameplayTag> TagHierarchy = new Dictionary<GameplayTag, GameplayTag>();
        Dictionary<string, GameplayTag> GameplayTagList = new Dictionary<string, GameplayTag>();
        for (var i = 0; i < Tags.Count; i++) {
            var tag = Tags[i];
            var tagName = tag.name;
            if (!(GameplayTagList.TryGetValue(tagName, out var gameplayTag))) {
                GameplayTagList.Add(tagName, tag);
                //// Split the tag by the "."
                //int lastIdx = tagName.LastIndexOf('.');
                //if (lastIdx != 1) {
                //    parentTag = tagName.Substring(0, lastIdx);
                //}
            }
            else {
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

            // Recursively try to get the first available ancestor
            var ancestorFoundOrNonExistent = false;
            GameplayTag Parent = null;
            while (!ancestorFoundOrNonExistent) {
                int lastIdx = gameplayTagName.LastIndexOf('.');
                if (lastIdx != -1) {
                    parentTagName = gameplayTagName.Substring(0, lastIdx);
                    // Check to see if there is a GameplayTag for this parent
                    if ((GameplayTagList.TryGetValue(parentTagName, out var parentTag))) {
                        Parent = parentTag;
                        ancestorFoundOrNonExistent = true;
                    }
                    else {
                        gameplayTagName = parentTagName;
                    }
                }
                else {
                    ancestorFoundOrNonExistent = true;
                }
            }

            TagHierarchy.Add(gameplayTagKvp.Value, Parent);
        }

        // Map Tag ancestor hierarchy to GameplayTagHashList
        TagHash.TagHash = new int[TagHierarchy.Count];
        TagHash.TagParentHash = new int[TagHierarchy.Count];
        TagHash.HasValidParent = new Bbool[TagHierarchy.Count];

        Dictionary<int, GameplayTag> hashTagDict = new Dictionary<int, GameplayTag>();

        for (var i = 0; i < TagHierarchy.Count; i++) {
            var tag = TagHierarchy.ElementAt(i);
            TagHash.TagHash[i] = tag.Key.GetHashCode();
            if (tag.Value != null) {
                TagHash.TagParentHash[i] = tag.Value.GetHashCode();
            }
            TagHash.HasValidParent[i] = tag.Value != null;
            hashTagDict.Add(tag.Key.GetHashCode(), tag.Key);
        }

    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        BuildHash();
        for (int i = 0; i < TagHash.TagHash.Length; i++) {
            var data = new GameplayTagComponent {
                TagHash = TagHash.TagHash[i],
                HasValidParent = TagHash.HasValidParent[i],
                TagParentHash = TagHash.TagParentHash[i]
            };
            var newEntity = dstManager.CreateEntity(typeof(GameplayTagComponent));
            dstManager.SetComponentData(newEntity, data);
        }
    }
}