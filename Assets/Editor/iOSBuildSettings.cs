using UnityEngine;

/// <summary>
/// iOS build settings for memory limit capability
/// </summary>
public class iOSBuildSettings : ScriptableObject
{
    private const string SettingsPath = "Assets/Editor/iOSBuildSettings.asset";

    [SerializeField]
    [Tooltip("Enable increased memory limit capability for iOS builds")]
    private bool enableIncreasedMemoryLimit = true;

    /// <summary>
    /// Gets whether increased memory limit is enabled
    /// </summary>
    public bool EnableIncreasedMemoryLimit => enableIncreasedMemoryLimit;

    /// <summary>
    /// Gets or creates the settings instance
    /// </summary>
    /// <returns>The settings instance</returns>
    public static iOSBuildSettings GetOrCreateSettings()
    {
        var settings = UnityEditor.AssetDatabase.LoadAssetAtPath<iOSBuildSettings>(SettingsPath);

        if (settings == null)
        {
            settings = CreateInstance<iOSBuildSettings>();
            settings.enableIncreasedMemoryLimit = true;

#if UNITY_EDITOR
            // Ensure the directory exists
            string directory = System.IO.Path.GetDirectoryName(SettingsPath);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            UnityEditor.AssetDatabase.CreateAsset(settings, SettingsPath);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }

        return settings;
    }

    /// <summary>
    /// Gets the serialized settings object
    /// </summary>
    /// <returns>The serialized object</returns>
    public static UnityEditor.SerializedObject GetSerializedSettings()
    {
        return new UnityEditor.SerializedObject(GetOrCreateSettings());
    }
}
