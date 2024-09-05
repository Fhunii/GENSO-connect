using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class settingsbutton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnClick()
    {
      //  StartCoroutine(SettingsClicked());

    }
    // ボタンが押された場合、今回呼び出される関数
    public IEnumerator SettingsClicked()
    {


        Transform settingsTransform = this.transform;
        GameObject archiveTransform = GameObject.Find("archive_button");
        GameObject startTransform = GameObject.Find("start_button");
        GameObject rogoTransform = GameObject.Find("rogo");

        // Declare and initialize the 'worldPos' variable
        // Vector3 worldPos1 = myTransform1.position;
        // ボタンの位置を移動

        for (int i = 0; i < 30; i++)
        {

            startTransform.transform.position += new Vector3(10, 0, 0);
            archiveTransform.transform.position += new Vector3(-10, 0, 0);
            settingsTransform.transform.position += new Vector3(0, -5, 0);
            rogoTransform.transform.position += new Vector3(0, -0.08f, 0);

            yield return new WaitForSeconds(0.01f);

            Debug.Log("Button Clicked!");
        }

        for (int i = 0; i < 30; i++)
        {

            rogoTransform.transform.localScale += new Vector3(2.5f, 2.5f, 2.5f);

            yield return new WaitForSeconds(0.01f);

            Debug.Log("Button Clicked!");
        }
        yield return new WaitForSeconds(0.1f);
        // シーン遷移
        UnityEngine.SceneManagement.SceneManager.LoadScene("settings scene");

    }
}
