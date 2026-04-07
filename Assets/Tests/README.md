# MemoryManager Tests

このディレクトリには、MemoryManagerクラスのユニットテストが含まれています。

## テストの種類

### EditMode Tests (`Editor/`)
エディターモードで実行される単体テスト。主に静的メソッドや個別のロジックをテストします。

**テストファイル**: `MemoryManagerTests.cs`

**テスト内容**:
- `FormatBytes()` メソッドの各種ケース
  - ゼロバイト
  - KB, MB, GB, TB, PB単位の変換
  - 負の値の処理
  - 小数点の丸め処理

### PlayMode Tests (`Runtime/`)
プレイモードで実行されるテスト。実際のGameObjectやコンポーネントの動作をテストします。

**テストファイル**: `MemoryManagerPlayModeTests.cs`

**テスト内容**:
- MemoryManagerコンポーネントの存在確認
- Updateメソッドの動作確認
- 静的メソッドの動作確認

## テストの実行方法

### Unity Editor内で実行

1. Unityエディターを開く
2. **Window > General > Test Runner** を選択
3. **EditMode** タブまたは **PlayMode** タブを選択
4. **Run All** ボタンをクリックしてすべてのテストを実行

### 個別テストの実行

Test Runnerウィンドウでテストをクリックして選択し、**Run Selected** ボタンをクリックします。

### コマンドラインから実行

```bash
# EditModeテストの実行
Unity.exe -runTests -batchmode -projectPath "path/to/project" -testResults "path/to/results.xml" -testPlatform EditMode

# PlayModeテストの実行
Unity.exe -runTests -batchmode -projectPath "path/to/project" -testResults "path/to/results.xml" -testPlatform PlayMode
```

## テストの追加方法

新しいテストを追加する場合：

1. `Editor/` (EditMode) または `Runtime/` (PlayMode) に新しい `.cs` ファイルを作成
2. `Tests` 名前空間を使用
3. `[Test]` 属性（EditMode）または `[UnityTest]` 属性（PlayMode）を使用
4. NUnit形式でテストを記述

### EditModeテストの例

```csharp
[Test]
public void TestName_Condition_ExpectedResult()
{
    // Arrange
    var input = ...;

    // Act
    var result = ...;

    // Assert
    Assert.AreEqual(expected, result);
}
```

### PlayModeテストの例

```csharp
[UnityTest]
public IEnumerator TestName_Condition_ExpectedResult()
{
    // Arrange
    var gameObject = new GameObject();

    // Wait for initialization
    yield return null;

    // Act
    var result = ...;

    // Assert
    Assert.IsNotNull(result);

    // Cleanup
    Object.Destroy(gameObject);
}
```

## 参考資料

- [Unity Test Framework Documentation](https://docs.unity3d.com/Packages/com.unity.test-framework@latest)
- [NUnit Documentation](https://docs.nunit.org/)
