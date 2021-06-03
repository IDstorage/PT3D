using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomFramework;

public class CustomBoxCollider : CustomCollider {

    public Vector3 size = Vector3.one;
    [System.NonSerialized] public Vector3[] axis = new Vector3[3];


#if UNITY_EDITOR
    Color boundaryColor = Color.green;
#endif


    public void UpdateAxis() {
        axis[0] = transform.right;
        axis[1] = transform.up;
        axis[2] = transform.forward;
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = boundaryColor;
        Gizmos.matrix = Matrix4x4.TRS(Center, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
    }
#endif

    public override void OnCollidedEnter(CustomCollider other) {
        base.OnCollidedEnter(other);

#if UNITY_EDITOR
        boundaryColor = Color.red;
#endif
    }
    public override void OnCollidedEnd(CustomCollider other) {
        base.OnCollidedEnd(other);

#if UNITY_EDITOR
        boundaryColor = Color.green;
#endif
    }



    public override bool Collide(CustomCollider other) {
        if (!(other is CustomBoxCollider)) return false;
        return Collide(this, other as CustomBoxCollider);
    }

    public static bool Collide(CustomBoxCollider a, CustomBoxCollider b) {
        a.UpdateAxis();
        b.UpdateAxis();

        float[,] cr = new float[3, 3];
        float[] tp = new float[3];
        float d = 0F, r1 = 0F, r2 = 0F;

        float ae0 = a.size.x, ae1 = a.size.y, ae2 = a.size.z;
        float be0 = b.size.x, be1 = b.size.y, be2 = b.size.z;

        Vector3 t = b.Center - a.Center;

        // a.axis[0]
        cr[0, 0] = Vector3.Dot(a.axis[0], b.axis[0]);
        cr[0, 1] = Vector3.Dot(a.axis[0], b.axis[1]);
        cr[0, 2] = Vector3.Dot(a.axis[0], b.axis[2]);
        tp[0] = Vector3.Dot(a.axis[0], t);

        d = Mathf.Abs(tp[0]);
        r1 = ae0;
        r2 = be0 * Mathf.Abs(cr[0, 0]) + be1 * Mathf.Abs(cr[0, 1]) + be2 * Mathf.Abs(cr[0, 2]);
        if (d > (r1 + r2) / 2F) return false;


        // a.axis[1]
        cr[1, 0] = Vector3.Dot(a.axis[1], b.axis[0]);
        cr[1, 1] = Vector3.Dot(a.axis[1], b.axis[1]);
        cr[1, 2] = Vector3.Dot(a.axis[1], b.axis[2]);
        tp[1] = Vector3.Dot(a.axis[1], t);

        d = Mathf.Abs(tp[1]);
        r1 = ae1;
        r2 = be0 * Mathf.Abs(cr[1, 0]) + be1 * Mathf.Abs(cr[1, 1]) + be2 * Mathf.Abs(cr[1, 2]);
        if (d > (r1 + r2) / 2F) return false;


        // a.axis[2]
        cr[2, 0] = Vector3.Dot(a.axis[2], b.axis[0]);
        cr[2, 1] = Vector3.Dot(a.axis[2], b.axis[1]);
        cr[2, 2] = Vector3.Dot(a.axis[2], b.axis[2]);
        tp[2] = Vector3.Dot(a.axis[2], t);

        d = Mathf.Abs(tp[2]);
        r1 = ae2;
        r2 = be0 * Mathf.Abs(cr[2, 0]) + be1 * Mathf.Abs(cr[2, 1]) + be2 * Mathf.Abs(cr[2, 2]);
        if (d > (r1 + r2) / 2F) return false;


        // b.axis[0]
        d = Mathf.Abs(Vector3.Dot(b.axis[0], t));
        r1 = ae0 * Mathf.Abs(cr[0, 0]) + ae1 * Mathf.Abs(cr[1, 0]) + ae2 * Mathf.Abs(cr[2, 0]);
        r2 = be0;
        if (d > (r1 + r2) / 2F) return false;


        // b.axis[1]
        d = Mathf.Abs(Vector3.Dot(b.axis[1], t));
        r1 = ae0 * Mathf.Abs(cr[0, 1]) + ae1 * Mathf.Abs(cr[1, 1]) + ae2 * Mathf.Abs(cr[2, 1]);
        r2 = be1;
        if (d > (r1 + r2) / 2F) return false;


        // b.axis[2]
        d = Mathf.Abs(Vector3.Dot(b.axis[2], t));
        r1 = ae0 * Mathf.Abs(cr[0, 2]) + ae1 * Mathf.Abs(cr[1, 2]) + ae2 * Mathf.Abs(cr[2, 2]);
        r2 = be2;
        if (d > (r1 + r2) / 2F) return false;


        // a.axis[0] * b.axis[0]
        d = Mathf.Abs(tp[2] * cr[1, 0] - tp[1] * cr[2, 0]);
        r1 = ae1 * Mathf.Abs(cr[2, 0]) + ae2 * Mathf.Abs(cr[1, 0]);
        r2 = be1 * Mathf.Abs(cr[0, 2]) + be2 * Mathf.Abs(cr[0, 1]);
        if (d > (r1 + r2) / 2F) return false;


        // a.axis[0] * b.axis[1]
        d = Mathf.Abs(tp[2] * cr[1, 1] - tp[1] * cr[2, 1]);
        r1 = ae1 * Mathf.Abs(cr[2, 1]) + ae2 * Mathf.Abs(cr[1, 1]);
        r2 = be0 * Mathf.Abs(cr[0, 2]) + be2 * Mathf.Abs(cr[0, 0]);
        if (d > (r1 + r2) / 2F) return false;


        // a.axis[0] * b.axis[2]
        d = Mathf.Abs(tp[2] * cr[1, 2] - tp[1] * cr[2, 2]);
        r1 = ae1 * Mathf.Abs(cr[2, 2]) + ae2 * Mathf.Abs(cr[1, 2]);
        r2 = be2 * Mathf.Abs(cr[0, 1]) + be1 * Mathf.Abs(cr[0, 0]);
        if (d > (r1 + r2) / 2F) return false;


        // a.axis[1] * b.axis[0]
        d = Mathf.Abs(tp[0] * cr[2, 0] - tp[2] * cr[0, 0]);
        r1 = ae0 * Mathf.Abs(cr[2, 0]) + ae2 * Mathf.Abs(cr[0, 0]);
        r2 = be1 * Mathf.Abs(cr[1, 2]) + be2 * Mathf.Abs(cr[1, 1]);
        if (d > (r1 + r2) / 2F) return false;


        // a.axis[1] * b.axis[1]
        d = Mathf.Abs(tp[0] * cr[2, 1] - tp[2] * cr[0, 1]);
        r1 = ae0 * Mathf.Abs(cr[2, 1]) + ae2 * Mathf.Abs(cr[0, 1]);
        r2 = be0 * Mathf.Abs(cr[1, 2]) + be2 * Mathf.Abs(cr[1, 0]);
        if (d > (r1 + r2) / 2F) return false;


        // a.axis[1] * b.axis[2]
        d = Mathf.Abs(tp[0] * cr[2, 2] - tp[2] * cr[0, 2]);
        r1 = ae0 * Mathf.Abs(cr[2, 2]) + ae2 * Mathf.Abs(cr[0, 2]);
        r2 = be0 * Mathf.Abs(cr[1, 1]) + be1 * Mathf.Abs(cr[1, 0]);
        if (d > (r1 + r2) / 2F) return false;


        // a.axis[2] * b.axis[0]
        d = Mathf.Abs(tp[1] * cr[0, 0] - tp[0] * cr[1, 0]);
        r1 = ae0 * Mathf.Abs(cr[1, 0]) + ae1 * Mathf.Abs(cr[0, 0]);
        r2 = be1 * Mathf.Abs(cr[2, 2]) + be2 * Mathf.Abs(cr[2, 1]);
        if (d > (r1 + r2) / 2F) return false;


        // a.axis[2] * b.axis[1]
        d = Mathf.Abs(tp[1] * cr[0, 1] - tp[0] * cr[1, 1]);
        r1 = ae0 * Mathf.Abs(cr[1, 1]) + ae1 * Mathf.Abs(cr[0, 1]);
        r2 = be0 * Mathf.Abs(cr[2, 2]) + be2 * Mathf.Abs(cr[2, 0]);
        if (d > (r1 + r2) / 2F) return false;


        // a.axis[2] * b.axis[2]
        d = Mathf.Abs(tp[1] * cr[0, 2] - tp[0] * cr[1, 2]);
        r1 = ae0 * Mathf.Abs(cr[1, 2]) + ae1 * Mathf.Abs(cr[0, 2]);
        r2 = be0 * Mathf.Abs(cr[2, 1]) + be1 * Mathf.Abs(cr[2, 0]);
        if (d > (r1 + r2) / 2F) return false;

        return true;
    }

    public static bool Collide(CustomBoxCollider a, CustomSphereCollider b) {
        return true;
    }
}
