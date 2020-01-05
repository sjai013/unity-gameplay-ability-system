/*
 * Created on Sun Jan 05 2020
 *
 * The MIT License (MIT)
 * Copyright (c) 2020 Sahil Jain
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;
namespace GameplayAbilitySystem.GameplayTags.Components {

    [StructLayout(LayoutKind.Explicit)]
    public struct GameplayTagComponent : IComponentData, IEquatable<GameplayTagComponent> {
        [FieldOffset(0)] public uint TagId;
        [FieldOffset(0)] public byte TagIdLevel0;
        [FieldOffset(1)] public byte TagIdLevel1;
        [FieldOffset(2)] public byte TagIdLevel2;
        [FieldOffset(3)] public byte TagIdLevel3;

        public override bool Equals(object other) {
            if (other is GameplayTagComponent) return this.Equals(other);
            return false;
        }

        public bool Equals(GameplayTagComponent other) {
            return this.GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode() {
            return TagId.GetHashCode();
        }

        public bool IsIdenticalTo(GameplayTagComponent other) {
            return this.Equals(other);
        }

        public bool IsMatchTo(GameplayTagComponent other) {
            // Reduce the incoming tag to the same level as this tag
            other.TagId = math.select(other.TagId & 0xffffff00, other.TagId, this.TagIdLevel3 == 0);
            other.TagId = math.select(other.TagId & 0xffff0000, other.TagId, this.TagIdLevel2 == 0);
            other.TagId = math.select(other.TagId & 0xff000000, other.TagId, this.TagIdLevel1 == 0);
            other.TagId = math.select(other.TagId & 0x00000000, other.TagId, this.TagIdLevel0 == 0);

            // Compare this tag to the reduced other tag
            return this.Equals(other);
        }

    }
}