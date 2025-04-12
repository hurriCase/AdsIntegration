using UnityEditor;

namespace AdsIntegration.Editor
{
    internal static class SerializedPropertyExtensions
    {
        internal static SerializedProperty FindField(this SerializedObject serializedObject, string name) =>
            serializedObject.FindProperty(name.ConvertToBackingField());

        internal static SerializedProperty FindFieldRelative(this SerializedProperty serializedObject, string name) =>
            serializedObject.FindPropertyRelative(name.ConvertToBackingField());

        private static string ConvertToBackingField(this string propertyName)
            => $"<{propertyName}>k__BackingField";
    }
}