using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRCBreeze
{
    [CustomPropertyDrawer(typeof(BoneObjects))]
    public class BoneObjectsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),property.isExpanded,label,true);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                var y = position.y + EditorGUIUtility.singleLineHeight;

                DrawField(ref y, position.width, property, "breezeBone");
                DrawField(ref y, position.width, property, "breezeBoneWeight");
                DrawField(ref y, position.width, property, "invertX");
                DrawField(ref y, position.width, property, "invertZ");

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        void DrawField(ref float y, float width, SerializedProperty property, string fieldName)
        {
            var prop = property.FindPropertyRelative(fieldName);

            float indent = EditorGUI.indentLevel * 30f;

            Rect r = new Rect(indent, y, width, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(r, prop, new GUIContent(VRCBreezeLocalizationBridge.Get(fieldName)));

            y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
                return EditorGUIUtility.singleLineHeight;

            return EditorGUIUtility.singleLineHeight * 5;
        }
    }

    public static class VRCBreezeLocalizationBridge
    {
        public static Dictionary<string, string> localized_texts;

        public static string Get(string key)
        {
            if (localized_texts != null && localized_texts.TryGetValue(key, out var value))
            {
                return value;
            }
            return key;
        }
    }
}