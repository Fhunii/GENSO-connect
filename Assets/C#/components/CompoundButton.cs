using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class CompoundButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText; // ボタンのテキスト表示
    private Button button; // ボタンコンポーネント
    private string compoundName; // 化合物の名前
    private Action<string> onClickAction; // クリック時のアクションを外部から設定

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    public void Initialize(string name, Action<string> action)
    {
        compoundName = name;
        buttonText.text = name; // ボタンのテキストを設定
        onClickAction = action; // 外部からのアクションを設定
    }

    private void OnButtonClick()
    {
        onClickAction?.Invoke(compoundName); // クリック時に設定されたアクションを実行
    }
}
