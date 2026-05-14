using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Configurations/Tasks/PreLoad/New Tasks Table", menuName = "ScriptableObjects/Task/New Tasks Table")]
public class PreLoadTaskLuaScriptsTable : ScriptableObject
{
    public List<string> luaScriptNames = new List<string>();
}
