using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClickStartButton()
    {
        Debug.Log("Start");
        SceneManager.LoadScene("GameScene");
    }

    public void OnclickModeButton()
    {
        Debug.Log("Mode");
    }
}
