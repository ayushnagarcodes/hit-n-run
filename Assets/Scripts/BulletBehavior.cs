using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class BulletBehavior : MonoBehaviour
{
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Border"))
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            int temp2 = collision.gameObject.GetComponent<EnemyBehavior>().temp;
            Destroy(collision.gameObject);
            
            _gameManager.points += (300 + 50 * temp2);
            if (_gameManager.points > PlayerPrefs.GetInt("highScore"))
            {
                PlayerPrefs.SetInt("highScore", _gameManager.points);
            }
            
            Destroy(gameObject);
        }
    }
}
