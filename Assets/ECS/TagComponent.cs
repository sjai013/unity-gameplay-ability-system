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

public struct GameplayTagComponent : IComponentData {
    public int Tag;
    public int Parent1Tag;
    public int Parent2Tag;
    public int Parent3Tag;
    public int Parent4Tag;
    public int Parent5Tag;
    public int Parent6Tag;
}
    
