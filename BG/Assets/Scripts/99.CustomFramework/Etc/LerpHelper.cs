using UnityEngine;

public static class LerpHelper {
    public static void Lerp(ref float targetV, float dest, float scale = 1F) {
        float dt = Time.deltaTime * scale;
        if (Mathf.Abs(dest - targetV) < dt) {
            targetV = dest;
            return;
        }

        if (targetV < dest) {
            targetV += dt;
        }
        else {
            targetV -= dt;
        }
    }

    public static void Lerp(ref Vector3 targetV, Vector3 dest, float scale = 1F) {
        float dt = Time.deltaTime * scale;
        Vector3 dtV = dest - targetV;
        if (dtV.magnitude <= 0.001F) {
            targetV = dest;
            return;
        }

        targetV += dtV * dt;
    }
}