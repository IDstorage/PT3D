using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSphereCollider : CustomCollider {

    public float radius = 1F;


#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.TRS(Center, transform.rotation, Vector3.one);
        Gizmos.DrawWireSphere(Vector3.zero, radius);
    }
#endif

}
