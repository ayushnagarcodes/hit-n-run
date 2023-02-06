using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyBehavior : MonoBehaviour
{
    private Transform _player;
    private Rigidbody2D _rb;
    private SpriteRenderer _enemyRenderer;
    private float _green;

    private float _enemySpeed = 5f;
    private Vector2 _totalEnemyVelocity;
    private Vector2 _playerOffset;
    private float _rotateSpeed = 200f;

    public int temp = 0;

    void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        _rb = GetComponent<Rigidbody2D>();
        _enemyRenderer = GetComponent<SpriteRenderer>();
        _green = _enemyRenderer.color.g;
        temp = GameObject.FindWithTag("GameController").GetComponent<GameManager>().gameLevel - 1;

        // Changing color
        _enemyRenderer.color = new Color(1, _green - 0.1f * temp, 0);

        // Changing speed
        _enemySpeed += 0.2f * temp;
    }

    void FixedUpdate()
    {
        _playerOffset = (_player.position - transform.position).normalized;
        _totalEnemyVelocity = _playerOffset * _enemySpeed;
        _rb.MovePosition(_rb.position + _totalEnemyVelocity * Time.fixedDeltaTime);
        
        _rb.MoveRotation(_rb.rotation + _rotateSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("Scenes/Game", LoadSceneMode.Single);
        }
    }
}
