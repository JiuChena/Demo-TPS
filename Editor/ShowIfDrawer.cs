using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = (ShowIfAttribute)attribute;
        SerializedProperty conditionProp = GetConditionProperty(property, attr.conditionField);
        
        if (conditionProp != null && conditionProp.intValue == attr.conditionValue)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var attr = (ShowIfAttribute)attribute;
        SerializedProperty conditionProp = GetConditionProperty(property, attr.conditionField);
        
        return (conditionProp != null && conditionProp.intValue == attr.conditionValue) 
            ? EditorGUI.GetPropertyHeight(property, label, true) 
            : -2;
    }

    // 🔑 核心修复：精准定位【同级】条件字段（支持嵌套类/数组）
    private SerializedProperty GetConditionProperty(SerializedProperty prop, string conditionFieldName)
    {
        string currentPath = prop.propertyPath;
        string conditionPath = BuildSiblingPath(currentPath, conditionFieldName);
        return prop.serializedObject.FindProperty(conditionPath);
    }

    // 构建同级字段路径（例：skillData.radius → skillData.skillArea）
    private string BuildSiblingPath(string currentPath, string siblingName)
    {
        // 处理数组元素: ...Array.data[3].radius → ...Array.data[3].skillArea
        if (currentPath.Contains(".Array.data["))
        {
            int arrayEnd = currentPath.LastIndexOf(']');
            if (arrayEnd > 0)
            {
                string prefix = currentPath.Substring(0, arrayEnd + 1); // 保留到 ]
                return prefix + "." + siblingName;
            }
        }
        
        // 普通嵌套类: characterData.skillData.radius → characterData.skillData.skillArea
        int lastDot = currentPath.LastIndexOf('.');
        if (lastDot > 0)
        {
            string parentPath = currentPath.Substring(0, lastDot);
            return parentPath + "." + siblingName;
        }
        
        // 根级字段（罕见）
        return siblingName;
    }
}