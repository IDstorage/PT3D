using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraGimbal : CustomBehaviour {

    [SerializeField] CustomBehaviour target;

    [SerializeField] Camera cam;

    Vector3 mainRootVector;
    public Vector3 subBranchVector;
    Quaternion cameraQuat;

    [SerializeField] float mainRootLength;
    [SerializeField] float subBranchLength;
    [SerializeField] Vector3 cameraEulerAngle;

    [Space(10), SerializeField] float mouseSpeedHorizontal = 1F;
    [SerializeField] float mouseSpeedVertical = 1F;


    public float AzimuthValue { get; set; } = -180F;
    public float ElevationValue { get; set; } = 55F;

    float flexibleMainRootLen;

    RaycastHit cameraBackHit;


    public override void OnStart() {
        mainRootVector = -target.transform.forward;
        cameraQuat = Quaternion.Euler(cameraEulerAngle);
    }

    public override void OnFixedUpdate() {
        bool hasObstacle = Physics.Raycast(target.transform.position, mainRootVector, out cameraBackHit, mainRootLength);
        if (hasObstacle) {
            flexibleMainRootLen = cameraBackHit.distance;
        }
        else {
            flexibleMainRootLen = mainRootLength;
        }
    }

    public override void OnUpdate() {
        float mouseH = -Input.GetAxis("Mouse X");
        float mouseV = -Input.GetAxis("Mouse Y");

        AzimuthValue += mouseH * mouseSpeedHorizontal * Time.deltaTime;
        ElevationValue = Mathf.Clamp(ElevationValue + mouseV * mouseSpeedVertical * Time.deltaTime, 30F, 100F);
        
        mainRootVector.x = Mathf.Sin(ElevationValue * Mathf.Deg2Rad) * Mathf.Sin(AzimuthValue * Mathf.Deg2Rad);
        mainRootVector.y = Mathf.Cos(ElevationValue * Mathf.Deg2Rad);
        mainRootVector.z = Mathf.Sin(ElevationValue * Mathf.Deg2Rad) * Mathf.Cos(AzimuthValue * Mathf.Deg2Rad);

        // Apply
        cam.transform.position 
            = target.transform.position + (mainRootVector * flexibleMainRootLen + subBranchVector * subBranchLength);

        cameraQuat = Quaternion.LookRotation((target.transform.position + target.transform.forward) - cam.transform.position);
        cam.transform.rotation = cameraQuat;
    }

}
