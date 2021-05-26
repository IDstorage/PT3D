using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraGimbal : CustomBehaviour {

    [SerializeField] CustomBehaviour target;

    [SerializeField] Camera cam;

    Vector3 mainRootVector;
    Vector3 subBranchVector;
    Quaternion cameraQuat;

    [SerializeField] float mainRootLength;
    [SerializeField] float subBranchLength;
    [SerializeField] Vector3 cameraEulerAngle;

    [Space(10), SerializeField] float mouseSpeedHorizontal = 1F;
    [SerializeField] float mouseSpeedVertical = 1F;


    float azimuthValue = -180F, elevationValue = 90F;


    public override void OnStart() {
        mainRootVector = -target.transform.forward;
        subBranchVector = Vector3.up;
        cameraQuat = Quaternion.Euler(cameraEulerAngle);
    }

    public override void OnUpdate() {
        float mouseH = -Input.GetAxis("Mouse X");
        float mouseV = -Input.GetAxis("Mouse Y");

        azimuthValue = Add(azimuthValue, mouseH * mouseSpeedHorizontal * Time.deltaTime);
        elevationValue = Mathf.Clamp(Add(elevationValue, mouseV * mouseSpeedVertical * Time.deltaTime), 20F, 160F);
        
        mainRootVector.x = Mathf.Sin(elevationValue * Mathf.Deg2Rad) * Mathf.Sin(azimuthValue * Mathf.Deg2Rad);
        mainRootVector.y = Mathf.Cos(elevationValue * Mathf.Deg2Rad);
        mainRootVector.z = Mathf.Sin(elevationValue * Mathf.Deg2Rad) * Mathf.Cos(azimuthValue * Mathf.Deg2Rad);

        // Apply
        cam.transform.position = target.transform.position + (mainRootVector * mainRootLength + subBranchVector * subBranchLength);

        cameraQuat = Quaternion.LookRotation(target.transform.position - cam.transform.position);
        cam.transform.rotation = cameraQuat;
    }

    float Add(float origin, float additional) {
        // 1/180 == 0.00555...
        origin += additional;// * 0.00556F;
        //if (origin >= 360F) origin -= 360F;
        //else if (origin <= -360F) origin += 360F;
        return origin;
    }

}
