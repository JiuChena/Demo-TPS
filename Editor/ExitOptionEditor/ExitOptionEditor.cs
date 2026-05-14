using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ToolbarPanel))]
public class ExitOptionEditor : Editor
{
    private SerializedProperty quitButtonProp;
    private SerializedProperty characterButtonProp;
    private SerializedProperty bagButtonProp;
    private SerializedProperty storeButtonProp;
    private SerializedProperty teamButtonProp;
    private SerializedProperty taskButtonProp;
    
    private SerializedProperty modeProp;
    private SerializedProperty textProp;

    private void OnEnable()
    {
        quitButtonProp = serializedObject.FindProperty("quitButton");
        characterButtonProp = serializedObject.FindProperty("characterButton");
        bagButtonProp = serializedObject.FindProperty("bagButton");
        storeButtonProp = serializedObject.FindProperty("storeButton");
        teamButtonProp = serializedObject.FindProperty("teamButton");
        taskButtonProp = serializedObject.FindProperty("taskButton");
        
        modeProp = serializedObject.FindProperty("exitOption");
        textProp = serializedObject.FindProperty("designedSceneName");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((ToolbarPanel)target), typeof(MonoScript), false);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.PropertyField(quitButtonProp, new GUIContent("退出按钮"));
        EditorGUILayout.PropertyField(characterButtonProp, new GUIContent("角色按钮"));
        EditorGUILayout.PropertyField(bagButtonProp, new GUIContent("背包按钮"));
        EditorGUILayout.PropertyField(storeButtonProp, new GUIContent("商店按钮"));
        EditorGUILayout.PropertyField(teamButtonProp, new GUIContent("编队按钮"));
        EditorGUILayout.PropertyField(taskButtonProp, new GUIContent("任务按钮"));

        // 绘制 mode 枚举
        EditorGUILayout.PropertyField(modeProp);

        // 仅当 mode == MyEnum.ShowField 时，才显示文本框
        if (modeProp.enumValueIndex == (int)ExitOption.ExitToDesignatedScene)
        {
            EditorGUILayout.PropertyField(textProp, new GUIContent("指定切换场景的场景名称"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
