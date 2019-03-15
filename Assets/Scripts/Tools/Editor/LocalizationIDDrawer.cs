using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(LocalizationIdAttribute))]
public class LocalizationIDDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property) * 2;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        if (property.propertyType == SerializedPropertyType.String)
        {
            LocalizationIdAttribute attr = attribute as LocalizationIdAttribute;
            //GUILayout.Button(property.stringValue);
            if (attr.DisplayMenu) DrawWithMenu(position,  property,  label);
            else DrawSimple( position,  property,  label);
            
        }
        else
        {
            EditorGUI.HelpBox(position, "To use the LocalizationId Attribute \"" +
                                        label.text + "\" must be an string!", MessageType.Error);
        }
        
        EditorGUI.EndProperty();
    }

    private void DrawSimple(Rect position, SerializedProperty property, GUIContent label)
    {
        property.stringValue = EditorGUI.TextField(new Rect(position.x,
          position.y, position.width, position.height / 2), property.stringValue);

       
    }

    private void DrawWithMenu(Rect position, SerializedProperty property, GUIContent label)
    {
        //EditorGUI.LabelField(new Rect(position.x,
          //  position.y, position.width, position.height / 2), property.stringValue);
        GUI.Button(new Rect(position.x,
            position.y, position.width, position.height / 2), property.stringValue);
    }
}