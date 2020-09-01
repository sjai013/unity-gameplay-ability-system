using System.Collections;
using System.Collections.Generic;
using Gamekit3D;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    public PlayerController Player;
    public CanvasGroup Reticule;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Reticule.alpha = Player.isAiming ? 1 : 0;
    }
}
