using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Vector2 _currentPosition;
    private float _inputVertical;
    private float _inputHorizontal;
    private Vector2 _totalVelocity;
    private float _moveSpeed = 8f;
    private float _speedLimiter = 0.7f;

    private Camera _mainCamera;
    private Vector2 _mousePosition;
    private Vector2 _offset;
    private float _rotateAngle;
              
    [SerializeField] private Transform shotgun;
    [SerializeField] private GameObject bullet;
    private Vector2 _bulletVelocity;                     
    private float _bulletSpeed = 10f;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _mainCamera = Camera.main;
    }
    
    void Update()
    {
        _currentPosition = transform.position;
        
        // Movement details
        _inputVertical = Input.GetAxisRaw("Vertical");
        _inputHorizontal = Input.GetAxisRaw("Horizontal");

        // Rotation details
        _mousePosition = Input.mousePosition;
        // Converting mouse's screen point to world point because here, screen size != world size
        Vector2 worldPoint = _mainCamera.ScreenToWorldPoint(new Vector2(_mousePosition.x, _mousePosition.y));
        _offset = new Vector2(worldPoint.x - _currentPosition.x, worldPoint.y - _currentPosition.y).normalized;
        _rotateAngle = MathF.Atan2(_offset.y, _offset.x) * Mathf.Rad2Deg;
        
        RotatePlayer();
        Fire();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        _totalVelocity = new Vector2(_inputHorizontal, _inputVertical) * _moveSpeed;
        
        if (_inputVertical != 0 || _inputHorizontal != 0)
        {
            if (_inputVertical != 0 && _inputHorizontal != 0)
            {
                _totalVelocity *= _speedLimiter;
            }
            
            _rb.MovePosition(_rb.position + _totalVelocity * Time.fixedDeltaTime);
        }
        else
        {
            _rb.velocity = new Vector2(0, 0);
        }
    }

    void RotatePlayer()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, _rotateAngle - 90f);
    }

    void Fire()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = Instantiate(bullet, shotgun.position, Quaternion.identity);
            obj.GetComponent<Rigidbody2D>().velocity = _offset * _bulletSpeed;
        }
    }
}
