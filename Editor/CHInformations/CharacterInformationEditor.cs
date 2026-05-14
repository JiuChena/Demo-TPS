// CharacterInformationEditor.cs (注意：文件名通常与要编辑的类名一致，加上 "Editor" 后缀)
using UnityEngine;
using UnityEditor;

// [CustomEditor] 属性告诉 Unity 这个脚本是用来编辑哪个目标类型的
[CustomEditor(typeof(CharacterAssetInforamtion))]
public class CharacterInformationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 调用 base.OnInspectorGUI() 来绘制 ScriptableObject 中定义的所有默认属性
        DrawDefaultInspector();

        // 获取当前正在编辑的 ScriptableObject 实例
        CharacterAssetInforamtion targetSO = (CharacterAssetInforamtion)target;

        // 添加一个间隔，使按钮与上面的属性分开
        EditorGUILayout.Space();

        // 使用 GUILayout.Button 或 EditorGUILayout.Button 来创建按钮
        if (GUILayout.Button("快速初始化"))
        {
            // 当按钮被点击时，调用目标脚本中的函数
            targetSO.DataInitialize();
        }
    }
}