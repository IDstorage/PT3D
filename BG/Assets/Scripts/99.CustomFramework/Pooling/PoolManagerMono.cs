using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManagerMono : CustomBehaviour {

    [SerializeField] PoolObject[] prefabs;

    void OnActivate() {
        PoolManager.Instance.Init(prefabs);
    }

}
