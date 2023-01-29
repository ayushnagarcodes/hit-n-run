using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyBehavior : MonoBehaviour
{
    private Transform _player;
    private Rigidbody2D _rb;
    
    private float _enemySpeed = 5f;
    private Vector2 _totalEnemyVelocity;
    private Vector2 _playerOffset;
    private float _rotateSpeed = 200f;

    void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        _playerOffset = _player.position - transform.position;
        _totalEnemyVelocity = _playerOffset.normalized * _enemySpeed;
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
