using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボタンのトグル状態を視覚的に表示するコンポーネント
/// </summary>
[RequireComponent(typeof(Button))]
public class MemoryToggleButton : MonoBehaviour
{
    [Header("状態による色")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color activeColor = new Color(0.3f, 1f, 0.3f, 1f); // 明るい緑

    [Header("オプション: テキスト変更")]
    [SerializeField] private bool changeText = false;
    [SerializeField] private string normalText = "Start";
    [SerializeField] private string activeText = "Stop";

    private Button button;
    private Image buttonImage;
    private TMPro.TextMeshProUGUI buttonText;
    private bool isActive = false;

    void Awake()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        // 子オブジェクトからTextを取得
        buttonText = GetComponentInChildren<TMPro.TextMeshProUGUI>();

        // 初期状態を設定
        UpdateVisuals();
    }

    /// <summary>
    /// ボタンの状態をトグル
    /// </summary>
    public void Toggle()
    {
        isActive = !isActive;
        UpdateVisuals();
    }

    /// <summary>
    /// 状態を直接設定
    /// </summary>
    public void SetActive(bool active)
    {
        isActive = active;
        UpdateVisuals();
    }

    /// <summary>
    /// 現在の状態を取得
    /// </summary>
    public bool IsActive()
    {
        return isActive;
    }

    /// <summary>
    /// ビジュアルを更新
    /// </summary>
    private void UpdateVisuals()
    {
        // ボタンの色を変更
        if (buttonImage != null)
        {
            buttonImage.color = isActive ? activeColor : normalColor;
        }

        // テキストを変更（オプション）
        if (changeText && buttonText != null)
        {
            buttonText.text = isActive ? activeText : normalText;
        }
    }

    /// <summary>
    /// リセット（非アクティブ状態に戻す）
    /// </summary>
    public void Reset()
    {
        isActive = false;
        UpdateVisuals();
    }
}
