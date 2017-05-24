using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour {

    Rigidbody rb;
    LogitechSteeringWheel lsw;

    LogitechGSDK.DIJOYSTATE2ENGINES logi;

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody>();
        lsw = LogitechSteeringWheel.GetInstance();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if(Input.GetKey(KeyCode.UpArrow))
        rb.AddForce(transform.forward * 20, ForceMode.Acceleration);
        brakes();
        logi = LogitechGSDK.LogiGetStateUnity(0);
    }

    void brakes()
    {
        Debug.Log((logi.lY / 32767) * 20);
        if (logi.lY < 0)
        rb.AddForce(-(transform.forward * ((logi.lY / 32767) * 20)), ForceMode.Acceleration);
    }
}
