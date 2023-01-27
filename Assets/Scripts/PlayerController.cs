using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 _currentPosition;
    private float _inputVertical;
    private float _inputHorizontal;
    private float _moveSpeed = 5f;
    private float _speedLimiter = 0.7f;
    private float _totalSpeedY;
    private float _totalSpeedX;

    private Camera _mainCamera;
    private Vector2 _mousePosition;
    private Vector2 _offset;
    private float _rotateAngle;

    void Start()
    {
        _mainCamera = Camera.main;
    }
    
    void Update()
    {
        _currentPosition = transform.position;
        
        MovePlayer();
        RotatePlayer();
    }

    void MovePlayer()
    {
        _inputVertical = Input.GetAxisRaw("Vertical");
        _inputHorizontal = Input.GetAxisRaw("Horizontal");
        _totalSpeedY = _inputVertical * _moveSpeed * Time.deltaTime;
        _totalSpeedX = _inputHorizontal * _moveSpeed * Time.deltaTime;
        
        // Reducing the excess diagonal speed
        if (_inputVertical != 0 && _inputHorizontal != 0)
        {
            _currentPosition = new Vector2(_currentPosition.x + _totalSpeedX * _speedLimiter, _currentPosition.y + _totalSpeedY * _speedLimiter);
        }
        else if (_inputVertical != 0)
        {
            _currentPosition = new Vector2(_currentPosition.x, _currentPosition.y + _totalSpeedY);
        }
        else if (_inputHorizontal != 0)
        {
            _currentPosition = new Vector2(_currentPosition.x + _totalSpeedX, _currentPosition.y);
        }

        transform.position = _currentPosition;
    }

    void RotatePlayer()
    {
        _mousePosition = Input.mousePosition;
        
        // Converting mouse's screen point to world point because here, screen size != world size
        Vector3 worldPoint = _mainCamera.ScreenToWorldPoint(new Vector2(_mousePosition.x, _mousePosition.y));
        _offset = new Vector2(worldPoint.x - _currentPosition.x, worldPoint.y - _currentPosition.y).normalized;
        _rotateAngle = MathF.Atan2(_offset.y, _offset.x) * Mathf.Rad2Deg;
        
        transform.rotation = Quaternion.Euler(0f, 0f, _rotateAngle - 90f);
    }
}
