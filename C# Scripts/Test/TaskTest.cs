using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskTest : MonoBehaviour
{
    private void Start()
    {
        //任务推送
        // TaskSystem.Instance.InsertTask("Fight Task 1");
        // TaskSystem.Instance.InsertTask("Interaction Task 1");
        // TaskSystem.Instance.InsertTask("Money Task 1");
        // TaskSystem.Instance.InsertTask("Store Task 1");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TaskSystem.Instance.SetTaskTrigger("Store Task 1", 100);
        }
    }
}
