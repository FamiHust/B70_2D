using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom PropertyDrawer cho ItemData — hiển thị "name" của item
/// thay vì "Element 0, 1, 2..." trong Inspector.
/// </summary>
[CustomPropertyDrawer(typeof(ItemsCollection.ItemData))]
public class ItemDataDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty nameProp = property.FindPropertyRelative("name");
        string displayName = (nameProp != null && !string.IsNullOrEmpty(nameProp.stringValue))
            ? nameProp.stringValue
            : label.text; // fallback về "Element N" nếu name rỗng

        label = new GUIContent(displayName, label.tooltip);
        EditorGUI.PropertyField(position, property, label, true);
    }
}
