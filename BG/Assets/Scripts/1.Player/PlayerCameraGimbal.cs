using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraGimbal : CustomBehaviour {

    [SerializeField] PlayerProfile setting;

    [Space(10), SerializeField] CustomBehaviour target;

    [SerializeField] CustomBehaviour camPivot;
    [SerializeField] Camera cam;
    public Camera PlayerCamera => cam;

    Vector3 mainRootVector;
    public Vector3 subBranchVector;

    Vector3 cameraLocalPosition = Vector3.zero;
    float cameraNormalizeSpeed = 1F;
    Quaternion cameraQuat;

    [System.NonSerialized] public Vector3 viewPoint;

    [SerializeField] float mainRootLength;
    [SerializeField] float subBranchLength;
    [SerializeField] Vector3 cameraEulerAngle;


    [Space(10), SerializeField] bool lockHorizontal = false;
    [SerializeField] bool lockVertical = false;
    [SerializeField] bool disableCamBlocking = false;


    public float AzimuthValue { get; set; } = -180F;
    public float ElevationValue { get; set; } = 80F;

    float cachedAzimuth, cachedElevation;

    float flexibleMainRootLen, flexibleSubBranchLen;

    RaycastHit cameraBackHit;


    void OnStart() {
        mainRootVector = -target.transform.forward;
        subBranchVector.Normalize();
        cameraQuat = Quaternion.Euler(cameraEulerAngle);
        flexibleMainRootLen = mainRootLength;
        flexibleSubBranchLen = subBranchLength;
    }

    void OnFixedUpdate() {
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

    void OnUpdate() {
        float mouseH = Input.GetAxis("Mouse X");
        float mouseV = Input.GetAxis("Mouse Y");

        if (!lockHorizontal) {
            AzimuthValue += mouseH * setting.MouseSpeedHorizontal * Time.deltaTime;
        }
        if (!lockVertical) {
            ElevationValue = Mathf.Clamp(ElevationValue + mouseV * setting.MouseSpeedVertical * Time.deltaTime, setting.CameraLimit.x, setting.CameraLimit.y);
        }
        
        mainRootVector.x = Mathf.Sin(ElevationValue * Mathf.Deg2Rad) * Mathf.Sin(AzimuthValue * Mathf.Deg2Rad);
        mainRootVector.y = Mathf.Cos(ElevationValue * Mathf.Deg2Rad);
        mainRootVector.z = Mathf.Sin(ElevationValue * Mathf.Deg2Rad) * Mathf.Cos(AzimuthValue * Mathf.Deg2Rad);

        Vector3 convertedSub = ConvertSubBranchVector();

        // Apply
        camPivot.transform.localPosition 
            = target.transform.localPosition + (mainRootVector * flexibleMainRootLen + convertedSub * flexibleSubBranchLen);

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


        if (Input.GetMouseButton(1)) {
            LerpHelper.Lerp(ref flexibleMainRootLen, 1F, 8.325f);
            LerpHelper.Lerp(ref flexibleSubBranchLen, 1.2f, 2.5f);
        } else {
            LerpHelper.Lerp(ref flexibleMainRootLen, mainRootLength, 8.325f);
            LerpHelper.Lerp(ref flexibleSubBranchLen, subBranchLength, 2.5f);
        }

        cam.transform.localPosition = cameraLocalPosition;
        LerpHelper.Lerp(ref cameraLocalPosition, Vector3.zero, cameraNormalizeSpeed);


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
        float radAzi = (-AzimuthValue - 92.15F) * Mathf.Deg2Rad;
        float radElv = (ElevationValue - 89F) * Mathf.Deg2Rad;

        viewPoint.x = Mathf.Cos(radAzi);
        viewPoint.y = Mathf.Sin(radElv);
        viewPoint.z = Mathf.Sin(radAzi);

        viewPoint = target.transform.position + viewPoint * setting.PointDistance;
    }

    
    public void ReboundCamera(Vector3 dir, float scale = 1F) {
        cameraLocalPosition = dir;
        cameraNormalizeSpeed = scale;
    }
}
