using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomFramework;

public class PlayerWeaponControl : CustomBehaviour {

    public enum EWeaponType {
        NONE,
        RIFLE,
        CANNON
    }

    /*[SerializeField]*/ Transform[] weapons;
    [SerializeField] PlayerCameraGimbal gimbal;
    [SerializeField] Transform root;
    [SerializeField] Transform shotPoint;
    public Transform ShotPosition => shotPoint;

    [SerializeField] LineRenderer shotTrail;

    [Space(7), SerializeField] Transform[] reboundingBones;
    Vector3[] cachedBonesPosition = null;
    Vector3 reboundVector = Vector3.zero;

    [Space(5), SerializeField] float shotDelay = 0.2f;
    float currentShotDelay = 0F;

    EWeaponType equippedWeapon = EWeaponType.NONE;
    Transform currentWeapon {
        get {
            if (equippedWeapon == EWeaponType.NONE) return null;
            return weapons[(int)equippedWeapon - 1];
        }
    }

    [SerializeField] bool canShoot = true, canControlRotate = true, canControlRebound = true;

    CSequence sequence = null;

    CAction trailAction = null;


    void EquipWeapon(EWeaponType type) {
        if (!canShoot) return;
        if (type == equippedWeapon) return;

        if (sequence == null) sequence = CSequence.Create();
        sequence.Clear();
        sequence.OnStart(() => { canShoot = false; });
        sequence.OnComplete(() => { canShoot = true; });

        if (equippedWeapon != EWeaponType.NONE) {
            sequence.Append(CLocalRotateTo.Create(weapons[(int)equippedWeapon - 1], Vector3.zero, 0.1f, EEaseAction.OUT))
                .Join(CLocalMoveTo.Create(transform, Vector3.zero, 0.5f, EEaseAction.INOUT))
                .AppendCallback(() => { 
                    weapons[(int)equippedWeapon - 1].gameObject.SetActive(false);
                    equippedWeapon = type;
                })
                .AppendInterval(0.5f);
        }

        if (type != EWeaponType.NONE) {
            sequence.AppendCallback(() => { 
                    weapons[(int)type - 1].gameObject.SetActive(true);
                    equippedWeapon = type;
                })
                .Append(CLocalMoveTo.Create(transform, Vector3.right * 0.75f, 0.5f, EEaseAction.OUT));
        }

        CAction.Play(sequence);
    }

    public override void OnStart() {
        //EquipWeapon(EWeaponType.RIFLE);
    }

    public override void OnFixedUpdate() {
        ControlRebound();
    }

    public override void OnUpdate() {
        if (Input.GetMouseButton(0) && canShoot && currentShotDelay <= 0F) {
            if (trailAction == null) trailAction = CDelay.Create(0.05f).OnStart(() => { shotTrail.enabled = true; }).OnComplete(() => { shotTrail.enabled = false; });
            CAction.Play(trailAction);

            var hit = CustomPhysics.Raycast(shotPoint.position, shotPoint.forward, 100F);
            if (hit != null) {
                HandleHit(hit);
            }

            reboundVector = Vector3.back * 0.01f;
            currentShotDelay = shotDelay;

            gimbal.ReboundCamera(Random.insideUnitCircle * 0.015f, 10F);
        }
    }

    public override void OnLateUpdate() {
        //if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(EWeaponType.RIFLE);
        //if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(EWeaponType.CANNON);
        //if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(EWeaponType.NONE);
        ControlRotate();
    }


    void ControlRotate() {
        if (!canControlRotate) return;

        float radian = 90F - gimbal.ElevationValue;

        Quaternion origin = transform.rotation;
        transform.rotation *= Quaternion.Inverse(transform.rotation);
        transform.eulerAngles = root.right * radian;    
        transform.rotation *= origin;
    }

    void ControlRebound() {
        if (!canControlRebound) return;

        if (cachedBonesPosition == null) {
            cachedBonesPosition = new Vector3[reboundingBones.Length];
            for (int i = 0; i < reboundingBones.Length; ++i) {
                cachedBonesPosition[i] = reboundingBones[i].localPosition;
            }
        }

        for (int i = 0; i < reboundingBones.Length; ++i) {
            reboundingBones[i].localPosition = cachedBonesPosition[i] + reboundingBones[i].localRotation * reboundVector;
        }

        LerpHelper.Lerp(ref reboundVector, Vector3.zero, 12F);

        LerpHelper.Lerp(ref currentShotDelay, 0F);
    }


    void HandleHit(CustomPhysics.RaycastHit hit) {
#if UNITY_EDITOR
        Debug.LogError(hit.target.name);
#endif

        var component = hit.target.GetComponent<ITargetInterface>();
        if (component != null) {
            component.OnHit();
        }
    }
}
