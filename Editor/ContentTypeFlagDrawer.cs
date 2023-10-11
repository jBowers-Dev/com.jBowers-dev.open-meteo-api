using UnityEditor;
using UnityEngine;

namespace JoshBowersDev.RestAPI.Editor
{
    [CustomPropertyDrawer(typeof(HttpEnums.ContentType))]
    public class ContentTypeFlagDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
        }
    }
}