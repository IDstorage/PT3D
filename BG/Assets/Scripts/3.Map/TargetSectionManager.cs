using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomFramework;
using CustomFramework.Extension;

public class TargetSectionManager : CustomBehaviour {

    [SerializeField] Transform[] targets;
    [SerializeField] int index = 0;

    void OnStart() {
        if (index == 3) Section3();
    }

    void Section3() {
        transform.RotateBy(transform.forward * 360F, 3F, true);
    }

}
