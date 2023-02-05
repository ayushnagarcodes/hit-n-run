using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Obstacle") || collider.gameObject.CompareTag("Border"))
        {
            Destroy(gameObject);
        } 
        else if (collider.gameObject.CompareTag("Enemy"))
        {
            Destroy(collider.gameObject);
            Destroy(gameObject);
        }
    }
}
