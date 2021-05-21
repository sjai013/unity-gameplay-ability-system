using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXDestroyer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private VisualEffect vfx;
    [SerializeField] private float minTime;

    [SerializeField] private float currentTime = 0;
    void Start()
    {
        currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (vfx.aliveParticleCount <= 0 && currentTime > minTime) Destroy(this.gameObject);
    }
}
