using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCollider : CustomBehaviour {

    public Vector3 center;

    public Vector3 Center {
        get {
            return transform.position + transform.right * center.x + transform.up * center.y + transform.forward * center.z;
        }
    }

}
