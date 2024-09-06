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

    void Start()
    {
        InitializeCompoundStatus();
        GenerateCompoundButtons();
        detailPanel.SetActive(false); // 詳細パネルを非表示に設定
    }

    private void InitializeCompoundStatus()
    {
        // 全ての化合物をリストに追加
        string[] compounds = { 
            "Methane", "Water", "CarbonDioxide", "CarbonMonoxide", "Methanol", 
            "Formaldehyde", "FormicAcid", "AceticAcid", "CarbonicAcid", "Bicarbonate" 
        };

        foreach (string compound in compounds)
        {
            compoundStatus[compound] = PlayerPrefs.GetInt(compound, 0) == 1;
            compoundDescriptions[compound] = GetCompoundDescription(compound); // 各化合物の説明を取得
        }
    }

    private void GenerateCompoundButtons()
    {
        foreach (var compound in compoundStatus)
        {
            GameObject buttonObj = Instantiate(compoundButtonPrefab, contentTransform);
            CompoundButton compoundButton = buttonObj.GetComponent<CompoundButton>();

            if (compound.Value) // 生成済みの場合
            {
                compoundButton.Initialize(compound.Key, ShowCompoundDetails); // 化合物名とアクションを設定
            }
            else // 未生成の場合
            {
                compoundButton.Initialize("???", null); // 未生成時の表示とアクションなし
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
        // 各化合物の説明を返す
        switch (compoundName)
        {
            case "Methane":
                return "メタンは最も簡単なアルカンで、化学式CH₄の有機化合物です。";
            case "Water":
                return "水は化学式H₂Oで表される化合物であり、地球上で生命に不可欠です。";
            case "CarbonDioxide":
                return "二酸化炭素は化学式CO₂で表される無機化合物です。";
            case "CarbonMonoxide":
                return "一酸化炭素は化学式COで表される有毒なガスです。";
            case "Methanol":
                return "メタノールは化学式CH₃OHの有機化合物で、毒性のあるアルコールです。";
            case "Formaldehyde":
                return "ホルムアルデヒドは化学式H₂COで、消毒液や防腐剤として使用される化学物質です。";
            case "FormicAcid":
                return "ギ酸は化学式HCOOHで、蟻酸としても知られ、虫や植物の防御化合物として自然界に存在します。";
            case "AceticAcid":
                return "酢酸は化学式CH₃COOHで、食酢の主成分です。";
            case "CarbonicAcid":
                return "炭酸は化学式H₂CO₃で、水と二酸化炭素が反応して生成される弱酸です。";
            case "Bicarbonate":
                return "炭酸水素イオン（HCO₃⁻）は、酸と塩基のバッファーシステムで重要な役割を果たします。";
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
        PlayerPrefs.DeleteAll(); // すべての生成状況をリセット
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject); // 古いボタンを削除
        }
        InitializeCompoundStatus();
        GenerateCompoundButtons(); // ボタンを再生成
    }

    // スプライトの生成状態をアーカイブに追加
    public void ArchivePiece(string compoundName, bool isGenerated)
    {
        compoundStatus[compoundName] = isGenerated;
        PlayerPrefs.SetInt(compoundName, isGenerated ? 1 : 0);
        PlayerPrefs.Save();
    }

    // スプライトが生成済みかどうかを確認
    public bool IsPieceGenerated(string compoundName)
    {
        return compoundStatus.ContainsKey(compoundName) && compoundStatus[compoundName];
    }
}
