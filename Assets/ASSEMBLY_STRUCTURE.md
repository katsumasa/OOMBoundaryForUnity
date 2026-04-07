# アセンブリ構造

このプロジェクトのアセンブリ定義ファイル (.asmdef) の構造を説明します。

## アセンブリ一覧

### 1. OOMBoundary.Scripts
**場所**: `Assets/Scripts/Scripts.asmdef`

**内容**:
- MemoryManager.cs
- その他のゲームスクリプト

**依存関係**: なし

**プラットフォーム**: すべて

---

### 2. OOMBoundary.Tests.Editor (EditMode Tests)
**場所**: `Assets/Tests/Editor/EditorTests.asmdef`

**内容**:
- MemoryManagerTests.cs (EditModeテスト)

**依存関係**:
- UnityEngine.TestRunner
- UnityEditor.TestRunner
- OOMBoundary.Scripts

**プラットフォーム**: Editor のみ

---

### 3. OOMBoundary.Tests.Runtime (PlayMode Tests)
**場所**: `Assets/Tests/Runtime/RuntimeTests.asmdef`

**内容**:
- MemoryManagerPlayModeTests.cs (PlayModeテスト)

**依存関係**:
- UnityEngine.TestRunner
- OOMBoundary.Scripts

**プラットフォーム**: すべて

---

## 依存関係図

```
┌─────────────────────────┐
│ OOMBoundary.Scripts     │
│ (MemoryManager)         │
└─────────────────────────┘
            ▲
            │
            │ 参照
            │
     ┌──────┴──────┐
     │             │
┌────┴────────┐  ┌─┴────────────────┐
│ Editor      │  │ Runtime          │
│ Tests       │  │ Tests            │
│ (EditMode)  │  │ (PlayMode)       │
└─────────────┘  └──────────────────┘
```

## なぜアセンブリ定義が必要か？

### 問題
Unityはデフォルトで以下のアセンブリを作成します：
- `Assembly-CSharp` (すべてのスクリプト)
- `Assembly-CSharp-Editor` (Editorスクリプト)

テストは別のアセンブリに属するため、デフォルトでは`Assembly-CSharp`のクラスを参照できません。

### 解決策
明示的なアセンブリ定義ファイル (.asmdef) を作成することで：
- ✅ テストからメインスクリプトを参照可能
- ✅ コンパイル時間の短縮（変更されたアセンブリのみ再コンパイル）
- ✅ 依存関係の明確化
- ✅ プラットフォーム固有のコード分離

## トラブルシューティング

### エラー: "The type or namespace name 'MemoryManager' could not be found"

**原因**: アセンブリ参照が正しく設定されていない

**解決策**:
1. Unityを再起動
2. `Assets > Reimport All` を実行
3. アセンブリ定義ファイルが正しく配置されているか確認

### エラー: "error CS0234: The type or namespace name 'TestRunner' does not exist"

**原因**: Unity Test Frameworkがインストールされていない

**解決策**:
1. `Window > Package Manager` を開く
2. `Test Framework` を検索
3. インストール

## 参考資料

- [Unity Assembly Definitions](https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html)
- [Unity Test Framework](https://docs.unity3d.com/Packages/com.unity.test-framework@latest)
