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
    private Vector3[] _spawnPositions;
    private float _spawnTime = 2f;

    private void Start()
    {
        _mainCamera = Camera.main;

        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        _spawnPositions = new Vector3[] {
            new Vector3(Random.Range(-0.3f, -0.1f), Random.Range(-0.1f, 1.1f), _mainCamera.nearClipPlane),
            new Vector3(Random.Range(1.1f, 1.3f), Random.Range(-0.1f, 1.1f), _mainCamera.nearClipPlane),
            new Vector3(Random.Range(-0.1f, 1.1f), Random.Range(-0.3f, -0.1f), _mainCamera.nearClipPlane),
            new Vector3(Random.Range(-0.1f, 1.1f), Random.Range(1.1f, 1.3f), _mainCamera.nearClipPlane)
        };
        
        GameObject prefab = Instantiate(enemy, _mainCamera.ViewportToWorldPoint(_spawnPositions[Random.Range(0, _spawnPositions.Length)]),
            Quaternion.identity);

        yield return new WaitForSeconds(_spawnTime);
        
        StartCoroutine(Spawn());
    }
}