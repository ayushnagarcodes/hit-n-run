using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject obstacle;
    
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject obj = Instantiate(obstacle,
                new Vector2(Random.Range(-12.5f, 12.5f), Random.Range(-12.5f, 12.5f)), Quaternion.identity);
            
            float randomScale = Random.Range(1.2f, 3.5f);
            obj.transform.localScale = new Vector3(randomScale, randomScale, obj.transform.localScale.z);
        }
    }
}
