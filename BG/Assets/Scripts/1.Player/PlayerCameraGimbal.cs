using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraGimbal : CustomBehaviour {

    [SerializeField] PlayerProfile setting;

    [Space(10), SerializeField] CustomBehaviour target;

    [SerializeField] CustomBehaviour camPivot;
    [SerializeField] Camera cam;

    Vector3 mainRootVector;
    public Vector3 subBranchVector;
    Quaternion cameraQuat;
    [System.NonSerialized] public Vector3 viewPoint;

    [SerializeField] float mainRootLength;
    [SerializeField] float subBranchLength;
    [SerializeField] Vector3 cameraEulerAngle;


    [Space(10), SerializeField] bool lockHorizontal = false;
    [SerializeField] bool lockVertical = false;
    [SerializeField] bool disableCamBlocking = false;


    public float AzimuthValue { get; set; } = -180F;
    public float ElevationValue { get; set; } = 55F;

    float cachedAzimuth, cachedElevation;

    float flexibleMainRootLen;

    RaycastHit cameraBackHit;


    public override void OnStart() {
        mainRootVector = -target.transform.forward;
        subBranchVector.Normalize();
        cameraQuat = Quaternion.Euler(cameraEulerAngle);
    }

    public override void OnFixedUpdate() {
        flexibleMainRootLen = mainRootLength;

        if (!disableCamBlocking) RaycastCamera();
    }

    void RaycastCamera() {
        bool hasObstacle = Physics.Raycast(target.transform.position, mainRootVector, out cameraBackHit, mainRootLength);
        if (hasObstacle) {
            flexibleMainRootLen = cameraBackHit.distance;
        }
        else {
            flexibleMainRootLen = mainRootLength;
        }

#if UNITY_EDITOR
        Debug.DrawLine(target.transform.position, target.transform.position + mainRootVector * mainRootLength, Color.magenta);
#endif
    }

    public override void OnUpdate() {
        float mouseH = -Input.GetAxis("Mouse X");
        float mouseV = -Input.GetAxis("Mouse Y");

        if (!lockHorizontal) {
            AzimuthValue += mouseH * setting.MouseSpeedHorizontal * Time.deltaTime;
        }
        if (!lockVertical) {
            ElevationValue = Mathf.Clamp(ElevationValue + mouseV * setting.MouseSpeedVertical * Time.deltaTime, 25F, 120F);
        }
        
        mainRootVector.x = Mathf.Sin(ElevationValue * Mathf.Deg2Rad) * Mathf.Sin(AzimuthValue * Mathf.Deg2Rad);
        mainRootVector.y = Mathf.Cos(ElevationValue * Mathf.Deg2Rad);
        mainRootVector.z = Mathf.Sin(ElevationValue * Mathf.Deg2Rad) * Mathf.Cos(AzimuthValue * Mathf.Deg2Rad);

        Vector3 convertedSub = ConvertSubBranchVector();

        // Apply
        camPivot.transform.localPosition 
            = target.transform.localPosition + (mainRootVector * flexibleMainRootLen + convertedSub * subBranchLength);

        CalculateViewPoint();
        cameraQuat = Quaternion.LookRotation(viewPoint - camPivot.transform.position);
        camPivot.transform.localRotation = cameraQuat;


        if (Input.GetKeyDown(KeyCode.LeftAlt)) {
            cachedAzimuth = AzimuthValue;
            cachedElevation = ElevationValue;
        } else if (Input.GetKeyUp(KeyCode.LeftAlt)) {
            AzimuthValue = cachedAzimuth;
            ElevationValue = cachedElevation;
        }


#if UNITY_EDITOR
        Debug.DrawLine(target.transform.position, target.transform.position + mainRootVector * flexibleMainRootLen, Color.red);
        Debug.DrawLine(target.transform.position + mainRootVector * flexibleMainRootLen, camPivot.transform.position, Color.yellow);
        Debug.DrawLine(camPivot.transform.position, viewPoint, Color.green);
#endif
    }


    Vector3 ConvertSubBranchVector() {
        float radian = (-AzimuthValue - 180F) * Mathf.Deg2Rad;

        Vector3 convertedSubBranch = subBranchVector;
        convertedSubBranch.x = subBranchVector.x * Mathf.Cos(radian) - subBranchVector.z * Mathf.Sin(radian);
        convertedSubBranch.z = subBranchVector.x * Mathf.Sin(radian) + subBranchVector.z * Mathf.Cos(radian);
        return convertedSubBranch;
    }


    void CalculateViewPoint() {
        float radAzi = (-AzimuthValue - 90F) * Mathf.Deg2Rad;
        float radElv = (ElevationValue - 90F) * Mathf.Deg2Rad;

        viewPoint.x = Mathf.Cos(radAzi);
        viewPoint.y = Mathf.Sin(radElv);
        viewPoint.z = Mathf.Sin(radAzi);

        viewPoint = target.transform.position + viewPoint * setting.PointDistance;
    }
}
