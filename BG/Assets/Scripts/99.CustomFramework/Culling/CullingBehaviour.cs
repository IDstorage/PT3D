using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CullingBehaviour : CustomBehaviour {
    [SerializeField] Camera cam;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer renderer;

    public Vector3[] GetFourEdges() {
        if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();

        Vector3 min = meshFilter.mesh.bounds.min, max = meshFilter.mesh.bounds.max;
        min.x *= transform.localScale.x;
        min.y *= transform.localScale.y;
        min.z *= transform.localScale.z;
        max.x *= transform.localScale.x;
        max.y *= transform.localScale.y;
        max.z *= transform.localScale.z;

        Vector3 pos = transform.position;

        return new Vector3[] {
            pos + min,
            pos + max,
            pos + new Vector3(min.x, min.y, max.z),
            pos + new Vector3(min.x, max.y, min.z),
            pos + new Vector3(max.x, min.y, min.z),
            pos + new Vector3(min.x, max.y, max.z),
            pos + new Vector3(max.x, min.y, max.z),
            pos + new Vector3(max.x, max.y, min.z),
        };
    }

    public override void OnUpdate() {
    //public void Update() {
        //gameObject.SetActive(CustomFramework.OcclusionCullingManager.Culling(this, cam));
        if (renderer == null) renderer = GetComponent<MeshRenderer>();
        renderer.enabled = CustomFramework.OcclusionCullingManager.Culling(this, cam);
    }

    //public override void OnISOUpdate() {
    //    OnUpdate();
    //}


    //public override void OnActivate() {
    //    Debug.LogError("등장!");
    //}
}
