using UnityEngine;

public struct EnemyAIData
{
    public bool targetExist;
    public Vector3 targetPos;
    public Vector3 startPos;
    
    public bool stateOccupy;
    public bool moveAllowed;
    
    public bool JCAllowed;
    public Vector3 JCDir;

    public bool jump;
    
    public bool death;
    
    public Vector3 moveDir;
    public Vector3 navMoveDir;
    
    public Vector3 unitToBunkerDir;

    public void ResetState()
    {
        targetExist = false;
        targetPos = Vector3.zero;
        startPos = Vector3.zero;
        stateOccupy = false;
        moveAllowed = true;
        JCAllowed = false;
        JCDir = Vector3.zero;
        moveDir = Vector3.zero;
        navMoveDir = Vector3.zero;
        unitToBunkerDir = Vector3.zero;
        jump = false;
        death = false;
    }
}
