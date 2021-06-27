using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CustomFramework;
using CustomFramework.Extension;

public class PlayerBodyControl : CustomBehaviour {

    [SerializeField] PlayerProfile setting;

    [SerializeField, Space(10)] CustomBehaviour root;
    [SerializeField] PlayerCameraGimbal gimbal;

    [SerializeField] CustomBehaviour top, middle, bottom;

    [SerializeField] Animator animator;

    public CustomBehaviour Top => top;
    public CustomBehaviour Middle => middle;
    public CustomBehaviour Bottom => bottom;

    Vector3 forwardVector, movingDirection;
    float gravityValue = 0F;
    bool canControl = true;

    float btSpeed = 0F, btDir = 0F;

    Quaternion moveRayRot = Quaternion.Euler(0F, 45F, 0F);
    float moveRayDist = Mathf.Sqrt(2F) * 0.5f + 0.1f;

    public override void OnUpdate() {
        float radian = (-gimbal.AzimuthValue - 90F) * Mathf.Deg2Rad;

        forwardVector.x = Mathf.Cos(radian);
        forwardVector.y = 0F;
        forwardVector.z = Mathf.Sin(radian);

        if (!Input.GetKeyDown(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.LeftAlt) && !Input.GetKeyUp(KeyCode.LeftAlt) && canControl) {
            transform.localRotation = Quaternion.LookRotation(forwardVector);
        }

        float hSpeed = Input.GetAxisRaw("Horizontal");
        float vSpeed = Input.GetAxisRaw("Vertical");

        LerpHelper.Lerp(ref btSpeed, vSpeed, 5F);
        LerpHelper.Lerp(ref btDir, hSpeed, 5F);

        animator.SetFloat("Speed", btSpeed);
        animator.SetFloat("Direction", btDir);

        //if (Input.GetKeyDown(KeyCode.Space)) {
        //    gravityValue = -setting.JumpPower;
        //}
        //else if (CustomPhysics.Raycast(transform.position, -transform.up, 0.5f) == null) {
        //    gravityValue += 9.8f * setting.GravityScale * Time.deltaTime;
        //    if (gravityValue > 9.8f) gravityValue = 9.8f;
        //}
        //else {
        //    gravityValue = 0F;
        //}

        movingDirection = (transform.forward * vSpeed + transform.right * hSpeed).normalized;

        if (CustomPhysics.Raycast(transform.position, movingDirection, moveRayDist) == null
            && CustomPhysics.Raycast(transform.position, Quaternion.Inverse(moveRayRot) * movingDirection, moveRayDist) == null
            && CustomPhysics.Raycast(transform.position, moveRayRot * movingDirection, moveRayDist) == null
            && canControl) {
            root.transform.position += movingDirection * setting.MoveSpeed * Time.deltaTime;
        }

        if (canControl) root.transform.position += -transform.up * gravityValue * Time.deltaTime;

        //UpdateGravityWithSurface();

#if UNITY_EDITOR
        Debug.DrawLine(transform.position, transform.position + transform.forward);
        Debug.DrawRay(transform.position, movingDirection * moveRayDist, Color.magenta);
        Debug.DrawRay(transform.position, moveRayRot * movingDirection * moveRayDist, Color.magenta);
        Debug.DrawRay(transform.position, Quaternion.Inverse(moveRayRot) * movingDirection * moveRayDist, Color.magenta);
#endif
    }


    //    void UpdateGravityWithSurface() {
    //        var ground = SquarePlanet.Instance.transform;

    //        Vector3 v = root.transform.position - ground.position;
    //        v.x = Mathf.Clamp(v.x, -ground.lossyScale.x * 0.5f, ground.lossyScale.x * 0.5f);
    //        v.y = Mathf.Clamp(v.y, -ground.lossyScale.y * 0.5f, ground.lossyScale.y * 0.5f);
    //        v.z = Mathf.Clamp(v.z, -ground.lossyScale.z * 0.5f, ground.lossyScale.z * 0.5f);

    //        v += ground.position;

    //        Vector3 vu = root.transform.position - v;
    //        vu.x *= Mathf.Abs(transform.up.x);
    //        vu.y *= Mathf.Abs(transform.up.y);
    //        vu.z *= Mathf.Abs(transform.up.z);

    //        Vector3 dir = root.transform.position - v - vu;

    //#if UNITY_EDITOR
    //        Debug.DrawRay(v, transform.up, Color.magenta);
    //        Debug.DrawRay(v, dir.normalized, Color.cyan);
    //#endif

    //        if (Input.GetKeyDown(KeyCode.Space)) {
    //            canControl = false;
    //            Debug.LogError(Quaternion.FromToRotation(transform.up, dir.normalized).eulerAngles);
    //            StartCoroutine(
    //                CoRotate(v,
    //                        Quaternion.FromToRotation((root.transform.position - v).normalized, dir.normalized),
    //                        Quaternion.FromToRotation(transform.up, dir.normalized)));
    //        }

    //        //if (CustomPhysics.Raycast())
    //    }


    //    IEnumerator CoRotate(Vector3 origin, Quaternion dRot, Quaternion rot) {
    //        float time = 0F;
    //        float duration = 1F;

    //        Vector3 converted = root.transform.position - origin;

    //        Quaternion qa = Quaternion.identity;
    //        Quaternion qb = dRot;

    //        Vector3 oa = root.transform.rotation.eulerAngles;
    //        Vector3 ob = oa + rot.eulerAngles;

    //        while (time < duration) {
    //            root.transform.position = Quaternion.Slerp(qa, qb, time / duration) * converted + origin;
    //            //root.transform.rotation = Quaternion.Slerp(ra, rb, time / duration);
    //            time += Time.deltaTime;
    //            yield return null;
    //        }

    //        root.transform.position = qb * converted + origin;
    //        root.transform.rotation = Quaternion.Euler(ob);

    //        //root.transform.rotation = transform.rotation * rot;

    //        canControl = true;
    //    }

}
