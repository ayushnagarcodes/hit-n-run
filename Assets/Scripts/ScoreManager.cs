using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] Text score;
    public int gameLevel;
    public int points;
    private float _increaseTime = 5f;

    void Start()
    {
        gameLevel = 1;
        points = 0;
        score.text = points.ToString();

        StartCoroutine(IncreaseDifficulty());
    }
    
    void Update()
    {
        score.text = points.ToString();
    }
    
    IEnumerator IncreaseDifficulty()
    {
        yield return new WaitForSeconds(_increaseTime);
        gameLevel++;
        StartCoroutine(IncreaseDifficulty());
    }
}
