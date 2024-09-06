using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArchiveManager : MonoBehaviour
{
    public GameObject compoundButtonPrefab; // 化合物ボタンのプレハブ
    public Transform contentTransform; // Scroll ViewのContentオブジェクト
    public GameObject detailPanel; // 詳細表示用のパネル
    public TextMeshProUGUI detailText; // 詳細テキスト
    public Image compoundImage; // 化合物画像表示用

    private Dictionary<string, bool> compoundStatus = new Dictionary<string, bool>();
    private Dictionary<string, string> compoundDescriptions = new Dictionary<string, string>();
    private CompoundManager compoundManager; // CompoundManagerの参照

    void Start()
    {
        compoundManager = FindObjectOfType<CompoundManager>(); // CompoundManagerの取得

        if (compoundManager == null)
        {
            Debug.LogError("CompoundManagerが見つかりません。");
            return;
        }

        InitializeCompoundStatus();
        GenerateCompoundButtons();
        detailPanel.SetActive(false); // 詳細パネルを非表示に設定
    }

    private void InitializeCompoundStatus()
    {
        // CompoundManagerから化合物の生成状況を取得
        string[] compounds = { "Methane", "Water", "CarbonDioxide", "Methanol", "FormicAcid", "AceticAcid", "CarbonicAcid", "Bicarbonate" }; // 他の化合物名も追加する
        foreach (string compound in compounds)
        {
            compoundStatus[compound] = compoundManager.IsCompoundGenerated(compound); // CompoundManagerから生成状況を取得
            compoundDescriptions[compound] = GetCompoundDescription(compound); // 各化合物の説明を取得
        }
    }

    private void GenerateCompoundButtons()
    {
        foreach (var compound in compoundStatus)
        {
            GameObject buttonObj = Instantiate(compoundButtonPrefab, contentTransform);
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (compound.Value) // 生成済みの場合
            {
                buttonText.text = compound.Key; // 化合物名を表示
                buttonObj.GetComponent<Button>().onClick.AddListener(() => ShowCompoundDetails(compound.Key));
            }
            else // 未生成の場合
            {
                buttonText.text = "???"; // 未生成時の表示
                buttonObj.GetComponent<Button>().interactable = false; // ボタンを無効化
            }
        }
    }

    private void ShowCompoundDetails(string compoundName)
    {
        detailText.text = compoundDescriptions[compoundName]; // 詳細テキストを設定
        compoundImage.sprite = GetCompoundImage(compoundName); // 化合物画像を設定
        detailPanel.SetActive(true); // 詳細パネルを表示
    }

    private string GetCompoundDescription(string compoundName)
    {
        // 化合物の説明を返す。ここでは仮のデータ。
        switch (compoundName)
        {
            case "Methane":
                return "メタンは最も簡単なアルカンで、化学式CH₄の有機化合物です。";
            case "Water":
                return "水は化学式H₂Oで表される化合物であり、地球上で生命に不可欠です。";
            case "CarbonDioxide":
                return "二酸化炭素は化学式CO₂で表される無機化合物です。";
            case "Methanol":
                return "メタノールはCH₃OHの化学式を持つ簡単なアルコールです。";
            // 他の化合物についても追加可能
            default:
                return "説明はありません。";
        }
    }

    private Sprite GetCompoundImage(string compoundName)
    {
        // 化合物の画像を返す。ここでは仮の画像を使用。
        return Resources.Load<Sprite>($"Images/{compoundName}"); // 例: Resources/Images/Methane.png
    }

    public void ResetArchive()
    {
        compoundManager.ResetAllCompounds(); // CompoundManagerを使用してリセット
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject); // 古いボタンを削除
        }
        InitializeCompoundStatus();
        GenerateCompoundButtons(); // ボタンを再生成
    }
}
