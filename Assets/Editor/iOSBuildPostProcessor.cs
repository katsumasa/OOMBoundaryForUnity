#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

/// <summary>
/// iOS build post processor to add increased memory limit capability
/// </summary>
public class iOSBuildPostProcessor
{
    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS)
        {
            return;
        }

        // Check if increased memory limit is enabled in settings
        var settings = iOSBuildSettings.GetOrCreateSettings();
        if (!settings.EnableIncreasedMemoryLimit)
        {
            Debug.Log("Increased memory limit capability is disabled in Project Settings > iOS Build Settings");
            return;
        }

        Debug.Log("Adding increased memory limit capability to iOS build...");

        // Get the Xcode project path
        string projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        PBXProject project = new PBXProject();
        project.ReadFromFile(projectPath);

        // Get the main target GUID
#if UNITY_2019_3_OR_NEWER
        string targetGuid = project.GetUnityMainTargetGuid();
#else
        string targetGuid = project.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif

        // Add the capability to the project
        string entitlementsFileName = "App.entitlements";
        string entitlementsPath = Path.Combine(pathToBuiltProject, entitlementsFileName);

        // Create or update the entitlements file
        PlistDocument entitlements = new PlistDocument();

        if (File.Exists(entitlementsPath))
        {
            entitlements.ReadFromFile(entitlementsPath);
        }

        // Add the increased memory limit key
        PlistElementDict rootDict = entitlements.root;
        rootDict.SetBoolean("com.apple.developer.kernel.increased-memory-limit", true);

        // Write the entitlements file
        entitlements.WriteToFile(entitlementsPath);
        Debug.Log($"Created/Updated entitlements file at: {entitlementsPath}");

        // Add the entitlements file to the Xcode project
        string entitlementsRelativePath = entitlementsFileName;
        project.AddFile(entitlementsRelativePath, entitlementsFileName);
        project.AddBuildProperty(targetGuid, "CODE_SIGN_ENTITLEMENTS", entitlementsRelativePath);

        // Write the modified project
        project.WriteToFile(projectPath);

        Debug.Log("Successfully added increased memory limit capability to iOS build");
    }
}
#endif
