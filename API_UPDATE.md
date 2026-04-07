# Unity API 更新完了

## 修正した警告

```
Assets\Editor\MemoryManagerUISetup.cs(37,29): 
warning CS0618: 'Object.FindObjectOfType<T>()' is obsolete
```

## 変更内容

### 非推奨APIから新APIへ移行 ✅

| Before (非推奨) | After (新API) | 箇所 |
|----------------|---------------|------|
| `FindObjectOfType<MemoryManager>()` | `FindFirstObjectByType<MemoryManager>()` | 2箇所 |
| `FindObjectsOfType<Button>()` | `FindObjectsByType<Button>(FindObjectsSortMode.None)` | 2箇所 |

### 変更理由

Unity 2023以降では、より明示的なAPIが推奨されています：

- **FindFirstObjectByType** - 最初に見つかったオブジェクトを返す（以前のFindObjectOfTypeと同じ動作）
- **FindAnyObjectByType** - 任意のオブジェクトを返す（より高速だが順序不定）
- **FindObjectsByType** - 複数のオブジェクトを返す（ソートモードを指定可能）

### 影響範囲

**ファイル**: `Assets/Editor/MemoryManagerUISetup.cs`

**修正箇所**:
- Line 37: MemoryManager自動検出
- Line 92: ボタン一覧取得（接続用）
- Line 209: VerifyConnections内のMemoryManager検出
- Line 217: VerifyConnections内のボタン一覧取得

## パフォーマンスへの影響

### FindFirstObjectByType vs FindAnyObjectByType

今回は `FindFirstObjectByType` を使用しました：

**選択理由**:
- MemoryManagerは通常シーンに1つしか存在しない
- 確実に同じオブジェクトを取得したい
- パフォーマンスの差は微小（ツールは頻繁に呼ばれない）

**代替案**:
もし高速化が必要な場合は `FindAnyObjectByType` に変更可能ですが、
Editor Toolなので現状で問題ありません。

## 動作確認

警告が消えたことを確認：

1. Unity Editorでスクリプトを再コンパイル
2. Consoleで警告が0件であることを確認
3. `Tools > Memory Manager > Auto Connect UI Buttons` が正常に動作することを確認

## 参考資料

- [Unity Documentation - Object.FindFirstObjectByType](https://docs.unity3d.com/ScriptReference/Object.FindFirstObjectByType.html)
- [Unity Documentation - Object.FindObjectsByType](https://docs.unity3d.com/ScriptReference/Object.FindObjectsByType.html)

---

**修正完了日**: 2026-04-07
**影響**: 警告のみ（機能的な変更なし）
