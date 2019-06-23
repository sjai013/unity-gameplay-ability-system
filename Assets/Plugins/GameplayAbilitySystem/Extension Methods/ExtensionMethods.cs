using System.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UniRx.Async;

namespace GameplayAbilitySystem.ExtensionMethods {
    public static class UnityEventExtensionMethods {
        /// <summary>
        /// Waits for event to execute, then returns.
        /// </summary>
        /// <param name="evt"></param>
        /// <returns>Task</returns>
        public static async Task WaitForEvent(this UnityEvent evt, CancellationToken cts = default(CancellationToken)) {
            var eventTriggered = false;
            UnityAction method = new UnityAction(() => {
                eventTriggered = true;
            });
            evt.AddListener(method);
            while (!eventTriggered) {
                await UniTask.DelayFrame(0);
                if (cts.IsCancellationRequested) {
                    return;
                }
            }
            evt.RemoveListener(method);
        }


        /// <summary>
        /// Waits for event to execute, then returns when T returned by event matches the comparer.
        /// </summary>
        /// <param name="evt">UnityEvent to wait for</param>
        /// <param name="compareFunc">Function to define how to compare the returned value from the Event to some other value</param>
        public static async Task<T> WaitForEvent<T>(this UnityEvent<T> evt, Func<T, bool> compareFunc, CancellationToken cts = default(CancellationToken)) {
            T val = default(T);
            UnityAction<T> method = new UnityAction<T>((x) => {
                val = x;
            });
            evt.AddListener(method);

            while (!compareFunc(val)) {
                await UniTask.DelayFrame(0);
                if (cts.IsCancellationRequested) {
                    return (default(T));
                }
            }
            evt.RemoveListener(method);
            return val;
        }

        public static async Task<(T1 T1, T2 T2)> WaitForEvent<T1, T2>(this UnityEvent<T1, T2> evt, Func<T1, T2, bool> compareFunc, CancellationToken cts = default(CancellationToken)) {
            T1 val1 = default(T1);
            T2 val2 = default(T2);
            UnityAction<T1, T2> method = new UnityAction<T1, T2>((x, y) => {
                val1 = x;
                val2 = y;
            });
            evt.AddListener(method);

            while (!compareFunc(val1, val2)) {
                await UniTask.DelayFrame(0);
                if (cts.IsCancellationRequested) {
                    return (default(T1), default(T2));
                }
            }
            evt.RemoveListener(method);
            return (val1, val2);
        }

        public static async Task<(T1 T1, T2 T2, T3 T3)> WaitForEvent<T1, T2, T3>(this UnityEvent<T1, T2, T3> evt, Func<T1, T2, T3, bool> compareFunc, CancellationToken cts = default(CancellationToken)) {
            T1 val1 = default(T1);
            T2 val2 = default(T2);
            T3 val3 = default(T3);
            UnityAction<T1, T2, T3> method = new UnityAction<T1, T2, T3>((x, y, z) => {
                val1 = x;
                val2 = y;
                val3 = z;
            });
            evt.AddListener(method);
            while (!compareFunc(val1, val2, val3)) {
                await UniTask.DelayFrame(0);
                if (cts.IsCancellationRequested) {
                    return (default(T1), default(T2), default(T3));
                }
            }
            evt.RemoveListener(method);
            return (val1, val2, val3);
        }

        public static async Task<(T1 T1, T2 T2, T3 T3, T4 T4)> WaitForEvent<T1, T2, T3, T4>(this UnityEvent<T1, T2, T3, T4> evt, Func<T1, T2, T3, T4, bool> compareFunc, CancellationToken cts = default(CancellationToken)) {
            T1 val1 = default(T1);
            T2 val2 = default(T2);
            T3 val3 = default(T3);
            T4 val4 = default(T4);
            UnityAction<T1, T2, T3, T4> method = new UnityAction<T1, T2, T3, T4>((x, y, z, u) => {
                val1 = x;
                val2 = y;
                val3 = z;
                val4 = u;
            });
            evt.AddListener(method);
            while (!compareFunc(val1, val2, val3, val4)) {
                await UniTask.DelayFrame(0);
                if (cts.IsCancellationRequested) {
                    return (val1, val2, val3, val4);
                }
            }
            evt.RemoveListener(method);
            return (val1, val2, val3, val4);
        }

    }

}