# 🚀 クイックスタートガイド

このガイドに従えば、**5分以内**にUIボタンの接続とテストが完了します。

## ステップ1: Unity Editorを起動 (1分)

1. Unity Hubから本プロジェクトを開く
2. `Assets/Scenes/MainScene.unity` をダブルクリックして開く
3. Unity Editorが完全に起動するまで待つ

## ステップ2: ボタンを自動接続 (1分) ⚡

### 自動接続ツールを使用（推奨）

1. Unity Editorのメニューバーから **`Tools > Memory Manager > Auto Connect UI Buttons`** を選択
2. 開いたウィンドウで **`Auto Connect All Buttons`** ボタンをクリック
3. "Success" ダイアログが表示されたら **OK** をクリック
4. **Ctrl+S** (または Cmd+S) でシーンを保存

✅ これで完了です！すべてのボタンが自動的に接続されました。

### 接続を確認する（オプション）

メニューから **`Tools > Memory Manager > Verify UI Connections`** を選択すると、
接続状況のレポートが表示されます。

---

## ステップ3: 動作テスト (2分) 🎮

### 基本動作の確認

1. Unity Editorの上部にある **▶️ 再生ボタン** をクリック
2. Game画面で以下の情報が表示されることを確認：
   - System Memory Size
   - Graphics Memory Size
   - Mono Heap Size
   - Total Reserved Memory
   - など

### ボタンテスト

#### Total Memory のテスト
1. **"Total Memory" セクション**の **Allocate** ボタンをクリック
2. **Total Reserved Memory** と **Total Allocated Memory** の数値が増加することを確認
3. もう一度 **Allocate** ボタンをクリックして停止
4. **Free** ボタンをクリック
5. 数値が減少することを確認

#### Graphics Driver のテスト
1. **"Graphics Driver" セクション**の **Allocate** ボタンをクリック
2. **Graphics Driver Allocated** の数値が増加することを確認
3. 停止して **Free** ボタンでテスト

#### MonoHeap のテスト
1. **"MonoHeap" セクション**の **Allocate** ボタンをクリック
2. **Mono Heap Size** と **Mono Used Size** が増加することを確認
3. 停止して **Free** ボタンでテスト

### 期待される動作

- ✅ **Allocate**ボタンをクリック → 対応するメモリ値が毎フレーム増加（約10MB/フレーム）
- ✅ もう一度クリック → 増加が停止
- ✅ **Free**ボタンをクリック → メモリ値が毎フレーム減少
- ✅ もう一度クリック → 減少が停止

### 停止

左上の **■ 停止ボタン** をクリックして再生モードを終了します。

---

## ステップ4: ユニットテストの実行 (1分) 🧪

1. Unity Editorのメニューから **`Window > General > Test Runner`** を選択
2. **EditMode** タブを選択
3. **Run All** ボタンをクリック
4. すべてのテストが緑色（✅ Pass）になることを確認
5. **PlayMode** タブに切り替えて同様に実行

---

## トラブルシューティング 🔧

### ボタンをクリックしても何も起こらない

**原因**: ボタンが正しく接続されていない

**解決策**:
1. `Tools > Memory Manager > Verify UI Connections` で接続状況を確認
2. 接続されていないボタンがあれば、`Auto Connect UI Buttons` を再実行

### "MemoryManagerが見つかりません" と表示される

**原因**: MainSceneにMemoryManagerコンポーネントがない

**解決策**:
1. Hierarchyウィンドウで「MemoryManager」という名前のGameObjectを探す
2. なければ、空のGameObjectを作成して `MemoryManager` スクリプトをアタッチ
3. ステップ2を再実行

### 数値が表示されない

**原因**: TextMeshProUGUIフィールドが接続されていない

**解決策**:
1. Hierarchyで「MemoryManager」GameObjectを選択
2. Inspectorで `Memory Manager` コンポーネントを確認
3. すべてのフィールド（mTextSystemMemorySize など）に対応するTextオブジェクトをドラッグ&ドロップ

### コンパイルエラーが出る

**原因**: Unityのバージョンが古い、または必要なパッケージがない

**解決策**:
1. Unity 2022.3 以降を使用
2. `Window > Package Manager` から以下をインストール:
   - TextMesh Pro
   - Unity Test Framework
   - Collections

---

## 次のステップ 🎯

### Out of Memory (OOM) を発生させる

⚠️ **注意**: これはエディタをクラッシュさせる可能性があります。保存してから実行してください。

1. シーンを保存（Ctrl+S）
2. 再生モード開始
3. すべての **Allocate** ボタンを同時にクリック
4. メモリ使用量を観察
5. Out of Memoryエラーが発生するまで待機

### Development Build を作成

1. `File > Build Settings` を開く
2. **Development Build** にチェック
3. プラットフォームを選択（Windows/Mac/Android など）
4. **Build** をクリック
5. ビルドしたアプリケーションで実機テスト

---

## 詳細ドキュメント 📚

- **プロジェクト概要**: `README.md`
- **UI接続の詳細**: `Assets/Scenes/UI_SETUP.md`
- **テストの実行方法**: `Assets/Tests/README.md`
- **変更サマリー**: `CHANGES_SUMMARY.md`

---

## サポート 💬

問題が解決しない場合は、以下を確認してください：
- Unity Console にエラーメッセージがないか
- MainScene が正しく開かれているか
- すべてのファイルが最新の状態か（git pull）

---

**所要時間**: 合計 約5分
**難易度**: ⭐ 簡単（自動化ツール使用）
