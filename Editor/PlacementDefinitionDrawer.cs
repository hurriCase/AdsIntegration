using AdsIntegration.Runtime.Config;
using UnityEditor;
using UnityEngine;

namespace AdsIntegration.Editor
{
    [CustomPropertyDrawer(typeof(PlacementDefinition))]
    internal class PlacementDefinitionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var placementNameProp = property.FindFieldRelative(nameof(PlacementDefinition.PlacementName));

            position.height = EditorGUIUtility.singleLineHeight;

            var placementRect = new Rect(position);
            EditorGUI.PropertyField(placementRect, placementNameProp, new GUIContent("Placement Name"));
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 3;
    }
}