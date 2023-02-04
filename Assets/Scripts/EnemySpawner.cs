using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    private Camera _mainCamera;
    private Vector2 _spawnPosition;
    private float _spawnTime = 2f;

    private void Start()
    {
        _mainCamera = Camera.main;

        StartCoroutine(Spawn());
    }

    void GenerateSpawnPos()
    {
        // Converting spawn position to viewport view
        _spawnPosition =
            _mainCamera.WorldToViewportPoint(new Vector2(Random.Range(-12.5f, 12.5f), Random.Range(-12.5f, 12.5f)));
    }

    IEnumerator Spawn()
    {
        GenerateSpawnPos();
        // If the generated position is in the camera view, then regenerating the position until it is out of the camera view
        while (_spawnPosition.x > 0 && _spawnPosition.x < 1 && _spawnPosition.y > 0 && _spawnPosition.y < 1)
        {
            GenerateSpawnPos();
        }
        
        // Converting spawn position back to world view
        GameObject prefab = Instantiate(enemy, _mainCamera.ViewportToWorldPoint(_spawnPosition), Quaternion.identity);

        yield return new WaitForSeconds(_spawnTime);
        
        StartCoroutine(Spawn());
    }
}