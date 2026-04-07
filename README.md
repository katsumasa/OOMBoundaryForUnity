# OOMBoundaryForUnity

## 概要

Out of Memoryを発生させ、それまでの過程を観察する為のToolです。

## 🚀 クイックスタート

**初めて使う方は [QUICK_START.md](Docs/QUICK_START.md) をご覧ください。**

5分で以下が完了します：
1. Unity Editorの起動
2. シーンの確認
3. 動作テスト
4. ユニットテストの実行

## 📋 ドキュメント

- **[QUICK_START.md](Docs/QUICK_START.md)** - 5分で始めるクイックガイド
- **[Assets/Tests/README.md](Assets/Tests/README.md)** - テストの実行方法
- **[CHANGES_SUMMARY.md](Docs/CHANGES_SUMMARY.md)** - 最新の変更内容
- **[その他のドキュメント](Docs/)** - Docsフォルダー内の詳細ドキュメント

## 説明

### SystemInfo

#### systemMemorySize

デバイスに搭載されているシステムメモリ（RAM）の概算総量です。


#### graphicsMemorySize

グラフィックスカードの物理的な総メモリ量（VRAM）

### Profiler

### Total

#### Reserved

UnityがOSから確保しているメモリの総量です。
以下の公式が成り立ちます。

Reserved = Allocated + UnReserved


#### Allocated

現在Unityが実際に割り当てて使用しているメモリ量。


#### UnReserved

ReserveされているがまだUnityが使用していないメモリ量。
UnReserved = Reserved - Allocated;


#### Allocate Button

毎フレームTotal Memory内に確保されるようにオブジェクトを生成します。
もう一度押すと停止します。


#### Free Button
毎フレーム確保したオブジェクトを削除します。
もう一度押すと停止します。


### Graphics Driver

#### Allocated

テクスチャ、メッシュ、コンピュートバッファ、ジオメトリバッファなど、すべての割り当てられたグラフィックス関連リソースが使用している総メモリ量を表します。
これは、Unityエンジンが使用する内部のグラフィックスリソースや、スクリプトから作成されたグラフィックスリソースのプロパティ（テクスチャの解像度やフォーマットなど）に基づいてUnity側が計算・***推定***した値です。
「Development Build（開発ビルド）」として書き出されたアプリケーションでのみ有効な値を返します。リリースビルド（Release Player）で呼び出した場合は、常に 0 が返されます。
この戻り値はあくまで「Unityが要求したリソースの設計サイズに基づく推計値」です。実際のOSやグラフィックスドライバ（OpenGLやVulkanなど）のレイヤーでは、メモリのアライメント（確保するブロックサイズの切り上げ）があったり、実際に描画に使用されるまで物理メモリを確保しない（遅延割り当て）挙動をとることがあります。そのため、Android StudioなどのOSネイティブのプロファイラが報告するグラフィックスメモリ消費量とは乖離が生じることがあります。


#### Allocate Button

毎フレームGraphics Driver内に確保されるようにオブジェクトを生成します。
もう一度押すと停止します。


#### Free Button
毎フレーム確保したオブジェクトを削除します。
もう一度押すと停止します。



### TempAllocator

一時メモリ用スタックアロケーターのサイズ(フレーム内で完結する短命な割り当て専用)（TotalReservedMemorの一部です。）

### MonoHeap

UnityがOSから確保している「マネージドヒープ（C#用のメモリ領域）」です。GetTotalReservedMemoryには含まれません。
string, List<T>, byte[] などのC#オブジェクトが占有するメモリです。


#### HeapSize

UnityがOSから確保している「マネージドヒープ（C#用のメモリ領域）の総サイズ」を返します。

#### UsedSize
 現在マネージドヒープ内で「実際にオブジェクトに割り当てられて使用中」のメモリサイズを返します。
現在も参照されていて生存しているオブジェクトと、不要になったもののまだGCによって回収されていない（ゴミとなった）オブジェクトの合計サイズです。
オブジェクトを生成すると値が増加し、ガベージコレクション（GC.Collect()）が走って不要なオブジェクトがメモリから解放されると値が減少します

#### Allocate Button

毎フレームMono Heap内に確保されるようにオブジェクトを生成します。
もう一度押すと停止します。


#### Free Button
毎フレーム確保したオブジェクトを削除します。
もう一度押すと停止します。

## 🔍 Native Memory Info

OSネイティブレベルのメモリ情報を取得する機能です。各プラットフォーム（iOS、Android、Windows）でネイティブプラグインを使用して、Unity APIでは取得できない詳細なメモリ情報を提供します。

### 対応プラットフォーム

- **iOS**: Objective-C++プラグイン (`Assets/Plugins/iOS/NativeMemoryInfo.mm`)
- **Android**: Javaプラグイン (`Assets/Plugins/Android/NativeMemoryInfo.java`)
- **Windows**: P/Invoke (`Assets/Scripts/NativeMemoryInfo.cs`)
- **Editor**: Windowsプラットフォームの実装を使用

### 取得できるメモリ情報

#### Allocated Memory（確保済みメモリ）

アプリケーションが確保しているメモリの総量です。

- **iOS**: `task_info`の`resident_size`
- **Android**: `Debug.MemoryInfo`の`getTotalPrivateDirty()`
- **Windows**: `PROCESS_MEMORY_COUNTERS`の`PagefileUsage`

#### Memory Footprint（物理メモリフットプリント）

アプリケーションが実際に使用している物理メモリ量です。

- **iOS**: `task_info`の`phys_footprint` - 実際のメモリ使用量を示す最も重要な指標
- **Android**: `Debug.MemoryInfo`の`getTotalPss()` - Proportional Set Size
- **Windows**: `PROCESS_MEMORY_COUNTERS`の`WorkingSetSize`

#### Available Memory（利用可能メモリ）

システムで現在利用可能なメモリ量です。

- **iOS**: `os_proc_available_memory()` (iOS 13.0+) - アプリが追加で利用可能なメモリ
- **Android**: `ActivityManager.MemoryInfo`の`availMem`
- **Windows**: `MEMORYSTATUSEX`の`ullAvailPhys`

#### Absolute Limit（絶対メモリ制限）

アプリケーションが使用できるメモリの上限です。この値を超えるとOOM（Out of Memory）が発生する可能性があります。

- **iOS**: `phys_footprint + os_proc_available_memory()` - 現在のフットプリント + 利用可能メモリ
- **Android**: `getTotalPss() + availMem` - 現在のフットプリント + 利用可能メモリ
- **Windows**: `WorkingSetSize + ullAvailPhys` - 現在のフットプリント + 利用可能メモリ

#### Physical Memory（物理メモリサイズ）

デバイスに搭載されている物理メモリの総量です。

- **iOS**: `NSProcessInfo.physicalMemory`
- **Android**: `ActivityManager.MemoryInfo`の`totalMem`
- **Windows**: `MEMORYSTATUSEX`の`ullTotalPhys`

### 使用方法

```csharp
// すべてのメモリ情報を一度に取得
var memoryData = NativeMemoryInfo.GetMemoryInfo();
Debug.Log($"Allocated: {memoryData.allocatedMemory}");
Debug.Log($"Footprint: {memoryData.memoryFootprint}");
Debug.Log($"Available: {memoryData.availableMemory}");
Debug.Log($"Absolute Limit: {memoryData.absoluteLimit}");
Debug.Log($"Physical Memory: {memoryData.physicalMemorySize}");

// 個別に取得
ulong allocated = NativeMemoryInfo.GetAllocatedMemorySize();
ulong footprint = NativeMemoryInfo.GetMemoryFootprintSize();
ulong available = NativeMemoryInfo.GetAvailableMemory();
ulong limit = NativeMemoryInfo.GetAbsoluteLimit();
ulong physical = NativeMemoryInfo.GetPhysicalMemorySize();
```

### 参考プロジェクト

この機能は [OOMBoundary](https://github.com/katsumasa/OOMBoundary) の実装を参考にしています。
