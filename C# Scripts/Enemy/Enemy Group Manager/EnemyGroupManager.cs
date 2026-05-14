using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroupManager : MonoBehaviour
{
    public SphereCollider managerCollider;
    public List<GameObject> enemies;
    
    private bool getTarget = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            getTarget = true;

            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
