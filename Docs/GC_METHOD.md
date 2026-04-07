# GCメソッドの追加

## 追加したメソッド

### ForceGarbageCollection()

**場所**: `Assets/Scripts/MemoryManager.cs`

```csharp
/// <summary>
/// ガベージコレクションを即座に実行
/// </summary>
public void ForceGarbageCollection()
{
    System.GC.Collect();
    System.GC.WaitForPendingFinalizers();
    System.GC.Collect();
}
```

## 実装の詳細

### 3段階のGC実行

1. **`GC.Collect()`** - 最初の回収
   - 到達不可能なオブジェクトをマークして回収

2. **`GC.WaitForPendingFinalizers()`** - ファイナライザーを待つ
   - ファイナライザー（デストラクタ）の実行を待機
   - ファイナライザー内でリソースが解放される

3. **`GC.Collect()`** - 2回目の回収
   - ファイナライザーで解放されたオブジェクトを回収
   - より完全なメモリ解放を実現

### なぜ2回呼ぶのか？

```
1回目のGC.Collect()
  ↓
オブジェクトA（ファイナライザー有り）が回収対象に
  ↓
GC.WaitForPendingFinalizers()
  ↓
オブジェクトAのファイナライザー実行
  ↓
オブジェクトB（Aが参照していた）が解放可能に
  ↓
2回目のGC.Collect()
  ↓
オブジェクトBも回収される ✅
```

## 使い方

### Unity Editorから

#### 方法1: ボタンに接続（推奨）
1. Hierarchy で GCボタンを作成
2. Button コンポーネントの On Click に設定
3. `MemoryManager > ForceGarbageCollection ()` を選択

#### 方法2: スクリプトから呼び出し
```csharp
MemoryManager manager = FindFirstObjectByType<MemoryManager>();
manager.ForceGarbageCollection();
```

#### 方法3: コンソールから（デバッグ用）
Debug.Logなどで確認しながら手動実行

### 期待される効果

- ✅ **Mono Used Size** が減少
- ✅ **Mono Heap Size** は変わらない（必要に応じてOSに返却）
- ✅ メモリフラグメンテーションの改善

## 注意事項

### ⚠️ パフォーマンスへの影響

**GC実行中はアプリケーションが一時停止します**

- 実行時間: 数ms〜数百ms（メモリ量に依存）
- フレームドロップの可能性あり
- ゲームプレイ中の実行は推奨されません

### 推奨される使用タイミング

✅ **使うべき時**:
- ローディング画面
- シーン切り替え時
- ポーズメニュー表示時
- メモリ使用量の確認・デバッグ時

❌ **避けるべき時**:
- ゲームプレイ中
- アニメーション再生中
- 60FPS維持が必要な場面

## 実行例

### シーン切り替え時
```csharp
IEnumerator LoadScene()
{
    // シーンをアンロード
    yield return SceneManager.UnloadSceneAsync("OldScene");
    
    // GC実行（不要なオブジェクトを削除）
    memoryManager.ForceGarbageCollection();
    
    // 新しいシーンをロード
    yield return SceneManager.LoadSceneAsync("NewScene");
}
```

### メモリ監視ツールとして
```csharp
void OnGUI()
{
    if (GUI.Button(new Rect(10, 10, 150, 30), "Force GC"))
    {
        memoryManager.ForceGarbageCollection();
        Debug.Log($"GC実行後: {Profiler.GetMonoUsedSizeLong() / 1024 / 1024} MB");
    }
}
```

## MonoHeapDecrease() との違い

| | ForceGarbageCollection() | MonoHeapDecrease() |
|---|---|---|
| **実行タイミング** | 即座に1回 | 毎フレーム継続的 |
| **対象** | すべてのマネージドメモリ | mManagedArraysのみ |
| **用途** | 手動でのメモリ解放 | メモリ解放の継続的テスト |
| **パフォーマンス影響** | 1回だけ停止 | 毎フレーム処理 |

## UIへの追加（オプション）

MainSceneにGCボタンを追加する場合：

1. **ボタンを作成**
   - Hierarchy右クリック > UI > Button
   - 名前を "Button(GC)" に変更

2. **テキストを設定**
   - 子オブジェクトのTextを "Force GC" に変更

3. **OnClickイベントを設定**
   - Button コンポーネントの On Click に MemoryManager を設定
   - `ForceGarbageCollection ()` を選択

4. **配置**
   - MonoHeap セクションの近くに配置推奨

## 参考資料

- [Unity Documentation - GC.Collect](https://docs.unity3d.com/ScriptReference/GC.Collect.html)
- [Microsoft Docs - GC.WaitForPendingFinalizers](https://docs.microsoft.com/dotnet/api/system.gc.waitforpendingfinalizers)

---

**追加日**: 2026-04-07
**用途**: メモリ管理・デバッグツール
