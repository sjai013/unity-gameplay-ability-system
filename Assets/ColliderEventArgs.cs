using System;
using UnityEngine;

public class ColliderEventArgs : EventArgs {
    public Collider other;
    public Collider @this;
}
