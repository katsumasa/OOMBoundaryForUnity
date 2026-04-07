# OOMBoundaryForUnity

## 概要

Out of Memoryを発生させ、それまでの過程を観察する為のToolです。

## 🚀 クイックスタート

**初めて使う方は [QUICK_START.md](QUICK_START.md) をご覧ください。**

5分で以下が完了します：
1. Unity Editorの起動
2. UIボタンの自動接続（ツール使用）
3. 動作テスト
4. ユニットテストの実行

## 📋 ドキュメント

- **[QUICK_START.md](QUICK_START.md)** - 5分で始めるクイックガイド
- **[Assets/Scenes/UI_SETUP.md](Assets/Scenes/UI_SETUP.md)** - UI接続の詳細手順
- **[Assets/Tests/README.md](Assets/Tests/README.md)** - テストの実行方法
- **[CHANGES_SUMMARY.md](CHANGES_SUMMARY.md)** - 最新の変更内容

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
