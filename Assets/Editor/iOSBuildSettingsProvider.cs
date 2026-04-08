using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Settings provider for iOS build settings in Project Settings
/// </summary>
public class iOSBuildSettingsProvider : SettingsProvider
{
    private SerializedObject m_Settings;

    private class Styles
    {
        public static GUIContent enableIncreasedMemoryLimit = new GUIContent(
            "Enable Increased Memory Limit",
            "When enabled, adds com.apple.developer.kernel.increased-memory-limit capability to iOS builds. " +
            "This allows the app to use more memory before being terminated by the system."
        );
    }

    public iOSBuildSettingsProvider(string path, SettingsScope scope = SettingsScope.Project)
        : base(path, scope)
    {
    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        m_Settings = iOSBuildSettings.GetSerializedSettings();
    }

    public override void OnGUI(string searchContext)
    {
        if (m_Settings == null)
        {
            m_Settings = iOSBuildSettings.GetSerializedSettings();
        }

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("iOS Memory Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        SerializedProperty enableProperty = m_Settings.FindProperty("enableIncreasedMemoryLimit");
        EditorGUILayout.PropertyField(enableProperty, Styles.enableIncreasedMemoryLimit);

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "This setting controls whether the increased memory limit capability is added during iOS builds. " +
            "Enable this to allow your app to use more memory on iOS devices.",
            MessageType.Info
        );

        if (EditorGUI.EndChangeCheck())
        {
            m_Settings.ApplyModifiedProperties();
        }
    }

    [SettingsProvider]
    public static SettingsProvider CreateiOSBuildSettingsProvider()
    {
        var provider = new iOSBuildSettingsProvider("Project/iOS Build Settings", SettingsScope.Project);
        provider.keywords = new[] { "iOS", "Memory", "Increased", "Limit", "Capability", "Entitlements" };
        return provider;
    }

    [MenuItem("Edit/Project Settings/iOS Build Settings")]
    public static void OpenSettings()
    {
        SettingsService.OpenProjectSettings("Project/iOS Build Settings");
    }
}
