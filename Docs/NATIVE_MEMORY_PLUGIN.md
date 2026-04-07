# ネイティブメモリ情報プラグイン

OSネイティブのメモリ情報を取得するUnityプラグインです。

---

## 📋 概要

このプラグインは、iOS、Android、WindowsでOSレベルのメモリ情報を取得します。

### 取得できる情報

| 情報 | 説明 | iOS | Android | Windows |
|------|------|-----|---------|---------|
| **Allocated Memory** | 確保したメモリ量 | ✅ | ✅ | ✅ |
| **Memory Footprint** | 実際の物理メモリ使用量 | ✅ phys_footprint | ✅ Total PSS | ✅ Working Set |
| **Available Memory** | 残り利用可能メモリ | ✅ iOS 13+ | ✅ | ✅ |
| **Absolute Limit** | 計算された絶対的限界値 | ✅ | ✅ | ✅ |
| **Physical Memory** | デバイスの物理メモリサイズ | ✅ | ✅ | ✅ |

---

## 🚀 使い方

### C#から呼び出し

```csharp
using UnityEngine;

public class Example : MonoBehaviour
{
    void Update()
    {
        // すべての情報を一度に取得
        var memoryInfo = NativeMemoryInfo.GetMemoryInfo();

        Debug.Log($"Allocated: {FormatBytes(memoryInfo.allocatedMemory)}");
        Debug.Log($"Footprint: {FormatBytes(memoryInfo.memoryFootprint)}");
        Debug.Log($"Available: {FormatBytes(memoryInfo.availableMemory)}");
        Debug.Log($"Limit: {FormatBytes(memoryInfo.absoluteLimit)}");

        // または個別に取得
        ulong footprint = NativeMemoryInfo.GetMemoryFootprintSize();
        ulong available = NativeMemoryInfo.GetAvailableMemory();
    }

    string FormatBytes(ulong bytes)
    {
        return MemoryManager.FormatBytes((long)bytes);
    }
}
```

### MemoryManagerへの統合

`MemoryManager` に以下のフィールドを追加：

```csharp
[Header("Native Memory Info")]
[SerializeField] TextMeshProUGUI mTextNativeAllocatedMemory;
[SerializeField] TextMeshProUGUI mTextNativeMemoryFootprint;
[SerializeField] TextMeshProUGUI mTextNativeAvailableMemory;
[SerializeField] TextMeshProUGUI mTextNativeAbsoluteLimit;
[SerializeField] TextMeshProUGUI mTextNativePhysicalMemory;
```

Unity Editorで対応するTextMeshProコンポーネントを接続すると、自動的に表示されます。

---

## 📱 プラットフォーム別実装

### iOS

**ファイル**: `Assets/Plugins/iOS/NativeMemoryInfo.mm`

**実装詳細**:
- `task_info()` でタスクメモリ情報を取得
- `phys_footprint` で実際の物理メモリ使用量を取得
- `os_proc_available_memory()` で利用可能メモリを取得（iOS 13+）
- デバイスの物理メモリに基づいて限界値を計算

**参考**: [OOMBoundary](https://github.com/katsumasa/OOMBoundary)

**メモリ限界値の計算**:
```objective-c
- 1GB以下: 物理メモリの70%
- 2GB以下: 物理メモリの75%
- 3GB以下: 物理メモリの80%
- 4GB以上: 物理メモリの85%
```

### Android

**ファイル**: `Assets/Plugins/Android/NativeMemoryInfo.java`

**実装詳細**:
- `Debug.MemoryInfo` でプロセスメモリ情報を取得
- `ActivityManager.MemoryInfo` でシステムメモリ情報を取得
- `getTotalPss()` で実際のメモリ使用量を取得
- 物理メモリの80%を限界値として計算

**権限**: 不要（標準APIのみ使用）

### Windows

**実装**: C#で直接実装（P/Invoke）

**使用API**:
- `GlobalMemoryStatusEx` - システムメモリ情報
- `GetProcessMemoryInfo` - プロセスメモリ情報
- 物理メモリの90%を限界値として計算

---

## 🔧 セットアップ

### 1. ファイルの配置

プラグインファイルは以下の場所に配置されています：

```
Assets/
├── Plugins/
│   ├── iOS/
│   │   ├── NativeMemoryInfo.mm
│   │   └── NativeMemoryInfo.mm.meta
│   └── Android/
│       ├── NativeMemoryInfo.java
│       └── NativeMemoryInfo.java.meta
└── Scripts/
    ├── NativeMemoryInfo.cs (C# Wrapper)
    └── MemoryManager.cs (統合済み)
```

### 2. iOS設定

**自動**: `.mm.meta` ファイルで設定済み

**手動で確認する場合**:
1. `NativeMemoryInfo.mm` を選択
2. Inspector で **iOS** プラットフォームが有効になっているか確認

### 3. Android設定

**自動**: `.java.meta` ファイルで設定済み

**パッケージ名**: `com.unity.nativememory`

**注意**: Androidビルド時に自動的にコンパイルされます。

### 4. Windows設定

**不要**: C#で直接実装されているため、追加設定不要。

---

## 📊 UI表示例

MainSceneに以下のようにTextMeshProを配置：

```
Canvas
└── Native Memory Info Panel
    ├── Text - Allocated Memory: [値]
    ├── Text - Memory Footprint: [値]
    ├── Text - Available Memory: [値]
    ├── Text - Absolute Limit: [値]
    └── Text - Physical Memory: [値]
```

MemoryManagerのInspectorで各TextMeshProを接続すると、リアルタイムで更新されます。

---

## 🧪 動作確認

### Unity Editor（Windows）

```csharp
void Start()
{
    var info = NativeMemoryInfo.GetMemoryInfo();
    Debug.Log($"Physical Memory: {info.physicalMemorySize / 1024 / 1024} MB");
    Debug.Log($"Available: {info.availableMemory / 1024 / 1024} MB");
}
```

### iOSデバイス

1. **Development Build** でビルド
2. Xcodeで実行
3. Consoleでメモリ情報を確認

### Androidデバイス

1. **Development Build** でビルド
2. Android Studioの Logcat で確認
3. または Unity Remote で確認

---

## 🎯 メモリ限界値について

### Absolute Limitの意味

**Absolute Limit** は、アプリがクラッシュする前に到達するメモリの推定上限値です。

**計算方法**:
- **iOS**: デバイスメモリに応じて70%〜85%
- **Android**: 物理メモリの80%
- **Windows**: 物理メモリの90%

**使い方**:
```csharp
var info = NativeMemoryInfo.GetMemoryInfo();
float usage = (float)info.memoryFootprint / info.absoluteLimit;

if (usage > 0.9f)
{
    Debug.LogWarning("メモリ使用量が限界に近づいています！");
    // リソースを解放
}
```

---

## ⚠️ 注意事項

### iOS

- **iOS 13未満**: `availableMemory` は 0 を返します
- **シミュレーター**: 実機と異なる値を返す場合があります
- **Jailbreak**: 正確な値が取得できない可能性があります

### Android

- **メーカー依存**: デバイスによって挙動が異なる場合があります
- **バックグラウンド**: アプリがバックグラウンドに入ると値が変化します
- **マルチプロセス**: マルチプロセスアプリでは注意が必要です

### Windows

- **仮想メモリ**: ページファイルを含む値を返します
- **共有メモリ**: 共有DLLなどの影響を受けます

---

## 🔍 トラブルシューティング

### iOSでコンパイルエラー

**症状**: `'os/proc.h' file not found`

**解決**:
- Xcode 12+ が必要
- iOS Deployment Target を 13.0+ に設定

### Android でクラッシュ

**症状**: `ClassNotFoundException: com.unity.nativememory.NativeMemoryInfo`

**解決**:
1. `NativeMemoryInfo.java` が `Assets/Plugins/Android/` にあるか確認
2. ビルドログでコンパイルエラーを確認
3. クリーンビルドを実行

### Windowsで値が0

**症状**: すべての値が0を返す

**解決**:
- 64bitビルドで実行
- Administrator権限で実行

---

## 📚 参考資料

### iOS
- [Apple Documentation - task_info](https://developer.apple.com/documentation/kernel/1537934-task_info)
- [Apple Documentation - os_proc_available_memory](https://developer.apple.com/documentation/os/os_proc_available_memory)
- [OOMBoundary GitHub](https://github.com/katsumasa/OOMBoundary)

### Android
- [Android Documentation - Debug.MemoryInfo](https://developer.android.com/reference/android/os/Debug.MemoryInfo)
- [Android Documentation - ActivityManager](https://developer.android.com/reference/android/app/ActivityManager)

### Windows
- [GlobalMemoryStatusEx](https://docs.microsoft.com/en-us/windows/win32/api/sysinfoapi/nf-sysinfoapi-globalmemorystatusex)
- [GetProcessMemoryInfo](https://docs.microsoft.com/en-us/windows/win32/api/psapi/nf-psapi-getprocessmemoryinfo)

---

**作成日**: 2026-04-07
**バージョン**: 1.0.0
**対応プラットフォーム**: iOS 13+, Android 5.0+, Windows 10+
