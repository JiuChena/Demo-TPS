using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorDriverBase : MonoBehaviour
{
    public bool stateDebug = false;
    [HideInInspector] public PlayerInputData inputData = new PlayerInputData();
    [HideInInspector] public Animator animator;
    [HideInInspector] public bool death = false;
    public HSM hsm;

    public float bulletSpeed = 50f;
}
