using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Text highScore;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("highScore"))
        {
            PlayerPrefs.SetInt("highScore", 0);
        }

        highScore.text = PlayerPrefs.GetInt("highScore").ToString();
    }

    private void Update()
    {
        if (PlayerPrefs.GetInt("highScore") > Int32.Parse(highScore.text))
        {
            highScore.text = PlayerPrefs.GetInt("highScore").ToString();
        }
    }
    
    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
