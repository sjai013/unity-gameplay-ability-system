using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public interface ITagIdentifier {

    void MatchTag(int TagHash);
}

public struct TagContainer : ITagIdentifier {
    private NativeList<int> Tags;
    //private NativeHashMap<int, int>

    public void MatchTag(int TagHash) {
        throw new System.NotImplementedException();
    }
}

public struct GameplayTagCache  {
    public int[] TagHash;
    public int[] TagParentHash;
    public Bbool[] HasValidParent;
}

public struct GameplayTagComponent : IComponentData {
    public int TagHash;
    public int TagParentHash;
    public Bbool HasValidParent;
}
    
