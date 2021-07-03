using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : CustomBehaviour {

    [SerializeField] PlayerCameraGimbal gimbal;
    [SerializeField] PlayerWeaponControl weapon;

    RectTransform rt;

    void OnUpdate() {
        if (rt == null) rt = transform as RectTransform;
        var converted = gimbal.PlayerCamera.WorldToScreenPoint(weapon.ShotPosition.position + weapon.ShotPosition.forward * 15F);
        rt.anchoredPosition = converted;
    }

}
