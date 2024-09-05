using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ToMenuButton : MonoBehaviour
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
        StartCoroutine(ToMenuClicked());

    }
    
    public IEnumerator ToMenuClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("menu scene");
        AudioManager.Instance.StopSettingsAudio();
        yield return null;
    }

}

