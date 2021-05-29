using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyControl : CustomBehaviour {

    [SerializeField] PlayerProfile setting;

    [SerializeField, Space(10)] CustomBehaviour root;
    [SerializeField] PlayerCameraGimbal gimbal;

    [SerializeField] CustomBehaviour top, middle, bottom;


    public CustomBehaviour Top => top;
    public CustomBehaviour Middle => middle;
    public CustomBehaviour Bottom => bottom;

    Vector3 forwardVector;

    public override void OnLateUpdate() {
        float radian = (-gimbal.AzimuthValue - 90F) * Mathf.Deg2Rad;

        forwardVector.x = Mathf.Cos(radian);
        forwardVector.y = 0F;
        forwardVector.z = Mathf.Sin(radian);

        transform.rotation = Quaternion.LookRotation(forwardVector);

        float hSpeed = Input.GetAxisRaw("Horizontal");
        float vSpeed = Input.GetAxisRaw("Vertical");

        root.transform.Translate((transform.forward * vSpeed + transform.right * hSpeed).normalized * setting.MoveSpeed * Time.deltaTime);

#if UNITY_EDITOR
        Debug.DrawLine(transform.position, transform.position + transform.forward);
#endif
    }

}
