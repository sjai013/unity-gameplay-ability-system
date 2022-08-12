# REPOSITORY NO LONGER IN DEVELOPMENT
This repository has not been updated in some time, and I do not have time to spend on this.  As a result, I am archiving this repository.
Existing README is below.
---
# Unity Gameplay Ability System
Gameplay Ability System for Unity (or GAS for short) is a Unity framework for creating games which contain any kind of abilities (e.g. DotA, Skyrim).  The framework helps to simplify the interaction between components in a unified manner.  

This project is heavily inspired by the Unreal Gameplay Ability System, and uses similar terminology, but the implementation is specific to Unity.

There are three main components provided by `Gameplay Ability System`:
1. Attribute System - for managing character attributes, such as health, speed, etc.
2. Gameplay Tags - for managing character states
3. Ability System - for coordinating the attribute system and gameplay tags

This demo includes a simple game where you can run around with the character, and cast an `Ice Blast` ability by pressing the left mouse button, and a `Blood Sacrifice` ability by pressing the right mouse button.  The `Ice Blast` ability fires a projectile forward if there is an enemy ahead, and damages the enemy (consuming mana and putting the ability on cooldown).  Using the `Blood Sacrifice` restores mana based on the percentage left, at the expense of health (and puts the ability on cooldown).  
## Prerequisites
1. Unity 2021.1.1f1 (Unity 2020.3+ should also work)

## Install Demo
To install this demo:
1. Clone the repository
```
git clone https://github.com/sjai013/unity-gameplay-ability-system.git
```
2. Run the main scene `Scenes/Main`

## Package Installation
To install the latest version of the package, import the package from a git URL using the `Unity Package Manager`:
```
https://github.com/sjai013/unity-gameplay-ability-system.git#upm
```

## Wiki
The [Wiki](https://github.com/sjai013/unity-gameplay-ability-system/wiki) is a useful resource for how-tos and tutorials on using the `Unity Gameplay Ability System`.  Check it out!

Read on for specific details about how the `Attribute System`, `Gameplay Tags`, and `Ability System` were incorporated into this demo.
## Attribute System
The `Attribute System` is a collection of code for defining attributes specific to a game (e.g. Strength, Health, etc.).  It also manages how these attributes mutate over the course of a game - for example, taking damage reduces health, or levelling up increases strength.

Attributes have a `Base Value` and a `Current Value`, and currently active modifiers which may be `additive`, `multiplicative`, or `overriding`.

The `Current Value` is calculated as:
```
Current Value = (Base Value + Additive) * Multiplicative
```

Attribute assets are created through the `Create/Gameplay Abiliy System/Attribute` asset menu.  

The behaviour of the default `AttributeScriptableObject` class can be modified by creating a child class derived from `AttributeScriptableObject`.  There is an in-built child class included in this sample (`LinearDerivedAttributeScriptableObject`) which demonstrates how to create "derived attributes" - for example, a "Max Health" attribute which is linearly related to a "Strength" attribute.

This project contains the following attributes (see `Assets/My Gameplay Ability System/Attributes/Base`):
1. Strength
2. Agility
3. Intelligence

All other attributes derive from these, using instances of `LinearDerivedAttributeScriptableObject` (see `Assets/My Gameplay Ability System/Attributes/Derived`):
1. Armour `4 + 0.17 * Agility`
2. Attack Speed `1 * Agility`
3. Max Health `200 + 20 * Strength`
4. Health Regen `1.25 + 0.1 * Strength`
5. Max Mana `75 + 12 * Intelligence`
6. Mana Regen `0.5 + 0.05 * Intelligence`

In addition, there are also `Health` and `Mana` attributes for tracking the current health and mana.

## Gameplay Tags
`Gameplay Tags` are a way of assigning tags to objects in the Ability System.  They can be used to represent boolean states on a character.

A parent can be defined for each tag, allowing you to create hierarchical gameplay tags.  Tags can be compared for an exact match, or partial matches (e.g. ancestor/descendant).

Gameplay Tags assets are created through the `Create/Gameplay Abiliy System/Tag` asset menu.

Gameplay Tag interaction is used in this sample for putting abilities on cooldown.

## Gameplay Effects
`Gameplay Effects` (GE) are at the heart of the ability system.  They are created through the `Create/Gameplay Ability System/Gameplay Effect Definition` asset menu.

![A brand new GE](/Images/new-ge-inspector.png)

When creating a new GE, the following data needs to be defined:

### Duration
The duration policy controls how the GE is applied, and whether it needs to be removed at a later point in time (e.g. a buff/debuff).  The `Duration Policy` can be one of:
1. Instant
2. Infinite
3. Durational

Instant GE affect an attribute's Base Value.  Infinite and Duration GE are similar to Instant GE, but they modify an attribute's Current Value, and references to the effect are stored so the effect can be reverted.  As their name implies, Infinite GE remain applied indefinitely, whereas Duration GE are applied for a set duration, and then automatically expire, reverting their change (like a temporary buff or debuff).

Generally, you would use `Instant` GE for base stats, `Infinite` GE for pseudo-temporary stat changes, such as extra damage from equipping a weapon, and `Durational` GE for timed duration stat changes.

The `Duration Modifier` and `Duration Multiplier` property controls the base value of the duration (in seconds).  This is only applicable for `Durational` GE:
```
    Duration = Duration Modifier * Duration Multiplier
```

A custom scriptable object inheriting from `ModifierMagnitudeScriptableObject` can be used to create new behaviours, such as using another attribute on the character as a base - see the `AttributeBackedModifierMagnitude` class, and it's instantiated Scriptable Objects `MaxHealthAttributeBackedModifier` and `MaxManaAttributeBackedModifier` in `Asset/My Gameplay Ability System/Ability System/Gameplay Effects/Modifiers`.  These particular modifiers are used to initialise the `Health` and `Mana` attributes to the same value as that of the `Max Health` and `Max Mana` attributes, respectively.

### Modifiers
The `Modifiers` collection defines what attributes this GE modifies, and how it modifies them.

![GE Modifiers](/Images/ge-modifiers-collection.png)

The `Attribute` property defines what attributes this modifier affects.  The `Modifier Operator` property, which can be one of `Add`, `Multiply`, or `Override`, determines how this modifier interacts with other modifiers on this attribute.  The `Modifier Magnitude` property is used to calculate the magnitude of the effect, which is then multiplied by the `Multiplier` to give the final modification value:
```
Effect Magnitude = Modifier Magnitude * Multiplier
```
### Conditional Gameplay Effects
WIP

### Gameplay Effect Tags
Tags are used to describe the GE, as well as dictate how it interacts with other GEs.  

#### Asset Tag
The `Asset Tag` is used to identify (usually uniquely, but not always) the GE.  The intent is instead of checking if the GE is an instance of some class, we can compare asset tags to determine if any two GE are the same.

#### Granted Tags
These tags are added to the character *while* the GE is applied.

#### Ongoing Tag Requirements
These tags determine if the GE is active or temporarily disabled.  For a GE to remain active, all tags in the `Require Tags` collection must be present on the character, and none of the tags in the `Ignore Tags` must be present.

#### Application Tag Requirements
These tags determine if the GE can be applied or not.  Since `Instant` GE are never applied, these tags only affect `Infinite` and `Durational` GE.  For an `Infinite` or `Durational` GE to be successfully applied, all tags in the `Require Tags` must be present on the character, and none of the tags in the `Ignore Tags` must be present.

#### Removal Tag Requirements
These tags determine if the GE should be removed prematurely after application.  Since `Instant` GE are never applied, these tags only affect `Infinite` and `Durational` GE.  For an `Infinite` or `Durational` GE to be removed, all tags in the `Require Tags` must be present on the character, and none of the tags in the `Ignore Tags` must be present.

#### Remove Gameplay Effects With Tag
Any existing GE on the character which have an `Asset Tag` contained in this list are prematurely removed.

### Period
The period determines how often to apply a Gameplay Effect.  A durational or infinite Gameplay Effect will continuously apply its Gameplay Effect at regular intervals.  This can be used for things such as health regen.


## Ability System
The `Ability System` is the most complex of the three, and ties together the `Attribute System` and `Gameplay Tags`.

The `Ability System` allows you to define how abilities in the game work, and how they interact with characters, characters' attributes, and each other.

Abilities, at some point of their execution, create `Gameplay Effects`, which are applied to a character, resulting in a change to the character's attributes.

An ability is defined by two classes, deriving from `AbstractAbilityScriptableObject` and `AbstractAbilitySpec`, respectively.  It is recommended to define the `AbstractAbilitySpec` class inside the `AbstractAbilityScriptableObject`.  

As the name suggests, `AbstractAbilityScriptableObject` is a scriptable object, so remember to annotate the class with `[CreateAssetMenu]` to create assets from it using the `Assets` menu.

Assets created from `AbstractAbilityScriptableObject` class defines the static data associated with an ability, such as its associated `Gameplay Tags` and what `Gameplay Effects` it applies.  This data is shared amongst all characters that have this ability.  The corresponding `AbstractAbilitySpec` defines the logic to execute, and any stateful data.

To activate an ability, an `Ability Spec` is created using the `CreateSpec` method on objects inheriting from `AbstractAbilityScriptableObject`.  This is then granted to the character using the `GrantSpec` method on the `AbilitySystemCharacter` component.

### Code Example
As an example, let's say we wanted to create a simple ability type that heals our character.  We would first create a new class inheriting from `AbstractAbilityScriptableObject` with the required data fields, and also create an inner class that inherits from `AbstractAbilitySpec`.  We'll call them `SimpleAbilityScriptableObject` and `SimpleAbilitySpec`, respectively.  In order to heal (or damage) a character, we have to use a `Gameplay Effect`, so our `SimpleAbilityScriptableObject` needs to contain a field for specifying the `Gameplay Effect` as well.

```csharp
    /// <summary>
    /// Simple Ability that applies a Gameplay Effect to the activating character
    /// </summary>
    [CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Simple Ability")]
    public class SimpleAbilityScriptableObject : AbstractAbilityScriptableObject
    {
        /// <summary>
        /// Gameplay Effect to apply
        /// </summary>
        public GameplayEffectScriptableObject GameplayEffect;

        /// <summary>
        /// Creates the Ability Spec, which is instantiated for each character.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
        {
            var spec = new SimpleAbilitySpec(this, owner);
            spec.Level = owner.Level;
            return spec;
        }

        /// <summary>
        /// The Ability Spec is the instantiation of the ability.  Since the Ability Spec
        /// is instantiated for each character, we can store stateful data here.
        /// </summary>
        public class SimpleAbilitySpec : AbstractAbilitySpec
        {
            public SimpleAbilitySpec(AbstractAbilityScriptableObject abilitySO, AbilitySystemCharacter owner) : base(abilitySO, owner)
            {
            }

            /// <summary>
            /// What to do when the ability is cancelled.  We don't care about there for this example.
            /// </summary>
            public override void CancelAbility() { }

            /// <summary>
            /// What happens when we activate the ability.
            /// 
            /// In this example, we apply the cost and cooldown, and then we apply the main
            /// gameplay effect
            /// </summary>
            /// <returns></returns>
            protected override IEnumerator ActivateAbility()
            {
                // Apply cost and cooldown
                var cdSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cooldown);
                var costSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cost);
                this.Owner.ApplyGameplayEffectSpecToSelf(cdSpec);
                this.Owner.ApplyGameplayEffectSpecToSelf(costSpec);


                // Apply primary effect
                var effectSpec = this.Owner.MakeOutgoingSpec((this.Ability as SimpleAbilityScriptableObject).GameplayEffect);
                this.Owner.ApplyGameplayEffectSpecToSelf(effectSpec);

                yield return null;
            }

            /// <summary>
            /// Checks to make sure Gameplay Tags checks are met. 
            /// 
            /// Since the target is also the character activating the ability,
            /// we can just use Owner for all of them.
            /// </summary>
            /// <returns></returns>
            public override bool CheckGameplayTags()
            {
                return AscHasAllTags(Owner, this.Ability.AbilityTags.OwnerTags.RequireTags)
                        && AscHasNoneTags(Owner, this.Ability.AbilityTags.OwnerTags.IgnoreTags)
                        && AscHasAllTags(Owner, this.Ability.AbilityTags.SourceTags.RequireTags)
                        && AscHasNoneTags(Owner, this.Ability.AbilityTags.SourceTags.IgnoreTags)
                        && AscHasAllTags(Owner, this.Ability.AbilityTags.TargetTags.RequireTags)
                        && AscHasNoneTags(Owner, this.Ability.AbilityTags.TargetTags.IgnoreTags);
            }

            /// <summary>
            /// Logic to execute before activating the ability.  We don't need to do anything here
            /// for this example.
            /// </summary>
            /// <returns></returns>

            protected override IEnumerator PreActivate()
            {
                yield return null;
            }
        }
    }

```

We can now create abilities that allow us to apply 3 `Gameplay Effects` to a single character - a `Cost` (which controls what resources our character must have to activate this ability), a `Cooldown` (which controls when the ability can be activated), and the actual `Gameplay Effect`, which could be anything we want - increase our strength, decrease our agility, whatever.

To activate this ability, we need to create an asset for this ability.  Because we took advantage of the `[CreateAssetMenu]` attribute, we just have to right click in the `Project` tab in Unity, and select `Create | Gameplay Ability System | Abilities | Simple Ability`, and fill in the required fields.
![Simple Ability](/Images/simple-ability.png)

We can then create a new `Component` on our character, that takes in this `AbstractAbilityScriptableObject`-derived class, and calls `CreateSpec`, passing in the character's `AbilitySystemCharacter` component.  We then grant this ability to the `AbilitySystemCharacter` using the `GrantAbility` method.

When we want to activate this ability, we call this component's `ActivateAbility` method.

```csharp
public class AbilityController : MonoBehaviour
{
    [SerializeField] private AbstractAbilityScriptableObject Ability;

    [SerializeField] AbilitySystemCharacter abilitySystemCharacter;

    private AbstractAbilitySpec abilitySpec;

    void Start() 
    {
      abilitySpec = Ability.CreateSpec(abilitySystemCharacter);
      abilitySystemCharacter.GrantAbility(spec);
    }

    public void ActivateAbility() 
    {
      StartCoroutine(abilitySpec.TryActivateAbility());
    }
}
```

And that's it!  A really simple ability, where all the data associated with the ability is now set through the inspector.  Go ahead and uninstall your code editor.

# Contributing
You can contribute to this project by:
* Posting issues
* Creating PRs with bug fixes or new features
* Testing this in your own games and providing feedback
* Adding to the Wiki
* Helping with documentation
* Telling your friends!

# License
Gameplay Ability System for Unity is release under the MIT license.  You are free to use, modify, and distribute this software, as long as the copyright header is left intact.

# Thank You
Thanks to @tranek for providing a comprehensive overview of the functionality of Unreal's Gameplay Ability System (https://github.com/tranek/GASDocumentation).  This resource is used heavily in understanding how Unreal's Gameplay Ability System works, so it can be recreated for Unity.
