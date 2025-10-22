using System;
using AdsIntegration.Runtime.Config;
using UnityEditor;
using UnityEngine;

namespace AdsIntegration.Editor
{
    internal sealed class AdSettingsWindow : EditorWindow
    {
        private SerializedObject _serializedObject;
        private AdServiceConfig _settings;

        private SerializedProperty _appKeyProperty;
        private SerializedProperty _rewardedAdUnitIdProperty;
        private SerializedProperty _interstitialAdUnitIdProperty;
        private SerializedProperty _timeBetweenInterstitialsProperty;
        private SerializedProperty _maxInterstitialLoadAttemptsProperty;
        private SerializedProperty _maxRewardedLoadAttemptsProperty;
        private SerializedProperty _retryLoadDelayProperty;
        private SerializedProperty _placementDefinitionsProperty;
        private SerializedProperty _placementEnumTypeProperty;

        private Vector2 _scrollPosition;
        private Type _selectedEnumType;
        private bool _showManualPlacementEditor = true;

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.delayCall += static () =>
            {
                if (AdServiceConfig.IsSettingsExist is false)
                    AdServiceConfig.GetOrCreateSettings();
            };
        }

        [MenuItem("Tools/Ads Integration Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<AdSettingsWindow>("Ads Integration Settings");
            window.minSize = new Vector2(450, 600);
            window.Show();
        }

        private void OnEnable()
        {
            _settings = AdServiceConfig.GetOrCreateSettings();
            _serializedObject = new SerializedObject(_settings);

            _appKeyProperty = _serializedObject.FindField(nameof(AdServiceConfig.AppKey));
            _rewardedAdUnitIdProperty = _serializedObject.FindField(nameof(AdServiceConfig.RewardedAdUnitId));
            _interstitialAdUnitIdProperty = _serializedObject.FindField(nameof(AdServiceConfig.InterstitialAdUnitId));
            _timeBetweenInterstitialsProperty =
                _serializedObject.FindField(nameof(AdServiceConfig.TimeBetweenInterstitials));

            _maxInterstitialLoadAttemptsProperty =
                _serializedObject.FindField(nameof(AdServiceConfig.MaxInterstitialLoadAttempts));

            _maxRewardedLoadAttemptsProperty =
                _serializedObject.FindField(nameof(AdServiceConfig.MaxRewardedLoadAttempts));

            _retryLoadDelayProperty = _serializedObject.FindField(nameof(AdServiceConfig.RetryLoadDelay));
            _placementDefinitionsProperty = _serializedObject.FindField(nameof(AdServiceConfig.PlacementDefinitions));
            _placementEnumTypeProperty = _serializedObject.FindField(nameof(AdServiceConfig.PlacementEnumType));

            _selectedEnumType = _settings.GetPlacementEnumType();
        }

        private void OnGUI()
        {
            if (_serializedObject == null || !_settings)
            {
                OnEnable();

                if (_serializedObject == null)
                {
                    EditorGUILayout.HelpBox("Failed to load or create settings asset.", MessageType.Error);
                    return;
                }
            }

            _serializedObject.Update();

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Ads Integration Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.LabelField("IronSource Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_appKeyProperty, new GUIContent("App Key"));
            EditorGUILayout.PropertyField(_rewardedAdUnitIdProperty, new GUIContent("Rewarded Ad Unit ID"));
            EditorGUILayout.PropertyField(_interstitialAdUnitIdProperty, new GUIContent("Interstitial Ad Unit ID"));
            EditorGUILayout.Space(15);

            EditorGUILayout.LabelField("Ad Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_timeBetweenInterstitialsProperty,
                new GUIContent("Time Between Interstitials"));

            EditorGUILayout.PropertyField(_maxInterstitialLoadAttemptsProperty,
                new GUIContent("Max Interstitial Load Attempts"));

            EditorGUILayout.PropertyField(_maxRewardedLoadAttemptsProperty,
                new GUIContent("Max Rewarded Load Attempts"));

            EditorGUILayout.PropertyField(_retryLoadDelayProperty, new GUIContent("Retry Load Delay"));
            EditorGUILayout.Space(15);

            DrawPlacementEnumSection();

            EditorGUILayout.Space(10);

            _showManualPlacementEditor = EditorGUILayout.Foldout(_showManualPlacementEditor, "Manual Placement Editor");
            if (_showManualPlacementEditor)
            {
                if (_settings.GetPlacementEnumType() != null)
                    EditorGUILayout.HelpBox(
                        "Placements are being managed by the enum type above. " +
                        "Manual edits will be overwritten when the enum is applied.",
                        MessageType.Warning);

                EditorGUILayout.PropertyField(_placementDefinitionsProperty, true);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Save Settings", GUILayout.Width(150), GUILayout.Height(30)))
            {
                _serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_settings);
                AssetDatabase.SaveAssets();

                EditorUtility.DisplayDialog("Ads Integration Settings", "Settings saved successfully!", "OK");
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            if (_serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(_settings);
        }

        private void DrawPlacementEnumSection()
        {
            EditorGUILayout.LabelField("Enum Placement Configuration", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Select an enum type to use for placement definitions." +
                " This provides type safety and IntelliSense support.",
                MessageType.Info);

            _selectedEnumType = AdPlacementEditorUtility.ShowPlacementEnumTypesDropdown(_selectedEnumType);

            if (_selectedEnumType == null)
                return;

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Placement Preview:", EditorStyles.boldLabel);

            var placementDefinitions = PlacementExtensions.GetPlacementDefinitions(_selectedEnumType);
            if (placementDefinitions.Count <= 0)
            {
                EditorGUILayout.HelpBox(
                    $"No placement attributes found in {_selectedEnumType.Name}." +
                    $" Make sure to add [Placement] attributes to your enum values.",
                    MessageType.Warning);
                return;
            }

            EditorGUI.indentLevel++;

            foreach (var placement in placementDefinitions)
            {
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField(placement.PlacementName, EditorStyles.boldLabel,
                        GUILayout.Width(150));
                }
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Space(5);

            if (GUILayout.Button("Apply This Enum Type") is false)
                return;

            Undo.RecordObject(_settings, "Update Ad Placement Enum Type");

            _settings.SetPlacementEnumType(_selectedEnumType);
            _placementEnumTypeProperty.stringValue = _selectedEnumType.AssemblyQualifiedName;

            EditorUtility.SetDirty(_settings);
            _serializedObject.Update();
        }
    }
}