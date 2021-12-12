using Unity.Entities;

namespace GameplayAbilitySystem.Systems
{
    public abstract class AttributeSystem<T> : SystemBase where T:System.Enum
    {
        protected abstract void UpdateAttribute();
        protected override void OnUpdate()
        {
            this.UpdateAttribute();
        }
    }
}
