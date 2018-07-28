using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public float speed = 10.0F;
    public float rotationSpeed = 10.0F;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float yAxis = Input.GetAxis("Vertical") * speed;
        float xAxis = Input.GetAxis("Horizontal") * rotationSpeed;
        yAxis *= Time.deltaTime;
        xAxis *= Time.deltaTime;
        transform.Translate(xAxis, yAxis, 0);
    }
}
