# ボタンのトグル表示ガイド

ボタンが押し込まれている（アクティブ）状態を視覚的に表示する方法を説明します。

---

## 方法1: MemoryToggleButtonスクリプトを使う（推奨）⭐

### 特徴
- ✅ ボタンの色が変わる（白 → 明るい緑）
- ✅ オプションでテキストも変更可能（Start/Stop）
- ✅ 簡単に追加できる

### セットアップ手順

#### 1. スクリプトを追加
1. Hierarchy でボタンを選択（例: `Button(Allocate)`）
2. Inspector で **Add Component** をクリック
3. `MemoryToggleButton` を検索して追加

#### 2. OnClickイベントを設定
**重要**: OnClickイベントに2つのメソッドを追加します

1. **Button コンポーネント** の `On Click ()` を展開
2. **既存のイベント**（例: `MemoryManager.ToggleTotalAllocate`）はそのまま
3. **+** ボタンをクリックして新しいイベントを追加
4. 同じボタンオブジェクトをドラッグ
5. ドロップダウンから `MemoryToggleButton > Toggle ()` を選択

**結果**: ボタンをクリックすると
- MemoryManagerのメソッドが実行される
- ボタンの見た目が変わる

#### 3. 色のカスタマイズ（オプション）
Inspector の `MemoryToggleButton` コンポーネントで：

- **Normal Color**: 通常時の色（デフォルト: 白）
- **Active Color**: アクティブ時の色（デフォルト: 明るい緑）

#### 4. テキスト変更（オプション）
- **Change Text** にチェック
- **Normal Text**: 通常時のテキスト（例: "Allocate"）
- **Active Text**: アクティブ時のテキスト（例: "Stop"）

---

## 方法2: Unity標準のToggleコンポーネントを使う

### 特徴
- ✅ Unity標準機能
- ✅ チェックボックス風の見た目
- ⚠️ セットアップがやや複雑

### セットアップ手順

1. Hierarchy で右クリック > UI > Toggle
2. Toggle の On Value Changed に MemoryManager のメソッドを設定
3. 見た目をカスタマイズ

**デメリット**: ボタンではなくチェックボックスになる

---

## 方法3: ボタンの色をスクリプトで直接変更

### MemoryManagerに色変更機能を追加

```csharp
[SerializeField] private Button totalAllocateButton;

public void ToggleTotalAllocate()
{
    if (mTotalAllocaterMode == AllocateMode.Increase)
    {
        mTotalAllocaterMode = AllocateMode.None;
        if (totalAllocateButton != null)
            totalAllocateButton.GetComponent<Image>().color = Color.white;
    }
    else
    {
        mTotalAllocaterMode = AllocateMode.Increase;
        if (totalAllocateButton != null)
            totalAllocateButton.GetComponent<Image>().color = Color.green;
    }
}
```

**デメリット**: すべてのボタンに対して個別に実装が必要

---

## 推奨される設定

### 各ボタンに MemoryToggleButton を追加

| ボタン | 用途 | 推奨設定 |
|--------|------|----------|
| **Button(Allocate)** | メモリ割り当て | Normal Color: 白、Active Color: 緑 |
| **Button(Free)** | メモリ解放 | Normal Color: 白、Active Color: 赤 |
| **Button(Force GC)** | GC実行 | トグル不要（1回実行） |

### 色の提案

**Allocateボタン（増加）**:
- Normal: 白 `(1, 1, 1, 1)`
- Active: 緑 `(0.3, 1, 0.3, 1)` または `(0, 0.8, 0, 1)`

**Freeボタン（減少）**:
- Normal: 白 `(1, 1, 1, 1)`
- Active: 赤 `(1, 0.3, 0.3, 1)` または `(0.8, 0, 0, 1)`

---

## 使用例

### Total Memory の Allocate ボタン

1. Hierarchy で `Button(Allocate)` を選択
2. **Add Component** > `MemoryToggleButton`
3. **On Click ()** に以下を設定:
   - イベント1: `MemoryManager > ToggleTotalAllocate()`
   - イベント2: `MemoryToggleButton > Toggle()`
4. **Active Color** を明るい緑に設定

**動作**:
- クリック1回目: ボタンが緑色になる → メモリ割り当て開始
- クリック2回目: ボタンが白色に戻る → メモリ割り当て停止

---

## トラブルシューティング

### ボタンの色が変わらない

**原因1**: MemoryToggleButton.Toggle() がOnClickに登録されていない
- **解決**: OnClickイベントに `MemoryToggleButton > Toggle()` を追加

**原因2**: ボタンにImageコンポーネントがない
- **解決**: Buttonには自動的にImageがあるはずですが、確認してください

**原因3**: 色が同じ
- **解決**: InspectorでActive Colorを別の色に変更

### テキストが変わらない

**原因**: Change Text にチェックが入っていない
- **解決**: InspectorでChange Textにチェックを入れる

---

## より高度なカスタマイズ

### アニメーションを追加

1. ボタンを選択
2. **Animation** ウィンドウを開く
3. 押下時のアニメーションを作成（スケール変更など）

### サウンドを追加

```csharp
[SerializeField] private AudioClip toggleSound;
private AudioSource audioSource;

public void Toggle()
{
    isActive = !isActive;
    UpdateVisuals();

    if (audioSource != null && toggleSound != null)
    {
        audioSource.PlayOneShot(toggleSound);
    }
}
```

---

## まとめ

**最も簡単な方法**: MemoryToggleButton スクリプトを追加
- 各ボタンに `MemoryToggleButton` コンポーネントを追加
- OnClickに `Toggle()` を追加
- 色をカスタマイズ

**所要時間**: ボタン1つあたり約30秒

---

**作成日**: 2026-04-07
