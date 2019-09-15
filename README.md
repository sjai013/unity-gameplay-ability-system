# Gameplay Ability System for Unity
Gameplay Ability System for Unity (or GAS for short) is a Unity framework for creating games which contain any kind of abilities (e.g. DotA, Skyrim).  The framework helps to simplify the interaction between components in a unified manner.  

The approach for this is taken from that used by Unreal's Gameplay Ability System, but implemented in Unity using the Data-Oriented Technology Stack (DOTS) where possible.  

![GAS in action](https://i.imgur.com/0OTe4KG.gif)


## Installation
1. Head over to [Releases](https://github.com/sjai013/UnityGameplayAbilitySystem/releases) and download the latest version.
2. Import package into your Unity project
3. Add the [UniRx](https://assetstore.unity.com/packages/tools/integration/unirx-reactive-extensions-for-unity-17276) extension from the Asset Store (or install the [UniRx Async](https://github.com/Cysharp/UniTask) package from GitHub)

## Demo
Clone this repository and open it in Unity.  The Demo project is set up with three abilities, bound to the '1', '2', and '3' keys respectively.  There is a sample "Fire" damaging ability, an ability that does nothing except trigger a global cooldown, and a "Heal" healing ability.

## Code Structure
The code is currently in a state of flux, sharing both Scriptable Object-based implementation and DOTS based implementation.  There is lots of redundant code.  The Scriptable Object-based implementation will eventually be removed.

To understand how everything ties together, go through the *GenericAbilitySystem.cs* class.  Raise an issue if you need help.

More detailed documentation/code layout will be added in a later release.  

## Roadmap
The following functionality will be implemented in GAS:
* ~~ Basic character stats ~~ [Done]
* ~~ Basic gameplay effects ~~ [Done]
* ~~ Basic stat modification from gameplay effects ~~ [Done]
* ~~ Basic abilities ~~ [Done]
* ~~ Cooldown effects ~~ [Done]
* ~~ Cooldown sharing between abilities (e.g. a Global cooldown after every ability cast) ~~ [Done]
* ~~ Visual display of abilities (e.g. an "ability bar") ~~ [Done]
* ~~ Visual display of cooldowns(e.g. on an "ability bar" ~~ [Done]
* Visual display of active gameplay effects
* Sample code showing how to restrict which gameplay effects are displayed (e.g. on a "status bar")
* Periodic effects (such as damage-over-time)
* Targetting system (e.g. single-target, multi-target, intelligent target selection, targettability feedback)
* Customised definition of Ability pre-casting checks
* Gameplay effect interactions (e.g. a "dispel" type effect removing a "poison" effect on the character, or a "dispel" type effect requiring character to be "poisoned" before allowing cast)
* Ability casting times
* Ability Combos (e.g. providing some method of defining ability sequences and then executing abilities based on some condition)
* Nicer/more designer-friendly way of defining abilities and effects
* Nicer/more designer-friendly way of defining character stats
* Networking capability (prediction, replication, synching)


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