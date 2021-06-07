using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CustomFramework;
using CustomFramework.Extension;

public class PlayerBodyControl : CustomBehaviour {

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

    public override void OnUpdate() {
        if (Input.GetKeyDown(KeyCode.V)) {
            CustomPhysics.Raycast(transform.position, transform.forward, 5F);
            Debug.DrawRay(transform.position, transform.forward * 100F, Color.red, 2.0f);
            //Debug.DrawLine(transform.position, transform.position + transform.forward * 5F, Color.red, 1.0f);
        }

        float radian = (-gimbal.AzimuthValue - 90F) * Mathf.Deg2Rad;

        forwardVector.x = Mathf.Cos(radian);
        forwardVector.y = 0F;
        forwardVector.z = Mathf.Sin(radian);

        if (!Input.GetKeyDown(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.LeftAlt) && !Input.GetKeyUp(KeyCode.LeftAlt)) {
            transform.rotation = Quaternion.LookRotation(forwardVector);
        }

        if (controller == null) return;

        float hSpeed = Input.GetAxisRaw("Horizontal");
        float vSpeed = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space)) {
            gravityValue = -setting.JumpPower;
        }
        else if (!controller.isGrounded) {
            gravityValue += 9.8f * setting.GravityScale * Time.deltaTime;
            if (gravityValue > 9.8f) gravityValue = 9.8f;
        }
        else {
            gravityValue = 0F;
        }

        movingDirection = (transform.forward * vSpeed + transform.right * hSpeed).normalized + transform.up * -gravityValue;
        
        controller.Move(movingDirection * setting.MoveSpeed * Time.deltaTime);

#if UNITY_EDITOR
        Debug.DrawLine(transform.position, transform.position + transform.forward);
#endif
    }

    bool Collision(out RaycastHit hit) {
#if UNITY_EDITOR
        Debug.DrawLine(transform.position, transform.position + movingDirection);
#endif

        return Physics.Raycast(transform.position, movingDirection, out hit, 0.5f * Mathf.Sqrt(2F));
    }

}
