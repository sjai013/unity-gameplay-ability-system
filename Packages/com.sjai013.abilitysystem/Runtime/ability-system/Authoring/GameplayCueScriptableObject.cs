using UnityEngine;

namespace AbilitySystem.Authoring
{
    public abstract class GameplayCueScriptableObject : ScriptableObject
    {
        public abstract AbstractGameplayCueSpec CreateSpec(GameplayEffectSpec spec);

        public abstract class AbstractGameplayCueSpec
        {
            public GameplayCueScriptableObject Instance;
            private GameplayEffectSpec geSpec;

            public AbstractGameplayCueSpec(GameplayEffectSpec geSpec)
            {
                this.geSpec = geSpec;
            }
            public abstract void Initialise(GameplayEffectSpec geSpec);
            public abstract void Activate(GameplayEffectSpec geSpec);
            public abstract void Remove(GameplayEffectSpec geSpec);
        }

    }

}