using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class CompoundButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText; // ボタンのテキスト表示
    private Button button; // ボタンコンポーネント
    private Action<string> onClickAction; // クリック時のアクションを外部から設定

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    public void SetCompoundName(string name)
    {
        buttonText.text = name; // ボタンのテキストを設定
    }

    public void SetOnClickAction(Action<string> action)
    {
        onClickAction = action; // 外部からのアクションを設定
    }

    private void OnButtonClick()
    {
        if (onClickAction != null)
        {
            onClickAction.Invoke(buttonText.text); // クリック時に設定されたアクションを実行
        }
        else
        {
            Debug.Log("Compound Button Clicked: " + buttonText.text);
        }
    }
}
