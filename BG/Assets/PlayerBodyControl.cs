using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyControl : CustomBehaviour {

    [SerializeField] CustomBehaviour top, middle, bottom;

    public CustomBehaviour Top => top;
    public CustomBehaviour Middle => middle;
    public CustomBehaviour Bottom => bottom;

}
