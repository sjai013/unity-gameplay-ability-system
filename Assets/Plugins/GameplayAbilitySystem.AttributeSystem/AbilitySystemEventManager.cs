/*
 * Created on Sun Mar 29 2020
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
using System.Collections.Generic;

/// <summary>
/// This class is used to manage raising events for the Ability System
/// 
/// Code was inspired by https://stackoverflow.com/questions/2237927/is-there-any-way-to-create-indexed-events-in-c-sharp-or-some-workaround
/// </summary>
public abstract class AbilitySystemEventManager<T1, TEventArgs, TEventArgsContainer>
where T1 : struct
where TEventArgs : struct {
    public abstract T1 KeyFromArgs(TEventArgs e);

    public class AbilitySystemEvent {
        public event EventHandler<TEventArgsContainer> OnEvent;

        public void RaiseEvent(ref TEventArgsContainer e) {
            OnEvent?.Invoke(this, e);
        }

        public void RaiseEvent(TEventArgsContainer e) {
            OnEvent?.Invoke(this, e);
        }

        public void Reset() {
            OnEvent = null;
        }
    }

    private Dictionary<T1, AbilitySystemEvent> m_objects = new Dictionary<T1, AbilitySystemEvent>();

    public AbilitySystemEvent this[T1 id] {
        get {
            if (!m_objects.ContainsKey(id))
                m_objects.Add(id, new AbilitySystemEvent());

            return m_objects[id];
        }
    }

    /// <summary>
    /// Clean up after itself
    /// </summary>
    public void Reset() {
        // Go through dict and clear all event indices
        foreach (KeyValuePair<T1, AbilitySystemEvent> item in m_objects) {
            item.Value.Reset();
        }

        // Clear dictionary
        m_objects.Clear();
    }

}