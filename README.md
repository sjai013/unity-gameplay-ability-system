# Gameplay Ability System for Unity
Gameplay Ability System for Unity (or GAS for short) is a Unity framework for creating games which contain any kind of abilities (e.g. DotA, Skyrim).  The framework helps to simplify the interaction between components in a unified manner.  

The approach for this is taken from that used by Unreal's Gameplay Ability System.

![GAS in action](https://i.imgur.com/8eDC5Cz.gif)

## Installation
1. Head over to [Releases](https://github.com/sjai013/UnityGameplayAbilitySystem/releases) and download the latest version.
2. Import package into your Unity project
3. Add the [UniRx](https://assetstore.unity.com/packages/tools/integration/unirx-reactive-extensions-for-unity-17276) extension from the Asset Store (or install the [UniRx Async](https://github.com/Cysharp/UniTask) package from GitHub)

## Demo
Clone this repository and open it in Unity.  The Demo project is set up with two abilities, bound to the 'f' and 'g' keys respectively.  Pressing the 'f' key will fire an ability which will damage the enemy character, and pressing the 'g' key will heal the player-controller character and grant movement speed.

## Usage

### Basic Setup
1. Add the **Ability System Component** and **Attribute Set** components to all objects that can cast abilities or have attribute
2. Create and add **Ability System/Attribute Type** Scriptable Objects for each different kind of attribute you want (e.g. Health, Mana, Speed, Damage, etc.).  _Note:  You will want to have **Max Health** and **Max Mana** as separate attributes as well - it'll make your life easier_. 

   ![Player Component](https://i.imgur.com/BrGWLxL.png)
3. Add the **AnimationBehaviourEventSystem** to the player Animator (this is so you can tie execution of abilities to character animation)

    ![Animation Behaviour](https://i.imgur.com/WNwSPkb.png)
4. Create an animation state in the Animator for the animation to play when using the ability.  Create a Trigger parameter for transitioning to this state.  
5. Create a **Ability System/Animation System/Animation Event** Scriptable Object to represent the "Fire Projectile" animation event (to tell the Ability System when to create the visual effect). 
6. Add an event to the ability animation, and call **OnAnimationEvent( Animation Event ), passing the "Fire Projectile" animation event Scriptable Object as the parameter.
![Fire Projectile](https://i.imgur.com/SOKyczL.png)

### Finally...
7. Create a **Ability System/Gameplay Tag** Scriptable Object to represent a unique tag for the ability.  _Note: Naming them something like A.B.C (nested as many levels as you need) will help with organisation_ 

    ![Ability Tag](https://i.imgur.com/pNvVP5g.png)
8. Create a **Ability System/Gameplay Tag** Scriptable Object to represent a unique tag for the ability cooldown

   ![Ability Cooldown Tag](https://i.imgur.com/kcS2Jk6.png)
9.  Create a **Ability System/Gameplay Cue/Spawn Object At Target** Scriptable Object to represent the graphical component of the ability.  Drag in a prefab of the object you want to spawn (e.g. a particle system prefab).  You'll also want to set the position/rotation/scale offsets, and if the object doesn't destroy itself, define when you want the object destroyed.

    ![Gameplay Cue](https://i.imgur.com/dLxXpZN.png)
10. Create a **Ability System/Gameplay Effect** Scriptable Object to represent the ability's impact on attributes (e.g. damage/healing, speed buff, etc.).  Set the **Gameplay Cue** to the **Spawn Object At Target** Scriptable Object.

    ![Gameplay Effect](https://i.imgur.com/XTyhtv3.png)
11. Create a **Ability System/Gameplay Effect** Scriptable Object to represent the cooldown of the ability.

    ![Cooldown Gameplay Effect](https://i.imgur.com/yGxRjIc.png) 
12. Create a **Ability System/Ability Logic/Ability** Scriptable Object, which defines some of the finer points on how the ability animation and sequencing is handled:
![Ability Logic](https://i.imgur.com/Z9blZub.png)
    *  **Target Gameplay Effect** - Gameplay Effect to apply to target  - created in Step 10.
    *  **Fire Projectile** - Animation Event to wait for, before creating the effect (such as firing the projectile) - created in Step 5.
    *  **Wait For Event Tag** The tag that the system will wait for at the start.  This will usually be the ability tag - created in Step 8.
    *  **Animation Trigger Name** - Trigger parameter in Animator which will transition the Animator to the ability casting animation - created in Step 4.
    *  **Completion Animator State Full Hash** - The full hash of the Animator state to indicate we have transitioned out of our animation state and are back to a "default" animation.  In the example below, the base layer is called Base, and the "default" animation is called "Idle".  After the animation completes, the Animator goes back to the Base.Idle state.
13. Create a **Ability System/Ability** Scriptable Object, which references the **Ability Tag** (Step 8), **Cooldown Gameplay Effect** (Step 11), and the **Ability Logic** (Step 12)

[Ability Logic](https://i.imgur.com/f46VYYo.png)

### BURN
14. Execute the ability by calling something like the following code
```csharp
        var gameplayEventData = new GameplayEventData();
        // eventTag is the Ability Tag set up in Step 8
        gameplayEventData.EventTag = eventTag;

        // this is the target of the ability
        // (the unlucky sob about to get hurt)
        // this Target object is a reference to 
        // an AbilitySystemComponent on the 
        // GameObject
        gameplayEventData.Target = this.Target1;

        // SelfAbilitySystem is a reference to the Ability System
        // of the casting player. 
        // Try casting the ability.
        if (SelfAbilitySystem.TryActivateAbility(Ability1))
        {
            // Casting was successful.
            // Send gameplay event to this player with information on target etc
            AbilitySystemStatics.SendGameplayEventToComponent
                (
                    SelfAbilitySystem,
                    eventTag, 
                    gameplayEventData
                );
        }
```

## Contributing
You can contribute to this project by:
* Posting issues
* Creating PRs with bug fixes or new features
* Testing this in your own games and telling us how this can be improved
* Adding to the Wiki
* Helping with documentation
* Telling your friends!

## License
Gameplay Ability System for Unity is release under the MIT license.  You are free to use, modify, and distribute this software, as long as the copyright header is left intact.