using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraScroll : MonoBehaviour {

	public Slider speedSlider;
	public float moveSpeed = 0.5f;

	// Use this for initialization
	void Start () {
		speedSlider.onValueChanged.AddListener (delegate{ChangeSpeed();});
	}
	
	// Update is called once per frame
	void Update () {
		//Move the camera to the left based on current speedSlider setting
		transform.Translate (Vector3.left * (Time.deltaTime * moveSpeed));

		//If the camera passes the last animation, loop to the beginning
		if(transform.position.x > 112){
			transform.position = new Vector3(0f, transform.position.y, transform.position.z);
		}
	}

	void ChangeSpeed(){
		moveSpeed = speedSlider.value;
	}
}
