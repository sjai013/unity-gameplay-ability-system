using GameplayTag.Authoring;


namespace AbilitySystem
{
    public interface IGameplayTagInspector
    {
        void Inspect(GameplayTagScriptableObject.GameplayTag[] tags, AbilitySystem.GameplayEffectContainer geContainer);
    }
}
