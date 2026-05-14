using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Configurations/Tasks/New Task", menuName = "ScriptableObjects/Task/New Task")]
public class GameTaskConfig : ScriptableObject
{
    public string taskName;
    public string taskDescription;
    public string taskRewardDescription;
    public string taskListenerFuncName;
    public string taskListDisplayFuncName;
}
