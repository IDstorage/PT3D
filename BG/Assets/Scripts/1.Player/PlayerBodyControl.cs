using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CustomFramework;
using CustomFramework.Extension;

public class PlayerBodyControl : CustomBehaviour {

    [SerializeField] CustomBoxCollider coll;

    [SerializeField] PlayerProfile setting;

    [SerializeField, Space(10)] CustomBehaviour root;
    [SerializeField] Rigidbody rigid;
    [SerializeField] CharacterController controller;
    [SerializeField] PlayerCameraGimbal gimbal;

    [SerializeField] CustomBehaviour top, middle, bottom;

    public CustomBehaviour Top => top;
    public CustomBehaviour Middle => middle;
    public CustomBehaviour Bottom => bottom;

    Vector3 forwardVector, movingDirection;
    float gravityValue = 0F;

    Quaternion moveRayRot = Quaternion.Euler(0F, 45F, 0F);
    float moveRayDist = Mathf.Sqrt(2F) * 0.5f + 0.1f;

    public override void OnUpdate() {
        float radian = (-gimbal.AzimuthValue - 90F) * Mathf.Deg2Rad;

        forwardVector.x = Mathf.Cos(radian);
        forwardVector.y = 0F;
        forwardVector.z = Mathf.Sin(radian);

        if (!Input.GetKeyDown(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.LeftAlt) && !Input.GetKeyUp(KeyCode.LeftAlt)) {
            transform.rotation = Quaternion.LookRotation(forwardVector);
        }

        float hSpeed = Input.GetAxisRaw("Horizontal");
        float vSpeed = Input.GetAxisRaw("Vertical");

        //if (Input.GetKeyDown(KeyCode.Space)) {
        //    gravityValue = -setting.JumpPower;
        //}
        //else if (!controller.isGrounded) {
        //    gravityValue += 9.8f * setting.GravityScale * Time.deltaTime;
        //    if (gravityValue > 9.8f) gravityValue = 9.8f;
        //}
        //else {
        //    gravityValue = 0F;
        //}

        movingDirection = (transform.forward * vSpeed + transform.right * hSpeed).normalized + transform.up * -gravityValue;

        if (CustomPhysics.Raycast(transform.position, movingDirection, moveRayDist) == null
            && CustomPhysics.Raycast(transform.position, Quaternion.Inverse(moveRayRot) * movingDirection, moveRayDist) == null
            && CustomPhysics.Raycast(transform.position, moveRayRot * movingDirection, moveRayDist) == null) {
            root.transform.position += movingDirection * setting.MoveSpeed * Time.deltaTime;
        }

#if UNITY_EDITOR
        Debug.DrawLine(transform.position, transform.position + transform.forward);
        Debug.DrawRay(transform.position, movingDirection * moveRayDist, Color.magenta);
        Debug.DrawRay(transform.position, moveRayRot * movingDirection * moveRayDist, Color.magenta);
        Debug.DrawRay(transform.position, Quaternion.Inverse(moveRayRot) * movingDirection * moveRayDist, Color.magenta);
#endif
    }

}
