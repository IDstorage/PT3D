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

    [SerializeField] Transform[] weapons;
    [SerializeField] PlayerCameraGimbal gimbal;

    EWeaponType equippedWeapon = EWeaponType.NONE;
    Transform currentWeapon {
        get {
            if (equippedWeapon == EWeaponType.NONE) return null;
            return weapons[(int)equippedWeapon - 1];
        }
    }

    bool canControl = true, canShoot = false;

    CSequence sequence = null;

    void EquipWeapon(EWeaponType type) {
        if (!canControl) return;

        if (sequence == null) sequence = CSequence.Create();
        sequence.Clear();
        sequence.OnStart(() => { canControl = false; });
        sequence.OnComplete(() => { canControl = true; });

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
        EquipWeapon(EWeaponType.RIFLE);
    }

    public override void OnUpdate() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(EWeaponType.RIFLE);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(EWeaponType.CANNON);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(EWeaponType.NONE);

        ControlRotate();
    }

    void ControlRotate() {
        if (!canControl || currentWeapon == null) return;

        float radian = (90F - gimbal.ElevationValue) * Mathf.Deg2Rad;

        
    }

}
