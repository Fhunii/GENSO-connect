using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArchiveManager : MonoBehaviour
{
    [SerializeField] private Transform content; // ScrollViewのContentオブジェクト
    [SerializeField] private GameObject compoundButtonPrefab; // 化合物ボタンのプレハブ
    [SerializeField] private GameObject detailPanel; // 詳細表示用パネル
    [SerializeField] private TextMeshProUGUI detailText; // 化合物の詳細テキスト
    [SerializeField] private Image compoundImage; // 化合物の拡大画像
    [SerializeField] private Button resetButton; // リセットボタン

    private Dictionary<string, bool> compoundStatus = new Dictionary<string, bool>();
    private List<string> compounds = new List<string> { "Methane", "Carbon Monoxide", "Carbon Dioxide", "Water", "Methanol", "Formaldehyde", "Formic Acid", "Acetic Acid", "Carbonic Acid", "Bicarbonate" };

    private void Start()
    {
        InitializeArchive();
        resetButton.onClick.AddListener(ResetArchive);
    }

    private void InitializeArchive()
    {
        // 生成した化合物の情報を読み込み
        LoadCompoundStatus();

        foreach (var compound in compounds)
        {
            GameObject buttonObj = Instantiate(compoundButtonPrefab, content);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (compoundStatus.ContainsKey(compound) && compoundStatus[compound])
            {
                buttonText.text = compound; // 化合物名を表示
                button.onClick.AddListener(() => ShowCompoundDetail(compound));
            }
            else
            {
                buttonText.text = "？？？"; // 未生成の場合は？？？を表示
                button.interactable = false;
            }
        }
    }

    private void ShowCompoundDetail(string compound)
    {
        detailPanel.SetActive(true);
        detailText.text = $"{compound} の詳細情報をここに表示します。";
        // TODO: compoundImageを設定する
    }

    private void LoadCompoundStatus()
    {
        foreach (var compound in compounds)
        {
            compoundStatus[compound] = PlayerPrefs.GetInt(compound, 0) == 1;
        }
    }

    private void ResetArchive()
    {
        foreach (var compound in compounds)
        {
            compoundStatus[compound] = false;
            PlayerPrefs.SetInt(compound, 0);
        }
        PlayerPrefs.Save();

        // 現在のシーンをリロードして、リセットした内容を反映
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
