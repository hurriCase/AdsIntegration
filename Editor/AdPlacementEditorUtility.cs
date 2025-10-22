using System;
using System.Collections.Generic;
using System.Linq;
using AdsIntegration.Runtime.Config;
using UnityEditor;
using UnityEngine;

namespace AdsIntegration.Editor
{
    internal static class AdPlacementEditorUtility
    {
        private static readonly List<Type> _enumTypes = new();
        private static bool _initialized;

        private static void EnsureInitialized()
        {
            if (_initialized) return;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var types = assembly.GetTypes()
                        .Where(static type => type.IsEnum && type.GetFields().Any(static fieldInfo =>
                            fieldInfo.GetCustomAttributes(typeof(PlacementAttribute), false).Length > 0))
                        .ToList();

                    _enumTypes.AddRange(types);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Exception scanning assembly {assembly.FullName}: {e.Message}");
                }
            }

            _initialized = true;
        }

        internal static Type ShowPlacementEnumTypesDropdown(Type currentType)
        {
            EnsureInitialized();

            var options = _enumTypes.Select(static type => type.FullName).ToList();
            options.Insert(0, "None");

            var currentIndex = currentType == null ? 0 : options.IndexOf(currentType.FullName);
            if (currentIndex < 0) currentIndex = 0;

            var newIndex = EditorGUILayout.Popup("Placement Enum Type", currentIndex, options.ToArray());

            if (newIndex == 0)
                return null;

            return newIndex == currentIndex ? currentType : _enumTypes[newIndex - 1];
        }

        public static Enum ShowPlacementDropdown(Enum currentValue, Type enumType, string label)
        {
            if (enumType is not { IsEnum: true })
                return null;

            return EditorGUILayout.EnumPopup(label,
                currentValue ?? Enum.GetValues(enumType).Cast<Enum>().FirstOrDefault());
        }
    }
}