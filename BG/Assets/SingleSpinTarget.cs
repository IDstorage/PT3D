using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomFramework;
using CustomFramework.Extension;

public interface ITargetInterface {
    void OnHit();
}

public class SingleSpinTarget : CustomBoxCollider, ITargetInterface {

    CAction action;

    bool canExecuteHitAnimation = true;

    [SerializeField] bool isHorizontal = false;

    public void OnHit() {
        if (!canExecuteHitAnimation) return;
        if (action == null) {
            action = CRotateBy.Create(transform, isHorizontal ? transform.right * 180F : transform.up * 180F, 0.4f)
            .OnStart(() => { canExecuteHitAnimation = false; })
            .OnComplete(() => { canExecuteHitAnimation = true; });
        }
        CAction.Play(action);
    }

}
