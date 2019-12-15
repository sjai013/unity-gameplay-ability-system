using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderMessageForwarder : MonoBehaviour {
    // Start is called before the first frame update
    public GameObject Receiver;
    void OnTriggerEnter(Collider other) {
        Receiver.SendMessage("OnTriggerEnter", other, SendMessageOptions.DontRequireReceiver);
    }
}
