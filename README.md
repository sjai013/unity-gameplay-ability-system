# Table of Contents
- [Table of Contents](#table-of-contents)
- [Gameplay Ability System for Unity](#gameplay-ability-system-for-unity)
  - [Attribute System](#attribute-system)
  - [Gameplay Tags](#gameplay-tags)
  - [Ability System](#ability-system)
  - [Gameplay Effects](#gameplay-effects)
    - [Duration](#duration)
    - [Modifiers](#modifiers)
    - [Conditional Gameplay Effects](#conditional-gameplay-effects)
    - [Gameplay Effect Tags](#gameplay-effect-tags)
      - [Asset Tag](#asset-tag)
      - [Granted Tags](#granted-tags)
      - [Ongoing Tag Requirements](#ongoing-tag-requirements)
      - [Application Tag Requirements](#application-tag-requirements)
      - [Removal Tag Requirements](#removal-tag-requirements)
      - [Remove Gameplay Effects With Tag](#remove-gameplay-effects-with-tag)
    - [Period](#period)
- [Contributing](#contributing)
- [License](#license)
- [Thank You](#thank-you)
# Gameplay Ability System for Unity
Gameplay Ability System for Unity (or GAS for short) is a Unity framework for creating games which contain any kind of abilities (e.g. DotA, Skyrim).  The framework helps to simplify the interaction between components in a unified manner.  

This project is heavily inspired by the Unreal Gameplay Ability System, and uses similar terminology, but the implementation is specific to Unity.

There are three main sub-projects here (TODO: Unity UPM package list):
1. Attribute System
2. Gameplay Tags
3. Ability System and Gameplay Effects

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

All other attributes derive from these, using instances of `LinearDerivedAttributeScriptableObject` ((see `Assets/My Gameplay Ability System/Attributes/Derived`)):
1. Armour
2. Attack Speed
3. Health Regen
4. Mana Regen
5. Max Health
6. Max Mana

In addition, there are also `Health` and `Mana` attributes for tracking the current health.


## Gameplay Tags
`Gameplay Tags` are a way of assigning tags to objects in the Ability System.  They're similar in concept to the way default Unity tags work, but the implementation here is using Scriptable Objects.

A parent can be defined for each tag, allowing you to create hierarchical gameplay tags.

Gameplay Tags assets are created through the `Create/Gameplay Abiliy System/Tag` asset menu.  

## Ability System
The `Ability System` is the most complex of the three, and ties together the `Attribute System` and `Gameplay Tags`.

The `Ability System` allows you to define how abilities in the game work, and how they interact with characters, characters' attributes, and each other.

Abilities, at some point of their execution, create `Gameplay Effects`, which are applied to a character, resulting in a change to the character's attributes.
## Gameplay Effects
`Gameplay Effects` (GE) are at the heart of the ability system.  They are created through the `Create/Gameplay Ability System/Gameplay Effect Definition` asset menu.

![A brand new GE](/images/new-ge-inspector.png)

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

![GE Modifiers](/images/ge-modifiers-collection.png)

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
WIP

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
