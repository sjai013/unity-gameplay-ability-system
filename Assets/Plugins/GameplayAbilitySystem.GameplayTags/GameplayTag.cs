using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GamplayAbilitySystem.GameplayTags
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct GameplayTag
    {
        [FieldOffset(0)]
        private uint Id;

        [FieldOffset(0)]
        public byte L0;

        [FieldOffset(1)]
        public byte L1;

        [FieldOffset(2)]
        public byte L2;

        [FieldOffset(3)]
        public byte L3;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEqualTo(GameplayTag other)
        {
            // Apply bitmask based on this
            // e.g. for A.B.C.D, this A matches other A.*
            var mask = 0xFFFFFFFF;
            mask &= L3 == 0 ? 0xFFFFFF00 : 0xFFFFFFFF;
            mask &= L2 == 0 ? 0xFFFF0000 : 0xFFFFFFFF;
            mask &= L1 == 0 ? 0xFF000000 : 0xFFFFFFFF;
            mask &= L0 == 0 ? 0x00000000 : 0xFFFFFFFF;
            return (mask & other.Id) == Id;
        }
    }

}