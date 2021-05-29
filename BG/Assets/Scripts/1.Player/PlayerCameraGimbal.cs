using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraGimbal : CustomBehaviour {

    [SerializeField] PlayerProfile setting;

    [Space(10), SerializeField] CustomBehaviour target;

    [SerializeField] Camera cam;

    Vector3 mainRootVector;
    public Vector3 subBranchVector;
    Quaternion cameraQuat;
    Vector3 viewPoint;

    [SerializeField] float mainRootLength;
    [SerializeField] float subBranchLength;
    [SerializeField] Vector3 cameraEulerAngle;


    public float AzimuthValue { get; set; } = -180F;
    public float ElevationValue { get; set; } = 55F;

    float flexibleMainRootLen;

    RaycastHit cameraBackHit;


    public override void OnStart() {
        mainRootVector = -target.transform.forward;
        subBranchVector.Normalize();
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

#if UNITY_EDITOR
        Debug.DrawLine(target.transform.position, target.transform.position + mainRootVector * mainRootLength, Color.magenta);
#endif
    }

    public override void OnUpdate() {
        float mouseH = -Input.GetAxis("Mouse X");
        float mouseV = -Input.GetAxis("Mouse Y");

        AzimuthValue += mouseH * setting.MouseSpeedHorizontal * Time.deltaTime;
        ElevationValue = Mathf.Clamp(ElevationValue + mouseV * setting.MouseSpeedVertical * Time.deltaTime, 30F, 100F);
        
        mainRootVector.x = Mathf.Sin(ElevationValue * Mathf.Deg2Rad) * Mathf.Sin(AzimuthValue * Mathf.Deg2Rad);
        mainRootVector.y = Mathf.Cos(ElevationValue * Mathf.Deg2Rad);
        mainRootVector.z = Mathf.Sin(ElevationValue * Mathf.Deg2Rad) * Mathf.Cos(AzimuthValue * Mathf.Deg2Rad);

        Vector3 convertedSub = ConvertSubBranchVector();

        // Apply
        cam.transform.position 
            = target.transform.position + (mainRootVector * flexibleMainRootLen + convertedSub * subBranchLength);

        CalculateViewPoint();
        cameraQuat = Quaternion.LookRotation(viewPoint - cam.transform.position);
        cam.transform.rotation = cameraQuat;


#if UNITY_EDITOR
        Debug.DrawLine(target.transform.position, target.transform.position + mainRootVector * flexibleMainRootLen, Color.red);
        Debug.DrawLine(target.transform.position + mainRootVector * flexibleMainRootLen, cam.transform.position, Color.yellow);
        Debug.DrawLine(cam.transform.position, viewPoint, Color.green);
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
        float radian = (-AzimuthValue - 90F) * Mathf.Deg2Rad;

        viewPoint.x = Mathf.Cos(radian);
        viewPoint.y = 0F;
        viewPoint.z = Mathf.Sin(radian);

        viewPoint = target.transform.position + viewPoint * setting.PointDistance;
    }

}
