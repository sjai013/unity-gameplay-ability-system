using System;
using System.Collections;
using System.Collections.Generic;
using Gamekit3D;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private int animationStateLayers;
    [SerializeField] private AnimationTagState[] animationStates;


    // Start is called before the first frame update
    void Start()
    {
        animationStates = new AnimationTagState[animationStateLayers];
        SceneLinkedSMB<PlayerBehaviour>.Initialise(animator, this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCurrentState(AnimationTagScriptableObject tag, int layerIndex)
    {
        var state = this.animationStates[layerIndex];
        state.CurrentState = tag;
        this.animationStates[layerIndex] = state;
    }

    public void SetNextState(AnimationTagScriptableObject tag, int layerIndex)
    {
        var state = this.animationStates[layerIndex];
        state.NextState = tag;
        this.animationStates[layerIndex] = state;
    }

    public AnimationTagState GetState(int layerIndex)
    {
        return this.animationStates[layerIndex];
    }

    [Serializable]
    public struct AnimationTagState
    {
        public AnimationTagScriptableObject CurrentState;
        public AnimationTagScriptableObject NextState;
    }
}
