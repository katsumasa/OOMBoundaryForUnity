using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// MemoryManagerのUIボタンを自動的に接続するEditor Script
/// </summary>
public class MemoryManagerUISetup : EditorWindow
{
    private MemoryManager memoryManager;

    [MenuItem("Tools/Memory Manager/Auto Connect UI Buttons")]
    public static void ShowWindow()
    {
        GetWindow<MemoryManagerUISetup>("UI Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("Memory Manager UI Auto Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "このツールは、MainScene内のすべてのボタンをMemoryManagerに自動的に接続します。\n\n" +
            "実行前に:\n" +
            "1. MainSceneを開いていることを確認してください\n" +
            "2. MemoryManagerコンポーネントがシーンに存在することを確認してください",
            MessageType.Info
        );

        GUILayout.Space(10);

        // MemoryManagerの自動検出
        if (memoryManager == null)
        {
            memoryManager = FindFirstObjectByType<MemoryManager>();
        }

        // MemoryManagerの表示
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("MemoryManager:", GUILayout.Width(120));

        if (memoryManager != null)
        {
            EditorGUILayout.LabelField(memoryManager.gameObject.name, EditorStyles.boldLabel);
        }
        else
        {
            EditorGUILayout.LabelField("Not Found", EditorStyles.boldLabel);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        // 実行ボタン
        GUI.enabled = memoryManager != null;

        if (GUILayout.Button("Auto Connect All Buttons", GUILayout.Height(40)))
        {
            ConnectAllButtons();
        }

        GUI.enabled = true;

        GUILayout.Space(10);

        // 手動接続用のセクション
        EditorGUILayout.LabelField("Manual Connection", EditorStyles.boldLabel);
        memoryManager = (MemoryManager)EditorGUILayout.ObjectField("MemoryManager", memoryManager, typeof(MemoryManager), true);

        GUILayout.Space(10);

        if (GUILayout.Button("Open UI Setup Guide"))
        {
            string guidePath = "Assets/Scenes/UI_SETUP.md";
            System.Diagnostics.Process.Start(guidePath);
        }
    }

    void ConnectAllButtons()
    {
        if (memoryManager == null)
        {
            EditorUtility.DisplayDialog("Error", "MemoryManagerが見つかりません。", "OK");
            return;
        }

        int connectedCount = 0;

        // すべてのButtonコンポーネントを取得
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in allButtons)
        {
            string buttonName = button.gameObject.name;

            // ボタン名から適切なメソッドを判定して接続
            if (ConnectButton(button, buttonName))
            {
                connectedCount++;
                Debug.Log($"Connected: {buttonName}");
            }
        }

        if (connectedCount > 0)
        {
            // シーンを保存
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            EditorUtility.DisplayDialog(
                "Success",
                $"{connectedCount}個のボタンを接続しました。\n\nシーンを保存してください。",
                "OK"
            );
        }
        else
        {
            EditorUtility.DisplayDialog(
                "Warning",
                "接続できるボタンが見つかりませんでした。\nボタンの名前を確認してください。",
                "OK"
            );
        }
    }

    bool ConnectButton(Button button, string buttonName)
    {
        // 既存のイベントをクリア
        button.onClick.RemoveAllListeners();

        // ボタン名に基づいてメソッドを接続
        bool connected = false;

        // Total Memory buttons
        if (buttonName.Contains("Button(Allocate)") || buttonName.Contains("Button(allocate)"))
        {
            GameObject parent = button.transform.parent?.gameObject;
            if (parent != null)
            {
                string parentName = parent.name;

                if (parentName.Contains("Total") || parentName.Contains("Profiler"))
                {
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(
                        button.onClick,
                        memoryManager.ToggleTotalAllocate
                    );
                    connected = true;
                }
                else if (parentName.Contains("Graphics") || parentName.Contains("Driver"))
                {
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(
                        button.onClick,
                        memoryManager.ToggleGraphicsDriverAllocate
                    );
                    connected = true;
                }
                else if (parentName.Contains("Mono") || parentName.Contains("Heap"))
                {
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(
                        button.onClick,
                        memoryManager.ToggleMonoHeapAllocate
                    );
                    connected = true;
                }
            }
        }
        else if (buttonName.Contains("Button(Free)") || buttonName.Contains("Button(free)"))
        {
            GameObject parent = button.transform.parent?.gameObject;
            if (parent != null)
            {
                string parentName = parent.name;

                if (parentName.Contains("Total") || parentName.Contains("Profiler"))
                {
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(
                        button.onClick,
                        memoryManager.ToggleTotalFree
                    );
                    connected = true;
                }
                else if (parentName.Contains("Graphics") || parentName.Contains("Driver"))
                {
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(
                        button.onClick,
                        memoryManager.ToggleGraphicsDriverFree
                    );
                    connected = true;
                }
                else if (parentName.Contains("Mono") || parentName.Contains("Heap"))
                {
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(
                        button.onClick,
                        memoryManager.ToggleMonoHeapFree
                    );
                    connected = true;
                }
            }
        }

        return connected;
    }

    [MenuItem("Tools/Memory Manager/Verify UI Connections")]
    public static void VerifyConnections()
    {
        MemoryManager memoryManager = FindFirstObjectByType<MemoryManager>();

        if (memoryManager == null)
        {
            EditorUtility.DisplayDialog("Error", "MemoryManagerが見つかりません。", "OK");
            return;
        }

        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        int connectedCount = 0;
        int totalButtons = 0;
        string report = "=== Button Connection Report ===\n\n";

        foreach (Button button in allButtons)
        {
            string buttonName = button.gameObject.name;

            if (buttonName.Contains("Button(Allocate)") || buttonName.Contains("Button(Free)") ||
                buttonName.Contains("Button(allocate)") || buttonName.Contains("Button(free)"))
            {
                totalButtons++;
                int listenerCount = button.onClick.GetPersistentEventCount();

                if (listenerCount > 0)
                {
                    connectedCount++;
                    report += $"✅ {buttonName}: Connected\n";
                }
                else
                {
                    report += $"❌ {buttonName}: Not Connected\n";
                }
            }
        }

        report += $"\n合計: {connectedCount}/{totalButtons} ボタンが接続されています。";

        Debug.Log(report);

        EditorUtility.DisplayDialog(
            "Connection Report",
            $"{connectedCount}/{totalButtons} ボタンが接続されています。\n\n詳細はConsoleを確認してください。",
            "OK"
        );
    }
}
