using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour {

    Rigidbody rb;
    LogitechGSDK.DIJOYSTATE2ENGINES logitechSDK;

    /* Engine sound of the vehicle */
    AudioSource enginesource;

    /* Get the button pressed on the wheel */
    int wheelButtonPressed;

    /* Engine Variables */
    bool engineStartBool;
    bool ranOnceEngine;
    bool slowDownVehicle;

    /* How long does it take to accelerate or turn? */
    double momentumAccelerate;
    double momentumTurning;

    /* Maximum number of how acceleration and turning can go to */
    float lengthOfAcceleration;
    float lengthOfTurning;

    /* Raw input of acceleration and turning of wheels and pedals */
    float acceleration;
    float turning;

    /* slowly increases based on momentumaccelerate and momentumturning */
    double accelerationTime;
    double turningTime;

    /* changes from cameraA to cameraB */
    bool drivingPosition;

    /* GameObject of Camera */
    public GameObject[] vehicleCamera;
    public GameObject vehicleTractor;

    private static VehicleManager instance = null;

    public static VehicleManager GetInstance()
    {
        return instance;
    }

    void Start()
    {
        slowDownVehicle = true;

         enginesource = GetComponent<AudioSource>();

        momentumAccelerate = 0.2;
        momentumTurning = 0.2;

        drivingPosition = true;
        rb = GetComponent<Rigidbody>();

        lengthOfAcceleration = 4f; // 0 to 5 Acceleration // How fast the vehicle can go
        lengthOfTurning = 2f;
    }

    void Update()
    {
        logitechSDK = LogitechGSDK.LogiGetStateUnity(0);
        engineStart();
        convertRangePosition();
        switchDrivingPosition();
        accelerationControl(drivingPosition);
        //Debug.Log(getWheelButtonPressed());
        //if (Input.GetButton("StartEngine")) Debug.Log("HELLO");
    }

    int getWheelButtonPressed()
    {
        for (int i = 0; i < 128; i++)
        {
            if (logitechSDK.rgbButtons[i] == 128)
            {
                return i;
            }
        }
        return 0;
    }

    //int getWheelButtonPressed()
    //{
    //    bool check = true;
    //    for (int i = 0; i < 128; i++)
    //    {
    //        if (logitechSDK.rgbButtons[i] == 128)
    //        {
    //            wheelButtonPressed = i;
    //            check = false;
    //        }
    //        else
    //        {
    //            if (check)
    //            {
    //                wheelButtonPressed = 999;
    //            }
    //        }
    //    }
    //    return wheelButtonPressed;
    //}

    //void engineStart()
    //{
    //    if (getWheelButtonPressed() == 23)
    //    {
    //        if (ranOnceEngine)
    //        {
    //            ranOnceEngine = false;
    //            if (engineStartBool)
    //            {
    //                enginesource.Stop(); slowDownVehicle = true;
    //            }
    //            else
    //            {
    //                enginesource.Play(); slowDownVehicle = false;
    //            }
    //            engineStartBool = !engineStartBool;
    //        }
    //    }

    //    if (getWheelButtonPressed() == 0)
    //        ranOnceEngine = true;
    //}

    void engineStart()
    {
        if (Input.GetButton("StartEngine"))
        {
            if (!ranOnceEngine)
            {
                ranOnceEngine = true;
                if (engineStartBool)
                {
                    enginesource.Stop(); slowDownVehicle = true;
                }
                else
                {
                    enginesource.Play(); slowDownVehicle = false;
                }
                engineStartBool = !engineStartBool;
            }
        }
        else
        {
            ranOnceEngine = false;
        }
    }

    void convertRangePosition() // Changes 500 to -500 into 0 to 1000
    {
        acceleration = -(((logitechSDK.lY - 32767f) / (32767f + 32767f) * lengthOfAcceleration)); // range from 0 to lengthOfAcceleration
        turning = -((((logitechSDK.lX - 32767f) / (32767f + 32767f) * lengthOfTurning)) + (lengthOfTurning / 2));
    }

    //Moves the vehicle forward & stops if no acceleration is given, changes view of the camera
    void accelerationControl(bool n)
    {
        //forces the start acceleration to be 1f // fixes rotation on 0.01f speed -.-
        if (acceleration < 0.5f && acceleration > 0f)
        {
            acceleration = 0.5f;
        }
        if (accelerationTime < momentumAccelerate && acceleration == 0)
        {
            accelerationTime = 0;
        }
        else if (accelerationTime > acceleration || slowDownVehicle) // Stopping
        {
            if (accelerationTime > 0)
                accelerationTime -= momentumAccelerate;
        }
        else if (accelerationTime < acceleration) // Drive Forward
        {
            accelerationTime += momentumAccelerate;
            if (accelerationTime > lengthOfAcceleration)
            {
                accelerationTime = lengthOfAcceleration;
            }
        }

        if (accelerationTime > 0)
        {
            if (n)
            {
                driveVehicle(-accelerationTime);
                turningVehicle(drivingPosition);
            }
            else
            {
                driveVehicle(accelerationTime);
                turningVehicle(drivingPosition);
            }
        }
    }

    void driveVehicle(double value)
    {
        var v = transform.forward * (float)value;
        v.y = rb.velocity.y;
        rb.velocity = v;
    }

    void turningVehicle(bool n)
    {
        if (n)
            turningTime = ((accelerationTime * turning));
        else
            turningTime = -((accelerationTime * turning));

        if (n)
        {
            var desiredRotQ = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + (float)-turningTime, transform.eulerAngles.x);
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotQ, Time.deltaTime * 10f);
        }
        else
        {
            var desiredRotQ = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + (float)turningTime, transform.eulerAngles.x);
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotQ, Time.deltaTime * 10f);
        }
    }

    //Changes the position of the driving position
    void switchDrivingPosition()
    {
        if (Input.GetKeyDown(KeyCode.M) && accelerationTime <= 0)
        {
            drivingPosition = !drivingPosition;
        }

        //if (drivingPosition)
        //{
        //    cameraViewB.SetActive(false);
        //    cameraViewA.SetActive(true);
        //}
        //else
        //{
        //    cameraViewB.SetActive(true);
        //    cameraViewA.SetActive(false);
        //}
    }
}
