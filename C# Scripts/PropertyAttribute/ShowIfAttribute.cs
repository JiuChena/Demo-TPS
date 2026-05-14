// ShowIfAttribute.cs
using UnityEngine;

public class ShowIfAttribute : PropertyAttribute
{
    public string conditionField;
    public int conditionValue;
    
    public ShowIfAttribute(string fieldName, object enumValue)
    {
        conditionField = fieldName;
        conditionValue = enumValue is int i ? i : (int)System.Convert.ChangeType(enumValue, typeof(int));
    }
}