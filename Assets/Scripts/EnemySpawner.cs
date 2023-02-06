using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemy;

    private Camera _mainCamera;
    private Vector3 _spawnPosition;
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
            _mainCamera.WorldToViewportPoint(new Vector3(Random.Range(-12.5f, 12.5f), Random.Range(-12.5f, 12.5f), 0f));
    }

    IEnumerator Spawn()
    {
        GenerateSpawnPos();
        // If the generated position is in the camera view, then regenerating the position until it is out of the camera view
        // Also, keeping in mind the scale of the enemy, so, increasing the check window
        while (_spawnPosition.x > -0.1f && _spawnPosition.x < 1.1f && _spawnPosition.y > -0.1f && _spawnPosition.y < 1.1f)
        {
            GenerateSpawnPos();
        }
        
        // Converting spawn position back to world view
        GameObject obj = Instantiate(enemy, _mainCamera.ViewportToWorldPoint(_spawnPosition), Quaternion.identity);

        yield return new WaitForSeconds(_spawnTime);
        
        StartCoroutine(Spawn());
    }
}