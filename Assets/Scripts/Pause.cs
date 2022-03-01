using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject pauseObject;

    bool isGameStop;
    void Awake()
    {
        isGameStop = false;    
    }
    public void OnClick_Continue()
    {
        isGameStop = !isGameStop;
        pauseObject.SetActive(isGameStop);

        if(isGameStop)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
    public void OnClick_Retry()
    {
        OnClick_Continue();
        gameManager.GameRetry();
    }
    public void OnClick_Exit()
    {
        Application.Quit();
    }
}
