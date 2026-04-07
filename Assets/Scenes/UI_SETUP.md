# UI接続ガイド

このドキュメントは、MainSceneのUIボタンとMemoryManagerスクリプトを接続する方法を説明します。

## 前提条件

- MainSceneが開いている
- MemoryManagerスクリプトがシーン内のGameObjectにアタッチされている

## UIの構造

MainSceneには以下の3つのメモリカテゴリがあり、それぞれに**Allocate**ボタンと**Free**ボタンがあります：

### 1. Total Memory (Profiler)
- **Button(Allocate)** - ネイティブメモリを割り当て
- **Button(Free)** - ネイティブメモリを解放

### 2. Graphics Driver
- **Button(Allocate)** - グラフィックスメモリ（テクスチャ）を割り当て
- **Button(Free)** - グラフィックスメモリを解放

### 3. MonoHeap
- **Button(Allocate)** - マネージドメモリを割り当て
- **Button(Free)** - マネージドメモリを解放（GC実行）

## ボタンの接続方法

### Unity Editorでの手順

#### 1. Total Memoryのボタン接続

**Allocateボタン:**
1. Hierarchyで `Total Memory > Button(Allocate)` を選択
2. Inspectorで **Button** コンポーネントを探す
3. **On Click ()** セクションを展開
4. **+** ボタンをクリックして新しいイベントを追加
5. MemoryManagerがアタッチされているGameObjectをドラッグ&ドロップ
6. ドロップダウンから `MemoryManager > ToggleTotalAllocate ()` を選択

**Freeボタン:**
1. Hierarchyで `Total Memory > Button(Free)` を選択
2. 上記と同じ手順で `MemoryManager > ToggleTotalFree ()` を選択

#### 2. Graphics Driverのボタン接続

**Allocateボタン:**
1. Hierarchyで `Graphics Driver > Button(Allocate)` を選択
2. `MemoryManager > ToggleGraphicsDriverAllocate ()` を接続

**Freeボタン:**
1. Hierarchyで `Graphics Driver > Button(Free)` を選択
2. `MemoryManager > ToggleGraphicsDriverFree ()` を接続

#### 3. MonoHeapのボタン接続

**Allocateボタン:**
1. Hierarchyで `MonoHeap > Button(Allocate)` を選択
2. `MemoryManager > ToggleMonoHeapAllocate ()` を接続

**Freeボタン:**
1. Hierarchyで `MonoHeap > Button(Free)` を選択
2. `MemoryManager > ToggleMonoHeapFree ()` を接続

## MemoryManagerの接続確認

MemoryManagerコンポーネントのInspectorで、以下のTextMeshProUGUIフィールドが正しく設定されているか確認してください：

- **m Text System Memory Size** - システムメモリの表示用
- **m Text Graphics Memory Size** - グラフィックスメモリの表示用
- **m Text Mono Heap Size** - MonoHeapサイズの表示用
- **m Text Mono Used Size** - MonoHeap使用量の表示用
- **m Text Total Reserved Memory** - Total Reserved Memoryの表示用
- **m Text Total Allocator Memory** - Total Allocated Memoryの表示用
- **m Text Total Un Reserved Memory** - UnReserved Memoryの表示用
- **m Text Graphic Driver Allocator Memory** - Graphics Driver Memoryの表示用
- **m Text Tmp Allocator Size** - Temp Allocatorサイズの表示用

## ボタンの動作

各ボタンは**トグル式**で動作します：

- **Allocateボタン**:
  - 1回目のクリック → メモリ割り当て開始（毎フレーム10MB追加）
  - 2回目のクリック → 停止

- **Freeボタン**:
  - 1回目のクリック → メモリ解放開始（毎フレーム10MB解放）
  - 2回目のクリック → 停止

## トラブルシューティング

### ボタンをクリックしても何も起こらない

1. MemoryManagerのGameObjectがActiveになっているか確認
2. ボタンのOnClickイベントが正しく設定されているか確認
3. Consoleにエラーが出ていないか確認

### テキストが更新されない

1. MemoryManagerのSerializedFieldが正しく設定されているか確認
2. TextMeshProUGUIコンポーネントがActiveになっているか確認

### メモリが増えない

1. Development Buildでビルドしているか確認（Release Buildでは一部の機能が無効）
2. Profilerウィンドウで実際のメモリ使用量を確認

## テスト方法

1. Unity Editorで再生ボタンを押す
2. 各カテゴリのAllocateボタンをクリック
3. メモリ表示の数値が毎フレーム増加することを確認
4. Freeボタンをクリック
5. メモリ表示の数値が毎フレーム減少することを確認
6. 十分にメモリを割り当てるとOut of Memoryエラーが発生

## 参考情報

- MemoryManagerスクリプト: `Assets/Scripts/MemoryManager.cs`
- テストコード: `Assets/Tests/`
- プロジェクトREADME: `README.md`
