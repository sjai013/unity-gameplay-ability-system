using Unity.Entities;

namespace GamplayAbilitySystem.GameplayTags.Components
{
    public struct GameplayTagBufferElement : IBufferElementData
    {
        // These implicit conversions are optional, but can help reduce typing.
        public static implicit operator GameplayTag(GameplayTagBufferElement e) { return e.Value; }
        public static implicit operator GameplayTagBufferElement(GameplayTag e) { return new GameplayTagBufferElement { Value = e }; }
        public GameplayTag Value;
    }
}