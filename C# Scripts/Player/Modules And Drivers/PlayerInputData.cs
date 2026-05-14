using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInputData
{
    /// <summary>
    /// 状态锁定
    /// </summary>
    public bool stateLocked;
    
    public Vector3 moveDirection;
    
    public bool talent;
    
    public bool burst;

    public Vector3 attackDir;
    
    public bool attack;
    
    public bool reload;
    
    public bool isAllowedCrouchOrJump;
    public Vector3 crouchJumpDir;
    
    public bool crouch;
    
    public bool jump;

    public bool death;
    
    public bool indicating;

    public void ResetInput()
    {
        stateLocked = false;
        moveDirection = Vector3.zero;
        burst = false;
        talent = false;
        attack = false;
        reload = false;
        isAllowedCrouchOrJump = false;
        jump = false;
        crouch = false;
        crouchJumpDir = Vector3.zero;
        
        death = false;
        indicating = false;
    }
}
