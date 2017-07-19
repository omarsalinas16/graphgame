using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearLines : MonoBehaviour {

	GameObject lines;
	// Use this for initialization
	void Start () {
		lines = gameObject.transform.GetChild(0).gameObject;
		lines.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey("z") ) {
			rotate(new Vector3(0,0,0));
		}
		else if (Input.GetKey("x") ) {
			rotate(new Vector3(90,0,0));
		}
		else if (Input.GetKey("s") ) {
			rotate(new Vector3(0,-90,0));
		}
		else {
			if(lines.activeSelf) 
				lines.SetActive(false);
			LookAtCamera.Instance.setRotatingFixed(false);
		}
	}
	
	private void rotate(Vector3 v) {
		Quaternion rotation = Quaternion.Euler(v);
		lines.SetActive(true);
		lines.transform.rotation = rotation;
		LookAtCamera.Instance.fixedRotation(rotation);
	}
}
