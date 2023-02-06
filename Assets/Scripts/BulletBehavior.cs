using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class BulletBehavior : MonoBehaviour
{
    private ScoreManager _scoreManager;

    private void Start()
    {
        _scoreManager = GameObject.FindWithTag("GameController").GetComponent<ScoreManager>();
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
            _scoreManager.points += (300 + 50 * temp2);
            Destroy(gameObject);
        }
    }
}
