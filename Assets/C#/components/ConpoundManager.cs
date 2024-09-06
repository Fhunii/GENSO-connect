using UnityEngine;

public class CompoundManager : MonoBehaviour
{
    // 化合物の名前リスト
    private string[] compoundNames = { "Methane", "CarbonMonoxide", "CarbonDioxide", "Water", "Methanol", "Formaldehyde", "FormicAcid", "AceticAcid", "CarbonicAcid", "Bicarbonate" };

    // 化合物が生成されたことを保存するメソッド
    public void SaveCompoundGenerated(string compoundName)
    {
        PlayerPrefs.SetInt(compoundName, 1);  // 1は生成済みを表す
        PlayerPrefs.Save(); // PlayerPrefsの変更を保存
    }

    // 化合物が生成されたかどうかを確認するメソッド
    public bool IsCompoundGenerated(string compoundName)
    {
        return PlayerPrefs.GetInt(compoundName, 0) == 1;  // 0は未生成を表す
    }

    // すべての生成状況をリセットするメソッド
    public void ResetAllCompounds()
    {
        foreach (string compoundName in compoundNames)
        {
            PlayerPrefs.SetInt(compoundName, 0);  // 0でリセット
        }
        PlayerPrefs.Save();  // PlayerPrefsの変更を保存
    }
}
