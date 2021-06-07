using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCollider : CustomBehaviour {

    void Awake() {
        CustomFramework.CustomPhysics.Add(this);
    }

    [SerializeField] bool ignoreRaycast = false;
    public bool IgnoreRaycast => ignoreRaycast;

    [SerializeField] bool isDynamic = false;
    public bool IsDynamic {
        get {
            return isDynamic;
        }
        set {
            if (isDynamic != value)
                CustomFramework.CustomPhysics.UpdateState(this, value);
            isDynamic = value;
        }
    }

    [SerializeField] Vector3 center;
    public Vector3 Center {
        get {
            return transform.position + transform.right * center.x + transform.up * center.y + transform.forward * center.z;
        }
    }

    public Vector3 size = Vector3.one;

    [System.NonSerialized] public Vector3[] axis = new Vector3[3];


    protected CustomFramework.CustomPhysics.PosIndex index = new CustomFramework.CustomPhysics.PosIndex();
    public CustomFramework.CustomPhysics.PosIndex Index => index;

    public virtual void UpdateIndex() { }


    public HashSet<CustomCollider> collisionList = new HashSet<CustomCollider>();

    public virtual void OnCollidedEnter(CustomCollider other) { }
    public virtual void OnCollidedStay(CustomCollider other) { }
    public virtual void OnCollidedEnd(CustomCollider other) { }



    public virtual bool Collide(CustomCollider other) {
        return false;
    }

}
