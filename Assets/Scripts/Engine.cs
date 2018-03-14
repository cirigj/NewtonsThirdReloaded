using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour {

    [Header("Engine Stats")]
    public float thrust;
    public float overheatTime;
    public float overheatRate;
    public float overheatDamage;
    public float kickbackMitigation;

    [Header("Runtime")]
    public float overheat;

}
