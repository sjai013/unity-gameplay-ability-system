using System;
using System.Linq;
using GameplayAbilitySystem.AttributeSystem.DOTS.Components;
using MyGameplayAbilitySystem.AttributeSystem.DOTS.Components;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace GameplayAbilitySystem.AttributeSystem.DOTS.Systems
{

    public class DynamicAttributeSystem : SystemBase
    {
        EntityQuery m_Query;
        ComponentType[] ComponentTypes;
        public static ComponentType[] GetAttributeTypes()
        {
            var attributeInterfaceType = typeof(IAttributeData);

            var attributeTypes = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => attributeInterfaceType.IsAssignableFrom(p) && !p.IsInterface && p.IsStruct() && UnsafeUtility.SizeOf(p) == UnsafeUtility.SizeOf<PlayerAttribute>())
                        .Select(x => new ComponentType(x))
                        .ToArray();

            return attributeTypes;
        }
        protected override void OnCreate()
        {
            var attributeTypes = GetAttributeTypes();

            var query = new EntityQueryDesc()
            {
                Any = attributeTypes
            };
            m_Query = GetEntityQuery(query);
            ComponentTypes = attributeTypes;
        }
        unsafe protected override void OnUpdate()
        {
            var job = new AttributeJob() { nTypes = ComponentTypes.Length };
            DynamicComponentTypeHandle* ptr = (DynamicComponentTypeHandle*)&job.t0;
            {
                for (int i = 0; i < ComponentTypes.Length; i++)
                {
                    ptr[i] = GetDynamicComponentTypeHandle(ComponentTypes[i]);
                }
            }
            Dependency = job.ScheduleParallel(m_Query, Dependency);
        }


        struct AttributeJob : IJobChunk
        {
            public int nTypes;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t0;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t1;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t2;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t3;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t4;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t5;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t6;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t7;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t8;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t9;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t10;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t11;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t12;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t13;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t14;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t15;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t16;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t17;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t18;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t19;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t20;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t21;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t22;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t23;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t24;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t25;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t26;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t27;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t28;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t29;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t30;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t31;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t32;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t33;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t34;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t35;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t36;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t37;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t38;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t39;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t40;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t41;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t42;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t43;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t44;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t45;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t46;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t47;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t48;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t49;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t50;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t51;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t52;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t53;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t54;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t55;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t56;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t57;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t58;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t59;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t60;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t61;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t62;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t63;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t64;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t65;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t66;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t67;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t68;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t69;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t70;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t71;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t72;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t73;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t74;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t75;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t76;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t77;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t78;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t79;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t80;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t81;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t82;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t83;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t84;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t85;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t86;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t87;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t88;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t89;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t90;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t91;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t92;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t93;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t94;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t95;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t96;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t97;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t98;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t99;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t100;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t101;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t102;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t103;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t104;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t105;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t106;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t107;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t108;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t109;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t110;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t111;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t112;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t113;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t114;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t115;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t116;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t117;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t118;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t119;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t120;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t121;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t122;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t123;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t124;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t125;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t126;
            [NativeDisableContainerSafetyRestriction] public DynamicComponentTypeHandle t127;
            unsafe public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {

                fixed (DynamicComponentTypeHandle* fixedT0 = &t0)
                {

                    for (var t = 0; t < nTypes; t++)
                    {
                        var chunkComponentType = fixedT0[t];
                        var chunkData = chunk.GetDynamicComponentDataArrayReinterpret<PlayerAttribute>(chunkComponentType, UnsafeUtility.SizeOf<PlayerAttribute>());
                        for (var i = 0; i < chunkData.Length; i++)
                        {
                            var data = chunkData[i];
                            data.CurrentValue = data.CalculateCurrentValue();
                            chunkData[i] = data;
                        }
                    }
                }

            }
        }
    }
}