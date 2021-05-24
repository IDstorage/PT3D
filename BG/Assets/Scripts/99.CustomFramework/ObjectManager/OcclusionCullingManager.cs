using UnityEngine;

namespace CustomFramework {

    public static class OcclusionCullingManager {

        public static Camera targetCamera { get; set; }

        static float threshold = Mathf.Cos(90F * Mathf.Deg2Rad);

        public static bool Culling(CullingBehaviour obj, Camera cam = null) {
            if (targetCamera == null) {
                if (cam == null) return false;
                targetCamera = cam;
            }
            if (ReferenceEquals(targetCamera, cam) == false) {
                targetCamera = cam;
            }

            var list = obj.GetFourEdges();
            //var list = new Vector3[] { obj.transform.position };
            for (int i = 0; i < list.Length; ++i) {
                if (Vector3.Dot(targetCamera.transform.forward, (targetCamera.transform.forward - list[i]).normalized) > threshold) continue;
                return true;
            }

            return false;
        }

    }

}